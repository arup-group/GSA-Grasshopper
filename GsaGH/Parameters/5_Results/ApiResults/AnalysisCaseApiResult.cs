using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class AnalysisCaseApiResult : IApiResult {
    public object Result { get; }
    public AnalysisCaseApiResult(AnalysisCaseResult result) {
      Result = result;
    }
  } 
}
