using System;

using GsaGH.Parameters.Results;

using GsaGHTests.Helper;

using Xunit;

namespace GsaGHTests.Parameters.Results {
  [Collection("GrasshopperFixture collection")]
  public class NodeResonantFootfallCacheTests {
    [Fact]
    public void SetAxisThrowsExceptionTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);

      // Act
      Assert.Throws<NotImplementedException>(() => result.NodeResonantFootfalls.SetStandardAxis(0));
    }
  }
}
