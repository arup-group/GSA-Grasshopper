using System;
using GsaGH.Components;
using GsaGHTests.Helpers;
using OasysGH.Components;
using Xunit;

namespace GsaGHTests.CustomComponent {
  [Collection("GrasshopperFixture collection")]
  public class DropDownComponentTests {

    [Theory]
    [InlineData(typeof(CreateModel), true)]
    [InlineData(typeof(CreateList))]
    [InlineData(typeof(ListInfo))]
    [InlineData(typeof(GetLoads))]
    [InlineData(typeof(CreateCustomMaterial))]
    [InlineData(typeof(CreateMaterial))]
    [InlineData(typeof(CreateOffset))]
    [InlineData(typeof(CreateProfile), true)]
    [InlineData(typeof(CreateProp2d))]
    [InlineData(typeof(CreateSectionModifier))]
    [InlineData(typeof(ElementFromMembers))]
    [InlineData(typeof(SectionAlignment))]
    [InlineData(typeof(CreateBeamLoads))]
    [InlineData(typeof(CreateFaceLoads))]
    [InlineData(typeof(CreateGridAreaLoad))]
    [InlineData(typeof(CreateGridLineLoad))]
    [InlineData(typeof(CreateGridPlane))]
    [InlineData(typeof(CreateGridPointLoad))]
    [InlineData(typeof(CreateGridSurface))]
    [InlineData(typeof(CreateNodeLoad))]
    [InlineData(typeof(LoadProperties))]
    [InlineData(typeof(GhAnalyse), true)]
    [InlineData(typeof(CreateAnalysisTask))]
    [InlineData(typeof(BeamDisplacement), true)]
    [InlineData(typeof(BeamForces))]
    [InlineData(typeof(BeamStrainEnergy), true)]
    [InlineData(typeof(Elem1dContourResults), true)]
    [InlineData(typeof(Elem2dContourResults), true)]
    [InlineData(typeof(Elem2dDisplacement))]
    [InlineData(typeof(Elem2dForces))]
    [InlineData(typeof(Elem2dStress))]
    [InlineData(typeof(Elem3dDisplacement))]
    [InlineData(typeof(Elem3dStress))]
    [InlineData(typeof(GlobalPerformanceResults))]
    [InlineData(typeof(NodeContourResults), true)]
    [InlineData(typeof(NodeDisplacement))]
    [InlineData(typeof(ReactionForce))]
    [InlineData(typeof(ReactionForceDiagrams))]
    [InlineData(typeof(SpringReactionForce))]
    [InlineData(typeof(TotalLoadsAndReactionResults))]
    public void DropDownComponentTest(Type t, bool ignoreSpacerDescriptionCount = false) {
      var comp = (GH_OasysDropDownComponent)Activator.CreateInstance(t);
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp, ignoreSpacerDescriptionCount);
    }
  }
}
