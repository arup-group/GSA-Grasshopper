using System;
using GsaGH.Components;
using GsaGH.Components.GraveyardComp;
using GsaGHTests.Helpers;
using OasysGH.Components;
using Xunit;

namespace GsaGHTests.CustomComponent {
  [Collection("GrasshopperFixture collection")]
  public class DeserializeTests {

    [Theory]
    //Model
    [InlineData(typeof(CreateModel))]
    [InlineData(typeof(OpenModel))]
    [InlineData(typeof(SaveModel))]
    //Properties
    [InlineData(typeof(CreateBool6))]
    [InlineData(typeof(CreateProp2dModifier))]
    [InlineData(typeof(CreateSectionModifier))]
    [InlineData(typeof(EditOffset))]
    [InlineData(typeof(EditProp2d))]
    [InlineData(typeof(GetMaterialProperties))]
    [InlineData(typeof(GetProp2dModifier))]
    [InlineData(typeof(GetSectionDimensions))]
    [InlineData(typeof(GetSectionModifier))]
    [InlineData(typeof(GetSectionProperties))]
    //Geometry
    [InlineData(typeof(CreateMember1d))]
    [InlineData(typeof(CreateSupport))]
    [InlineData(typeof(EditMember1d))]
    [InlineData(typeof(EditNode))]
    [InlineData(typeof(Element2dFromBrep))]
    [InlineData(typeof(ElementFromMembers))]
    //Loads
    [InlineData(typeof(CreateGridAreaLoad))]
    [InlineData(typeof(CreateGridLineLoad))]
    [InlineData(typeof(CreateGridPlane))]
    [InlineData(typeof(CreateGridPointLoad))]
    [InlineData(typeof(CreateGridSurface))]
    [InlineData(typeof(GridPlaneSurfaceProperties))]
    //Analysis
    [InlineData(typeof(GhAnalyse))]
    //Results
    [InlineData(typeof(BeamStrainEnergy))]
    [InlineData(typeof(Elem1dContourResults))]
    [InlineData(typeof(Elem1dResultDiagram))]
    [InlineData(typeof(Elem2dContourResults))]
    [InlineData(typeof(Elem3dContourResults))]
    [InlineData(typeof(NodeContourResults))]
    [InlineData(typeof(ReactionForceDiagrams))]
    //Graveyard
    [InlineData(typeof(CreateBool6_OBSOLETE))]
    [InlineData(typeof(CreateGridPlane_OBSOLETE))]
    [InlineData(typeof(CreateGridSurface_OBSOLETE))]
    [InlineData(typeof(CreateMaterial_OBSOLETE))]
    [InlineData(typeof(CreateMember1d_OBSOLETE))]
    [InlineData(typeof(CreateMember2d_OBSOLETE))]
    [InlineData(typeof(CreateProfile_OBSOLETE))]
    [InlineData(typeof(CreateProp2d_OBSOLETE))]
    [InlineData(typeof(EditMember1d3_OBSOLETE))]
    [InlineData(typeof(EditMember1d_OBSOLETE))]
    [InlineData(typeof(EditMember2d_OBSOLETE))]
    [InlineData(typeof(EditMember3d_OBSOLETE))]
    [InlineData(typeof(EditNode_OBSOLETE))]
    [InlineData(typeof(EditOffset_OBSOLETE))]
    [InlineData(typeof(EditProp2d3_OBSOLETE))]
    [InlineData(typeof(EditProp2d4_OBSOLETE))]
    [InlineData(typeof(EditProp2d5_OBSOLETE))]
    [InlineData(typeof(EditProp2d6_OBSOLETE))]
    [InlineData(typeof(EditProp2d_OBSOLETE))]
    [InlineData(typeof(EditSectionModifier2_OBSOLETE))]
    [InlineData(typeof(EditSectionModifier_OBSOLETE))]
    [InlineData(typeof(Elem1dContourResults_OBSOLETE))]
    [InlineData(typeof(Elem2dContourResults_OBSOLETE))]
    [InlineData(typeof(Elem2dFromBrep2_OBSOLETE))]
    [InlineData(typeof(GetSectionProperties2_OBSOLETE))]
    [InlineData(typeof(GetSectionProperties_OBSOLETE))]
    [InlineData(typeof(NodeContourResults_OBSOLETE))]
    [InlineData(typeof(ReactionForceDiagrams2_OBSOLETE))]
    [InlineData(typeof(ReactionForceDiagrams_OBSOLETE))]
    public void DeSerializeComponentTest(Type t) {
      var comp = (GH_OasysComponent)Activator.CreateInstance(t);
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }
  }
}
