﻿using Grasshopper.Kernel;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysUnits;
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
      // Act
      GsaBool6 b6 = new GsaBool6(x, y, z, xx, yy, zz);

      // Assert
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
      // Arrange
      GsaBool6 original = new GsaBool6(x, y, z, xx, yy, zz);

      // Act
      GsaBool6 duplicate = original.Duplicate();

      // Assert
      Duplicates.AreEqual(original, duplicate);

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
