using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using GsaAPI;

using GsaGH.Helpers;
using GsaGH.Parameters.Results;

using GsaGHTests.Helper;

using Xunit;

namespace GsaGHTests.Parameters.Results {
  [Collection("GrasshopperFixture collection")]
  public class Element1dInternalForcesTests {

    private static readonly string ElementList = "2 to 6";

    [Fact]
    public void Element1dInternalForcesElement1dIdsFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Element1dInternalForces.ResultSubset(elementIds, 1);

      // Assert element IDs
      var expectedIds = result.Model.ApiModel.Elements(ElementList).Keys.OrderBy(x => x).ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
    }

    [Fact]
    public void Element1dInternalForcesElement1dIdsFromCombinationCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Element1dInternalForces.ResultSubset(elementIds, 1);

      // Assert element IDs
      var expectedIds = result.Model.ApiModel.Elements(ElementList).Keys.OrderBy(x => x).ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
    }

    [Theory]
    [InlineData(ResultVector6.X)]
    [InlineData(ResultVector6.Y)]
    [InlineData(ResultVector6.Z)]
    [InlineData(ResultVector6.Xyz)]
    [InlineData(ResultVector6.Xx)]
    [InlineData(ResultVector6.Yy)]
    [InlineData(ResultVector6.Zz)]
    [InlineData(ResultVector6.Xxyyzz)]
    public void Element1dInternalForcesMaxFromAnalysisCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      double expected = ExpectedAnalysisCaseValues(component).Max();

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Element1dInternalForces.ResultSubset(elementIds, 5);

      // Assert Max in set
      double max = TestsResultHelper.ResultsHelper(resultSet, component, true);
      Assert.Equal(expected, max, DoubleComparer.Default);
    }

    [Theory]
    [InlineData(ResultVector6.X)]
    [InlineData(ResultVector6.Y)]
    [InlineData(ResultVector6.Z)]
    [InlineData(ResultVector6.Xyz)]
    [InlineData(ResultVector6.Xx)]
    [InlineData(ResultVector6.Yy)]
    [InlineData(ResultVector6.Zz)]
    [InlineData(ResultVector6.Xxyyzz)]
    public void Element1dInternalForcesMaxFromCombinationCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      double expected = Math.Max(ExpectedCombinationCaseC4p1Values(component).Max(),
        ExpectedCombinationCaseC4p2Values(component).Max());

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Element1dInternalForces.ResultSubset(elementIds, 5);

      // Assert Max in set
      double max = TestsResultHelper.ResultsHelper(resultSet, component, true);
      Assert.Equal(expected, max, DoubleComparer.Default);
    }

    [Theory]
    [InlineData(ResultVector6.X)]
    [InlineData(ResultVector6.Y)]
    [InlineData(ResultVector6.Z)]
    [InlineData(ResultVector6.Xyz)]
    [InlineData(ResultVector6.Xx)]
    [InlineData(ResultVector6.Yy)]
    [InlineData(ResultVector6.Zz)]
    [InlineData(ResultVector6.Xxyyzz)]
    public void Element1dInternalForcesMinFromAnalysisCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      double expected = ExpectedAnalysisCaseValues(component).Min();

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Element1dInternalForces.ResultSubset(elementIds, 5);

      // Assert Max in set
      double min = TestsResultHelper.ResultsHelper(resultSet, component, false);
      Assert.Equal(expected, min, DoubleComparer.Default);
    }

    [Theory]
    [InlineData(ResultVector6.X)]
    [InlineData(ResultVector6.Y)]
    [InlineData(ResultVector6.Z)]
    [InlineData(ResultVector6.Xyz)]
    [InlineData(ResultVector6.Xx)]
    [InlineData(ResultVector6.Yy)]
    [InlineData(ResultVector6.Zz)]
    [InlineData(ResultVector6.Xxyyzz)]
    public void Element1dInternalForcesMinFromcombinationCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      double expected = Math.Min(ExpectedCombinationCaseC4p1Values(component).Min(),
        ExpectedCombinationCaseC4p2Values(component).Min());

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Element1dInternalForces.ResultSubset(elementIds, 5);

      // Assert Max in set
      double min = TestsResultHelper.ResultsHelper(resultSet, component, false);
      Assert.Equal(expected, min, DoubleComparer.Default);
    }

    [Theory]
    [InlineData(ResultVector6.X)]
    [InlineData(ResultVector6.Y)]
    [InlineData(ResultVector6.Z)]
    [InlineData(ResultVector6.Xyz)]
    [InlineData(ResultVector6.Xx)]
    [InlineData(ResultVector6.Yy)]
    [InlineData(ResultVector6.Zz)]
    [InlineData(ResultVector6.Xxyyzz)]
    public void Element1dInternalForcesValuesFromAnalysisCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      List<double> expected = ExpectedAnalysisCaseValues(component);
      int positionsCount = 5;

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Element1dInternalForces.ResultSubset(elementIds, positionsCount);

      // Assert result values
      int i = 0;
      foreach (int id in resultSet.Ids) {
        IList<IEntity1dQuantity<IInternalForce>> displacementQuantity = resultSet.Subset[id];

        // for analysis case results we expect 4 positions
        Assert.Single(displacementQuantity);
        var positions = Enumerable.Range(0, positionsCount).Select(
        k => (double)k / (positionsCount - 1)).ToList();
        foreach (double position in positions) {
          double x = TestsResultHelper.ResultsHelper(displacementQuantity[0].Results[position], component);
          Assert.Equal(expected[i++], x, DoubleComparer.Default);
        }
      }
    }

    [Theory]
    [InlineData(ResultVector6.X)]
    [InlineData(ResultVector6.Y)]
    [InlineData(ResultVector6.Z)]
    [InlineData(ResultVector6.Xyz)]
    [InlineData(ResultVector6.Xx)]
    [InlineData(ResultVector6.Yy)]
    [InlineData(ResultVector6.Zz)]
    [InlineData(ResultVector6.Xxyyzz)]
    public void Element1dInternalForcesValuesFromCombinationCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      List<double> expectedP1 = ExpectedCombinationCaseC4p1Values(component);
      List<double> expectedP2 = ExpectedCombinationCaseC4p2Values(component);
      int positionsCount = 5;

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Element1dInternalForces.ResultSubset(elementIds, positionsCount);

      // Assert result values
      int i = 0;
      foreach (int id in resultSet.Ids) {
        IList<IEntity1dQuantity<IInternalForce>> displacementQuantity = resultSet.Subset[id];

        // for C4 case results we expect two permutations in the collection
        Assert.Equal(2, displacementQuantity.Count);

        var positions = Enumerable.Range(0, positionsCount).Select(
        k => (double)k / (positionsCount - 1)).ToList();
        foreach (double position in positions) {
          double perm1 = TestsResultHelper.ResultsHelper(displacementQuantity[0].Results[position], component);
          Assert.Equal(expectedP1[i], perm1, DoubleComparer.Default);
          double perm2 = TestsResultHelper.ResultsHelper(displacementQuantity[1].Results[position], component);
          Assert.Equal(expectedP2[i++], perm2, DoubleComparer.Default);
        }
      }
    }

    [Fact]
    public void Element1dInternalForcesCacheChangePositionsTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      int positionsCount = 5;

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Element1dInternalForces.ResultSubset(elementIds, positionsCount);

      // Assert
      Assert.Equal(positionsCount,
        result.Element1dInternalForces.Cache.FirstOrDefault().Value.FirstOrDefault().Results.Count);
      Assert.Equal(positionsCount,
        resultSet.Subset.FirstOrDefault().Value.FirstOrDefault().Results.Count);

      // Act again
      int newPositionsCount = 4;
      resultSet = result.Element1dInternalForces.ResultSubset(elementIds, newPositionsCount);

      // Assert again
      Assert.NotEqual(newPositionsCount,
        result.Element1dInternalForces.Cache.FirstOrDefault().Value.FirstOrDefault().Results.Count);
      Assert.Equal(newPositionsCount,
        resultSet.Subset.FirstOrDefault().Value.FirstOrDefault().Results.Count);
    }

    private List<double> ExpectedAnalysisCaseValues(ResultVector6 component) {
      switch (component) {
        case ResultVector6.X: return Element1dForcesAndMomentsA1.XInKiloNewton();
        case ResultVector6.Y: return Element1dForcesAndMomentsA1.YInKiloNewton();
        case ResultVector6.Z: return Element1dForcesAndMomentsA1.ZInKiloNewton();
        case ResultVector6.Xyz: return Element1dForcesAndMomentsA1.YzInKiloNewton();
        case ResultVector6.Xx: return Element1dForcesAndMomentsA1.XxInKiloNewtonMeter();
        case ResultVector6.Yy: return Element1dForcesAndMomentsA1.YyInKiloNewtonMeter();
        case ResultVector6.Zz: return Element1dForcesAndMomentsA1.ZzInKiloNewtonMeter();
        case ResultVector6.Xxyyzz: return Element1dForcesAndMomentsA1.YyzzInKiloNewtonMeter();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC4p1Values(ResultVector6 component) {
      switch (component) {
        case ResultVector6.X: return Element1dForcesAndMomentsC4p1.XInKiloNewton();
        case ResultVector6.Y: return Element1dForcesAndMomentsC4p1.YInKiloNewton();
        case ResultVector6.Z: return Element1dForcesAndMomentsC4p1.ZInKiloNewton();
        case ResultVector6.Xyz: return Element1dForcesAndMomentsC4p1.YzInKiloNewton();
        case ResultVector6.Xx: return Element1dForcesAndMomentsC4p1.XxInKiloNewtonMeter();
        case ResultVector6.Yy: return Element1dForcesAndMomentsC4p1.YyInKiloNewtonMeter();
        case ResultVector6.Zz: return Element1dForcesAndMomentsC4p1.ZzInKiloNewtonMeter();
        case ResultVector6.Xxyyzz: return Element1dForcesAndMomentsC4p1.YyzzInKiloNewtonMeter();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC4p2Values(ResultVector6 component) {
      switch (component) {
        case ResultVector6.X: return Element1dForcesAndMomentsC4p2.XInKiloNewton();
        case ResultVector6.Y: return Element1dForcesAndMomentsC4p2.YInKiloNewton();
        case ResultVector6.Z: return Element1dForcesAndMomentsC4p2.ZInKiloNewton();
        case ResultVector6.Xyz: return Element1dForcesAndMomentsC4p2.YzInKiloNewton();
        case ResultVector6.Xx: return Element1dForcesAndMomentsC4p2.XxInKiloNewtonMeter();
        case ResultVector6.Yy: return Element1dForcesAndMomentsC4p2.YyInKiloNewtonMeter();
        case ResultVector6.Zz: return Element1dForcesAndMomentsC4p2.ZzInKiloNewtonMeter();
        case ResultVector6.Xxyyzz: return Element1dForcesAndMomentsC4p2.YyzzInKiloNewtonMeter();
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
      result.Element1dInternalForces.SetStandardAxis(axisId);
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);

      // Assert
      Assert.Throws<GsaApiException>(() => result.Element1dInternalForces.ResultSubset(elementIds, 4));
    }
  }
}
