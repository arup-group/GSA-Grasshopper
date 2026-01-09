using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

using Grasshopper;

using GsaAPI;

using GsaGH.Helpers;
using GsaGH.Parameters;

using GsaGHTests.Helpers;

using Rhino.Collections;
using GsaGHTests.Components.Geometry;
using Rhino.Geometry;

using Xunit;

using LengthUnit = OasysUnits.Units.LengthUnit;
using Polyline = Rhino.Geometry.Polyline;
using System;

namespace GsaGHTests.Parameters {

  [Collection("GrasshopperFixture collection")]
  public class GsaElement2dTests {

    [Fact]
    public void DuplicateTest() {
      var original = new GsaElement2d(new Mesh());
      var duplicate = new GsaElement2d(original);
      Duplicates.AreEqual(original, duplicate, new List<string>() {
        "Guid"
      });

      GsaElement2d originalLoadPanel = CreateLoadPanel();
      var duplicateLoadPanel = new GsaElement2d(originalLoadPanel);
      Duplicates.AreEqual(originalLoadPanel, duplicateLoadPanel, new List<string>() {
        "Guid", "Offset", "OrientationNode", "Release", "GetEndRelease"
      });

    }

    [Fact]
    public void TestCreateGsaElem2dFromMesh() {
      var mesh = Mesh.CreateFromPlanarBoundary(CreateElement2dTests.Get2dPolyline(),
        MeshingParameters.DefaultAnalysisMesh, 0.001);

      var elem = new GsaElement2d(mesh);
      int initialElementId = 14;
      int initialSectionId = 3;
      var groupId = new List<int>();
      var dummy = new List<bool>();
      var name = new List<string>();
      var off = new List<GsaOffset>();
      elem.Prop2ds = new List<GsaProperty2d>();
      for (int i = 0; i < elem.ApiElements.Count; i++) {
        elem.Ids[i] = initialElementId++;
        groupId.Add(22);
        dummy.Add(true);
        name.Add("Shahin");
        off.Add(new GsaOffset(0, 0, 0, 0.1, LengthUnit.Meter));
        elem.Prop2ds.Add(new GsaProperty2d(initialSectionId));
      }

      elem.ApiElements.SetMembers(groupId);
      elem.ApiElements.SetMembers(dummy);
      elem.ApiElements.SetMembers(name);
      elem.ApiElements.SetMembers(off);

      for (int i = 0; i < elem.Topology.Count; i++) {
        Assert.Equal(mesh.Vertices[i].X, elem.Topology[i].X);
        Assert.Equal(mesh.Vertices[i].Y, elem.Topology[i].Y);
        Assert.Equal(mesh.Vertices[i].Z, elem.Topology[i].Z);
      }

      int checkElementId = 14;
      int checkSectionId = 3;
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

        Assert.Equal(checkElementId++, elem.Ids[i]);
        Assert.Equal(checkSectionId, elem.Prop2ds[i].Id);
        Assert.Equal(22, elem.ApiElements[i].Group);
        Assert.True(elem.ApiElements[i].IsDummy);
        Assert.Equal("Shahin", elem.ApiElements[i].Name);
        Assert.Equal(0.1, elem.Offsets[i].Z.Value, DoubleComparer.Default);
      }
    }

    [Fact]
    public void TestDuplicateElem2d() {
      var mesh = Mesh.CreateFromPlanarBoundary(CreateElement2dTests.Get2dPolyline(),
        MeshingParameters.DefaultAnalysisMesh, 0.001);

      var origi = new GsaElement2d(mesh) {
        Prop2ds = new List<GsaProperty2d>()
      };
      int initialElementId = 3;
      int initialSectionId = 4;
      var groupId = new List<int>();
      var dummy = new List<bool>();
      var name = new List<string>();
      var off = new List<GsaOffset>();
      for (int i = 0; i < origi.ApiElements.Count; i++) {
        origi.Ids[i] = initialElementId++;
        origi.Prop2ds.Add(new GsaProperty2d(initialSectionId++));
        groupId.Add(2);
        dummy.Add(false);
        name.Add("Esmaeil");
        off.Add(new GsaOffset(0, 0, 0, -0.15, LengthUnit.Meter));
      }

      origi.ApiElements.SetMembers(groupId);
      origi.ApiElements.SetMembers(dummy);
      origi.ApiElements.SetMembers(name);
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
      initialElementId = 15;
      initialSectionId = 30;
      var ids2 = new List<int>();
      var grps2 = new List<int>();
      var dum2 = new List<bool>();
      var nms2 = new List<string>();
      var off2 = new List<GsaOffset>();
      var sects = new List<GsaProperty2d>();
      for (int i = 0; i < origi.ApiElements.Count; i++) {
        ids2.Add(initialElementId++);
        grps2.Add(4);
        dum2.Add(true);
        nms2.Add("Mani");
        off2.Add(new GsaOffset(0, 0, 0, -0.17));
        sects.Add(new GsaProperty2d(initialSectionId++));
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

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CreateSectionNotNull(bool isLoadPanel) {
      GsaElement2d element = null;
      if (isLoadPanel) {
        element = CreateLoadPanel();
      } else {
        element = CreateSampleElement2dWithQuad4Type();
      }
      element.CreateSection3dPreview();

      Assert.NotNull(element.Section3dPreview);
      Assert.NotNull(element.Section3dPreview.Outlines);
      Assert.NotNull(element.Section3dPreview.Mesh);
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
      Assert.Equal("Quad-4 N:25 E:2", ele.ToString());
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
      var mesh = Mesh.CreateFromPlanarBoundary(CreateElement2dTests.Get2dPolyline(),
        MeshingParameters.DefaultAnalysisMesh, 0.001);

      var elem = new GsaElement2d(mesh);
      int initialElementId = 14;
      int initialSectionId = 3;
      var groupId = new List<int>();
      var dummy = new List<bool>();
      var name = new List<string>();
      var off = new List<GsaOffset>();
      elem.Prop2ds = new List<GsaProperty2d>();
      for (int i = 0; i < elem.ApiElements.Count; i++) {
        elem.Ids[i] = initialElementId++;
        groupId.Add(22);
        dummy.Add(true);
        name.Add("Shahin");
        off.Add(new GsaOffset(0, 0, 0, 0.1, LengthUnit.Meter));
        elem.Prop2ds.Add(new GsaProperty2d(initialSectionId) {
          ApiProp2d = new Prop2D(),
        });
      }

      elem.ApiElements.SetMembers(groupId);
      elem.ApiElements.SetMembers(dummy);
      elem.ApiElements.SetMembers(name);
      elem.ApiElements.SetMembers(off);

      elem.ApiElements[0].Type = ElementType.QUAD4;
      elem.ApiElements[0].Topology = new ReadOnlyCollection<int>(new List<int>(4) { 1, 2, 3, 4 });
      elem.ApiElements[1].Type = ElementType.QUAD4;
      elem.ApiElements[1].Topology = new ReadOnlyCollection<int>(new List<int>(4) { 4, 3, 2, 1 });
      elem.ApiElements.RemoveRange(2, elem.ApiElements.Count - 2);

      return elem;
    }

    private GsaElement2d CreateLoadPanel() {
      var element = new GsaElement2d(CreateElement2dTests.Get2dPolyline());
      int initialElementId = 14;
      int sectionId = 3;
      var groups = new List<int>();
      var dummy = new List<bool>();
      var names = new List<string>();
      element.Prop2ds = new List<GsaProperty2d>();
      for (int i = 0; i < element.ApiElements.Count; i++) {
        element.Ids[i] = initialElementId++;
        groups.Add(22);
        dummy.Add(true);
        names.Add("Shahin");
        element.Prop2ds.Add(new GsaProperty2d(sectionId) {
          ApiProp2d = new Prop2D(),
        });
      }
      element.ApiElements.SetMembers(groups);
      element.ApiElements.SetMembers(dummy);
      element.ApiElements.SetMembers(names);
      return element;
    }

    [Fact]
    public void GetCenterPointsReturnsValidPointsForLoadPanel() {
      GsaElement2d loadPanel = CreateLoadPanel();
      Point3dList points = loadPanel.GetCenterPoints();
      Assert.NotNull(points);
      Assert.Single(points);
      Assert.Equal(0.5, points[0].X, DoubleComparer.Default);
      Assert.Equal(0.5, points[0].Y, DoubleComparer.Default);
      Assert.Equal(0, points[0].Z, DoubleComparer.Default);
    }

    [Fact]
    public void GeometryTypeOfLoadPanelElementIsCurve() {
      var loadPanel = new GsaElement2d(CreateElement2dTests.Get2dPolyline());
      Assert.Equal(typeof(PolylineCurve), loadPanel.Geometry().GetType());
    }

    [Fact]
    public void GeometryTypeOfFeaElementIsMesh() {
      var feElement = new GsaElement2d(CreateElement2dTests.GetMesh());
      Assert.Equal(typeof(Mesh), feElement.Geometry().GetType());
    }
  }
}
