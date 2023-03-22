using System;
using System.Drawing;
using System.IO;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
#pragma warning disable IDE0059

namespace GsaGH.Components {
  /// <summary>
  /// Component to open an existing GSA model
  /// </summary>
  public class SaveModel : GH_OasysDropDownComponent {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("e9989dce-717e-47ea-992c-e22d718e9ebb");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Properties.Resources.SaveModel;

    public SaveModel() : base("Save GSA Model",
      "Save",
      "Saves your GSA model from this parametric nightmare",
      CategoryName.Name(),
      SubCategoryName.Cat0()) {
      Hidden = true;
    }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaModelParameter(), "GSA Model", "GSA", "GSA model to save", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Save?", "Save", "Input 'True' to save or use button", GH_ParamAccess.item, true);
      pManager.AddTextParameter("File and Path", "File", "Filename and path", GH_ParamAccess.item);
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaModelParameter());
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess da) {
      var ghTyp = new GH_ObjectWrapper();
      Message = "";
      if (!da.GetData(0, ref ghTyp))
        return;

      if (ghTyp == null)
        return;

      var gsaModel = new GsaModel();
      if (ghTyp.Value is GsaModelGoo) {
        ghTyp.CastTo(ref gsaModel);
        Message = "";
      }
      else {
        this.AddRuntimeError("Error converting input to GSA Model");
        return;
      }

      string fileName = "";
      da.GetData(2, ref fileName);

      bool save = false;
      if (da.GetData(1, ref save) && save)
        Save(ref gsaModel, fileName);

      da.SetData(0, new GsaModelGoo(gsaModel));
    }

    internal void Save(ref GsaModel model, string fileNameAndPath) {
      if (!fileNameAndPath.EndsWith(".gwb"))
        fileNameAndPath += ".gwb";

      Directory.CreateDirectory(Path.GetDirectoryName(fileNameAndPath) ?? string.Empty);

      string mes = model.Model.SaveAs(fileNameAndPath).ToString();
      if (mes == GsaAPI.ReturnValue.GS_OK.ToString()) {
        _fileNameLastSaved = fileNameAndPath;
        OasysGH.Helpers.PostHog.ModelIO(GsaGH.PluginInfo.Instance, "saveGWB", (int)(new FileInfo(fileNameAndPath).Length / 1024));
        model.FileNameAndPath = fileNameAndPath;
      }
      else
        this.AddRuntimeError(mes);
    }

    #region Custom UI
    private string _fileNameLastSaved;
    public override void SetSelected(int i, int j) { }
    public override void InitialiseDropdowns() { }
    public override void CreateAttributes() {
      m_attributes = new OasysGH.UI.ThreeButtonAtrributes(this, "Save", "Save As", "Open in GSA", SaveButtonClick, SaveAsButtonClick, OpenGsaExe, true, "Save GSA file");
    }

    internal void SaveButtonClick() {
      UpdateUI();
    }

    internal void SaveAsButtonClick() {
      var fdi = new Rhino.UI.SaveFileDialog { Filter = "GSA File (*.gwb)|*.gwb|All files (*.*)|*.*" };
      bool res = fdi.ShowSaveDialog();
      if (!res)
        return;
      while (Params.Input[2].Sources.Count > 0)
        Grasshopper.Instances.ActiveCanvas.Document.RemoveObject(Params.Input[2].Sources[0], false);

      var panel = new Grasshopper.Kernel.Special.GH_Panel();
      panel.CreateAttributes();

      panel.Attributes.Pivot = new PointF(Attributes.DocObject.Attributes.Bounds.Left -
                                          panel.Attributes.Bounds.Width - 40, Attributes.DocObject.Attributes.Bounds.Bottom - panel.Attributes.Bounds.Height);
      panel.UserText = fdi.FileName;
      Grasshopper.Instances.ActiveCanvas.Document.AddObject(panel, false);

      Params.Input[2].AddSource(panel);
      Params.OnParametersChanged();
      ExpireSolution(true);
    }

    internal void OpenGsaExe() {
      if (!string.IsNullOrEmpty(_fileNameLastSaved))
        System.Diagnostics.Process.Start(_fileNameLastSaved);
    }

    public override bool Read(GH_IO.Serialization.GH_IReader reader) {
      bool flag = base.Read(reader);
      var saveInput = (Param_Boolean)Params.Input[1];
      if (saveInput.PersistentData.First().Value)
        return flag;

      saveInput.PersistentData.Clear();
      saveInput.PersistentData.Append(new GH_Boolean(true));
      return flag;
    }
    #endregion
  }
}
