using System;
using System.IO;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Parameters;
using GsaGH.Helpers;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to open an existing GSA model
  /// </summary>
  public class OpenModel : GH_OasysComponent, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("10bb2aac-504e-4054-9708-5053fbca61fc");
    public OpenModel()
      : base("Open Model", "Open", "Open an existing GSA model",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat0())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.primary;

    protected override Bitmap Icon => GsaGH.Properties.Resources.OpenModel;
    #endregion

    #region Custom UI
    //This region overrides the typical component layout
    public override void CreateAttributes()
    {
      m_attributes = new UI.ButtonComponentUI(this, "Open", OpenFile, "Open GSA file");
    }
    public void OpenFile()
    {
      var fdi = new Rhino.UI.OpenFileDialog { Filter = "GSA Files(*.gwb)|*.gwb|All files (*.*)|*.*" }; //"GSA Files(*.gwa; *.gwb)|*.gwa;*.gwb|All files (*.*)|*.*"
      var res = fdi.ShowOpenDialog();
      if (res) // == DialogResult.OK)
      {
        fileName = fdi.FileName;

        // instantiate  new panel
        var panel = new Grasshopper.Kernel.Special.GH_Panel();
        panel.CreateAttributes();

        // set the location relative to the open component on the canvas
        panel.Attributes.Pivot = new PointF((float)Attributes.DocObject.Attributes.Bounds.Left -
            panel.Attributes.Bounds.Width - 30, (float)Params.Input[0].Attributes.Pivot.Y - panel.Attributes.Bounds.Height / 2);

        // check for existing input
        while (Params.Input[0].Sources.Count > 0)
        {
          var input = Params.Input[0].Sources[0];
          // check if input is the one we automatically create below
          if (Params.Input[0].Sources[0].InstanceGuid == panelGUID)
          {
            // update the UserText in existing panel
            //RecordUndoEvent("Changed OpenGSA Component input");
            panel = input as Grasshopper.Kernel.Special.GH_Panel;
            panel.UserText = fileName;
            panel.ExpireSolution(true); // update the display of the panel
          }

          // remove input
          Params.Input[0].RemoveSource(input);
        }

        //populate panel with our own content
        panel.UserText = fileName;

        // record the panel's GUID if new, so that we can update it on change
        panelGUID = panel.InstanceGuid;

        //Until now, the panel is a hypothetical object.
        // This command makes it 'real' and adds it to the canvas.
        Grasshopper.Instances.ActiveCanvas.Document.AddObject(panel, false);

        //Connect the new slider to this component
        Params.Input[0].AddSource(panel);

        (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
        Params.OnParametersChanged();

        ExpireSolution(true);
      }
    }
    #endregion

    #region Input and output
    // This region handles input and output parameters

    string fileName = null;
    Guid panelGUID = Guid.NewGuid();
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter("Filename and path", "File", "GSA model to open and work with." +
              System.Environment.NewLine + "Input either path component, a text string with path and " +
              System.Environment.NewLine + "filename or an existing GSA model created in Grasshopper.", GH_ParamAccess.item);
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("GSA Model", "GSA", "GSA Model", GH_ParamAccess.item);
    }
    #region IGH_VariableParameterComponent null implementation
    //This sub region handles any changes to the component after it has been placed on the canvas
    bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index)
    {
      return false;
    }
    bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index)
    {
      return false;
    }
    IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index)
    {
      return null;
    }
    bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index)
    {
      return false;
    }
    void IGH_VariableParameterComponent.VariableParameterMaintenance()
    {
      Params.Input[0].Optional = fileName != null; //filename can have input from user input
      Params.Input[0].ClearRuntimeMessages(); // this needs to be called to avoid having a runtime warning message after changed to optional

      //    Params.Output[i].NickName = "P";
      //    Params.Output[i].Name = "Points";
      //    Params.Output[i].Description = "Points imported from GSA";
      //    Params.Output[i].Access = GH_ParamAccess.list;

    }
    #endregion
    #endregion

    #region (de)serialization
    //This region handles serialisation and deserialisation, meaning that 
    // component states will be remembered when reopening GH script
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      //writer.SetInt32("Mode", (int)_mode);
      writer.SetString("File", (string)fileName);
      //writer.SetBoolean("Advanced", (bool)advanced);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      //_mode = (FoldMode)reader.GetInt32("Mode");
      fileName = (string)reader.GetString("File");
      //advanced = (bool)reader.GetBoolean("Advanced");
      return base.Read(reader);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      Model model = new Model();

      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(0, ref gh_typ))
      {
        if (gh_typ.Value is GH_String)
        {
          string tempfile = "";
          if (GH_Convert.ToString(gh_typ, out tempfile, GH_Conversion.Both))
            fileName = tempfile;

          if (!fileName.EndsWith(".gwb"))
            fileName = fileName + ".gwb";

          ReturnValue status = model.Open(fileName);

          if (status == 0)
          {
            GsaModel gsaModel = new GsaModel
            {
              Model = model,
              FileName = fileName
            };

            Titles.GetTitlesFromGSA(model);

            string mes = Path.GetFileName(fileName);
            mes = mes.Substring(0, mes.Length - 4);
            Message = mes;
            DA.SetData(0, new GsaModelGoo(gsaModel));

            PostHog.ModelIO("openGWB", (int)(new FileInfo(fileName).Length / 1024));

            return;
          }
          else
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to open Model" + System.Environment.NewLine + status.ToString());
            return;
          }
        }
        else if (gh_typ.Value is Model)
        {
          gh_typ.CastTo(ref model);
          GsaModel gsaModel = new GsaModel
          {
            Model = model,
          };

          DA.SetData(0, new GsaModelGoo(gsaModel));
          return;
        }
        else
        {
          AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to open Model");
          return;
        }
      }
      else
      {
        ReturnValue status = model.Open(fileName);

        if (status == 0)
        {
          GsaModel gsaModel = new GsaModel
          {
            Model = model,
            FileName = fileName
          };

          Titles.GetTitlesFromGSA(model);

          string mes = Path.GetFileName(fileName);
          mes = mes.Substring(0, mes.Length - 4);
          Message = mes;
          DA.SetData(0, new GsaModelGoo(gsaModel));
          return;
        }
        else
        {
          AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to open Model" + System.Environment.NewLine + status.ToString());
          return;
        }
      }
    }
  }
}

