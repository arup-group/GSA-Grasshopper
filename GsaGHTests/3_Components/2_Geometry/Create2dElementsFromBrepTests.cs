using System.Collections.Generic;

using GsaAPI;

using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Components.Properties;
using GsaGHTests.Helpers;

using OasysGH.Components;

using Rhino.Geometry;

using Xunit;

namespace GsaGHTests.Components.Geometry {
  [Collection("GrasshopperFixture collection")]
  public class Create2dElementsFromBrepTests {
    public static Create2dElementsFromBrep ComponentMother(bool isLoadPanel = false) {
      var comp = new Create2dElementsFromBrep();
      comp.CreateAttributes();

      var brep = Brep.CreateFromCornerPoints(new Point3d(0, 0, 0), new Point3d(10, 0, 2.5),
          new Point3d(10, 10, 0), new Point3d(0, 10, 1.5), 1);

      var pts = new List<Point3d> {
        brep.Surfaces[0].PointAt(0.1, 0.9),
        brep.Surfaces[0].PointAt(0.9, 0.1),
      };

      var crvUVs = new List<Point2d> {
        new Point2d(1, 1),
        new Point2d(5, 5),
        new Point2d(9, 9)
      };
      Curve crv = brep.Surfaces[0].InterpolatedCurveOnSurfaceUV(crvUVs, 1);

      ComponentTestHelper.SetInput(comp, brep, 0);
      ComponentTestHelper.SetInput(comp, pts, 1);
      ComponentTestHelper.SetInput(comp, crv, 2);

      ComponentTestHelper.SetInput(comp,
       ComponentTestHelper.GetOutput(CreateProp2dTests.ComponentMother(isLoadPanel)), 3);
      return comp;
    }

    public static Create2dElementsFromBrep ComponentFather() {
      var comp = new Create2dElementsFromBrep();
      comp.CreateAttributes();

      var brep = Brep.CreateFromCornerPoints(new Point3d(0, 0, 0), new Point3d(10, 0, 2.5),
          new Point3d(10, 10, 0), new Point3d(0, 10, 1.5), 1);

      var node = new GsaNode(brep.Surfaces[0].PointAt(0.1, 0.9));

      var crvUVs = new List<Point2d> {
        new Point2d(1, 1),
        new Point2d(5, 5),
        new Point2d(9, 9)
      };
      var mem1d = new GsaMember1d(brep.Surfaces[0].InterpolatedCurveOnSurfaceUV(crvUVs, 1));

      var crvUVs2 = new List<Point2d> {
        new Point2d(0, 0),
        new Point2d(0, 1),
      };
      Curve edge = brep.Surfaces[0].InterpolatedCurveOnSurfaceUV(crvUVs, 1);
      var e1d = new GsaElement1d(new LineCurve(edge.PointAtStart, edge.PointAtEnd));

      ComponentTestHelper.SetInput(comp, brep, 0);
      ComponentTestHelper.SetInput(comp, new GsaNodeGoo(node), 1);
      ComponentTestHelper.SetInput(comp, new GsaElement1dGoo(e1d), 2);
      ComponentTestHelper.SetInput(comp, new GsaMember1dGoo(mem1d), 2);
      return comp;
    }

    [Fact]
    public void CreateComponentWithRhinoInclusionGeometryTest() {
      GH_OasysComponent comp = ComponentMother();
      var e2d = (GsaElement2dGoo)ComponentTestHelper.GetOutput(comp);

      Assert.NotNull(e2d);
      Assert.Equal(230, e2d.Value.Ids.Count);
      Assert.Equal(230, e2d.Value.Mesh.Faces.Count);

      for (int i = 0; i < e2d.Value.Mesh.Faces.Count; i++) {
        if (e2d.Value.Mesh.Faces[i].IsTriangle) {
          Assert.Equal(ElementType.TRI6, e2d.Value.ApiElements[i].Type);
        } else {
          Assert.Equal(ElementType.QUAD8, e2d.Value.ApiElements[i].Type);
        }
      }
    }

    [Fact]
    public void CreateComponentWithGsaGhInclusionGeometryTest() {
      GH_OasysComponent comp = ComponentFather();
      var e2d = (GsaElement2dGoo)ComponentTestHelper.GetOutput(comp, 0);

      Assert.NotNull(e2d);
      Assert.Equal(240, e2d.Value.Ids.Count);
      Assert.Equal(240, e2d.Value.Mesh.Faces.Count);

      for (int i = 0; i < e2d.Value.Mesh.Faces.Count; i++) {
        if (e2d.Value.Mesh.Faces[i].IsTriangle) {
          Assert.Equal(ElementType.TRI6, e2d.Value.ApiElements[i].Type);
        } else {
          Assert.Equal(ElementType.QUAD8, e2d.Value.ApiElements[i].Type);
        }
      }

      var node = (GsaNodeGoo)ComponentTestHelper.GetOutput(comp, 1);
      Assert.NotNull(node);
      var e1d = (GsaElement1dGoo)ComponentTestHelper.GetOutput(comp, 2);
      Assert.NotNull(e1d);
    }

    [Fact]
    public void Tri6MeshModeTest() {
      var comp = new Create2dElementsFromBrep();
      comp.CreateAttributes();
      var brep = Brep.CreateFromCornerPoints(new Point3d(0, 0, 0), new Point3d(10, 0, 2.5),
          new Point3d(10, 10, 0), new Point3d(0, 10, 1.5), 1);
      ComponentTestHelper.SetInput(comp, brep, 0);
      comp.SetSelected(0, 0); // tri-6 only
      var e2d = (GsaElement2dGoo)ComponentTestHelper.GetOutput(comp);

      Assert.NotNull(e2d);
      Assert.Equal(230, e2d.Value.Ids.Count);
      Assert.Equal(230, e2d.Value.Mesh.Faces.Count);
      foreach (GSAElement elem in e2d.Value.ApiElements) {
        Assert.Equal(ElementType.TRI6, elem.Type);
      }
    }

    [Fact]
    public void Quad8MeshModeTest() {
      var comp = new Create2dElementsFromBrep();
      comp.CreateAttributes();
      var brep = Brep.CreateFromCornerPoints(new Point3d(0, 0, 0), new Point3d(10, 0, 5),
      new Point3d(10, 10, 0), new Point3d(0, 10, 5), 1);
      ComponentTestHelper.SetInput(comp, brep, 0);
      ComponentTestHelper.SetInput(comp, 1.75, 4); // mesh size
      comp.SetSelected(0, 2); // quad-8 only
      var e2d = (GsaElement2dGoo)ComponentTestHelper.GetOutput(comp);

      Assert.NotNull(e2d);
      Assert.Equal(96, e2d.Value.Ids.Count);
      Assert.Equal(96, e2d.Value.Mesh.Faces.Count);
      foreach (GSAElement elem in e2d.Value.ApiElements) {
        Assert.Equal(ElementType.QUAD8, elem.Type);
      }
    }

    [Fact]
    public void DrawViewportMeshesAndWiresTest() {
      GH_OasysComponent comp = ComponentMother();
      var e2d = (GsaElement2dGoo)ComponentTestHelper.GetOutput(comp);
      ComponentTestHelper.DrawViewportMeshesAndWiresTest(comp);
    }

    [Fact]
    public void ErrorAndWarningsNullTest() {
      var comp = new Create2dElementsFromBrep();
      comp.CreateAttributes();
      Brep brep = null;
      ComponentTestHelper.SetInput(comp, brep);
      comp.ExpireSolution(true);
      comp.Params.Output[0].ExpireSolution(true);
      comp.Params.Output[0].CollectData();

      Assert.Single(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void NoErrorAndWarningsMultipleSrfsTest() {
      var comp = new Create2dElementsFromBrep();
      comp.CreateAttributes();
      var brep = Brep.CreateFromCornerPoints(new Point3d(0, 0, 0), new Point3d(10, 0, 2.5),
          new Point3d(10, 10, 0), new Point3d(0, 10, 1.5), 1);
      var b2 = (Surface)brep.Surfaces[0].Duplicate();
      b2.Transform(Transform.Translation(new Vector3d(0, 0, 5)));
      brep.AddSurface(b2.Rebuild(3, 3, 10, 10));
      ComponentTestHelper.SetInput(comp, brep);
      comp.ExpireSolution(true);
      comp.Params.Output[0].ExpireSolution(true);
      comp.Params.Output[0].CollectData();

      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void RemarkPlanarSrfTest() {
      var comp = new Create2dElementsFromBrep();
      comp.CreateAttributes();
      var brep = Brep.CreateFromCornerPoints(new Point3d(0, 0, 0), new Point3d(10, 0, 0),
          new Point3d(10, 10, 0), new Point3d(0, 10, 0), 1);
      ComponentTestHelper.SetInput(comp, brep);
      comp.ExpireSolution(true);
      comp.Params.Output[0].ExpireSolution(true);
      comp.Params.Output[0].CollectData();

      Assert.Equal(2, comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Remark).Count);
    }

    [Fact]
    public void RuntimeErrorWhenLoadpanelPropertyIsAssigned() {
      GH_OasysComponent comp = ComponentMother(true);
      ComponentTestHelper.GetOutput(comp);
      Assert.Contains(Create2dElementsFromBrep.DoesNotSupportLoadPanelErrorMessage, comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error)[1]);
    }
  }
}
