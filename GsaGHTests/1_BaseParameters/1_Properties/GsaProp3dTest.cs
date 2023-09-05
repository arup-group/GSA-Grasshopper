using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Xunit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaProp3Tests {

    [Fact]
    public void DuplicateTest() {
      var original = new GsaProperty3d(new GsaMaterial()) {
      };

      var duplicate = new GsaProperty3d(original);

      Duplicates.AreEqual(original, duplicate, true);
    }
  }
}
