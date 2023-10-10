using System.Collections.Generic;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Xunit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaAnalysisTaskTest {

    [Fact]
    public void DuplicateTest() {
      var original = new GsaAnalysisTask();

      GsaAnalysisTask duplicate = original.Duplicate();

      Duplicates.AreEqual(original, duplicate);

      duplicate.Id = 1;
      duplicate.Name = "name";
      duplicate.Type = AnalysisTaskType.Buckling;
      duplicate.Cases = new List<GsaAnalysisCase>();

      Assert.Equal(0, original.Id);
      Assert.Null(original.Name);
      Assert.Equal(AnalysisTaskType.Static, original.Type);
      Assert.Empty(original.Cases);
    }

    [Fact]
    public void EmptyConstructorTest() {
      var task = new GsaAnalysisTask();

      Assert.Equal(0, task.Id);
      Assert.Null(task.Name);
      Assert.Equal(AnalysisTaskType.Static, task.Type);
      Assert.Empty(task.Cases);
    }
  }
}
