using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;

using Grasshopper.Kernel.Geometry;
using Grasshopper.Kernel.Geometry.SpatialTrees;

using GsaAPI;

using GsaGH.Helpers.Import;
using GsaGH.Parameters;

using Rhino.Collections;
using Rhino.Geometry;

using Xunit;

namespace GsaGHTests.Helpers.Import {
  [Collection("GrasshopperFixture collection")]
  public class ElementsTest {
    [Fact]
    public void ImportElement1dTest() {
      var model = new GsaModel(ImportElementsMotherModel());
      var elements = new Elements(model);
      GsaElement1d elem = elements.Element1ds.First().Value;
      Assert.Equal("EN 1993-1-1:2005", elem.Section.Material.SteelDesignCodeName);
      Assert.Equal("STD R 800 400", elem.Section.ApiSection.Profile);
      Duplicates.AreEqual(model.Materials.SteelMaterials[1], elem.Section.Material);
      Duplicates.AreEqual(model.Sections[1].Value, elem.Section);

      AssertPointsAreEqual(new Point3d(0, 0, 0), elem.Line.PointAtStart);
      AssertPointsAreEqual(new Point3d(4.5, 4.5, 0), elem.Line.PointAtEnd);
    }

    [Fact]
    public void ImportElement2dTest() {
      var model = new GsaModel(ImportElementsMotherModel());
      var elements = new Elements(model);
      GsaElement2d elem = elements.Element2ds.Where(x => x.Value.IsLoadPanel == false).First().Value;
      Assert.Equal("EC2-1-1", elem.Prop2ds[0].Material.ConcreteDesignCodeName);
      Assert.Equal(200, elem.Prop2ds[0].Thickness.Millimeters);
      Duplicates.AreEqual(model.Materials.ConcreteMaterials[1], elem.Prop2ds[0].Material);
      Duplicates.AreEqual(model.Prop2ds[2].Value, elem.Prop2ds[0]);

      AssertPointsAreEqual(new Point3d(0, 0, 0), elem.Mesh.Vertices[0]);
      AssertPointsAreEqual(new Point3d(4.5, 0, 0), elem.Mesh.Vertices[1]);
      AssertPointsAreEqual(new Point3d(4.5, 4.5, 0), elem.Mesh.Vertices[2]);
      AssertPointsAreEqual(new Point3d(0, 4.5, 0), elem.Mesh.Vertices[3]);
    }

    [Fact]
    public void ImportLoadPanelTest() {
      var model = new GsaModel(ImportElementsMotherModel());
      var elements = new Elements(model);
      GsaElement2d elem = elements.Element2ds.Where(x => x.Value.IsLoadPanel).First().Value;
      Assert.Equal(Property2D_Type.LOAD, elem.Prop2ds[0].ApiProp2d.Type);
      var polyline = new Rhino.Geometry.Polyline();
      elem.Curve.TryGetPolyline(out polyline);
      AssertPointsAreEqual(new Point3d(0, 0, 0), polyline.ElementAt(0));
      AssertPointsAreEqual(new Point3d(4.5, 0, 0), polyline.ElementAt(1));
      AssertPointsAreEqual(new Point3d(4.5, 4.5, 0), polyline.ElementAt(2));
      AssertPointsAreEqual(new Point3d(0, 4.5, 0), polyline.ElementAt(3));
    }

    [Fact]
    public void ImportElement3dTest() {
      var model = new GsaModel(ImportElementsMotherModel());
      var elements = new Elements(model);
      GsaElement3d elem = elements.Element3ds.First().Value;
      Assert.Equal(MatType.Timber, elem.Prop3ds[0].Material.MaterialType);
      Duplicates.AreEqual(model.Materials.TimberMaterials[1], elem.Prop3ds[0].Material);
      Duplicates.AreEqual(model.Prop3ds[5].Value, elem.Prop3ds[0]);

      AssertPointsAreEqual(new Point3d(0, 0, 0), elem.Topology[0]);
      AssertPointsAreEqual(new Point3d(4.5, 0, 0), elem.Topology[1]);
      AssertPointsAreEqual(new Point3d(4.5, 4.5, 0), elem.Topology[2]);
      AssertPointsAreEqual(new Point3d(0, 4.5, 0), elem.Topology[3]);
      AssertPointsAreEqual(new Point3d(0, 0, 5), elem.Topology[4]);
      AssertPointsAreEqual(new Point3d(4.5, 0, 5), elem.Topology[5]);
      AssertPointsAreEqual(new Point3d(4.5, 4.5, 5), elem.Topology[6]);
      AssertPointsAreEqual(new Point3d(0, 4.5, 5), elem.Topology[7]);
    }

    private void AssertPointsAreEqual(Point3d expected, Point3d actual) {
      Assert.Equal(expected.X, actual.X);
      Assert.Equal(expected.Y, actual.Y);
      Assert.Equal(expected.Z, actual.Z);
    }

    private static GsaAPI.Model ImportElementsMotherModel() {
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

      model.AddElement(new Element() {
        Topology = new ReadOnlyCollection<int>(new List<int>() {
          1, 3,
          }),
        Property = 1,
        Type = ElementType.BEAM
      });

      model.AddElement(new Element() {
        Type = ElementType.QUAD4,
        Topology = new ReadOnlyCollection<int>(new List<int>() {
          1, 2, 3, 4
          }),
        Property = 2,
      });
      model.AddElement(new Element() {
        Type = ElementType.BRICK8,
        Topology = new ReadOnlyCollection<int>(new List<int>() {
          1, 2, 3, 4, 5, 6, 7, 8,
          }),
        Property = 5,
      });

      model.AddLoadPanelElement(new LoadPanelElement {
        Topology = new ReadOnlyCollection<int>(new List<int>() {
          1, 2, 3, 4
          }),
        Property = 8,
      });

      return model;
    }
  }
}
