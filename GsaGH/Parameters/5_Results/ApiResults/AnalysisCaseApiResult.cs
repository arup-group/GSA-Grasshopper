using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class AnalysisCaseApiResult : IApiResult {
    public object Result { get; }
    internal AnalysisCaseApiResult(AnalysisCaseResult result) {
      Result = result;
    }
  } 
}
