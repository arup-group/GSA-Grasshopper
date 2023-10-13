﻿using System;
using System.Windows.Forms;
using Grasshopper.Kernel;
using GsaGH.Components;
using Xunit;

namespace GsaGHTests.CustomComponent {
  [Collection("GrasshopperFixture collection")]
  public class AppendAdditionalMenuItemsTests {
    [Theory]
    // AppendAdditionalMenuItems
    [InlineData(typeof(CreateModel), 2)]
    [InlineData(typeof(Edit2dProperty), 2)]
    [InlineData(typeof(EditOffset), 2)]
    [InlineData(typeof(EditSection), 2)]
    [InlineData(typeof(Get2dPropertyModifier), 2)]
    [InlineData(typeof(GetSectionModifier), 2)]
    [InlineData(typeof(MaterialProperties), 2)]
    [InlineData(typeof(ProfileDimensions), 2)]
    [InlineData(typeof(SectionProperties), 2)]
    [InlineData(typeof(Create2dElementsFromBrep), 2)]
    [InlineData(typeof(CreateElementsFromMembers), 2)]
    [InlineData(typeof(Edit1dMember), 3)]
    [InlineData(typeof(GridPlaneSurfaceProperties), 2)]
    [InlineData(typeof(AnalyseModel), 2)]
    // AppendAdditionalComponentMenuItems
    [InlineData(typeof(GetModelGeometry), 7)]
    [InlineData(typeof(EditNode), 4)]
    [InlineData(typeof(CreateGridAreaLoad), 6)]
    [InlineData(typeof(CreateGridLineLoad), 6)]
    [InlineData(typeof(CreateGridPointLoad), 6)]
    [InlineData(typeof(Contour1dResults), 7)]
    [InlineData(typeof(Contour2dResults), 7)]
    [InlineData(typeof(Contour3dResults), 7)]
    [InlineData(typeof(ContourNodeResults), 7)]
    [InlineData(typeof(LoadDiagrams), 4)]
    [InlineData(typeof(ReactionForceDiagrams), 4)]
    [InlineData(typeof(ResultDiagrams), 4)]
    [InlineData(typeof(Create1dElement), 1)] // Section3dPreviewComponent
    [InlineData(typeof(Create1dMember), 1)] // Section3dPreviewDropDownComponent
    public void DropDownComponentTest(Type t, int expectedItems) {
      var comp = (GH_Component)Activator.CreateInstance(t);
      var form = new ContextMenuStrip();
      comp.AppendAdditionalMenuItems(form);
      Assert.Equal(expectedItems, form.Items.Count);
    }
  }
}
