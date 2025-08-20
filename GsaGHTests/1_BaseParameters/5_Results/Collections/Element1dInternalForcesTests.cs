using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using GsaAPI;

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
      Assert.Equal(expected, max);
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
      Assert.Equal(expected, max);
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
      Assert.Equal(expected, min);
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
      Assert.Equal(expected, min);
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
          Assert.Equal(expected[i++], x);
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
          Assert.Equal(expectedP1[i], perm1);
          double perm2 = TestsResultHelper.ResultsHelper(displacementQuantity[1].Results[position], component);
          Assert.Equal(expectedP2[i++], perm2);
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

    [Fact]
    public void Element1dInternalForcesWithEmptyElementListTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      var emptyElementIds = new ReadOnlyCollection<int>(new List<int>());

      // Act
      IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Element1dInternalForces.ResultSubset(emptyElementIds, 5);

      // Assert
      Assert.Empty(resultSet.Ids);
      Assert.Empty(resultSet.Subset);
    }

    [Fact]
    public void Element1dInternalForcesWithSingleElementTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      var singleElementIds = new ReadOnlyCollection<int>(new List<int> { 2 });

      // Act
      IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Element1dInternalForces.ResultSubset(singleElementIds, 5);

      // Assert
      Assert.Single(resultSet.Ids);
      Assert.Equal(2, resultSet.Ids.First());
      Assert.Single(resultSet.Subset);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)] 
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(6)]
    [InlineData(10)]
    public void Element1dInternalForcesWithDifferentPositionCountsTest(int positionCount) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);

      // Act
      IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Element1dInternalForces.ResultSubset(elementIds, positionCount);

      // Assert
      Assert.NotEmpty(resultSet.Ids);
      foreach (int id in resultSet.Ids) {
        IList<IEntity1dQuantity<IInternalForce>> forceQuantity = resultSet.Subset[id];
        Assert.Single(forceQuantity); // for analysis case results we expect 1 permutation
        Assert.Equal(positionCount, forceQuantity[0].Results.Count);
      }
    }

    [Fact]
    public void Element1dInternalForcesWithNonExistentElementIdTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      var nonExistentElementIds = new ReadOnlyCollection<int>(new List<int> { 999999 });

      // Act
      IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Element1dInternalForces.ResultSubset(nonExistentElementIds, 5);

      // Assert
      Assert.Empty(resultSet.Ids);
      Assert.Empty(resultSet.Subset);
    }

    [Fact]
    public void Element1dInternalForcesWithMixedValidAndInvalidElementIdsTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      var mixedElementIds = new ReadOnlyCollection<int>(new List<int> { 2, 999999, 3, 888888 });

      // Act
      IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Element1dInternalForces.ResultSubset(mixedElementIds, 5);

      // Assert
      Assert.Equal(2, resultSet.Ids.Count); // only valid IDs should be returned
      Assert.Contains(2, resultSet.Ids);
      Assert.Contains(3, resultSet.Ids);
      Assert.DoesNotContain(999999, resultSet.Ids);
      Assert.DoesNotContain(888888, resultSet.Ids);
    }

    [Fact]
    public void Element1dInternalForcesElementIdsFromDifferentTestFileTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignSimple, 1);
      string elementList = "All";

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(elementList, 1);
      IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Element1dInternalForces.ResultSubset(elementIds, 5);

      // Assert element IDs
      var expectedIds = result.Model.ApiModel.Elements(elementList).Keys.OrderBy(x => x).ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
      Assert.NotEmpty(resultSet.Ids);
    }

    [Fact]
    public void Element1dInternalForcesResultSubsetConsistencyTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);

      // Act - call ResultSubset multiple times
      IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet1
        = result.Element1dInternalForces.ResultSubset(elementIds, 5);
      IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet2
        = result.Element1dInternalForces.ResultSubset(elementIds, 5);

      // Assert - results should be consistent
      Assert.Equal(resultSet1.Ids, resultSet2.Ids);
      Assert.Equal(resultSet1.Subset.Count, resultSet2.Subset.Count);
      
      foreach (int id in resultSet1.Ids) {
        Assert.Equal(resultSet1.Subset[id].Count, resultSet2.Subset[id].Count);
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
    public void Element1dInternalForcesExtremaConsistencyTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Element1dInternalForces.ResultSubset(elementIds, 5);

      // Act - get max and min values
      double max = TestsResultHelper.ResultsHelper(resultSet, component, true);
      double min = TestsResultHelper.ResultsHelper(resultSet, component, false);

      // Assert - max should be greater than or equal to min
      Assert.True(max >= min, $"Max value ({max}) should be >= min value ({min}) for component {component}");
    }

    [Fact]
    public void Element1dInternalForcesAllPositionsNonNullTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      int positionsCount = 5;

      // Act
      IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Element1dInternalForces.ResultSubset(elementIds, positionsCount);

      // Assert - all positions should have non-null results
      foreach (int id in resultSet.Ids) {
        IList<IEntity1dQuantity<IInternalForce>> forceQuantity = resultSet.Subset[id];
        Assert.Single(forceQuantity);
        
        var positions = Enumerable.Range(0, positionsCount).Select(
          k => (double)k / (positionsCount - 1)).ToList();
        
        foreach (double position in positions) {
          IInternalForce forces = forceQuantity[0].Results[position];
          Assert.NotNull(forces);
          Assert.NotNull(forces.X);
          Assert.NotNull(forces.Y);
          Assert.NotNull(forces.Z);
          Assert.NotNull(forces.Xx);
          Assert.NotNull(forces.Yy);
          Assert.NotNull(forces.Zz);
        }
      }
    }

    [Fact]
    public void Element1dInternalForcesLargeElementListPerformanceTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      string largeElementList = "All"; // Use all elements to stress test
      
      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(largeElementList, 1);
      var stopwatch = System.Diagnostics.Stopwatch.StartNew();
      IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Element1dInternalForces.ResultSubset(elementIds, 5);
      stopwatch.Stop();

      // Assert - performance should be reasonable (under 5 seconds for typical models)
      Assert.True(stopwatch.ElapsedMilliseconds < 5000, 
        $"Performance test failed: took {stopwatch.ElapsedMilliseconds} ms");
      Assert.NotEmpty(resultSet.Ids);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-5)]
    public void Element1dInternalForcesInvalidPositionCountThrowsExceptionTest(int invalidPositionCount) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);

      // Act & Assert
      Assert.Throws<ArgumentOutOfRangeException>(() => 
        result.Element1dInternalForces.ResultSubset(elementIds, invalidPositionCount));
    }

    [Fact]
    public void Element1dInternalForcesNullElementIdsThrowsExceptionTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);

      // Act & Assert
      Assert.Throws<ArgumentNullException>(() => 
        result.Element1dInternalForces.ResultSubset(null, 5));
    }

    [Fact]
    public void Element1dInternalForcesSubsetHasCorrectKeysTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);

      // Act
      IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Element1dInternalForces.ResultSubset(elementIds, 5);

      // Assert - every element ID should have corresponding entries in Subset
      foreach (int id in resultSet.Ids) {
        Assert.True(resultSet.Subset.ContainsKey(id), 
          $"Subset should contain key for element ID {id}");
        Assert.NotNull(resultSet.Subset[id]);
        Assert.NotEmpty(resultSet.Subset[id]);
      }
    }

    [Fact]
    public void Element1dInternalForcesCompareTwoDifferentModelsTest() {
      // Assemble
      var result1 = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      var result2 = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignSimple, 1);
      
      ReadOnlyCollection<int> elementIds1 = result1.ElementIds("All", 1);
      ReadOnlyCollection<int> elementIds2 = result2.ElementIds("All", 1);

      // Act
      IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet1
        = result1.Element1dInternalForces.ResultSubset(elementIds1, 5);
      IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet2
        = result2.Element1dInternalForces.ResultSubset(elementIds2, 5);

      // Assert - different models should have different results
      Assert.NotEmpty(resultSet1.Ids);
      Assert.NotEmpty(resultSet2.Ids);
      // Element IDs may be different between models
      // This test validates that both models can be processed successfully
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(1, 4)]
    public void Element1dInternalForcesConsistentResultsAcrossCaseTypesTest(int analysisCaseId, int combinationCaseId) {
      // Assemble
      var analysisResult = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, analysisCaseId);
      var combinationResult = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, combinationCaseId);
      
      ReadOnlyCollection<int> elementIds = analysisResult.ElementIds(ElementList, 1);

      // Act
      IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> analysisResultSet
        = analysisResult.Element1dInternalForces.ResultSubset(elementIds, 5);
      IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> combinationResultSet
        = combinationResult.Element1dInternalForces.ResultSubset(elementIds, 5);

      // Assert - both result types should return valid data for same element IDs
      Assert.Equal(analysisResultSet.Ids, combinationResultSet.Ids);
      Assert.Equal(analysisResultSet.Subset.Count, combinationResultSet.Subset.Count);
      
      foreach (int id in analysisResultSet.Ids) {
        Assert.Single(analysisResultSet.Subset[id]); // Analysis case has 1 permutation
        Assert.Equal(2, combinationResultSet.Subset[id].Count); // Combination case has 2 permutations
      }
    }
  }
}
