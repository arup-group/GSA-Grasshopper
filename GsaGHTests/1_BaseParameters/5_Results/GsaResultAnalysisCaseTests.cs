using System.Collections.Generic;
using System.Collections.ObjectModel;

using GsaAPI;

using GsaGH.Parameters;
using GsaGH.Parameters.Results;

namespace GsaGHTests.Parameters {
  public partial class GsaResultTests {

    public static GsaResult AnalysisCaseResult(string file, int caseId) {
      var apiModel = new GsaAPI.Model(file);
      var model = new GsaModel(apiModel);
      ReadOnlyDictionary<int, AnalysisCaseResult> analysisCaseResults = model.ApiModel.Results();
      return new GsaResult(model, analysisCaseResults[caseId], caseId);
    }

    public static GsaResult CombinationCaseResult(string file, int caseId, IEnumerable<int> permutations = null) {
      var apiModel = new GsaAPI.Model(file);
      var model = new GsaModel(apiModel);
      ReadOnlyDictionary<int, CombinationCaseResult> combinationCaseResults = model.ApiModel.CombinationCaseResults();
      if (permutations == null) {
        permutations = new List<int>() { 1 };
      }

      return new GsaResult(model, combinationCaseResults[caseId], caseId, permutations);
    }
  }
}
