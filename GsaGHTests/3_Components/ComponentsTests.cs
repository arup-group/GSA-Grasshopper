using System;
using GsaGH.Components;
using GsaGH.Components.GraveyardComp;
using OasysGH.Components;
using Xunit;

namespace GsaGHTests._3_Components {
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
    [InlineData(typeof(Elem1dResultDiagram), 2)]
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
    //Graveyard
    [InlineData(typeof(CreateCustomMaterial_OBSOLETE), 4)]
    [InlineData(typeof(CreateGridPlane_OBSOLETE), 2)]
    [InlineData(typeof(CreateGridSurface_OBSOLETE), 2)]
    [InlineData(typeof(CreateMaterial2_OBSOLETE), 1)]
    [InlineData(typeof(CreateMember2d_OBSOLETE), 1)]
    [InlineData(typeof(CreateMember3d_OBSOLETE), 1)]
    [InlineData(typeof(CreateProfile_OBSOLETE), 2)]
    [InlineData(typeof(Elem1dContourResults_OBSOLETE), 3)]
    [InlineData(typeof(Elem2dContourResults_OBSOLETE), 3)]
    [InlineData(typeof(Elem2dFromBrep_OBSOLETE), 1)]
    [InlineData(typeof(Elem2dFromBrep2_OBSOLETE), 1)]
    [InlineData(typeof(NodeContourResults_OBSOLETE), 3)]
    [InlineData(typeof(ReactionForceDiagrams2_OBSOLETE), 1)]
    [InlineData(typeof(ReactionForceDiagrams_OBSOLETE), 1)]
    private void WhenInitialiseDropdowns_ThenDropDownItemsCount_ShouldBeValid(
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
    private void WhenInitialiseDropdowns_ThenDropDownItems_ShouldBeNull(Type t) {
      var obj = (GH_OasysDropDownComponent)Activator.CreateInstance(t);
      obj.InitialiseDropdowns();

      Assert.Null(obj._dropDownItems);
    }

    //[Theory]
    ////Model
    //[InlineData(typeof(CreateList), 0)]
    //[InlineData(typeof(CreateModel), 1)]
    //[InlineData(typeof(GetLoads), 1)]
    //[InlineData(typeof(ListInfo), 1)]
    ////Properties
    //[InlineData(typeof(CreateCustomMaterial), 1)]
    //[InlineData(typeof(CreateMaterial), 1)]
    //[InlineData(typeof(CreateOffset), 1)]
    //[InlineData(typeof(CreateProp2d), 1)]
    //[InlineData(typeof(CreateProp2dModifier), 1)]
    //[InlineData(typeof(CreateSection), 1)]
    //[InlineData(typeof(CreateSectionModifier), 1)]
    ////Geometry
    //[InlineData(typeof(Element2dFromBrep), 1)]
    //[InlineData(typeof(ElementFromMembers), 1)]
    //[InlineData(typeof(SectionAlignment), 1)]
    ////Loads
    //[InlineData(typeof(CreateBeamLoads), 1)]
    //[InlineData(typeof(CreateFaceLoads), 1)]
    //[InlineData(typeof(CreateGridAreaLoad), 1)]
    //[InlineData(typeof(CreateGridLineLoad), 1)]
    //[InlineData(typeof(CreateGridPlane), 1)]
    //[InlineData(typeof(CreateGridPointLoad), 1)]
    //[InlineData(typeof(CreateGridSurface), 1)]
    //[InlineData(typeof(CreateNodeLoad), 0)]
    //[InlineData(typeof(LoadProperties), 1)]
    ////Analysis
    //[InlineData(typeof(GhAnalyse), 2)]
    //[InlineData(typeof(CreateAnalysisTask), 0)]
    ////Results
    //[InlineData(typeof(BeamDisplacement), 0)]
    //[InlineData(typeof(BeamForces), 1)]
    //[InlineData(typeof(BeamStrainEnergy), 1)]
    //[InlineData(typeof(Elem1dContourResults), 1)]
    //[InlineData(typeof(Elem1dResultDiagram), 1)]
    //[InlineData(typeof(Elem2dContourResults), 1)]
    //[InlineData(typeof(Elem2dDisplacement), 1)]
    //[InlineData(typeof(Elem2dForces), 1)]
    //[InlineData(typeof(Elem2dStress), 1)]
    //[InlineData(typeof(Elem3dContourResults), 1)]
    //[InlineData(typeof(Elem3dDisplacement), 1)]
    //[InlineData(typeof(Elem3dStress), 1)]
    //[InlineData(typeof(GlobalPerformanceResults), 1)]
    //[InlineData(typeof(NodeContourResults), 1)]
    //[InlineData(typeof(NodeDisplacement), 1)]
    //[InlineData(typeof(ReactionForce), 1)]
    //[InlineData(typeof(ReactionForceDiagrams), 1)]
    //[InlineData(typeof(SelectResult), 1)]
    //[InlineData(typeof(SpringReactionForce), 1)]
    //[InlineData(typeof(TotalLoadsAndReactionResults), 1)]
    ////Graveyard
    //[InlineData(typeof(CreateCustomMaterial_OBSOLETE), 1)]
    //[InlineData(typeof(CreateGridPlane_OBSOLETE), 1)]
    //[InlineData(typeof(CreateGridSurface_OBSOLETE), 1)]
    //[InlineData(typeof(CreateMaterial2_OBSOLETE), 1)]
    //[InlineData(typeof(CreateMember2d_OBSOLETE), 1)]
    //[InlineData(typeof(CreateMember3d_OBSOLETE), 1)]
    //[InlineData(typeof(CreateProfile_OBSOLETE), 1)]
    //[InlineData(typeof(Elem1dContourResults_OBSOLETE), 1)]
    //[InlineData(typeof(Elem2dContourResults_OBSOLETE), 1)]
    //[InlineData(typeof(Elem2dFromBrep_OBSOLETE), 1)]
    //[InlineData(typeof(Elem2dFromBrep2_OBSOLETE), 1)]
    //[InlineData(typeof(NodeContourResults_OBSOLETE), 1)]
    //[InlineData(typeof(ReactionForceDiagrams2_OBSOLETE), 3)]
    //[InlineData(typeof(ReactionForceDiagrams_OBSOLETE), 3)]
    //private void WhenSetSelected_ThenSelectedItems_ShouldBeValid(Type t, string defaultValue, string expectedValue) {
    //  var obj = (GH_OasysDropDownComponent)Activator.CreateInstance(t);
    //  obj.InitialiseDropdowns();

    //  Assert.Equal(obj._selectedItems[0], defaultValue);

    //  obj.SetSelected(0, 0);

    //  Assert.Equal(obj._selectedItems[0], expectedValue);
    //}
  }
}
