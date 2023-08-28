using System;
using Grasshopper.Kernel;
using GsaGH;
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
    [InlineData(typeof(GetModelAnalysis))]
    //[InlineData(typeof(GetGeometry))]
    [InlineData(typeof(GetModelLists))]
    [InlineData(typeof(GetModelLoads))]
    [InlineData(typeof(GetModelProperties))]
    [InlineData(typeof(GwaCommand))]
    [InlineData(typeof(ListInfo))]
    [InlineData(typeof(OpenModel))]
    [InlineData(typeof(CreateGridLine))]
    [InlineData(typeof(GetModelGridLines))]
    [InlineData(typeof(GridLineInfo))]
    [InlineData(typeof(SaveGsaModel))]
    [InlineData(typeof(ModelTitles))]
    [InlineData(typeof(CreateBool6))]
    [InlineData(typeof(CreateBucklingFactors))]
    [InlineData(typeof(CreateCustomMaterial))]
    [InlineData(typeof(CreateMaterial))]
    [InlineData(typeof(CreateOffset))]
    [InlineData(typeof(CreateProfile))]
    [InlineData(typeof(Create2dProperty))]
    [InlineData(typeof(Create3dProperty))]
    [InlineData(typeof(CreateSection))]
    [InlineData(typeof(CreateSectionModifier))]
    [InlineData(typeof(EditBool6))]
    [InlineData(typeof(EditBucklingFactors))]
    [InlineData(typeof(EditMaterial))]
    [InlineData(typeof(EditOffset))]
    [InlineData(typeof(EditProfile))]
    [InlineData(typeof(Edit2dProperty))]
    [InlineData(typeof(GetProperty2dModifier))]
    [InlineData(typeof(Edit3dProperty))]
    [InlineData(typeof(EditSection))]
    [InlineData(typeof(GetSectionModifier))]
    [InlineData(typeof(MaterialProperties))]
    [InlineData(typeof(ProfileDimensions))]
    [InlineData(typeof(SectionProperties))]
    [InlineData(typeof(TaperProfile))]
    // 2_Geometry
    [InlineData(typeof(Create1dElement))]
    [InlineData(typeof(Create2dElement))]
    [InlineData(typeof(Create1dMember))]
    [InlineData(typeof(Create2dMember))]
    [InlineData(typeof(Create3dMember))]
    [InlineData(typeof(CreateSupport))]
    [InlineData(typeof(Edit1dElement))]
    [InlineData(typeof(Edit2dElement))]
    [InlineData(typeof(Edit3dElement))]
    [InlineData(typeof(Edit1dMember))]
    [InlineData(typeof(Edit2dMember))]
    [InlineData(typeof(Edit3dMember))]
    [InlineData(typeof(EditNode))]
    [InlineData(typeof(Create2dElementsFromBrep))]
    [InlineData(typeof(CreateElementsFromMembers))]
    [InlineData(typeof(LocalAxes))]
    [InlineData(typeof(SectionAlignment))]
    [InlineData(typeof(Annotate))]
    // 3_Loads
    [InlineData(typeof(CreateBeamLoad))]
    [InlineData(typeof(CreateBeamThermalLoad))]
    [InlineData(typeof(CreateFaceLoad))]
    [InlineData(typeof(CreateFaceThermalLoad))]
    [InlineData(typeof(CreateGravityLoad))]
    [InlineData(typeof(CreateGridAreaLoad))]
    [InlineData(typeof(CreateGridLineLoad))]
    [InlineData(typeof(CreateGridPointLoad))]
    [InlineData(typeof(CreateGridPlane))]
    [InlineData(typeof(CreateGridSurface))]
    [InlineData(typeof(CreateNodeLoad))]
    [InlineData(typeof(GridPlaneSurfaceProperties))]
    [InlineData(typeof(LoadProperties))]
    [InlineData(typeof(LoadDiagrams))]
    // 4_Analysis
    [InlineData(typeof(AnalyseModel))]
    [InlineData(typeof(AnalysisCaseInfo))]
    [InlineData(typeof(CreateAnalysisCase))]
    [InlineData(typeof(CreateAnalysisTask))]
    [InlineData(typeof(CreateCombinationCase))]
    [InlineData(typeof(EditAnalysisTask))]
    // 5_Results
    [InlineData(typeof(BeamDisplacements))]
    [InlineData(typeof(BeamForcesAndMoments))]
    [InlineData(typeof(BeamStrainEnergyDensity))]
    [InlineData(typeof(Contour1dResults))]
    [InlineData(typeof(Contour2dResults))]
    [InlineData(typeof(Element2dDisplacements))]
    [InlineData(typeof(Element2dForcesAndMoments))]
    [InlineData(typeof(Element2dStresses))]
    [InlineData(typeof(Contour3dResults))]
    [InlineData(typeof(Element3dDisplacements))]
    [InlineData(typeof(Element3dStresses))]
    [InlineData(typeof(FootfallResults))]
    [InlineData(typeof(LineResultInfo))]
    [InlineData(typeof(MeshResultInfo))]
    [InlineData(typeof(PointResultInfo))]
    [InlineData(typeof(GetResult))]
    [InlineData(typeof(GlobalPerformanceResults))]
    [InlineData(typeof(ContourNodeResults))]
    [InlineData(typeof(NodeDisplacements))]
    [InlineData(typeof(ReactionForces))]
    [InlineData(typeof(ReactionForceDiagrams))]
    [InlineData(typeof(GetResultCases))]
    [InlineData(typeof(SelectResult))]
    [InlineData(typeof(SpringReactionForces))]
    [InlineData(typeof(TotalLoadsAndReactions))]
    public void GH_OasysComponentTest(Type t) {
      var comp = (GH_OasysComponent)Activator.CreateInstance(t);
      Assert.NotNull(comp.Icon_24x24);
      Assert.NotEqual(GH_Exposure.hidden, comp.Exposure);
      Assert.NotEqual(new Guid(), comp.ComponentGuid);
      Assert.Equal(PluginInfo.Instance, comp.PluginInfo);
    }

    [Fact]
    public void GH_OasysTaskCapableComponent() {
      var comp = new GetModelGeometry();
      Assert.NotNull(comp.Icon_24x24);
      Assert.NotEqual(GH_Exposure.hidden, comp.Exposure);
      Assert.NotEqual(new Guid(), comp.ComponentGuid);
      Assert.Equal(PluginInfo.Instance, comp.PluginInfo);
    }
  }
}
