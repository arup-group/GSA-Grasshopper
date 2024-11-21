
using GsaAPI;

using GsaGH.Parameters.Enums;

namespace GsaGH.Parameters {
  /// <summary>
  /// <para>Analysis <see href="https://docs.oasys-software.com/structural/gsa/references/element-types.html#element-types">parameter</see> used in modal analysis.</para>
  /// </summary>
  public class GsaModalDynamicAnalysis {
    public ModeCalculationStrategy ModeCalculationStrategy { get; internal set; }
    public MassOption MassOption { get; internal set; }
    public AdditionalMassDerivedFromLoads AdditionalMassDerivedFromLoads { get; internal set; }
    public ModalDamping ModalDamping { get; internal set; }

    /// <summary>
    /// Empty constructor instantiating a new API object
    /// </summary>
    public GsaModalDynamicAnalysis(ModeCalculationMethod modeMethod = ModeCalculationMethod.NumberOfMode) {
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
    /// Create new modal dynamic parameter
    /// </summary>
    /// <param name="modeCalcuationStrategy"></param>
    /// <param name="massOption"></param>
    /// <param name="additionalMassDerivedFromLoads"></param>
    /// <param name="modalDamping"></param>
    internal GsaModalDynamicAnalysis(ModeCalculationStrategy modeCalcuationStrategy, MassOption massOption,
                                   AdditionalMassDerivedFromLoads additionalMassDerivedFromLoads,
                                   ModalDamping modalDamping) {
      AdditionalMassDerivedFromLoads = additionalMassDerivedFromLoads;
      ModeCalculationStrategy = modeCalcuationStrategy;
      ModalDamping = modalDamping;
      MassOption = massOption;
    }

    /// <summary>
    /// Create parameter from task
    /// </summary>
    /// <param name="apiAnalysisTask"></param>
    internal GsaModalDynamicAnalysis(AnalysisTask apiAnalysisTask) {
      var parameter = new ModalDynamicTaskParameter(apiAnalysisTask);
      AdditionalMassDerivedFromLoads = parameter.AdditionalMassDerivedFromLoads;
      ModeCalculationStrategy = parameter.ModeCalculationStrategy;
      ModalDamping = parameter.ModalDamping;
      MassOption = parameter.MassOption;
    }

    /// <summary>
    /// Create a duplicate instance from another instance
    /// </summary>
    /// <param name="other"></param>
    public GsaModalDynamicAnalysis(GsaModalDynamicAnalysis other) {
      AdditionalMassDerivedFromLoads = other.AdditionalMassDerivedFromLoads;
      ModeCalculationStrategy = other.ModeCalculationStrategy;
      ModalDamping = other.ModalDamping;
      MassOption = other.MassOption;
    }

    public override string ToString() {
      return "";
    }

  }
}
