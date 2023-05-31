using System.Collections.Generic;
using GsaAPI;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;
using Xunit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaElement2dTests {

    [Fact]
    public void DuplicateTest() {
      var original = new GsaElement2d(new Mesh());

      GsaElement2d duplicate = original.Duplicate();

      Duplicates.AreEqual(original, duplicate);
    }

    [Fact]
    public void TestCreateGsaElem2dFromMesh() {
      var pts = new List<Point3d> {
        new Point3d(-3, -4, 0),
        new Point3d(5, -2, 0),
        new Point3d(6, 7, 0),
        new Point3d(-1, 2, 0),
      };
      pts.Add(pts[0]);
      var pol = new Polyline(pts);

      var mesh = Mesh.CreateFromPlanarBoundary(pol.ToPolylineCurve(),
        MeshingParameters.DefaultAnalysisMesh, 0.001);

      var elem = new GsaElement2d(mesh);
      int elid = 14;
      int secid = 3;
      var grps = new List<int>();
      var dum = new List<bool>();
      var nms = new List<string>();
      var off = new List<GsaOffset>();
      for (int i = 0; i < elem.Count; i++) {
        elem.Ids[i] = elid++;
        elem.Prop2ds[i].Id = secid++;
        grps.Add(22);
        dum.Add(true);
        nms.Add("Shahin");
        off.Add(new GsaOffset(0, 0, 0, 0.1, LengthUnit.Meter));
      }

      elem.Groups = grps;
      elem.IsDummies = dum;
      elem.Names = nms;
      elem.Offsets = off;

      for (int i = 0; i < elem.Topology.Count; i++) {
        Assert.Equal(mesh.Vertices[i].X, elem.Topology[i].X);
        Assert.Equal(mesh.Vertices[i].Y, elem.Topology[i].Y);
        Assert.Equal(mesh.Vertices[i].Z, elem.Topology[i].Z);
      }

      int chelid = 14;
      int chsecid = 3;
      for (int i = 0; i < elem.Count; i++) {
        if (mesh.Faces[i].IsTriangle) {
          Assert.True(elem.Types[i] == ElementType.TRI3);
        }

        if (mesh.Faces[i].IsQuad) {
          Assert.True(elem.Types[i] == ElementType.QUAD4);
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

        if (elem.Types[i] == ElementType.QUAD4) {
          mPt = mesh.Vertices[mesh.Faces[i].D];
          ePt = elem.Topology[elem.TopoInt[i][3]]; // topology fourth pt
          Assert.Equal(mPt.X, ePt.X);
          Assert.Equal(mPt.Y, ePt.Y);
          Assert.Equal(mPt.Z, ePt.Z);
        }

        Assert.Equal(chelid++, elem.Ids[i]);
        Assert.Equal(chsecid++, elem.Prop2ds[i].Id);
        Assert.Equal(22, elem.Groups[i]);
        Assert.True(elem.IsDummies[i]);
        Assert.Equal("Shahin", elem.Names[i]);
        Assert.Equal(0.1, elem.Offsets[i].Z.Value);
      }
    }

    [Fact]
    public void TestDuplicateElem2d() {
      var pts = new List<Point3d> {
        new Point3d(0, 0, 0),
        new Point3d(0, 5, 0),
        new Point3d(5, 5, 0),
        new Point3d(5, 0, 0),
      };
      pts.Add(pts[0]);
      var pol = new Polyline(pts);

      var mesh = Mesh.CreateFromPlanarBoundary(pol.ToPolylineCurve(),
        MeshingParameters.DefaultAnalysisMesh, 0.001);

      var origi = new GsaElement2d(mesh);
      int elid = 3;
      int secid = 4;
      var grps = new List<int>();
      var dum = new List<bool>();
      var nms = new List<string>();
      var off = new List<GsaOffset>();
      for (int i = 0; i < origi.Count; i++) {
        origi.Ids[i] = elid++;
        origi.Prop2ds.Add(new GsaProp2d());
        origi.Prop2ds[i].Id = secid++;
        grps.Add(2);
        dum.Add(false);
        nms.Add("Esmaeil");
        off.Add(new GsaOffset(0, 0, 0, -0.15, LengthUnit.Meter));
      }

      origi.Groups = grps;
      origi.IsDummies = dum;
      origi.Names = nms;
      origi.Offsets = off;

      GsaElement2d dup = origi.Duplicate();

      for (int i = 0; i < dup.Topology.Count; i++) {
        Assert.Equal(mesh.Vertices[i].X, dup.Topology[i].X);
        Assert.Equal(mesh.Vertices[i].Y, dup.Topology[i].Y);
        Assert.Equal(mesh.Vertices[i].Z, origi.Topology[i].Z);
      }

      for (int i = 0; i < dup.Count; i++) {
        if (mesh.Faces[i].IsTriangle) {
          Assert.True(dup.Types[i] == ElementType.TRI3);
        }

        if (mesh.Faces[i].IsQuad) {
          Assert.True(dup.Types[i] == ElementType.QUAD4);
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

        if (dup.Types[i] != ElementType.QUAD4) {
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
      secid = 16;
      var grps2 = new List<int>();
      var dum2 = new List<bool>();
      var nms2 = new List<string>();
      var off2 = new List<GsaOffset>();
      for (int i = 0; i < origi.Count; i++) {
        origi.Ids[i] = elid++;
        origi.Prop2ds[i].Id = secid++;
        origi.Groups[i] = 4;
        origi.IsDummies[i] = true;
        origi.Names[i] = "Mani";
        origi.Offsets[i].Z = new Length(-0.17, LengthUnit.Meter);
        grps2.Add(4);
        dum2.Add(true);
        nms2.Add("Mani");
        off2.Add(new GsaOffset(0, 0, 0, -0.17));
      }

      origi.Groups = grps2;
      origi.IsDummies = dum2;
      origi.Names = nms2;
      origi.Offsets = off2;

      // check that duplicate maintains values
      int chelid = 3;
      int chsecid = 4;
      for (int i = 0; i < dup.Count; i++) {
        Assert.Equal(chelid++, dup.Ids[i]);
        Assert.Equal(chsecid++, dup.Prop2ds[i].Id);
        Assert.Equal(2, dup.Groups[i]);
        Assert.False(dup.IsDummies[i]);
        Assert.Equal("Esmaeil", dup.Names[i]);
        Assert.Equal(-0.15, dup.Offsets[i].Z.Value);
      }

      // check that values in original are changed
      chelid = 15;
      chsecid = 16;
      for (int i = 0; i < origi.Count; i++) {
        // check other members are valid
        Assert.Equal(chelid++, origi.Ids[i]);
        Assert.Equal(chsecid++, origi.Prop2ds[i].Id);
        Assert.Equal(4, origi.Groups[i]);
        Assert.True(origi.IsDummies[i]);
        Assert.Equal("Mani", origi.Names[i]);
        Assert.Equal(-0.17, origi.Offsets[i].Z.Value);
      }
    }
  }
}
