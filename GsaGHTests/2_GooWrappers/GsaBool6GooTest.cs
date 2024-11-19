using System;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaGH.Helpers;
using GsaGH.Parameters;

using GsaGHTests.Helpers;

using Rhino.Geometry;

using Xunit;

namespace GsaGHTests.GooWrappers {
  [Collection("GrasshopperFixture collection")]
  public class GsaBool6GooTest {
    private const string NonExistingString = "NonExistingString";

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CastFromBoolShouldSetAllToTheSameBool(bool value) {
      var param = new GsaBool6Parameter();
      param.CreateAttributes();
      ComponentTestHelper.SetInput(param, new GH_Boolean(value));
      var output = (GsaBool6Goo)ComponentTestHelper.GetOutput(param);

      Assert.True(output.Value.Equals(value));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CastFromBoolShouldSetAllToTheSameBoolForReleases(bool value) {
      var param = new GsaReleaseParameter();
      param.CreateAttributes();
      ComponentTestHelper.SetInput(param, new GH_Boolean(value));
      var output = (GsaBool6Goo)ComponentTestHelper.GetOutput(param);

      Assert.True(output.Value.Equals(value));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CastFromBoolShouldSetAllToTheSameBoolForRestrains(bool value) {
      var param = new GsaRestraintParameter();
      param.CreateAttributes();
      ComponentTestHelper.SetInput(param, new GH_Boolean(value));
      var output = (GsaBool6Goo)ComponentTestHelper.GetOutput(param);

      Assert.True(output.Value.Equals(value));
    }

    [Fact]
    public void NotTheSameTypeThereforeNotEqual() {
      Assert.False(new GsaBool6().Equals(new Point3d()));
    }

    [Fact]
    public void NullIsNotEqual() {
      Assert.False(new GsaBool6().Equals(null));
    }

    [Fact]
    public void ShouldCheckAllComponentWhenComparingWithBoolFalse() {
      Assert.False(new GsaBool6(true, true, true, true, true, false).Equals(true));
    }

    [Fact]
    public void ShouldCheckAllComponentWhenComparingWithBoolTrue() {
      Assert.True(new GsaBool6(true, true, true, true, true, true).Equals(true));
    }

    [Fact]
    public void ShouldHaveTheSameHashCode() {
      Assert.Equal(new GsaBool6(true, true, false, false, true, true).GetHashCode(),
        new GsaBool6(true, true, false, false, true, true).GetHashCode());
    }

    [Theory]
    [InlineData("free", true, true, true, true, true, true)]
    [InlineData("pin", false, false, false, true, true, true)]
    [InlineData("pinned", false, false, false, true, true, true)]
    [InlineData("fix", false, false, false, false, false, false)]
    [InlineData("fixed", false, false, false, false, false, false)]
    [InlineData("release", true, true, true, true, false, false)]
    [InlineData("released", true, true, true, true, false, false)]
    [InlineData("hinge", true, true, true, true, false, false)]
    [InlineData("hinged", true, true, true, true, false, false)]
    [InlineData("charnier", true, true, true, true, false, false)]
    [InlineData("rrrrrr", true, true, true, true, true, true)]
    [InlineData("ffffff", false, false, false, false, false, false)]
    [InlineData("ffffrr", false, false, false, false, true, true)]
    public void CastFromStringTest(
      string text, bool expectedX, bool expectedY, bool expectedZ, bool expectedXx, bool expectedYy, bool expectedZz) {
      //release
      var output = StringExtension.ParseRelease(text);
      var releaseBool6 = new GsaBool6 {
        X = expectedX,
        Y = expectedY,
        Z = expectedZ,
        Xx = expectedXx,
        Yy = expectedYy,
        Zz = expectedZz
      };
      Assert.Equal(releaseBool6, output);

      //restraint is the opposite of release
      output = StringExtension.ParseRestrain(text);
      Assert.Equal(!releaseBool6, output);
    }

    [Fact]
    public void ReleasesShouldThrowErrorWhenInvalidStringIsUsed() {
      Assert.Throws<InvalidCastException>(() => StringExtension.ParseRelease(NonExistingString));
    }

    [Fact]
    public void RestrainsShouldThrowErrorWhenInvalidStringIsUsed() {
      Assert.Throws<InvalidCastException>(() => StringExtension.ParseRestrain(NonExistingString));
    }

    [Fact]
    public void Bool6ShouldThrowErrorWhenInvalidStringIsUsed() {
      Assert.Throws<InvalidCastException>(() => StringExtension.ParseBool6(NonExistingString));
    }

    [Fact]
    public void Bool6ShouldThrowExceptionWhen6Characters() {
      string exactlySixCharacters = "ABCDEF";
      Assert.Throws<InvalidCastException>(() => StringExtension.ParseBool6(exactlySixCharacters));
    }

    [Fact]
    public void ParseReleaseShouldThrowExceptionWhenNullObjectIsUsed() {
      Assert.Throws<InvalidCastException>(() => StringExtension.ParseRelease(null));
    }

    private class DummyClass { }
  }
}
