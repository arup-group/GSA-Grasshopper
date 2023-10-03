using System;
using System.Collections.Generic;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Components;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGHTests.Components.Geometry;
using GsaGHTests.Components.Loads;
using GsaGHTests.Helper;
using GsaGHTests.Model;
using OasysGH.Components;
using Rhino.Collections;
using Rhino.Geometry;
using Xunit;

namespace GsaGHTests.Helpers.GH {
  [Collection("GrasshopperFixture collection")]
  public class InputsForModelAssemblyTests {
    [Fact]
    public void GetAnalysisFromTaskTest() {
      var getModelAnalysis = new GetModelAnalysis();
      var model = new GsaModel();
      model.Model.Open(GsaFile.SteelDesignSimple);
      ComponentTestHelper.SetInput(getModelAnalysis, new GsaModelGoo(model));
      var taskGoo = (GsaAnalysisTaskGoo)ComponentTestHelper.GetOutput(getModelAnalysis);
      var comp = new CreateModel();
      ComponentTestHelper.SetInput(comp, taskGoo, 4);
      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Remark));
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Warning));
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void GetAnalysisFromCombinationTest() {
      var getModelAnalysis = new GetModelAnalysis();
      var model = new GsaModel();
      model.Model.Open(GsaFile.SteelDesignSimple);
      ComponentTestHelper.SetInput(getModelAnalysis, new GsaModelGoo(model));
      var combinationGoo = (GsaCombinationCaseGoo)ComponentTestHelper.GetOutput(getModelAnalysis, 2);
      var comp = new CreateModel();
      ComponentTestHelper.SetInput(comp, combinationGoo, 4);
      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Remark));
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Warning));
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void GetAnalysisFromAnalysisCaseErrorTest() {
      var getModelAnalysis = new GetModelAnalysis();
      var model = new GsaModel();
      model.Model.Open(GsaFile.SteelDesignSimple);
      ComponentTestHelper.SetInput(getModelAnalysis, new GsaModelGoo(model));
      var analysisCaseGoo = (GsaAnalysisCaseGoo)ComponentTestHelper.GetOutput(getModelAnalysis, 1);
      var comp = new CreateModel();
      ComponentTestHelper.SetInput(comp, analysisCaseGoo, 4);
      comp.Params.Output[0].ExpireSolution(true);
      comp.Params.Output[0].CollectData();
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Remark));
      Assert.Single(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Warning));
      Assert.NotEmpty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void GetGeometryFromNodeTest() {
      var goo = (GsaNodeGoo)ComponentTestHelper.GetOutput(
        CreateSupportTests.ComponentMother());
      var comp = new CreateModel();
      ComponentTestHelper.SetInput(comp, goo, 2);
      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Remark));
      Assert.Single(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Warning));
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void GetGeometryFromElement1dTest() {
      var goo = (GsaElement1dGoo)ComponentTestHelper.GetOutput(
        CreateElement1dTests.ComponentMother());
      var comp = new CreateModel();
      ComponentTestHelper.SetInput(comp, goo, 2);
      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Remark));
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Warning));
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void GetGeometryFromMember1dTest() {
      var goo = (GsaMember1dGoo)ComponentTestHelper.GetOutput(
        CreateMember1dTests.ComponentMother());
      var comp = new CreateModel();
      ComponentTestHelper.SetInput(comp, goo, 2);
      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Remark));
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Warning));
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void GetGeometryFromElement2dTest() {
      var goo = (GsaElement2dGoo)ComponentTestHelper.GetOutput(
        CreateElement2dTests.ComponentMother());
      var comp = new CreateModel();
      ComponentTestHelper.SetInput(comp, goo, 2);
      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Remark));
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Warning));
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void GetGeometryFromMember2dTest() {
      var goo = (GsaMember2dGoo)ComponentTestHelper.GetOutput(
        CreateMember2dTests.ComponentMother());
      var comp = new CreateModel();
      ComponentTestHelper.SetInput(comp, goo, 2);
      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Remark));
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Warning));
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void GetGeometryFromElement3dTest() {
      var goo = (GsaElement3dGoo)ComponentTestHelper.GetOutput(
        CreateElementsFromMembersTests.ComponentMother(), 3);
      var comp = new CreateModel();
      ComponentTestHelper.SetInput(comp, goo, 2);
      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Remark));
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Warning));
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void GetGeometryFromMember3dTest() {
      var goo = (GsaMember3dGoo)ComponentTestHelper.GetOutput(
        CreateMember3dTests.ComponentMother()); 
      var comp = new CreateModel();
      ComponentTestHelper.SetInput(comp, goo, 2);
      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Remark));
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Warning));
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void GetGeometryErrorTest() {
      var comp = new CreateModel();
      var goo = new GH_Integer(1);
      ComponentTestHelper.SetInput(comp, goo, 2);
      comp.Params.Output[0].ExpireSolution(true);
      comp.Params.Output[0].CollectData();
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Remark));
      Assert.Single(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Warning));
      Assert.NotEmpty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error));
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
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Remark));
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Warning));
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void GetLoadingFromLoadCaseTest() {
      var goo = (GsaLoadCaseGoo)ComponentTestHelper.GetOutput(
        CreateLoadCaseTests.ComponentMother());
      var comp = new CreateModel();
      ComponentTestHelper.SetInput(comp, goo, 3);
      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Remark));
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Warning));
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void GetLoadingFromGridPlaneSurfaceTest() {
      var goo = (GsaGridPlaneSurfaceGoo)ComponentTestHelper.GetOutput(
        GridSurfaceTests.ComponentMother());
      var comp = new CreateModel();
      ComponentTestHelper.SetInput(comp, goo, 3);
      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Remark));
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Warning));
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void GetLoadingErrorTest() {
      var comp = new CreateModel();
      var goo = new GH_Integer(1);
      ComponentTestHelper.SetInput(comp, goo, 3);
      comp.Params.Output[0].ExpireSolution(true);
      comp.Params.Output[0].CollectData();
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Remark));
      Assert.Single(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Warning));
      Assert.NotEmpty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void GetModelFromModelTest() {
      var model = new GsaModel();
      model.Model.Open(GsaFile.SteelDesignSimple);
      var goo = new GsaModelGoo(model);
      var comp = new CreateModel();
      ComponentTestHelper.SetInput(comp, goo, 0);
      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Remark));
      Assert.Single(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Warning));
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void GetModelFromListTest() {
      GH_OasysDropDownComponent createListComp = CreateListTest.ComponentMother();
      ComponentTestHelper.SetInput(createListComp, (double)4, 2);
      var goo = (GsaListGoo)ComponentTestHelper.GetOutput(createListComp);
      var comp = new CreateModel();
      ComponentTestHelper.SetInput(comp, goo, 0);
      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Remark));
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Warning));
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void GetModelFromGridlineTest() {
      var goo = (GsaGridLineGoo)ComponentTestHelper.GetOutput(
        CreateGridLineTest.GridLineComponentMother());
      var comp = new CreateModel();
      ComponentTestHelper.SetInput(comp, goo, 0);
      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Remark));
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Warning));
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void GetModelErrorTest() {
      var comp = new CreateModel();
      var goo = new GH_Integer(1);
      ComponentTestHelper.SetInput(comp, goo, 0);
      comp.Params.Output[0].ExpireSolution(true);
      comp.Params.Output[0].CollectData();
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Remark));
      Assert.Single(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Warning));
      Assert.NotEmpty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error));
    }
  }
}
