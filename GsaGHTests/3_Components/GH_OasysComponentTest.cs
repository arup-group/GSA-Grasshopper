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
    [InlineData(typeof(PropertyQuantities))]
    [InlineData(typeof(MaterialQuantities))]
    // 1_Properties
    [InlineData(typeof(CreateBool6))]
    [InlineData(typeof(CreateCustomMaterial))]
    [InlineData(typeof(CreateMaterial))]
    [InlineData(typeof(CreateOffset))]
    [InlineData(typeof(CreateProfile))]
    [InlineData(typeof(Create2dProperty))]
    [InlineData(typeof(Create3dProperty))]
    [InlineData(typeof(CreateSection))]
    [InlineData(typeof(CreateSectionModifier))]
    [InlineData(typeof(CreateSpringProperty))]
    [InlineData(typeof(EditBool6))]
    [InlineData(typeof(EditMaterial))]
    [InlineData(typeof(EditOffset))]
    [InlineData(typeof(EditProfile))]
    [InlineData(typeof(Edit2dProperty))]
    [InlineData(typeof(Get2dPropertyModifier))]
    [InlineData(typeof(Edit3dProperty))]
    [InlineData(typeof(EditSection))]
    [InlineData(typeof(GetSectionModifier))]
    [InlineData(typeof(MaterialProperties))]
    [InlineData(typeof(ProfileDimensions))]
    [InlineData(typeof(SectionProperties))]
    [InlineData(typeof(TaperProfile))]
    [InlineData(typeof(GetSpringProperty))]
    // 2_Geometry
    [InlineData(typeof(Create1dElement))]
    [InlineData(typeof(Create2dElement))]
    [InlineData(typeof(CreateAssembly))]
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
    [InlineData(typeof(CreateEffectiveLengthOptions))]
    [InlineData(typeof(GetEffectiveLengthOptions))]
    [InlineData(typeof(CreateMemberEndRestraint))]
    [InlineData(typeof(MemberEndRestraintInfo))]
    [InlineData(typeof(ExpandBeamToShell))]
    [InlineData(typeof(LocalAxes))]
    [InlineData(typeof(SectionAlignment))]
    [InlineData(typeof(GetAssembly))]
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
    [InlineData(typeof(AnalysisTaskInfo))]
    [InlineData(typeof(CreateSteelSectionPool))]
    [InlineData(typeof(SteelSectionPoolNames))]
    [InlineData(typeof(CreateSteelDesignTask))]
    [InlineData(typeof(DesignTaskInfo))]
    [InlineData(typeof(SteelDesign))]
    // 5_Results
    [InlineData(typeof(AssemblyDisplacements))]
    [InlineData(typeof(AssemblyDriftIndices))]
    [InlineData(typeof(AssemblyDrifts))]
    [InlineData(typeof(AssemblyForcesAndMoments))]
    [InlineData(typeof(BeamDerivedStresses))]
    [InlineData(typeof(BeamDisplacements))]
    [InlineData(typeof(BeamForcesAndMoments))]
    [InlineData(typeof(BeamStrainEnergyDensity))]
    [InlineData(typeof(BeamStresses))]
    [InlineData(typeof(Element2dDisplacements))]
    [InlineData(typeof(Element2dForcesAndMoments))]
    [InlineData(typeof(Element2dStresses))]
    [InlineData(typeof(Element3dDisplacements))]
    [InlineData(typeof(Element3dStresses))]
    [InlineData(typeof(FootfallResults))]
    [InlineData(typeof(GetResult))]
    [InlineData(typeof(GlobalPerformanceResults))]
    [InlineData(typeof(NodalForcesAndMoments))]
    [InlineData(typeof(NodeDisplacements))]
    [InlineData(typeof(ReactionForces))]
    [InlineData(typeof(ReactionForceDiagrams))]
    [InlineData(typeof(GetResultCases))]
    [InlineData(typeof(SelectResult))]
    [InlineData(typeof(SpringReactionForces))]
    [InlineData(typeof(TotalLoadsAndReactions))]
    [InlineData(typeof(SteelUtilisations))]
    // 6_Display
    [InlineData(typeof(AnnotateDetailed))]
    [InlineData(typeof(Annotate))]
    [InlineData(typeof(ContourNodeResults))]
    [InlineData(typeof(Contour1dResults))]
    [InlineData(typeof(Contour2dResults))]
    [InlineData(typeof(Contour3dResults))]
    [InlineData(typeof(LineResultInfo))]
    [InlineData(typeof(MeshResultInfo))]
    [InlineData(typeof(PointResultInfo))]
    // 99_Obsolete
    [InlineData(typeof(GlobalPerformanceResults_OBSOLETE), true)]
    [InlineData(typeof(GetModelProperties_OBSOLETE), true)]
    [InlineData(typeof(CreateAnalysisTask_OBSOLETE), true)]
    public void GH_OasysComponentTest(Type t, bool obsolete = false) {
      var comp = (GH_OasysComponent)Activator.CreateInstance(t);
      Assert.NotNull(comp.Icon_24x24);
      if (!obsolete) {
        Assert.NotEqual(GH_Exposure.hidden, comp.Exposure);
      }
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

    [Fact]
    public void GH_OasysTaskCapableComponent_OBSOLETE() {
      var comp = new GetModelGeometry_OBSOLETE();
      Assert.NotNull(comp.Icon_24x24);
      Assert.Equal(GH_Exposure.hidden, comp.Exposure);
      Assert.NotEqual(new Guid(), comp.ComponentGuid);
      Assert.Equal(PluginInfo.Instance, comp.PluginInfo);
    }
  }
}
