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
  public class Contour3dResultsTests {
    [Fact]
    public void CombinationCaseWithMultiplePermutationsMessageTests() {
      var caseResult = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 2, new List<int>() { 1, 2, 3, });

      var comp = new Contour3dResults();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(caseResult));
      comp.Params.Output[0].CollectData();
      IList<string> messages = comp.RuntimeMessages(GH_RuntimeMessageLevel.Remark);

      Assert.Single(messages);
      Assert.Equal("Combination Case 2 contains 3 permutations which have been enveloped using Absolute method.\r\nChange the enveloping method by right-clicking the component.", messages[0]);
    }

    [Fact]
    public void DefaultDropSelectionsTest() {
      var comp = new Contour3dResults();
      Assert.Equal("Displacement", comp.SelectedItems[0]);
      Assert.Equal("Resolved |U|", comp.SelectedItems[1]);

      comp.SetSelected(0, 1);
      Assert.Equal("Stress", comp.SelectedItems[0]);
      Assert.Equal("Stress zz", comp.SelectedItems[1]);

      comp.SetSelected(0, 0);
      Assert.Equal("Displacement", comp.SelectedItems[0]);
      Assert.Equal("Resolved |U|", comp.SelectedItems[1]);
    }

    [Fact]
    public void SetMaxMinTest() {
      var comp = new Contour3dResults();
      comp.SetMaxMin(100, 10);
      Assert.NotNull((DropDownSliderComponentAttributes)comp.Attributes);
    }

    [Fact]
    public void GetGradientTest() {
      var comp = new Contour3dResults();
      GH_GradientControl gradient = comp.CreateGradient(new GH_Document());
      Assert.NotNull(gradient);
    }

    [Fact]
    public void DrawViewportMeshesAndWiresTest() {
      var comp = new Contour3dResults();
      ComponentTestHelper.SetInput(comp, GsaResultTests.NodeAndElement3dCombinationResultsMother());
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
    }

    [Fact]
    public void ShowLegendTest() {
      var comp = new Contour3dResults();
      ComponentTestHelper.SetInput(comp, GsaResultTests.NodeAndElement3dCombinationResultsMother());
      comp.ShowLegend(null, null);
    }

    [Fact]
    public void UpdateLengthTest() {
      var comp = new Contour3dResults();
      ComponentTestHelper.SetInput(comp, GsaResultTests.NodeAndElement3dCombinationResultsMother());
      comp.UpdateLength("mm");
    }

    [Fact]
    public void UpdateStressTest() {
      var comp = new Contour3dResults();
      ComponentTestHelper.SetInput(comp, GsaResultTests.NodeAndElement3dCombinationResultsMother());
      comp.UpdateStress("MPa");
    }

    private void SetSelectedDrawViewportMeshesAndWiresTest(Contour3dResults comp, int i, int j) {
      comp.SetSelected(i, j);
      var resultsGoo = (MeshResultGoo)ComponentTestHelper.GetOutput(comp);
      Assert.NotNull(resultsGoo);
      ComponentTestHelper.DrawViewportMeshesAndWiresTest(comp);
    }
  }
}
