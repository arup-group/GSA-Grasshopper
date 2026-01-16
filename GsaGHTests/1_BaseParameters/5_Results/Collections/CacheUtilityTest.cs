using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using GsaGHTests.Helper;
using GsaGHTests.Parameters;

using Xunit;

namespace GsaGH.Parameters.Results {
  [Collection("GrasshopperFixture collection")]
  public class CacheUtilityTest {
    // we have ids: 2, 3, 5, 6
    // we don't have 1, 4

    [Theory]
    [InlineData("2", 1, 3, true)]
    [InlineData("2", 1, -3, true)]
    [InlineData("2", 1, 2, false)]
    [InlineData(null, 0, -3, true)]
    public void GetMissingKeysTests(
      string elementList, int cacheCount, int newKeyId, bool newKeyIsMissing) {
      // Assemble
      GsaResult result = GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);

      // Act
      int invalidDimension = 666;
      int validDimension = 1;
      ReadOnlyCollection<int> elementIds = result.ElementIds(elementList, string.IsNullOrEmpty(elementList) ? invalidDimension : validDimension);
      IEntity1dResultSubset<IDisplacement, ResultVector6<Entity1dExtremaKey>>
        resultSet = result.Element1dDisplacements.ResultSubset(elementIds, 1);

      if (cacheCount == 0) {
        Assert.Empty(result.Element1dDisplacements.Cache);
      } else {
        Assert.Single(result.Element1dDisplacements.Cache);
      }

      var newKeys = new Collection<int>(new List<int>() {
        newKeyId,
      });
      // Assert
      ConcurrentBag<int> missingIds = result.Element1dDisplacements.Cache.GetMissingKeys(newKeys);

      if (newKeyIsMissing) {
        Assert.Single(missingIds);
        Assert.Equal(newKeys[0], missingIds.First());
      } else {
        Assert.Empty(missingIds);
      }
    }

    [Theory]
    [InlineData("2", 1, 3, true)]
    [InlineData("2", 1, -3, true)]
    [InlineData("2", 1, 2, false)]
    [InlineData(null, 0, -3, true)]
    public void GetMissingKeysAndPositionsTests(string elementList, int cacheCount, int newKeyId, bool newKeyIsMissing) {
      // Assemble
      GsaResult result = GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);

      // Act
      int invalidDimension = 666;
      int validDimension = 1;

      ReadOnlyCollection<int> elementIds = result.ElementIds(elementList, string.IsNullOrEmpty(elementList) ? invalidDimension : validDimension);
      IEntity1dResultSubset<IDisplacement, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Element1dDisplacements.ResultSubset(elementIds, 2);

      if (cacheCount == 0) {
        Assert.Empty(result.Element1dDisplacements.Cache);
      } else {
        Assert.Single(result.Element1dDisplacements.Cache);
      }

      // Assert
      var positions = new ReadOnlyCollection<double>(new Collection<double>() { 0.0, 1.0 });
      var newKeys = new Collection<int>() { newKeyId };

      ConcurrentBag<int> missingIds = result.Element1dDisplacements.Cache.
        GetMissingKeysAndPositions<IDisplacement>(newKeys, positions);

      if (newKeyIsMissing) {
        Assert.Single(missingIds);
        Assert.Equal(newKeys[0], missingIds.First());
      } else {
        Assert.Empty(missingIds);
      }
    }

    [Theory]
    [InlineData(3, true)]
    [InlineData(4, false)]
    [InlineData(-4, false)]
    public void GetSubsetIsValidWhenKeyIsValid(int newKey, bool newKeyIsValid) {
      // Assemble
      GsaResult result = GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds("2 3 4 5", 1);
      IEntity1dResultSubset<IDisplacement, ResultVector6<Entity1dExtremaKey>>
        resultSet = result.Element1dDisplacements.ResultSubset(elementIds, 2);

      Assert.Equal(3, result.Element1dDisplacements.Cache.Count);

      // Assert
      var newKeys = new Collection<int>() {
        newKey
      };

      IDictionary<int, IList<IEntity1dQuantity<IDisplacement>>> subset
        = result.Element1dDisplacements.Cache.GetSubset(newKeys);

      if (newKeyIsValid) {
        Assert.Single(subset);
        Assert.Equal(newKey, subset.Keys.First());
      } else {
        Assert.Empty(subset);
      }
    }
  }
}
