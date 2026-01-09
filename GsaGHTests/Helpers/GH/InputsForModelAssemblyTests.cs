using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaAPI;

using GsaGH.Components;
using GsaGH.Parameters;
using GsaGH.Parameters.Results;

using GsaGHTests.Components.Geometry;
using GsaGHTests.Components.Loads;
using GsaGHTests.Helper;
using GsaGHTests.Model;
using GsaGHTests.Parameters;

using OasysGH.Components;

using Xunit;

using EntityType = GsaAPI.EntityType;
using NodeDisplacements = GsaGH.Components.NodeDisplacements;

namespace GsaGHTests.Helpers.GH {
  [Collection("GrasshopperFixture collection")]
  public class InputsForModelAssemblyTests {
    [Fact]
    public void GetAnalysisFromTaskTest() {
      var getModelAnalysis = new GetModelAnalysis();
      var model = new GsaModel();
      model.ApiModel.Open(GsaFile.SteelDesignSimple);
      ComponentTestHelper.SetInput(getModelAnalysis, new GsaModelGoo(model));
      var taskGoo = (GsaAnalysisTaskGoo)ComponentTestHelper.GetOutput(getModelAnalysis);
      var comp = new CreateModel();
      ComponentTestHelper.SetInput(comp, taskGoo, 4);
      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Remark));
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Warning));
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void GetAnalysisFromCombinationTest() {
      var getModelAnalysis = new GetModelAnalysis();
      var model = new GsaModel();
      model.ApiModel.Open(GsaFile.SteelDesignSimple);
      ComponentTestHelper.SetInput(getModelAnalysis, new GsaModelGoo(model));
      var combinationGoo = (GsaCombinationCaseGoo)ComponentTestHelper.GetOutput(getModelAnalysis, 2);
      var comp = new CreateModel();
      ComponentTestHelper.SetInput(comp, combinationGoo, 4);
      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Remark));
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Warning));
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void GetAnalysisFromAnalysisCaseErrorTest() {
      var getModelAnalysis = new GetModelAnalysis();
      var model = new GsaModel();
      model.ApiModel.Open(GsaFile.SteelDesignSimple);
      ComponentTestHelper.SetInput(getModelAnalysis, new GsaModelGoo(model));
      var analysisCaseGoo = (GsaAnalysisCaseGoo)ComponentTestHelper.GetOutput(getModelAnalysis, 1);
      var comp = new CreateModel();
      ComponentTestHelper.SetInput(comp, analysisCaseGoo, 4);
      comp.Params.Output[0].ExpireSolution(true);
      comp.Params.Output[0].CollectData();
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Remark));
      Assert.Single(comp.RuntimeMessages(GH_RuntimeMessageLevel.Warning));
      Assert.NotEmpty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void GetGeometryFromNodeTest() {
      var goo = (GsaNodeGoo)ComponentTestHelper.GetOutput(
        CreateSupportTests.ComponentMother());
      var comp = new CreateModel();
      ComponentTestHelper.SetInput(comp, goo, 2);
      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Remark));
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Warning));
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void GetGeometryFromElement1dTest() {
      var goo = (GsaElement1dGoo)ComponentTestHelper.GetOutput(
        CreateElement1dTests.ComponentMother());
      var comp = new CreateModel();
      ComponentTestHelper.SetInput(comp, goo, 2);
      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Remark));
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Warning));
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void GetGeometryFromMember1dTest() {
      var goo = (GsaMember1dGoo)ComponentTestHelper.GetOutput(
        CreateMember1dTests.ComponentMother());
      var comp = new CreateModel();
      ComponentTestHelper.SetInput(comp, goo, 2);
      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Remark));
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Warning));
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void GetGeometryFromElement2dTest() {
      var goo = (GsaElement2dGoo)ComponentTestHelper.GetOutput(
        CreateElement2dTests.ComponentMother());
      var comp = new CreateModel();
      ComponentTestHelper.SetInput(comp, goo, 2);
      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Remark));
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Warning));
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void GetGeometryFromMember2dTest() {
      var goo = (GsaMember2dGoo)ComponentTestHelper.GetOutput(
        CreateMember2dTests.ComponentMother());
      var comp = new CreateModel();
      ComponentTestHelper.SetInput(comp, goo, 2);
      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Remark));
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Warning));
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void GetGeometryFromElement3dTest() {
      var goo = (GsaElement3dGoo)ComponentTestHelper.GetOutput(
        CreateElementsFromMembersTests.ComponentMother(), 3);
      var comp = new CreateModel();
      ComponentTestHelper.SetInput(comp, goo, 2);
      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Remark));
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Warning));
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void GetGeometryFromMember3dTest() {
      var goo = (GsaMember3dGoo)ComponentTestHelper.GetOutput(
        CreateMember3dTests.ComponentMother());
      var comp = new CreateModel();
      ComponentTestHelper.SetInput(comp, goo, 2);
      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Remark));
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Warning));
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void GetGeometryErrorTest() {
      var comp = new CreateModel();
      var goo = new GH_Integer(1);
      ComponentTestHelper.SetInput(comp, goo, 2);
      comp.Params.Output[0].ExpireSolution(true);
      comp.Params.Output[0].CollectData();
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Remark));
      Assert.Single(comp.RuntimeMessages(GH_RuntimeMessageLevel.Warning));
      Assert.NotEmpty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void GetLoadFromLoadTest() {
      var beamLoad = new CreateBeamLoad();
      ComponentTestHelper.SetInput(beamLoad, "All", 1);
      ComponentTestHelper.SetInput(beamLoad, -5, 6);
      var goo = (GsaLoadGoo)ComponentTestHelper.GetOutput(beamLoad);
      var comp = new CreateModel();
      ComponentTestHelper.SetInput(comp, goo, 3);
      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Remark));
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Warning));
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void GetLoadingFromLoadCaseTest() {
      var goo = (GsaLoadCaseGoo)ComponentTestHelper.GetOutput(
        CreateLoadCaseTests.ComponentMother());
      var comp = new CreateModel();
      ComponentTestHelper.SetInput(comp, goo, 3);
      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Remark));
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Warning));
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void GetLoadingFromGridPlaneSurfaceTest() {
      var goo = (GsaGridPlaneSurfaceGoo)ComponentTestHelper.GetOutput(
        GridSurfaceTests.ComponentMother());
      var comp = new CreateModel();
      ComponentTestHelper.SetInput(comp, goo, 3);
      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Remark));
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Warning));
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void GetLoadingErrorTest() {
      var comp = new CreateModel();
      var goo = new GH_Integer(1);
      ComponentTestHelper.SetInput(comp, goo, 3);
      comp.Params.Output[0].ExpireSolution(true);
      comp.Params.Output[0].CollectData();
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Remark));
      Assert.Single(comp.RuntimeMessages(GH_RuntimeMessageLevel.Warning));
      Assert.NotEmpty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void GetModelFromModelTest() {
      var model = new GsaModel();
      model.ApiModel.Open(GsaFile.SteelDesignSimple);
      var goo = new GsaModelGoo(model);
      var comp = new CreateModel();
      ComponentTestHelper.SetInput(comp, goo, 0);
      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Remark));
      Assert.Single(comp.RuntimeMessages(GH_RuntimeMessageLevel.Warning));
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void GetModelFromListTest() {
      GH_OasysDropDownComponent createListComp = CreateListTest.ComponentMother();
      ComponentTestHelper.SetInput(createListComp, (double)4, 2);
      var goo = (GsaListGoo)ComponentTestHelper.GetOutput(createListComp);
      var comp = new CreateModel();
      ComponentTestHelper.SetInput(comp, goo, 0);
      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Remark));
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Warning));
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void GetModelFromGridlineTest() {
      var _helper = new CreateGridLineTestHelper();
      _helper.CreateComponentWithLineInput();
      GsaGridLineGoo goo = _helper.GetGridLineOutput();
      var comp = new CreateModel();
      ComponentTestHelper.SetInput(comp, goo, 0);
      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Remark));
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Warning));
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void GetModelErrorTest() {
      var comp = new CreateModel();
      var goo = new GH_Integer(1);
      ComponentTestHelper.SetInput(comp, goo, 0);
      comp.Params.Output[0].ExpireSolution(true);
      comp.Params.Output[0].CollectData();
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Remark));
      Assert.Single(comp.RuntimeMessages(GH_RuntimeMessageLevel.Warning));
      Assert.NotEmpty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void TestGetMemberListDefinitionFromMemberList() {
      GsaResult result = GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignSimple, 1);
      var apiList = new EntityList() {
        Type = EntityType.Member,
        Definition = "1",
        Name = "myList"
      };
      result.Model.ApiModel.AddList(apiList);

      var comp = new Member1dDisplacements();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      var list = new GsaList("myList", "1", EntityType.Member);
      ComponentTestHelper.SetInput(comp, new GsaListGoo(list), 1);
      comp.Params.Output[0].CollectData();

      Assert.True((int)comp.RuntimeMessageLevel < 10);
    }

    [Fact]
    public void TestGetMemberListDefinitionErrorFromElementList() {
      GsaResult result = GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignSimple, 1);
      var apiList = new EntityList() {
        Type = EntityType.Element,
        Definition = "1",
        Name = "myList"
      };
      result.Model.ApiModel.AddList(apiList);

      var comp = new Member1dDisplacements();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      var list = new GsaList("myList", "1", EntityType.Element);
      ComponentTestHelper.SetInput(comp, new GsaListGoo(list), 1);
      comp.Params.Output[0].CollectData();

      Assert.True((int)comp.RuntimeMessageLevel >= 10);
    }

    [Fact]
    public void TestGetMemberListDefinitionFromUnnamedMemberList() {
      GsaResult result = GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignSimple, 1);
      var apiList = new EntityList() {
        Type = EntityType.Member,
        Definition = "1",
        Name = string.Empty
      };
      result.Model.ApiModel.AddList(apiList);

      var comp = new Member1dDisplacements();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      var list = new GsaList(string.Empty, "1", EntityType.Member);
      ComponentTestHelper.SetInput(comp, new GsaListGoo(list), 1);
      comp.Params.Output[0].CollectData();

      Assert.True((int)comp.RuntimeMessageLevel < 10);
    }

    [Fact]
    public void TestGetMemberListDefinitionFromUnnamedMemberListNotInModel() {
      GsaResult result = GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignSimple, 1);
      var comp = new Member1dDisplacements();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      var list = new GsaList(string.Empty, "1", EntityType.Member);
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


    [Fact]
    public void TestGetNodeListDefinitionFromNodeList() {
      GsaResult result = GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignSimple, 1);
      var apiList = new EntityList() {
        Type = EntityType.Node,
        Definition = "2",
        Name = "myList"
      };
      result.Model.ApiModel.AddList(apiList);

      var comp = new NodeDisplacements();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      var list = new GsaList("myList", "2", EntityType.Node);
      ComponentTestHelper.SetInput(comp, new GsaListGoo(list), 1);
      comp.Params.Output[0].CollectData();

      Assert.True((int)comp.RuntimeMessageLevel < 10);
    }

    [Fact]
    public void TestGetNodeListDefinitionErrorFromElementList() {
      GsaResult result = GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignSimple, 1);
      var apiList = new EntityList() {
        Type = EntityType.Element,
        Definition = "2",
        Name = "myList"
      };
      result.Model.ApiModel.AddList(apiList);

      var comp = new NodeDisplacements();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      var list = new GsaList("myList", "2", EntityType.Element);
      ComponentTestHelper.SetInput(comp, new GsaListGoo(list), 1);
      comp.Params.Output[0].CollectData();

      Assert.True((int)comp.RuntimeMessageLevel >= 10);
    }

    [Fact]
    public void TestGetNodeListDefinitionFromUnnamedMemberList() {
      GsaResult result = GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignSimple, 1);
      var apiList = new EntityList() {
        Type = EntityType.Node,
        Definition = "1",
        Name = string.Empty
      };
      result.Model.ApiModel.AddList(apiList);

      var comp = new NodeDisplacements();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      var list = new GsaList(string.Empty, "1", EntityType.Node);
      ComponentTestHelper.SetInput(comp, new GsaListGoo(list), 1);
      comp.Params.Output[0].CollectData();

      Assert.True((int)comp.RuntimeMessageLevel < 10);
    }

    [Fact]
    public void TestGetNodeListDefinitionFromUnnamedMemberListNotInModel() {
      GsaResult result = GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignSimple, 1);
      var comp = new NodeDisplacements();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      var list = new GsaList(string.Empty, "1", EntityType.Node);
      ComponentTestHelper.SetInput(comp, new GsaListGoo(list), 1);
      comp.Params.Output[0].CollectData();

      Assert.True((int)comp.RuntimeMessageLevel < 10);
    }

    [Fact]
    public void TestGetNodeListDefinitionFromString() {
      GsaResult result = GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignSimple, 1);
      var comp = new NodeDisplacements();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, "all", 1);
      comp.Params.Output[0].CollectData();
      Assert.True((int)comp.RuntimeMessageLevel < 10);
    }

    [Fact]
    public void TestGetElementListDefinitionErrorFromNodeList() {
      GsaResult result = GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignSimple, 1);
      var comp = new BeamDisplacements();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      var list = new GsaList("myList", "2", EntityType.Node);
      ComponentTestHelper.SetInput(comp, new GsaListGoo(list), 1);
      comp.Params.Output[0].CollectData();

      Assert.True((int)comp.RuntimeMessageLevel >= 10);
    }

    [Fact]
    public void TestGetElementListDefinitionFromMemberChildList() {
      GsaResult result = GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignSimple, 1);
      string listName = "myList";
      var apiList = new EntityList() {
        Type = EntityType.Element,
        Definition = "1 2",
        Name = $"Children of '{listName}'"
      };
      result.Model.ApiModel.AddList(apiList);

      var comp = new BeamDisplacements();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      var list = new GsaList("myList", "1", EntityType.Member);
      ComponentTestHelper.SetInput(comp, new GsaListGoo(list), 1);
      comp.Params.Output[0].CollectData();

      Assert.Equal($"Element definition was derived from Children of '{listName}' List",
            comp.RuntimeMessages(GH_RuntimeMessageLevel.Remark)[0]);
    }

    [Fact]
    public void TestGetElementListDefinitionFromMemberList() {
      GsaResult result = GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignSimple, 1);
      string listName = "myList";
      var apiList = new EntityList() {
        Type = EntityType.Member,
        Definition = "1",
        Name = listName
      };
      result.Model.ApiModel.AddList(apiList);

      var comp = new BeamDisplacements();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      var list = new GsaList(listName, "1", EntityType.Member);
      ComponentTestHelper.SetInput(comp, new GsaListGoo(list), 1);
      comp.Params.Output[0].CollectData();

      Assert.Equal($"Element definition was derived from Elements with parent " +
            $"Members included in '{listName}' List",
            comp.RuntimeMessages(GH_RuntimeMessageLevel.Remark)[0]);
    }

    [Fact]
    public void TestGetElementListDefinitionErrorFromMemberListWithoutChildren() {
      GsaResult result = GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignSimple, 1);
      string listName = "myList";
      var apiList = new EntityList() {
        Type = EntityType.Member,
        Definition = "2",
        Name = listName
      };
      result.Model.ApiModel.AddList(apiList);
      result.Model.ApiModel.AddMember(new Member());

      var comp = new BeamDisplacements();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      var list = new GsaList(listName, "2", EntityType.Member);
      ComponentTestHelper.SetInput(comp, new GsaListGoo(list), 1);
      comp.Params.Output[0].CollectData();

      Assert.Equal($"No child elements found for Members 2",
            comp.RuntimeMessages(GH_RuntimeMessageLevel.Warning)[0]);
    }

    [Fact]
    public void TestGetElementListDefinitionErrorFromMemberListNotInModel() {
      GsaResult result = GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignSimple, 1);
      result.Model.ApiModel.AddMember(new Member());

      var comp = new BeamDisplacements();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      var list = new GsaList(string.Empty, "2", EntityType.Member);
      ComponentTestHelper.SetInput(comp, new GsaListGoo(list), 1);
      comp.Params.Output[0].CollectData();

      Assert.Equal($"No child elements found for Members 2",
            comp.RuntimeMessages(GH_RuntimeMessageLevel.Warning)[0]);
    }

    [Fact]
    public void TestGetElementListDefinitionFromMemberListNotInModel() {
      GsaResult result = GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignSimple, 1);
      result.Model.ApiModel.AddMember(new Member());

      var comp = new BeamDisplacements();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      var list = new GsaList(string.Empty, "1", EntityType.Member);
      ComponentTestHelper.SetInput(comp, new GsaListGoo(list), 1);
      comp.Params.Output[0].CollectData();

      Assert.Equal($"Element definition was derived from Elements with parent "
          + $"Members included in 'Member list' List",
            comp.RuntimeMessages(GH_RuntimeMessageLevel.Remark)[0]);
    }

    [Fact]
    public void TestGetElementOrMemberListFromString() {
      GsaResult result = GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignSimple, 1);
      result.Model.ApiModel.AddMember(new Member());

      var comp = new ResultDiagrams();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, "all", 1);
      comp.Params.Output[0].CollectData();

      Assert.True((int)comp.RuntimeMessageLevel < 10);
    }

    [Fact]
    public void TestGetElementOrMemberListFromElementList() {
      GsaResult result = GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignSimple, 1);
      result.Model.ApiModel.AddMember(new Member());

      var comp = new ResultDiagrams();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      var list = new GsaList(string.Empty, "1", EntityType.Element);
      ComponentTestHelper.SetInput(comp, new GsaListGoo(list), 1);
      comp.Params.Output[0].CollectData();

      Assert.True((int)comp.RuntimeMessageLevel < 10);
    }

    [Fact]
    public void TestGetElementOrMemberListFromMemberList() {
      GsaResult result = GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignSimple, 1);
      result.Model.ApiModel.AddMember(new Member());

      var comp = new ResultDiagrams();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      var list = new GsaList(string.Empty, "1", EntityType.Member);
      ComponentTestHelper.SetInput(comp, new GsaListGoo(list), 1);
      comp.Params.Output[0].CollectData();

      Assert.True((int)comp.RuntimeMessageLevel < 10);
    }

    [Fact]
    public void TestGetElementOrMemberListErrorFromNodeList() {
      GsaResult result = GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignSimple, 1);
      result.Model.ApiModel.AddMember(new Member());

      var comp = new ResultDiagrams();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      var list = new GsaList(string.Empty, "1", EntityType.Node);
      ComponentTestHelper.SetInput(comp, new GsaListGoo(list), 1);
      comp.Params.Output[0].CollectData();

      Assert.True((int)comp.RuntimeMessageLevel >= 10);
    }
  }
}
