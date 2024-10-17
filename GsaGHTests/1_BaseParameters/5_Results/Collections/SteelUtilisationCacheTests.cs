using System;

using GsaGH.Parameters.Results;

using GsaGHTests.Helper;

using Xunit;

namespace GsaGHTests.Parameters.Results {
  [Collection("GrasshopperFixture collection")]
  public class SteelUtilisationsCacheTests {
    [Fact]
    public void SetAxisThrowsExceptionTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);

      // Act
      Assert.Throws<NotImplementedException>(() => result.SteelUtilisations.SetStandardAxis(0));
    }
  }
}
