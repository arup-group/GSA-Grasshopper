using System.Collections.Generic;
using Grasshopper.Kernel;
using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Xunit;

namespace GsaGHTests.Analysis
{
  [Collection("GrasshopperFixture collection")]
  public class GetResultsTest
  {
    [Fact]
    public void TestAnalysisNoInputs()
    {
      var comp = new GetResult();
      comp.CreateAttributes();

      GsaModelGoo modelInput = Model.ModelTests.GsaModelGooMother;

      ComponentTestHelper.SetInput(comp, modelInput, 0);

      GsaResultGoo result = (GsaResultGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(GsaResult.ResultType.AnalysisCase, result.Value.Type);
      Assert.Equal(1, result.Value.CaseID);
      Assert.Equal(GH_RuntimeMessageLevel.Remark, comp.RuntimeMessageLevel);
      Assert.Equal("By default, Analysis Case 1 has been selected.", comp.RuntimeMessages(GH_RuntimeMessageLevel.Remark)[0]);
    }

    [Fact]
    public void TestAnalysisWithInputs()
    {
      var comp = new GetResult();
      comp.CreateAttributes();

      GsaModelGoo modelInput = Model.ModelTests.GsaModelGooMother;

      ComponentTestHelper.SetInput(comp, modelInput, 0);
      ComponentTestHelper.SetInput(comp, "Analysis", 1);
      ComponentTestHelper.SetInput(comp, 2, 2);

      GsaResultGoo result = (GsaResultGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(GsaResult.ResultType.AnalysisCase, result.Value.Type);
      Assert.Equal(2, result.Value.CaseID);
      Assert.Equal(GH_RuntimeMessageLevel.Blank, comp.RuntimeMessageLevel);
    }

    [Fact]
    public void TestCombinationNoPermutation()
    {
      var comp = new GetResult();
      comp.CreateAttributes();

      GsaModelGoo modelInput = Model.ModelTests.GsaModelGooMother;

      ComponentTestHelper.SetInput(comp, modelInput, 0);
      ComponentTestHelper.SetInput(comp, "Combination", 1);
      ComponentTestHelper.SetInput(comp, 1, 2);

      GsaResultGoo result = (GsaResultGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(GsaResult.ResultType.Combination, result.Value.Type);
      Assert.Equal(1, result.Value.CaseID);
      Assert.Equal(new List<int>() { 1 }, result.Value.SelectedPermutationIDs);
      Assert.Equal(GH_RuntimeMessageLevel.Remark, comp.RuntimeMessageLevel);
      Assert.Equal("By default, all permutations have been selected.", comp.RuntimeMessages(GH_RuntimeMessageLevel.Remark)[0]);
    }

    [Fact]
    public void TestCombinationWithPermutation()
    {
      var comp = new GetResult();
      comp.CreateAttributes();

      GsaModelGoo modelInput = Model.ModelTests.GsaModelGooMother;

      ComponentTestHelper.SetInput(comp, modelInput, 0);
      ComponentTestHelper.SetInput(comp, "Combination", 1);
      ComponentTestHelper.SetInput(comp, 1, 2);
      ComponentTestHelper.SetInput(comp, 1, 3);

      GsaResultGoo result = (GsaResultGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(GsaResult.ResultType.Combination, result.Value.Type);
      Assert.Equal(1, result.Value.CaseID);
      Assert.Equal(new List<int>() { 1 }, result.Value.SelectedPermutationIDs);
      Assert.Equal(GH_RuntimeMessageLevel.Blank, comp.RuntimeMessageLevel);
    }
  }
}
