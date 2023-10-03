using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using GsaGHTests.Model;
using OasysGH.Components;
using Xunit;

namespace GsaGHTests.Results {
  [Collection("GrasshopperFixture collection")]
  public class SelectResultsTests {
    public static GH_OasysDropDownComponent ResultsComponentMother() {
      var comp = new SelectResult();
      comp.CreateAttributes();

      Assert.Equal("AnalysisCase", comp._selectedItems[0]);
      Assert.Equal("   ", comp._selectedItems[1]);

      GsaModelGoo modelInput = ModelTests.GsaModelGooMother;
      ComponentTestHelper.SetInput(comp, modelInput, 0);

      return comp;
    }

    [Fact]
    public void DefaultDropSelectionsTest() {
      GH_OasysDropDownComponent comp = ResultsComponentMother();
      var result = (GsaResultGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal("A1", comp._selectedItems[1]);
      Assert.Equal(2, comp._selectedItems.Count);
      result = (GsaResultGoo)ComponentTestHelper.GetOutput(comp);

      comp.SetSelected(0, 1);
      Assert.Equal("C1", comp._selectedItems[1]);
      Assert.Equal("All", comp._selectedItems[2]);
      Assert.Equal(3, comp._selectedItems.Count);
      result = (GsaResultGoo)ComponentTestHelper.GetOutput(comp);
      comp.SetSelected(0, 1);

      comp.SetSelected(0, 0);
      result = (GsaResultGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal("A1", comp._selectedItems[1]);
      Assert.Equal(2, comp._selectedItems.Count);
    }
  }
}
