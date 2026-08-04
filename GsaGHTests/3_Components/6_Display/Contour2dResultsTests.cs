using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;

using GsaGH.Components;
using GsaGH.Parameters;
using GsaGH.Parameters.Results;

using GsaGHTests.Helper;
using GsaGHTests.Helpers;
using GsaGHTests.Parameters;

using OasysGH.UI;

using Xunit;

namespace GsaGHTests.Components.Display {
  [Collection("GrasshopperFixture collection")]
  public class Contour2dResultsTests {
    [Fact]
    public void CombinationCaseWithMultiplePermutationsMessageTests() {
      var caseResult = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 2, new List<int>() { 1, 2, 3, });

      var comp = new Contour2dResults();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(caseResult));
      comp.Params.Output[0].CollectData();
      IList<string> messages = comp.RuntimeMessages(GH_RuntimeMessageLevel.Remark);

      Assert.Single(messages);
      Assert.Equal("Combination Case 2 contains 3 permutations which have been enveloped using Absolute method.\r\nChange the enveloping method by right-clicking the component.", messages[0]);
    }

    [Fact]
    public void DefaultDropSelectionsTest() {
      var comp = new Contour2dResults();
      Assert.Equal("Displacement", comp.SelectedItems[0]);
      Assert.Equal("Resolved |U|", comp.SelectedItems[1]);

      comp.SetSelected(0, 1);
      Assert.Equal("Force", comp.SelectedItems[0]);
      Assert.Equal("Force Nx", comp.SelectedItems[1]);

      comp.SetSelected(0, 2);
      Assert.Equal("Stress", comp.SelectedItems[0]);
      Assert.Equal("Stress xx", comp.SelectedItems[1]);
      Assert.Equal("Middle", comp.SelectedItems[2]);

      comp.SetSelected(0, 3);
      Assert.Equal("Footfall", comp.SelectedItems[0]);
      Assert.Equal("Resonant", comp.SelectedItems[1]);

      comp.SetSelected(0, 0);
      Assert.Equal("Displacement", comp.SelectedItems[0]);
      Assert.Equal("Resolved |U|", comp.SelectedItems[1]);
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
      GH_GradientControl gradient = comp.CreateGradient(new GH_Document());
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

    private void SetSelectedDrawViewportMeshesAndWiresTest(Contour2dResults comp, int i, int j) {
      comp.SetSelected(i, j);
      var resultsGoo = (MeshResultGoo)ComponentTestHelper.GetOutput(comp);
      Assert.NotNull(resultsGoo);
      ComponentTestHelper.DrawViewportMeshesAndWiresTest(comp);
    }

    [Fact]
    public void ContourResultsComponentShouldNotThrowException() {
      var comp = new Contour2dResults();
      ComponentTestHelper.SetInput(comp, GsaResultTests.FabricModelResultsMother());

      //displacement
      ShouldNotThrowException(comp, 0, 0);

      ShouldNotThrowException(comp, 1, 0);
      ShouldNotThrowException(comp, 1, 1);
      ShouldNotThrowException(comp, 1, 2);
      ShouldNotThrowException(comp, 1, 3);

      //forces
      ShouldNotThrowException(comp, 0, 1);

      ShouldNotThrowException(comp, 1, 0);
      ShouldNotThrowException(comp, 1, 1);
      ShouldNotThrowException(comp, 1, 2);
      ShouldNotThrowException(comp, 1, 3);
      ShouldNotThrowException(comp, 1, 4);
      ShouldNotThrowException(comp, 1, 5);
      ShouldNotThrowException(comp, 1, 6);
      ShouldNotThrowException(comp, 1, 7);
      ShouldNotThrowException(comp, 1, 8);
      ShouldNotThrowException(comp, 1, 9);

      //strress
      ShouldNotThrowException(comp, 0, 2);
      ShouldNotThrowException(comp, 2, 0);

      ShouldNotThrowException(comp, 1, 0);
      ShouldNotThrowException(comp, 1, 1);
      ShouldNotThrowException(comp, 1, 2);
      ShouldNotThrowException(comp, 1, 3);
      ShouldNotThrowException(comp, 1, 4);
      ShouldNotThrowException(comp, 1, 5);

      ShouldNotThrowException(comp, 2, 1);

      ShouldNotThrowException(comp, 1, 0);
      ShouldNotThrowException(comp, 1, 1);
      ShouldNotThrowException(comp, 1, 2);
      ShouldNotThrowException(comp, 1, 3);
      ShouldNotThrowException(comp, 1, 4);
      ShouldNotThrowException(comp, 1, 5);

      ShouldNotThrowException(comp, 2, 2);

      ShouldNotThrowException(comp, 1, 0);
      ShouldNotThrowException(comp, 1, 1);
      ShouldNotThrowException(comp, 1, 2);
      ShouldNotThrowException(comp, 1, 3);
      ShouldNotThrowException(comp, 1, 4);
      ShouldNotThrowException(comp, 1, 5);

      //footfall
      ShouldNotThrowException(comp, 0, 3);
      ShouldNotThrowException(comp, 1, 0);
      ShouldNotThrowException(comp, 1, 1);
    }

    private void ShouldNotThrowException(Contour2dResults component, int i, int j) {
      component.SetSelected(i, j);
      var resultsGoo = (MeshResultGoo)ComponentTestHelper.GetOutput(component);
      if(resultsGoo == null) {
        Assert.Single(component.RuntimeMessages(GH_RuntimeMessageLevel.Warning));
        Assert.Contains("Result is not available for the selected component", component.RuntimeMessages(GH_RuntimeMessageLevel.Warning)[0]);
      }
    }

  }
}
