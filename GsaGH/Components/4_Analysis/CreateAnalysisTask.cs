using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to create a GSA Analysis Task
  /// </summary>
  public class CreateAnalysisTask : GH_OasysDropDownComponent {
    internal static readonly IReadOnlyDictionary<string, AnalysisTaskType> _solverTypes
      = new Dictionary<string, AnalysisTaskType> {
        { "Static", AnalysisTaskType.Static },
        { "Static P-delta", AnalysisTaskType.StaticPDelta },
        { "Footfall", AnalysisTaskType.Footfall },
      };

    public override Guid ComponentGuid => new Guid("581601cc-c0bc-47fe-ada5-b821327a4409");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateAnalysisTask;
    private AnalysisTaskType _type = AnalysisTaskType.Static;
    private int _casesParamIndex = 2;

    public CreateAnalysisTask() : base(
      "Create " + GsaAnalysisTaskGoo.Name,
      GsaAnalysisTaskGoo.NickName.Replace(" ", string.Empty),
      "Create a " + GsaAnalysisTaskGoo.Description, CategoryName.Name(), SubCategoryName.Cat4()) {
      Hidden = true;
    }

    public override bool Read(GH_IReader reader) {
      _casesParamIndex = reader.GetInt32("_casesParamIndex");
      return base.Read(reader);
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];

      if (i == 0) {
        _type = _solverTypes[_selectedItems[0]];
        UpdateDropdownItems();
      }

      UpdateParameters();

      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      switch (_type) {
        case AnalysisTaskType.StaticPDelta:
          switch (_selectedItems[1]) {
            case "Own":
            default:
              // do nothing
              break;

            case "Load case":
              Params.Input[2].NickName = "LC";
              Params.Input[2].Name = "Load case";
              Params.Input[2].Description = "Load case definition (e.g. 1.2L1 + 1.2L2)";
              Params.Input[2].Access = GH_ParamAccess.item;
              Params.Input[2].Optional = false;
              break;

            case "Result case":
              Params.Input[2].NickName = "RC";
              Params.Input[2].Name = "Result case";
              Params.Input[2].Description = "The result case that forms the basis for the geometric stiffness";
              Params.Input[2].Access = GH_ParamAccess.item;
              Params.Input[2].Optional = false;
              break;
          }

          break;

        case AnalysisTaskType.Footfall:
          Params.Input[2].NickName = "RN";
          Params.Input[2].Name = "Response nodes";
          Params.Input[2].Description = "Node list to define the nodes that the responses will be calculated in the analysis";
          Params.Input[2].Access = GH_ParamAccess.item;
          Params.Input[2].Optional = false;

          int i = 3;
          if (_selectedItems[1] != "Self excitation") {
            Params.Input[i].NickName = "EN";
            Params.Input[i].Name = "Excitation nodes";
            Params.Input[i].Description = "Node list to define the nodes that will be excited for evaluation of the response of the response nodes";
            Params.Input[i].Access = GH_ParamAccess.item;
            Params.Input[i].Optional = false;
          }

          Params.Input[i++].NickName = "N";
          Params.Input[i].Name = "Number of footfalls";
          Params.Input[i].Description = "The number of footfalls to be considered in the analysis";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          Params.Input[i++].NickName = "W";
          Params.Input[i].Name = "Walker";
          Params.Input[i].Description = "The mass representing a sample walker";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          Params.Input[i++].NickName = "D";
          Params.Input[i].Name = "Direction of responses";
          Params.Input[i].Description = "The direction of response in the GSA global axis direction. It can be in Z (vertical), X, Y or XY plane (horizontal)";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          Params.Input[i++].NickName = "F";
          Params.Input[i].Name = "Frequency weighting curve";
          Params.Input[i].Description = "The Frequency Weighting Curve (FWC) is used in calculating the response factors. Standard and user defined curves can be used";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          Params.Input[i++].NickName = "EF";
          Params.Input[i].Name = "Excitation forces (DLFs)";
          Params.Input[i].Description = "This defines the way of the structure to be excited (the dynamic Load Factor to be used)";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          break;

        case AnalysisTaskType.Static:
        default:
          // do nothing
          break;
      }
    }

    public override bool Write(GH_IWriter writer) {
      writer.SetInt32("_casesParamIndex", _casesParamIndex);
      return base.Write(writer);
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Solver",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(_solverTypes.Keys.ToList());
      _selectedItems.Add(_dropDownItems[0].First());

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddIntegerParameter("Task ID", "ID", "The Task number of the Analysis Task",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Name", "Na", "Task Name", GH_ParamAccess.item);
      pManager.AddGenericParameter("Analysis Cases", "ΣAs",
        "List of GSA Analysis Cases (if left empty, all load cases in model will be added)",
        GH_ParamAccess.list);
      pManager[0].Optional = true;
      pManager[1].Optional = true;
      pManager[2].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaAnalysisTaskParameter());
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      int id = 0;
      da.GetData(0, ref id);

      string name = _type.ToString();
      da.GetData(1, ref name);

      List<GsaAnalysisCase> cases = null;
      var ghTypes = new List<GH_ObjectWrapper>();
      if (da.GetDataList(_casesParamIndex, ghTypes)) {
        cases = new List<GsaAnalysisCase>();
        for (int i = 0; i < ghTypes.Count; i++) {
          GH_ObjectWrapper ghTyp = ghTypes[i];
          if (ghTyp == null) {
            Params.Owner.AddRuntimeWarning("Analysis Case input (index: " + i
              + ") is null and has been ignored");
            continue;
          }

          if (ghTyp.Value is GsaAnalysisCaseGoo goo) {
            cases.Add(goo.Value.Duplicate());
          } else {
            string type = ghTyp.Value.GetType().ToString();
            type = type.Replace("GsaGH.Parameters.", string.Empty);
            type = type.Replace("Goo", string.Empty);
            Params.Owner.AddRuntimeError("Unable to convert Analysis Case input parameter of type "
              + type + " to GsaAnalysisCase");
            return;
          }
        }
      }

      if (cases == null) {
        this.AddRuntimeRemark(
          "Default Task has been created; it will by default contain all cases found in model");
      }

      AnalysisTask task = null;
      switch (_type) {
        case AnalysisTaskType.Static:
          task = AnalysisTaskFactory.CreateStaticAnalysisTask(name);
          break;

        case AnalysisTaskType.StaticPDelta:
          switch (_selectedItems[1]) {
            case "Own":
              task = AnalysisTaskFactory.CreateStaticPDeltaAnalysisTask(name, new GeometricStiffnessFromOwnLoad());
              break;

            case "Load case":
              string caseDescription = string.Empty;
              da.GetData(2, ref caseDescription);
              task = AnalysisTaskFactory.CreateStaticPDeltaAnalysisTask(name, new GeometricStiffnessFromLoadCase(caseDescription));
              break;

            case "Result case":
              int resultCase = 0;
              da.GetData(2, ref resultCase);
              task = AnalysisTaskFactory.CreateStaticPDeltaAnalysisTask(name, new GeometricStiffnessFromResultCase(resultCase));
              break;
          }
          break;

        case AnalysisTaskType.Footfall:

          string responseNodes = string.Empty;
          da.GetData(2, ref responseNodes);

          int i = 2;
          string excitationNodes = string.Empty;
          if (_selectedItems[i++] == "Self excitation") {
            da.GetData(i, ref excitationNodes);
          }

          int numberOfFootfalls = 0;
          da.GetData(i, ref numberOfFootfalls);

          switch (_selectedItems[1]) {
            case "Self excitation ":
              break; 

            case "Rigorous excitation":
              break;

            case "Rigorous excitation?":
              break;

            case "Fast excitation":
              break;
          }

          NumberOfFootfalls numberofFootfalls = new ConstantFootfallsForAllModes(numberOfFootfalls);
          WeightingOption frequencyWeightingCurve = WeightingOption.Wg;
          var excitationForces = new ExcitationForces();
          DampingOption dampingOption = new ConstantDampingOption();

          var parameter = new FootfallAnalysisTaskParameter() {
            ModalAnalysisTaskId = 1,
            ExcitationMethod = ExcitationMethod.SelfExcitation,
            ResponseNodes = "",
            ExcitationNodes = excitationNodes,
            NumberofFootfalls = numberofFootfalls,
            WalkerMass = 76,
            ResponseDirection = ResponseDirection.Z,
            FrequencyWeightingCurve = frequencyWeightingCurve,
            ExcitationForces = excitationForces,
            DampingOption = dampingOption,
          };
          task = AnalysisTaskFactory.CreateFootfallAnalysisTask(name, parameter);
          break;

        default:
          this.AddRuntimeWarning("It is currently not possible to create Analysis Tasks of type " + _type);
          break;
      }

      var gsaAnalysisTask = new GsaAnalysisTask() {
        Cases = cases,
        ApiTask = task,
        Id = id
      };

      da.SetData(0, new GsaAnalysisTaskGoo(gsaAnalysisTask));
    }

    protected override void UpdateUIFromSelectedItems() {
      _type = _solverTypes[_selectedItems[0]];

      base.UpdateUIFromSelectedItems();
    }

    private void UpdateParameters() {
      IGH_Param casesParam;
      switch (_type) {
        case AnalysisTaskType.Static:
          casesParam = Params.Input[Params.Input.Count - 1];

          while (Params.Input.Count > 2) {
            Params.UnregisterInputParameter(Params.Input[2], true);
          }

          _casesParamIndex = 2;
          Params.RegisterInputParam(casesParam);
          break;

        case AnalysisTaskType.StaticPDelta:
          casesParam = Params.Input[Params.Input.Count - 1];

          while (Params.Input.Count > 2) {
            Params.UnregisterInputParameter(Params.Input[2], true);
          }

          switch (_selectedItems[1]) {
            case "Own":
            default:
              _casesParamIndex = 2;
              break;

            case "Load case":
              _casesParamIndex = 3;
              Params.RegisterInputParam(new Param_String());
              break;

            case "Result case":
              _casesParamIndex = 3;
              Params.RegisterInputParam(new Param_Integer());
              break;
          }

          Params.RegisterInputParam(casesParam);
          break;

        case AnalysisTaskType.Footfall:
          casesParam = Params.Input[Params.Input.Count - 1];

          while (Params.Input.Count > 2) {
            Params.UnregisterInputParameter(Params.Input[2], true);
          }

          // response nodes
          Params.RegisterInputParam(new Param_String());

          // excitation nodes
          if (_selectedItems[1] != "Self excitation") {
            _casesParamIndex = 9;
            Params.RegisterInputParam(new Param_String());
          } else {
            _casesParamIndex = 8;
          }

          // number of footfalls
          Params.RegisterInputParam(new Param_Integer());
          // walker
          Params.RegisterInputParam(new Param_Number());
          // direction of responses
          Params.RegisterInputParam(new Param_String());
          // frequency weighting curve
          Params.RegisterInputParam(new Param_String());
          // excitation forces
          Params.RegisterInputParam(new Param_String());

          Params.RegisterInputParam(casesParam);
          break;

        default:
          break;
      }
    }

    private void UpdateDropdownItems() {
      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      switch (_type) {
        case AnalysisTaskType.StaticPDelta:
          _spacerDescriptions = new List<string>(new[] {
            "Solver",
            "P-delta Case"
          });

          _dropDownItems.Add(_solverTypes.Keys.ToList());
          _selectedItems.Add(_dropDownItems[0][1]);

          _dropDownItems.Add(new List<string>() {
            "Own",
            "Load case",
            "Result case"
          });
          _selectedItems.Add("Own");

          break;

        case AnalysisTaskType.Footfall:
          _spacerDescriptions = new List<string>(new[] {
            "Solver",
            "Method"
          });

          _dropDownItems.Add(_solverTypes.Keys.ToList());
          _selectedItems.Add(_dropDownItems[0][2]);

          _dropDownItems.Add(new List<string>() {
            "Self excitation",
            "Rigorous excitation",
            "Rigorous excitation?",
            "Fast excitation"
          });
          _selectedItems.Add("Self excitation");

          break;

        case AnalysisTaskType.Static:
        default:
          _spacerDescriptions = new List<string>(new[] {
            "Solver",
          });

          _dropDownItems.Add(_solverTypes.Keys.ToList());
          _selectedItems.Add(_dropDownItems[0][0]);
          break;
      }

      ReDrawComponent();
    }

    private void ReDrawComponent() {
      var pivot = new PointF(Attributes.Pivot.X, Attributes.Pivot.Y);
      base.CreateAttributes();
      Attributes.Pivot = pivot;
      Attributes.ExpireLayout();
      Attributes.PerformLayout();
    }
  }
}
