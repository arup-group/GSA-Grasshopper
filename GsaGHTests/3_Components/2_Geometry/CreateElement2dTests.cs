using System.Linq;

using Grasshopper.Kernel.Types;

using GsaGH.Components;
using GsaGH.Helpers;
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
    public static PolylineCurve Get2dPolyline() {
      var points = new Point3dList {
        new Point3d(0, 0, 0),
        new Point3d(1, 0, 0),
        new Point3d(1, 1, 0),
        new Point3d(0, 1, 0),
      };
      points.Add(points[0]);
      var pol = new Polyline(points);
      return pol.ToPolylineCurve();
    }

    public static Mesh GetMesh() {
      var mesh = new Mesh();
      mesh.Vertices.Add(new Point3d(0, 0, 0));
      mesh.Vertices.Add(new Point3d(1, 0, 0));
      mesh.Vertices.Add(new Point3d(1, 1, 0));
      mesh.Vertices.Add(new Point3d(0, 1, 0));
      mesh.Faces.AddFace(0, 1, 2, 3);
      return mesh;
    }

    public static GH_OasysComponent ComponentMother(bool isLoadPanel = false) {
      return ComponentMother(isLoadPanel, isLoadPanel);
    }

    public static GH_OasysComponent ComponentMother(bool isCurve, bool propertyIsLoadPanelType) {
      var comp = new Create2dElement();
      comp.CreateAttributes();
      if (isCurve) {
        ComponentTestHelper.SetInput(comp, Get2dPolyline(), 0);
      } else {
        ComponentTestHelper.SetInput(comp, GetMesh(), 0);
      }

      ComponentTestHelper.SetInput(comp,
        ComponentTestHelper.GetOutput(CreateProp2dTests.ComponentMother(propertyIsLoadPanelType)), 1);
      comp.Preview3dSection = true;
      comp.ExpireSolution(true);
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

    [Theory]
    [InlineData(true, 4, 6)]
    [InlineData(false, 10, 8)]
    public void CreateComponentWithSection3dPreviewTest(
      bool isLoadPanel, int expectedVerticesCount, int expectedOutlineCount) {
      var component = (Section3dPreviewComponent)ComponentMother(isLoadPanel);
      var output = (GsaElement2dGoo)ComponentTestHelper.GetOutput(component);
      Assert.Equal(expectedVerticesCount, output.Value.Section3dPreview.Mesh.Vertices.Count);
      Assert.Equal(expectedOutlineCount, output.Value.Section3dPreview.Outlines.Count());
      if (isLoadPanel) {
        var curve = new GH_Curve();
        Assert.True(output.CastTo(ref curve));
      } else {
        var mesh = new GH_Mesh();
        Assert.True(output.CastTo(ref mesh));
      }
    }

    private void TestPoint3d(Point3d expected, Point3d actual) {
      Assert.Equal(expected.X, actual.X, 11);
      Assert.Equal(expected.Y, actual.Y, 11);
      Assert.Equal(expected.Z, actual.Z, 11);
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void WillThrowExceptionIfProp2dIsNotCompatible(bool isCurve, bool isLoadPanel) {
      GH_OasysComponent comp = ComponentMother(isCurve, isLoadPanel);
      ComponentTestHelper.GetOutput(comp);
      Assert.Contains("One runtime error", comp.InstanceDescription);
    }
  }

  [Collection("GrasshopperFixture collection")]
  public class Element2dErrorMessagesTests {
    private readonly GH_OasysComponent _component;

    public Element2dErrorMessagesTests() {
      _component = CreateElement2dTests.ComponentMother(true);
    }

    [Fact]
    public void InvalidPolylineToCreateLoadPanel() {
      ComponentTestHelper.SetInput(_component, new PolylineCurve(), 0);
      ComponentTestHelper.GetOutput(_component);
      Assert.Contains(InvalidGeometryForProperty.CouldNotBeConvertedToPolyline,
        _component.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error)[0]);
    }

    [Fact]
    public void InvalidGeometryToCreateLoadPanel() {
      ComponentTestHelper.SetInput(_component, new Line(), 0);
      ComponentTestHelper.GetOutput(_component);
      Assert.Contains(InvalidGeometryForProperty.NotSupportedType,
        _component.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error)[0]);
    }
  }
}
