using System;
using Grasshopper.Kernel;
using GsaGH.Components;
using Xunit;

namespace GsaGHTests.CustomComponent {
  [Collection("GrasshopperFixture collection")]
  public class IGH_VariableParameterComponentTests {
    [Theory]
    [InlineData(typeof(GetModelGeometry))]
    [InlineData(typeof(OpenModel))]
    [InlineData(typeof(Edit2dProperty))]
    [InlineData(typeof(EditOffset))]
    [InlineData(typeof(EditSection))]
    [InlineData(typeof(GetSectionModifier))]
    [InlineData(typeof(MaterialProperties))]
    [InlineData(typeof(ProfileDimensions))]
    [InlineData(typeof(SectionProperties))]
    [InlineData(typeof(Edit1dMember))]
    [InlineData(typeof(Edit2dMember))]
    [InlineData(typeof(Edit3dMember))]
    [InlineData(typeof(EditNode))]
    [InlineData(typeof(BeamStrainEnergyDensity))]
    [InlineData(typeof(Contour1dResults))]
    [InlineData(typeof(Contour2dResults))]
    [InlineData(typeof(Contour3dResults))]
    [InlineData(typeof(ContourNodeResults))]
    [InlineData(typeof(GridPlaneSurfaceProperties))]
    public void DropDownComponentTest(Type t) {
      var comp = (IGH_VariableParameterComponent)Activator.CreateInstance(t);
      Assert.False(comp.CanRemoveParameter(GH_ParameterSide.Input, 0));
      Assert.False(comp.CanRemoveParameter(GH_ParameterSide.Output, 0));
      Assert.False(comp.CanInsertParameter(GH_ParameterSide.Input, 0));
      Assert.False(comp.CanInsertParameter(GH_ParameterSide.Output, 0));
    }

    [Theory]
    [InlineData(typeof(GetModelGeometry))]
    [InlineData(typeof(Edit2dProperty))]
    [InlineData(typeof(EditOffset))]
    [InlineData(typeof(EditSection))]
    [InlineData(typeof(GetSectionModifier))]
    [InlineData(typeof(MaterialProperties))]
    [InlineData(typeof(ProfileDimensions))]
    [InlineData(typeof(SectionProperties))]
    [InlineData(typeof(Edit1dMember))]
    [InlineData(typeof(Edit2dMember))]
    [InlineData(typeof(Edit3dMember))]
    [InlineData(typeof(EditNode))]
    [InlineData(typeof(GridPlaneSurfaceProperties))]
    public void CreateDestroyParamIGH_VariableParameterComponentTest(Type t) {
      var comp = (IGH_VariableParameterComponent)Activator.CreateInstance(t);
      Assert.Null(comp.CreateParameter(GH_ParameterSide.Input, 0));
      Assert.False(comp.DestroyParameter(GH_ParameterSide.Input, 0));
    }
  }
}
