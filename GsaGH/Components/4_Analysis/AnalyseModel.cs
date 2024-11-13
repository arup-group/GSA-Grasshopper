using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using GH_IO.Serialization;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;

using GsaAPI;

using GsaGH.Helpers;
using GsaGH.Helpers.Assembly;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.GsaApi;
using GsaGH.Helpers.Import;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;
using OasysGH.UI;
using OasysGH.Units;
using OasysGH.Units.Helpers;

using OasysUnits;

using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to assemble and analyse a GSA model
  /// </summary>
  public class AnalyseModel : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("78fe156d-6ab4-4683-96a4-2d40eb5cce8f");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.AnalyseModel;
    private bool _analysis = true;
    private List<string> _checkboxTexts = new List<string>() {
      "Analyse task(s)",
      "ElemsFromMems",
    };
    private List<bool> _initialCheckState = new List<bool>() {
      true,
      true,
    };

    private bool _reMesh = true;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitGeometry;
    internal ToleranceContextMenu ToleranceMenu { get; set; } = new ToleranceContextMenu();

    public AnalyseModel() : base("Analyse Model", "Analyse", "Assemble and Analyse a GSA Model",
      CategoryName.Name(), SubCategoryName.Cat4()) {
      Hidden = true;
    }

    public override void AppendAdditionalMenuItems(ToolStripDropDown menu) {
      ToleranceMenu.AppendAdditionalMenuItems(this, menu, _lengthUnit);
    }

    public override void CreateAttributes() {
      if (!_isInitialised) {
        InitialiseDropdowns();
      }

      m_attributes = new DropDownCheckBoxesComponentAttributes(this, SetSelected, _dropDownItems,
        _selectedItems, SetAnalysis, _initialCheckState, _checkboxTexts, _spacerDescriptions);
    }

    public override bool Read(GH_IReader reader) {
      _analysis = reader.GetBoolean("Analyse");
      _reMesh = reader.GetBoolean("ReMesh");
      _initialCheckState = new List<bool>() {
        _analysis,
        _reMesh,
      };
      bool flag = base.Read(reader);
      if (reader.ItemExists("Tolerance")) {
        double tol = reader.GetDouble("Tolerance");
        ToleranceMenu.Tolerance = new Length(tol, _lengthUnit);
      } else {
        ToleranceMenu.Tolerance = DefaultUnits.Tolerance;
      }

      ToleranceMenu.UpdateMessage(this, _lengthUnit);
      return flag;
    }

    public void SetAnalysis(List<bool> value) {
      _analysis = value[0];
      _reMesh = value[1];
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[i]);
      ToleranceMenu.UpdateMessage(this, _lengthUnit);
      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      Params.Input[2].Name = "GSA Geometry in [" + Length.GetAbbreviation(_lengthUnit) + "]";
    }

    public override bool Write(GH_IWriter writer) {
      writer.SetBoolean("Analyse", _analysis);
      writer.SetBoolean("ReMesh", _reMesh);
      writer.SetDouble("Tolerance", ToleranceMenu.Tolerance.Value);
      return base.Write(writer);
    }

    protected override void BeforeSolveInstance() {
      base.BeforeSolveInstance();
      ToleranceMenu.UpdateMessage(this, _lengthUnit);

      // add report output to old components
      if (Params.Output.Count < 5) {
        Params.Output[1].Name = "Errors";
        Params.Output[1].NickName = "E";
        Params.Output[1].Description = "Analysis Task Errors";
        Params.Output[1].Access = GH_ParamAccess.list;
        Params.RegisterOutputParam(new Param_String());
        Params.Output[2].Name = "Warnings";
        Params.Output[2].NickName = "W";
        Params.Output[2].Description = "Analysis Task Warnings";
        Params.Output[2].Access = GH_ParamAccess.list;
        Params.RegisterOutputParam(new Param_String());
        Params.Output[3].Name = "Remarks";
        Params.Output[3].NickName = "R";
        Params.Output[3].Description = "Analysis Task Notes and Remarks";
        Params.Output[3].Access = GH_ParamAccess.list;
        Params.RegisterOutputParam(new Param_String());
        Params.Output[4].Name = "Log";
        Params.Output[4].NickName = "L";
        Params.Output[4].Description = "Analysis Task logs";
        Params.Output[4].Access = GH_ParamAccess.list;
        VariableParameterMaintenance();
      }
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Unit",
        "Settings",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      _selectedItems.Add(Length.GetAbbreviation(_lengthUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddGenericParameter("Model(s) and Lists", "GSA",
        "Existing GSA Model(s) to append to and Lists" + Environment.NewLine
        + "If you input more than one model they will be merged" + Environment.NewLine
        + "with first model in list taking priority for IDs", GH_ParamAccess.list);
      pManager.AddGenericParameter("Properties", "Pro",
        "GSA Sections (PB), Prop2Ds (PA) and Prop3Ds (PV) to add/set in the model"
        + Environment.NewLine + "Properties already added to Elements or Members"
        + Environment.NewLine + "will automatically be added with Geometry input",
        GH_ParamAccess.list);
      pManager.AddGenericParameter("GSA Geometry in [" + Length.GetAbbreviation(_lengthUnit) + "]",
        "Geo",
        "GSA Nodes, Element1Ds, Element2Ds, Member1Ds, Member2Ds and Member3Ds to add/set in model",
        GH_ParamAccess.list);
      pManager.AddGenericParameter("Load", "Ld",
        "Loads to add to the model" + Environment.NewLine
        + "You can also use this input to add Edited GridPlaneSurfaces", GH_ParamAccess.list);
      pManager.AddGenericParameter("Analysis Tasks & Combinations", "ΣT",
        "GSA Analysis Tasks and Combination Cases to add to the model", GH_ParamAccess.list);
      for (int i = 0; i < pManager.ParamCount; i++) {
        pManager[i].Optional = true;
      }
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaModelParameter());
      pManager.AddTextParameter("Errors", "E", "Analysis Task Errors", GH_ParamAccess.list);
      pManager.AddTextParameter("Warnings", "W", "Analysis Task Warnings", GH_ParamAccess.list);
      pManager.AddTextParameter("Remarks", "R", "Analysis Task Notes and Remarks", GH_ParamAccess.list);
      pManager.AddTextParameter("Logs", "L", "Analysis Task logs", GH_ParamAccess.list);
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      // Collect inputs
      (List<GsaModel> models, List<GsaList> lists, List<GsaGridLine> gridLines) = InputsForModelAssembly.GetModelsAndLists(this, da, 0, true);
      GsaProperties properties = InputsForModelAssembly.GetProperties(this, da, 1, true);
      GsaGeometry geometry = InputsForModelAssembly.GetGeometry(this, da, 2, true);
      GsaLoading loading = InputsForModelAssembly.GetLoading(this, da, 3, true);
      GsaAnalysis analysis = InputsForModelAssembly.GetAnalysis(this, da, 4, true);

      if (models.IsNullOrEmpty() & lists.IsNullOrEmpty() & gridLines.IsNullOrEmpty()
        & geometry.IsNullOrEmpty() & properties.IsNullOrEmpty()
        & loading.IsNullOrEmpty() & analysis.IsNullOrEmpty()) {
        this.AddRuntimeWarning("Input parameters failed to collect data");
        return;
      }

      // Merge models
      var model = new GsaModel();
      if (models != null) {
        if (models.Count > 0) {
          model = models.Count > 1
            ? MergeModels.MergeModel(models, this, ToleranceMenu.Tolerance) :
            models[0].Clone();
        }
      }

      // Assemble model
      var assembly = new ModelAssembly(model, lists, gridLines, geometry, properties, loading,
        analysis, _lengthUnit, ToleranceMenu.Tolerance, _reMesh, this);
      model.ApiModel = assembly.GetModel();

      // Run analysis
      if (_analysis) {
        IReadOnlyDictionary<int, AnalysisTask> gsaTasks = model.ApiModel.AnalysisTasks();
        if (gsaTasks.Count < 1) {
          var task = new GsaAnalysisTask {
            Id = model.ApiModel.AddAnalysisTask(),
          };
          ModelFactory.BuildAnalysisTask(model.ApiModel, new List<GsaAnalysisTask> { task }, true);
          if (model.ApiModel.AnalysisTasks()[task.Id].Cases.Count == 0) {
            this.AddRuntimeWarning(
              " Model contains no loads and has not been analysed, but has been assembled.");
          } else {
            this.AddRuntimeRemark(
              " Model contained no Analysis Tasks. Default Task has been created containing " +
              "all cases found in model");
            gsaTasks = model.ApiModel.AnalysisTasks();
          }
        }

        bool tryAnalyse = true;
        if (!SolverRequiredDll.IsCorrectVersionLoaded()) {
          tryAnalyse = false;
          string message
            = " A dll required to run analysis has been previously loaded by another application. " +
            "Please remove this file and try again:"
            + Environment.NewLine + Environment.NewLine + SolverRequiredDll.LoadedFromPath
            + Environment.NewLine + "Either uninstall the host application or delete the file.";
          this.AddRuntimeError(message);
        }

        if (tryAnalyse) {
          var logs = new List<string>();
          var notes = new List<string>();
          var remarks = new List<string>();
          var warnings = new List<string>();
          var errors = new List<string>();

          foreach (KeyValuePair<int, AnalysisTask> task in gsaTasks) {
            if (model.ApiModel.Analyse(task.Key, out TaskReport report)) {
              OasysGH.Helpers.PostHog.ModelIO(GsaGH.PluginInfo.Instance, "analyse",
                model.ApiModel.Elements().Count);
            } else {
              string message = "Analysis Task " + task.Key +
                " failed with one or more errors. Check report output for details";
              this.AddRuntimeError($" {message}");
            }

            logs.Add(report.Log);
            notes.AddRange(report.Notes);
            remarks.AddRange(report.Warnings);
            foreach (string message in report.SevereWarnings) {
              this.AddRuntimeWarning($"Task {task.Key}: {message}");
              warnings.Add($"Task {task.Key}: {message}");
            }

            foreach (string message in report.Errors) {
              this.AddRuntimeError($"Task {task.Key}: {message}");
              errors.Add($"Task {task.Key}: {message}");
            }
          }

          var notesAndRemarks = new List<string>();
          foreach (string message in remarks) {
            if (!RuntimeMessages(GH_RuntimeMessageLevel.Remark).Contains(message)) {
              this.AddRuntimeRemark(message);
              notesAndRemarks.Add($"Warning: {message}");
            }
          }

          foreach (string message in notes) {
            if (!notesAndRemarks.Contains($"Note: {message}")) {
              notesAndRemarks.Add($"Note: {message}");
            }
          }

          string syncWarn = CatchElemFromMemSyncWarning();
          if (!string.IsNullOrEmpty(syncWarn)) {
            string warning = "Member and element synchronisation check has one or more warnings. " +
            "Check report output for details";
            this.AddRuntimeWarning(warning);
            notesAndRemarks.Add(syncWarn);
          }

          model.Guid = Guid.NewGuid();
          da.SetDataList(1, errors);
          da.SetDataList(2, warnings);
          da.SetDataList(3, notesAndRemarks);
          da.SetDataList(4, logs);
        }
      }

      model.ModelUnit = _lengthUnit;
      da.SetData(0, new GsaModelGoo(model));
    }

    protected override void UpdateUIFromSelectedItems() {
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[0]);
      base.UpdateUIFromSelectedItems();
    }

    private string CatchElemFromMemSyncWarning() {
      var remarks = RuntimeMessages(GH_RuntimeMessageLevel.Remark).ToList();
      var warnings = RuntimeMessages(GH_RuntimeMessageLevel.Warning).ToList();
      var errors = RuntimeMessages(GH_RuntimeMessageLevel.Error).ToList();
      ClearRuntimeMessages();
      foreach (string remark in remarks) {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, remark);
      }
      foreach (string error in errors) {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, error);
      }

      string syncWarning = string.Empty;
      if (warnings.Count > 0) {
        for (int i = 0; i < warnings.Count; i++) {
          if (warnings[i].Trim().StartsWith(
            "Creating Elements From Members will recreate child Elements")) {
            string[] split = warnings[i].Split(new string[] {
                "The following former Element IDs were updated:"
                }, StringSplitOptions.None);

            string warning = "Member and element synchronisation check\n" +
              "Warning: Creating Elements From Members will recreate child Elements. " +
            Environment.NewLine + "This will update the Element's property to the parent " +
            "Member's property, and may also renumber element IDs. " +
            Environment.NewLine + "The following former Element IDs were updated:"
            + Environment.NewLine;
            string ids = split[1].Replace(
              "(list may be too long to display, click to copy)", string.Empty)
              .Replace(Environment.NewLine, string.Empty);
            syncWarning = warning + ids + "\n \n";
            warnings.RemoveAt(i);
            break;
          }
        }
      }

      foreach (string warning in warnings) {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, warning);
      }

      return syncWarning;
    }
  }
}
