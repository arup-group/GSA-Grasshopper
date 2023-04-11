using System;
using System.Drawing;
using System.IO;
using GH_IO.Serialization;
using Grasshopper;
using Grasshopper.Kernel;
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
  ///   Component to open an existing GSA model
  /// </summary>
  public class OpenModel : GH_OasysDropDownComponent {
    protected override void SolveInstance(IGH_DataAccess da) {
      var model = new Model();
      var ghTyp = new GH_ObjectWrapper();
      if (da.GetData(0, ref ghTyp))
        switch (ghTyp.Value) {
          case GH_String _: {
              if (GH_Convert.ToString(ghTyp, out string tempFile, GH_Conversion.Both))
                _fileName = tempFile;

              if (!_fileName.EndsWith(".gwb"))
                _fileName += ".gwb";

              ReturnValue status = model.Open(_fileName);

              if (status == 0) {
                var gsaModel = new GsaModel {
                  Model = model,
                  FileNameAndPath = _fileName,
                };

                GetTitles(model);
                GetUnit(ref gsaModel);

                da.SetData(0, new GsaModelGoo(gsaModel));

                PostHog.ModelIO(GsaGH.PluginInfo.Instance,
                  "openGWB",
                  (int)(new FileInfo(_fileName).Length / 1024));

                return;
              }

              this.AddRuntimeError("Unable to open Model" + Environment.NewLine + status.ToString());
              return;
            }
          case Model _: {
              ghTyp.CastTo(ref model);
              var gsaModel = new GsaModel {
                Model = model,
              };

              da.SetData(0, new GsaModelGoo(gsaModel));
              return;
            }
          default:
            this.AddRuntimeError("Unable to open Model");
            return;
        }
      else {
        ReturnValue status = model.Open(_fileName);

        if (status == 0) {
          var gsaModel = new GsaModel {
            Model = model,
            FileNameAndPath = _fileName,
          };

          GetTitles(model);
          GetUnit(ref gsaModel);

          da.SetData(0, new GsaModelGoo(gsaModel));
        }
        else
          this.AddRuntimeError("Unable to open Model" + Environment.NewLine + status.ToString());
      }
    }

    private void GetTitles(Model model) {
      Titles.GetTitlesFromGsa(model);
      string mes = Path.GetFileName(_fileName);
      mes = mes.Substring(0, mes.Length - 4);
      Message = mes;
    }

    private static void GetUnit(ref GsaModel gsaModel) {
      // none of this works:
      // 1. save as gwa is not possible with GsaAPI
      // 2. Output_UnitFactor and Output_String() both result COM error

      //Interop.Gsa_10_1.ComAuto m = new Interop.Gsa_10_1.ComAuto();
      //string temp = Path.GetTempPath() + Guid.NewGuid().ToString() + ".gwa";
      //gsaModel.Model.SaveAs(temp);
      //m.Open(temp);
      //float unit = m.Output_UnitFactor();

      //gsaModel.ModelGeometryUnit = (OasysUnits.Units.LengthUnit)OasysGH.Units.Helpers.UnitsHelper.Parse(typeof(OasysUnits.Units.LengthUnit), unit);
    }

    #region Name and Ribbon Layout

    public override Guid ComponentGuid => new Guid("10bb2aac-504e-4054-9708-5053fbca61fc");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.OpenModel;

    public OpenModel() : base("Open Model",
      "Open",
      "Open an existing GSA model",
      CategoryName.Name(),
      SubCategoryName.Cat0())
      => Hidden = true;

    #endregion

    #region Input and output

    private string _fileName;
    private Guid _panelGuid = Guid.NewGuid();

    protected override void RegisterInputParams(GH_InputParamManager pManager)
      => pManager.AddGenericParameter("Filename and path",
        "File",
        "GSA model to open and work with."
        + Environment.NewLine
        + "Input either path component, a text string with path and "
        + Environment.NewLine
        + "filename or an existing GSA model created in Grasshopper.",
        GH_ParamAccess.item);

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
      => pManager.AddParameter(new GsaModelParameter());

    #endregion

    #region Custom UI

    public override void SetSelected(int i, int j) { }
    protected override void InitialiseDropdowns() { }

    public override void CreateAttributes()
      => m_attributes = new ButtonComponentAttributes(this, "Open", OpenFile, "Open GSA file");

    public override void VariableParameterMaintenance() {
      Params.Input[0]
        .Optional = _fileName != null;
      Params.Input[0]
        .ClearRuntimeMessages();
    }

    public void OpenFile() {
      var fdi = new OpenFileDialog {
        Filter = "GSA Files(*.gwb)|*.gwb|All files (*.*)|*.*",
      }; //"GSA Files(*.gwa; *.gwb)|*.gwa;*.gwb|All files (*.*)|*.*"
      bool res = fdi.ShowOpenDialog();

      if (!res)
        return;

      _fileName = fdi.FileName;

      var panel = new GH_Panel();
      panel.CreateAttributes();
      panel.Attributes.Pivot = new PointF(
        Attributes.DocObject.Attributes.Bounds.Left - panel.Attributes.Bounds.Width - 30,
        Params.Input[0]
          .Attributes.Pivot.Y
        - panel.Attributes.Bounds.Height / 2);

      while (Params.Input[0]
          .Sources.Count
        > 0) {
        IGH_Param input = Params.Input[0]
          .Sources[0];
        if (Params.Input[0]
            .Sources[0]
            .InstanceGuid
          == _panelGuid) {
          panel = input as GH_Panel;
          panel.UserText = _fileName;
          panel.ExpireSolution(true);
        }

        Params.Input[0]
          .RemoveSource(input);
      }

      panel.UserText = _fileName;
      _panelGuid = panel.InstanceGuid;
      Instances.ActiveCanvas.Document.AddObject(panel, false);
      Params.Input[0]
        .AddSource(panel);

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();

      ExpireSolution(true);
    }

    #endregion

    #region (de)serialization

    public override bool Write(GH_IWriter writer) {
      writer.SetString("File", _fileName);
      return base.Write(writer);
    }

    public override bool Read(GH_IReader reader) {
      _fileName = reader.GetString("File");
      return base.Read(reader);
    }

    #endregion
  }
}
