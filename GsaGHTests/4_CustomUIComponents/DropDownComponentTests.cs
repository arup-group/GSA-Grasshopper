﻿using System;
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
    [InlineData(typeof(PropertyQuantities))]
    [InlineData(typeof(MaterialQuantities))]
    //prop
    [InlineData(typeof(CreateCustomMaterial))]
    [InlineData(typeof(CreateMaterial))]
    [InlineData(typeof(Create2dMember))]
    [InlineData(typeof(CreateOffset))]
    [InlineData(typeof(Create2dProperty))]
    [InlineData(typeof(Create2dPropertyModifier))]
    [InlineData(typeof(CreateSection))]
    [InlineData(typeof(CreateSectionModifier))]
    [InlineData(typeof(CreateSpringProperty))]
    [InlineData(typeof(CreateProfile), true)]
    //geometry
    [InlineData(typeof(Create2dElementsFromBrep))]
    [InlineData(typeof(CreateElementsFromMembers))]
    [InlineData(typeof(SectionAlignment))]
    [InlineData(typeof(CreateEffectiveLengthOptions))]
    [InlineData(typeof(CreateMemberEndRestraint))]
    [InlineData(typeof(CreateAssembly))]
    //loads
    [InlineData(typeof(CreateBeamLoad))]
    [InlineData(typeof(CreateBeamThermalLoad))]
    [InlineData(typeof(CreateFaceLoad))]
    [InlineData(typeof(CreateFaceThermalLoad))]
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
    [InlineData(typeof(CreateSteelDesignTask))]
    //results
    [InlineData(typeof(BeamDerivedStresses), true)]
    [InlineData(typeof(BeamDisplacements), true)]
    [InlineData(typeof(BeamForcesAndMoments))]
    [InlineData(typeof(BeamStrainEnergyDensity), true)]
    [InlineData(typeof(BeamStresses), true)]
    [InlineData(typeof(Element2dDisplacements))]
    [InlineData(typeof(Element2dForcesAndMoments))]
    [InlineData(typeof(Element2dStresses))]
    [InlineData(typeof(Element3dDisplacements))]
    [InlineData(typeof(Element3dStresses))]
    [InlineData(typeof(FootfallResults))]
    [InlineData(typeof(GlobalPerformanceResults_OBSOLETE))]
    [InlineData(typeof(GlobalPerformanceResults))]
    [InlineData(typeof(Member1dDisplacements))]
    [InlineData(typeof(Member1dForcesAndMoments))]
    [InlineData(typeof(NodeDisplacements))]
    [InlineData(typeof(ReactionForces))]
    [InlineData(typeof(SpringReactionForces))]
    [InlineData(typeof(TotalLoadsAndReactions))]
    //display
    [InlineData(typeof(Contour1dResults), true)]
    [InlineData(typeof(Contour2dResults), true)]
    [InlineData(typeof(Contour3dResults), true)]
    [InlineData(typeof(ContourNodeResults), true)]
    [InlineData(typeof(LoadDiagrams), true)]
    [InlineData(typeof(ReactionForceDiagrams))]
    [InlineData(typeof(ResultDiagrams))]
    public void ToggleDropDownsTest(Type t, bool ignoreSpacerDescriptionCount = false) {
      var comp = (GH_OasysDropDownComponent)Activator.CreateInstance(t);
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp, ignoreSpacerDescriptionCount);
    }
  }
}
