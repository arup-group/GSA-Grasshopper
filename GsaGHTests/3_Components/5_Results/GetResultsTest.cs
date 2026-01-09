using System.Collections.Generic;

using Grasshopper.Kernel;

using GsaAPI;

using GsaGH.Components;
using GsaGH.Parameters;
using GsaGH.Parameters.Results;

using GsaGHTests.Helper;
using GsaGHTests.Helpers;
using GsaGHTests.Model;
using GsaGHTests.Parameters;

using OasysGH.Components;

using Xunit;

namespace GsaGHTests.Components.Results {
  [Collection("GrasshopperFixture collection")]
  public class GetResultsTest {
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

      Assert.Equal(CaseType.AnalysisCase, result.Value.CaseType);
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
      Assert.Equal(CaseType.AnalysisCase, result.Value.CaseType);
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
      Assert.Equal(CaseType.CombinationCase, result.Value.CaseType);
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
      Assert.Equal(CaseType.CombinationCase, result.Value.CaseType);
      Assert.Equal(1, result.Value.CaseId);
      Assert.Equal(new List<int>() {
        1,
      }, result.Value.SelectedPermutationIds);
      Assert.Equal(GH_RuntimeMessageLevel.Blank, comp.RuntimeMessageLevel);
    }

    [Fact]
    public void TestGetMemberListDefinitionFromMemberList() {
      GsaResult result = GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignSimple, 1);
      var apiList = new EntityList() {
        Type = GsaAPI.EntityType.Member,
        Definition = "1",
        Name = "myList"
      };
      result.Model.ApiModel.AddList(apiList);

      var comp = new Member1dDisplacements();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      var list = new GsaList("myList", "1", GsaAPI.EntityType.Member);
      ComponentTestHelper.SetInput(comp, new GsaListGoo(list), 1);
      comp.Params.Output[0].CollectData();

      Assert.True((int)comp.RuntimeMessageLevel < 10);
    }

    [Fact]
    public void TestGetMemberListDefinitionErrorFromElementList() {
      GsaResult result = GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignSimple, 1);
      var apiList = new EntityList() {
        Type = GsaAPI.EntityType.Element,
        Definition = "1",
        Name = "myList"
      };
      result.Model.ApiModel.AddList(apiList);

      var comp = new Member1dDisplacements();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      var list = new GsaList("myList", "1", GsaAPI.EntityType.Element);
      ComponentTestHelper.SetInput(comp, new GsaListGoo(list), 1);
      comp.Params.Output[0].CollectData();

      Assert.True((int)comp.RuntimeMessageLevel >= 10);
    }

    [Fact]
    public void TestGetMemberListDefinitionFromUnnamedMemberList() {
      GsaResult result = GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignSimple, 1);
      var apiList = new EntityList() {
        Type = GsaAPI.EntityType.Member,
        Definition = "1",
        Name = string.Empty
      };
      result.Model.ApiModel.AddList(apiList);

      var comp = new Member1dDisplacements();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      var list = new GsaList(string.Empty, "1", GsaAPI.EntityType.Member);
      ComponentTestHelper.SetInput(comp, new GsaListGoo(list), 1);
      comp.Params.Output[0].CollectData();

      Assert.True((int)comp.RuntimeMessageLevel < 10);
    }

    [Fact]
    public void TestGetMemberListDefinitionFromUnnamedMemberListNotInModel() {
      GsaResult result = GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignSimple, 1);
      var comp = new Member1dDisplacements();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      var list = new GsaList(string.Empty, "1", GsaAPI.EntityType.Member);
      ComponentTestHelper.SetInput(comp, new GsaListGoo(list), 1);
      comp.Params.Output[0].CollectData();

      Assert.True((int)comp.RuntimeMessageLevel < 10);
    }

    [Fact]
    public void TestGetMemberListDefinitionFromString() {
      GsaResult result = GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignSimple, 1);
      var comp = new Member1dDisplacements();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, "all", 1);
      comp.Params.Output[0].CollectData();
      Assert.True((int)comp.RuntimeMessageLevel < 10);
    }
  }
}
