using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;

using GsaGH.Parameters.Results;

using GsaGHTests.Helper;

using Xunit;

namespace GsaGHTests.Parameters.Results {
  [Collection("GrasshopperFixture collection")]
  public class NodeResultsCacheTests {

    [Fact]
    public void GetMissingKeysEmptyTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds("442");
      ConcurrentBag<int> missingIds
        = result.NodeDisplacements.Cache.GetMissingKeys(nodeIds);
      Assert.Single(missingIds);
      Assert.Equal(442, missingIds.First());
    }

    [Fact]
    public void GetMissingKeysUpdateTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds("442");
      IEntity0dResultSubset<IDisplacement, ResultVector6<Entity0dExtremaKey>> resultSet
        = result.NodeDisplacements.ResultSubset(nodeIds);
      nodeIds = result.NodeIds("444");
      ConcurrentBag<int> missingIds
        = result.NodeDisplacements.Cache.GetMissingKeys(nodeIds);

      Assert.Single(missingIds);
      Assert.Equal(444, missingIds.First());
    }
  }
}
