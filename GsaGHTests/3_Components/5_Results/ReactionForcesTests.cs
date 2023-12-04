using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysGH.Parameters;
using Xunit;

namespace GsaGHTests.Components.Results {
  [Collection("GrasshopperFixture collection")]
  public class ReactionForcesTests {
    [Fact]
    public void TestAnalysisNoInputs() {
      var comp = new ReactionForces();
      var result = (GsaResultGoo)ComponentTestHelper.GetOutput(
        GetResultsTest.ResultsComponentMother());
      ComponentTestHelper.SetInput(comp, result);
      var output = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, 2);
      Assert.Equal(37.50, output.Value.As(OasysUnits.Units.ForceUnit.Kilonewton));
    }
  }
}
