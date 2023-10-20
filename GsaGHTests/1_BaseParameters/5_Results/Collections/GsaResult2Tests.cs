﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using GsaAPI;
using GsaGH.Parameters;
using GsaGH.Parameters.Results;
using Xunit;

namespace GsaGHTests.Parameters.Results {
  [Collection("GrasshopperFixture collection")]
  public partial class GsaResult2Tests {
    public static IGsaResult AnalysisCaseResult(string file, int caseId) {
      var apiModel = new GsaAPI.Model(file);
      var model = new GsaModel(apiModel);
      ReadOnlyDictionary<int, AnalysisCaseResult> analysisCaseResults = model.Model.Results();
      return new GsaResult2(model, analysisCaseResults[caseId], caseId);
    }

    public static IGsaResult CombinationCaseResult(string file, int caseId, IEnumerable<int> permutations = null) {
      var apiModel = new GsaAPI.Model(file);
      var model = new GsaModel(apiModel);
      ReadOnlyDictionary<int, CombinationCaseResult> combinationCaseResults = model.Model.CombinationCaseResults();
      if (permutations == null) {
        permutations = new List<int>() { 1 };
      }

      return new GsaResult2(model, combinationCaseResults[caseId], caseId, permutations);
    }
  }
}
