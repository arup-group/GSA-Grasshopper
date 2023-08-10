using System;
using GsaGH.Components;
using GsaGH.Components.GraveyardComp;
using OasysGH.Components;
using Xunit;

namespace GsaGHTests.Components {
  [Collection("GrasshopperFixture collection")]
  public class ComponentsTests {

    [Theory]
    //Model
    [InlineData(typeof(CreateList), 1)]
    [InlineData(typeof(CreateModel), 1)]
    [InlineData(typeof(GetLoads), 1)]
    [InlineData(typeof(ListInfo), 1)]
    //Properties
    [InlineData(typeof(CreateCustomMaterial), 4)]
    [InlineData(typeof(CreateMaterial), 3)]
    [InlineData(typeof(CreateOffset), 1)]
    [InlineData(typeof(CreateProp2d), 2)]
    [InlineData(typeof(CreateProp2dModifier), 2)]
    [InlineData(typeof(CreateSection), 1)]
    [InlineData(typeof(CreateSectionModifier), 3)]
    //Geometry
    [InlineData(typeof(Element2dFromBrep), 1)]
    [InlineData(typeof(ElementFromMembers), 1)]
    [InlineData(typeof(SectionAlignment), 1)]
    //Loads
    [InlineData(typeof(CreateBeamLoads), 2)]
    [InlineData(typeof(CreateFaceLoads), 2)]
    [InlineData(typeof(CreateGridAreaLoad), 1)]
    [InlineData(typeof(CreateGridLineLoad), 1)]
    [InlineData(typeof(CreateGridPlane), 1)]
    [InlineData(typeof(CreateGridPointLoad), 1)]
    [InlineData(typeof(CreateGridSurface), 1)]
    [InlineData(typeof(CreateNodeLoad), 2)]
    [InlineData(typeof(LoadProperties), 2)]
    //Analysis
    [InlineData(typeof(GhAnalyse), 1)]
    [InlineData(typeof(CreateAnalysisTask), 1)]
    //Results
    [InlineData(typeof(BeamDisplacement), 1)]
    [InlineData(typeof(BeamForces), 2)]
    [InlineData(typeof(BeamStrainEnergy), 1)]
    [InlineData(typeof(Elem1dContourResults), 2)]
    [InlineData(typeof(ResultDiagrams), 2)]
    [InlineData(typeof(Elem2dContourResults), 2)]
    [InlineData(typeof(Elem2dDisplacement), 1)]
    [InlineData(typeof(Elem2dForces), 2)]
    [InlineData(typeof(Elem2dStress), 1)]
    [InlineData(typeof(Elem3dContourResults), 2)]
    [InlineData(typeof(Elem3dDisplacement), 1)]
    [InlineData(typeof(Elem3dStress), 1)]
    [InlineData(typeof(GlobalPerformanceResults), 3)]
    [InlineData(typeof(NodeContourResults), 2)]
    [InlineData(typeof(NodeDisplacement), 1)]
    [InlineData(typeof(ReactionForce), 2)]
    [InlineData(typeof(ReactionForceDiagrams), 1)]
    [InlineData(typeof(SelectResult), 2)]
    [InlineData(typeof(SpringReactionForce), 2)]
    [InlineData(typeof(TotalLoadsAndReactionResults), 2)]
    public void WhenInitialiseDropdowns_ThenDropDownItemsCount_ShouldBeValid(
      Type t, int expectedListCount) {
      var obj = (GH_OasysDropDownComponent)Activator.CreateInstance(t);
      obj.InitialiseDropdowns();

      Assert.Equal(expectedListCount, obj._dropDownItems.Count);
    }

    [Theory]
    [InlineData(typeof(OpenModel))]
    [InlineData(typeof(SaveModel))]
    [InlineData(typeof(CreateBool6))]
    [InlineData(typeof(CreateMember1d))]
    [InlineData(typeof(CreateSupport))]
    [InlineData(typeof(CreateMember1d_OBSOLETE))]
    public void WhenInitialiseDropdowns_ThenDropDownItems_ShouldBeNull(Type t) {
      var obj = (GH_OasysDropDownComponent)Activator.CreateInstance(t);
      obj.InitialiseDropdowns();

      Assert.Null(obj._dropDownItems);
    }

    [Theory]
    //Model
    [InlineData(typeof(CreateList), "Node", "Case")]
    [InlineData(typeof(CreateModel), "m", "ft")]
    [InlineData(typeof(GetLoads), "m", "ft")]
    [InlineData(typeof(ListInfo), "m", "ft")]
    //Properties
    [InlineData(typeof(CreateCustomMaterial), "Timber", "Fabric")]
    [InlineData(typeof(CreateMaterial), "Concrete", "Fabric")]
    [InlineData(typeof(CreateOffset), "m", "ft")]
    [InlineData(typeof(CreateProp2d), "Shell", "Load Panel")]
    [InlineData(typeof(CreateProp2dModifier), "Modify by", "Modify to")]
    [InlineData(typeof(CreateSection), "Centroid", "BottomRight")]
    [InlineData(typeof(CreateSectionModifier), "Modify by", "Modify to")]
    //Geometry
    [InlineData(typeof(Element2dFromBrep), "m", "ft")]
    [InlineData(typeof(ElementFromMembers), "m", "ft")]
    [InlineData(typeof(SectionAlignment), "Centroid", "BottomRight")]
    //Loads
    [InlineData(typeof(CreateBeamLoads), "Uniform", "Trilinear")]
    [InlineData(typeof(CreateFaceLoads), "Uniform", "Point")]
    [InlineData(typeof(CreateGridAreaLoad), "kN/m²", "kipf/ft²")]
    [InlineData(typeof(CreateGridLineLoad), "kN/m", "kipf/ft")]
    [InlineData(typeof(CreateGridPlane), "General", "Storey")]
    [InlineData(typeof(CreateGridPointLoad), "kN", "tf")]
    [InlineData(typeof(CreateGridSurface), "1D, One-way span", "2D")]
    [InlineData(typeof(CreateNodeLoad), "NodeForce", "Settlement")]
    [InlineData(typeof(LoadProperties), "kN", "kipf")]
    //Analysis
    [InlineData(typeof(GhAnalyse), "m", "ft")]
    [InlineData(typeof(CreateAnalysisTask), "Static", "Static")]
    //Results
    [InlineData(typeof(BeamDisplacement), "mm", "ft")]
    [InlineData(typeof(BeamForces), "kN", "tf")]
    [InlineData(typeof(BeamStrainEnergy), "MJ", "BTU")]
    [InlineData(typeof(Elem1dContourResults), "Displacement", "Footfall")]
    [InlineData(typeof(ResultDiagrams), "Force", "Stress")]
    [InlineData(typeof(Elem2dContourResults), "Displacement", "Footfall")]
    [InlineData(typeof(Elem2dDisplacement), "mm", "ft")]
    [InlineData(typeof(Elem2dForces), "kN/m", "kipf/ft")]
    [InlineData(typeof(Elem2dStress), "Megapascal", "kipf/ft²")]
    [InlineData(typeof(Elem3dContourResults), "Displacement", "Stress")]
    [InlineData(typeof(Elem3dDisplacement), "mm", "ft")]
    [InlineData(typeof(Elem3dStress), "Megapascal", "kipf/ft²")]
    [InlineData(typeof(GlobalPerformanceResults), "t", "slug")]
    [InlineData(typeof(NodeContourResults), "Displacement", "Footfall")]
    [InlineData(typeof(NodeDisplacement), "mm", "ft")]
    [InlineData(typeof(ReactionForce), "kN", "tf")]
    [InlineData(typeof(ReactionForceDiagrams), "Resolved |F|", "Resolved |M|")]
    [InlineData(typeof(SelectResult), "AnalysisCase", "Combination")]
    [InlineData(typeof(SpringReactionForce), "kN", "tf")]
    [InlineData(typeof(TotalLoadsAndReactionResults), "kN", "tf")]
    public void WhenSetSelected_ThenSelectedItems_ShouldBeValid(
      Type t, string defaultValue, string firstValue) {
      var obj = (GH_OasysDropDownComponent)Activator.CreateInstance(t);
      obj.InitialiseDropdowns();

      Assert.Equal(obj._selectedItems[0].ToString(), defaultValue, true, true, true);

      obj.SetSelected(0, obj._dropDownItems[0].Count - 1);

      Assert.Equal(obj._selectedItems[0].ToString(), firstValue, true, true, true);
    }
  }
}
