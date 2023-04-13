using GsaGH.Parameters;
using Xunit;

namespace GsaGHTests.GooWrappers {
  [Collection("GrasshopperFixture collection")]
  public class GsaBool6GooTest {

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CastFromBoolTest(bool b) {
      var goo = new GsaBool6Goo(new GsaBool6());

      goo.CastFrom(b);

      Assert.Equal(b, goo.Value.X);
      Assert.Equal(b, goo.Value.Y);
      Assert.Equal(b, goo.Value.Z);
      Assert.Equal(b, goo.Value.Xx);
      Assert.Equal(b, goo.Value.Yy);
      Assert.Equal(b, goo.Value.Zz);
    }

    [Theory]
    [InlineData("free", false, false, false, false, false, false)]
    [InlineData("pin", true, true, true, false, false, false)]
    [InlineData("pinned", true, true, true, false, false, false)]
    [InlineData("fix", true, true, true, true, true, true)]
    [InlineData("fixed", true, true, true, true, true, true)]
    [InlineData("release", false, false, false, false, true, true)]
    [InlineData("released", false, false, false, false, true, true)]
    [InlineData("hinge", false, false, false, false, true, true)]
    [InlineData("hinged", false, false, false, false, true, true)]
    [InlineData("charnier", false, false, false, false, true, true)]
    [InlineData("rrrrrr", false, false, false, false, false, false)]
    [InlineData("ffffff", true, true, true, true, true, true)]
    public void CastFromStringtest(
      string s, bool expectedX, bool expectedY, bool expectedZ, bool expectedXx, bool expectedYy,
      bool expectedZz) {
      var goo = new GsaBool6Goo(new GsaBool6());

      goo.CastFrom(s);

      Assert.Equal(expectedX, goo.Value.X);
      Assert.Equal(expectedY, goo.Value.Y);
      Assert.Equal(expectedZ, goo.Value.Z);
      Assert.Equal(expectedXx, goo.Value.Xx);
      Assert.Equal(expectedYy, goo.Value.Yy);
      Assert.Equal(expectedZz, goo.Value.Zz);
    }
  }
}
