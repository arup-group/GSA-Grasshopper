using Grasshopper.Kernel.Special;
using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Analysis;
using GsaGHTests.Helpers;
using OasysGH.UI;
using Xunit;

namespace GsaGHTests.Components.Display {
  [Collection("GrasshopperFixture collection")]
  public class Contour1dResultsTests {
    [Fact]
    public void DefaultDropSelectionsTest() {
      var comp = new Contour1dResults();
      Assert.Equal("Displacement", comp._selectedItems[0]);
      Assert.Equal("Resolved |U|", comp._selectedItems[1]);

      comp.SetSelected(0, 1);
      Assert.Equal("Force", comp._selectedItems[0]);
      Assert.Equal("Moment Myy", comp._selectedItems[1]);

      comp.SetSelected(0, 2);
      Assert.Equal("Strain Energy", comp._selectedItems[0]);
      Assert.Equal("Average", comp._selectedItems[1]);

      comp.SetSelected(0, 3);
      Assert.Equal("Footfall", comp._selectedItems[0]);
      Assert.Equal("Resonant", comp._selectedItems[1]);

      comp.SetSelected(0, 0);
      Assert.Equal("Displacement", comp._selectedItems[0]);
      Assert.Equal("Resolved |U|", comp._selectedItems[1]);
    }

    [Fact]
    public void SetMaxMinTest() {
      var comp = new Contour1dResults();
      comp.SetMaxMin(100, 10);
      Assert.NotNull((DropDownSliderComponentAttributes)comp.Attributes);
    }

    [Fact]
    public void GetGradientTest() {
      var comp = new Contour1dResults();
      GH_GradientControl gradient = comp.CreateGradient(new Grasshopper.Kernel.GH_Document());
      Assert.NotNull(gradient);
    }

    [Fact]
    public void DrawViewportMeshesAndWiresTest() {
      var comp = new Contour1dResults();
      ComponentTestHelper.SetInput(comp, GetResultsTest.NodeAndElement1dCombinationResultsMother());
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 0, 0);

      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 0);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 1);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 2);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 3);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 4);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 5);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 6);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 7);

      SetSelectedDrawViewportMeshesAndWiresTest(comp, 0, 1);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 0);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 1);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 2);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 3);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 4);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 5);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 6);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 7);

      SetSelectedDrawViewportMeshesAndWiresTest(comp, 0, 2);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 0);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 1);
    }

    [Fact]
    public void DrawViewportMeshesAndWiresFootfallTest() {
      var comp = new Contour1dResults();
      ComponentTestHelper.SetInput(comp, GetResultsTest.NodeAndElement1dFootfallResultsMother());
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 0, 3);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 0);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 1);
    }

    private void SetSelectedDrawViewportMeshesAndWiresTest(Contour1dResults comp, int i, int j) {
      comp.SetSelected(i, j);
      var resultsGoo = (LineResultGoo)ComponentTestHelper.GetOutput(comp);
      Assert.NotNull(resultsGoo);
      ComponentTestHelper.DrawViewportMeshesAndWiresTest(comp);
    }
  }
}
