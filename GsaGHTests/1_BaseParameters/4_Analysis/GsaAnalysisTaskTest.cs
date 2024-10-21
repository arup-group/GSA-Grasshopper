using GsaGH.Parameters;

using Xunit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaAnalysisTaskTest {
    [Fact]
    public void EmptyConstructorTest() {
      var task = new GsaAnalysisTask();

      Assert.Equal(0, task.Id);
      Assert.Null(task.ApiTask);
      Assert.Empty(task.Cases);
    }
  }
}
