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
  public class ContourNodeResultsTests {
    [Fact]
    public void CombinationCaseWithMultiplePermutationsMessageTests() {
      var caseResult = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 2, new List<int>() { 1, 2, 3, });

      var comp = new ContourNodeResults();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(caseResult));
      comp.Params.Output[0].CollectData();
      IList<string> messages = comp.RuntimeMessages(GH_RuntimeMessageLevel.Remark);

      Assert.Single(messages);
      Assert.Equal("Combination Case 2 contains 3 permutations which have been enveloped using Absolute method.\r\nChange the enveloping method by right-clicking the component.", messages[0]);
    }

    [Fact]
    public void DefaultDropSelectionsTest() {
      var comp = new ContourNodeResults();
      Assert.Equal("Displacement", comp.SelectedItems[0]);
      Assert.Equal("Resolved |U|", comp.SelectedItems[1]);

      comp.SetSelected(0, 1);
      Assert.Equal("Reaction", comp.SelectedItems[0]);
      Assert.Equal("Resolved |F|", comp.SelectedItems[1]);

      comp.SetSelected(0, 2);
      Assert.Equal("SpringForce", comp.SelectedItems[0]);
      Assert.Equal("Resolved |F|", comp.SelectedItems[1]);

      comp.SetSelected(0, 3);
      Assert.Equal("Footfall", comp.SelectedItems[0]);
      Assert.Equal("Resonant", comp.SelectedItems[1]);

      comp.SetSelected(0, 0);
      Assert.Equal("Displacement", comp.SelectedItems[0]);
      Assert.Equal("Resolved |U|", comp.SelectedItems[1]);
    }

    [Fact]
    public void SetMaxMinTest() {
      var comp = new ContourNodeResults();
      comp.SetMaxMin(100, 10);
      Assert.NotNull((DropDownSliderComponentAttributes)comp.Attributes);
    }

    [Fact]
    public void GetGradientTest() {
      var comp = new ContourNodeResults();
      GH_GradientControl gradient = comp.CreateGradient(new GH_Document());
      Assert.NotNull(gradient);
    }

    [Fact]
    public void DrawViewportMeshesAndWiresTest() {
      var comp = new ContourNodeResults();
      ComponentTestHelper.SetInput(comp, GsaResultTests.NodeAndElement1dCombinationResultsMother());
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
    }

    [Fact]
    public void DrawViewportMeshesAndWiresFootfallTest() {
      var comp = new ContourNodeResults();
      ComponentTestHelper.SetInput(comp, GsaResultTests.NodeAndElement1dFootfallResultsMother());
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 0, 3);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 0, 3);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 0);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 1);
    }

    [Fact]
    public void DrawViewportMeshesAndWiresSpringForceTest() {
      var comp = new ContourNodeResults();
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SpringForces, 1);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 0, 2);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 0);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 1);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 2);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 3);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 4);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 5);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 6);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 7);
    }

    [Fact]
    public void ShowLegendTest() {
      var comp = new ContourNodeResults();
      ComponentTestHelper.SetInput(comp, GsaResultTests.NodeAndElement1dCombinationResultsMother());
      comp.ShowLegend(null, null);
    }

    [Fact]
    public void UpdateForceTest() {
      var comp = new ContourNodeResults();
      ComponentTestHelper.SetInput(comp, GsaResultTests.NodeAndElement1dCombinationResultsMother());
      comp.UpdateForce("kN");
    }

    [Fact]
    public void UpdateLengthTest() {
      var comp = new ContourNodeResults();
      ComponentTestHelper.SetInput(comp, GsaResultTests.NodeAndElement1dCombinationResultsMother());
      comp.UpdateLength("mm");
    }

    [Fact]
    public void UpdateMomentTest() {
      var comp = new ContourNodeResults();
      ComponentTestHelper.SetInput(comp, GsaResultTests.NodeAndElement1dCombinationResultsMother());
      comp.UpdateMoment("N·cm");
    }

    private void SetSelectedDrawViewportMeshesAndWiresTest(ContourNodeResults comp, int i, int j) {
      comp.SetSelected(i, j);
      var resultsGoo = (PointResultGoo)ComponentTestHelper.GetOutput(comp);
      Assert.NotNull(resultsGoo);
      ComponentTestHelper.DrawViewportMeshesAndWiresTest(comp);
    }
  }
}
