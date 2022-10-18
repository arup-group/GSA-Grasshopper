using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Rhino.Geometry;
using Xunit;

namespace GsaGHTests.Parameters
{
  [Collection("GrasshopperFixture collection")]
  public class GsaGridPlaneSurfaceTest
  {
    [Fact]
    public void DuplicateTest()
    {
      // Arrange
      GsaGridPlaneSurface original = new GsaGridPlaneSurface();

      // Act
      GsaGridPlaneSurface duplicate = original.Duplicate();

      // Assert
      Duplicates.AreEqual(original, duplicate);

      // make some changes to duplicate
      // todo
    }
  }
}