using Grasshopper.Kernel.Types;

using GsaGH.Helpers;
using GsaGH.Parameters;

using GsaGHTests.Helpers;

using Xunit;

namespace GsaGHTests.GooWrappers {
  [Collection("GrasshopperFixture collection")]
  public class GsaBool6GooTest {

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CastFromBoolTest(bool b) {
      var param = new GsaBool6Parameter();
      param.CreateAttributes();
      ComponentTestHelper.SetInput(param, new GH_Boolean(b));
      var output = (GsaBool6Goo)ComponentTestHelper.GetOutput(param);

      Assert.Equal(b, output.Value.X);
      Assert.Equal(b, output.Value.Y);
      Assert.Equal(b, output.Value.Z);
      Assert.Equal(b, output.Value.Xx);
      Assert.Equal(b, output.Value.Yy);
      Assert.Equal(b, output.Value.Zz);
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
      //restraint
      GsaBool6 output = StringExtension.ParseBool6(s);
      Assert.Equal(expectedX, output.X);
      Assert.Equal(expectedY, output.Y);
      Assert.Equal(expectedZ, output.Z);
      Assert.Equal(expectedXx, output.Xx);
      Assert.Equal(expectedYy, output.Yy);
      Assert.Equal(expectedZz, output.Zz);

      //releases
      output = StringExtension.ParseBool6(s,true);
      Assert.Equal(!expectedX, output.X);
      Assert.Equal(!expectedY, output.Y);
      Assert.Equal(!expectedZ, output.Z);
      Assert.Equal(!expectedXx, output.Xx);
      Assert.Equal(!expectedYy, output.Yy);
      Assert.Equal(!expectedZz, output.Zz);
    }
  }
}
