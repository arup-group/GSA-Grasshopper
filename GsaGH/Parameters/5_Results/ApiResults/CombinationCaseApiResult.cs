using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class CombinationCaseApiResult : IApiResult {
    public object Result { get; }
    public CombinationCaseApiResult(CombinationCaseResult result) {
      Result = result;
    }
  } 
}
