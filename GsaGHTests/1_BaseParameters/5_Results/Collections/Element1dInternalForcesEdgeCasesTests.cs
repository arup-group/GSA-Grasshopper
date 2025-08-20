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
  public class Element1dInternalForcesEdgeCasesTests {

    private static readonly string ElementList = "2 to 6";

    [Fact]
    public void Element1dInternalForcesWithZeroLengthElementTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);

      // Act
      IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Element1dInternalForces.ResultSubset(elementIds, 1); // Single position

      // Assert - even with single position, should have valid results
      Assert.NotEmpty(resultSet.Ids);
      foreach (int id in resultSet.Ids) {
        IList<IEntity1dQuantity<IInternalForce>> forceQuantity = resultSet.Subset[id];
        Assert.Single(forceQuantity);
        Assert.Single(forceQuantity[0].Results); // Only one position
      }
    }

    [Theory]
    [InlineData(50)]
    [InlineData(100)]
    [InlineData(1000)]
    public void Element1dInternalForcesWithLargePositionCountTest(int positionCount) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      var singleElementIds = new ReadOnlyCollection<int>(new List<int> { 2 }); // Use single element for large position test

      // Act
      IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Element1dInternalForces.ResultSubset(singleElementIds, positionCount);

      // Assert
      Assert.Single(resultSet.Ids);
      IList<IEntity1dQuantity<IInternalForce>> forceQuantity = resultSet.Subset[2];
      Assert.Single(forceQuantity);
      Assert.Equal(positionCount, forceQuantity[0].Results.Count);
    }

    [Fact]
    public void Element1dInternalForcesStandardAxisPersistenceTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);

      // Act - Set a valid standard axis
      result.Element1dInternalForces.SetStandardAxis(-1); // Global axis
      IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet1
        = result.Element1dInternalForces.ResultSubset(elementIds, 5);

      // Change to different axis
      result.Element1dInternalForces.SetStandardAxis(-10); // Local axis
      IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet2
        = result.Element1dInternalForces.ResultSubset(elementIds, 5);

      // Assert - Both should succeed with valid axes
      Assert.NotEmpty(resultSet1.Ids);
      Assert.NotEmpty(resultSet2.Ids);
      Assert.Equal(resultSet1.Ids, resultSet2.Ids); // Same elements
    }

    [Fact]
    public void Element1dInternalForcesRoundTripPositionCalculationTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      int positionsCount = 5;

      // Act
      IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Element1dInternalForces.ResultSubset(elementIds, positionsCount);

      // Assert - Verify position keys are correctly calculated
      var expectedPositions = Enumerable.Range(0, positionsCount).Select(
        k => (double)k / (positionsCount - 1)).ToList();

      foreach (int id in resultSet.Ids) {
        IList<IEntity1dQuantity<IInternalForce>> forceQuantity = resultSet.Subset[id];
        Assert.Single(forceQuantity);
        
        var actualPositions = forceQuantity[0].Results.Keys.OrderBy(x => x).ToList();
        
        for (int i = 0; i < expectedPositions.Count; i++) {
          Assert.Equal(expectedPositions[i], actualPositions[i], 10); // Allow for small floating point differences
        }
      }
    }

    [Fact]
    public void Element1dInternalForcesMemoryConsistencyTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);

      // Act - Create multiple result sets and verify they don't interfere
      IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet1
        = result.Element1dInternalForces.ResultSubset(elementIds, 3);
      IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet2
        = result.Element1dInternalForces.ResultSubset(elementIds, 7);
      IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet3
        = result.Element1dInternalForces.ResultSubset(elementIds, 3); // Same as first

      // Assert - Results should be consistent
      Assert.Equal(resultSet1.Ids, resultSet3.Ids);
      Assert.Equal(3, resultSet1.Subset.First().Value.First().Results.Count);
      Assert.Equal(7, resultSet2.Subset.First().Value.First().Results.Count);
      Assert.Equal(3, resultSet3.Subset.First().Value.First().Results.Count);
    }

    [Fact]
    public void Element1dInternalForcesUniqueElementIdsTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      var duplicateElementIds = new ReadOnlyCollection<int>(new List<int> { 2, 3, 2, 4, 3, 5 }); // Duplicates

      // Act
      IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Element1dInternalForces.ResultSubset(duplicateElementIds, 5);

      // Assert - Should handle duplicates gracefully and return unique results
      var uniqueIds = duplicateElementIds.Distinct().Where(id => result.Model.ApiModel.Elements().ContainsKey(id)).OrderBy(x => x).ToList();
      Assert.Equal(uniqueIds, resultSet.Ids.OrderBy(x => x).ToList());
    }

    [Theory]
    [InlineData("2")]
    [InlineData("2 to 3")]
    [InlineData("2,3,4")]
    [InlineData("2 to 4 6")]
    public void Element1dInternalForcesWithDifferentElementListFormatsTest(string elementListFormat) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(elementListFormat, 1);
      IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Element1dInternalForces.ResultSubset(elementIds, 5);

      // Assert
      var expectedIds = result.Model.ApiModel.Elements(elementListFormat).Keys.OrderBy(x => x).ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
      Assert.NotEmpty(resultSet.Ids);
    }

    [Fact]
    public void Element1dInternalForcesResultValidityTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);

      // Act
      IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Element1dInternalForces.ResultSubset(elementIds, 5);

      // Assert - All force values should be finite numbers
      foreach (int id in resultSet.Ids) {
        IList<IEntity1dQuantity<IInternalForce>> forceQuantity = resultSet.Subset[id];
        Assert.Single(forceQuantity);
        
        foreach (var positionResult in forceQuantity[0].Results) {
          IInternalForce forces = positionResult.Value;
          
          // Check that all values are finite (not NaN or infinite)
          Assert.True(double.IsFinite(forces.X.Kilonewtons), $"X force should be finite for element {id}");
          Assert.True(double.IsFinite(forces.Y.Kilonewtons), $"Y force should be finite for element {id}");
          Assert.True(double.IsFinite(forces.Z.Kilonewtons), $"Z force should be finite for element {id}");
          Assert.True(double.IsFinite(forces.Xx.KilonewtonMeters), $"Xx moment should be finite for element {id}");
          Assert.True(double.IsFinite(forces.Yy.KilonewtonMeters), $"Yy moment should be finite for element {id}");
          Assert.True(double.IsFinite(forces.Zz.KilonewtonMeters), $"Zz moment should be finite for element {id}");
        }
      }
    }

    [Fact]
    public void Element1dInternalForcesThreadSafetyTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      var exceptions = new List<Exception>();

      // Act - Simulate concurrent access
      var tasks = new List<System.Threading.Tasks.Task>();
      for (int i = 0; i < 10; i++) {
        int positionCount = 3 + (i % 5); // Vary position count
        tasks.Add(System.Threading.Tasks.Task.Run(() => {
          try {
            IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
              = result.Element1dInternalForces.ResultSubset(elementIds, positionCount);
            Assert.NotEmpty(resultSet.Ids);
          } catch (Exception ex) {
            lock (exceptions) {
              exceptions.Add(ex);
            }
          }
        }));
      }

      System.Threading.Tasks.Task.WaitAll(tasks.ToArray());

      // Assert - No exceptions should occur during concurrent access
      Assert.Empty(exceptions);
    }
  }
}