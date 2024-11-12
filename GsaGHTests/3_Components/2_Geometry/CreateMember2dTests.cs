using System.Linq;

using Grasshopper.Kernel.Types;

using GsaAPI;

using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Components.Properties;
using GsaGHTests.Helpers;

using OasysGH.Components;

using OasysUnits;

using Rhino.Geometry;

using Xunit;

using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGHTests.Components.Geometry {
  [Collection("GrasshopperFixture collection")]
  public class CreateMember2dTests {
    private readonly GsaMember2dGoo _gsaMember2dGoo;
    private GsaMember2dGoo _gsaMember2dGooFromSection3d;

    public CreateMember2dTests() {
      GH_OasysComponent comp = ComponentMother();

      _gsaMember2dGoo = (GsaMember2dGoo)ComponentTestHelper.GetOutput(comp);
    }

    public static GH_OasysComponent ComponentMother() {
      var comp = new Create2dMember();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(comp,
        Brep.CreateFromCornerPoints(new Point3d(0, 0, 0), new Point3d(10, 0, 0),
          new Point3d(10, 10, 0), new Point3d(0, 10, 0), 1), 0);
      ComponentTestHelper.SetInput(comp,
        ComponentTestHelper.GetOutput(CreateProp2dTests.ComponentMother(false)), 3);
      ComponentTestHelper.SetInput(comp, 0.5, 4);

      return comp;
    }

    private void SetMember2dFromSection3d() {
      if (_gsaMember2dGooFromSection3d != null) {
        return;
      }

      var section3dPreview = (Section3dPreviewDropDownComponent)ComponentMother();
      section3dPreview.Preview3dSection = true;
      section3dPreview.ExpireSolution(true);
      _gsaMember2dGooFromSection3d = (GsaMember2dGoo)ComponentTestHelper.GetOutput(section3dPreview);
    }

    [Fact]
    public void ComponentShouldReturn100AsAreaValue() {
      Assert.Equal(100, _gsaMember2dGoo.Value.Brep.GetArea());
    }

    [Fact]
    public void ComponentShouldReturnPlateTypeAsPropertyValue() {
      Assert.Equal(Property2D_Type.PLATE, _gsaMember2dGoo.Value.Prop2d.ApiProp2d.Type);
    }

    [Fact]
    public void ComponentShouldReturn14InchAsThicknessValue() {
      Assert.Equal(new Length(14, LengthUnit.Inch), _gsaMember2dGoo.Value.Prop2d.Thickness);
    }

    [Fact]
    public void ComponentShouldReturn05AsMeshSizeValue() {
      Assert.Equal(0.5, _gsaMember2dGoo.Value.ApiMember.MeshSize);
    }

    [Fact]
    public void ComponentShouldReturnDefaultGroupValue() {
      Assert.Equal(1, _gsaMember2dGoo.Value.ApiMember.Group);
    }

    [Fact]
    public void CreateComponentWithSection3dPreviewShouldReturn10Vertices() {
      SetMember2dFromSection3d();
      Assert.Equal(4, _gsaMember2dGooFromSection3d.Value.Section3dPreview.Mesh.Vertices.Count);
    }

    [Fact]
    public void CreateComponentWithSection3dPreviewShouldReturn12Outlines() {
      SetMember2dFromSection3d();
      Assert.Equal(7, _gsaMember2dGooFromSection3d.Value.Section3dPreview.Outlines.Count());
    }

    [Fact]
    public void CreateComponentWithSection3dPreviewShouldShouldCastToMesh() {
      SetMember2dFromSection3d();
      var mesh = new GH_Mesh();
      Assert.True(_gsaMember2dGooFromSection3d.CastTo(ref mesh));
    }

    [Fact]
    public void SetInclusionCurveShouldReturnSingleInclusionLine() {
      GH_OasysComponent comp = ComponentMother();
      ComponentTestHelper.SetInput(
        comp, new LineCurve(new Point3d(1, 1, 0), new Point3d(9, 9, 0)), 2);

      var output = (GsaMember2dGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Single(output.Value.InclusionLines);
    }

    [Fact]
    public void SetInclusionCurveShouldReturn2InclusionLines() {
      GH_OasysComponent comp = ComponentMother();
      ComponentTestHelper.SetInput(comp, new LineCurve(new Point3d(1, 1, 0), new Point3d(9, 9, 0)), 2);
      ComponentTestHelper.SetInput(comp, new LineCurve(new Point3d(9, 9, 0), new Point3d(1, 1, 0)), 2);
      var output = (GsaMember2dGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(2, output.Value.InclusionLines.Count);
    }

    [Fact]
    public void SetInclusionPointsShouldReturnSingleInclusionPoint() {
      GH_OasysComponent comp = ComponentMother();
      ComponentTestHelper.SetInput(comp, new Point3d(9, 9, 0), 1);

      var output = (GsaMember2dGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Single(output.Value.InclusionPoints);
    }

    [Fact]
    public void SetInclusionPointsShouldReturn2InclusionPoints() {
      GH_OasysComponent comp = ComponentMother();
      ComponentTestHelper.SetInput(comp, new Point3d(9, 9, 0), 1);
      ComponentTestHelper.SetInput(comp, new Point3d(1, 1, 0), 1);
      var output = (GsaMember2dGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(2, output.Value.InclusionPoints.Count);
    }

    [Fact]
    public void DefaultValueInADropdownShouldReturnMeshModeMixed() {
      Assert.Equal(MeshMode2d.Mixed, _gsaMember2dGoo.Value.ApiMember.MeshMode2d);
    }

    [Fact]
    public void SelectFirstValueInADropdownShouldReturnMeshModeTri() {
      GsaMember2dGoo component = SetSelected(0);
      Assert.Equal(MeshMode2d.Tri, component.Value.ApiMember.MeshMode2d);
    }

    [Fact]
    public void SelectThirdndValueInADropdownShouldReturnMeshModeTri() {
      GsaMember2dGoo component = SetSelected(2);
      Assert.Equal(MeshMode2d.Quad, component.Value.ApiMember.MeshMode2d);
    }

    private GsaMember2dGoo SetSelected(int i) {
      var comp = (GH_OasysDropDownComponent)ComponentMother();
      comp.CreateAttributes();
      comp.SetSelected(0, i);
      return (GsaMember2dGoo)ComponentTestHelper.GetOutput(comp);
    }
  }
}
