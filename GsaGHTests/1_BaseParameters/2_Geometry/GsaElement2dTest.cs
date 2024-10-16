using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

using Grasshopper;

using GsaAPI;

using GsaGH.Helpers;
using GsaGH.Parameters;

using GsaGHTests.Helpers;

using Rhino.Collections;
using Rhino.Geometry;

using Xunit;

using LengthUnit = OasysUnits.Units.LengthUnit;
using Polyline = Rhino.Geometry.Polyline;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaElement2dTests {

    [Fact]
    public void DuplicateTest() {
      var original = new GsaElement2d(new Mesh());
      var duplicate = new GsaElement2d(original);
      Duplicates.AreEqual(original, duplicate, new List<string>() { "Guid" });
    }

    [Fact]
    public void TestCreateGsaElem2dFromMesh() {
      Mesh mesh = ToMesh(new Polyline(Point3dList1()));

      var elem = new GsaElement2d(mesh);
      int elid = 14;
      int secid = 3;
      var grps = new List<int>();
      var dum = new List<bool>();
      var nms = new List<string>();
      var off = new List<GsaOffset>();
      elem.Prop2ds = new List<GsaProperty2d>();
      for (int i = 0; i < elem.ApiElements.Count; i++) {
        elem.Ids[i] = elid++;
        grps.Add(22);
        dum.Add(true);
        nms.Add("Shahin");
        off.Add(new GsaOffset(0, 0, 0, 0.1, LengthUnit.Meter));
        elem.Prop2ds.Add(new GsaProperty2d(secid));
      }

      elem.ApiElements.SetMembers(grps);
      elem.ApiElements.SetMembers(dum);
      elem.ApiElements.SetMembers(nms);
      elem.ApiElements.SetMembers(off);

      for (int i = 0; i < elem.Topology.Count; i++) {
        Assert.Equal(mesh.Vertices[i].X, elem.Topology[i].X);
        Assert.Equal(mesh.Vertices[i].Y, elem.Topology[i].Y);
        Assert.Equal(mesh.Vertices[i].Z, elem.Topology[i].Z);
      }

      int chelid = 14;
      int chsecid = 3;
      for (int i = 0; i < elem.ApiElements.Count; i++) {
        if (mesh.Faces[i].IsTriangle) {
          Assert.True(elem.ApiElements[i].Type == ElementType.TRI3);
        }

        if (mesh.Faces[i].IsQuad) {
          Assert.True(elem.ApiElements[i].Type == ElementType.QUAD4);
        }

        Point3d mPt = mesh.Vertices[mesh.Faces[i].A];
        Point3d ePt = elem.Topology[elem.TopoInt[i][0]]; // topology first pt
        Assert.Equal(mPt.X, ePt.X);
        Assert.Equal(mPt.Y, ePt.Y);
        Assert.Equal(mPt.Z, ePt.Z);

        mPt = mesh.Vertices[mesh.Faces[i].B];
        ePt = elem.Topology[elem.TopoInt[i][1]]; // topology second pt
        Assert.Equal(mPt.X, ePt.X);
        Assert.Equal(mPt.Y, ePt.Y);
        Assert.Equal(mPt.Z, ePt.Z);

        mPt = mesh.Vertices[mesh.Faces[i].C];
        ePt = elem.Topology[elem.TopoInt[i][2]]; // topology third pt
        Assert.Equal(mPt.X, ePt.X);
        Assert.Equal(mPt.Y, ePt.Y);
        Assert.Equal(mPt.Z, ePt.Z);

        if (elem.ApiElements[i].Type == ElementType.QUAD4) {
          mPt = mesh.Vertices[mesh.Faces[i].D];
          ePt = elem.Topology[elem.TopoInt[i][3]]; // topology fourth pt
          Assert.Equal(mPt.X, ePt.X);
          Assert.Equal(mPt.Y, ePt.Y);
          Assert.Equal(mPt.Z, ePt.Z);
        }

        Assert.Equal(chelid++, elem.Ids[i]);
        Assert.Equal(chsecid, elem.Prop2ds[i].Id);
        Assert.Equal(22, elem.ApiElements[i].Group);
        Assert.True(elem.ApiElements[i].IsDummy);
        Assert.Equal("Shahin", elem.ApiElements[i].Name);
        Assert.Equal(0.1, elem.Offsets[i].Z.Value);
      }
    }

    private static Point3dList Point3dList1() {
      var pts = new Point3dList {
        new Point3d(-3, -4, 0),
        new Point3d(5, -2, 0),
        new Point3d(6, 7, 0),
        new Point3d(-1, 2, 0),
      };
      pts.Add(pts[0]);
      return pts;
    }

    [Fact]
    public void TestDuplicateElem2d() {
      Mesh mesh = ToMesh(new Polyline(Point3dList2()));

      var origi = new GsaElement2d(mesh) { Prop2ds = new List<GsaProperty2d>() };
      int elid = 3;
      int secid = 4;
      var grps = new List<int>();
      var dum = new List<bool>();
      var nms = new List<string>();
      var off = new List<GsaOffset>();
      for (int i = 0; i < origi.ApiElements.Count; i++) {
        origi.Ids[i] = elid++;
        origi.Prop2ds.Add(new GsaProperty2d(secid++));
        grps.Add(2);
        dum.Add(false);
        nms.Add("Esmaeil");
        off.Add(new GsaOffset(0, 0, 0, -0.15, LengthUnit.Meter));
      }

      origi.ApiElements.SetMembers(grps);
      origi.ApiElements.SetMembers(dum);
      origi.ApiElements.SetMembers(nms);
      origi.ApiElements.SetMembers(off);

      var dup = new GsaElement2d(origi);

      for (int i = 0; i < dup.Topology.Count; i++) {
        Assert.Equal(mesh.Vertices[i].X, dup.Topology[i].X);
        Assert.Equal(mesh.Vertices[i].Y, dup.Topology[i].Y);
        Assert.Equal(mesh.Vertices[i].Z, origi.Topology[i].Z);
      }

      for (int i = 0; i < dup.ApiElements.Count; i++) {
        if (mesh.Faces[i].IsTriangle) {
          Assert.True(dup.ApiElements[i].Type == ElementType.TRI3);
        }

        if (mesh.Faces[i].IsQuad) {
          Assert.True(dup.ApiElements[i].Type == ElementType.QUAD4);
        }

        Point3d mPt = mesh.Vertices[mesh.Faces[i].A];
        Point3d ePt = dup.Topology[dup.TopoInt[i][0]]; // topology first pt
        Assert.Equal(mPt.X, ePt.X);
        Assert.Equal(mPt.Y, ePt.Y);
        Assert.Equal(mPt.Z, ePt.Z);

        mPt = mesh.Vertices[mesh.Faces[i].B];
        ePt = dup.Topology[dup.TopoInt[i][1]]; // topology second pt
        Assert.Equal(mPt.X, ePt.X);
        Assert.Equal(mPt.Y, ePt.Y);
        Assert.Equal(mPt.Z, ePt.Z);

        mPt = mesh.Vertices[mesh.Faces[i].C];
        ePt = dup.Topology[dup.TopoInt[i][2]]; // topology third pt
        Assert.Equal(mPt.X, ePt.X);
        Assert.Equal(mPt.Y, ePt.Y);
        Assert.Equal(mPt.Z, ePt.Z);

        if (dup.ApiElements[i].Type != ElementType.QUAD4) {
          continue;
        }

        mPt = mesh.Vertices[mesh.Faces[i].D];
        ePt = dup.Topology[dup.TopoInt[i][3]]; // topology fourth pt
        Assert.Equal(mPt.X, ePt.X);
        Assert.Equal(mPt.Y, ePt.Y);
        Assert.Equal(mPt.Z, ePt.Z);
      }

      // make some changes to original
      elid = 15;
      secid = 30;
      var ids2 = new List<int>();
      var grps2 = new List<int>();
      var dum2 = new List<bool>();
      var nms2 = new List<string>();
      var off2 = new List<GsaOffset>();
      var sects = new List<GsaProperty2d>();
      for (int i = 0; i < origi.ApiElements.Count; i++) {
        ids2.Add(elid++);
        grps2.Add(4);
        dum2.Add(true);
        nms2.Add("Mani");
        off2.Add(new GsaOffset(0, 0, 0, -0.17));
        sects.Add(new GsaProperty2d(secid++));
      }

      origi.Ids = ids2;
      origi.ApiElements.SetMembers(grps2);
      origi.ApiElements.SetMembers(dum2);
      origi.ApiElements.SetMembers(nms2);
      origi.ApiElements.SetMembers(off2);
      origi.Prop2ds = sects;

      // check that duplicate maintains values
      int checkId = 3;
      int checkSectId = 4;
      for (int i = 0; i < dup.ApiElements.Count; i++) {
        Assert.Equal(checkId++, dup.Ids[i]);
        Assert.Equal(checkSectId++, dup.Prop2ds[i].Id);
        Assert.Equal(2, dup.ApiElements[i].Group);
        Assert.False(dup.ApiElements[i].IsDummy);
        Assert.Equal("Esmaeil", dup.ApiElements[i].Name);
        Assert.Equal(-0.15, dup.Offsets[i].Z.Value);
      }

      // check that values in original are changed
      checkId = 15;
      checkSectId = 30;
      for (int i = 0; i < origi.ApiElements.Count; i++) {
        // check other members are valid
        Assert.Equal(checkId++, origi.Ids[i]);
        Assert.Equal(checkSectId++, origi.Prop2ds[i].Id);
        Assert.Equal(4, origi.ApiElements[i].Group);
        Assert.True(origi.ApiElements[i].IsDummy);
        Assert.Equal("Mani", origi.ApiElements[i].Name);
        Assert.Equal(-0.17, origi.Offsets[i].Z.Value);
      }
    }

    private static Point3dList Point3dList2() {
      var pts = new Point3dList {
        new Point3d(0, 0, 0),
        new Point3d(0, 5, 0),
        new Point3d(5, 5, 0),
        new Point3d(5, 0, 0),
      };
      pts.Add(pts[0]);
      return pts;
    }

    [Fact]
    public void CreateSectionNotNull() {
      GsaElement2d ele = CreateSampleElement2dWithQuad4Type();
      ele.CreateSection3dPreview();

      Assert.NotNull(ele.Section3dPreview);
      Assert.NotNull(ele.Section3dPreview.Outlines);
      Assert.NotNull(ele.Section3dPreview.Mesh);
    }

    [Fact]
    public void DuplicateApiObjectReturnsValidObjectForQuad4Type() {
      GsaElement2d ele = CreateSampleElement2dWithQuad4Type();

      List<GSAElement> list = ele.DuplicateApiObjects();

      Assert.NotNull(list);
      Assert.Equal(2, list.Count);
      for (int i = 0; i < list.Count; i++) {
        Assert.Equal(ele.ApiElements[i].Type, list[i].Type);
        Assert.Equal(ele.ApiElements[i].Topology, list[i].Topology);
        Assert.Equal((Color)ele.ApiElements[i].Colour, (Color)list[i].Colour);
        Assert.Equal(ele.ApiElements[i].Group, list[i].Group);
        Assert.Equal(ele.ApiElements[i].IsDummy, list[i].IsDummy);
        Assert.Equal(ele.ApiElements[i].Name, list[i].Name);
        Assert.Equal(ele.ApiElements[i].Offset.ToString(), list[i].Offset.ToString());
        Assert.Equal(ele.ApiElements[i].OrientationAngle, list[i].OrientationAngle);
        Assert.Equal(ele.ApiElements[i].OrientationNode, list[i].OrientationNode);
        Assert.Equal(ele.ApiElements[i].ParentMember?.Member, list[i].ParentMember?.Member);
        Assert.Equal(ele.ApiElements[i].ParentMember?.Replica, list[i].ParentMember?.Replica);
        Assert.Equal(ele.ApiElements[i].Property, list[i].Property);
      }
    }

    [Fact]
    public void GetCenterPointsReturnsValidPointsForQuad4Type() {
      GsaElement2d ele = CreateSampleElement2dWithQuad4Type();

      Point3dList points = ele.GetCenterPoints();

      Assert.NotNull(points);
      Assert.Equal(2, points.Count);

      ele.ApiElements[0].Type = ElementType.QUAD8;
      ele.ApiElements[1].Type = ElementType.QUAD8;

      Point3dList points2 = ele.GetCenterPoints();

      Assert.NotNull(points2);
      Assert.Equal(2, points2.Count);

      Assert.NotEqual(points[0], points2[0]);
      Assert.NotEqual(points[1], points2[1]);
    }

    [Fact]
    public void GetTopologyIDsIsValid() {
      GsaElement2d ele = CreateSampleElement2dWithQuad4Type();

      DataTree<int> topology = ele.GetTopologyIDs();
      Assert.NotNull(topology);
      Assert.Equal(2, topology.BranchCount);
      Assert.Equal(8, topology.DataCount);
      Assert.Equal("Tree (Branches = 2)\r\n{14} (N = 4)\r\n{15} (N = 4)", topology.TopologyDescription);
      Assert.Equal(2, topology.Paths.Count);
      Assert.Equal(2, topology.Branches.Count);
    }

    [Fact]
    public void ToStringReturnsValidString() {
      GsaElement2d ele = CreateSampleElement2dWithQuad4Type();
      Assert.Equal("Quad-4 N:21 E:2", ele.ToString());
    }

    [Fact]
    public void UpdateMeshColoursChangeColor() {
      GsaElement2d ele = CreateSampleElement2dWithQuad4Type();
      ele.ApiElements[0].Colour = Color.DarkCyan;
      ele.ApiElements[1].Colour = Color.DarkBlue;

      Assert.Empty(ele.Mesh.VertexColors);

      ele.UpdateMeshColours();

      Assert.Equal(Color.DarkCyan.ToArgb(), ele.Mesh.VertexColors[0].ToArgb());
      Assert.Equal(Color.DarkBlue.ToArgb(), ele.Mesh.VertexColors[1].ToArgb());
    }

    private GsaElement2d CreateSampleElement2dWithQuad4Type() {
      Mesh mesh = ToMesh(new Polyline(Point3dList1()));

      var elem = new GsaElement2d(mesh);
      int elid = 14;
      int secid = 3;
      var grps = new List<int>();
      var dum = new List<bool>();
      var nms = new List<string>();
      var off = new List<GsaOffset>();
      elem.Prop2ds = new List<GsaProperty2d>();
      for (int i = 0; i < elem.ApiElements.Count; i++) {
        elem.Ids[i] = elid++;
        grps.Add(22);
        dum.Add(true);
        nms.Add("Shahin");
        off.Add(new GsaOffset(0, 0, 0, 0.1, LengthUnit.Meter));
        elem.Prop2ds.Add(new GsaProperty2d(secid) {
          ApiProp2d = new Prop2D(),
        });
      }

      elem.ApiElements.SetMembers(grps);
      elem.ApiElements.SetMembers(dum);
      elem.ApiElements.SetMembers(nms);
      elem.ApiElements.SetMembers(off);

      elem.ApiElements[0].Type = ElementType.QUAD4;
      elem.ApiElements[0].Topology = new ReadOnlyCollection<int>(new List<int>(4) {
        1,
        2,
        3,
        4
      });
      elem.ApiElements[1].Type = ElementType.QUAD4;
      elem.ApiElements[1].Topology = new ReadOnlyCollection<int>(new List<int>(4) {
        4,
        3,
        2,
        1
      });
      elem.ApiElements.RemoveRange(2, 20);

      return elem;
    }

    private static Mesh ToMesh(Polyline pol) {
      var mesh = Mesh.CreateFromPlanarBoundary(pol.ToPolylineCurve(), MeshingParameters.DefaultAnalysisMesh, 0.001);
      return mesh;
    }
  }
}
