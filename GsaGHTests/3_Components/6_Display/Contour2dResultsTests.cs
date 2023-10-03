using Grasshopper.Kernel.Special;
using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Analysis;
using GsaGHTests.Helpers;
using GsaGHTests.Parameters;
using OasysGH.UI;
using Xunit;

namespace GsaGHTests.Components.Display {
  [Collection("GrasshopperFixture collection")]
  public class Contour2dResultsTests {
    [Fact]
    public void DefaultDropSelectionsTest() {
      var comp = new Contour2dResults();
      Assert.Equal("Displacement", comp._selectedItems[0]);
      Assert.Equal("Resolved |U|", comp._selectedItems[1]);

      comp.SetSelected(0, 1);
      Assert.Equal("Force", comp._selectedItems[0]);
      Assert.Equal("Force Nx", comp._selectedItems[1]);

      comp.SetSelected(0, 2);
      Assert.Equal("Stress", comp._selectedItems[0]);
      Assert.Equal("Stress xx", comp._selectedItems[1]);
      Assert.Equal("Middle", comp._selectedItems[2]);

      comp.SetSelected(0, 3);
      Assert.Equal("Footfall", comp._selectedItems[0]);
      Assert.Equal("Resonant", comp._selectedItems[1]);

      comp.SetSelected(0, 0);
      Assert.Equal("Displacement", comp._selectedItems[0]);
      Assert.Equal("Resolved |U|", comp._selectedItems[1]);
    }

    [Fact]
    public void SetMaxMinTest() {
      var comp = new Contour2dResults();
      comp.SetMaxMin(100, 10);
      Assert.NotNull((DropDownSliderComponentAttributes)comp.Attributes);
    }

    [Fact]
    public void GetGradientTest() {
      var comp = new Contour2dResults();
      GH_GradientControl gradient = comp.CreateGradient(new Grasshopper.Kernel.GH_Document());
      Assert.NotNull(gradient);
    }

    [Fact]
    public void DrawViewportMeshesAndWiresTest() {
      var comp = new Contour2dResults();
      ComponentTestHelper.SetInput(comp, GsaResultTests.NodeAndElement2dCombinationResultsMother());
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 0, 0);

      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 0);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 1);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 2);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 3);

      SetSelectedDrawViewportMeshesAndWiresTest(comp, 0, 1);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 0);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 1);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 2);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 3);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 4);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 5);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 6);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 7);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 8);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 9);

      SetSelectedDrawViewportMeshesAndWiresTest(comp, 0, 2);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 2, 0);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 0);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 1);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 2);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 3);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 4);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 5);

      SetSelectedDrawViewportMeshesAndWiresTest(comp, 2, 1);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 0);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 1);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 2);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 3);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 4);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 5);

      SetSelectedDrawViewportMeshesAndWiresTest(comp, 2, 2);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 0);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 1);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 2);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 3);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 4);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 5);

      SetSelectedDrawViewportMeshesAndWiresTest(comp, 0, 0);
    }

    [Fact]
    public void DrawViewportMeshesAndWiresFootfallTest() {
      var comp = new Contour2dResults();
      ComponentTestHelper.SetInput(comp, GsaResultTests.NodeAndElement2dFootfallResultsMother());
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 0, 3);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 0);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 1);
    }

    [Fact]
    public void ShowLegendTest() {
      var comp = new Contour2dResults();
      ComponentTestHelper.SetInput(comp, GsaResultTests.NodeAndElement2dCombinationResultsMother());
      comp.ShowLegend(null, null);
    }

    [Fact]
    public void UpdateForceTest() {
      var comp = new Contour2dResults();
      ComponentTestHelper.SetInput(comp, GsaResultTests.NodeAndElement2dCombinationResultsMother());
      comp.UpdateForce("kN/m");
    }

    [Fact]
    public void UpdateLengthTest() {
      var comp = new Contour2dResults();
      ComponentTestHelper.SetInput(comp, GsaResultTests.NodeAndElement2dCombinationResultsMother());
      comp.UpdateLength("mm");
    }

    [Fact]
    public void UpdateModelTest() {
      var comp = new Contour2dResults();
      ComponentTestHelper.SetInput(comp, GsaResultTests.NodeAndElement2dCombinationResultsMother());
      comp.UpdateModel("mm");
    }

    [Fact]
    public void UpdateMomentTest() {
      var comp = new Contour2dResults();
      ComponentTestHelper.SetInput(comp, GsaResultTests.NodeAndElement2dCombinationResultsMother());
      comp.UpdateMoment("kN");
    }

    [Fact]
    public void UpdateStressTest() {
      var comp = new Contour2dResults();
      ComponentTestHelper.SetInput(comp, GsaResultTests.NodeAndElement2dCombinationResultsMother());
      comp.UpdateStress("MPa");
    }

    [Fact]
    public void UpdateLegendScaleTest() {
      var comp = new Contour2dResults();
      ComponentTestHelper.SetInput(comp, GsaResultTests.NodeAndElement2dCombinationResultsMother());
      comp.UpdateLegendScale();
    }

    private void SetSelectedDrawViewportMeshesAndWiresTest(Contour2dResults comp, int i, int j) {
      comp.SetSelected(i, j);
      var resultsGoo = (MeshResultGoo)ComponentTestHelper.GetOutput(comp);
      Assert.NotNull(resultsGoo);
      ComponentTestHelper.DrawViewportMeshesAndWiresTest(comp);
    }
  }
}
