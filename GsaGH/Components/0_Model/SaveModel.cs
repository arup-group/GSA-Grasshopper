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
using System.Linq;
using Grasshopper.Kernel.Parameters;
using GH_IO.Serialization;
using GsaGH.Components.GraveyardComp;
using OasysGH.Units;

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
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddParameter(new GsaModelParameter(), "GSA Model", "GSA", "GSA model to save", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Save?", "Save", "Input 'True' to save or use button", GH_ParamAccess.item, true);
      pManager.AddTextParameter("File and Path", "File", "Filename and path", GH_ParamAccess.item);
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
      this.Message = "";
      if (DA.GetData(0, ref gh_typ))
      {
        if (gh_typ == null) { return; }
        if (gh_typ.Value is GsaModelGoo)
        {
          gh_typ.CastTo(ref gsaModel);
          Message = "";
        }
        else
        {
          AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error converting input to GSA Model");
          return;
        }

        string fileName = "";
        DA.GetData(2, ref fileName);

        bool save = false;
        if (DA.GetData(1, ref save) && save)
          this.Save(ref gsaModel, fileName);

        DA.SetData(0, new GsaModelGoo(gsaModel));
      }
    }

    internal void Save(ref GsaModel model, string fileNameAndPath)
    {
      if (!fileNameAndPath.EndsWith(".gwb"))
        fileNameAndPath += ".gwb";

      Directory.CreateDirectory(Path.GetDirectoryName(fileNameAndPath));

      string mes = model.Model.SaveAs(fileNameAndPath).ToString();
      if (mes == GsaAPI.ReturnValue.GS_OK.ToString())
      {
        _fileNameLastSaved = fileNameAndPath;
        PostHog.ModelIO(GsaGH.PluginInfo.Instance, "saveGWB", (int)(new FileInfo(fileNameAndPath).Length / 1024));
        model.FileNameAndPath = fileNameAndPath;
      }
      else
        this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, mes);
    }

    #region Custom UI
    string _fileNameLastSaved;
    public override void SetSelected(int i, int j) { }
    public override void InitialiseDropdowns() { }
    public override void CreateAttributes()
    {
      m_attributes = new OasysGH.UI.ThreeButtonAtrributes(this, "Save", "Save As", "Open in GSA", SaveButtonClick, SaveAsButtonClick, OpenGSAexe, true, "Save GSA file");
    }

    internal void SaveButtonClick()
    {
      // trigger rerunning the component
      UpdateUI();
    }

    internal void SaveAsButtonClick()
    {
      var fdi = new Rhino.UI.SaveFileDialog { Filter = "GSA File (*.gwb)|*.gwb|All files (*.*)|*.*" };
      var res = fdi.ShowSaveDialog();
      if (res) // == DialogResult.OK)
      {
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
        panel.UserText = fdi.FileName;

        //Until now, the panel is a hypothetical object.
        // This command makes it 'real' and adds it to the canvas.
        Grasshopper.Instances.ActiveCanvas.Document.AddObject(panel, false);

        //Connect the new slider to this component
        Params.Input[2].AddSource(panel);
        Params.OnParametersChanged();
        ExpireSolution(true);
      }
    }

    internal void OpenGSAexe()
    {
      if (_fileNameLastSaved != null && _fileNameLastSaved != "")
        System.Diagnostics.Process.Start(_fileNameLastSaved);
    }

    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      bool flag = base.Read(reader);
      Param_Boolean saveInput = (Param_Boolean)this.Params.Input[1];
      if (saveInput.PersistentData.First().Value == false)
      {
        saveInput.PersistentData.Clear();
        saveInput.PersistentData.Append(new GH_Boolean(true));
      }
      return flag;
    }
    #endregion
  }
}
