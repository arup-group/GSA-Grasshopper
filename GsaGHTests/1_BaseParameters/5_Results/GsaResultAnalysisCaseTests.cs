using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaAPI;
using GsaGH.Parameters;
using GsaGHTests.Helper;
using OasysUnits;
using Xunit;
using static GsaGH.Helpers.GsaApi.ResultHelper;
using AngleUnit = OasysUnits.Units.AngleUnit;
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

    private const string NodeList = "442 to 450";

    //ACase
    private const double ACaseMaxX = 2.786;
    private const double ACaseMaxY = 10.27;
    private const double ACaseMaxZ = 4.700;
    private const double ACaseMaxXYZ = 77.89;
    private const double ACaseMaxXx = -872.6E-6;
    private const double ACaseMaxYy = 0.007726;
    private const double ACaseMaxZz = 124.5E-6;
    private const double ACaseMaxXxyyzz = 0.008781;

    private const double ACaseMinX = 0.1886;
    private const double ACaseMinY = 8.510;
    private const double ACaseMinZ = -77.21;
    private const double ACaseMinXYZ = 9.592;
    private const double ACaseMinXx = -0.004172;
    private const double ACaseMinYy = 169.9E-6;
    private const double ACaseMinZz = -251.5E-6;
    private const double ACaseMinXxyyzz = 890.0E-6;

    //CCase
    private const double CCaseMaxX = 3.607;
    private const double CCaseMaxY = 2.318;
    private const double CCaseMaxZ = 2.754;
    private const double CCaseMaxXYZ = 101.6;
    private const double CCaseMaxXx = -0.001579;
    private const double CCaseMaxYy = 0.009273;
    private const double CCaseMaxZz = 30.74E-6;
    private const double CCaseMaxXxyyzz = 0.01009;

    private const double CCaseMinX = 0.9682;
    private const double CCaseMinY = -0.9809;
    private const double CCaseMinZ = -101.5;
    private const double CCaseMinXYZ = 3.039;
    private const double CCaseMinXx = -0.003965;
    private const double CCaseMinYy = 0.001579;
    private const double CCaseMinZz = -432.0E-6;
    private const double CCaseMinXxyyzz = 0.002233;

    //in mm
    private readonly List<DisplacementsExpectedResults> listOfExpectedA1Displacements
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
    //in mm
    private readonly List<DisplacementsExpectedResults> listOfExpectedC1Displacements
      = new List<DisplacementsExpectedResults>() {
        new DisplacementsExpectedResults(2.318, 2.318, 2.754, 4.281, -0.001579, 0.001579, 0.0,
          0.002233),
        new DisplacementsExpectedResults(2.046, 1.401, 2.436, 3.476, -0.003125, 0.001878, 30.74E-6,
          0.003646),
        new DisplacementsExpectedResults(1.648, 1.435, -2.111, 3.039, -0.003206, 0.004661,
          -432.0E-6, 0.005673),
        new DisplacementsExpectedResults(1.088, 0.05658, -22.48, 22.51, -0.003965, 0.009273,
          -285.6E-6, 0.01009),
        new DisplacementsExpectedResults(0.9682, -0.6355, -46.02, 46.03, -0.003874, 0.009004,
          -113.9E-6, 0.009803),
        new DisplacementsExpectedResults(1.256, -0.7498, -67.15, 67.17, -0.003417, 0.007665,
          -15.04E-6, 0.008392),
        new DisplacementsExpectedResults(1.858, -0.7276, -83.86, 83.89, -0.002870, 0.005784,
          -25.93E-6, 0.006457),
        new DisplacementsExpectedResults(2.674, -0.7855, -95.37, 95.41, -0.002413, 0.003703,
          -72.65E-6, 0.004420),
        new DisplacementsExpectedResults(3.607, -0.9809, -101.5, 101.6, -0.002093, 0.001596,
          -121.3E-6, 0.002635), };

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
    [Theory]
    [InlineData("A")]
    [InlineData("C")]
    public void IsModelValidFor(string ca) {
      GsaResult result = ca == "A" ? AnalysisCaseResult(GsaFile.SteelDesignComplex, 1) :
        CombinationCaseResult(GsaFile.SteelDesignComplex, 1);

      Assert.NotNull(result);
      Assert.Equal(1, result.CaseId);
      Assert.Equal(ca == "A" ? CaseType.AnalysisCase : CaseType.Combination, result.Type);
    }

    [Theory]
    [InlineData("A")]
    [InlineData("C")]
    public void IsNodeListValid(string ca) {
      GsaResult result = ca == "A" ? AnalysisCaseResult(GsaFile.SteelDesignComplex, 1) :
        CombinationCaseResult(GsaFile.SteelDesignComplex, 1);

      Tuple<List<GsaResultsValues>, List<int>> resultValueTuple
        = result.NodeDisplacementValues(NodeList, LengthUnit.Millimeter);

      var expectedIds = new List<int>() {
        442,
        443,
        444,
        445,
        446,
        447,
        448,
        449,
        450,
      };
      Assert.Equal(expectedIds, resultValueTuple.Item2);
    }

    [Theory]
    [InlineData("A")]
    [InlineData("C")]
    public void GsaResultNodeDisplacementTest(string ca) {
      GsaResult result = ca == "A" ? AnalysisCaseResult(GsaFile.SteelDesignComplex, 1) :
        CombinationCaseResult(GsaFile.SteelDesignComplex, 1);

      Tuple<List<GsaResultsValues>, List<int>> resultValueTuple
        = result.NodeDisplacementValues(NodeList, LengthUnit.Millimeter);

      List<GsaResultsValues> resultValues = resultValueTuple.Item1;
      List<int> nodeIds = resultValueTuple.Item2;

      // Assert
      List<DisplacementsExpectedResults> expectedDisp = ca == "A" ? listOfExpectedA1Displacements :
        listOfExpectedC1Displacements;

      IEnumerable<double> expectedX = expectedDisp.Select(item => item.X);
      var actualX = nodeIds.Select(id => resultValues[0].XyzResults[id][0]).Select(res
        => RoundedLengthValueTo4Digits(res.X)).ToList();

      Assert.Equal(expectedX, actualX);

      IEnumerable<double> expectedY = expectedDisp.Select(item => item.Y);
      var actualY = nodeIds.Select(id => resultValues[0].XyzResults[id][0]).Select(res
        => RoundedLengthValueTo4Digits(res.Y)).ToList();

      Assert.Equal(expectedY, actualY);

      IEnumerable<double> expectedZ = expectedDisp.Select(item => item.Z);
      var actualZ = nodeIds.Select(id => resultValues[0].XyzResults[id][0]).Select(res
        => RoundedLengthValueTo4Digits(res.Z)).ToList();

      Assert.Equal(expectedZ, actualZ);

      IEnumerable<double> expectedXYZ = expectedDisp.Select(item => item.Xyz);
      var actualXYZ = nodeIds.Select(id => resultValues[0].XyzResults[id][0]).Select(res
        => RoundedLengthValueTo4Digits(res.Xyz)).ToList();

      Assert.Equal(expectedXYZ, actualXYZ);

      IEnumerable<double> expectedXx = expectedDisp.Select(item => item.Xx);
      var actualXx = nodeIds.Select(id => resultValues[0].XxyyzzResults[id][0]).Select(res
        => RoundedAngleValueTo4Digits(res.X)).ToList();

      Assert.Equal(expectedXx, actualXx);
      IEnumerable<double> expectedYy = expectedDisp.Select(item => item.Yy);
      var actualYy = nodeIds.Select(id => resultValues[0].XxyyzzResults[id][0]).Select(res
        => RoundedAngleValueTo4Digits(res.Y)).ToList();

      Assert.Equal(expectedYy, actualYy);

      IEnumerable<double> expectedZz = expectedDisp.Select(item => item.Zz);
      var actualZz = nodeIds.Select(id => resultValues[0].XxyyzzResults[id][0]).Select(res
        => RoundedAngleValueTo4Digits(res.Z)).ToList();

      Assert.Equal(expectedZz, actualZz);

      IEnumerable<double> expectedXxyyzz = expectedDisp.Select(item => item.Xxyyzz);
      var actualXxyyzz = nodeIds.Select(id => resultValues[0].XxyyzzResults[id][0]).Select(res
        => RoundedAngleValueTo4Digits(res.Xyz)).ToList();

      Assert.Equal(expectedXxyyzz, actualXxyyzz);
    }

    [Theory]
    [InlineData("A")]
    [InlineData("C")]
    public void IsMaximaAndMinimaValidFor(string ca) {
      GsaResult result = ca == "A" ? AnalysisCaseResult(GsaFile.SteelDesignComplex, 1) :
        CombinationCaseResult(GsaFile.SteelDesignComplex, 1);

      Tuple<List<GsaResultsValues>, List<int>> resultValueTuple
        = result.NodeDisplacementValues(NodeList, LengthUnit.Millimeter);

      List<GsaResultsValues> resultValues = resultValueTuple.Item1;

      Assert.Equal(ca == "A" ? ACaseMaxX : CCaseMaxX,
        RoundedLengthValueTo4Digits(resultValues[0].DmaxX));
      Assert.Equal(ca == "A" ? ACaseMaxY : CCaseMaxY,
        RoundedLengthValueTo4Digits(resultValues[0].DmaxY));
      Assert.Equal(ca == "A" ? ACaseMaxZ : CCaseMaxZ,
        RoundedLengthValueTo4Digits(resultValues[0].DmaxZ));
      Assert.Equal(ca == "A" ? ACaseMaxXYZ : CCaseMaxXYZ,
        RoundedLengthValueTo4Digits(resultValues[0].DmaxXyz));
      Assert.Equal(ca == "A" ? ACaseMaxXx : CCaseMaxXx,
        RoundedAngleValueTo4Digits(resultValues[0].DmaxXx));
      Assert.Equal(ca == "A" ? ACaseMaxYy : CCaseMaxYy,
        RoundedAngleValueTo4Digits(resultValues[0].DmaxYy));
      Assert.Equal(ca == "A" ? ACaseMaxZz : CCaseMaxZz,
        RoundedAngleValueTo4Digits(resultValues[0].DmaxZz));
      Assert.Equal(ca == "A" ? ACaseMaxXxyyzz : CCaseMaxXxyyzz,
        RoundedAngleValueTo4Digits(resultValues[0].DmaxXxyyzz));

      Assert.Equal(ca == "A" ? ACaseMinX : CCaseMinX,
        RoundedLengthValueTo4Digits(resultValues[0].DminX));
      Assert.Equal(ca == "A" ? ACaseMinY : CCaseMinY,
        RoundedLengthValueTo4Digits(resultValues[0].DminY));
      Assert.Equal(ca == "A" ? ACaseMinZ : CCaseMinZ,
        RoundedLengthValueTo4Digits(resultValues[0].DminZ));
      Assert.Equal(ca == "A" ? ACaseMinXYZ : CCaseMinXYZ,
        RoundedLengthValueTo4Digits(resultValues[0].DminXyz));
      Assert.Equal(ca == "A" ? ACaseMinXx : CCaseMinXx,
        RoundedAngleValueTo4Digits(resultValues[0].DminXx));
      Assert.Equal(ca == "A" ? ACaseMinYy : CCaseMinYy,
        RoundedAngleValueTo4Digits(resultValues[0].DminYy));
      Assert.Equal(ca == "A" ? ACaseMinZz : CCaseMinZz,
        RoundedAngleValueTo4Digits(resultValues[0].DminZz));
      Assert.Equal(ca == "A" ? ACaseMinXxyyzz : CCaseMinXxyyzz,
        RoundedAngleValueTo4Digits(resultValues[0].DminXxyyzz));
    }

    private static double RoundedLengthValueTo4Digits(IQuantity val) {
      return RoundToSignificantDigits(val.As(LengthUnit.Millimeter), 4);
    }

    private static double RoundedAngleValueTo4Digits(IQuantity val) {
      return RoundToSignificantDigits(val.As(AngleUnit.Radian), 4);
    }
  }
}
