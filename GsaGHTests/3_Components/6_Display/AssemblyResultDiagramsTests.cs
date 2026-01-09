using GsaGH.Components;
using GsaGH.Parameters;
using GsaGH.Parameters.Results;

using GsaGHTests.Helper;
using GsaGHTests.Helpers;
using GsaGHTests.Parameters;

using Xunit;

namespace GsaGHTests.Components.Display {
  [Collection("GrasshopperFixture collection")]
  public class AssemblyResultDiagramsTests {
    [Fact]
    public void UpdateForceTest() {
      var comp = new AssemblyResultDiagrams();
      GsaResult result = GsaResultTests.AnalysisCaseResult(GsaFile.AssemblyByStorey, 1);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      comp.SetSelected(0, 3); // Force
      comp.SetSelected(1, 0); // Axial force
      comp.UpdateForce("MN");
      comp.Params.Output[0].CollectData();
      Assert.Equal("MN", comp.Message);
    }

    [Fact]
    public void UpdateLengthTest() {
      var comp = new AssemblyResultDiagrams();
      GsaResult result = GsaResultTests.AnalysisCaseResult(GsaFile.AssemblyByStorey, 1);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      comp.SetSelected(0, 0); // Displacement
      comp.UpdateLength("m");
      comp.Params.Output[0].CollectData();
      Assert.Equal("m", comp.Message);
    }

    [Fact]
    public void UpdateMomentTest() {
      var comp = new AssemblyResultDiagrams();
      GsaResult result = GsaResultTests.AnalysisCaseResult(GsaFile.AssemblyByStorey, 1);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      comp.SetSelected(0, 3); // Force
      comp.SetSelected(1, 4); // Moment Myy
      comp.UpdateMoment("MN·m");
      comp.Params.Output[0].CollectData();
      Assert.Equal("MN·m", comp.Message);
    }
  }
}
