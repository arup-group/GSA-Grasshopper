using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Xunit;

namespace GsaGHTests.Parameters
{
  [Collection("GrasshopperFixture collection")]
  public class GsaAnalysisCaseTest
  {
    [Fact]
    public void EmptyConstructorTest()
    {
      // Act
      GsaAnalysisCase analysisCase = new GsaAnalysisCase();

      // Assert
      Assert.Equal(0, analysisCase.ID);
      Assert.Null(analysisCase.Name);
      Assert.Null(analysisCase.Description);
    }

    [Theory]
    [InlineData(0, "name", "description")]
    [InlineData(100, "name", "description")]
    public void ConstructorTest(int id, string name, string description)
    {
      // Act
      GsaAnalysisCase analysisCase = new GsaAnalysisCase(id, name, description);

      // Assert
      Assert.Equal(id, analysisCase.ID);
      Assert.Equal(name, analysisCase.Name);
      Assert.Equal(description, analysisCase.Description);
    }

    [Fact]
    public void DuplicateTest()
    {
      // Arrange
      GsaAnalysisCase original = new GsaAnalysisCase(1, "name", "description");

      // Act
      GsaAnalysisCase duplicate = original.Duplicate();

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
