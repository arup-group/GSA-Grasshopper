using System;
using GsaGH.Components;
using GsaGHTests.Helpers;
using OasysGH.Components;
using Xunit;

namespace GsaGHTests.CustomComponent {
  [Collection("GrasshopperFixture collection")]
  public class DropDownComponentTests {

    [Theory]
    [InlineData(typeof(CreateList))]
    [InlineData(typeof(CreateModel), true)]
    [InlineData(typeof(GetModelLoads))]
    [InlineData(typeof(ListInfo))]
    //prop
    [InlineData(typeof(CreateCustomMaterial))]
    [InlineData(typeof(CreateMaterial))]
    [InlineData(typeof(Create2dMember))]
    [InlineData(typeof(CreateOffset))]
    [InlineData(typeof(Create2dProperty))]
    [InlineData(typeof(Create2dPropertyModifier))]
    [InlineData(typeof(CreateSection))]
    [InlineData(typeof(CreateSectionModifier))]
    //geometry
    [InlineData(typeof(Create2dElementsFromBrep))]
    [InlineData(typeof(CreateElementsFromMembers))]
    [InlineData(typeof(SectionAlignment))]
    //loads
    [InlineData(typeof(CreateBeamLoad))]
    [InlineData(typeof(CreateFaceLoad))]
    [InlineData(typeof(CreateGridAreaLoad))]
    [InlineData(typeof(CreateGridLineLoad))]
    [InlineData(typeof(CreateGridPlane))]
    [InlineData(typeof(CreateGridPointLoad))]
    [InlineData(typeof(CreateGridSurface))]
    [InlineData(typeof(CreateNodeLoad))]
    [InlineData(typeof(LoadProperties))]
    //analysis
    [InlineData(typeof(AnalyseModel), true)]
    [InlineData(typeof(CreateAnalysisTask))]
    //results
    [InlineData(typeof(BeamDisplacements), true)]
    [InlineData(typeof(BeamForcesAndMoments))]
    [InlineData(typeof(BeamStrainEnergyDensity), true)]
    [InlineData(typeof(Contour1dResults), true)]
    [InlineData(typeof(ResultDiagrams))]
    [InlineData(typeof(Contour2dResults), true)]
    [InlineData(typeof(Element2dDisplacements))]
    [InlineData(typeof(Element2dForcesAndMoments))]
    [InlineData(typeof(Element2dStresses))]
    [InlineData(typeof(Contour3dResults), true)]
    [InlineData(typeof(Element3dDisplacements))]
    [InlineData(typeof(Element3dStresses))]
    [InlineData(typeof(GlobalPerformanceResults))]
    [InlineData(typeof(ContourNodeResults), true)]
    [InlineData(typeof(NodeDisplacements))]
    [InlineData(typeof(ReactionForces))]
    [InlineData(typeof(ReactionForceDiagrams))]
    [InlineData(typeof(SpringReactionForces))]
    [InlineData(typeof(TotalLoadsAndReactions))]
    [InlineData(typeof(CreateProfile), true)]
    public void DropDownComponentTest(Type t, bool ignoreSpacerDescriptionCount = false) {
      var comp = (GH_OasysDropDownComponent)Activator.CreateInstance(t);
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp, ignoreSpacerDescriptionCount);
    }
  }
}
