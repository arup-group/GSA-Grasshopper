using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Rhino.Geometry;
using Xunit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaElement3dTest {

    [Fact]
    public void DuplicateTest() {
      var original = new GsaElement3d(new Mesh());

      GsaElement3d duplicate = original.Duplicate();

      Duplicates.AreEqual(original, duplicate);
    }
  }
}
