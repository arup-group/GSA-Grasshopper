﻿using System.Collections.Generic;
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
  public class Contour1dResultsTests {
    [Fact]
    public void InvalidInputErrorTests() {
      var caseResult = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 2, new List<int>() { 1, 2, 3, });

      var comp = new Contour1dResults();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(caseResult));
      comp.Params.Output[0].CollectData();
      IList<string> messages = comp.RuntimeMessages(GH_RuntimeMessageLevel.Warning);

      Assert.Single(messages);
      Assert.Equal("Combination Case 2 contains 3 permutations - only one permutation can be displayed at a time.\r\nDisplaying first permutation; please use the 'Select Results' to select other single permutations", messages[0]);
    }

    [Fact]
    public void DefaultDropSelectionsTest() {
      var comp = new Contour1dResults();
      Assert.Equal("Displacement", comp._selectedItems[0]);
      Assert.Equal("Resolved |U|", comp._selectedItems[1]);

      comp.SetSelected(0, 1);
      Assert.Equal("Force", comp._selectedItems[0]);
      Assert.Equal("Moment Myy", comp._selectedItems[1]);

      comp.SetSelected(0, 2);
      Assert.Equal("Stress", comp._selectedItems[0]);
      Assert.Equal("Combined, C1", comp._selectedItems[1]);

      comp.SetSelected(0, 3);
      Assert.Equal("Derived Stress", comp._selectedItems[0]);
      Assert.Equal("Von Mises", comp._selectedItems[1]);

      comp.SetSelected(0, 4);
      Assert.Equal("Strain Energy", comp._selectedItems[0]);
      Assert.Equal("Average", comp._selectedItems[1]);

      comp.SetSelected(0, 5);
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

      SetSelectedDrawViewportMeshesAndWiresTest(comp, 0, 2);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 0);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 1);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 2);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 3);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 4);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 5);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 6);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 7);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 8);

      SetSelectedDrawViewportMeshesAndWiresTest(comp, 0, 3);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 0);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 1);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 2);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 3);

      SetSelectedDrawViewportMeshesAndWiresTest(comp, 0, 4);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 0);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 1);
    }

    [Fact]
    public void DrawViewportMeshesAndWiresFootfallTest() {
      var comp = new Contour1dResults();
      ComponentTestHelper.SetInput(comp, GsaResultTests.NodeAndElement1dFootfallResultsMother());
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 0, 5);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 0);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 1);
    }

    [Fact]
    public void ShowLegendTest() {
      var comp = new Contour1dResults();
      ComponentTestHelper.SetInput(comp, GsaResultTests.NodeAndElement1dCombinationResultsMother());
      comp.ShowLegend(null, null);
    }

    [Fact]
    public void UpdateForceTest() {
      var comp = new Contour1dResults();
      ComponentTestHelper.SetInput(comp, GsaResultTests.NodeAndElement1dCombinationResultsMother());
      comp.UpdateForce("kN");
    }

    [Fact]
    public void UpdateLengthTest() {
      var comp = new Contour1dResults();
      ComponentTestHelper.SetInput(comp, GsaResultTests.NodeAndElement1dCombinationResultsMother());
      comp.UpdateLength("mm");
    }

    [Fact]
    public void UpdateMomentTest() {
      var comp = new Contour1dResults();
      ComponentTestHelper.SetInput(comp, GsaResultTests.NodeAndElement1dCombinationResultsMother());
      comp.UpdateMoment("N·cm");
    }

    [Fact]
    public void UpdateStressTest() {
      var comp = new Contour1dResults();
      ComponentTestHelper.SetInput(comp, GsaResultTests.NodeAndElement1dCombinationResultsMother());
      comp.UpdateStress("MPa");
    }

    [Fact]
    public void UpdateEnergyTest() {
      var comp = new Contour1dResults();
      ComponentTestHelper.SetInput(comp, GsaResultTests.NodeAndElement1dCombinationResultsMother());
      comp.UpdateEnergy("J");
    }

    [Fact]
    public void UpdateLegendScaleTest() {
      var comp = new Contour1dResults();
      ComponentTestHelper.SetInput(comp, GsaResultTests.NodeAndElement1dCombinationResultsMother());
      comp.UpdateLegendScale();
    }

    private void SetSelectedDrawViewportMeshesAndWiresTest(Contour1dResults comp, int i, int j) {
      comp.SetSelected(i, j);
      var resultsGoo = (LineResultGoo)ComponentTestHelper.GetOutput(comp);
      Assert.NotNull(resultsGoo);
      ComponentTestHelper.DrawViewportMeshesAndWiresTest(comp);
    }
  }
}
