using GsaGH.Parameters;

using GsaGHTests.Helpers;

using OasysUnits;
using OasysUnits.Units;

using Xunit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaOffsetTest {

    [Theory]
    [InlineData(1, 2, 3, 4)]
    public void ConstructorTest1(double x1, double x2, double y, double z) {
      var offset = new GsaOffset(x1, x2, y, z);

      Assert.Equal(new Length(x1, LengthUnit.Meter), offset.X1);
      Assert.Equal(new Length(x2, LengthUnit.Meter), offset.X2);
      Assert.Equal(new Length(y, LengthUnit.Meter), offset.Y);
      Assert.Equal(new Length(z, LengthUnit.Meter), offset.Z);
    }

    [Theory]
    [InlineData(1, 2, 3, 4, LengthUnit.Angstrom)]
    public void ConstructorTest2(double x1, double x2, double y, double z, LengthUnit unit) {
      var offset = new GsaOffset(x1, x2, y, z, unit);

      Assert.Equal(new Length(x1, unit), offset.X1);
      Assert.Equal(new Length(x2, unit), offset.X2);
      Assert.Equal(new Length(y, unit), offset.Y);
      Assert.Equal(new Length(z, unit), offset.Z);
    }

    [Theory]
    [InlineData(1, 2, 3, 4, LengthUnit.Angstrom)]
    public void DuplicateTest(double x1, double x2, double y, double z, LengthUnit unit) {
      var original = new GsaOffset(x1, x2, y, z, unit);

      GsaOffset duplicate = original.Duplicate();

      Duplicates.AreEqual(original, duplicate);

      duplicate.X1 = new Length(4, LengthUnit.Millimeter);
      duplicate.X2 = new Length(3, LengthUnit.Millimeter);
      duplicate.Y = new Length(2, LengthUnit.Millimeter);
      duplicate.Z = new Length(1, LengthUnit.Millimeter);

      Assert.Equal(new Length(x1, unit), original.X1);
      Assert.Equal(new Length(x2, unit), original.X2);
      Assert.Equal(new Length(y, unit), original.Y);
      Assert.Equal(new Length(z, unit), original.Z);

      Assert.Equal(new Length(4, LengthUnit.Millimeter), duplicate.X1);
      Assert.Equal(new Length(3, LengthUnit.Millimeter), duplicate.X2);
      Assert.Equal(new Length(2, LengthUnit.Millimeter), duplicate.Y);
      Assert.Equal(new Length(1, LengthUnit.Millimeter), duplicate.Z);
    }

    [Fact]
    public void EmptyConstructorTest() {
      var offset = new GsaOffset();

      Assert.Equal(Length.Zero, offset.X1);
      Assert.Equal(Length.Zero, offset.X2);
      Assert.Equal(Length.Zero, offset.Y);
      Assert.Equal(Length.Zero, offset.Z);
    }
  }
}
