using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Xunit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaProp3Tests {

    [Fact]
    public void DuplicateTest() {
      var original = new GsaProperty3d(new GsaMaterial()) {
        Name = "Name",
      };

      GsaProperty3d duplicate = original.Duplicate();

      Duplicates.AreEqual(original, duplicate);
    }
  }
}
