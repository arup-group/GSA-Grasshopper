using GsaGH.Parameters;
using OasysUnits;
using OasysUnits.Units;
using Xunit;

namespace GsaGHTests
{
  public class OffsetTests
  {
    [Fact]
    public void TestCreateOffset()
    {
      // create new offset
      GsaOffset offset = new GsaOffset();
      offset.X1 = new Length(1.57, LengthUnit.Meter);
      offset.X2 = new Length(-2.5, LengthUnit.Meter);
      offset.Y = new Length(4.2, LengthUnit.Meter);
      offset.Z = new Length(-10.5, LengthUnit.Meter);

      Assert.Equal(1.57, offset.X1.Meters);
      Assert.Equal(-2.5, offset.X2.Meters);
      Assert.Equal(4.2, offset.Y.Meters);
      Assert.Equal(-10.5, offset.Z.Meters);
    }

    [Fact]
    public void TestDuplicateOffset()
    {
      // create new offset
      GsaOffset offset = new GsaOffset();
      offset.X1 = new Length(-1.57, LengthUnit.Meter);
      offset.X2 = new Length(2.5, LengthUnit.Meter);
      offset.Y = new Length(-4.2, LengthUnit.Meter);
      offset.Z = new Length(10.5, LengthUnit.Meter);

      // duplicate original
      GsaOffset dup = offset.Duplicate();

      // make some changes to original
      offset.X1 = new Length(-1000, LengthUnit.Meter);
      offset.X2 = new Length(0.0025, LengthUnit.Meter);
      offset.Y = new Length(42, LengthUnit.Meter);
      offset.Z = new Length(0, LengthUnit.Meter);

      Assert.Equal(-1.57, dup.X1.Meters);
      Assert.Equal(2.5, dup.X2.Meters);
      Assert.Equal(-4.2, dup.Y.Meters);
      Assert.Equal(10.5, dup.Z.Meters);

      Assert.Equal(-1000, offset.X1.Meters);
      Assert.Equal(0.0025, offset.X2.Meters);
      Assert.Equal(42, offset.Y.Meters);
      Assert.Equal(0, offset.Z.Meters);
    }
  }
}
