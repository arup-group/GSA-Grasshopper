using System;

using GsaGH.Parameters.Results;

using GsaGHTests.Helper;

using Xunit;

namespace GsaGHTests.Parameters.Results {
  [Collection("GrasshopperFixture collection")]
  public class Element1dAverageStrainEnergyDensityCacheTests {
    [Fact]
    public void SetAxisThrowsExceptionTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);

      // Act
      Assert.Throws<NotImplementedException>(() => result.Element1dAverageStrainEnergyDensities.SetStandardAxis(0));
    }
  }
}
