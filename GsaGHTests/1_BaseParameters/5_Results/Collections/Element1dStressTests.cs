using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using GsaAPI;

using GsaGH.Parameters.Results;

using GsaGHTests.Helper;
using GsaGHTests.TestHelpers;

using Xunit;

namespace GsaGHTests.Parameters.Results {
  [Collection("GrasshopperFixture collection")]
  public class Element1dStressTests {
    private static readonly string ElementList = "2 to 3";

    [Fact]
    public void Element1dStressElement1dIdsFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IStress1d, ResultStress1d<Entity1dExtremaKey>> resultSet
        = result.Element1dStresses.ResultSubset(elementIds, 1);

      // Assert element IDs
      var expectedIds = result.Model.ApiModel.Elements(ElementList).Keys.OrderBy(x => x).ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
    }

    [Fact]
    public void Element1dStresssElement1dIdsFromCombinationCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IStress1d, ResultStress1d<Entity1dExtremaKey>> resultSet
        = result.Element1dStresses.ResultSubset(elementIds, 1);

      // Assert element IDs
      var expectedIds = result.Model.ApiModel.Elements(ElementList).Keys.OrderBy(x => x).ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
    }

    [Theory]
    [InlineData(ResultStress1d.Axial)]
    [InlineData(ResultStress1d.ShearY)]
    [InlineData(ResultStress1d.ShearZ)]
    [InlineData(ResultStress1d.ByPos)]
    [InlineData(ResultStress1d.ByNeg)]
    [InlineData(ResultStress1d.BzPos)]
    [InlineData(ResultStress1d.BzNeg)]
    [InlineData(ResultStress1d.C1)]
    [InlineData(ResultStress1d.C2)]
    public void Element1dStresssMaxFromAnalysisCaseTest(ResultStress1d component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      double expected = ExpectedAnalysisCaseValues(component).Max();

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IStress1d, ResultStress1d<Entity1dExtremaKey>> resultSet
        = result.Element1dStresses.ResultSubset(elementIds, 4);

      // Assert Max in set
      double max = TestsResultHelper.ResultsHelper(resultSet, component, true);

      DoubleAssertHelper.Equals(expected, max);
    }

    [Theory]
    [InlineData(ResultStress1d.Axial)]
    [InlineData(ResultStress1d.ShearY)]
    [InlineData(ResultStress1d.ShearZ)]
    [InlineData(ResultStress1d.ByPos)]
    [InlineData(ResultStress1d.ByNeg)]
    [InlineData(ResultStress1d.BzPos)]
    [InlineData(ResultStress1d.BzNeg)]
    [InlineData(ResultStress1d.C1)]
    [InlineData(ResultStress1d.C2)]
    public void Element1dStresssMaxFromCombinationCaseTest(ResultStress1d component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      double expected = Math.Max(ExpectedCombinationCaseC4p1Values(component).Max(),
        ExpectedCombinationCaseC4p2Values(component).Max());

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IStress1d, ResultStress1d<Entity1dExtremaKey>> resultSet
        = result.Element1dStresses.ResultSubset(elementIds, 4);

      // Assert Max in set
      double max = TestsResultHelper.ResultsHelper(resultSet, component, true);
      DoubleAssertHelper.Equals(expected, max);
    }

    [Theory]
    [InlineData(ResultStress1d.Axial)]
    [InlineData(ResultStress1d.ShearY)]
    [InlineData(ResultStress1d.ShearZ)]
    [InlineData(ResultStress1d.ByPos)]
    [InlineData(ResultStress1d.ByNeg)]
    [InlineData(ResultStress1d.BzPos)]
    [InlineData(ResultStress1d.BzNeg)]
    [InlineData(ResultStress1d.C1)]
    [InlineData(ResultStress1d.C2)]
    public void Element1dStresssMinFromAnalysisCaseTest(ResultStress1d component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      double expected = ExpectedAnalysisCaseValues(component).Min();

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IStress1d, ResultStress1d<Entity1dExtremaKey>> resultSet
        = result.Element1dStresses.ResultSubset(elementIds, 4);

      // Assert Max in set
      double min = TestsResultHelper.ResultsHelper(resultSet, component, false);
      Assert.Equal(expected, min);
    }

    [Theory]
    [InlineData(ResultStress1d.Axial)]
    [InlineData(ResultStress1d.ShearY)]
    [InlineData(ResultStress1d.ShearZ)]
    [InlineData(ResultStress1d.ByPos)]
    [InlineData(ResultStress1d.ByNeg)]
    [InlineData(ResultStress1d.BzPos)]
    [InlineData(ResultStress1d.BzNeg)]
    [InlineData(ResultStress1d.C1)]
    [InlineData(ResultStress1d.C2)]
    public void Element1dStresssMinFromcombinationCaseTest(ResultStress1d component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      double expected = Math.Min(ExpectedCombinationCaseC4p1Values(component).Min(),
        ExpectedCombinationCaseC4p2Values(component).Min());

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IStress1d, ResultStress1d<Entity1dExtremaKey>> resultSet
        = result.Element1dStresses.ResultSubset(elementIds, 4);

      // Assert Max in set
      double min = TestsResultHelper.ResultsHelper(resultSet, component, false);
      Assert.Equal(expected, min);
    }

    [Theory]
    [InlineData(ResultStress1d.Axial)]
    [InlineData(ResultStress1d.ShearY)]
    [InlineData(ResultStress1d.ShearZ)]
    [InlineData(ResultStress1d.ByPos)]
    [InlineData(ResultStress1d.ByNeg)]
    [InlineData(ResultStress1d.BzPos)]
    [InlineData(ResultStress1d.BzNeg)]
    [InlineData(ResultStress1d.C1)]
    [InlineData(ResultStress1d.C2)]
    public void Element1dStresssValuesFromAnalysisCaseTest(ResultStress1d component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      List<double> expected = ExpectedAnalysisCaseValues(component);
      int positionsCount = 4;

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IStress1d, ResultStress1d<Entity1dExtremaKey>> resultSet
        = result.Element1dStresses.ResultSubset(elementIds, positionsCount);

      // Assert result values
      int i = 0;
      foreach (int id in resultSet.Ids) {
        IList<IEntity1dQuantity<IStress1d>> stressQuantity = resultSet.Subset[id];

        // for analysis case results we expect 4 positions
        Assert.Single(stressQuantity);
        var positions = Enumerable.Range(0, positionsCount).Select(
        k => (double)k / (positionsCount - 1)).ToList();
        foreach (double position in positions) {
          double x = TestsResultHelper.ResultsHelper(stressQuantity[0].Results[position], component);
          DoubleAssertHelper.Equals(expected[i++], x);
        }
      }
    }

    [Theory]
    [InlineData(ResultStress1d.Axial)]
    [InlineData(ResultStress1d.ShearY)]
    [InlineData(ResultStress1d.ShearZ)]
    [InlineData(ResultStress1d.ByPos)]
    [InlineData(ResultStress1d.ByNeg)]
    [InlineData(ResultStress1d.BzPos)]
    [InlineData(ResultStress1d.BzNeg)]
    [InlineData(ResultStress1d.C1)]
    [InlineData(ResultStress1d.C2)]
    public void Element1dStresssValuesFromCombinationCaseTest(ResultStress1d component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      List<double> expectedP1 = ExpectedCombinationCaseC4p1Values(component);
      List<double> expectedP2 = ExpectedCombinationCaseC4p2Values(component);
      int positionsCount = 4;

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IStress1d, ResultStress1d<Entity1dExtremaKey>> resultSet
        = result.Element1dStresses.ResultSubset(elementIds, positionsCount);

      // Assert result values
      int i = 0;
      foreach (int id in resultSet.Ids) {
        IList<IEntity1dQuantity<IStress1d>> displacementQuantity = resultSet.Subset[id];

        // for C4 case results we expect two permutations in the collection
        Assert.Equal(2, displacementQuantity.Count);

        var positions = Enumerable.Range(0, positionsCount).Select(
        k => (double)k / (positionsCount - 1)).ToList();
        foreach (double position in positions) {
          double perm1 = TestsResultHelper.ResultsHelper(displacementQuantity[0].Results[position], component);
          Assert.Equal(expectedP1[i], perm1);
          double perm2 = TestsResultHelper.ResultsHelper(displacementQuantity[1].Results[position], component);
          Assert.Equal(expectedP2[i++], perm2);
        }
      }
    }

    [Fact]
    public void Element1dStresssCacheChangePositionsTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      int positionsCount = 5;

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IStress1d, ResultStress1d<Entity1dExtremaKey>> resultSet
        = result.Element1dStresses.ResultSubset(elementIds, positionsCount);

      // Assert
      Assert.Equal(positionsCount,
        result.Element1dStresses.Cache.FirstOrDefault().Value.FirstOrDefault().Results.Count);
      Assert.Equal(positionsCount,
        resultSet.Subset.FirstOrDefault().Value.FirstOrDefault().Results.Count);

      // Act again
      int newPositionsCount = 4;
      resultSet = result.Element1dStresses.ResultSubset(elementIds, newPositionsCount);

      // Assert again
      Assert.NotEqual(newPositionsCount,
        result.Element1dStresses.Cache.FirstOrDefault().Value.FirstOrDefault().Results.Count);
      Assert.Equal(newPositionsCount,
        resultSet.Subset.FirstOrDefault().Value.FirstOrDefault().Results.Count);
    }

    private List<double> ExpectedAnalysisCaseValues(ResultStress1d component) {
      switch (component) {
        case ResultStress1d.Axial: return Element1dStressA1.AxialInMPa();
        case ResultStress1d.ShearY: return Element1dStressA1.SyInMPa();
        case ResultStress1d.ShearZ: return Element1dStressA1.SzInMPa();
        case ResultStress1d.ByPos: return Element1dStressA1.ByPosInMPa();
        case ResultStress1d.ByNeg: return Element1dStressA1.ByNegInMPa();
        case ResultStress1d.BzPos: return Element1dStressA1.BzPosInMPa();
        case ResultStress1d.BzNeg: return Element1dStressA1.BzNegInMPa();
        case ResultStress1d.C1: return Element1dStressA1.C1InMPa();
        case ResultStress1d.C2: return Element1dStressA1.C2InMPa();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC4p1Values(ResultStress1d component) {
      switch (component) {
        case ResultStress1d.Axial: return Element1dStressC4p1.AxialInMPa();
        case ResultStress1d.ShearY: return Element1dStressC4p1.SyInMPa();
        case ResultStress1d.ShearZ: return Element1dStressC4p1.SzInMPa();
        case ResultStress1d.ByPos: return Element1dStressC4p1.ByPosInMPa();
        case ResultStress1d.ByNeg: return Element1dStressC4p1.ByNegInMPa();
        case ResultStress1d.BzPos: return Element1dStressC4p1.BzPosInMPa();
        case ResultStress1d.BzNeg: return Element1dStressC4p1.BzNegInMPa();
        case ResultStress1d.C1: return Element1dStressC4p1.C1InMPa();
        case ResultStress1d.C2: return Element1dStressC4p1.C2InMPa();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC4p2Values(ResultStress1d component) {
      switch (component) {
        case ResultStress1d.Axial: return Element1dStressC4p2.AxialInMPa();
        case ResultStress1d.ShearY: return Element1dStressC4p2.SyInMPa();
        case ResultStress1d.ShearZ: return Element1dStressC4p2.SzInMPa();
        case ResultStress1d.ByPos: return Element1dStressC4p2.ByPosInMPa();
        case ResultStress1d.ByNeg: return Element1dStressC4p2.ByNegInMPa();
        case ResultStress1d.BzPos: return Element1dStressC4p2.BzPosInMPa();
        case ResultStress1d.BzNeg: return Element1dStressC4p2.BzNegInMPa();
        case ResultStress1d.C1: return Element1dStressC4p2.C1InMPa();
        case ResultStress1d.C2: return Element1dStressC4p2.C2InMPa();
      }

      throw new NotImplementedException();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(0)]
    [InlineData(-2)]
    [InlineData(-11)]
    [InlineData(-12)]
    [InlineData(-13)]
    [InlineData(-14)]
    public void SetAxisThrowsExceptionTest(int axisId) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);

      // Act
      result.Element1dStresses.SetStandardAxis(axisId);
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);

      // Assert
      Assert.Throws<GsaApiException>(() => result.Element1dStresses.ResultSubset(elementIds, 4));
    }
  }
}
