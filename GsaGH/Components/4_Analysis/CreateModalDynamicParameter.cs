using System;
using System.Collections.Generic;
using System.Drawing;

using Grasshopper.Kernel;

using GsaAPI;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;
using GsaGH.Parameters.Enums;

using System.Linq;
using Grasshopper.Kernel.Parameters;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to create a GSA Analysis Case
  /// </summary>
  public class CreateModalDynamicParameter : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("75bf6454-92c4-4a3c-8abf-75f1d449cb85");
    public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateModalDynamicParameter;
    private ModeCalculationMethod _modeMethod = ModeCalculationMethod.NumberOfMode;
    public CreateModalDynamicParameter() : base(
      $"Create {GsaModalDynamicGoo.Name}",
      GsaModalDynamicGoo.NickName.Replace(" ", string.Empty),
      $"Create {GsaModalDynamicGoo.Description}", CategoryName.Name(), SubCategoryName.Cat4()) {
      Hidden = true;
    }

    private static readonly IReadOnlyDictionary<ModeCalculationMethod, string> _modeCalculationMethod
     = new Dictionary<ModeCalculationMethod, string> {
        { ModeCalculationMethod.NumberOfMode, "Number of modes" },
        { ModeCalculationMethod.FrquencyRange, "Frequency range" },
        { ModeCalculationMethod.TargetMassRatio, "Target mass ratio" },
     };

    private static readonly IReadOnlyDictionary<ModalMassOption, string> _massOptions
    = new Dictionary<ModalMassOption, string> {
        { ModalMassOption.LumpMassAtNode, "Mass at node" },
        { ModalMassOption.MassFromElementShapeFunction, "Mass from shape function" },
        { ModalMassOption.NodalMass, "Nodal mass" },
    };

    private static readonly IReadOnlyDictionary<Direction, string> _massDirection
    = new Dictionary<Direction, string> {
        { Direction.X, "X" },
        { Direction.Y, "Y" },
        { Direction.Z, "Z" },
    };

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaAnalysisTaskParameter(), GsaAnalysisTaskGoo.Name,
       GsaAnalysisTaskGoo.NickName,
      "Extract modal dynamic analysis parameter from analyis task", GH_ParamAccess.item);
      pManager[0].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaModalDynamicParameter());
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      var taskParameter = new GsaModalDynamic(_modeMethod);
      GsaAnalysisTaskGoo analysisTaskGoo = null;
      if (da.GetData(0, ref analysisTaskGoo)) {
        if (analysisTaskGoo.Value.ApiTask != null) {
          taskParameter = new GsaModalDynamic(analysisTaskGoo.Value.ApiTask);
          if (_modeMethod != taskParameter.Method()) {
            switch (taskParameter.Method()) {
              case ModeCalculationMethod.NumberOfMode:
                SetSelected(0, 0);
                break;
              case ModeCalculationMethod.FrquencyRange:
                SetSelected(0, 1);
                break;
              case ModeCalculationMethod.TargetMassRatio:
                SetSelected(0, 2);
                break;
              default:
                break;
            }
            ExpireSolution(true);
          }
        }
      }
      int positionIndex = 0;
      int maxMode = 0;
      switch (_modeMethod) {
        case ModeCalculationMethod.NumberOfMode:
          int modeCount = 0;
          if (da.GetData(++positionIndex, ref modeCount)) {
            taskParameter.ModeCalculationStrategy = new ModeCalculationStrategyByNumberOfModes(modeCount);
          }
          break;
        case ModeCalculationMethod.FrquencyRange:
          var frequencyOption = taskParameter.ModeCalculationStrategy as ModeCalculationStrategyByFrequency;
          double? lowFrequency = frequencyOption.LowerFrequency;
          da.GetData(++positionIndex, ref lowFrequency);
          double? highFrequency = frequencyOption.HigherFrequency;
          da.GetData(++positionIndex, ref highFrequency);
          maxMode = frequencyOption.MaximumNumberOfModes;
          da.GetData(++positionIndex, ref maxMode);
          taskParameter.ModeCalculationStrategy = new ModeCalculationStrategyByFrequency(lowFrequency, highFrequency, maxMode);
          break;
        case ModeCalculationMethod.TargetMassRatio:
          var targetMassOption = taskParameter.ModeCalculationStrategy as ModeCalculationStrategyByMassParticipation;
          double targetMassInXDirection = targetMassOption.TargetMassInXDirection;
          da.GetData(++positionIndex, ref targetMassInXDirection);
          double targetMassInYDirection = targetMassOption.TargetMassInYDirection;
          da.GetData(++positionIndex, ref targetMassInYDirection);
          double targetMassInZDirection = targetMassOption.TargetMassInZDirection;
          da.GetData(++positionIndex, ref targetMassInZDirection);
          maxMode = targetMassOption.MaximumNumberOfModes;
          da.GetData(++positionIndex, ref maxMode);
          bool masil = targetMassOption.SkipModesWithLowMassParticipation;
          da.GetData(++positionIndex, ref masil);
          taskParameter.ModeCalculationStrategy = new ModeCalculationStrategyByMassParticipation(targetMassInXDirection, targetMassInYDirection, targetMassInZDirection, maxMode, masil);
          break;
        default:
          break;
      }
      string loadCase = taskParameter.AdditionalMassDerivedFromLoads.CaseDefinition;
      da.GetData(++positionIndex, ref loadCase);
      double loadScaleFactor = taskParameter.AdditionalMassDerivedFromLoads.ScaleFactor;
      da.GetData(++positionIndex, ref loadScaleFactor);
      taskParameter.AdditionalMassDerivedFromLoads = new AdditionalMassDerivedFromLoads(loadCase, GetDirection(_selectedItems[2]), loadScaleFactor);

      double massScaleFactor = taskParameter.MassOption.ScaleFactor;
      da.GetData(++positionIndex, ref massScaleFactor);
      taskParameter.MassOption = new MassOption(GetMassOption(_selectedItems[1]), massScaleFactor);

      double? dampingStiffness = taskParameter.ModalDamping.StiffnessProportion;
      da.GetData(positionIndex + 1, ref dampingStiffness);
      taskParameter.ModalDamping = new ModalDamping(dampingStiffness);

      da.SetData(0, new GsaModalDynamicGoo(taskParameter));
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];

      ModeCalculationMethod modeMethod = GetModeStrategy(_selectedItems[0]);
      if (i == 0) {
        UpdateParameters(modeMethod);
      }
      base.UpdateUI();
    }

    private static ModeCalculationMethod GetModeStrategy(string name) {
      IEnumerable<ModeCalculationMethod> items = _modeCalculationMethod.Where(x => x.Value.Equals(name)).Select(x => x.Key);
      if (items.Any()) {
        return items.First();
      } else {
        throw new ArgumentException("Unable to convert " + name + " to mode calculation strategy");
      }
    }

    private static Direction GetDirection(string name) {
      IEnumerable<Direction> items = _massDirection.Where(x => x.Value.Equals(name)).Select(x => x.Key);
      if (items.Any()) {
        return items.First();
      } else {
        throw new ArgumentException("Unable to convert " + name + " to direction");
      }
    }

    private static ModalMassOption GetMassOption(string name) {
      IEnumerable<ModalMassOption> items = _massOptions.Where(x => x.Value.Equals(name)).Select(x => x.Key);
      if (items.Any()) {
        return items.First();
      } else {
        throw new ArgumentException("Unable to convert " + name + " to mass option");
      }
    }

    private void UpdateParameters(ModeCalculationMethod modeMethod) {
      if (modeMethod == _modeMethod) {
        return;
      }

      _modeCalculationMethod.TryGetValue(modeMethod, out string eventName);
      RecordUndoEvent($"{eventName} Parameters");

      UnregisterParameters();

      _modeMethod = modeMethod;
    }

    private void UnregisterParameters() {
      for (int i = Params.Input.Count - 1; i >= 1; i--) {
        Params.UnregisterInputParameter(Params.Input[i], true);
      }
    }



    public override void VariableParameterMaintenance() {
      if (Params.Input.Count > 1) {
        //other dropdown has been selected
        return;
      }

      int index = 0;
      if (_modeMethod == ModeCalculationMethod.NumberOfMode) {
        CreateParameter.Create(Params, new Param_Integer(), ++index, "Modes", "Md", "Set number of mode", GH_ParamAccess.item);
      } else if (_modeMethod == ModeCalculationMethod.FrquencyRange) {
        CreateParameter.Create(Params, new Param_Number(), ++index, "Lower frequency", "LF", "Set lower frequency range", GH_ParamAccess.item);
        CreateParameter.Create(Params, new Param_Number(), ++index, "Upper frequency", "UF", "Set upper frequency range", GH_ParamAccess.item);
        CreateParameter.Create(Params, new Param_Integer(), ++index, "Limiting modes", "LM", "Limit maximum number of mode", GH_ParamAccess.item);

      } else {
        CreateParameter.Create(Params, new Param_Number(), ++index, "X-direction mass participation", "X", "Set x-direction mass participation", GH_ParamAccess.item);
        CreateParameter.Create(Params, new Param_Number(), ++index, "Y-direction mass participation", "Y", "Set y-direction mass participation", GH_ParamAccess.item);
        CreateParameter.Create(Params, new Param_Number(), ++index, "Z-direction mass participation", "Z", "Set z-direction mass participation", GH_ParamAccess.item);
        CreateParameter.Create(Params, new Param_Integer(), ++index, "Limiting modes", "LM", "Set limiting maximum number of mode", GH_ParamAccess.item);
        CreateParameter.Create(Params, new Param_Boolean(), ++index, "Skip modes with low mass participation", "MASIL", "Set the value to true to use the Masil algorithm", GH_ParamAccess.item);
      }
      CreateParameter.Create(Params, new Param_String(), ++index, "Load case ", "LC", "Additional mass load case", GH_ParamAccess.item);
      CreateParameter.Create(Params, new Param_Number(), ++index, "Load scale factor", "LSF", "Set load scale factor", GH_ParamAccess.item);
      CreateParameter.Create(Params, new Param_Number(), ++index, "Mass scale factor", "MSF", "Set mass scale factor", GH_ParamAccess.item);
      CreateParameter.Create(Params, new Param_Number(), index + 1, "Damping stiffness proportion", "DSP", "Set model damping stiffness proportion", GH_ParamAccess.item);

    }


    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Mode Strategy",
        "Mass Option",
         "Mass Direction",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(_modeCalculationMethod.Values.ToList());
      _selectedItems.Add(_modeCalculationMethod.Values.ElementAt(0));

      _dropDownItems.Add(_massOptions.Values.ToList());
      _selectedItems.Add(_massOptions.Values.ElementAt(0));

      _dropDownItems.Add(_massDirection.Values.ToList());
      _selectedItems.Add(_massDirection.Values.ElementAt(0));

      _isInitialised = true;
    }
  }
}
