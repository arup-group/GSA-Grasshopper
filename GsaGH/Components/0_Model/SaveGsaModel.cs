using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;

using GH_IO.Serialization;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Types;

using GsaAPI;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.UI;

using Rhino.UI;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to save a GSA model
  /// </summary>
  public class SaveGsaModel : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("e9989dce-717e-47ea-992c-e22d718e9ebb");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.SaveGsaModel;
    private string _fileNameLastSaved;
    private bool _saveInputOverride = false;

    public SaveGsaModel() : base("Save GSA Model", "Save",
      "Saves your GSA model from this parametric nightmare", CategoryName.Name(),
      SubCategoryName.Cat0()) {
      Hidden = true;
    }

    public override void CreateAttributes() {
      if (!_isInitialised) {
        InitialiseDropdowns();
      }
      m_attributes = new ThreeButtonComponentAttributes(this, "Save", "Save As", "Open in GSA",
        SaveButtonClick, SaveAsButtonClick, OpenGsaExe, true, "Save GSA file");
    }

    public override void SetSelected(int i, int j) { }

    public override bool Read(GH_IReader reader) {
      bool flag = base.Read(reader);
      var saveInput = (Param_Boolean)Params.Input[1];
      if (saveInput.PersistentData.First().Value) {
        return flag;
      }

      saveInput.PersistentData.Clear();
      saveInput.PersistentData.Append(new GH_Boolean(true));
      return flag;
    }

    protected override void InitialiseDropdowns() {
      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaModelParameter(), "GSA Model", "GSA", "GSA model to save",
        GH_ParamAccess.item);
      pManager.AddBooleanParameter("Save?", "Save", "Input 'True' to save or use button",
        GH_ParamAccess.item, true);
      pManager.AddTextParameter("File and Path", "File", "Filename and path", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaModelParameter());
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      var ghTyp = new GH_ObjectWrapper();
      Message = string.Empty;
      if (!da.GetData(0, ref ghTyp)) {
        return;
      }

      if (ghTyp == null) {
        return;
      }

      var gsaModel = new GsaModel();
      if (ghTyp.Value is GsaModelGoo modelGoo) {
        gsaModel = modelGoo.Value;
        Message = string.Empty;
      } else {
        this.AddRuntimeError("Error converting input to GSA Model");
        return;
      }

      string fileName = string.Empty;
      da.GetData(2, ref fileName);

      bool save = false;
      if (da.GetData(1, ref save) && (save || _saveInputOverride)) {
        Save(ref gsaModel, fileName);
        _saveInputOverride = false;
      }

      da.SetData(0, new GsaModelGoo(gsaModel));
    }

    internal void Save(ref GsaModel model, string fileNameAndPath) {
      if (!fileNameAndPath.EndsWith(".gwa") && !fileNameAndPath.EndsWith(".gwb")) {
        fileNameAndPath += ".gwb";
      }

      Directory.CreateDirectory(Path.GetDirectoryName(fileNameAndPath) ?? string.Empty);

      string mes = model.ApiModel.SaveAs(fileNameAndPath).ToString();
      if (mes == ReturnValue.GS_OK.ToString()) {
        _fileNameLastSaved = fileNameAndPath;
        PostHog.ModelIO(GsaGH.PluginInfo.Instance, $"save{fileNameAndPath.Substring(fileNameAndPath.LastIndexOf('.') + 1).ToUpper()}",
          (int)(new FileInfo(fileNameAndPath).Length / 1024));
        model.FileNameAndPath = fileNameAndPath;
      } else {
        this.AddRuntimeError(mes);
      }
    }

    internal void SaveButtonClick() {
      if (string.IsNullOrEmpty(_fileNameLastSaved)) {
        SaveAsButtonClick();
        return;
      }

      _saveInputOverride = true;
    }

    internal void SaveAsButtonClick() {
      var fdi = new SaveFileDialog {
        Filter = "GSA Files (*.gwb)|*.gwb|GSA Text Files (*.gwa)|*.gwa",
      };
      bool res = fdi.ShowSaveDialog();
      if (!res) {
        return;
      }

      while (Params.Input[2].Sources.Count > 0) {
        OnPingDocument().RemoveObject(Params.Input[2].Sources[0], false);
      }

      var panel = new GH_Panel();
      panel.CreateAttributes();

      panel.Attributes.Pivot
        = new PointF(
          Attributes.DocObject.Attributes.Bounds.Left - panel.Attributes.Bounds.Width - 40,
          Attributes.DocObject.Attributes.Bounds.Bottom - panel.Attributes.Bounds.Height);
      panel.UserText = fdi.FileName;
      OnPingDocument().AddObject(panel, false);

      Params.Input[2].AddSource(panel);
      Params.OnParametersChanged();
      ExpireSolution(true);
    }

    internal void OpenGsaExe() {
      if (string.IsNullOrEmpty(_fileNameLastSaved)) {
        Params.Input[0].CollectData();
        var tempModel = (GsaModelGoo)Params.Input[0].VolatileData.AllData(true).First();
        string tempPath = Path.GetTempPath() + tempModel.Value.Guid.ToString() + ".gwa";
        GsaModel gsaModel = tempModel.Value;
        Save(ref gsaModel, tempPath);
      }

      string fullPath = Path.GetFullPath(_fileNameLastSaved);
      string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
      Process.Start(programFiles + @"\Oasys\GSA 10.2\GSA.exe", fullPath);
    }
  }
}
