﻿using System;
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
    [InlineData(typeof(CreateEffectiveLength))]
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
    [InlineData(typeof(BeamDerivedStresses))]
    [InlineData(typeof(BeamDisplacements))]
    [InlineData(typeof(BeamForcesAndMoments))]
    [InlineData(typeof(BeamStrainEnergyDensity))]
    [InlineData(typeof(BeamStresses))]
    [InlineData(typeof(Element2dDisplacements))]
    [InlineData(typeof(Element2dForcesAndMoments))]
    [InlineData(typeof(Element2dStresses))]
    [InlineData(typeof(Element3dDisplacements))]
    [InlineData(typeof(Element3dStresses))]
    [InlineData(typeof(FootfallResults))]
    [InlineData(typeof(GlobalPerformanceResults))]
    [InlineData(typeof(Member1dDisplacements))]
    [InlineData(typeof(Member1dForcesAndMoments))]
    [InlineData(typeof(NodeDisplacements))]
    [InlineData(typeof(ReactionForces))]
    [InlineData(typeof(SpringReactionForces))]
    [InlineData(typeof(TotalLoadsAndReactions))]
    //Display
    [InlineData(typeof(AnnotateDetailed))]
    [InlineData(typeof(Contour1dResults))]
    [InlineData(typeof(Contour2dResults))]
    [InlineData(typeof(Contour3dResults))]
    [InlineData(typeof(ContourNodeResults))]
    [InlineData(typeof(LoadDiagrams))]
    [InlineData(typeof(ResultDiagrams))]
    [InlineData(typeof(ReactionForceDiagrams))]
    public void DeSerializeComponentTest(Type t) {
      var comp = (GH_OasysComponent)Activator.CreateInstance(t);
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }
  }
}
