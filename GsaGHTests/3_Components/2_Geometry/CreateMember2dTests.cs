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

    [Fact]
    public void CreateComponentTest() {
      GH_OasysComponent comp = ComponentMother();

      var output = (GsaMember2dGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(100, output.Value.Brep.GetArea());
      Assert.Equal(Property2D_Type.PLATE, output.Value.Prop2d.ApiProp2d.Type);
      Assert.Equal(new Length(14, LengthUnit.Inch), output.Value.Prop2d.Thickness);
      Assert.Equal(0.5, output.Value.ApiMember.MeshSize);
      Assert.Equal(1, output.Value.ApiMember.Group);
    }

    [Fact]
    public void CreateComponentWithSection3dPreviewTest() {
      var comp = (Section3dPreviewDropDownComponent)ComponentMother();
      comp.Preview3dSection = true;
      comp.ExpireSolution(true);

      var output = (GsaMember2dGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(10, output.Value.Section3dPreview.Mesh.Vertices.Count);
      Assert.Equal(12, output.Value.Section3dPreview.Outlines.Count());

      var mesh = new GH_Mesh();
      Assert.True(output.CastTo(ref mesh));
    }

    [Fact]
    public void InclusionLineTest() {
      GH_OasysComponent comp = ComponentMother();
      ComponentTestHelper.SetInput(
        comp, new LineCurve(new Point3d(1, 1, 0), new Point3d(9, 9, 0)), 2);

      var output = (GsaMember2dGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Single(output.Value.InclusionLines);

      ComponentTestHelper.SetInput(
        comp, new LineCurve(new Point3d(9, 9, 0), new Point3d(1, 1, 0)), 2);
      output = (GsaMember2dGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(2, output.Value.InclusionLines.Count);
    }

    [Fact]
    public void InclusionPointTest() {
      GH_OasysComponent comp = ComponentMother();
      ComponentTestHelper.SetInput(comp, new Point3d(9, 9, 0), 1);

      var output = (GsaMember2dGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Single(output.Value.InclusionPoints);

      ComponentTestHelper.SetInput(comp, new Point3d(1, 1, 0), 1);
      output = (GsaMember2dGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(2, output.Value.InclusionPoints.Count);
    }

    [Fact]
    public void MeshModeTest() {
      var comp = (GH_OasysDropDownComponent)ComponentMother();
      comp.CreateAttributes();
      var output = (GsaMember2dGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(MeshMode2d.Mixed, output.Value.ApiMember.MeshMode2d);

      comp.SetSelected(0, 0); // tri mode
      output = (GsaMember2dGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(MeshMode2d.Tri, output.Value.ApiMember.MeshMode2d);

      comp.SetSelected(0, 2); // quad mode
      output = (GsaMember2dGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(MeshMode2d.Quad, output.Value.ApiMember.MeshMode2d);
    }
  }
}
