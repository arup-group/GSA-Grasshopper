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
    [InlineData(typeof(GetLoads))]
    [InlineData(typeof(ListInfo))]
    //prop
    [InlineData(typeof(CreateCustomMaterial))]
    [InlineData(typeof(CreateMaterial))]
    [InlineData(typeof(CreateMember2d))]
    [InlineData(typeof(CreateOffset))]
    [InlineData(typeof(CreateProp2d))]
    [InlineData(typeof(CreateProp2dModifier))]
    [InlineData(typeof(CreateSection))]
    [InlineData(typeof(CreateSectionModifier))]
    //geometry
    [InlineData(typeof(Element2dFromBrep))]
    [InlineData(typeof(ElementFromMembers))]
    [InlineData(typeof(SectionAlignment))]
    //loads
    [InlineData(typeof(CreateBeamLoads))]
    [InlineData(typeof(CreateFaceLoads))]
    [InlineData(typeof(CreateGridAreaLoad))]
    [InlineData(typeof(CreateGridLineLoad))]
    [InlineData(typeof(CreateGridPlane))]
    [InlineData(typeof(CreateGridPointLoad))]
    [InlineData(typeof(CreateGridSurface))]
    [InlineData(typeof(CreateNodeLoad))]
    [InlineData(typeof(LoadProperties))]
    //analysis
    [InlineData(typeof(GhAnalyse), true)]
    [InlineData(typeof(CreateAnalysisTask))]
    //results
    [InlineData(typeof(BeamDisplacement), true)]
    [InlineData(typeof(BeamForces))]
    [InlineData(typeof(BeamStrainEnergy), true)]
    [InlineData(typeof(Elem1dContourResults), true)]
    [InlineData(typeof(Elem1dResultDiagram))]
    [InlineData(typeof(Elem2dContourResults), true)]
    [InlineData(typeof(Elem2dDisplacement))]
    [InlineData(typeof(Elem2dForces))]
    [InlineData(typeof(Elem2dStress))]
    [InlineData(typeof(Elem3dContourResults), true)]
    [InlineData(typeof(Elem3dDisplacement))]
    [InlineData(typeof(Elem3dStress))]
    [InlineData(typeof(GlobalPerformanceResults))]
    [InlineData(typeof(NodeContourResults), true)]
    [InlineData(typeof(NodeDisplacement))]
    [InlineData(typeof(ReactionForce))]
    [InlineData(typeof(ReactionForceDiagrams))]
    [InlineData(typeof(SpringReactionForce))]
    [InlineData(typeof(TotalLoadsAndReactionResults))]
    [InlineData(typeof(CreateProfile), true)]
    public void DropDownComponentTest(Type t, bool ignoreSpacerDescriptionCount = false) {
      var comp = (GH_OasysDropDownComponent)Activator.CreateInstance(t);
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp, ignoreSpacerDescriptionCount);
    }
  }
}
