using System;
using GsaGH.Components;
using OasysGH.Components;
using Xunit;

namespace GsaGHTests.Components {
  [Collection("GrasshopperFixture collection")]
  public class GH_OasysComponentTests {

    [Theory]
    // 0_Model
    [InlineData(typeof(CreateList))]
    [InlineData(typeof(CreateModel))]
    [InlineData(typeof(GetAnalysis))]
    //[InlineData(typeof(GetGeometry))]
    [InlineData(typeof(GetLists))]
    [InlineData(typeof(GetLoads))]
    [InlineData(typeof(GetProperties))]
    [InlineData(typeof(GwaCommand))]
    [InlineData(typeof(ListInfo))]
    [InlineData(typeof(OpenModel))]
    [InlineData(typeof(SaveModel))]
    [InlineData(typeof(Titles))]
    // 1_Properties
    [InlineData(typeof(CreateBool6))]
    [InlineData(typeof(CreateBucklingFactors))]
    [InlineData(typeof(CreateCustomMaterial))]
    [InlineData(typeof(CreateMaterial))]
    [InlineData(typeof(CreateOffset))]
    [InlineData(typeof(CreateProfile))]
    [InlineData(typeof(CreateProp2d))]
    [InlineData(typeof(CreateProp3d))]
    [InlineData(typeof(CreateSection))]
    [InlineData(typeof(CreateSectionModifier))]
    [InlineData(typeof(EditBool6))]
    [InlineData(typeof(EditBucklingFactors))]
    [InlineData(typeof(EditMaterial))]
    [InlineData(typeof(EditOffset))]
    [InlineData(typeof(EditProfile))]
    [InlineData(typeof(EditProp2d))]
    [InlineData(typeof(GetProp2dModifier))]
    [InlineData(typeof(EditProp3d))]
    [InlineData(typeof(EditSection))]
    [InlineData(typeof(GetSectionModifier))]
    [InlineData(typeof(GetMaterialProperties))]
    [InlineData(typeof(GetSectionDimensions))]
    [InlineData(typeof(GetSectionProperties))]
    [InlineData(typeof(TaperProfile))]
    // 2_Geometry
    [InlineData(typeof(CreateElement1d))]
    [InlineData(typeof(CreateElement2d))]
    [InlineData(typeof(CreateMember1d))]
    [InlineData(typeof(CreateMember2d))]
    [InlineData(typeof(CreateMember3d))]
    [InlineData(typeof(CreateSupport))]
    [InlineData(typeof(EditElement1d))]
    [InlineData(typeof(EditElement2d))]
    [InlineData(typeof(EditElement3d))]
    [InlineData(typeof(EditMember1d))]
    [InlineData(typeof(EditMember2d))]
    [InlineData(typeof(EditMember3d))]
    [InlineData(typeof(EditNode))]
    [InlineData(typeof(Element2dFromBrep))]
    [InlineData(typeof(ElementFromMembers))]
    [InlineData(typeof(LocalAxes))]
    [InlineData(typeof(SectionAlignment))]
    [InlineData(typeof(ShowId))]
    // 3_Loads
    [InlineData(typeof(CreateBeamLoads))]
    [InlineData(typeof(CreateFaceLoads))]
    [InlineData(typeof(CreateGravityLoad))]
    [InlineData(typeof(CreateGridAreaLoad))]
    [InlineData(typeof(CreateGridLineLoad))]
    [InlineData(typeof(CreateGridPointLoad))]
    [InlineData(typeof(CreateGridPlane))]
    [InlineData(typeof(CreateGridSurface))]
    [InlineData(typeof(CreateNodeLoad))]
    [InlineData(typeof(GridPlaneSurfaceProperties))]
    [InlineData(typeof(LoadProperties))]
    // 4_Analysis
    [InlineData(typeof(GhAnalyse))]
    [InlineData(typeof(AnalysisCaseInfo))]
    [InlineData(typeof(CreateAnalysisCase))]
    [InlineData(typeof(CreateAnalysisTask))]
    [InlineData(typeof(CreateCombinationCase))]
    [InlineData(typeof(EditAnalysisTask))]
    // 5_Results
    [InlineData(typeof(BeamDisplacement))]
    [InlineData(typeof(BeamForces))]
    [InlineData(typeof(BeamStrainEnergy))]
    [InlineData(typeof(Elem1dContourResults))]
    [InlineData(typeof(Elem2dContourResults))]
    [InlineData(typeof(Elem2dDisplacement))]
    [InlineData(typeof(Elem2dForces))]
    [InlineData(typeof(Elem2dStress))]
    [InlineData(typeof(Elem3dContourResults))]
    [InlineData(typeof(Elem3dDisplacement))]
    [InlineData(typeof(Elem3dStress))]
    [InlineData(typeof(FootfallResults))]
    [InlineData(typeof(GetLineResultsInfo))]
    [InlineData(typeof(GetMeshResultsInfo))]
    [InlineData(typeof(GetPointResultsInfo))]
    [InlineData(typeof(GetResult))]
    [InlineData(typeof(GlobalPerformanceResults))]
    [InlineData(typeof(NodeContourResults))]
    [InlineData(typeof(NodeDisplacement))]
    [InlineData(typeof(ReactionForce))]
    [InlineData(typeof(ReactionForceDiagrams))]
    [InlineData(typeof(ResultsInfo))]
    [InlineData(typeof(SelectResult))]
    [InlineData(typeof(SpringReactionForce))]
    [InlineData(typeof(TotalLoadsAndReactionResults))]
    public void GH_OasysComponentTest(Type t) {
      var comp = (GH_OasysComponent)Activator.CreateInstance(t);
      Assert.NotNull(comp.Icon_24x24);
      Assert.NotEqual(Grasshopper.Kernel.GH_Exposure.hidden, comp.Exposure);
      Assert.NotEqual(new Guid(), comp.ComponentGuid);
      Assert.Equal(GsaGH.PluginInfo.Instance, comp.PluginInfo);
    }

    [Fact]
    public void GH_OasysTaskCapableComponent() {
      var comp = new GetGeometry();
      Assert.NotNull(comp.Icon_24x24);
      Assert.NotEqual(Grasshopper.Kernel.GH_Exposure.hidden, comp.Exposure);
      Assert.NotEqual(new Guid(), comp.ComponentGuid);
      Assert.Equal(GsaGH.PluginInfo.Instance, comp.PluginInfo);
    }
  }
}
