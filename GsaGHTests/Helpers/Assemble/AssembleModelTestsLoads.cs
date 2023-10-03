using System.Collections.Generic;
using GsaGH.Parameters;
using GsaGHTests.Helper;
using GsaGHTests.Model;
using Xunit;
using LoadCaseType = GsaGH.Parameters.LoadCaseType;

namespace GsaGHTests.Helpers.Export {
  public partial class AssembleModelTests {

    [Fact]
    public void AssembleModelFromGravityLoadTest() {
      var load = new GsaGravityLoad();

      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromLoads(
          new List<IGsaLoad>() { 
            load }, 
          null));

      Assert.Empty(modelGoo.Value.Model.LoadCases());
      Assert.Single(modelGoo.Value.Model.GravityLoads());
      Assert.Equal(1, modelGoo.Value.Model.GravityLoads()[0].Case);
    }

    [Fact]
    public void AssembleModelFromBeamLoadTest() {
      var load = new GsaBeamLoad();

      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromLoads(
          new List<IGsaLoad>() {
            load },
          null));

      Assert.Empty(modelGoo.Value.Model.LoadCases());
      Assert.Single(modelGoo.Value.Model.BeamLoads());
      Assert.Equal(1, modelGoo.Value.Model.BeamLoads()[0].Case);
    }

    [Fact]
    public void AssembleModelFromBeamThermalLoadTest() {
      var load = new GsaBeamThermalLoad();

      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromLoads(
          new List<IGsaLoad>() {
            load },
          null));

      Assert.Empty(modelGoo.Value.Model.LoadCases());
      Assert.Single(modelGoo.Value.Model.BeamThermalLoads());
      Assert.Equal(1, modelGoo.Value.Model.BeamThermalLoads()[0].Case);
    }

    [Fact]
    public void AssembleModelFromFaceLoadTest() {
      var load = new GsaFaceLoad();

      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromLoads(
          new List<IGsaLoad>() {
            load },
          null));

      Assert.Empty(modelGoo.Value.Model.LoadCases());
      Assert.Single(modelGoo.Value.Model.FaceLoads());
      Assert.Equal(1, modelGoo.Value.Model.FaceLoads()[0].Case);
    }

    [Fact]
    public void AssembleModelFromFaceThermalLoadTest() {
      var load = new GsaFaceThermalLoad();

      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromLoads(
          new List<IGsaLoad>() {
            load },
          null));

      Assert.Empty(modelGoo.Value.Model.LoadCases());
      Assert.Single(modelGoo.Value.Model.FaceThermalLoads());
      Assert.Equal(1, modelGoo.Value.Model.FaceThermalLoads()[0].Case);
    }

    [Fact]
    public void AssembleModelFromNodeLoadTest() {
      var load = new GsaNodeLoad();

      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromLoads(
          new List<IGsaLoad>() {
            load },
          null));

      Assert.Empty(modelGoo.Value.Model.LoadCases());
      Assert.Single(modelGoo.Value.Model.NodeLoads(GsaAPI.NodeLoadType.NODE_LOAD));
      Assert.Equal(1, modelGoo.Value.Model.NodeLoads(GsaAPI.NodeLoadType.NODE_LOAD)[0].Case);
    }

    [Fact]
    public void AssembleModelFromGravityWithLoadCaseRefTest() {
      var load = new GsaGravityLoad {
        LoadCase = new GsaLoadCase(4)
      };

      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromLoads(
          new List<IGsaLoad>() {
            load },
          null));

      Assert.Empty(modelGoo.Value.Model.LoadCases());
      Assert.Single(modelGoo.Value.Model.GravityLoads());
      Assert.Equal(4, modelGoo.Value.Model.GravityLoads()[0].Case);
    }

    [Fact]
    public void AssembleModelFromNodeWithLoadCaseRefTest() {
      var load = new GsaNodeLoad {
        LoadCase = new GsaLoadCase(4)
      };

      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromLoads(
          new List<IGsaLoad>() {
            load },
          null));

      Assert.Empty(modelGoo.Value.Model.LoadCases());
      Assert.Single(modelGoo.Value.Model.NodeLoads(GsaAPI.NodeLoadType.NODE_LOAD));
      Assert.Equal(4, modelGoo.Value.Model.NodeLoads(GsaAPI.NodeLoadType.NODE_LOAD)[0].Case);
    }

    [Fact]
    public void AssembleModelFromGravityWithLoadCaseTest() {
      var load = new GsaGravityLoad {
        LoadCase = new GsaLoadCase(4, LoadCaseType.Dead, "dead")
      };

      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromLoads(
          new List<IGsaLoad>() {
            load },
          null));

      Assert.Single(modelGoo.Value.Model.LoadCases());
      Assert.NotNull(modelGoo.Value.Model.LoadCases()[4]);
      Assert.Equal(GsaAPI.LoadCaseType.Dead, modelGoo.Value.Model.LoadCases()[4].CaseType);
      Assert.Equal("dead", modelGoo.Value.Model.LoadCases()[4].Name);
      Assert.Single(modelGoo.Value.Model.GravityLoads());
      Assert.Equal(4, modelGoo.Value.Model.GravityLoads()[0].Case);
    }

    [Fact]
    public void AssembleModelFromNodeWithLoadCaseTest() {
      var load = new GsaNodeLoad {
        LoadCase = new GsaLoadCase(4, LoadCaseType.Dead, "dead")
      };

      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromLoads(
          new List<IGsaLoad>() {
            load },
          null));

      Assert.Single(modelGoo.Value.Model.LoadCases());
      Assert.NotNull(modelGoo.Value.Model.LoadCases()[4]);
      Assert.Equal(GsaAPI.LoadCaseType.Dead, modelGoo.Value.Model.LoadCases()[4].CaseType);
      Assert.Equal("dead", modelGoo.Value.Model.LoadCases()[4].Name);
      Assert.Single(modelGoo.Value.Model.NodeLoads(GsaAPI.NodeLoadType.NODE_LOAD));
      Assert.Equal(4, modelGoo.Value.Model.NodeLoads(GsaAPI.NodeLoadType.NODE_LOAD)[0].Case);
    }

    [Fact]
    public void AppendModelFromGravityWithExistingLoadCaseTest() {
      var load = new GsaGravityLoad {
        LoadCase = new GsaLoadCase(1, LoadCaseType.Dead, "DL")
      };

      var loadModelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromLoads(
          new List<IGsaLoad>() {
            load },
          null));

      var steelExample = new GsaModel();
      steelExample.Model.Open(GsaFile.SteelDesignSimple);

      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromModels(new List<GsaModelGoo>() {
          new GsaModelGoo(steelExample),
          loadModelGoo,
        }));


      Assert.Equal(2, modelGoo.Value.Model.LoadCases().Count);
      Assert.Equal(GsaAPI.LoadCaseType.Dead, modelGoo.Value.Model.LoadCases()[1].CaseType);
      Assert.Equal("DL", modelGoo.Value.Model.LoadCases()[1].Name);
      Assert.Single(modelGoo.Value.Model.GravityLoads());
      Assert.Equal(1, modelGoo.Value.Model.GravityLoads()[0].Case);
    }

    [Fact]
    public void AppendModelFromGravityWithNewLoadCaseTest() {
      var load = new GsaGravityLoad {
        LoadCase = new GsaLoadCase(1, LoadCaseType.Dead, "dead")
      };

      var loadModelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromLoads(
          new List<IGsaLoad>() {
            load },
          null));

      var steelExample = new GsaModel();
      steelExample.Model.Open(GsaFile.SteelDesignSimple);

      OasysGH.Components.GH_OasysDropDownComponent assemblyComponent = CreateModelTest.CreateModelFromModels(new List<GsaModelGoo>() {
          new GsaModelGoo(steelExample),
          loadModelGoo,
        });
      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(assemblyComponent);
      Assert.Single(assemblyComponent.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Remark));

      Assert.Equal(2, modelGoo.Value.Model.LoadCases().Count);
      Assert.Equal(GsaAPI.LoadCaseType.Dead, modelGoo.Value.Model.LoadCases()[1].CaseType);
      Assert.Equal("dead", modelGoo.Value.Model.LoadCases()[1].Name);
      Assert.Single(modelGoo.Value.Model.GravityLoads());
      Assert.Equal(1, modelGoo.Value.Model.GravityLoads()[0].Case);
    }
  }
}
