using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Helper;
using GsaGHTests.Helpers;

using OasysGH.Components;

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

      Assert.Equal("AnalysisCase", comp.SelectedItems[0]);
      Assert.Equal("   ", comp.SelectedItems[1]);

      ComponentTestHelper.SetInput(comp, modelInput, 0);

      return comp;
    }

    [Fact]
    public void DropSelectionsTest() {
      GH_OasysDropDownComponent comp = ResultsComponentMother();
      var result = (GsaResultGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal("A1", comp.SelectedItems[1]);
      Assert.Equal(2, comp.SelectedItems.Count);
      result = (GsaResultGoo)ComponentTestHelper.GetOutput(comp);
      comp.SetSelected(1, 1);
      Assert.Equal("A2", comp.SelectedItems[1]);
      comp.SetSelected(1, 0);
      Assert.Equal("A1", comp.SelectedItems[1]);

      comp.SetSelected(0, 1);
      Assert.Equal("C1", comp.SelectedItems[1]);
      Assert.Equal("All", comp.SelectedItems[2]);
      Assert.Equal(3, comp.SelectedItems.Count);
      result = (GsaResultGoo)ComponentTestHelper.GetOutput(comp);
      comp.SetSelected(1, 1);
      Assert.Equal("C2", comp.SelectedItems[1]);
      result = (GsaResultGoo)ComponentTestHelper.GetOutput(comp);
      comp.SetSelected(1, 2);
      Assert.Equal("C3", comp.SelectedItems[1]);
      comp.SetSelected(1, 0);
      Assert.Equal("C1", comp.SelectedItems[1]);

      result = (GsaResultGoo)ComponentTestHelper.GetOutput(comp);
      comp.SetSelected(2, 1);
      Assert.Equal("P1", comp.SelectedItems[2]);

      result = (GsaResultGoo)ComponentTestHelper.GetOutput(comp);
      comp.SetSelected(2, 0);
      Assert.Equal("All", comp.SelectedItems[2]);

      comp.SetSelected(0, 0);
      result = (GsaResultGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal("A1", comp.SelectedItems[1]);
      Assert.Equal(2, comp.SelectedItems.Count);
    }

    [Fact]
    public void SetInputsAfterRunTest() {
      GH_OasysDropDownComponent comp = ResultsComponentMother();
      var result = (GsaResultGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal("A1", comp.SelectedItems[1]);
      Assert.Equal(2, comp.SelectedItems.Count);
      result = (GsaResultGoo)ComponentTestHelper.GetOutput(comp);

      ComponentTestHelper.SetInput(comp, 2, 2);
      result = (GsaResultGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal("A2", comp.SelectedItems[1]);

      ComponentTestHelper.SetInput(comp, "C", 1);
      result = (GsaResultGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal("C2", comp.SelectedItems[1]);
    }

    [Fact]
    public void SetInputsBeforeRunTest() {
      GH_OasysDropDownComponent comp = ResultsComponentMother();
      var result = (GsaResultGoo)ComponentTestHelper.GetOutput(comp);
      ComponentTestHelper.SetInput(comp, "C", 1);
      result = (GsaResultGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal("C1", comp.SelectedItems[1]);
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
      Assert.Equal("A13", comp.SelectedItems[1]);

      // change first dropdown to Combination case
      comp.SetSelected(0, 1);
      comp.Params.Output[0].CollectData();
      Assert.Equal("C4", comp.SelectedItems[1]);
    }

    [Fact]
    public void UpdateDropdownsTest() {
      var comp = new SelectResult();
      var apiModel = new GsaAPI.Model(GsaFile.SteelDesignComplex);
      var model = new GsaModel(apiModel);
      ComponentTestHelper.SetInput(comp, new GsaModelGoo(model));
      comp.Params.Output[0].CollectData();
      Assert.Equal(2, comp.DropDownItems.Count);
      Assert.Equal(2, comp.DropDownItems[0].Count);
      Assert.Equal(13, comp.DropDownItems[1].Count);
      comp.SetSelected(0, 1); // combination case
      comp.Params.Output[0].CollectData();
      Assert.Equal(3, comp.DropDownItems.Count);
      Assert.Equal(2, comp.DropDownItems[0].Count);
      Assert.Equal(4, comp.DropDownItems[1].Count);
      Assert.Equal(2, comp.DropDownItems[2].Count); // All, P1
      comp.SetSelected(1, 3); // C4 (contains 2 permutations)
      comp.Params.Output[0].CollectData();
      Assert.Equal(3, comp.DropDownItems[2].Count); // All, P1, P2
    }

    [Fact]
    public void UpdateModelTest() {
      var comp = new SelectResult();
      var apiModel1 = new GsaAPI.Model(GsaFile.SteelDesignComplex);
      var model1 = new GsaModel(apiModel1);
      ComponentTestHelper.SetInput(comp, new GsaModelGoo(model1));
      comp.Params.Output[0].CollectData();
      Assert.Equal(2, comp.DropDownItems.Count);
      Assert.Equal(2, comp.DropDownItems[0].Count);
      Assert.Equal(13, comp.DropDownItems[1].Count);
      var apiModel2 = new GsaAPI.Model(GsaFile.SteelDesignSimple);
      var model2 = new GsaModel(apiModel2);
      ComponentTestHelper.SetInput(comp, new GsaModelGoo(model2));
      comp.Params.Output[0].CollectData();
      Assert.Equal(2, comp.DropDownItems.Count);
      Assert.Equal(2, comp.DropDownItems[0].Count);
      Assert.Equal(2, comp.DropDownItems[1].Count);
    }

    [Fact]
    public void InvalidModelInputsTest() {
      var comp = new SelectResult();
      ComponentTestHelper.SetInput(comp, "not a model");
      comp.Params.Output[0].CollectData();
      Assert.True((int)comp.RuntimeMessageLevel >= 10);
    }

    [Fact]
    public void SetCombinationCaseByDropdownThenAnalysisCaseByInput() {
      var comp = new SelectResult();
      var apiModel = new GsaAPI.Model(GsaFile.SteelDesignComplex);
      var model = new GsaModel(apiModel);
      ComponentTestHelper.SetInput(comp, new GsaModelGoo(model));
      comp.SetSelected(0, 1); // combination case
      comp.Params.Output[0].CollectData();
      comp.SetSelected(1, 1); // case C2
      comp.Params.Output[0].CollectData();
      Assert.Equal("Combination", comp.SelectedItems[0]);
      Assert.Equal(3, comp.DropDownItems.Count);
      ComponentTestHelper.SetInput(comp, "Analysis", 1);
      comp.Params.Output[0].CollectData();
      Assert.Equal("AnalysisCase", comp.SelectedItems[0]);
      Assert.Equal("A2", comp.SelectedItems[1]);
      Assert.Equal(2, comp.DropDownItems.Count);
    }

    [Fact]
    public void SetAnalysisCaseByDropdownThenCombinationCaseByInput() {
      var comp = new SelectResult();
      var apiModel = new GsaAPI.Model(GsaFile.SteelDesignComplex);
      var model = new GsaModel(apiModel);
      ComponentTestHelper.SetInput(comp, new GsaModelGoo(model));
      comp.SetSelected(0, 0); // analysis case
      comp.Params.Output[0].CollectData();
      comp.SetSelected(1, 1); // case A2
      comp.Params.Output[0].CollectData();
      Assert.Equal("AnalysisCase", comp.SelectedItems[0]);
      Assert.Equal(2, comp.DropDownItems.Count);
      ComponentTestHelper.SetInput(comp, "Combination", 1);
      comp.Params.Output[0].CollectData();
      Assert.Equal("Combination", comp.SelectedItems[0]);
      Assert.Equal("C2", comp.SelectedItems[1]);
      Assert.Equal(3, comp.DropDownItems.Count);
    }

    [Fact]
    public void SetAnalysisCaseIdByInput() {
      var comp = new SelectResult();
      var apiModel = new GsaAPI.Model(GsaFile.SteelDesignComplex);
      var model = new GsaModel(apiModel);
      ComponentTestHelper.SetInput(comp, new GsaModelGoo(model));
      ComponentTestHelper.SetInput(comp, 0, 2);
      comp.Params.Output[0].CollectData();
      Assert.Equal("All", comp.SelectedItems[1]);
      ComponentTestHelper.SetInput(comp, 2, 2);
      comp.Params.Output[0].CollectData();
      Assert.Equal("A2", comp.SelectedItems[1]);
    }

    [Fact]
    public void SetPermutationIdByInput() {
      var comp = new SelectResult();
      var apiModel = new GsaAPI.Model(GsaFile.SteelDesignComplex);
      var model = new GsaModel(apiModel);
      ComponentTestHelper.SetInput(comp, new GsaModelGoo(model));
      comp.SetSelected(0, 1); // combination case
      comp.Params.Output[0].CollectData();
      comp.SetSelected(1, 3); // C4
      comp.Params.Output[0].CollectData();
      comp.SetSelected(2, 2); // C4p2
      comp.Params.Output[0].CollectData();
      Assert.Equal("P2", comp.SelectedItems[2]);
      ComponentTestHelper.SetInput(comp, 0, 3);
      comp.Params.Output[0].CollectData();
      Assert.Equal("All", comp.SelectedItems[2]);
      ComponentTestHelper.SetInput(comp, 2, 3);
      comp.Params.Output[0].CollectData();
      Assert.Equal("from input", comp.SelectedItems[2]);
    }
  }
}
