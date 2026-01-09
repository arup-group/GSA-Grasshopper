using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Helpers;
using GsaGHTests.Model;

using Xunit;

namespace GsaGHTests.Components.Results {
  [Collection("GrasshopperFixture collection")]
  public class ResultsInfoTest {

    [Fact]
    public void TestOutputs() {
      var comp = new GetResultCases();
      comp.CreateAttributes();

      GsaModelGoo modelInput = ModelTests.GsaModelGooMother;

      ComponentTestHelper.SetInput(comp, modelInput);

      var types1 = (GH_String)ComponentTestHelper.GetOutput(comp, 0, 0, 0);
      var types2 = (GH_String)ComponentTestHelper.GetOutput(comp, 0, 0, 1);
      var types3 = (GH_String)ComponentTestHelper.GetOutput(comp, 0, 0, 2);
      Assert.Equal("Analysis", types1.Value);
      Assert.Equal("Analysis", types2.Value);
      Assert.Equal("Combination", types3.Value);

      var case1 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 1, 0, 0);
      var case2 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 1, 0, 1);
      var case3 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 1, 0, 2);
      Assert.Equal(1, case1.Value);
      Assert.Equal(2, case2.Value);
      Assert.Equal(1, case3.Value);

      var path = new GH_Path(1);
      var perm3 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 2, path, 0);
      Assert.Equal(1, perm3.Value);
    }
  }
}
