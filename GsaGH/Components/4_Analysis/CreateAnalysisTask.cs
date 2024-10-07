﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using GH_IO.Serialization;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;

using GsaAPI;

using GsaGH.Data;
using GsaGH.Helpers;
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

    private enum PDeltaCases {
      Own,
      LoadCase,
      ResultCase,
    }

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

    private readonly InputAttributes _analysisCaseInputAttributes = new InputAttributes {
      NickName = "ΣAs",
      Name = "Analysis Cases",
      Description = "List of GSA Analysis Cases (if left empty, all load cases in model will be added)",
      Access = GH_ParamAccess.list,
      Optional = true,
    };

    private readonly Dictionary<ExcitationMethod, string> _excitationMethod = new Dictionary<ExcitationMethod, string> {
      { ExcitationMethod.SelfExcitation, "Self excitation" },
      { ExcitationMethod.FullExcitationRigorous, "Rigorous excitation" },
      { ExcitationMethod.FullExcitationFastExcludingResponseNode, "Fast rigorous exc." },
      { ExcitationMethod.FullExcitationFast, "Fast excitation" },
    };

    private readonly InputAttributes _loadCaseAttribute = new InputAttributes {
      NickName = "LC",
      Name = "Load case",
      Description = "Load case definition (e.g. 1.2L1 + 1.2L2)",
      Access = GH_ParamAccess.item,
      Optional = false,
    };

    private readonly Dictionary<PDeltaCases, string> _pDeltaCases = new Dictionary<PDeltaCases, string> {
      { PDeltaCases.Own, "Own" },
      { PDeltaCases.LoadCase, "Load case" },
      { PDeltaCases.ResultCase, "Result case" },
    };

    private readonly InputAttributes _resultCaseAttributes = new InputAttributes {
      NickName = "RC",
      Name = "Result case",
      Description = "The result case that forms the basis for the geometric stiffness",
      Access = GH_ParamAccess.item,
      Optional = false,
    };
    private int _casesParamIndex = 2;

    private readonly FootfallInputManager _footfallInputManager;
    private AnalysisTaskType _type = AnalysisTaskType.Static;
    public const string _unableToConvertResponseDirectionInputMessage = "Unable to convert response direction input";
    public const string _unableToConvertWeightOptionInputMessage = "Unable to convert frequency weighting curve input";
    public const string _unableToConvertsExcitationForcesInputMessage
      = "Unable to convert excitation forces (DLFs) input";

    public CreateAnalysisTask() : base($"Create {GsaAnalysisTaskGoo.Name}",
      GsaAnalysisTaskGoo.NickName.Replace(" ", string.Empty), $"Create a {GsaAnalysisTaskGoo.Description}",
      CategoryName.Name(), SubCategoryName.Cat4()) {
      Hidden = true;
      _footfallInputManager = new FootfallInputManager(true);
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
          SetInputAttributes(_casesParamIndex, _analysisCaseInputAttributes);
          PDeltaCases selectedPDeltaCase = GetKeyFromMatchingValue(_selectedItems[1]);

          switch (selectedPDeltaCase) {
            case PDeltaCases.Own: break;
            case PDeltaCases.LoadCase:
              SetInputAttributes(2, _loadCaseAttribute);
              break;
            case PDeltaCases.ResultCase:
              SetInputAttributes(2, _resultCaseAttributes);
              break;
            default: break;
          }

          break;

        case AnalysisTaskType.Footfall:
          SetFootfallInput();
          break;
        case AnalysisTaskType.Static:
        default:
          SetInputAttributes(_casesParamIndex, _analysisCaseInputAttributes);
          break;
      }
    }

    private PDeltaCases GetKeyFromMatchingValue(string valueToMatch) {
      return _pDeltaCases.TryGetKeyFrom(valueToMatch);
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
      pManager.AddIntegerParameter("Task ID", "ID", "The Task number of the Analysis Task", GH_ParamAccess.item);
      pManager.AddTextParameter("Name", "Na", "Task Name", GH_ParamAccess.item);
      pManager.AddGenericParameter(_analysisCaseInputAttributes.Name, _analysisCaseInputAttributes.NickName,
        _analysisCaseInputAttributes.Description, _analysisCaseInputAttributes.Access);
      pManager[0].Optional = true;
      pManager[1].Optional = true;
      pManager[2].Optional = _analysisCaseInputAttributes.Optional;
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
            GH_ObjectWrapper ghTypeWrapper = ghTypes[i];
            if (ghTypeWrapper == null) {
              this.AddRuntimeWarning($"Analysis Case input (index: {i}) is null and has been ignored");
              continue;
            }

            if (ghTypeWrapper.Value is GsaAnalysisCaseGoo goo) {
              cases.Add(goo.Value.Duplicate());
            } else {
              UnsupportedValueError(ghTypeWrapper);
              return;
            }
          }
        }

        if (cases == null) {
          this.AddRuntimeRemark("Default Task has been created; it will by default contain all cases found in model");
        }
      } else {
        cases = new List<GsaAnalysisCase>();
        var footfallAnalysisCase = new GsaAnalysisCase {
          Name = name,
          Definition = name,
        };
        cases.Add(footfallAnalysisCase);
      }

      AnalysisTask task = null;
      switch (_type) {
        case AnalysisTaskType.Static:
          task = AnalysisTaskFactory.CreateStaticAnalysisTask(name);
          break;

        case AnalysisTaskType.StaticPDelta:
          PDeltaCases selectedPDeltaCase = _pDeltaCases.FirstOrDefault(x => x.Value.Equals(_selectedItems[1])).Key;

          switch (selectedPDeltaCase) {
            case PDeltaCases.Own:
              task = AnalysisTaskFactory.CreateStaticPDeltaAnalysisTask(name, new GeometricStiffnessFromOwnLoad());
              break;
            case PDeltaCases.LoadCase:
              string caseDescription = string.Empty;
              da.GetData(2, ref caseDescription);
              task = AnalysisTaskFactory.CreateStaticPDeltaAnalysisTask(name,
                new GeometricStiffnessFromLoadCase(caseDescription));
              break;
            case PDeltaCases.ResultCase:
              int resultCase = 0;
              da.GetData(2, ref resultCase);
              task = AnalysisTaskFactory.CreateStaticPDeltaAnalysisTask(name,
                new GeometricStiffnessFromResultCase(resultCase));
              break;
            default: break;
          }

          break;

        case AnalysisTaskType.Footfall:
          int analysisTaskId = 0;
          da.GetData(2, ref analysisTaskId);

          var parameter = new FootfallAnalysisTaskParameter {
            ModalAnalysisTaskId = analysisTaskId,
          };

          string responseNodes = "All";
          if (da.GetData(3, ref responseNodes)) {
            parameter.ResponseNodes = responseNodes;
          }

          int i = 4;
          string excitationNodes = "All";
          if (!IsSelfExcitationSelected()) {
            if (da.GetData(i++, ref excitationNodes)) {
              parameter.ExcitationNodes = excitationNodes;
            }
          }

          int numberOfFootfalls = 0;
          if (da.GetData(i++, ref numberOfFootfalls)) {
            NumberOfFootfalls numberofFootfalls = new ConstantFootfallsForAllModes(numberOfFootfalls);
            parameter.NumberOfFootfalls = numberofFootfalls;
          }

          double walkerMass = 0;
          if (da.GetData(i++, ref walkerMass)) {
            parameter.WalkerMass = walkerMass;
          }

          var ghTyp = new GH_ObjectWrapper();
          if (da.GetData(i++, ref ghTyp)) {
            if (!HasValidDirection(ghTyp, out ResponseDirection responseDirection)) {
              UnableToConvertDirection();
              return;
            }

            parameter.ResponseDirection = responseDirection;
          }

          int weightingOption = 0;
          if (da.GetData(i++, ref weightingOption)) {
            if (!HasValidFrequencyWeightingOption(weightingOption, out WeightingOption frequencyWeightingCurve)) {
              UnableToConvertWeightOption();
              return;
            }

            parameter.FrequencyWeightingCurve = frequencyWeightingCurve;
          }

          int excitationForceOption = 0;
          if (da.GetData(i++, ref excitationForceOption)) {
            if (!HasValidExcitationForces(excitationForceOption, out ExcitationForces excitationForces)) {
              UnableToConvertExcitationForces();
              return;
            }

            parameter.ExcitationForces = excitationForces;
          }

          double damping = 0;
          if (da.GetData(i++, ref damping)) {
            parameter.DampingOption = new ConstantDampingOption {
              ConstantDamping = damping,
            };
          }

          parameter.ExcitationMethod = _excitationMethod.FirstOrDefault(x => x.Value.Equals(_selectedItems[1])).Key;

          task = AnalysisTaskFactory.CreateFootfallAnalysisTask(name, parameter);
          break;

        default:
          this.AddRuntimeWarning($"It is currently not possible to create Analysis Tasks of type {_type}");
          break;
      }

      var gsaAnalysisTask = new GsaAnalysisTask {
        Cases = cases,
        ApiTask = task,
        Id = id,
      };

      da.SetData(0, new GsaAnalysisTaskGoo(gsaAnalysisTask));
    }

    private static bool HasValidFrequencyWeightingOption(
      int weightingOption, out WeightingOption frequencyWeightingCurve) {
      bool hasValidFrequencyWeighting = true;
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
          frequencyWeightingCurve = WeightingOption.Wd;
          hasValidFrequencyWeighting = false;
          break;
      }

      return hasValidFrequencyWeighting;
    }

    private static bool HasValidExcitationForces(int excitationForceOption, out ExcitationForces excitationForces) {
      bool hasValidValue = true;
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
          hasValidValue = false;
          excitationForces = null;
          break;
      }

      return hasValidValue;
    }

    private static bool HasValidDirection(GH_ObjectWrapper ghDirection, out ResponseDirection responseDirection) {
      bool hasDirection;
      responseDirection = default;
      switch (ghDirection.Value) {
        case GH_Integer intDirection:

          hasDirection = HasDirectionFromString(out responseDirection, intDirection.Value.ToString());
          break;
        case GH_String stringDirection:
          hasDirection = HasDirectionFromString(out responseDirection, stringDirection.Value);
          break;
        case GH_Number numberDirection:
          hasDirection = HasDirectionFromString(out responseDirection, numberDirection.Value.ToString());
          break;
        default:
          hasDirection = false;
          break;
      }

      return hasDirection;
    }

    private static bool HasDirectionFromString(out ResponseDirection responseDirection, string ghString) {
      switch (ghString.Trim().ToUpper()) {
        case "1":
        case "Z":
          responseDirection = ResponseDirection.Z;
          break;

        case "2":
        case "X":
          responseDirection = ResponseDirection.X;
          break;

        case "3":
        case "Y":
          responseDirection = ResponseDirection.Y;
          break;

        case "4":
        case "XY":
          responseDirection = ResponseDirection.XY;
          break;

        default:
          responseDirection = ResponseDirection.Z;
          return false;
      }

      return true;
    }

    private void UnableToConvertDirection() {
      this.AddRuntimeError(_unableToConvertResponseDirectionInputMessage);
    }

    private void UnableToConvertWeightOption() {
      this.AddRuntimeError(_unableToConvertWeightOptionInputMessage);
    }

    private void UnableToConvertExcitationForces() {
      this.AddRuntimeError(_unableToConvertsExcitationForcesInputMessage);
    }

    protected override void UpdateUIFromSelectedItems() {
      _type = _solverTypes[_selectedItems[0]];
      //UpdateParameters();

      base.UpdateUIFromSelectedItems();
    }

    private void UpdateParameters() {
      UnregisterInputsOverTwo();
      switch (_type) {
        case AnalysisTaskType.Static:
          _casesParamIndex = 2;
          Params.RegisterInputParam(new Param_GenericObject());
          break;

        case AnalysisTaskType.StaticPDelta:
          PDeltaCases selectedPDeltaCase = _pDeltaCases.FirstOrDefault(x => x.Value.Equals(_selectedItems[1])).Key;

          switch (selectedPDeltaCase) {
            case PDeltaCases.Own:
              _casesParamIndex = 2;
              break;
            case PDeltaCases.LoadCase:
              _casesParamIndex = 3;
              Params.RegisterInputParam(new Param_String());
              break;
            case PDeltaCases.ResultCase:
              _casesParamIndex = 3;
              Params.RegisterInputParam(new Param_Integer());
              break;
            default: break;
          }

          Params.RegisterInputParam(new Param_GenericObject());
          break;

        case AnalysisTaskType.Footfall:
          Params.RegisterInputParam(FootfallInputManager._modalAnalysisTaskAttributes.ParamType);
          Params.RegisterInputParam(FootfallInputManager._responseNodesAttributes.ParamType);
          if (!IsSelfExcitationSelected()) {
            Params.RegisterInputParam(new Param_String());
          }

          Params.RegisterInputParam(FootfallInputManager._numberOfFootfallsAttributes.ParamType);
          Params.RegisterInputParam(FootfallInputManager._walkerAttributes.ParamType);
          Params.RegisterInputParam(FootfallInputManager._responseDirectionAttributes.ParamType);
          Params.RegisterInputParam(FootfallInputManager._frequencyWeightingCurveAttributes.ParamType);
          Params.RegisterInputParam(FootfallInputManager._excitationForcesAttributes.ParamType);
          Params.RegisterInputParam(FootfallInputManager._dampingAttributes.ParamType);
          _casesParamIndex = !IsSelfExcitationSelected() ? 9 : 8;

          break;
        default: break;
      }
    }

    private void UnregisterInputsOverTwo() { ReplaceParam.UnregisterInputsFrom(Params, 2); }

    private void UpdateDropdownItems() {
      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();
      _spacerDescriptions = new List<string>(new[] {
        "Solver",
      });

      switch (_type) {
        case AnalysisTaskType.StaticPDelta:
          _spacerDescriptions.Add("P-delta Case");

          _dropDownItems.Add(_solverTypes.Keys.ToList());
          _selectedItems.Add(_dropDownItems[0][1]);

          _dropDownItems.Add(_pDeltaCases.Values.ToList());
          _selectedItems.Add(_pDeltaCases[PDeltaCases.Own]);

          break;

        case AnalysisTaskType.Footfall:
          _spacerDescriptions.Add("Method");

          _dropDownItems.Add(_solverTypes.Keys.ToList());
          _selectedItems.Add(_dropDownItems[0][2]);

          _dropDownItems.Add(_excitationMethod.Values.ToList());
          _selectedItems.Add(_excitationMethod[ExcitationMethod.SelfExcitation]);

          break;

        case AnalysisTaskType.Static:
        default:
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

    private bool IsSelfExcitationSelected() { return _selectedItems[1] == "Self excitation"; }

    private void SetInputAttributes(int index, InputAttributes inputAttributes) {
      IGH_Param ghParam = Params.Input[index];
      ghParam.NickName = inputAttributes.NickName;
      ghParam.Name = inputAttributes.Name;
      ghParam.Description = inputAttributes.Description;
      ghParam.Access = inputAttributes.Access;
      ghParam.Optional = inputAttributes.Optional;
    }

    private void UnsupportedValueError(GH_ObjectWrapper ghTypeWrapper) {
      string type = ReplaceParam.UnsupportedValue(ghTypeWrapper);
      Params.Owner.AddRuntimeError(
        $"Unable to convert Analysis Case input parameter of type {type} to GsaAnalysisCase");
    }

    private void SetFootfallInput() {
      List<InputAttributes> inputs = _footfallInputManager.GetInputsForSelfExcitation(!IsSelfExcitationSelected());
      for (int j = 0; j < inputs.Count; j++) {
        SetInputAttributes(j + 2, inputs[j]);
      }
    }
  }
}
