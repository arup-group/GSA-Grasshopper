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
  public class IGH_VariableParameterComponentTests {
    [Theory]
    [InlineData(typeof(GetSectionModifier))]
    [InlineData(typeof(Edit1dMember))]
    [InlineData(typeof(Edit2dMember))]
    [InlineData(typeof(Edit2dProperty))]
    [InlineData(typeof(Edit3dMember))]
    [InlineData(typeof(SectionProperties))]
    [InlineData(typeof(ProfileDimensions))]
    [InlineData(typeof(MaterialProperties))]
    [InlineData(typeof(GridPlaneSurfaceProperties))]
    [InlineData(typeof(EditSection))]
    [InlineData(typeof(EditOffset))]
    [InlineData(typeof(EditNode))]
    [InlineData(typeof(GetModelGeometry))]
    public void DropDownComponentTest(Type t) {
      var comp = (IGH_VariableParameterComponent)Activator.CreateInstance(t);
      Assert.False(comp.CanRemoveParameter(GH_ParameterSide.Input, 0));
      Assert.False(comp.CanRemoveParameter(GH_ParameterSide.Output, 0));
      Assert.False(comp.CanInsertParameter(GH_ParameterSide.Input, 0));
      Assert.False(comp.CanInsertParameter(GH_ParameterSide.Output, 0));
    }

    [Theory]
    [InlineData(typeof(GetModelGeometry))]
    [InlineData(typeof(EditNode))]
    public void CreateDestroyParamIGH_VariableParameterComponentTest(Type t) {
      var comp = (IGH_VariableParameterComponent)Activator.CreateInstance(t);
      Assert.Null(comp.CreateParameter(GH_ParameterSide.Input, 0));
      Assert.False(comp.DestroyParameter(GH_ParameterSide.Input, 0));
    }
  }
}
