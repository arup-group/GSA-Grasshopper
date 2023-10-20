using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaAPI;
using GsaGH.Helpers.GsaApi;
using GsaGH.Parameters;
using GsaGHTests.Helper;
using Xunit;
using AngleUnit = OasysUnits.Units.AngleUnit;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGHTests.Parameters {
  public partial class GsaResultTests {
    public static GsaResult AnalysisCaseResult(string file, int caseId) {
      var apiModel = new GsaAPI.Model(file);
      var model = new GsaModel(apiModel);
      ReadOnlyDictionary<int, AnalysisCaseResult> analysisCaseResults = model.Model.Results();
      return new GsaResult(model, analysisCaseResults[caseId], caseId);
    }

    public static GsaResult CombinationCaseResult(
      string file, int caseId, IEnumerable<int> permutations = null) {
      var apiModel = new GsaAPI.Model(file);
      var model = new GsaModel(apiModel);
      ReadOnlyDictionary<int, CombinationCaseResult> combinationCaseResults
        = model.Model.CombinationCaseResults();
      if (permutations == null) {
        permutations = new List<int>() {
          1,
        };
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
      List<GsaResultsValues> resultValues
        = result.NodeDisplacementValues(nodeList, LengthUnit.Millimeter);

      var expectedIds = result.Model.Model.Nodes(nodeList).Keys.ToList();
      Assert.Equal(expectedIds, resultValues[0].Ids);

      List<double> expectedX = ExpectedDisplacementXInMillimeter();
      var actualX = new List<double>();
      foreach (int id in expectedIds) {
        GsaResultQuantity res
          = resultValues[0].XyzResults[id][0]; // there is only one result per node
        actualX.Add(ResultHelper.RoundToSignificantDigits(res.X.As(LengthUnit.Millimeter), 4));
      }

      Assert.Equal(expectedX, actualX);

      Assert.Equal(6.426,
        ResultHelper.RoundToSignificantDigits(resultValues[0].DmaxX.As(LengthUnit.Millimeter), 4));
      Assert.Equal(-0.1426,
        ResultHelper.RoundToSignificantDigits(resultValues[0].DminX.As(LengthUnit.Millimeter), 4));
    }

    // these are regression tests, the values are taken directly from GSA results
    // "Steel_Design_Complex.gwb"
    [Fact]
    public void GsaResultAnalysisCaseDisplacementXxTest() {
      GsaResult result = AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);

      Assert.NotNull(result);
      Assert.Equal(1, result.CaseId);
      Assert.Equal(CaseType.AnalysisCase, result.CaseType);

      string nodeList = "442 to 468";
      List<GsaResultsValues> resultValues
        = result.NodeDisplacementValues(nodeList, LengthUnit.Millimeter);

      var expectedIds = result.Model.Model.Nodes(nodeList).Keys.ToList();
      Assert.Equal(expectedIds, resultValues[0].Ids);

      List<double> expectedXx = ExpectedDisplacementXxInRadians();
      var actualXx = new List<double>();
      foreach (int id in expectedIds) {
        GsaResultQuantity res
          = resultValues[0].XxyyzzResults[id][0]; // there is only one result per node
        actualXx.Add(ResultHelper.RoundToSignificantDigits(res.X.As(AngleUnit.Radian), 4));
      }

      Assert.Equal(expectedXx, actualXx);

      Assert.Equal(0.0004991,
        ResultHelper.RoundToSignificantDigits(resultValues[0].DmaxXx.As(AngleUnit.Radian), 4));
      Assert.Equal(-0.004172,
        ResultHelper.RoundToSignificantDigits(resultValues[0].DminXx.As(AngleUnit.Radian), 4));
    }

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
        0.4770,
      };
    }

    private static List<double> ExpectedDisplacementXxInRadians() {
      return new List<double>() {
        -0.002972,
        -0.004069,
        -0.004135,
        -0.004172,
        -0.003659,
        -0.002896,
        -0.002095,
        -0.001404,
        -0.0008726,
        -0.0005007,
        -0.0002587,
        -1.28E-05,
        0.0001824,
        0.000353,
        0.0004895,
        0.0004991,
        0.0002841,
        -1.473E-05,
        -0.0002784,
        -0.0004398,
        -0.0005168,
        -0.002999,
        -0.003962,
        -0.003166,
        -0.004169,
        -0.004045,
        -0.003267,
      };
    }
  }
}
