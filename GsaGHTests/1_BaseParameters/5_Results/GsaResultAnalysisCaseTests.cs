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

    private class DisplacementsExpectedResults {
      public double X,
        Y,
        Z,
        Xyz,
        Xx,
        Yy,
        Zz,
        Xxyyzz;

      public DisplacementsExpectedResults(
        double x, double y, double z, double xyz, double xx, double yy, double zz, double xxyyzz) {
        X = x;
        Y = y;
        Z = z;
        Xyz = xyz;
        Xx = xx;
        Yy = yy;
        Zz = zz;
        Xxyyzz = xxyyzz;
      }
    }

    //in mm
    private readonly List<DisplacementsExpectedResults> listOfExpectedDisplacements
      = new List<DisplacementsExpectedResults>() {
        //x,y,z, xx, yy,zz
        new DisplacementsExpectedResults(1.108, 10.27, 4.700, 11.35, -0.002972, 0.001090, -78.27E-6,
          0.003166),
        new DisplacementsExpectedResults(0.9107, 9.467, 4.609, 10.57, -0.004069, 0.001338, 16.27E-6,
          0.004283),
        new DisplacementsExpectedResults(0.6290, 9.513, 1.052, 9.592, -0.004135, 0.003758,
          -251.5E-6, 0.005593),
        new DisplacementsExpectedResults(0.2078, 8.731, -16.58, 18.74, -0.004172, 0.007726,
          -122.1E-6, 0.008781),
        new DisplacementsExpectedResults(0.1886, 8.510, -36.44, 37.42, -0.003659, 0.007265,
          25.22E-6, 0.008135),
        new DisplacementsExpectedResults(0.5299, 8.808, -53.62, 54.35, -0.002896, 0.005896,
          123.7E-6, 0.006570),
        new DisplacementsExpectedResults(1.140, 9.253, -66.46, 67.11, -0.002095, 0.004087, 124.5E-6,
          0.004594),
        new DisplacementsExpectedResults(1.923, 9.635, -74.35, 74.99, -0.001404, 0.002129, 86.95E-6,
          0.002552),
        new DisplacementsExpectedResults(2.786, 9.883, -77.21, 77.89, -872.6E-6, 169.9E-6, 41.32E-6,
          890.0E-6), };
    private readonly List<DisplacementsExpectedResults> listOfExpectedMaximas //in mm
      = new List<DisplacementsExpectedResults>() {
        new DisplacementsExpectedResults(2.786, 9.883, -77.21, 77.89, -872.6E-6, 169.9E-6, 41.32E-6,
          890.0E-6),
        new DisplacementsExpectedResults(1.108, 10.27, 4.700, 11.35, -0.002972, 0.001090, -78.27E-6,
          0.003166),
        new DisplacementsExpectedResults(1.108, 10.27, 4.700, 11.35, -0.002972, 0.001090, -78.27E-6,
          0.003166),
        new DisplacementsExpectedResults(2.786, 9.883, -77.21, 77.89, -872.6E-6, 169.9E-6, 41.32E-6,
          890.0E-6),
        new DisplacementsExpectedResults(2.786, 9.883, -77.21, 77.89, -872.6E-6, 169.9E-6, 41.32E-6,
          890.0E-6),
        new DisplacementsExpectedResults(0.2078, 8.731, -16.58, 18.74, -0.004172, 0.007726,
          -122.1E-6, 0.008781),
        new DisplacementsExpectedResults(1.140, 9.253, -66.46, 67.11, -0.002095, 0.004087, 124.5E-6,
          0.004594),
        new DisplacementsExpectedResults(0.2078, 8.731, -16.58, 18.74, -0.004172, 0.007726,
          -122.1E-6, 0.008781),
        new DisplacementsExpectedResults(1.108, 10.27, 4.700, 11.35, -0.002972, 0.001090, -78.27E-6,
          0.003166), };
    private readonly List<DisplacementsExpectedResults> listOfExpectedMinimas //in mm
      = new List<DisplacementsExpectedResults>() {
        new DisplacementsExpectedResults(0.1886, 8.510, -36.44, 37.42, -0.003659, 0.007265,
          25.22E-6, 0.008135),
        new DisplacementsExpectedResults(0.1886, 8.510, -36.44, 37.42, -0.003659, 0.007265,
          25.22E-6, 0.008135),
        new DisplacementsExpectedResults(2.786, 9.883, -77.21, 77.89, -872.6E-6, 169.9E-6, 41.32E-6,
          890.0E-6),
        new DisplacementsExpectedResults(0.6290, 9.513, 1.052, 9.592, -0.004135, 0.003758,
          -251.5E-6, 0.005593),
        new DisplacementsExpectedResults(0.2078, 8.731, -16.58, 18.74, -0.004172, 0.007726,
          -122.1E-6, 0.008781),
        new DisplacementsExpectedResults(2.786, 9.883, -77.21, 77.89, -872.6E-6, 169.9E-6, 41.32E-6,
          890.0E-6),
        new DisplacementsExpectedResults(0.6290, 9.513, 1.052, 9.592, -0.004135, 0.003758,
          -251.5E-6, 0.005593),
        new DisplacementsExpectedResults(2.786, 9.883, -77.21, 77.89, -872.6E-6, 169.9E-6, 41.32E-6,
          890.0E-6),
        new DisplacementsExpectedResults(0.1886, 8.510, -36.44, 37.42, -0.003659, 0.007265,
          25.22E-6, 0.008135), };

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
    public void GsaResultAnalysisCaseNodeDisplacementTest() {
      GsaResult result = AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);

      Assert.NotNull(result);
      Assert.Equal(1, result.CaseId);
      Assert.Equal(CaseType.AnalysisCase, result.Type);

      string nodeList = "442 to 450";
      Tuple<List<GsaResultsValues>, List<int>> resultValueTuple
        = result.NodeDisplacementValues(nodeList, LengthUnit.Millimeter);

      var expectedIds = result.Model.Model.Nodes(nodeList).Keys.ToList();
      Assert.Equal(expectedIds, resultValueTuple.Item2);

      List<GsaResultsValues> resultValues = resultValueTuple.Item1;

      // Assert
      IEnumerable<double> expectedX = listOfExpectedDisplacements.Select(item => item.X);
      var actualX = expectedIds.Select(id => resultValues[0].XyzResults[id][0]).Select(res
        => ResultHelper.RoundToSignificantDigits(res.X.As(LengthUnit.Millimeter), 4)).ToList();

      Assert.Equal(expectedX, actualX);

      IEnumerable<double> expectedY = listOfExpectedDisplacements.Select(item => item.Y);
      var actualY = expectedIds.Select(id => resultValues[0].XyzResults[id][0]).Select(res
        => ResultHelper.RoundToSignificantDigits(res.Y.As(LengthUnit.Millimeter), 4)).ToList();

      Assert.Equal(expectedY, actualY);

      IEnumerable<double> expectedZ = listOfExpectedDisplacements.Select(item => item.Z);
      var actualZ = expectedIds.Select(id => resultValues[0].XyzResults[id][0]).Select(res
        => ResultHelper.RoundToSignificantDigits(res.Z.As(LengthUnit.Millimeter), 4)).ToList();

      Assert.Equal(expectedZ, actualZ);

      IEnumerable<double> expectedXYZ = listOfExpectedDisplacements.Select(item => item.Xyz);
      var actualXYZ = expectedIds.Select(id => resultValues[0].XyzResults[id][0]).Select(res
        => ResultHelper.RoundToSignificantDigits(res.Xyz.As(LengthUnit.Millimeter), 4)).ToList();

      Assert.Equal(expectedXYZ, actualXYZ);

      IEnumerable<double> expectedXx = listOfExpectedDisplacements.Select(item => item.Xx);
      var actualXx = expectedIds.Select(id => resultValues[0].XxyyzzResults[id][0]).Select(res
        => ResultHelper.RoundToSignificantDigits(res.X.As(LengthUnit.Millimeter), 4)).ToList();

      Assert.Equal(expectedXx, actualXx);
      IEnumerable<double> expectedYy = listOfExpectedDisplacements.Select(item => item.Yy);
      var actualYy = expectedIds.Select(id => resultValues[0].XxyyzzResults[id][0]).Select(res
        => ResultHelper.RoundToSignificantDigits(res.Y.As(LengthUnit.Millimeter), 4)).ToList();

      Assert.Equal(expectedYy, actualYy);

      IEnumerable<double> expectedZz = listOfExpectedDisplacements.Select(item => item.Zz);
      var actualZz = expectedIds.Select(id => resultValues[0].XxyyzzResults[id][0]).Select(res
        => ResultHelper.RoundToSignificantDigits(res.Z.As(LengthUnit.Millimeter), 4)).ToList();

      Assert.Equal(expectedZz, actualZz);

      IEnumerable<double> expectedXxyyzz = listOfExpectedDisplacements.Select(item => item.Xxyyzz);
      var actualXxyyzz = expectedIds.Select(id => resultValues[0].XxyyzzResults[id][0]).Select(res
        => ResultHelper.RoundToSignificantDigits(res.Xyz.As(LengthUnit.Millimeter), 4)).ToList();

      Assert.Equal(expectedXxyyzz, actualXxyyzz);
    }

    //ReadOnlyCollection<int> expectedIds = result.Model.Model.ExpandList(
    //  new EntityList() {
    //    Definition = nodeList,
    //    Name = "testList",
    //    Type = GsaAPI.EntityType.Undefined }
    //  );
  }
}
