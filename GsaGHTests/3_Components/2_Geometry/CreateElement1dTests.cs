using System.Linq;


using Grasshopper.Kernel.Types;


using GsaGH.Components;

using GsaGH.Parameters;


using GsaGHTests.Helpers;


using OasysGH.Components;


using Rhino.Geometry;


using GsaGH.Helpers;

using Xunit;
namespace GsaGHTests.Components.Geometry {
  [Collection("GrasshopperFixture collection")]
  public class CreateElement1dTests {
    private readonly GsaElement1dGoo _element1dGoo;
    private GsaElement1dGoo _element1dGooFromSection3d;

    public CreateElement1dTests() {
      GH_OasysComponent comp = ComponentMother();
      _element1dGoo = (GsaElement1dGoo)ComponentTestHelper.GetOutput(comp);
    }

    public static GH_OasysComponent ComponentMother() {
      var comp = new Create1dElement();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(comp, new LineCurve(new Point3d(0, -1, 0), new Point3d(7, 3, 1)), 0);
      ComponentTestHelper.SetInput(comp, "STD CH(ft) 1 2 3 4", 1);

      return comp;
    }

    private void SetElement1dFromSection3d() {
      if (_element1dGooFromSection3d != null) {
        return;
      }

      var section3dPreview = (Section3dPreviewComponent)ComponentMother();
      section3dPreview.Preview3dSection = true;
      section3dPreview.ExpireSolution(true);
      _element1dGooFromSection3d = (GsaElement1dGoo)ComponentTestHelper.GetOutput(section3dPreview);
    }

    [Fact]
    public void ComponentShouldReturn0AsPointAtStartXValue() {
      Assert.Equal(0, _element1dGoo.Value.Line.PointAtStart.X, DoubleComparer.Default);
    }

    [Fact]
    public void ComponentShouldReturnMinus1AsPointAtStartYValue() {
      Assert.Equal(-1, _element1dGoo.Value.Line.PointAtStart.Y, DoubleComparer.Default);
    }

    [Fact]
    public void ComponentShouldReturn0AsPointAtStartZValue() {
      Assert.Equal(0, _element1dGoo.Value.Line.PointAtStart.Z, DoubleComparer.Default);
    }

    [Fact]
    public void ComponentShouldReturn7AsPointAtEndXValue() {
      Assert.Equal(7, _element1dGoo.Value.Line.PointAtEnd.X, DoubleComparer.Default);
    }

    [Fact]
    public void ComponentShouldReturn3AsPointAtEndYValue() {
      Assert.Equal(3, _element1dGoo.Value.Line.PointAtEnd.Y, DoubleComparer.Default);
    }

    [Fact]
    public void ComponentShouldReturn1AsPointAtEndZValue() {
      Assert.Equal(1, _element1dGoo.Value.Line.PointAtEnd.Z, DoubleComparer.Default);
    }

    [Fact]
    public void ComponentShouldReturnCorrectProfile() {
      Assert.Equal("STD CH(ft) 1 2 3 4", _element1dGoo.Value.Section.ApiSection.Profile);
    }

    [Fact]
    public void ComponentShouldReturnDefaultGroupValue() {
      Assert.Equal(1, _element1dGoo.Value.ApiElement.Group);
    }

    [Fact]
    public void CreateComponentWithSection3dPreviewShouldReturn16Vertices() {
      SetElement1dFromSection3d();
      Assert.Equal(16, _element1dGooFromSection3d.Value.Section3dPreview.Mesh.Vertices.Count);
    }

    [Fact]
    public void CreateComponentWithSection3dPreviewShouldReturn24Outlines() {
      SetElement1dFromSection3d();
      Assert.Equal(24, _element1dGooFromSection3d.Value.Section3dPreview.Outlines.Count());
    }

    [Fact]
    public void CreateComponentWithSection3dPreviewShouldCastToMesh() {
      SetElement1dFromSection3d();
      var mesh = new GH_Mesh();
      Assert.True(_element1dGooFromSection3d.CastTo(ref mesh));
    }
  }
}
