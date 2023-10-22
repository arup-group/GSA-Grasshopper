using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class CombinationCaseApiResult : IApiResult {
    public object Result { get; }
    internal CombinationCaseApiResult(CombinationCaseResult result) {
      Result = result;
    }
  } 
}
