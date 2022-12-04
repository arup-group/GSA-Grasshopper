using GsaAPI;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Xunit;

namespace GsaGHTests.Parameters
{
  [Collection("GrasshopperFixture collection")]
  public class GsaProp3Tests
  {
    [Fact]
    public void DuplicateTest()
    {
      // Arrange
      GsaProp3d original = new GsaProp3d(new GsaMaterial());
      original.Name = "Name";

      // Act
      GsaProp3d duplicate = original.Duplicate();


      // Assert
      Duplicates.AreEqual(original, duplicate);

      // make some changes to duplicate
      // todo
    }
  }
}
