﻿using Grasshopper.Kernel;
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
  public class AssemblyResultsTests {
    [Fact]
    public void DefaultDropSelectionsTest() {
      var comp = new AssemblyResults();
      Assert.Equal("Displacement", comp._selectedItems[0]);
      Assert.Equal("Resolved |U|", comp._selectedItems[1]);

      comp.SetSelected(0, 1);
      Assert.Equal("Drift", comp._selectedItems[0]);
      Assert.Equal("Drift Dx", comp._selectedItems[1]);

      comp.SetSelected(0, 2);
      Assert.Equal("Drift Index", comp._selectedItems[0]);
      Assert.Equal("Drift Index DIx", comp._selectedItems[1]);

      comp.SetSelected(0, 3);
      Assert.Equal("Force", comp._selectedItems[0]);
      Assert.Equal("Moment Myy", comp._selectedItems[1]);
    }

    [Fact]
    public void SetMaxMinTest() {
      var comp = new AssemblyResults();
      comp.SetMaxMin(100, 10);
      Assert.NotNull((DropDownSliderComponentAttributes)comp.Attributes);
    }

    [Fact]
    public void GetGradientTest() {
      var comp = new AssemblyResults();
      GH_GradientControl gradient = comp.CreateGradient(new GH_Document());
      Assert.NotNull(gradient);
    }

    [Fact]
    public void DrawViewportMeshesAndWiresTest() {
      var comp = new AssemblyResults();
      GsaResult result = GsaResultTests.AnalysisCaseResult(GsaFile.AssemblyByStorey, 1);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));

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

      SetSelectedDrawViewportMeshesAndWiresTest(comp, 0, 2);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 0);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 1);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 2);

      SetSelectedDrawViewportMeshesAndWiresTest(comp, 0, 3);
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
      var comp = new AssemblyResults();
      GsaResult result = GsaResultTests.AnalysisCaseResult(GsaFile.AssemblyByStorey, 1);
      ComponentTestHelper.SetInput(comp, result);
      comp.ShowLegend(null, null);
    }

    [Fact]
    public void UpdateForceTest() {
      var comp = new AssemblyResults();
      GsaResult result = GsaResultTests.AnalysisCaseResult(GsaFile.AssemblyByStorey, 1);
      ComponentTestHelper.SetInput(comp, result);
      comp.UpdateForce("kN");
    }

    [Fact]
    public void UpdateLengthTest() {
      var comp = new AssemblyResults();
      GsaResult result = GsaResultTests.AnalysisCaseResult(GsaFile.AssemblyByStorey, 1);
      ComponentTestHelper.SetInput(comp, result);
      comp.UpdateLength("mm");
    }

    [Fact]
    public void UpdateMomentTest() {
      var comp = new AssemblyResults();
      GsaResult result = GsaResultTests.AnalysisCaseResult(GsaFile.AssemblyByStorey, 1);
      ComponentTestHelper.SetInput(comp, result);
      comp.UpdateMoment("N·cm");
    }

    [Fact]
    public void UpdateLegendScaleTest() {
      var comp = new AssemblyResults();
      GsaResult result = GsaResultTests.AnalysisCaseResult(GsaFile.AssemblyByStorey, 1);
      ComponentTestHelper.SetInput(comp, result);
      comp.UpdateLegendScale();
    }

    private void SetSelectedDrawViewportMeshesAndWiresTest(AssemblyResults comp, int i, int j) {
      comp.SetSelected(i, j);
      var resultsGoo = (LineResultGoo)ComponentTestHelper.GetOutput(comp);
      Assert.NotNull(resultsGoo);
      ComponentTestHelper.DrawViewportMeshesAndWiresTest(comp);
    }
  }
}
