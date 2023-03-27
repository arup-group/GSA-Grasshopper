using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Xunit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaAnalysisCaseTest {
    [Fact]
    public void EmptyConstructorTest() {
      // Act
      var analysisCase = new GsaAnalysisCase();

      // Assert
      Assert.Equal(0, analysisCase.Id);
      Assert.Null(analysisCase.Name);
      Assert.Null(analysisCase.Description);
    }

    [Theory]
    [InlineData(0, "name", "description")]
    [InlineData(100, "name", "description")]
    public void ConstructorTest(int id, string name, string description) {
      // Act
      var analysisCase = new GsaAnalysisCase(id, name, description);

      // Assert
      Assert.Equal(id, analysisCase.Id);
      Assert.Equal(name, analysisCase.Name);
      Assert.Equal(description, analysisCase.Description);
    }

    [Fact]
    public void DuplicateTest() {
      // Arrange
      var original = new GsaAnalysisCase(1, "name", "description");

      // Act
      GsaAnalysisCase duplicate = original.Duplicate();

      // Assert
      Duplicates.AreEqual(original, duplicate);

      // make some changes to duplicate
      duplicate.Id = 0;
      duplicate.Name = "";
      duplicate.Description = "";

      Assert.Equal(1, original.Id);
      Assert.Equal("name", original.Name);
      Assert.Equal("description", original.Description);
    }
  }
}
