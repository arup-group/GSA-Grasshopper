using Grasshopper.Kernel;

using GsaAPI;

using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Helpers;

using Xunit;

namespace GsaGHTests.Components.Properties {
  [Collection("GrasshopperFixture collection")]
  public class LoadPanelPropertyTests {

    private readonly Create2dProperty _component;

    public LoadPanelPropertyTests() {
      _component = CreateProp2dTests.GetLoadPanelProperty();
    }

    [Fact]
    public void CreateLoadComponent() {
      var output = (GsaProperty2dGoo)ComponentTestHelper.GetOutput(_component);
      Assert.Equal(Property2D_Type.LOAD, output.Value.ApiProp2d.Type);
      Assert.Equal(SupportType.TwoEdges, output.Value.ApiProp2d.SupportType);
      Assert.Equal(2, output.Value.ApiProp2d.ReferenceEdge);
    }

    // This is required as the SupportType enum (being from c++) cannot be used
    // as an inlineData parameter, due to the way xUnit works
    public enum LocalSupportType {
      Undefined,
      Auto,
      AllEdges,
      ThreeEdges,
      TwoEdges,
      TwoAdjacentEdges,
      OneEdge,
      Cantilever,
      OneWay,
      TwoWay,
    }

    [Theory]
    [InlineData(LocalSupportType.Auto)]
    [InlineData(LocalSupportType.AllEdges)]
    [InlineData(LocalSupportType.ThreeEdges)]
    [InlineData(LocalSupportType.TwoEdges)]
    [InlineData(LocalSupportType.TwoAdjacentEdges)]
    [InlineData(LocalSupportType.OneEdge)]
    [InlineData(LocalSupportType.Cantilever)]
    public void ShouldAddWarningForLegacy(LocalSupportType supportType) {
      SetLoadPanelSupportType(supportType);
      ComponentTestHelper.ComputeOutput(_component);
      Assert.Contains(Create2dProperty.UseOfLegacyTypeWarningMessage,
        _component.RuntimeMessages(GH_RuntimeMessageLevel.Warning));
    }

    private void SetLoadPanelSupportType(LocalSupportType supportType) {
      _component.SetLoadPanelSupportType((SupportType)supportType);
    }

    [Theory]
    [InlineData(LocalSupportType.OneWay)]
    [InlineData(LocalSupportType.TwoWay)]
    public void ShouldNotHaveWarningForLegacy(LocalSupportType supportType) {
      SetLoadPanelSupportType(supportType);
      ComponentTestHelper.ComputeOutput(_component);
      Assert.DoesNotContain(Create2dProperty.UseOfLegacyTypeWarningMessage,
        _component.RuntimeMessages(GH_RuntimeMessageLevel.Warning));
    }

    [Fact]
    public void ShouldBeAbleToSetDropdownBySupportType() {
      _component.SetLoadPanelSupportType(SupportType.OneWay);
      Assert.Equal(SupportType.OneWay, _component.GetLoadPanelSupportType());
    }

    [Fact]
    public void ShouldBeAbleToRetrieveTheRightSupportType() {
      ComponentTestHelper.ComputeOutput(_component);
      Assert.Equal(SupportType.TwoEdges, _component.GetLoadPanelSupportType());
    }
  }
}
