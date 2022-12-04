using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Grasshopper.Kernel.Types;
using Xunit;

namespace GsaGHTests.Analysis
{
  [Collection("GrasshopperFixture collection")]
  public class ResultsInfoTest
  {
    [Fact]
    public void TestOutputs()
    {
      var comp = new ResultsInfo();
      comp.CreateAttributes();

      GsaModelGoo modelInput = Model.ModelTests.GsaModelGooMother;

      ComponentTestHelper.SetInput(comp, modelInput);

      GH_String types1 = (GH_String)ComponentTestHelper.GetOutput(comp, 0, 0, 0);
      GH_String types2 = (GH_String)ComponentTestHelper.GetOutput(comp, 0, 0, 1);
      GH_String types3 = (GH_String)ComponentTestHelper.GetOutput(comp, 0, 0, 2);
      Assert.Equal("Analysis", types1.Value);
      Assert.Equal("Analysis", types2.Value);
      Assert.Equal("Combination", types3.Value);

      GH_Integer case1 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 1, 0, 0);
      GH_Integer case2 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 1, 0, 1);
      GH_Integer case3 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 1, 0, 2);
      Assert.Equal(1, case1.Value);
      Assert.Equal(2, case2.Value);
      Assert.Equal(1, case3.Value);

      GH_Integer perm3 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 2, 2, 0);
      Assert.Equal(1, perm3.Value);
    }
  }
}
