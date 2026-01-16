using System;

using GsaGH.Components;

using OasysGH.Components;

using Xunit;

namespace GsaGHTests.Components {
  [Collection("GrasshopperFixture collection")]
  public class ComponentsTests {
    [Theory]
    // Model
    [InlineData(typeof(CreateList), 1)]
    [InlineData(typeof(CreateModel), 1)]
    [InlineData(typeof(GetModelLoads), 1)]
    [InlineData(typeof(ListInfo), 1)]
    [InlineData(typeof(PropertyQuantities), 1)]
    [InlineData(typeof(MaterialQuantities), 1)]
    // Properties
    [InlineData(typeof(CreateCustomMaterial), 4)]
    [InlineData(typeof(CreateMaterial), 3)]
    [InlineData(typeof(CreateOffset), 1)]
    [InlineData(typeof(Create2dProperty), 2)]
    [InlineData(typeof(Create2dPropertyModifier), 2)]
    [InlineData(typeof(CreateSection), 1)]
    [InlineData(typeof(CreateSectionModifier), 3)]
    [InlineData(typeof(CreateSpringProperty), 2)]
    // Geometry
    [InlineData(typeof(Create2dElementsFromBrep), 2)]
    [InlineData(typeof(CreateElementsFromMembers), 1)]
    [InlineData(typeof(SectionAlignment), 1)]
    [InlineData(typeof(CreateMemberEndRestraint), 1)]
    [InlineData(typeof(CreateAssembly), 2)]
    // Loads
    [InlineData(typeof(CreateBeamLoad), 2)]
    [InlineData(typeof(CreateBeamThermalLoad), 2)]
    [InlineData(typeof(CreateFaceLoad), 2)]
    [InlineData(typeof(CreateFaceThermalLoad), 2)]
    [InlineData(typeof(CreateGridAreaLoad), 1)]
    [InlineData(typeof(CreateGridLineLoad), 1)]
    [InlineData(typeof(CreateGridPlane), 1)]
    [InlineData(typeof(CreateGridPointLoad), 1)]
    [InlineData(typeof(CreateGridSurface), 1)]
    [InlineData(typeof(CreateNodeLoad), 2)]
    [InlineData(typeof(LoadProperties), 2)]
    // Analysis
    [InlineData(typeof(AnalyseModel), 1)]
    [InlineData(typeof(CreateAnalysisTask), 1)]
    [InlineData(typeof(CreateSteelDesignTask), 2)]
    //Results
    [InlineData(typeof(AssemblyDisplacements), 2)]
    [InlineData(typeof(AssemblyDriftIndices), 1)]
    [InlineData(typeof(AssemblyDrifts), 2)]
    [InlineData(typeof(AssemblyForcesAndMoments), 3)]
    [InlineData(typeof(BeamDerivedStresses), 2)]
    [InlineData(typeof(BeamDisplacements), 2)]
    [InlineData(typeof(BeamForcesAndMoments), 3)]
    [InlineData(typeof(BeamStrainEnergyDensity), 2)]
    [InlineData(typeof(BeamStresses), 2)]
    [InlineData(typeof(Element2dDisplacements), 2)]
    [InlineData(typeof(Element2dForcesAndMoments), 3)]
    [InlineData(typeof(Element2dStresses), 2)]
    [InlineData(typeof(Element3dDisplacements), 2)]
    [InlineData(typeof(Element3dStresses), 2)]
    [InlineData(typeof(FootfallResults), 2)]
    [InlineData(typeof(GlobalPerformanceResults), 3)]
    [InlineData(typeof(GlobalPerformanceResults_OBSOLETE), 3)]
    [InlineData(typeof(Member1dDisplacements), 2)]
    [InlineData(typeof(Member1dForcesAndMoments), 3)]
    [InlineData(typeof(NodalForcesAndMoments), 2)]
    [InlineData(typeof(NodeDisplacements), 2)]
    [InlineData(typeof(ReactionForces), 3)]
    [InlineData(typeof(SelectResult), 2)]
    [InlineData(typeof(SpringReactionForces), 3)]
    [InlineData(typeof(SteelDesignEffectiveLength), 2)]
    [InlineData(typeof(SteelUtilisations), 1)]
    [InlineData(typeof(TotalLoadsAndReactions), 2)]
    // Display
    [InlineData(typeof(AssemblyResultDiagrams), 2)]
    [InlineData(typeof(AssemblyResults), 2)]
    [InlineData(typeof(Contour1dResults), 2)]
    [InlineData(typeof(Contour2dResults), 2)]
    [InlineData(typeof(Contour3dResults), 2)]
    [InlineData(typeof(ContourNodeResults), 2)]
    [InlineData(typeof(ReactionForceDiagrams), 1)]
    [InlineData(typeof(ResultDiagrams), 2)]
    [InlineData(typeof(CreateAnalysisTask_OBSOLETE), 1)]
    public void WhenInitialiseDropdowns_ThenDropDownItemsCount_ShouldBeValid(
      Type t, int expectedListCount) {
      var obj = (GH_OasysDropDownComponent)Activator.CreateInstance(t);


      Assert.Equal(expectedListCount, obj.DropDownItems.Count);
    }

    [Theory]
    [InlineData(typeof(OpenModel))]
    [InlineData(typeof(SaveGsaModel))]
    [InlineData(typeof(CreateBool6))]
    [InlineData(typeof(Create1dMember))]
    [InlineData(typeof(CreateSupport))]
    public void WhenInitialiseDropdowns_ThenDropDownItems_ShouldBeNull(Type t) {
      var obj = (GH_OasysDropDownComponent)Activator.CreateInstance(t);


      Assert.Null(obj.DropDownItems);
    }

    [Theory]
    // Model
    [InlineData(typeof(CreateList), "Node", "Case")]
    [InlineData(typeof(CreateModel), "m", "ft")]
    [InlineData(typeof(GetModelLoads), "m", "ft")]
    [InlineData(typeof(ListInfo), "m", "ft")]
    // Properties
    [InlineData(typeof(CreateCustomMaterial), "Timber", "Fabric")]
    [InlineData(typeof(CreateMaterial), "Concrete", "Fabric")]
    [InlineData(typeof(CreateOffset), "m", "ft")]
    [InlineData(typeof(Create2dProperty), "Shell", "Load Panel")]
    [InlineData(typeof(Create2dPropertyModifier), "Modify by", "Modify to")]
    [InlineData(typeof(CreateSection), "Centroid", "BottomRight")]
    [InlineData(typeof(CreateSectionModifier), "Modify by", "Modify to")]
    // Geometry
    [InlineData(typeof(Create2dElementsFromBrep), "Tri-6 only", "Quad-8 only")]
    [InlineData(typeof(CreateElementsFromMembers), "m", "ft")]
    [InlineData(typeof(SectionAlignment), "Centroid", "BottomRight")]
    // Loads
    [InlineData(typeof(CreateBeamLoad), "Uniform", "Trilinear")]
    [InlineData(typeof(CreateBeamThermalLoad), "Uniform", "Uniform")]
    [InlineData(typeof(CreateFaceLoad), "Uniform", "Equation")]
    [InlineData(typeof(CreateFaceThermalLoad), "Uniform", "Uniform")]
    [InlineData(typeof(CreateGridAreaLoad), "kN/m²", "kipf/ft²")]
    [InlineData(typeof(CreateGridLineLoad), "kN/m", "kipf/ft")]
    [InlineData(typeof(CreateGridPlane), "General", "Storey")]
    [InlineData(typeof(CreateGridPointLoad), "kN", "tf")]
    [InlineData(typeof(CreateGridSurface), "1D, One-way span", "2D")]
    [InlineData(typeof(CreateNodeLoad), "NodeForce", "Settlement")]
    [InlineData(typeof(LoadProperties), "kN", "kipf")]
    // Analysis
    [InlineData(typeof(AnalyseModel), "m", "ft")]
    [InlineData(typeof(CreateAnalysisTask_OBSOLETE), "Static", "Static")]
    // Results
    [InlineData(typeof(BeamDisplacements), "All", "Min |R|")]
    [InlineData(typeof(BeamForcesAndMoments), "All", "Min |Myz|")]
    [InlineData(typeof(BeamStrainEnergyDensity), "All", "Min")]
    [InlineData(typeof(Contour1dResults), "Displacement", "Steel Design")]
    [InlineData(typeof(ResultDiagrams), "Force", "Stress")]
    [InlineData(typeof(Contour2dResults), "Displacement", "Footfall")]
    [InlineData(typeof(Element2dDisplacements), "All", "Min |R|")]
    [InlineData(typeof(Element2dForcesAndMoments), "kN/m", "kipf/ft", 1)]
    [InlineData(typeof(Element2dStresses), "All", "Min zx")]
    [InlineData(typeof(Contour3dResults), "Displacement", "Stress")]
    [InlineData(typeof(Element3dDisplacements), "All", "Min |U|")]
    [InlineData(typeof(Element3dStresses), "All", "Min zx")]
    [InlineData(typeof(GlobalPerformanceResults_OBSOLETE), "t", "slug")]
    [InlineData(typeof(GlobalPerformanceResults), "t", "slug")]
    [InlineData(typeof(ContourNodeResults), "Displacement", "Footfall")]
    [InlineData(typeof(NodeDisplacements), "All", "Min |R|")]
    [InlineData(typeof(ReactionForces), "kN", "tf", 1)]
    [InlineData(typeof(ReactionForceDiagrams), "Resolved |F|", "Resolved |M|")]
    [InlineData(typeof(SelectResult), "AnalysisCase", "Combination")]
    [InlineData(typeof(SpringReactionForces), "kN", "tf", 1)]
    [InlineData(typeof(SteelDesignEffectiveLength), "mm", "ft", 1)]
    [InlineData(typeof(SteelUtilisations), "All", "Min FB")]
    [InlineData(typeof(TotalLoadsAndReactions), "kN", "tf")]
    [InlineData(typeof(Member1dDisplacements), "All", "Min |R|")]
    [InlineData(typeof(Member1dForcesAndMoments), "All", "Min |Myz|")]
    public void WhenSetSelected_ThenSelectedItems_ShouldBeValid(
      Type t, string defaultValue, string firstValue, int index = 0) {
      var obj = (GH_OasysDropDownComponent)Activator.CreateInstance(t);


      Assert.Equal(obj.SelectedItems[index].ToString(), defaultValue, true, true, true);

      obj.SetSelected(index, obj.DropDownItems[index].Count - 1);

      Assert.Equal(obj.SelectedItems[index].ToString(), firstValue, true, true, true);
    }
  }
}
