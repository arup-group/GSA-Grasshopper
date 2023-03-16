using System;
using System.Drawing;
using System.IO;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {
  /// <summary>
  /// Component to open an existing GSA model
  /// </summary>
  public class OpenModel : GH_OasysDropDownComponent {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("10bb2aac-504e-4054-9708-5053fbca61fc");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Properties.Resources.OpenModel;

    public OpenModel() : base("Open Model",
      "Open",
      "Open an existing GSA model",
      CategoryName.Name(),
      SubCategoryName.Cat0()) {
      Hidden = true;
    } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    private string _fileName = null;
    private Guid _panelGuid = Guid.NewGuid();
    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddGenericParameter("Filename and path", "File", "GSA model to open and work with." +
              Environment.NewLine + "Input either path component, a text string with path and " +
              Environment.NewLine + "filename or an existing GSA model created in Grasshopper.", GH_ParamAccess.item);
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaModelParameter());
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess da) {
      var model = new Model();
      var ghTyp = new GH_ObjectWrapper();
      if (da.GetData(0, ref ghTyp)) {
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

                OasysGH.Helpers.PostHog.ModelIO(GsaGH.PluginInfo.Instance, "openGWB", (int)(new FileInfo(_fileName).Length / 1024));

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
        else {
          this.AddRuntimeError("Unable to open Model" + Environment.NewLine + status.ToString());
        }
      }
    }

    private void GetTitles(Model model) {
      Titles.GetTitlesFromGSA(model);
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

    #region Custom UI
    public override void SetSelected(int i, int j) { }
    public override void InitialiseDropdowns() { }

    public override void CreateAttributes() {
      m_attributes = new OasysGH.UI.ButtonComponentAttributes(this, "Open", OpenFile, "Open GSA file");
    }

    public override void VariableParameterMaintenance() {
      Params.Input[0].Optional = _fileName != null; //filename can have input from user input
      Params.Input[0].ClearRuntimeMessages(); // this needs to be called to avoid having a runtime warning message after changed to optional
    }
    public void OpenFile() {
      var fdi = new Rhino.UI.OpenFileDialog { Filter = "GSA Files(*.gwb)|*.gwb|All files (*.*)|*.*" }; //"GSA Files(*.gwa; *.gwb)|*.gwa;*.gwb|All files (*.*)|*.*"
      bool res = fdi.ShowOpenDialog();

      if (!res)
        return;

      // == DialogResult.OK)
      _fileName = fdi.FileName;

      // instantiate  new panel
      var panel = new Grasshopper.Kernel.Special.GH_Panel();
      panel.CreateAttributes();

      // set the location relative to the open component on the canvas
      panel.Attributes.Pivot = new PointF(Attributes.DocObject.Attributes.Bounds.Left -
                                          panel.Attributes.Bounds.Width - 30, Params.Input[0].Attributes.Pivot.Y - panel.Attributes.Bounds.Height / 2);

      // check for existing input
      while (Params.Input[0].Sources.Count > 0) {
        IGH_Param input = Params.Input[0].Sources[0];
        // check if input is the one we automatically create below
        if (Params.Input[0].Sources[0].InstanceGuid == _panelGuid) {
          // update the UserText in existing panel
          //RecordUndoEvent("Changed OpenGSA Component input");
          panel = input as Grasshopper.Kernel.Special.GH_Panel;
          panel.UserText = _fileName;
          panel.ExpireSolution(true); // update the display of the panel
        }

        // remove input
        Params.Input[0].RemoveSource(input);
      }

      //populate panel with our own content
      panel.UserText = _fileName;

      // record the panel's GUID if new, so that we can update it on change
      _panelGuid = panel.InstanceGuid;

      //Until now, the panel is a hypothetical object.
      // This command makes it 'real' and adds it to the canvas.
      Grasshopper.Instances.ActiveCanvas.Document.AddObject(panel, false);

      //Connect the new slider to this component
      Params.Input[0].AddSource(panel);

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();

      ExpireSolution(true);
    }
    #endregion

    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer) {
      writer.SetString("File", _fileName);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader) {
      _fileName = reader.GetString("File");
      return base.Read(reader);
    }
    #endregion
  }
}

