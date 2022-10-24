using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Rhino.Geometry;
using Xunit;

namespace GsaGHTests.Parameters
{
  [Collection("GrasshopperFixture collection")]
  public class GsaElement3dTest
  {
    [Fact]
    public void DuplicateTest()
    {
      // Arrange
      GsaElement3d original = new GsaElement3d(new Mesh());

      // Act
      GsaElement3d duplicate = original.Duplicate();

      // Assert
      Duplicates.AreEqual(original, duplicate);

      // make some changes to duplicate
      // todo
    }
  }
}