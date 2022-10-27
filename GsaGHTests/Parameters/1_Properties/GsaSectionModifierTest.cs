using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Xunit;

namespace GsaGHTests.Parameters
{
  [Collection("GrasshopperFixture collection")]
  public class GsaSectionModifierTest
  {
    [Fact]
    public void DuplicateTest()
    {
      // Arrange
      GsaSectionModifier original = new GsaSectionModifier();
      original.StressOption = GsaSectionModifier.StressOptionType.NoCalculation;

      // Act
      GsaSectionModifier duplicate = original.Duplicate();

      // Assert
      Duplicates.AreEqual(original, duplicate);

      // make some changes to duplicate
      // todo
    }
  }
}
