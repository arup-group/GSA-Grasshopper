using GsaGH.Parameters;

using GsaGHTests.Helpers;

using Xunit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaAnalysisCaseTest {

    [Theory]
    [InlineData(0, "name", "description")]
    [InlineData(100, "name", "description")]
    public void ConstructorTest(int id, string name, string description) {
      var analysisCase = new GsaAnalysisCase(id, name, description);

      Assert.Equal(id, analysisCase.Id);
      Assert.Equal(name, analysisCase.ApiCase.Name);
      Assert.Equal(description, analysisCase.ApiCase.Description);
    }

    [Fact]
    public void DuplicateTest() {
      var original = new GsaAnalysisCase(1, "name", "description");

      GsaAnalysisCase duplicate = original.Duplicate();

      Duplicates.AreEqual(original, duplicate);

      Assert.Equal(1, original.Id);
      Assert.Equal("name", original.ApiCase.Name);
      Assert.Equal("description", original.ApiCase.Description);
    }

    [Fact]
    public void EmptyConstructorTest() {
      var analysisCase = new GsaAnalysisCase();
      Assert.Equal(0, analysisCase.Id);
      Assert.NotNull(analysisCase.ApiCase);
    }
  }
}
