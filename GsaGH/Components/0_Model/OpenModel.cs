using System;
using System.Drawing;
using System.IO;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;

using GsaAPI;

using GsaGH.Helpers.GH;
using GsaGH.Helpers.GsaApi.EnumMappings;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.UI;

using Rhino.UI;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to open an existing GSA model
  /// </summary>
  public class OpenModel : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("10bb2aac-504e-4054-9708-5053fbca61fc");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.OpenModel;
    private Guid _panelGuid = Guid.NewGuid();

    public OpenModel() : base("Open Model", "Open", "Open an existing GSA model",
      CategoryName.Name(), SubCategoryName.Cat0()) {
      Hidden = true;
    }

    public override void CreateAttributes() {
      if (!_isInitialised) {
        InitialiseDropdowns();
      }
      m_attributes = new ButtonComponentAttributes(this, "Open", OpenFile, "Open GSA file");
    }

    public void OpenFile() {
      var fdi = new OpenFileDialog {
        Filter = "GSA Files (*.gwb)|*.gwb|GSA Text Files (*.gwa)|*.gwa|All files (*.*)|*.*",
      };
      bool res = fdi.ShowOpenDialog();

      if (!res) {
        return;
      }

      string fileName = fdi.FileName;

      var panel = new GH_Panel();
      panel.CreateAttributes();
      panel.Attributes.Pivot = new PointF(
        Attributes.DocObject.Attributes.Bounds.Left - panel.Attributes.Bounds.Width - 30,
        Params.Input[0].Attributes.Pivot.Y - (panel.Attributes.Bounds.Height / 2));

      while (Params.Input[0].Sources.Count > 0) {
        IGH_Param input = Params.Input[0].Sources[0];
        if (Params.Input[0].Sources[0].InstanceGuid == _panelGuid) {
          panel = input as GH_Panel;
          panel.UserText = fileName;
          panel.ExpireSolution(true);
        }

        Params.Input[0].RemoveSource(input);
      }

      panel.UserText = fileName;
      _panelGuid = panel.InstanceGuid;
      OnPingDocument().AddObject(panel, false);
      Params.Input[0].AddSource(panel);

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();

      ExpireSolution(true);
    }

    public override void SetSelected(int i, int j) { }

    protected override void InitialiseDropdowns() {
      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddTextParameter("Filename and path", "File",
        "GSA model to open and work with." + Environment.NewLine
        + "Input either path component, a text string with path and " + Environment.NewLine
        + "filename or an existing GSA model created in Grasshopper.", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaModelParameter());
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      var model = new Model();
      string fileName = string.Empty;
      da.GetData(0, ref fileName);

      model.Open(fileName);

      var gsaModel = new GsaModel(model) {
        FileNameAndPath = fileName,
        ModelUnit = UnitMapping.GetUnit(model)
      };
      UpdateMessage(fileName);

      if (gsaModel.Materials.SanitizeGenericCodeNames()) {
        this.AddRuntimeRemark("The opened model contains generic materials with no design code");
      }

      da.SetData(0, new GsaModelGoo(gsaModel));
      PostHog.ModelIO(GsaGH.PluginInfo.Instance, $"open{fileName.Substring(fileName.LastIndexOf('.') + 1).ToUpper()}",
        (int)(new FileInfo(fileName).Length / 1024));
    }

    private void UpdateMessage(string fileName) {
      string mes = Path.GetFileName(fileName);
      mes = mes.Substring(0, mes.Length - 4);
      Message = mes;
    }
  }
}
