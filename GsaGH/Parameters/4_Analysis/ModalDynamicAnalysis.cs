
using System.Collections.Generic;
using System.Xml.Linq;

using GsaAPI;

namespace GsaGH.Parameters {
  /// <summary>
  /// <para>Analysis <see href="https://docs.oasys-software.com/structural/gsa/references/element-types.html#element-types">parameter</see> used in modal analysis.</para>
  /// </summary>
  public class GsaModalDynamicAnalysis {
    public AnalysisTask ApiAnalysisTask { get; internal set; }
    public ModeCalculationStrategy ModeCalculationStrategy { get; internal set; }
    public MassOption MassOption { get; internal set; }
    public AdditionalMassDerivedFromLoads AdditionalMassDerivedFromLoads { get; internal set; }
    public ModalDamping ModalDamping { get; internal set; }

    /// <summary>
    /// Empty constructor instantiating a new API object
    /// </summary>
    public GsaModalDynamicAnalysis() {
      var parameter = new ModalDynamicTaskParameter(new ModeCalculationStrategyByNumberOfModes(1), new MassOption(ModalMassOption.LumpMassAtNode, 1), new AdditionalMassDerivedFromLoads("", Direction.X, 1), new ModalDamping(1));
      ApiAnalysisTask = AnalysisTaskFactory.CreateModalDynamicAnalysisTask("task 1", parameter);
      CopyParameter(parameter);
    }

    /// <summary>
    ///  Create task from parameter
    /// </summary>
    /// <param name="modeCalcuationStrategy"></param>
    /// <param name="massOption"></param>
    /// <param name="additionalMassDerivedFromLoads"></param>
    /// <param name="modalDamping"></param>
    /// <param name="name"></param>
    internal GsaModalDynamicAnalysis(ModeCalculationStrategy modeCalcuationStrategy, MassOption massOption,
                                   AdditionalMassDerivedFromLoads additionalMassDerivedFromLoads,
                                   ModalDamping modalDamping, string name = "task 1") {
      var parameter = new ModalDynamicTaskParameter(modeCalcuationStrategy, massOption, additionalMassDerivedFromLoads, modalDamping);
      ApiAnalysisTask = AnalysisTaskFactory.CreateModalDynamicAnalysisTask(name, parameter);
      CopyParameter(parameter);
    }

    /// <summary>
    /// Create parameter from task
    /// </summary>
    /// <param name="gsaAnalysisTask"></param>
    public GsaModalDynamicAnalysis(GsaAnalysisTask gsaAnalysisTask) {
      var parameter = new ModalDynamicTaskParameter(gsaAnalysisTask.ApiTask);
      ApiAnalysisTask = AnalysisTaskFactory.CreateModalDynamicAnalysisTask(gsaAnalysisTask.ApiTask.Name, parameter);
      CopyParameter(parameter);
    }

    /// <summary>
    /// Create a duplicate instance from another instance
    /// </summary>
    /// <param name="other"></param>
    public GsaModalDynamicAnalysis(GsaModalDynamicAnalysis other) {
      ApiAnalysisTask = other.ApiAnalysisTask;
      ModeCalculationStrategy = other.ModeCalculationStrategy;
      MassOption = other.MassOption;
      AdditionalMassDerivedFromLoads = other.AdditionalMassDerivedFromLoads;
      ModalDamping = other.ModalDamping;
    }

    private void CopyParameter(ModalDynamicTaskParameter parameter) {
      ModeCalculationStrategy = parameter.ModeCalculationStrategy;
      MassOption = parameter.MassOption;
      AdditionalMassDerivedFromLoads = parameter.AdditionalMassDerivedFromLoads;
      ModalDamping = parameter.ModalDamping;
    }

    public override string ToString() {
      return "";
    }

  }
}
