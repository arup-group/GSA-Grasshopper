using GsaGH.Helpers;

using GsaGHTests.Helpers;

using OasysGH.Components;

using Rhino.Geometry;

using Xunit;

namespace GsaGHTests.Components.Geometry
{
  [Collection("GrasshopperFixture collection")]
  public class Element2dErrorMessagesTests {
    private GH_OasysComponent _component;

    public Element2dErrorMessagesTests() {
      _component = CreateElement2dTests.ComponentMother(true);
    }

    [Fact]
    public void FeAnalysisShouldNotWorkWithCurve() {
      _component = CreateElement2dTests.ComponentMother(false);
      ComponentTestHelper.SetInput(_component, new PolyCurve(), 0);
      ComponentTestHelper.ComputeOutput(_component);
      Assert.Contains(InvalidGeometryForProperty.WrongGeometryTypeForAnalysis,
        _component.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void MeshShouldNotWorkForLoadPanel() {
      ComponentTestHelper.SetInput(_component, new Mesh(), 0);
      ComponentTestHelper.ComputeOutput(_component);
      Assert.Contains(InvalidGeometryForProperty.WrongGeometryTypeForLoadPanel,
        _component.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void InvalidPolylineToCreateLoadPanel() {
      ComponentTestHelper.SetInput(_component, new PolylineCurve(), 0);
      ComponentTestHelper.ComputeOutput(_component);
      Assert.Contains(InvalidGeometryForProperty.CouldNotBeConvertedToPolyline,
        _component.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void InvalidGeometryToCreateLoadPanel() {
      ComponentTestHelper.SetInput(_component, new Line(), 0);
      ComponentTestHelper.ComputeOutput(_component);
      Assert.Contains(InvalidGeometryForProperty.NotSupportedType,
        _component.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error));
    }
  }
}
