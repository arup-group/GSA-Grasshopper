using System.Collections.Generic;

using GsaGH.Parameters;

namespace GsaGH.Helpers.Import {
  internal class GsaAnalysis {
    internal List<GsaAnalysisTask> AnalysisTasks { get; set; } = new List<GsaAnalysisTask>();
    internal List<GsaCombinationCase> CombinationCases { get; set; } = new List<GsaCombinationCase>();
    internal List<IGsaDesignTask> DesignTasks { get; set; } = new List<IGsaDesignTask>();

    internal GsaAnalysis() { }

    internal bool IsNullOrEmpty() {
      return AnalysisTasks.IsNullOrEmpty() & CombinationCases.IsNullOrEmpty() & DesignTasks.IsNullOrEmpty();
    }
  }
}
