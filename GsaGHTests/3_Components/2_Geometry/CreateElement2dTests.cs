﻿using System.Linq;
using System.Security.Cryptography;

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
    static public PolylineCurve Get2dPolyline() {
      var points = new Point3dList {
        new Point3d(0, 0, 0),
        new Point3d(0, 5, 0),
        new Point3d(5, 5, 0),
        new Point3d(5, 0, 0),
      };
      points.Add(points[0]);
      var pol = new Polyline(points);
      return pol.ToPolylineCurve();
    }

    static public Mesh GetMesh() {
      var mesh = new Mesh();
      mesh.Vertices.Add(new Point3d(0, 0, 0));
      mesh.Vertices.Add(new Point3d(1, 0, 0));
      mesh.Vertices.Add(new Point3d(1, 1, 0));
      mesh.Vertices.Add(new Point3d(0, 1, 0));
      mesh.Faces.AddFace(0, 1, 2, 3);
      return mesh;
    }

    public static GH_OasysComponent ComponentMother(bool isCurve = false, bool propertyIsLoadPanelType = false) {
      var comp = new Create2dElement();
      comp.CreateAttributes();
      if (isCurve) {
        ComponentTestHelper.SetInput(
          comp, Get2dPolyline(), 0);
      } else {
        ComponentTestHelper.SetInput(
          comp, GetMesh(), 0);
      }
      ComponentTestHelper.SetInput(comp,
       ComponentTestHelper.GetOutput(CreateProp2dTests.ComponentMother(propertyIsLoadPanelType)), 1);
      return comp;
    }

    public static GH_OasysComponent ComponentMotherLoadPanel(bool setGeometry = true) {
      var comp = new Create2dElement();
      comp.CreateAttributes();
      if (setGeometry) {
        ComponentTestHelper.SetInput(
         comp, Get2dPolyline(), 0);
      }
      ComponentTestHelper.SetInput(comp,
       ComponentTestHelper.GetOutput(CreateProp2dTests.ComponentMother(true)), 1);
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
    public void CreateComponentWithSection3dPreviewTest(bool isLoadPanel, int expectedVerticesCount, int expectedOutlineCount) {
      var comp = (Section3dPreviewComponent)ComponentMother(isLoadPanel, isLoadPanel);
      comp.Preview3dSection = true;
      comp.ExpireSolution(true);
      var output = (GsaElement2dGoo)ComponentTestHelper.GetOutput(comp);
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

    [Fact]
    public void InvalidPolylineToCreateLoadPanel() {
      GH_OasysComponent comp = ComponentMotherLoadPanel(false);
      var curve = new PolylineCurve();
      curve.SetPoint(0, new Point3d(0, 0, 0));
      curve.SetPoint(0, new Point3d(0, 1, 0));
      ComponentTestHelper.SetInput(
         comp, curve, 0);
      ComponentTestHelper.GetOutput(comp);
      Assert.Contains("Polyline could not be extracted from the given curve geometry", comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error)[0]);

    }

    [Fact]
    public void InvalidGeometryToCreateLoadPanel() {
      GH_OasysComponent comp = ComponentMotherLoadPanel(false);
      var curve = new Line(new Point3d(0, 0, 0), new Point3d(0, 1, 0));
      ComponentTestHelper.SetInput(
         comp, curve, 0);
      ComponentTestHelper.GetOutput(comp);
      Assert.Contains("Input geometry is not supported to create a 2D element", comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error)[0]);

    }
  }
}
