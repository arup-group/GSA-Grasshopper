using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Xunit;

namespace GsaGHTests.Parameters
{
  [Collection("GrasshopperFixture collection")]
  public class GsaMaterialTest
  {
    [Fact]
    public void DuplicateTest()
    {
      // Arrange
      GsaMaterial original = new GsaMaterial();
      original.MaterialType = GsaMaterial.MatType.ALUMINIUM;

      // Act
      GsaMaterial duplicate = original.Duplicate();

      Duplicates.AreEqual(original, duplicate);

      // make some changes to duplicate
      // todo
    }
  }
}