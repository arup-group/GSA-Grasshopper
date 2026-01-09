using System.Linq;

using GsaAPI;

using GsaGH.Helpers.Import;
using GsaGH.Parameters;

using Rhino.Geometry;

using Xunit;

namespace GsaGHTests.Helpers.Import {
  [Collection("GrasshopperFixture collection")]
  public class MembersTest {
    [Fact]
    public void ImportMember1dTest() {
      var model = new GsaModel(ImportMembersMotherModel());
      var members = new Members(model, null);
      GsaMember1d mem = members.Member1ds.First().Value;
      Assert.Equal("EN 1993-1-1:2005", mem.Section.Material.SteelDesignCodeName);
      Assert.Equal("STD R 800 400", mem.Section.ApiSection.Profile);
      Duplicates.AreEqual(model.Materials.SteelMaterials[1], mem.Section.Material);
      Duplicates.AreEqual(model.Sections[1].Value, mem.Section);

      AssertPointsAreEqual(new Point3d(0, 0, 0), mem.PolyCurve.PointAtStart);
      AssertPointsAreEqual(new Point3d(4.5, 4.5, 0), mem.PolyCurve.PointAtEnd);
    }

    [Fact]
    public void ImportMember2dTest() {
      var model = new GsaModel(ImportMembersMotherModel());
      var members = new Members(model, null);
      GsaMember2d mem = members.Member2ds.First().Value;
      Assert.Equal("EC2-1-1", mem.Prop2d.Material.ConcreteDesignCodeName);
      Assert.Equal(200, mem.Prop2d.Thickness.Millimeters);
      Duplicates.AreEqual(model.Materials.ConcreteMaterials[1], mem.Prop2d.Material);
      Duplicates.AreEqual(model.Prop2ds[2].Value, mem.Prop2d);

      AssertPointsAreEqual(new Point3d(0, 0, 0), mem.Topology[0]);
      AssertPointsAreEqual(new Point3d(4.5, 0, 0), mem.Topology[1]);
      AssertPointsAreEqual(new Point3d(4.5, 4.5, 0), mem.Topology[2]);
      AssertPointsAreEqual(new Point3d(0, 4.5, 0), mem.Topology[3]);
    }

    [Fact]
    public void ImportMember3dTest() {
      var model = new GsaModel(ImportMembersMotherModel());
      var members = new Members(model, null);
      GsaMember3d mem = members.Member3ds.First().Value;
      Assert.Equal(MatType.Timber, mem.Prop3d.Material.MaterialType);
      Duplicates.AreEqual(model.Materials.TimberMaterials[1], mem.Prop3d.Material);
      Duplicates.AreEqual(model.Prop3ds[5].Value, mem.Prop3d);

      AssertPointsAreEqual(new Point3d(0, 4.5, 0), mem.SolidMesh.Vertices[0]);
      AssertPointsAreEqual(new Point3d(4.5, 4.5, 0), mem.SolidMesh.Vertices[1]);
      AssertPointsAreEqual(new Point3d(4.5, 4.5, 5), mem.SolidMesh.Vertices[2]);
      AssertPointsAreEqual(new Point3d(0, 0, 0), mem.SolidMesh.Vertices[3]);
      AssertPointsAreEqual(new Point3d(0, 0, 5), mem.SolidMesh.Vertices[4]);
      AssertPointsAreEqual(new Point3d(4.5, 0, 0), mem.SolidMesh.Vertices[5]);
      AssertPointsAreEqual(new Point3d(4.5, 0, 5), mem.SolidMesh.Vertices[6]);
      AssertPointsAreEqual(new Point3d(0, 4.5, 5), mem.SolidMesh.Vertices[7]);
    }

    private void AssertPointsAreEqual(Point3d expected, Point3d actual) {
      Assert.Equal(expected.X, actual.X);
      Assert.Equal(expected.Y, actual.Y);
      Assert.Equal(expected.Z, actual.Z);
    }

    private static GsaAPI.Model ImportMembersMotherModel() {
      GsaAPI.Model model = PropertiesTest.ImportPropertiesMotherModel();

      model.AddNode(new Node() {
        Position = new Vector3() {
          X = 0,
          Y = 0,
          Z = 0
        }
      });

      model.AddNode(new Node() {
        Position = new Vector3() {
          X = 4.5,
          Y = 0,
          Z = 0
        }
      });

      model.AddNode(new Node() {
        Position = new Vector3() {
          X = 4.5,
          Y = 4.5,
          Z = 0
        }
      });

      model.AddNode(new Node() {
        Position = new Vector3() {
          X = 0,
          Y = 4.5,
          Z = 0
        }
      });

      model.AddNode(new Node() {
        Position = new Vector3() {
          X = 0,
          Y = 0,
          Z = 5
        }
      });

      model.AddNode(new Node() {
        Position = new Vector3() {
          X = 4.5,
          Y = 0,
          Z = 5
        }
      });

      model.AddNode(new Node() {
        Position = new Vector3() {
          X = 4.5,
          Y = 4.5,
          Z = 5
        }
      });

      model.AddNode(new Node() {
        Position = new Vector3() {
          X = 0,
          Y = 4.5,
          Z = 5
        }
      });

      model.AddMember(new Member() {
        Topology = "1 3",
        Property = 1,
        Type = MemberType.GENERIC_1D,
        Type1D = ElementType.BEAM
      });

      model.AddMember(new Member() {
        Topology = "1 2 3 4",
        Property = 2,
        Type = MemberType.GENERIC_2D,
        Type2D = AnalysisOrder.LINEAR
      });

      model.AddMember(new Member() {
        Type = MemberType.GENERIC_3D,
        Topology = "1 2 4 3; 5 6 8 7; 1 5 4 8; 3 7 2 6; 1 5 2 6; 4 8 3 7",
        Property = 5,
      });

      return model;
    }
  }
}
