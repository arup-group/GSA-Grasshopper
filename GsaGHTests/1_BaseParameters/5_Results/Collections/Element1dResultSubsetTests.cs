using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;

using GsaGH.Parameters.Results;

using GsaGHTests.Helper;

using Xunit;

namespace GsaGHTests.Parameters.Results {
  [Collection("GrasshopperFixture collection")]
  public class Element1dResultSubsetTests {

    [Fact]
    public void GetMissingKeysTests() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds("1", 1);
      IEntity1dResultSubset<IDisplacement, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Element1dDisplacements.ResultSubset(elementIds, 1);

      // Assert
      var positions = new ReadOnlyCollection<double>(new Collection<double>() { 0.0, 1.0 });
      var newKeys = new Collection<int>() { 2 };
      ConcurrentBag<int> missingIds = result.Element1dDisplacements.Cache.
        GetMissingKeysAndPositions<IDisplacement>(newKeys, positions);
      Assert.Single(missingIds);
      Assert.Equal(newKeys[0], missingIds.First());
    }

    [Fact]
    public void GetMissingKeysAndPositionsTests() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds("1", 1);
      IEntity1dResultSubset<IDisplacement, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Element1dDisplacements.ResultSubset(elementIds, 1);

      // Assert
      var positions = new ReadOnlyCollection<double>(new Collection<double>() { 0.0, 1.0 });
      var newKeys = new Collection<int>() { 1 };
      ConcurrentBag<int> missingIds = result.Element1dDisplacements.Cache.
        GetMissingKeysAndPositions<IDisplacement>(newKeys, positions);
      Assert.Single(missingIds);
      Assert.Equal(newKeys[0], missingIds.First());
    }
  }
}
