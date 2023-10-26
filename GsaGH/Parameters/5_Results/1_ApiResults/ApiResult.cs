using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class ApiResult : IApiResult {
    public object Result { get; }
    internal ApiResult(AnalysisCaseResult result) {
      Result = result;
    }

    internal ApiResult(CombinationCaseResult result) {
      Result = result;
    }
  }
}
