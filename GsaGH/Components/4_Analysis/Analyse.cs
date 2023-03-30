using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using GsaAPI;
using GsaGH.Helpers.Export;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.UI;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components {

  /// <summary>
  ///   Component to assemble and analyse a GSA model
  /// </summary>
  public class GhAnalyse : GH_OasysDropDownComponent {

    #region Properties + Fields
    public override Guid ComponentGuid => new Guid("78fe156d-6ab4-4683-96a4-2d40eb5cce8f");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.Analyse;
    private bool _analysis = true;

    private List<string> _checkboxTexts = new List<string>() {
      "Analyse task(s)",
      "ElemsFromMems",
    };

    private List<bool> _initialCheckState = new List<bool>() {
      true,
      true,
    };

    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitGeometry;
    private bool _reMesh = true;
    private Length _tolerance = DefaultUnits.Tolerance;
    private string _toleranceTxt = "";
    #endregion Properties + Fields

    #region Public Constructors
    public GhAnalyse() : base("Analyse Model",
      "Analyse",
      "Assemble and Analyse a GSA Model",
      CategoryName.Name(),
      SubCategoryName.Cat4())
      => Hidden = true;

    #endregion Public Constructors

    #region Public Methods
    public override void AppendAdditionalMenuItems(ToolStripDropDown menu) {
      Menu_AppendSeparator(menu);

      var tolerance = new ToolStripTextBox();
      _toleranceTxt = _tolerance.ToUnit(_lengthUnit)
        .ToString()
        .Replace(" ", string.Empty);
      tolerance.Text = _toleranceTxt;
      tolerance.BackColor = Color.FromArgb(255, 180, 255, 150);
      tolerance.TextChanged += (s, e) => MaintainText(tolerance);

      var toleranceMenu = new ToolStripMenuItem("Set Tolerance", Resources.Units) {
        Enabled = true,
        ImageScaling = ToolStripItemImageScaling.SizeToFit,
      };

      toleranceMenu.DropDownItems[1]
        .MouseUp += (s, e) => {
          UpdateMessage();
          (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
          ExpireSolution(true);
        };
      menu.Items.Add(toleranceMenu);

      Menu_AppendSeparator(menu);

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
    }

    public override void CreateAttributes() {
      if (!IsInitialised)
        InitialiseDropdowns();
      m_attributes = new DropDownCheckBoxesComponentAttributes(this,
        SetSelected,
        DropDownItems,
        SelectedItems,
        SetAnalysis,
        _initialCheckState,
        _checkboxTexts,
        SpacerDescriptions);
    }

    public override void InitialiseDropdowns() {
      SpacerDescriptions = new List<string>(new[] {
        "Unit",
        "Settings",
      });

      DropDownItems = new List<List<string>>();
      SelectedItems = new List<string>();

      DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      SelectedItems.Add(Length.GetAbbreviation(_lengthUnit));

      IsInitialised = true;
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
        _tolerance = new Length(tol, _lengthUnit);
      }
      else
        _tolerance = DefaultUnits.Tolerance;

      UpdateMessage();
      return flag;
    }

    public void SetAnalysis(List<bool> value) {
      _analysis = value[0];
      _reMesh = value[1];
    }

    public override void SetSelected(int i, int j) {
      SelectedItems[i] = DropDownItems[i][j];
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), SelectedItems[i]);
      UpdateMessage();
      base.UpdateUI();
    }

    public override void UpdateUIFromSelectedItems() {
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), SelectedItems[0]);
      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance()
      => Params.Input[2]
        .Name = "GSA Geometry in [" + Length.GetAbbreviation(_lengthUnit) + "]";

    public override bool Write(GH_IWriter writer) {
      writer.SetBoolean("Analyse", _analysis);
      writer.SetBoolean("ReMesh", _reMesh);
      writer.SetDouble("Tolerance", _tolerance.Value);
      return base.Write(writer);
    }

    #endregion Public Methods

    #region Protected Methods
    protected override void BeforeSolveInstance() {
      base.BeforeSolveInstance();
      UpdateMessage();
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaModelParameter(),
        "Model(s)",
        "GSA",
        "Existing GSA Model(s) to append to"
        + Environment.NewLine
        + "If you input more than one model they will be merged"
        + Environment.NewLine
        + "with first model in list taking priority for IDs",
        GH_ParamAccess.list);
      pManager.AddGenericParameter("Properties",
        "Pro",
        "GSA Sections (PB), Prop2Ds (PA) and Prop3Ds (PV) to add/set in the model"
        + Environment.NewLine
        + "Properties already added to Elements or Members"
        + Environment.NewLine
        + "will automatically be added with Geometry input",
        GH_ParamAccess.list);
      pManager.AddGenericParameter("GSA Geometry in [" + Length.GetAbbreviation(_lengthUnit) + "]",
        "Geo",
        "GSA Nodes, Element1Ds, Element2Ds, Member1Ds, Member2Ds and Member3Ds to add/set in model",
        GH_ParamAccess.list);
      pManager.AddGenericParameter("Load",
        "Ld",
        "Loads to add to the model"
        + Environment.NewLine
        + "You can also use this input to add Edited GridPlaneSurfaces",
        GH_ParamAccess.list);
      pManager.AddGenericParameter("Analysis Tasks & Combinations",
        "ΣT",
        "GSA Analysis Tasks and Combination Cases to add to the model",
        GH_ParamAccess.list);
      for (int i = 0; i < pManager.ParamCount; i++)
        pManager[i]
          .Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
      => pManager.AddParameter(new GsaModelParameter());

    protected override void SolveInstance(IGH_DataAccess da) {

      #region GetData

      List<GsaModel> models = GetInputsForModelAssembly.GetModels(this, da, 0, true);

      (List<GsaSection> sections, List<GsaProp2d> prop2Ds, List<GsaProp3d> prop3Ds)
        = GetInputsForModelAssembly.GetProperties(this, da, 1, true);

      (List<GsaNode> nodes, List<GsaElement1d> elem1ds, List<GsaElement2d> elem2ds,
        List<GsaElement3d> elem3ds, List<GsaMember1d> mem1ds, List<GsaMember2d> mem2ds,
        List<GsaMember3d> mem3ds) = GetInputsForModelAssembly.GetGeometry(this, da, 2, true);

      (List<GsaLoad> loads, List<GsaGridPlaneSurface> gridPlaneSurfaces)
        = GetInputsForModelAssembly.GetLoading(this, da, 3, true);

      (List<GsaAnalysisTask> analysisTasks, List<GsaCombinationCase> combinationCases)
        = GetInputsForModelAssembly.GetAnalysis(this, da, 4, true);

      if (models is null
        & nodes is null
        & elem1ds is null
        & elem2ds is null
        & mem1ds is null
        & mem2ds is null
        & mem3ds is null
        & sections is null
        & prop2Ds is null
        & loads is null
        & gridPlaneSurfaces is null) {
        this.AddRuntimeWarning("Input parameters failed to collect data");
        return;
      }

      #endregion GetData

      var model = new GsaModel();
      if (models != null)
        if (models.Count > 0)
          model = models.Count > 1
            ? MergeModels.MergeModel(models, this, _tolerance)
            : models[0]
              .Clone();

      model.Model = AssembleModel.Assemble(model,
        nodes,
        elem1ds,
        elem2ds,
        elem3ds,
        mem1ds,
        mem2ds,
        mem3ds,
        sections,
        prop2Ds,
        prop3Ds,
        loads,
        gridPlaneSurfaces,
        analysisTasks,
        combinationCases,
        _lengthUnit,
        _tolerance,
        _reMesh,
        this);

      #region analysis

      if (_analysis) {
        IReadOnlyDictionary<int, AnalysisTask> gsaTasks = model.Model.AnalysisTasks();
        if (gsaTasks.Count < 1) {
          var task = new GsaAnalysisTask {
            Id = model.Model.AddAnalysisTask(),
          };
          task.CreateDefaultCases(model.Model);
          if (task.Cases == null || task.Cases.Count == 0)
            this.AddRuntimeWarning(
              "Model contains no loads and has not been analysed, but has been assembled.");
          else {
            this.AddRuntimeRemark(
              "Model contained no Analysis Tasks. Default Task has been created containing all cases found in model");
            foreach (GsaAnalysisCase ca in task.Cases)
              model.Model.AddAnalysisCaseToTask(task.Id, ca.Name, ca.Description);
            gsaTasks = model.Model.AnalysisTasks();
          }
        }

        // Workaround BUG in GsaAPI that will crash the application if element.property = 0:
        ReadOnlyDictionary<int, Element> apielems = model.Model.Elements();
        bool tryAnalyse = true;
        foreach (int key in apielems.Keys) {
          if (apielems[key]
              .Property
            != 0
            || apielems[key]
              .IsDummy)
            continue;

          {
            this.AddRuntimeError("Unable to analyse model. Element ID "
              + key
              + " has no property set!");
            tryAnalyse = false;
          }
        }

        if (tryAnalyse)
          if (!SolverRequiredDll.IsCorrectVersionLoaded()) {
            tryAnalyse = false;
            string message
              = "A dll required to run analysis has been previously loaded by another application. Please remove this file and try again:"
              + Environment.NewLine
              + Environment.NewLine
              + SolverRequiredDll.LoadedFromPath
              + Environment.NewLine
              + "Either uninstall the host application or delete the file.";
            this.AddRuntimeError(message);
          }

        if (tryAnalyse) {
          foreach (KeyValuePair<int, AnalysisTask> task in gsaTasks) {
            try {
              if (model.Model.Analyse(task.Key))
                PostHog.ModelIO(GsaGH.PluginInfo.Instance, "analyse", apielems.Count);
              else
                this.AddRuntimeWarning("Analysis Case " + task.Key + " could not be analysed");
              if (!model.Model.Results()
                .ContainsKey(task.Key))
                this.AddRuntimeWarning("Analysis Case " + task.Key + " could not be analysed");
            }
            catch (Exception e) {
              this.AddRuntimeError(e.Message);
            }
          }

          model.Guid = Guid.NewGuid();
        }
      }

      #endregion analysis

      model.ModelUnit = _lengthUnit;
      da.SetData(0, new GsaModelGoo(model));
    }

    #endregion Protected Methods

    #region Private Methods
    private void MaintainText(ToolStripItem tolerance) {
      _toleranceTxt = tolerance.Text;
      tolerance.BackColor = Length.TryParse(_toleranceTxt, out Length _)
        ? Color.FromArgb(255, 180, 255, 150)
        : Color.FromArgb(255, 255, 100, 100);
    }

    private void UpdateMessage() {
      if (_toleranceTxt != "")
        try {
          _tolerance = Length.Parse(_toleranceTxt);
        }
        catch (Exception e) {
          MessageBox.Show(e.Message);
          return;
        }

      _tolerance = _tolerance.ToUnit(_lengthUnit);
      Message = "Tol: "
        + _tolerance.ToString()
          .Replace(" ", string.Empty);
      if (_tolerance.Meters < 0.001)
        this.AddRuntimeRemark(
          "Set tolerance is quite small, you can change this by right-clicking the component.");
      if (_tolerance.Meters > 0.25)
        this.AddRuntimeRemark(
          "Set tolerance is quite large, you can change this by right-clicking the component.");
    }

    #endregion Private Methods
  }
}
