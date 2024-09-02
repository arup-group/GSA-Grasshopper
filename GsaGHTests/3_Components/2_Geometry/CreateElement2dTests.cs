using System.Linq;

using Grasshopper.Kernel.Types;

using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Components.Properties;
using GsaGHTests.Helpers;

using OasysGH.Components;

using OasysUnits;
using OasysUnits.Units;

using Rhino.Collections;
using Rhino.Geometry;

using Xunit;



namespace GsaGHTests.Components.Geometry {
  [Collection("GrasshopperFixture collection")]
  public class CreateElement2dTests {

    public static GH_OasysComponent ComponentMother(bool isCurve = false, bool isLoadPanel = false) {
      var comp = new Create2dElement();

      comp.CreateAttributes();
      if (isCurve) {
        var points = new Point3dList {
        new Point3d(0, 0, 0),
        new Point3d(1, 0, 0),
        new Point3d(1, 1, 0),
        new Point3d(0, 1, 0),
      };
        points.Add(points[0]);
        var polyline = new Rhino.Geometry.Polyline(points);
        ComponentTestHelper.SetInput(
          comp, polyline.ToPolylineCurve(), 0);
      } else {
        var mesh = new Mesh();
        mesh.Vertices.Add(new Point3d(0, 0, 0));
        mesh.Vertices.Add(new Point3d(1, 0, 0));
        mesh.Vertices.Add(new Point3d(1, 1, 0));
        mesh.Vertices.Add(new Point3d(0, 1, 0));
        mesh.Faces.AddFace(0, 1, 2, 3);
        ComponentTestHelper.SetInput(
          comp, mesh, 0);
      }

      ComponentTestHelper.SetInput(comp,
       ComponentTestHelper.GetOutput(CreateProp2dTests.ComponentMother(isLoadPanel)), 1);

      return comp;
    }

    [Fact]
    public void CreateComponentTest() {
      GH_OasysComponent comp = ComponentMother();

      var output = (GsaElement2dGoo)ComponentTestHelper.GetOutput(comp);
      TestPoint3d(new Point3d(0, 0, 0), output.Value.Mesh.Vertices[0]);
      TestPoint3d(new Point3d(1, 0, 0), output.Value.Mesh.Vertices[1]);
      TestPoint3d(new Point3d(1, 1, 0), output.Value.Mesh.Vertices[2]);
      TestPoint3d(new Point3d(0, 1, 0), output.Value.Mesh.Vertices[3]);
      Assert.Equal(new Length(14, LengthUnit.Inch), output.Value.Prop2ds[0].Thickness);
    }

    [Fact]
    public void CreateComponentWithSection3dPreviewTest() {
      var comp = (Section3dPreviewComponent)ComponentMother();
      comp.Preview3dSection = true;
      comp.ExpireSolution(true);

      var output = (GsaElement2dGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(10, output.Value.Section3dPreview.Mesh.Vertices.Count);
      Assert.Equal(8, output.Value.Section3dPreview.Outlines.Count());

      var mesh = new GH_Mesh();
      Assert.True(output.CastTo(ref mesh));
    }

    private void TestPoint3d(Point3d expected, Point3d actual) {
      Assert.Equal(expected.X, actual.X, 11);
      Assert.Equal(expected.Y, actual.Y, 11);
      Assert.Equal(expected.Z, actual.Z, 11);
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void WillThrowExceptionIfProp2dIsNotCompatible(bool isCurve, bool isLoadPanel) {
      GH_OasysComponent comp = ComponentMother(isCurve, isLoadPanel);
      ComponentTestHelper.GetOutput(comp);
      if ((isCurve && isLoadPanel) || (!isCurve && !isLoadPanel)) {
        Assert.DoesNotContain("One runtime error", comp.InstanceDescription);
      } else {
        Assert.Contains("One runtime error", comp.InstanceDescription);
      }
    }

  }
}
