using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Xunit;

namespace GsaGHTests.Parameters
{
  [Collection("GrasshopperFixture collection")]
  public class GsaCombinationCaseTest
  {
    [Fact]
    public void EmptyConstructorTest()
    {
      // Act
      GsaCombinationCase combinationCase = new GsaCombinationCase();

      // Assert
      Assert.Equal(0, combinationCase.ID);
      Assert.Null(combinationCase.Name);
      Assert.Null(combinationCase.Description);
    }

    [Theory]
    [InlineData(0, "name", "description")]
    [InlineData(100, "name", "description")]
    public void ConstructorTest1(int id, string name, string description)
    {
      // Act
      GsaCombinationCase combinationCase = new GsaCombinationCase(id, name, description);

      // Assert
      Assert.Equal(id, combinationCase.ID);
      Assert.Equal(name, combinationCase.Name);
      Assert.Equal(description, combinationCase.Description);
    }

    [Theory]
    [InlineData("name", "description")]
    public void ConstructorTest2(string name, string description)
    {
      // Act
      GsaCombinationCase combinationCase = new GsaCombinationCase(name, description);

      // Assert
      Assert.Equal(0, combinationCase.ID);
      Assert.Equal(name, combinationCase.Name);
      Assert.Equal(description, combinationCase.Description);
    }

    [Fact]
    public void DuplicateTest()
    {
      // Arrange
      GsaCombinationCase original = new GsaCombinationCase(1, "name", "description");

      // Act
      GsaCombinationCase duplicate = original.Duplicate();

      // Assert
      Duplicates.AreEqual(original, duplicate);

      // make some changes to duplicate
      duplicate.ID = 0;
      duplicate.Name = "";
      duplicate.Description = "";

      Assert.Equal(1, original.ID);
      Assert.Equal("name", original.Name);
      Assert.Equal("description", original.Description);
    }
  }
}
