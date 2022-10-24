using GsaGH.Parameters;
using Xunit;

namespace GsaGHTests.Parameters
{
  [Collection("GrasshopperFixture collection")]
  public class GsaBool6GooTest
  {
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CastFromBoolTest(bool b)
    {
      // Arrange
      GsaBool6Goo goo = new GsaBool6Goo(new GsaBool6());

      // Act
      goo.CastFrom(b);

      // Assert
      Assert.Equal(b, goo.Value.X);
      Assert.Equal(b, goo.Value.Y);
      Assert.Equal(b, goo.Value.Z);
      Assert.Equal(b, goo.Value.XX);
      Assert.Equal(b, goo.Value.YY);
      Assert.Equal(b, goo.Value.ZZ);
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
    [InlineData("ffffff", false, false, false, false, false, false)]
    [InlineData("rrrrrr", true, true, true, true, true, true)]
    public void CastFromStringtest(string s, bool expectedX, bool expectedY, bool expectedZ, bool expectedXX, bool expectedYY, bool expectedZZ)
    {
      // Arrange
      GsaBool6Goo goo = new GsaBool6Goo(new GsaBool6());

      // Act
      goo.CastFrom(s);

      // Assert
      Assert.Equal(expectedX, goo.Value.X);
      Assert.Equal(expectedY, goo.Value.Y);
      Assert.Equal(expectedZ, goo.Value.Z);
      Assert.Equal(expectedXX, goo.Value.XX);
      Assert.Equal(expectedYY, goo.Value.YY);
      Assert.Equal(expectedZZ, goo.Value.ZZ);
    }
  }
}
