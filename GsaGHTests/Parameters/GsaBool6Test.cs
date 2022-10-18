using GsaGH.Parameters;
using Xunit;

namespace GsaGHTests.Parameters
{
  [Collection("GrasshopperFixture collection")]
  public class GsaBool6Test
  {
    [Theory]
    [InlineData(true, true, true, true, true, true)]
    [InlineData(false, false, false, false, false, false)]
    public void ConstructorTest(bool x, bool y, bool z, bool xx, bool yy, bool zz)
    {
      // create new bool6
      GsaBool6 b6 = new GsaBool6(x, y, z, xx, yy, zz);

      Assert.Equal(x, b6.X);
      Assert.Equal(y, b6.Y);
      Assert.Equal(z, b6.Z);
      Assert.Equal(xx, b6.XX);
      Assert.Equal(yy, b6.YY);
      Assert.Equal(zz, b6.ZZ);
    }

    [Theory]
    [InlineData(true, true, true, true, true, true)]
    [InlineData(false, false, false, false, false, false)]
    public void DuplicateTest(bool x, bool y, bool z, bool xx, bool yy, bool zz)
    {
      // create new bool6
      GsaBool6 original = new GsaBool6(x, y, z, xx, yy, zz);

      // duplicate
      GsaBool6 duplicate = original.Duplicate();

      Assert.Equal(x, duplicate.X);
      Assert.Equal(y, duplicate.Y);
      Assert.Equal(z, duplicate.Z);
      Assert.Equal(xx, duplicate.XX);
      Assert.Equal(yy, duplicate.YY);
      Assert.Equal(zz, duplicate.ZZ);

      // make some changes to duplicate
      duplicate.X = false;
      duplicate.Y = true;
      duplicate.Z = false;
      duplicate.XX = true;
      duplicate.YY = false;
      duplicate.ZZ = true;

      Assert.Equal(x, original.X);
      Assert.Equal(y, original.Y);
      Assert.Equal(z, original.Z);
      Assert.Equal(xx, original.XX);
      Assert.Equal(yy, original.YY);
      Assert.Equal(zz, original.ZZ);
    }
  }
}
