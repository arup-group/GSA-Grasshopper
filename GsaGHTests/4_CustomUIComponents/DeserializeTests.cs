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
    [InlineData(typeof(SaveGsaModel))]
    //Properties
    [InlineData(typeof(CreateBool6))]
    [InlineData(typeof(Create2dPropertyModifier))]
    [InlineData(typeof(CreateSectionModifier))]
    [InlineData(typeof(EditOffset))]
    [InlineData(typeof(Edit2dProperty))]
    [InlineData(typeof(MaterialProperties))]
    [InlineData(typeof(Get2dPropertyModifier))]
    [InlineData(typeof(ProfileDimensions))]
    [InlineData(typeof(GetSectionModifier))]
    [InlineData(typeof(SectionProperties))]
    //Geometry
    [InlineData(typeof(Create1dMember))]
    [InlineData(typeof(CreateSupport))]
    [InlineData(typeof(Edit1dMember))]
    [InlineData(typeof(EditNode))]
    [InlineData(typeof(Create2dElementsFromBrep))]
    [InlineData(typeof(CreateElementsFromMembers))]
    //Loads
    [InlineData(typeof(CreateGridAreaLoad))]
    [InlineData(typeof(CreateGridLineLoad))]
    [InlineData(typeof(CreateGridPlane))]
    [InlineData(typeof(CreateGridPointLoad))]
    [InlineData(typeof(CreateGridSurface))]
    [InlineData(typeof(GridPlaneSurfaceProperties))]
    //Analysis
    [InlineData(typeof(AnalyseModel))]
    //Results
    [InlineData(typeof(BeamStrainEnergyDensity))]
    [InlineData(typeof(Contour1dResults))]
    [InlineData(typeof(ResultDiagrams))]
    [InlineData(typeof(Contour2dResults))]
    [InlineData(typeof(Contour3dResults))]
    [InlineData(typeof(ContourNodeResults))]
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
    [InlineData(typeof(EditProp2d3_OBSOLETE))]
    [InlineData(typeof(EditProp2d4_OBSOLETE))]
    [InlineData(typeof(EditProp2d5_OBSOLETE))]
    [InlineData(typeof(EditProp2d6_OBSOLETE))]
    [InlineData(typeof(EditProp2d_OBSOLETE))]
    [InlineData(typeof(EditSectionModifier2_OBSOLETE))]
    [InlineData(typeof(Elem1dContourResults_OBSOLETE))]
    [InlineData(typeof(Elem2dContourResults_OBSOLETE))]
    [InlineData(typeof(Elem2dFromBrep2_OBSOLETE))]
    [InlineData(typeof(GetSectionProperties2_OBSOLETE))]
    [InlineData(typeof(NodeContourResults_OBSOLETE))]
    [InlineData(typeof(ReactionForceDiagrams2_OBSOLETE))]
    [InlineData(typeof(ReactionForceDiagrams_OBSOLETE))]
    public void DeSerializeComponentTest(Type t) {
      var comp = (GH_OasysComponent)Activator.CreateInstance(t);
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }
  }
}
