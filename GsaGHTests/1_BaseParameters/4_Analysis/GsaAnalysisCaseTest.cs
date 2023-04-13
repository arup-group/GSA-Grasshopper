﻿using GsaGH.Parameters;
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
      Assert.Equal(name, analysisCase.Name);
      Assert.Equal(description, analysisCase.Description);
    }

    [Fact]
    public void DuplicateTest() {
      var original = new GsaAnalysisCase(1, "name", "description");

      GsaAnalysisCase duplicate = original.Duplicate();

      Duplicates.AreEqual(original, duplicate);

      duplicate.Id = 0;
      duplicate.Name = "";
      duplicate.Description = "";

      Assert.Equal(1, original.Id);
      Assert.Equal("name", original.Name);
      Assert.Equal("description", original.Description);
    }

    [Fact]
    public void EmptyConstructorTest() {
      var analysisCase = new GsaAnalysisCase();

      Assert.Equal(0, analysisCase.Id);
      Assert.Null(analysisCase.Name);
      Assert.Null(analysisCase.Description);
    }
  }
}
