using GsaAPI;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Xunit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaMaterialTest {

    [Fact]
    public void DuplicateTest1() {
      var original = new GsaMaterial {
        MaterialType = GsaMaterial.MatType.Aluminium,
      };

      GsaMaterial duplicate = original.Duplicate();
      Duplicates.AreEqual(original, duplicate);
    }

    [Fact]
    public void DuplicateTest2() {
      var analysisMaterial = new AnalysisMaterial() {
        CoefficientOfThermalExpansion = 1,
        Density = 2,
        ElasticModulus = 3,
        PoissonsRatio = 4,
        Name = "name"
      };
      var original = new GsaMaterial {
        AnalysisProperty = 7,
        MaterialType = GsaMaterial.MatType.Generic,
        AnalysisMaterial = analysisMaterial,
      };

      GsaMaterial duplicate = original.Duplicate();
      Duplicates.AreEqual(original, duplicate);
    }
  }
}
