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
    private readonly InputAttributes _analysisCaseInputAttributes = new InputAttributes() {
      NickName = "ΣAs",
      Name = "Analysis Cases",
      Description = "List of GSA Analysis Cases (if left empty, all load cases in model will be added)",
      Access = GH_ParamAccess.list,
      Optional = true
    };
    private readonly InputAttributes _loadCaseAttribute = new InputAttributes() {
      NickName = "LC",
      Name = "Load case",
      Description = "Load case definition (e.g. 1.2L1 + 1.2L2)",
      Access = GH_ParamAccess.item,
      Optional = false
    };
    private readonly InputAttributes _resultCaseAttributes = new InputAttributes() {
      NickName = "RC",
      Name = "Result case",
      Description = "The result case that forms the basis for the geometric stiffness",
      Access = GH_ParamAccess.item,
      Optional = false
    };

    public CreateAnalysisTask() : base($"Create {GsaAnalysisTaskGoo.Name}",
      GsaAnalysisTaskGoo.NickName.Replace(" ", string.Empty), $"Create a {GsaAnalysisTaskGoo.Description}",
      CategoryName.Name(), SubCategoryName.Cat4()) {
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

    private void SetCaseInput(int index, InputAttributes inputAttributes) {
      IGH_Param ghParam = Params.Input[index];
      ghParam.NickName = inputAttributes.NickName;
      ghParam.Name = inputAttributes.Name;
      ghParam.Description = inputAttributes.Description;
      ghParam.Access = inputAttributes.Access;
      ghParam.Optional = inputAttributes.Optional;
    }

    public override void VariableParameterMaintenance() {
      switch (_type) {
        case AnalysisTaskType.StaticPDelta:
          SetCaseInput(_casesParamIndex, _analysisCaseInputAttributes);
          switch (_selectedItems[1]) {
            case "Own":
            default:
              break;
            case "Load case":
              SetCaseInput(2, _loadCaseAttribute);
              break;
            case "Result case":
              SetCaseInput(2, _resultCaseAttributes);
              break;
          }

          break;

        case AnalysisTaskType.Footfall:
          var footFallManager = new FootfallInputManager(_selectedItems[1] != "Self excitation");
          List<InputAttributes> attributes = footFallManager.GetInputs();
          for (int j = 0; j < attributes.Count; j++) {
            SetCaseInput(j + 2, attributes[j]);
          }

          break;
        case AnalysisTaskType.Static:
        default:
          SetCaseInput(_casesParamIndex, _analysisCaseInputAttributes);
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
      pManager.AddIntegerParameter("Task ID", "ID", "The Task number of the Analysis Task", GH_ParamAccess.item);
      pManager.AddTextParameter("Name", "Na", "Task Name", GH_ParamAccess.item);
      pManager.AddGenericParameter(_analysisCaseInputAttributes.Name, _analysisCaseInputAttributes.NickName,
        _analysisCaseInputAttributes.Description, GH_ParamAccess.list);
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
              Params.Owner.AddRuntimeWarning($"Analysis Case input (index: {i}) is null and has been ignored");
              continue;
            }

            if (ghTyp.Value is GsaAnalysisCaseGoo goo) {
              cases.Add(goo.Value.Duplicate());
            } else {
              string type = ghTyp.Value.GetType().ToString();
              type = type.Replace("GsaGH.Parameters.", string.Empty);
              type = type.Replace("Goo", string.Empty);
              Params.Owner.AddRuntimeError(
                $"Unable to convert Analysis Case input parameter of type {type} to GsaAnalysisCase");
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
          Definition = name
        };
        cases.Add(footfallAnalysisCase);
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
              task = AnalysisTaskFactory.CreateStaticPDeltaAnalysisTask(name,
                new GeometricStiffnessFromLoadCase(caseDescription));
              break;

            case "Result case":
              int resultCase = 0;
              da.GetData(2, ref resultCase);
              task = AnalysisTaskFactory.CreateStaticPDeltaAnalysisTask(name,
                new GeometricStiffnessFromResultCase(resultCase));
              break;
          }

          break;

        case AnalysisTaskType.Footfall:
          int analysisTaskId = 0;
          da.GetData(2, ref analysisTaskId);

          var parameter = new FootfallAnalysisTaskParameter() {
            ModalAnalysisTaskId = analysisTaskId,
          };

          string responseNodes = "All";
          if (da.GetData(3, ref responseNodes)) {
            parameter.ResponseNodes = responseNodes;
          }

          int i = 4;
          string excitationNodes = "All";
          if (_selectedItems[1] != "Self excitation") {
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
            ResponseDirection responseDirection = ResponseDirection.Z;
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
                    this.AddRuntimeError("Unable to convert response direction input");
                    return;
                }

                break;

              default:
                this.AddRuntimeError("Unable to convert response direction input");
                return;
            }

            parameter.ResponseDirection = responseDirection;
          }

          int weightingOption = 0;
          if (da.GetData(i++, ref weightingOption)) {
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

            parameter.FrequencyWeightingCurve = frequencyWeightingCurve;
          }

          int excitationForceOption = 0;
          if (da.GetData(i++, ref excitationForceOption)) {
            ExcitationForces excitationForces = null;
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

            parameter.ExcitationForces = excitationForces;
          }

          double damping = 0;
          if (da.GetData(i++, ref damping)) {
            parameter.DampingOption = new ConstantDampingOption() { ConstantDamping = damping };
          }

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
          this.AddRuntimeWarning($"It is currently not possible to create Analysis Tasks of type {_type}");
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
      UpdateParameters();

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

        default: break;
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
            "Fast rigorous exc.",
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

namespace GsaGH.Data {
}
