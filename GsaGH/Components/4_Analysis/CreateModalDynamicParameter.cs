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
using Rhino.PlugIns;
using Grasshopper;
using GsaGH.Helpers;

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
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaModalDynamicParameter());
    }


    protected override void SolveInternal(IGH_DataAccess da) {

      var taskParameter = new GsaModalDynamic(_modeMethod);
      int index = 0;
      int maxMode = 0;
      switch (_modeMethod) {
        case ModeCalculationMethod.NumberOfMode:
          int modeCount = 0;
          if (da.GetData(index++, ref modeCount)) {
            taskParameter.ModeCalculationStrategy = new ModeCalculationStrategyByNumberOfModes(modeCount);
          }
          break;
        case ModeCalculationMethod.FrquencyRange:
          var frequencyOption = taskParameter.ModeCalculationStrategy as ModeCalculationStrategyByFrequency;
          double? lowFrequency = frequencyOption.LowerFrequency;
          da.GetData(index++, ref lowFrequency);
          double? highFrequency = frequencyOption.HigherFrequency;
          da.GetData(index++, ref highFrequency);
          maxMode = frequencyOption.MaximumNumberOfModes;
          da.GetData(index++, ref maxMode);
          taskParameter.ModeCalculationStrategy = new ModeCalculationStrategyByFrequency(lowFrequency, highFrequency, maxMode);
          break;
        case ModeCalculationMethod.TargetMassRatio:
          var targetMassOption = taskParameter.ModeCalculationStrategy as ModeCalculationStrategyByMassParticipation;
          double targetMassInXDirection = targetMassOption.TargetMassInXDirection;
          da.GetData(index++, ref targetMassInXDirection);
          double targetMassInYDirection = targetMassOption.TargetMassInYDirection;
          da.GetData(index++, ref targetMassInYDirection);
          double targetMassInZDirection = targetMassOption.TargetMassInZDirection;
          da.GetData(index++, ref targetMassInZDirection);
          maxMode = targetMassOption.MaximumNumberOfModes;
          da.GetData(index++, ref maxMode);
          bool masil = targetMassOption.SkipModesWithLowMassParticipation;
          da.GetData(index++, ref masil);
          taskParameter.ModeCalculationStrategy = new ModeCalculationStrategyByMassParticipation(targetMassInXDirection, targetMassInYDirection, targetMassInZDirection, maxMode, masil);
          break;
        default:
          break;
      }
      string loadCase = taskParameter.AdditionalMassDerivedFromLoads.CaseDefinition;
      da.GetData(index++, ref loadCase);
      double loadScaleFactor = taskParameter.AdditionalMassDerivedFromLoads.ScaleFactor;
      da.GetData(index++, ref loadScaleFactor);
      taskParameter.AdditionalMassDerivedFromLoads = new AdditionalMassDerivedFromLoads(loadCase, GetDirection(_selectedItems[2]), loadScaleFactor);

      double massScaleFactor = taskParameter.MassOption.ScaleFactor;
      da.GetData(index++, ref massScaleFactor);
      taskParameter.MassOption = new MassOption(GetMassOption(_selectedItems[1]), massScaleFactor);

      double? dampingStiffness = taskParameter.ModalDamping.StiffnessProportion;
      da.GetData(index, ref dampingStiffness);

      taskParameter.ModalDamping = new ModalDamping(dampingStiffness);
      if (!ValidateMassParticipation(taskParameter)) {
        return;
      }
      da.SetData(0, new GsaModalDynamicGoo(taskParameter));
    }


    private bool ValidateMassParticipation(GsaModalDynamic parameter) {
      bool validationStatus = true;
      switch (parameter.Method()) {
        case ModeCalculationMethod.NumberOfMode: {
            var method = parameter.ModeCalculationStrategy as ModeCalculationStrategyByNumberOfModes;
            if (method.NumberOfModes < 1) {
              this.AddRuntimeError("Number of mode should be greater than 1");
              validationStatus = false;
            }
          }
          break;
        case ModeCalculationMethod.FrquencyRange: {
            var method = parameter.ModeCalculationStrategy as ModeCalculationStrategyByFrequency;
            double higherFrequency = method.HigherFrequency ?? double.MaxValue;
            if (method.LowerFrequency.HasValue && (!GsaGH.Helpers.Utility.IsInRange
              (method.LowerFrequency.Value, 0, higherFrequency) || GsaGH.Helpers.Utility.IsApproxEqual(method.LowerFrequency.Value, higherFrequency))) {
              this.AddRuntimeError("Lower frquency should be positive value and less than higher frquency");
              validationStatus = false;
            }
            double lowerFrequency = method.LowerFrequency ?? 0;
            if (method.HigherFrequency.HasValue && (!GsaGH.Helpers.Utility.IsInRange
              (method.HigherFrequency.Value, lowerFrequency, method.HigherFrequency.Value) || GsaGH.Helpers.Utility.IsApproxEqual(method.HigherFrequency.Value, lowerFrequency))) {
              this.AddRuntimeError("Upper frquency should be positive value and greater than lower frquency");
              validationStatus = false;
            }

          }
          break;
        case ModeCalculationMethod.TargetMassRatio: {
            var method = parameter.ModeCalculationStrategy as ModeCalculationStrategyByMassParticipation;
            if (!GsaGH.Helpers.Utility.IsInRange(method.TargetMassInXDirection, 0, 100) || !GsaGH.Helpers.Utility.IsInRange(method.TargetMassInYDirection, 0, 100) || !GsaGH.Helpers.Utility.IsInRange(method.TargetMassInZDirection, 0, 100)) {
              this.AddRuntimeError("Target Mass participation ratio should be within the range of [0:100]");
              validationStatus = false;
            }
          }
          break;
      }

      double? dampingStiffness = parameter.ModalDamping.StiffnessProportion;
      if (dampingStiffness.HasValue && !GsaGH.Helpers.Utility.IsInRange(dampingStiffness.Value, 0, 1)) {
        this.AddRuntimeError("Damping stiffness should be within the range [0:1]");
        validationStatus = false;
      }

      if (parameter.AdditionalMassDerivedFromLoads.ScaleFactor < 0) {
        this.AddRuntimeError("Load scale factor should have positive value");
        validationStatus = false;
      }

      if (parameter.MassOption.ScaleFactor < 0) {
        this.AddRuntimeError("Mass scale factor should have positive value");
        validationStatus = false;
      }
      return validationStatus;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];

      ModeCalculationMethod modeMethod = GetModeStrategy(_selectedItems[0]);
      if (i == 0) {
        UpdateParameters(modeMethod);
        base.UpdateUI();
      }
    }

    private static ModeCalculationMethod GetModeStrategy(string name) {
      return _modeCalculationMethod.Where(x => x.Value.Equals(name)).Select(x => x.Key).First();
    }

    private static Direction GetDirection(string name) {
      return _massDirection.Where(x => x.Value.Equals(name)).Select(x => x.Key).First();
    }

    private static ModalMassOption GetMassOption(string name) {
      return _massOptions.Where(x => x.Value.Equals(name)).Select(x => x.Key).First();
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
      for (int i = Params.Input.Count - 1; i > -1; i--) {
        Params.UnregisterInputParameter(Params.Input[i], true);
      }
    }

    public override void VariableParameterMaintenance() {

      int index = 0;
      if (_modeMethod == ModeCalculationMethod.NumberOfMode) {
        CreateParameter.Create(Params, new Param_Integer(), index++, "Modes", "Md", "Set number of mode", GH_ParamAccess.item);
      } else if (_modeMethod == ModeCalculationMethod.FrquencyRange) {
        CreateParameter.Create(Params, new Param_Number(), index++, "Lower frequency", "LF", "Set lower frequency range", GH_ParamAccess.item);
        CreateParameter.Create(Params, new Param_Number(), index++, "Upper frequency", "UF", "Set upper frequency range", GH_ParamAccess.item);
        CreateParameter.Create(Params, new Param_Integer(), index++, "Limiting modes", "LM", "Limit maximum number of mode", GH_ParamAccess.item);

      } else {
        CreateParameter.Create(Params, new Param_Number(), index++, "X-direction mass participation", "X", "Set x-direction mass participation", GH_ParamAccess.item);
        CreateParameter.Create(Params, new Param_Number(), index++, "Y-direction mass participation", "Y", "Set y-direction mass participation", GH_ParamAccess.item);
        CreateParameter.Create(Params, new Param_Number(), index++, "Z-direction mass participation", "Z", "Set z-direction mass participation", GH_ParamAccess.item);
        CreateParameter.Create(Params, new Param_Integer(), index++, "Limiting modes", "LM", "Set limiting maximum number of mode", GH_ParamAccess.item);
        CreateParameter.Create(Params, new Param_Boolean(), index++, "Skip modes with low mass participation", "MASIL", "Set the value to true to use the Masil algorithm", GH_ParamAccess.item);
      }
      CreateParameter.Create(Params, new Param_String(), index++, "Load case ", "LC", "Additional mass load case", GH_ParamAccess.item);
      CreateParameter.Create(Params, new Param_Number(), index++, "Load scale factor", "LSF", "Set load scale factor", GH_ParamAccess.item);
      CreateParameter.Create(Params, new Param_Number(), index++, "Mass scale factor", "MSF", "Set mass scale factor", GH_ParamAccess.item);
      CreateParameter.Create(Params, new Param_Number(), index, "Damping stiffness proportion", "DSP", "Set model damping stiffness proportion", GH_ParamAccess.item);

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
