using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaAPI;
using GsaGH.Helpers.GsaApi;
using GsaGH.Parameters;
using GsaGHTests.Helper;
using Xunit;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGHTests.Parameters {
  public partial class GsaResultTests {

    public static GsaResult AnalysisCaseResult(string file, int caseId) {
      var apiModel = new GsaAPI.Model(file);
      var model = new GsaModel(apiModel);
      ReadOnlyDictionary<int, AnalysisCaseResult> analysisCaseResults = model.Model.Results();
      return new GsaResult(model, analysisCaseResults[caseId], caseId);
    }

    public static GsaResult CombinationCaseResult(string file, int caseId, IEnumerable<int> permutations = null) {
      var apiModel = new GsaAPI.Model(file);
      var model = new GsaModel(apiModel);
      ReadOnlyDictionary<int, CombinationCaseResult> combinationCaseResults = model.Model.CombinationCaseResults();
      if (permutations == null) {
        permutations = new List<int>() { 1 };
      }

      return new GsaResult(model, combinationCaseResults[caseId], caseId, permutations);
    }

    // these are regression tests, the values are taken directly from GSA results
    // "Steel_Design_Complex.gwb"
    [Fact]
    public void GsaResultAnalysisCaseDisplacementXTest() {
      GsaResult result = AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);

      Assert.NotNull(result);
      Assert.Equal(1, result.CaseId);
      Assert.Equal(CaseType.AnalysisCase, result.CaseType);

      string nodeList = "442 to 468";
      List<GsaNodeDisplacements> resultValues =
        result.NodeDisplacementValues(nodeList, LengthUnit.Millimeter);
      
      var expectedIds = result.Model.Model.Nodes(nodeList).Keys.ToList();
      Assert.Equal(expectedIds, resultValues[0].Ids);

      List<double> expectedX = ExpectedDisplacementXInMillimeter();
      var actualX = new List<double>();
      foreach (int id in expectedIds) {
        GsaResultQuantity res = resultValues[0].XyzResults[id][0]; // there is only one result per node
        actualX.Add(ResultHelper.RoundToSignificantDigits(res.X.As(LengthUnit.Millimeter), 4));
      }

      Assert.Equal(expectedX, actualX);

      Assert.Equal(6.426, ResultHelper.RoundToSignificantDigits(resultValues[0].DmaxX.As(LengthUnit.Millimeter), 4));
      Assert.Equal(-0.1426, ResultHelper.RoundToSignificantDigits(resultValues[0].DminX.As(LengthUnit.Millimeter), 4));
    }

    //ReadOnlyCollection<int> expectedIds = result.Model.Model.ExpandList(
    //  new EntityList() {
    //    Definition = nodeList,
    //    Name = "testList",
    //    Type = GsaAPI.EntityType.Undefined }
    //  );

    private static List<double> ExpectedDisplacementXInMillimeter() {
      return new List<double>() {
        1.108,
0.9107,
0.6290,
0.2078,
0.1886,
0.5299,
1.140,
1.923,
2.786,
3.646,
4.429,
5.087,
5.601,
5.973,
6.214,
6.349,
6.409,
6.426,
6.423,
6.419,
6.417,
0.3396,
0.2390,
0.01895,
-0.1426,
0.01656,
0.4770
      };
    }
  }
}
