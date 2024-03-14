using System.Collections.Generic;
using GsaAPI;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Xunit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaAnalysisTaskTest {

    [Fact]
    public void DuplicateStaticTest() {
      var original = new GsaAnalysisTask();

      GsaAnalysisTask duplicate = original.Duplicate();

      Duplicates.AreEqual(original, duplicate);

      duplicate.Id = 1;
      duplicate.Task.Name = "name";
      duplicate.Task.Type = (int)AnalysisTaskType.Buckling;
      duplicate.Cases = new List<GsaAnalysisCase>();

      Assert.Equal(0, original.Id);
      Assert.Null(original.Task.Name);
      Assert.Equal((int)AnalysisTaskType.Static, original.Task.Type);
      Assert.Empty(original.Cases);
    }

    [Fact]
    public void DuplicateStaticPDeltaWithGeometricStiffnessFromLoadCaseTest() {
      var original = new GsaAnalysisTask {
        Id = 1,
        Task = AnalysisTaskFactory.CreateStaticPDeltaAnalysisTask("task1", new GeometricStiffnessFromLoadCase("L1"))
      };

      GsaAnalysisTask duplicate = original.Duplicate();

      Duplicates.AreEqual(original, duplicate);

      duplicate.Id = 2;
      duplicate.Task = AnalysisTaskFactory.CreateStaticPDeltaAnalysisTask("task2", new GeometricStiffnessFromResultCase(1));

      Assert.Equal(1, original.Id);
      Assert.Equal("task1", original.Task.Name);
      Assert.Equal((int)AnalysisTaskType.StaticPDelta, original.Task.Type);
      Assert.Empty(original.Cases);
    }

    [Fact]
    public void DuplicateStaticPDeltaWithGeometricStiffnessFromOwnLoadTest() {
      var original = new GsaAnalysisTask {
        Id = 1,
        Task = AnalysisTaskFactory.CreateStaticPDeltaAnalysisTask("task1", new GeometricStiffnessFromOwnLoad())
      };

      GsaAnalysisTask duplicate = original.Duplicate();

      Duplicates.AreEqual(original, duplicate);

      duplicate.Id = 2;
      duplicate.Task = AnalysisTaskFactory.CreateStaticPDeltaAnalysisTask("task2", new GeometricStiffnessFromResultCase(1));

      Assert.Equal(1, original.Id);
      Assert.Equal("task1", original.Task.Name);
      Assert.Equal((int)AnalysisTaskType.StaticPDelta, original.Task.Type);
      Assert.Empty(original.Cases);
    }

    [Fact]
    public void DuplicateStaticPDeltaWithGeometricStiffnessFromResultCaseTest() {
      var original = new GsaAnalysisTask {
        Id = 1,
        Task = AnalysisTaskFactory.CreateStaticPDeltaAnalysisTask("task1", new GeometricStiffnessFromResultCase(1))
      };

      GsaAnalysisTask duplicate = original.Duplicate();

      Duplicates.AreEqual(original, duplicate);

      duplicate.Id = 2;
      duplicate.Task = AnalysisTaskFactory.CreateStaticPDeltaAnalysisTask("task2", new GeometricStiffnessFromOwnLoad());

      Assert.Equal(1, original.Id);
      Assert.Equal("task1", original.Task.Name);
      Assert.Equal((int)AnalysisTaskType.StaticPDelta, original.Task.Type);
      Assert.Empty(original.Cases);
    }

    [Fact]
    public void EmptyConstructorTest() {
      var task = new GsaAnalysisTask();

      Assert.Equal(0, task.Id);
      Assert.Null(task.Task.Name);
      Assert.Equal((int)AnalysisTaskType.Static, task.Task.Type);
      Assert.Empty(task.Cases);
    }
  }
}
