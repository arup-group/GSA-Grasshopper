using System;

using GsaGH.Parameters;

using Xunit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaLoadCaseTest {
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void ConstructorTest(int caseId) {
      var lc = new GsaLoadCase(caseId);

      Assert.Equal(caseId, lc.Id);
      Assert.Null(lc.LoadCase);
    }

    [Fact]
    public void ConstructorThrowsExpectionTest() {
      Assert.Throws<ArgumentException>(() => new GsaLoadCase(0));
    }

    [Theory]
    [InlineData(1, LoadCaseType.Dead, "DeadLoad")]
    [InlineData(2, LoadCaseType.Live, "Live Load")]
    [InlineData(3, LoadCaseType.Wind, "Wind in X-direction")]
    public void Constructor2Test(int caseId, LoadCaseType type, string name) {
      var lc = new GsaLoadCase(caseId, type, name);

      Assert.Equal(caseId, lc.Id);
      Assert.NotNull(lc.LoadCase);
      Assert.Equal(type.ToString(), lc.LoadCase.CaseType.ToString());
      Assert.Equal(name, lc.LoadCase.Name);
    }
  }
}
