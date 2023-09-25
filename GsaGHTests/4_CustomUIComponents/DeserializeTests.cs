using System;
using GsaGH.Components;
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

    public void DeSerializeComponentTest(Type t) {
      var comp = (GH_OasysComponent)Activator.CreateInstance(t);
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }
  }
}
