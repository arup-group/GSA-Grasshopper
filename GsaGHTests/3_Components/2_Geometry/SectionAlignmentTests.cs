using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysGH.Components;
using OasysUnits.Units;
using Rhino.Geometry;
using Xunit;

namespace GsaGHTests.Components.Geometry
{
  [Collection("GrasshopperFixture collection")]
  public class SectionAlignmentTests
  {
    public static GH_OasysComponent ComponentMother()
    {
      var comp = new SectionAlignment();
      comp.CreateAttributes();

      GsaMember1d member = new GsaMember1d(new LineCurve(new Point3d(0, 0, 0), new Point3d(0, 0, 10)), 0);
      member.Section = new GsaSection("CAT HE HE300.B");
      GsaMember1dGoo goo = new GsaMember1dGoo(member);

      ComponentTestHelper.SetInput(comp, goo, 0);

      return comp;
    }

    [Theory]
    [InlineData("Top-Left", -150, -150)]
    [InlineData("Top-Centre", 0, -150)]
    [InlineData("Top-Center", 0, -150)]
    [InlineData("Top", 0, -150)]
    [InlineData("Top-Right", 150, -150)]
    [InlineData("Mid-Left", -150, 0)]
    [InlineData("Left", -150, 0)]
    [InlineData("Centroid", 0, 0)]
    [InlineData("Mid-Right", 150, 0)]
    [InlineData("Right", 150, 0)]
    [InlineData("Bottom-Left", -150, 150)]
    [InlineData("Bottom-Centre", 0, 150)]
    [InlineData("Bottom-Center", 0, 150)]
    [InlineData("Bottom", 0, 150)]
    [InlineData("Bottom-Right", 150, 150)]
    public void AlignmentTest(string alignment, double expectedY, double expectedZ)
    {
      // Arrange & Act
      var comp = ComponentMother();

      ComponentTestHelper.SetInput(comp, alignment, 1);

      // Assert
      GsaOffsetGoo output = (GsaOffsetGoo)ComponentTestHelper.GetOutput(comp, 1);

      Assert.Equal(expectedY, output.Value.Y.Millimeters, 6);
      Assert.Equal(expectedZ, output.Value.Z.Millimeters, 6);
    }

    [Theory]
    [InlineData("Top-Left", 10, 10, -150, -150)]
    [InlineData("Top-Centre", -10, -10, 0, -150)]
    [InlineData("Top-Center", 2, 2, 0, -150)]
    [InlineData("Top", 3, 3, 0, -150)]
    [InlineData("Top-Right", 4, 4, 150, -150)]
    [InlineData("Mid-Left", 5, 5, -150, 0)]
    [InlineData("Left", 6, 6, -150, 0)]
    [InlineData("Centroid", 7, 7, 0, 0)]
    [InlineData("Mid-Right", 8, 8, 150, 0)]
    [InlineData("Right", 9, 9, 150, 0)]
    [InlineData("Bottom-Left", 10, 10, -150, 150)]
    [InlineData("Bottom-Centre", 11, 11, 0, 150)]
    [InlineData("Bottom-Center", 12, 12, 0, 150)]
    [InlineData("Bottom", 13, 13, 0, 150)]
    [InlineData("Bottom-Right", -14, -14, 150, 150)]
    public void AlignmentOffsetTest(string alignment, double y, double z, double expectedY, double expectedZ)
    {
      // Arrange & Act
      var comp = ComponentMother();

      ComponentTestHelper.SetInput(comp, alignment, 1);
      ComponentTestHelper.SetInput(comp, new GsaOffsetGoo(new GsaOffset(0, 0, y, z, LengthUnit.Millimeter)), 2);

      // Assert
      GsaOffsetGoo output = (GsaOffsetGoo)ComponentTestHelper.GetOutput(comp, 1);

      Assert.Equal(expectedY + y, output.Value.Y.Millimeters, 6);
      Assert.Equal(expectedZ + z, output.Value.Z.Millimeters, 6);
    }
  }
}
