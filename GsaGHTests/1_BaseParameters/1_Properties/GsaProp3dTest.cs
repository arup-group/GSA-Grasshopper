using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Xunit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaProp3Tests {
    [Fact]
    public void DuplicateTest() {
      var original = new GsaProp3d(new GsaMaterial()) {
        Name = "Name",
      };

      GsaProp3d duplicate = original.Duplicate();

      Duplicates.AreEqual(original, duplicate);
    }
  }
}
