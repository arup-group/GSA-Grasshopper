
using GsaAPI;

using GsaGH.Parameters.Enums;

namespace GsaGH.Parameters {
  /// <summary>
  /// <para>Analysis <see href="https://docs.oasys-software.com/structural/gsa/references/element-types.html#element-types">parameter</see> used in modal analysis.</para>
  /// </summary>
  public class GsaModalDynamic {
    public ModeCalculationStrategy ModeCalculationStrategy { get; internal set; }
    public MassOption MassOption { get; internal set; }
    public AdditionalMassDerivedFromLoads AdditionalMassDerivedFromLoads { get; internal set; }
    public ModalDamping ModalDamping { get; internal set; }

    /// <summary>
    /// Empty constructor instantiating a new API object
    /// </summary>
    public GsaModalDynamic(ModeCalculationMethod modeMethod = ModeCalculationMethod.NumberOfMode) {
      switch (modeMethod) {
        case ModeCalculationMethod.NumberOfMode:
          ModeCalculationStrategy = new ModeCalculationStrategyByNumberOfModes(1);
          break;
        case ModeCalculationMethod.FrquencyRange:
          ModeCalculationStrategy = new ModeCalculationStrategyByFrequency(null, null, 100);
          break;
        case ModeCalculationMethod.TargetMassRatio:
          ModeCalculationStrategy = new ModeCalculationStrategyByMassParticipation(0, 0, 0, 100, false);
          break;
      }
      MassOption = new MassOption(ModalMassOption.LumpMassAtNode, 1);
      AdditionalMassDerivedFromLoads = new AdditionalMassDerivedFromLoads("", Direction.X, 1);
      ModalDamping = new ModalDamping(1);
    }


    /// <summary>
    /// Create parameter from task
    /// </summary>
    /// <param name="apiAnalysisTask"></param>
    internal GsaModalDynamic(AnalysisTask apiAnalysisTask) {
      var parameter = new ModalDynamicTaskParameter(apiAnalysisTask);
      AdditionalMassDerivedFromLoads = parameter.AdditionalMassDerivedFromLoads;
      ModeCalculationStrategy = parameter.ModeCalculationStrategy;
      ModalDamping = parameter.ModalDamping;
      MassOption = parameter.MassOption;
    }


    public ModeCalculationMethod Method() {

      if (ModeCalculationStrategy is ModeCalculationStrategyByNumberOfModes) {
        return ModeCalculationMethod.NumberOfMode;
      }

      if (ModeCalculationStrategy is ModeCalculationStrategyByFrequency) {
        return ModeCalculationMethod.FrquencyRange;
      }

      return ModeCalculationMethod.TargetMassRatio;
    }

    public override string ToString() {
      return Method().ToString();
    }

  }
}
