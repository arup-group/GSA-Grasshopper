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
          Params.Input[_casesParamIndex].NickName = "ΣAs";
          Params.Input[_casesParamIndex].Name = "Analysis Cases";
          Params.Input[_casesParamIndex].Description = "List of GSA Analysis Cases (if left empty, all load cases in model will be added)";
          Params.Input[_casesParamIndex].Access = GH_ParamAccess.list;
          Params.Input[_casesParamIndex].Optional = true;

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
          Params.Input[2].NickName = "T";
          Params.Input[2].Name = "Modal analysis task";
          Params.Input[2].Description = "Modal or Ritz analysis task";
          Params.Input[2].Access = GH_ParamAccess.item;
          Params.Input[2].Optional = false;

          Params.Input[3].NickName = "RN";
          Params.Input[3].Name = "Response nodes";
          Params.Input[3].Description = "Node list to define the nodes that the responses will be calculated in the analysis";
          Params.Input[3].Access = GH_ParamAccess.item;
          Params.Input[3].Optional = true;

          if (_selectedItems[1] != "Self excitation") {
            Params.Input[4].NickName = "EN";
            Params.Input[4].Name = "Excitation nodes";
            Params.Input[4].Description = "Node list to define the nodes that will be excited for evaluation of the response of the response nodes";
            Params.Input[4].Access = GH_ParamAccess.item;
            Params.Input[4].Optional = true;
          }

          int i = _selectedItems[1] != "Self excitation" ? 5 : 4;
          Params.Input[i].NickName = "N";
          Params.Input[i].Name = "Number of footfalls";
          Params.Input[i].Description = "The number of footfalls to be considered in the analysis";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          Params.Input[++i].NickName = "W";
          Params.Input[i].Name = "Walker";
          Params.Input[i].Description = "The mass representing a sample walker";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          Params.Input[++i].NickName = "D";
          Params.Input[i].Name = "Direction of responses";
          Params.Input[i].Description = "The direction of response in the GSA global axis direction." + "\nInput either text string or a integer:" + "\n 1 : Z (vertical)" + "\n 2 : X" + "\n 3 : Y" + "\n4 : XY (horizontal)";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          Params.Input[++i].NickName = "F";
          Params.Input[i].Name = "Frequency Weighting Curve";
          Params.Input[i].Description = "The Frequency Weighting Curve (FWC) is used in calculating the response factors." + "\nInput the corresponding integer:" + "\n1 : (Freq. Weighting) Wb (BS6472-1:2008)" + "\n2 : (Freq. Weighting) Wd (BS6472-1:2008)" + "\n3 : (Freq. Weighting) Wg (BS6472:1992)";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          Params.Input[++i].NickName = "EF";
          Params.Input[i].Name = "Excitation forces (DLFs)";
          Params.Input[i].Description = "This defines the way of the structure to be excited (the dynamic Load Factor to be used)" +
            "\nInput the corresponding integer:" +
            "\n1 : Walking on floor (AISC SDGS11)" +
            "\n2 : Walking on floor (AISC SDGS11 2nd ed)" +
            "\n3 : Walking on floor (CCIP-016)" +
            "\n4 : Walking on floor (SCI P354)" +
            "\n5 : Walking on stair (AISC SDGS11 2nd ed)" +
            "\n6 : Walking on stair (Arup)" +
            "\n7 : Walking on stair (SCI P354)" +
            "\n8 : Running on floor (AISC SDGS11 2nd)";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          Params.Input[++i].NickName = "DC";
          Params.Input[i].Name = "Constant Damping";
          Params.Input[i].Description = "Constant damping in percent";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;
          break;

        case AnalysisTaskType.Static:
        default:
          // do nothing
          Params.Input[_casesParamIndex].NickName = "ΣAs";
          Params.Input[_casesParamIndex].Name = "Analysis Cases";
          Params.Input[_casesParamIndex].Description = "List of GSA Analysis Cases (if left empty, all load cases in model will be added)";
          Params.Input[_casesParamIndex].Access = GH_ParamAccess.list;
          Params.Input[_casesParamIndex].Optional = true;
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
      if (_type != AnalysisTaskType.Footfall) {
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
          int analysisTaskId = 0;
          da.GetData(2, ref analysisTaskId);

          string responseNodes = "All";
          da.GetData(3, ref responseNodes);

          int i = 3;
          string excitationNodes = "All";
          if (_selectedItems[1] == "Self excitation") {
            da.GetData(i++, ref excitationNodes);
          }

          int numberOfFootfalls = 0;
          da.GetData(i++, ref numberOfFootfalls);

          double walkerMass = 0;
          da.GetData(i++, ref walkerMass);

          ResponseDirection responseDirection = ResponseDirection.Y; // why do we need to initialize it here?
          var ghTyp = new GH_ObjectWrapper();
          if (da.GetData(i++, ref ghTyp)) {
            switch (ghTyp.Value) {
              case GH_Integer ghInt:
                switch (ghInt.Value) {
                  case 1:
                    responseDirection = ResponseDirection.Z;
                    break;

                  case 2:
                    responseDirection = ResponseDirection.X;
                    break;

                  case 3:
                    responseDirection = ResponseDirection.Y;
                    break;

                  case 4:
                    responseDirection = ResponseDirection.XY;
                    break;

                  default:
                    this.AddRuntimeError("Unable to convert response direction input");
                    return;
                }
                break;

              case GH_String ghString:
                switch (ghString.Value.Trim().ToUpper()) {
                  case "Z":
                    responseDirection = ResponseDirection.Z;
                    break;

                  case "X":
                    responseDirection = ResponseDirection.X;
                    break;

                  case "Y":
                    responseDirection = ResponseDirection.Y;
                    break;

                  case "XY":
                    responseDirection = ResponseDirection.XY;
                    break;

                  default:
                    this.AddRuntimeError("Unable to convert response direction input");
                    return;
                }
                break;

              default:
                this.AddRuntimeError("Unable to convert response direction input");
                return;
            }
          }

          int weightingOption = 0;
          da.GetData(i++, ref weightingOption);
          WeightingOption frequencyWeightingCurve;
          switch (weightingOption) {
            case 1:
              frequencyWeightingCurve = WeightingOption.Wb;
              break;

            case 2:
              frequencyWeightingCurve = WeightingOption.Wd;
              break;

            case 3:
              frequencyWeightingCurve = WeightingOption.Wg;
              break;

            default:
              this.AddRuntimeError("Unable to convert frequency weighting curve input");
              return;
          }

          int excitationForceOption = 0;
          da.GetData(i++, ref excitationForceOption);
          ExcitationForces excitationForces;
          switch (excitationForceOption) {
            case 1: // walking on floor AISC
              excitationForces = new WalkingOnFloorAISC();
              break;

            case 2: // walking on floor AISC 2nd ed
              excitationForces = new WalkingOnFloorAISC2ndEdition();
              break;

            case 3: // walking on floor CCIP
              excitationForces = new WalkingOnFloorCCIP();
              break;

            case 4: // walking on floor SCI
              excitationForces = new WalkingOnFloorSCI();
              break;

            case 5: // walking on stair AISC 2nd ed
              excitationForces = new WalkingOnStairAISC2ndEdition();
              break;

            case 6: // walking on stair Arup
              excitationForces = new WalkingOnStairArup();
              break;

            case 7: // walking on stair SCI
              excitationForces = new WalkingOnStairSCI();
              break;

            case 8: // running on floor AISC 2nd ed
              excitationForces = new RunningOnFloorAISC2ndEdition();
              break;

            default:
              this.AddRuntimeError("Unable to convert excitation forces (DLFs) input");
              return;
          }

          double damping = 0;
          da.GetData(i++, ref damping);

          NumberOfFootfalls numberofFootfalls = new ConstantFootfallsForAllModes(numberOfFootfalls);
          DampingOption dampingOption = new ConstantDampingOption() {
            ConstantDamping = damping
          };
          var parameter = new FootfallAnalysisTaskParameter() {
            ModalAnalysisTaskId = analysisTaskId,
            ResponseNodes = responseNodes,
            ExcitationNodes = excitationNodes,
            NumberofFootfalls = numberofFootfalls,
            WalkerMass = walkerMass,
            ResponseDirection = responseDirection,
            FrequencyWeightingCurve = frequencyWeightingCurve,
            ExcitationForces = excitationForces,
            DampingOption = dampingOption,
          };

          switch (_selectedItems[1]) {
            case "Self excitation":
              parameter.ExcitationMethod = ExcitationMethod.SelfExcitation;
              break;

            case "Rigorous excitation":
              parameter.ExcitationMethod = ExcitationMethod.FullExcitationRigorous;
              break;

            case "Fast rigorous exc.":
              parameter.ExcitationMethod = ExcitationMethod.FullExcitationFastExcludingResponseNode;
              break;

            case "Fast excitation":
              parameter.ExcitationMethod = ExcitationMethod.FullExcitationFast;
              break;
          }

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
      switch (_type) {
        case AnalysisTaskType.Static:
          while (Params.Input.Count > 2) {
            Params.UnregisterInputParameter(Params.Input[2], true);
          }

          _casesParamIndex = 2;
          Params.RegisterInputParam(new Param_GenericObject());
          break;

        case AnalysisTaskType.StaticPDelta:
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

          Params.RegisterInputParam(new Param_GenericObject());
          break;

        case AnalysisTaskType.Footfall:
          while (Params.Input.Count > 2) {
            Params.UnregisterInputParameter(Params.Input[2], true);
          }

          // modal analysis task
          Params.RegisterInputParam(new Param_Integer());

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
          Params.RegisterInputParam(new Param_GenericObject());
          // frequency weighting curve
          Params.RegisterInputParam(new Param_Integer());
          // excitation forces
          Params.RegisterInputParam(new Param_Integer());
          // damping constant
          Params.RegisterInputParam(new Param_Number());
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
            "Fast Rigorous exc.",
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
