using System;
using System.Windows.Forms;
using Grasshopper.Kernel;
using GsaGH.Components;
using GsaGH.Components.GraveyardComp;
using GsaGHTests.Helpers;
using OasysGH.Components;
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
    [InlineData(typeof(GetModelGeometry), 4)]
    [InlineData(typeof(EditNode), 1)]
    [InlineData(typeof(CreateGridAreaLoad), 3)]
    [InlineData(typeof(CreateGridLineLoad), 3)]
    [InlineData(typeof(CreateGridPointLoad), 3)]
    [InlineData(typeof(Contour1dResults), 5)]
    [InlineData(typeof(Contour2dResults), 5)]
    [InlineData(typeof(Contour3dResults), 5)]
    [InlineData(typeof(ContourNodeResults), 5)]
    [InlineData(typeof(LoadDiagrams), 2)]
    [InlineData(typeof(ReactionForceDiagrams), 2)]
    [InlineData(typeof(ResultDiagrams), 2)]
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
