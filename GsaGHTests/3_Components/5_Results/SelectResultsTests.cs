using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helper;
using GsaGHTests.Helpers;
using OasysGH.Components;
using Rhino.Commands;
using Xunit;

namespace GsaGHTests.Components.Results {
  [Collection("GrasshopperFixture collection")]
  public class SelectResultsTests {
    public static GH_OasysDropDownComponent ResultsComponentMother() {
      var open = new OpenModel();
      open.CreateAttributes();

      string file = GsaFile.SteelDesignComplex;
      ComponentTestHelper.SetInput(open, file);
      var modelInput = (GsaModelGoo)ComponentTestHelper.GetOutput(open);

      var comp = new SelectResult();
      comp.CreateAttributes();

      Assert.Equal("AnalysisCase", comp._selectedItems[0]);
      Assert.Equal("   ", comp._selectedItems[1]);

      ComponentTestHelper.SetInput(comp, modelInput, 0);

      return comp;
    }

    [Fact]
    public void DropSelectionsTest() {
      GH_OasysDropDownComponent comp = ResultsComponentMother();
      var result = (GsaResultGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal("A1", comp._selectedItems[1]);
      Assert.Equal(2, comp._selectedItems.Count);
      result = (GsaResultGoo)ComponentTestHelper.GetOutput(comp);
      comp.SetSelected(1, 1);
      Assert.Equal("A2", comp._selectedItems[1]);
      comp.SetSelected(1, 0);
      Assert.Equal("A1", comp._selectedItems[1]);

      comp.SetSelected(0, 1);
      Assert.Equal("C1", comp._selectedItems[1]);
      Assert.Equal("All", comp._selectedItems[2]);
      Assert.Equal(3, comp._selectedItems.Count);
      result = (GsaResultGoo)ComponentTestHelper.GetOutput(comp);
      comp.SetSelected(1, 1);
      Assert.Equal("C2", comp._selectedItems[1]);
      result = (GsaResultGoo)ComponentTestHelper.GetOutput(comp);
      comp.SetSelected(1, 2);
      Assert.Equal("C3", comp._selectedItems[1]);
      comp.SetSelected(1, 0);
      Assert.Equal("C1", comp._selectedItems[1]);

      result = (GsaResultGoo)ComponentTestHelper.GetOutput(comp);
      comp.SetSelected(2, 1);
      Assert.Equal("P1", comp._selectedItems[2]);

      result = (GsaResultGoo)ComponentTestHelper.GetOutput(comp);
      comp.SetSelected(2, 0);
      Assert.Equal("All", comp._selectedItems[2]);

      comp.SetSelected(0, 0);
      result = (GsaResultGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal("A1", comp._selectedItems[1]);
      Assert.Equal(2, comp._selectedItems.Count);
    }

    [Fact]
    public void SetInputsAfterRunTest() {
      GH_OasysDropDownComponent comp = ResultsComponentMother();
      var result = (GsaResultGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal("A1", comp._selectedItems[1]);
      Assert.Equal(2, comp._selectedItems.Count);
      result = (GsaResultGoo)ComponentTestHelper.GetOutput(comp);

      ComponentTestHelper.SetInput(comp, 2, 2);
      result = (GsaResultGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal("A2", comp._selectedItems[1]);

      ComponentTestHelper.SetInput(comp, "C", 1);
      result = (GsaResultGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal("C2", comp._selectedItems[1]);
    }

    [Fact]
    public void SetInputsBeforeRunTest() {
      GH_OasysDropDownComponent comp = ResultsComponentMother();
      var result = (GsaResultGoo)ComponentTestHelper.GetOutput(comp);
      ComponentTestHelper.SetInput(comp, "C", 1);
      result = (GsaResultGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal("C1", comp._selectedItems[1]);
    }

    [Fact]
    public void ChangeFromAnalysisCaseToNonExistentCombinationCase() {
      var comp = new SelectResult();

      var apiModel = new GsaAPI.Model(GsaFile.SteelDesignComplex);
      var model = new GsaModel(apiModel);
      ComponentTestHelper.SetInput(comp, new GsaModelGoo(model));
      comp.Params.Output[0].CollectData();

      // pick a high analysis case id
      comp.SetSelected(1, 12);
      comp.Params.Output[0].CollectData();
      Assert.Equal("A13", comp._selectedItems[1]);

      // change first dropdown to Combination case
      comp.SetSelected(0, 1);
      comp.Params.Output[0].CollectData();
      Assert.Equal("C4", comp._selectedItems[1]);
    }
  }
}
