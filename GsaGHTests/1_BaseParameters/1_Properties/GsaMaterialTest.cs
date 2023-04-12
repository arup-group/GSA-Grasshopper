using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Xunit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaMaterialTest {

    [Fact]
    public void DuplicateTest() {
      var original = new GsaMaterial {
        MaterialType = GsaMaterial.MatType.Aluminium,
      };

      GsaMaterial duplicate = original.Duplicate();
      Duplicates.AreEqual(original, duplicate);
    }
  }
}
