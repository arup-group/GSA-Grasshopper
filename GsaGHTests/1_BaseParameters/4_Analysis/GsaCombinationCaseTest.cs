using GsaGH.Parameters;

using GsaGHTests.Helpers;

using Xunit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaCombinationCaseTest {

    [Theory]
    [InlineData(0, "name", "description")]
    [InlineData(100, "name", "description")]
    public void ConstructorTest1(int id, string name, string description) {
      var combinationCase = new GsaCombinationCase(id, name, description);

      Assert.Equal(id, combinationCase.Id);
      Assert.Equal(name, combinationCase.Name);
      Assert.Equal(description, combinationCase.Definition);
    }

    [Theory]
    [InlineData("name", "description")]
    public void ConstructorTest2(string name, string description) {
      var combinationCase = new GsaCombinationCase(name, description);

      Assert.Equal(0, combinationCase.Id);
      Assert.Equal(name, combinationCase.Name);
      Assert.Equal(description, combinationCase.Definition);
    }

    [Fact]
    public void DuplicateTest() {
      var original = new GsaCombinationCase(1, "name", "description");

      GsaCombinationCase duplicate = original.Duplicate();

      Duplicates.AreEqual(original, duplicate);

      duplicate.Id = 0;
      duplicate.Name = "";
      duplicate.Definition = "";

      Assert.Equal(1, original.Id);
      Assert.Equal("name", original.Name);
      Assert.Equal("description", original.Definition);
    }

    [Fact]
    public void EmptyConstructorTest() {
      var combinationCase = new GsaCombinationCase();

      Assert.Equal(0, combinationCase.Id);
      Assert.Null(combinationCase.Name);
      Assert.Null(combinationCase.Definition);
    }
  }
}
