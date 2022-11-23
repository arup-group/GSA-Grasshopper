using System;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Parameters;
using GsaGH.Helpers;
using System.IO;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using GsaGH.Helpers.GH;

namespace GsaGH.Components
{
    /// <summary>
    /// Component to open an existing GSA model
    /// </summary>
    public class SaveModel : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("e9989dce-717e-47ea-992c-e22d718e9ebb");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => GsaGH.Properties.Resources.SaveModel;

    public SaveModel() : base("Save GSA Model",
      "Save",
      "Saves your GSA model from this parametric nightmare",
      CategoryName.Name(),
      SubCategoryName.Cat0())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    string FileName = null;
    bool usersetFileName = false;
    Model gsaSaveModel;
    bool canOpen = false;
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddParameter(new GsaModelParameter(), "GSA Model", "GSA", "GSA model to save", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Save?", "Save", "Input 'True' to save or use button", GH_ParamAccess.item, false);
      pManager.AddTextParameter("File and Path", "File", "Filename and path", GH_ParamAccess.item);
      pManager[1].Optional = true;
      pManager[2].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new GsaModelParameter());
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      Model model = new Model();
      GsaModel gsaModel = new GsaModel();
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(0, ref gh_typ))
      {
        if (gh_typ == null) { return; }
        if (gh_typ.Value is GsaModelGoo)
        {
          gh_typ.CastTo(ref gsaModel);
          gsaSaveModel = gsaModel.Model;
          Message = "";
        }
        else
        {
          AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error converting input to GSA Model");
          return;
        }

        if (!usersetFileName)
        {
          if (gsaModel.FileName != "")
            FileName = gsaModel.FileName;
        }

        string tempfile = "";
        if (DA.GetData(2, ref tempfile))
          FileName = tempfile;

        bool save = false;
        if (DA.GetData(1, ref save))
        {
          if (save)
          {
            Message = gsaSaveModel.SaveAs(FileName).ToString();
            PostHog.ModelIO(GsaGH.PluginInfo.Instance, "saveGWB", (int)(new FileInfo(FileName).Length / 1024));
          }
        }

        DA.SetData(0, new GsaModelGoo(gsaModel));
      }
    }
    #region Custom UI
    public override void SetSelected(int i, int j) { }
    public override void InitialiseDropdowns() { }
    public override void CreateAttributes()
    {
      m_attributes = new OasysGH.UI.ThreeButtonAtrributes(this, "Save", "Save As", "Open in GSA", SaveFile, SaveAsFile, OpenGSAexe, true, "Save GSA file");
    }

    public override void VariableParameterMaintenance()
    {
      Params.Input[0].Optional = FileName != null; //filename can have input from user input
      Params.Input[0].ClearRuntimeMessages(); // this needs to be called to avoid having a runtime warning message after changed to optional
    }

    public void SaveFile()
    {
      if (FileName == null | FileName == "")
        SaveAsFile();
      else
      {
        string mes = gsaSaveModel.SaveAs(FileName).ToString();
        if (mes == GsaAPI.ReturnValue.GS_OK.ToString())
        {
          canOpen = true;
          //this.OnAttributesChanged();
          //CreateAttributes();
          mes = "Saved file";
          //ExpireSolution(true);
        }
        else
        {
          mes = Char.ToUpper(mes[3]) + mes.Substring(4).ToLower().Replace("_", " ");
        }
        this.Message = mes;
      }
    }

    public void SaveAsFile()
    {
      var fdi = new Rhino.UI.SaveFileDialog { Filter = "GSA File (*.gwb)|*.gwb|All files (*.*)|*.*" };
      var res = fdi.ShowSaveDialog();
      if (res) // == DialogResult.OK)
      {
        FileName = fdi.FileName;
        usersetFileName = true;
        string mes = gsaSaveModel.SaveAs(FileName).ToString();
        if (mes == GsaAPI.ReturnValue.GS_OK.ToString())
        {
          canOpen = true;
          //CreateAttributes();
          mes = "Saved file";

          PostHog.ModelIO(GsaGH.PluginInfo.Instance, "saveGWB", (int)(new FileInfo(FileName).Length / 1024));

          //add panel input with string
          //delete existing inputs if any
          while (Params.Input[2].Sources.Count > 0)
            Grasshopper.Instances.ActiveCanvas.Document.RemoveObject(Params.Input[2].Sources[0], false);

          //instantiate  new panel
          var panel = new Grasshopper.Kernel.Special.GH_Panel();
          panel.CreateAttributes();

          panel.Attributes.Pivot = new PointF((float)Attributes.DocObject.Attributes.Bounds.Left -
              panel.Attributes.Bounds.Width - 40, (float)Attributes.DocObject.Attributes.Bounds.Bottom - panel.Attributes.Bounds.Height);

          //populate value list with our own data
          panel.UserText = FileName;

          //Until now, the panel is a hypothetical object.
          // This command makes it 'real' and adds it to the canvas.
          Grasshopper.Instances.ActiveCanvas.Document.AddObject(panel, false);

          //Connect the new slider to this component
          Params.Input[2].AddSource(panel);
          Params.OnParametersChanged();
          ExpireSolution(true);
        }
        else
        {
          mes = Char.ToUpper(mes[3]) + mes.Substring(4).ToLower().Replace("_", " ");
        }
        this.Message = mes;
      }
    }

    public void OpenGSAexe()
    {
      if (FileName != null)
      {
        if (FileName != "")
        {
          if (canOpen)
            System.Diagnostics.Process.Start(FileName);
        }
      }
    }
    #endregion

    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      writer.SetString("File", this.FileName);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      this.FileName = reader.GetString("File");
      return base.Read(reader);
    }
    #endregion
  }
}

