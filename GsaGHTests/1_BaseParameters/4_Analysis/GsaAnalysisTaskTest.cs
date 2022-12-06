using System.Collections.Generic;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Xunit;
using static GsaGH.Parameters.GsaAnalysisTask;

namespace GsaGHTests.Parameters
{
  [Collection("GrasshopperFixture collection")]
  public class GsaAnalysisTaskTest
  {
    [Fact]
    public void EmptyConstructorTest()
    {
      // Act
      GsaAnalysisTask task = new GsaAnalysisTask();

      // Assert
      Assert.Equal(0, task.ID);
      Assert.Null(task.Name);
      Assert.Equal(AnalysisType.Static, task.Type);
      Assert.Empty(task.Cases);
    }

    [Fact]
    public void DuplicateTest()
    {
      // Arrange
      GsaAnalysisTask original = new GsaAnalysisTask();

      // Act
      GsaAnalysisTask duplicate = original.Duplicate();

      // Assert
      Duplicates.AreEqual(original, duplicate);

      // make some changes to duplicate
      duplicate.ID = 1;
      duplicate.Name = "name";
      duplicate.Type = AnalysisType.Buckling;
      duplicate.Cases = new List<GsaAnalysisCase>();

      Assert.Equal(0, original.ID);
      Assert.Null(original.Name);
      Assert.Equal(AnalysisType.Static, original.Type);
      Assert.Empty(original.Cases);
    }
  }
}
