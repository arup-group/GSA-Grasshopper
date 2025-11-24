using System.Collections.Generic;
using System.Collections.ObjectModel;

using GsaAPI;
using GsaAPI.Materials;

using GsaGH.Parameters;

using OasysUnits;

using Rhino.Geometry;

using Xunit;

using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGHTests.Helpers.Export {
  public partial class AssembleModelTests {

    internal void TestAnalysisMaterial(GsaMaterial expected, int expectedId, GsaModel actualModel) {
      ReadOnlyDictionary<int, AnalysisMaterial> apiMaterials
        = actualModel.ApiModel.AnalysisMaterials();
      Assert.True(apiMaterials.ContainsKey(expectedId),
        "Analysis material with id " + expectedId + " is not present in model");

      AnalysisMaterial api = apiMaterials[expectedId];
      Assert.Equal(expected.AnalysisMaterial.CoefficientOfThermalExpansion,
        api.CoefficientOfThermalExpansion);
      Assert.Equal(expected.AnalysisMaterial.Density, api.Density);
      Assert.Equal(expected.AnalysisMaterial.ElasticModulus, api.ElasticModulus);
      Assert.Equal(expected.AnalysisMaterial.PoissonsRatio, api.PoissonsRatio);
    }

    internal void TestElement1d(
      GsaElement1D expected, LengthUnit unit, int expectedId, GsaModel actualModel) {
      ReadOnlyDictionary<int, Element> apiElements = actualModel.ApiModel.Elements();
      Assert.True(apiElements.ContainsKey(expectedId),
        "Element with id " + expectedId + " is not present in model");

      ReadOnlyDictionary<int, Node> apiNodes = actualModel.ApiModel.Nodes();
      Element api = apiElements[expectedId];
      Node apiStart = apiNodes[api.Topology[0]];
      Node apiEnd = apiNodes[api.Topology[1]];
      Assert.Equal(apiStart.Position.X, new Length(expected.Line.PointAtStart.X, unit).Meters);
      Assert.Equal(apiStart.Position.Y, new Length(expected.Line.PointAtStart.Y, unit).Meters);
      Assert.Equal(apiStart.Position.Z, new Length(expected.Line.PointAtStart.Z, unit).Meters);
      Assert.Equal(apiEnd.Position.X, new Length(expected.Line.PointAtEnd.X, unit).Meters);
      Assert.Equal(apiEnd.Position.Y, new Length(expected.Line.PointAtEnd.Y, unit).Meters);
      Assert.Equal(apiEnd.Position.Z, new Length(expected.Line.PointAtEnd.Z, unit).Meters);

      Assert.Equal(expected.ApiElement.Group, api.Group);
      Assert.Equal(expected.ApiElement.Type, api.Type);
      Assert.Equal(expected.ApiElement.Name, api.Name);
      Assert.Equal(expected.ApiElement.IsDummy, api.IsDummy);
      Assert.Equal(expected.Offset.X1.Meters, api.Offset.X1);
      Assert.Equal(expected.Offset.X2.Meters, api.Offset.X2);
      Assert.Equal(expected.Offset.Y.Meters, api.Offset.Y);
      Assert.Equal(expected.Offset.Z.Meters, api.Offset.Z);
      Assert.Equal(expected.OrientationAngle.Degrees, api.OrientationAngle);
      if (expected.OrientationNode != null) {
        Node apiNode = apiNodes[api.OrientationNode];
        Point3d pt = expected.OrientationNode.Point;
        Assert.Equal(apiNode.Position.X, new Length(pt.X, unit).Meters);
        Assert.Equal(apiNode.Position.Y, new Length(pt.Y, unit).Meters);
        Assert.Equal(apiNode.Position.Z, new Length(pt.Z, unit).Meters);
      }

      Assert.Equal(expected.ReleaseStart.X, api.GetEndRelease(0).Releases.X);
      Assert.Equal(expected.ReleaseStart.Y, api.GetEndRelease(0).Releases.Y);
      Assert.Equal(expected.ReleaseStart.Z, api.GetEndRelease(0).Releases.Z);
      Assert.Equal(expected.ReleaseStart.Xx, api.GetEndRelease(0).Releases.XX);
      Assert.Equal(expected.ReleaseStart.Yy, api.GetEndRelease(0).Releases.YY);
      Assert.Equal(expected.ReleaseStart.Zz, api.GetEndRelease(0).Releases.ZZ);
      Assert.Equal(expected.ReleaseEnd.X, api.GetEndRelease(1).Releases.X);
      Assert.Equal(expected.ReleaseEnd.Y, api.GetEndRelease(1).Releases.Y);
      Assert.Equal(expected.ReleaseEnd.Z, api.GetEndRelease(1).Releases.Z);
      Assert.Equal(expected.ReleaseEnd.Xx, api.GetEndRelease(1).Releases.XX);
      Assert.Equal(expected.ReleaseEnd.Yy, api.GetEndRelease(1).Releases.YY);
      Assert.Equal(expected.ReleaseEnd.Zz, api.GetEndRelease(1).Releases.ZZ);

      TestSection(expected.Section, api.Property, actualModel);
    }

    internal void TestElement2d(
      GsaElement2D expected, LengthUnit unit, List<int> expectedIds, GsaModel actualModel) {
      ReadOnlyDictionary<int, Element> apiElements = actualModel.ApiModel.Elements();
      ReadOnlyDictionary<int, Node> apiNodes = actualModel.ApiModel.Nodes();

      int i = 0;
      foreach (int id in expectedIds) {
        Assert.True(apiElements.ContainsKey(id),
          "Element with id " + id + " is not present in model");
        Element api = apiElements[id];
        List<int> topoInts = expected.TopoInt[i];
        int j = 0;
        foreach (int topo in api.Topology) {
          Node apiNode = apiNodes[topo];
          Point3d pt = expected.Topology[topoInts[j++]];
          Assert.Equal(apiNode.Position.X, new Length(pt.X, unit).Meters);
          Assert.Equal(apiNode.Position.Y, new Length(pt.Y, unit).Meters);
          Assert.Equal(apiNode.Position.Z, new Length(pt.Z, unit).Meters);
        }

        int group = expected.ApiElements[i].Group;
        Assert.Equal(group, api.Group);
        bool dummy = expected.ApiElements[i].IsDummy;
        Assert.Equal(dummy, api.IsDummy);
        string name = expected.ApiElements[i].Name;
        Assert.Equal(name, api.Name);
        GsaOffset offset = expected.Offsets[i];
        Assert.Equal(offset.Z.Meters, api.Offset.Z);

        GsaProperty2d prop = expected.Prop2ds[i];
        TestProp2d(prop, api.Property, actualModel);
        i++;
      }
    }

    internal void TestMember1d(
      GsaMember1D expected, LengthUnit unit, int expectedId, GsaModel actualModel) {
      ReadOnlyDictionary<int, Member> apiElements = actualModel.ApiModel.Members();
      Assert.True(apiElements.ContainsKey(expectedId),
        "Member with id " + expectedId + " is not present in model");

      ReadOnlyDictionary<int, Node> apiNodes = actualModel.ApiModel.Nodes();
      Member api = apiElements[expectedId];
      string[] topologySplit = api.Topology.Split(' ');
      int i = 0;
      foreach (string topo in topologySplit) {
        Node apiNode = apiNodes[int.Parse(topo)];
        Point3d pt = expected.Topology[i++];
        Assert.Equal(apiNode.Position.X, new Length(pt.X, unit).Meters);
        Assert.Equal(apiNode.Position.Y, new Length(pt.Y, unit).Meters);
        Assert.Equal(apiNode.Position.Z, new Length(pt.Z, unit).Meters);
      }

      Assert.Equal(expected.ApiMember.Group, api.Group);
      Assert.Equal(expected.ApiMember.Type, api.Type);
      Assert.Equal(expected.ApiMember.Type1D, api.Type1D);
      Assert.Equal(expected.ApiMember.Name, api.Name);
      Assert.Equal(expected.ApiMember.IsDummy, api.IsDummy);
      Assert.Equal(expected.ApiMember.MeshSize, api.MeshSize);
      Assert.Equal(expected.ApiMember.IsIntersector, api.IsIntersector);
      Assert.Equal(expected.Offset.X1.Meters, api.Offset.X1);
      Assert.Equal(expected.Offset.X2.Meters, api.Offset.X2);
      Assert.Equal(expected.Offset.Y.Meters, api.Offset.Y);
      Assert.Equal(expected.Offset.Z.Meters, api.Offset.Z);
      Assert.Equal(expected.OrientationAngle.Degrees, api.OrientationAngle);
      if (expected.OrientationNode != null) {
        Node apiNode = apiNodes[api.OrientationNode];
        Point3d pt = expected.OrientationNode.Point;
        Assert.Equal(apiNode.Position.X, new Length(pt.X, unit).Meters);
        Assert.Equal(apiNode.Position.Y, new Length(pt.Y, unit).Meters);
        Assert.Equal(apiNode.Position.Z, new Length(pt.Z, unit).Meters);
      }

      Assert.Equal(expected.ReleaseStart.X, api.GetEndRelease(0).Releases.X);
      Assert.Equal(expected.ReleaseStart.Y, api.GetEndRelease(0).Releases.Y);
      Assert.Equal(expected.ReleaseStart.Z, api.GetEndRelease(0).Releases.Z);
      Assert.Equal(expected.ReleaseStart.Xx, api.GetEndRelease(0).Releases.XX);
      Assert.Equal(expected.ReleaseStart.Yy, api.GetEndRelease(0).Releases.YY);
      Assert.Equal(expected.ReleaseStart.Zz, api.GetEndRelease(0).Releases.ZZ);
      Assert.Equal(expected.ReleaseEnd.X, api.GetEndRelease(1).Releases.X);
      Assert.Equal(expected.ReleaseEnd.Y, api.GetEndRelease(1).Releases.Y);
      Assert.Equal(expected.ReleaseEnd.Z, api.GetEndRelease(1).Releases.Z);
      Assert.Equal(expected.ReleaseEnd.Xx, api.GetEndRelease(1).Releases.XX);
      Assert.Equal(expected.ReleaseEnd.Yy, api.GetEndRelease(1).Releases.YY);
      Assert.Equal(expected.ReleaseEnd.Zz, api.GetEndRelease(1).Releases.ZZ);

      TestSection(expected.Section, api.Property, actualModel);
    }

    internal void TestMember2d(
      GsaMember2D expected, LengthUnit unit, int expectedId, GsaModel actualModel) {
      ReadOnlyDictionary<int, Member> apiElements = actualModel.ApiModel.Members();
      Assert.True(apiElements.ContainsKey(expectedId),
        "Member with id " + expectedId + " is not present in model");

      ReadOnlyDictionary<int, Node> apiNodes = actualModel.ApiModel.Nodes();
      Member api = apiElements[expectedId];
      string[] topologySplit = api.Topology.Split(' ');
      for (int i = 0; i < expected.Topology.Count; i++) {
        Node apiNode = apiNodes[int.Parse(topologySplit[i])];
        Point3d pt = expected.Topology[i];
        Assert.Equal(apiNode.Position.X, new Length(pt.X, unit).Meters);
        Assert.Equal(apiNode.Position.Y, new Length(pt.Y, unit).Meters);
        Assert.Equal(apiNode.Position.Z, new Length(pt.Z, unit).Meters);
      }

      Assert.Equal(expected.ApiMember.Group, api.Group);
      Assert.Equal(expected.ApiMember.Type, api.Type);
      Assert.Equal(expected.ApiMember.Type2D, api.Type2D);
      Assert.Equal(expected.ApiMember.Name, api.Name);
      Assert.Equal(expected.ApiMember.IsDummy, api.IsDummy);
      Assert.Equal(expected.ApiMember.MeshSize, api.MeshSize);
      Assert.Equal(expected.ApiMember.IsIntersector, api.IsIntersector);
      Assert.Equal(expected.Offset.Z.Meters, api.Offset.Z);
      Assert.Equal(expected.OrientationAngle.Degrees, api.OrientationAngle);

      TestProp2d(expected.Prop2d, api.Property, actualModel);
    }

    internal void TestMember3d(
      GsaMember3D expected, LengthUnit unit, int expectedId, GsaModel actualModel) {
      ReadOnlyDictionary<int, Member> apiElements = actualModel.ApiModel.Members();
      Assert.True(apiElements.ContainsKey(expectedId),
        "Member with id " + expectedId + " is not present in model");

      ReadOnlyDictionary<int, Node> apiNodes = actualModel.ApiModel.Nodes();
      Member api = apiElements[expectedId];
      string[] faces = api.Topology.Split(';');
      int faceId = 0;
      foreach (string face in faces) {
        string[] topo = face.Trim().Split(' ');
        MeshFace mface = expected.SolidMesh.Faces[faceId++];

        Node apiNode = apiNodes[int.Parse(topo[0])];
        Point3d pt = expected.SolidMesh.TopologyVertices[mface.A];
        Assert.Equal(apiNode.Position.X, new Length(pt.X, unit).Meters);
        Assert.Equal(apiNode.Position.Y, new Length(pt.Y, unit).Meters);
        Assert.Equal(apiNode.Position.Z, new Length(pt.Z, unit).Meters);

        apiNode = apiNodes[int.Parse(topo[1])];
        pt = expected.SolidMesh.TopologyVertices[mface.B];
        Assert.Equal(apiNode.Position.X, new Length(pt.X, unit).Meters);
        Assert.Equal(apiNode.Position.Y, new Length(pt.Y, unit).Meters);
        Assert.Equal(apiNode.Position.Z, new Length(pt.Z, unit).Meters);

        apiNode = apiNodes[int.Parse(topo[2])];
        pt = expected.SolidMesh.TopologyVertices[mface.C];
        Assert.Equal(apiNode.Position.X, new Length(pt.X, unit).Meters);
        Assert.Equal(apiNode.Position.Y, new Length(pt.Y, unit).Meters);
        Assert.Equal(apiNode.Position.Z, new Length(pt.Z, unit).Meters);
      }

      Assert.Equal(expected.ApiMember.Group, api.Group);
      Assert.Equal(expected.ApiMember.Name, api.Name);
      Assert.Equal(expected.ApiMember.IsDummy, api.IsDummy);
      Assert.Equal(expected.ApiMember.MeshSize, api.MeshSize);
      Assert.Equal(expected.ApiMember.IsIntersector, api.IsIntersector);

      TestProp3d(expected.Prop3d, api.Property, actualModel);
    }

    internal void TestNode(
      GsaNode expected, LengthUnit unit, int expectedId, GsaModel actualModel) {
      ReadOnlyDictionary<int, Node> apiNodes = actualModel.ApiModel.Nodes();
      Assert.True(apiNodes.ContainsKey(expectedId),
        "Node with id " + expectedId + " is not present in model");

      Node apiNode = apiNodes[expectedId];
      Point3d pt = expected.Point;
      Assert.Equal(apiNode.Position.X, new Length(pt.X, unit).Meters);
      Assert.Equal(apiNode.Position.Y, new Length(pt.Y, unit).Meters);
      Assert.Equal(apiNode.Position.Z, new Length(pt.Z, unit).Meters);

      Assert.Equal(apiNode.Restraint.X, expected.Restraint.X);
      Assert.Equal(apiNode.Restraint.Y, expected.Restraint.Y);
      Assert.Equal(apiNode.Restraint.Z, expected.Restraint.Z);
      Assert.Equal(apiNode.Restraint.XX, expected.Restraint.Xx);
      Assert.Equal(apiNode.Restraint.YY, expected.Restraint.Yy);
      Assert.Equal(apiNode.Restraint.ZZ, expected.Restraint.Zz);

      if (!expected.IsGlobalAxis) {
        ReadOnlyDictionary<int, Axis> apiAxes = actualModel.ApiModel.Axes();
        Assert.True(apiAxes.ContainsKey(apiNode.AxisProperty),
          "Axis with id " + apiNode.AxisProperty + " is not present in model");
        Axis apiAxis = apiAxes[apiNode.AxisProperty];
        Point3d origin = expected.LocalAxis.Origin;
        Assert.Equal(apiAxis.Origin.X, new Length(origin.X, unit).Meters);
        Assert.Equal(apiAxis.Origin.Y, new Length(origin.Y, unit).Meters);
        Assert.Equal(apiAxis.Origin.Z, new Length(origin.Z, unit).Meters);
        Assert.Equal(apiAxis.XVector.X, expected.LocalAxis.XAxis.X);
        Assert.Equal(apiAxis.XVector.Y, expected.LocalAxis.XAxis.Y);
        Assert.Equal(apiAxis.XVector.Z, expected.LocalAxis.XAxis.Z);
        Assert.Equal(apiAxis.XYPlane.X, expected.LocalAxis.YAxis.X);
        Assert.Equal(apiAxis.XYPlane.Y, expected.LocalAxis.YAxis.Y);
        Assert.Equal(apiAxis.XYPlane.Z, expected.LocalAxis.YAxis.Z);
      }

      Assert.Equal(apiNode.Name, expected.ApiNode.Name);
      Assert.Equal(apiNode.Colour, expected.ApiNode.Colour);
      Assert.Equal(apiNode.DamperProperty, expected.ApiNode.DamperProperty);
      Assert.Equal(apiNode.MassProperty, expected.ApiNode.MassProperty);
      if (expected.SpringProperty != null) {
        Assert.Equal(apiNode.SpringProperty, expected.SpringProperty.Id);
      }
    }

    internal void TestProp2d(GsaProperty2d expected, int expectedId, GsaModel actualModel) {
      ReadOnlyDictionary<int, Prop2D> apiProp2ds = actualModel.ApiModel.Prop2Ds();
      Assert.True(apiProp2ds.ContainsKey(expectedId),
        "Prop2d with id " + expectedId + " is not present in model");

      Prop2D api = apiProp2ds[expectedId];
      Assert.Equal(expected.ApiProp2d.Description, api.Description);

      if (api.MaterialAnalysisProperty > 0) {
        TestAnalysisMaterial(expected.Material, api.MaterialAnalysisProperty, actualModel);
      } else {
        Assert.Null(expected.Material.AnalysisMaterial);
      }
    }

    internal void TestProp3d(GsaProperty3d expected, int expectedId, GsaModel actualModel) {
      ReadOnlyDictionary<int, Prop3D> apiProp3ds = actualModel.ApiModel.Prop3Ds();
      Assert.True(apiProp3ds.ContainsKey(expectedId),
        "Prop3d with id " + expectedId + " is not present in model");

      Prop3D api = apiProp3ds[expectedId];

      if (api.MaterialAnalysisProperty > 0) {
        TestAnalysisMaterial(expected.Material, api.MaterialAnalysisProperty, actualModel);
      } else {
        Assert.Null(expected.Material.AnalysisMaterial);
      }
    }

    internal void TestSection(GsaSection expected, int expectedId, GsaModel actualModel) {
      ReadOnlyDictionary<int, Section> apiSections = actualModel.ApiModel.Sections();
      Assert.True(apiSections.ContainsKey(expectedId),
        "Section with id " + expectedId + " is not present in model");

      Section api = apiSections[expectedId];
      Assert.Equal(expected.ApiSection.Profile, api.Profile);

      if (api.MaterialAnalysisProperty > 0) {
        TestAnalysisMaterial(expected.Material, api.MaterialAnalysisProperty, actualModel);
      } else {
        Assert.Null(expected.Material.AnalysisMaterial);
      }
    }
  }
}
