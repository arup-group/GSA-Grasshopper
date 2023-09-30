using System.Collections.Generic;
using Grasshopper.Kernel;
using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helper;
using GsaGHTests.Helpers;
using GsaGHTests.Model;
using OasysGH.Components;
using Xunit;

namespace GsaGHTests.Analysis {
  [Collection("GrasshopperFixture collection")]
  public class GetResultsTest {
    public static GsaResultGoo NodeAndElement1dCombinationResultsMother() {
      var open = new OpenModel();
      open.CreateAttributes();
      string file = GsaFile.SteelDesignComplex;
      ComponentTestHelper.SetInput(open, file);
      var model = (GsaModelGoo)ComponentTestHelper.GetOutput(open);
      var getResults = new GetResult();
      getResults.CreateAttributes();
      
      ComponentTestHelper.SetInput(getResults, model);
      ComponentTestHelper.SetInput(getResults, "C", 1);

      return (GsaResultGoo)ComponentTestHelper.GetOutput(getResults);
    }

    public static GsaResultGoo NodeAndElement2dCombinationResultsMother() {
      var open = new OpenModel();
      open.CreateAttributes();
      string file = GsaFile.Element2dSimple;
      ComponentTestHelper.SetInput(open, file);
      var model = (GsaModelGoo)ComponentTestHelper.GetOutput(open);
      var getResults = new GetResult();
      getResults.CreateAttributes();

      ComponentTestHelper.SetInput(getResults, model);
      ComponentTestHelper.SetInput(getResults, "C", 1);

      return (GsaResultGoo)ComponentTestHelper.GetOutput(getResults);
    }

    public static GsaResultGoo NodeAndElement3dCombinationResultsMother() {
      var open = new OpenModel();
      open.CreateAttributes();
      string file = GsaFile.Element3dSimple;
      ComponentTestHelper.SetInput(open, file);
      var model = (GsaModelGoo)ComponentTestHelper.GetOutput(open);
      var getResults = new GetResult();
      getResults.CreateAttributes();

      ComponentTestHelper.SetInput(getResults, model);
      ComponentTestHelper.SetInput(getResults, "C", 1);

      return (GsaResultGoo)ComponentTestHelper.GetOutput(getResults);
    }

    public static GsaResultGoo NodeAndElement1dFootfallResultsMother() {
      var open = new OpenModel();
      open.CreateAttributes();
      string file = GsaFile.SteelDesignComplex;
      ComponentTestHelper.SetInput(open, file);
      var model = (GsaModelGoo)ComponentTestHelper.GetOutput(open);
      var getResults = new GetResult();
      getResults.CreateAttributes();

      ComponentTestHelper.SetInput(getResults, model);
      ComponentTestHelper.SetInput(getResults, "A", 1);
      ComponentTestHelper.SetInput(getResults, 13, 2);

      return (GsaResultGoo)ComponentTestHelper.GetOutput(getResults);
    }

    public static GsaResultGoo NodeAndElement2dFootfallResultsMother() {
      var open = new OpenModel();
      open.CreateAttributes();
      string file = GsaFile.Element2dSimple;
      ComponentTestHelper.SetInput(open, file);
      var model = (GsaModelGoo)ComponentTestHelper.GetOutput(open);
      var getResults = new GetResult();
      getResults.CreateAttributes();

      ComponentTestHelper.SetInput(getResults, model);
      ComponentTestHelper.SetInput(getResults, "A", 1);
      ComponentTestHelper.SetInput(getResults, 13, 2);

      return (GsaResultGoo)ComponentTestHelper.GetOutput(getResults);
    }

    public static GH_OasysComponent ResultsComponentMother() {
      var comp = new GetResult();
      comp.CreateAttributes();

      GsaModelGoo modelInput = ModelTests.GsaModelGooMother;
      ComponentTestHelper.SetInput(comp, modelInput, 0);

      return comp;
    }

    [Fact]
    public void TestAnalysisNoInputs() {
      var comp = (GetResult)ResultsComponentMother();
      var result = (GsaResultGoo)ComponentTestHelper.GetOutput(comp);

      Assert.Equal(CaseType.AnalysisCase, result.Value.Type);
      Assert.Equal(1, result.Value.CaseId);
      Assert.Equal(GH_RuntimeMessageLevel.Remark, comp.RuntimeMessageLevel);
      Assert.Equal("By default, Analysis Case 1 has been selected.",
        comp.RuntimeMessages(GH_RuntimeMessageLevel.Remark)[0]);
    }

    [Fact]
    public void TestAnalysisWithInputs() {
      var comp = new GetResult();
      comp.CreateAttributes();

      GsaModelGoo modelInput = ModelTests.GsaModelGooMother;

      ComponentTestHelper.SetInput(comp, modelInput, 0);
      ComponentTestHelper.SetInput(comp, "Analysis", 1);
      ComponentTestHelper.SetInput(comp, 2, 2);

      var result = (GsaResultGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(CaseType.AnalysisCase, result.Value.Type);
      Assert.Equal(2, result.Value.CaseId);
      Assert.Equal(GH_RuntimeMessageLevel.Blank, comp.RuntimeMessageLevel);
    }

    [Fact]
    public void TestCombinationNoPermutation() {
      var comp = new GetResult();
      comp.CreateAttributes();

      GsaModelGoo modelInput = ModelTests.GsaModelGooMother;

      ComponentTestHelper.SetInput(comp, modelInput, 0);
      ComponentTestHelper.SetInput(comp, "Combination", 1);
      ComponentTestHelper.SetInput(comp, 1, 2);

      var result = (GsaResultGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(CaseType.Combination, result.Value.Type);
      Assert.Equal(1, result.Value.CaseId);
      Assert.Equal(new List<int>() {
        1,
      }, result.Value.SelectedPermutationIds);
      Assert.Equal(GH_RuntimeMessageLevel.Remark, comp.RuntimeMessageLevel);
      Assert.Equal("By default, all permutations have been selected.",
        comp.RuntimeMessages(GH_RuntimeMessageLevel.Remark)[0]);
    }

    [Fact]
    public void TestCombinationWithPermutation() {
      var comp = new GetResult();
      comp.CreateAttributes();

      GsaModelGoo modelInput = ModelTests.GsaModelGooMother;

      ComponentTestHelper.SetInput(comp, modelInput, 0);
      ComponentTestHelper.SetInput(comp, "Combination", 1);
      ComponentTestHelper.SetInput(comp, 1, 2);
      ComponentTestHelper.SetInput(comp, 1, 3);

      var result = (GsaResultGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(CaseType.Combination, result.Value.Type);
      Assert.Equal(1, result.Value.CaseId);
      Assert.Equal(new List<int>() {
        1,
      }, result.Value.SelectedPermutationIds);
      Assert.Equal(GH_RuntimeMessageLevel.Blank, comp.RuntimeMessageLevel);
    }
  }
}
