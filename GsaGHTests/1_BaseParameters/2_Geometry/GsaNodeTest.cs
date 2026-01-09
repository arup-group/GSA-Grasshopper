using System.Drawing;

using GsaGH.Parameters;
using GsaGH.Helpers;

using GsaGHTests.Helpers;

using Rhino.Geometry;

using Xunit;
namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaNodeTest {

    [Fact]
    public void GsaNodeEqualsTest() {
      var original = new GsaNode();

      var duplicate = new GsaNode(original);

      Duplicates.AreEqual(original, duplicate);
    }

    [Fact]
    public void TestCreateGsaNodeFromPt() {
      var node = new GsaNode(new Point3d(10, 15, 7.8));

      Assert.Equal(10, node.Point.X, DoubleComparer.Default);
      Assert.Equal(15, node.Point.Y, DoubleComparer.Default);
      Assert.Equal(7.8, node.Point.Z, DoubleComparer.Default);

      Assert.Equal(10, node.Point.X, DoubleComparer.Default);
      Assert.Equal(15, node.Point.Y, DoubleComparer.Default);
      Assert.Equal(7.8, node.Point.Z, DoubleComparer.Default);
    }

    [Fact]
    public void TestCreateGsaNodeIdRestrainLocAxis() {
      var bool6 = new GsaBool6 {
        X = false,
        Y = true,
        Z = false,
        Xx = true,
        Yy = false,
        Zz = true,
      };
      Plane pln = Plane.WorldZX;

      int id = 44;

      var node = new GsaNode(new Point3d(-40, -3.654, -99)) {
        Restraint = bool6,
      };
      pln.Origin = node.Point;
      node.LocalAxis = pln;
      node.Id = id;

      Assert.Equal(-40, node.Point.X, DoubleComparer.Default);
      Assert.Equal(-3.654, node.Point.Y);
      Assert.Equal(-99, node.Point.Z, DoubleComparer.Default);

      Assert.Equal(44, node.Id);

      Assert.False(node.Restraint.X);
      Assert.True(node.Restraint.Y);
      Assert.False(node.Restraint.Z);
      Assert.True(node.Restraint.Xx);
      Assert.False(node.Restraint.Yy);
      Assert.True(node.Restraint.Zz);

      Assert.Equal(-40, node.LocalAxis.OriginX, DoubleComparer.Default);
      Assert.Equal(-3.654, node.LocalAxis.OriginY, DoubleComparer.Default);
      Assert.Equal(-99, node.LocalAxis.OriginZ, DoubleComparer.Default);
      Assert.Equal(0, node.LocalAxis.Normal.X, DoubleComparer.Default);
      Assert.Equal(1, node.LocalAxis.Normal.Y, DoubleComparer.Default);
      Assert.Equal(0, node.LocalAxis.Normal.Z, DoubleComparer.Default);
    }

    [Fact]
    public void TestCreateGsaNodeRestrained() {
      var bool6 = new GsaBool6 {
        X = true,
        Y = false,
        Z = true,
        Xx = false,
        Yy = true,
        Zz = false,
      };

      var node = new GsaNode(new Point3d(5.3, 9.9, 2017)) {
        Restraint = bool6,
      };

      Assert.Equal(5.3, node.Point.X, DoubleComparer.Default);
      Assert.Equal(9.9, node.Point.Y, DoubleComparer.Default);
      Assert.Equal(2017, node.Point.Z);
      Assert.True(node.Restraint.X);
      Assert.False(node.Restraint.Y);
      Assert.True(node.Restraint.Z);
      Assert.False(node.Restraint.Xx);
      Assert.True(node.Restraint.Yy);
      Assert.False(node.Restraint.Zz);
    }

    [Fact]
    public void TestDuplicateNode() {
      var node = new GsaNode(new Point3d(-3.3, 0, 1.5)) {
        LocalAxis = Plane.WorldYZ,
        Id = 3,
      };
      node.ApiNode.Colour = Color.Red;
      node.ApiNode.Name = "Mariam";

      var duplicate = new GsaNode(node);

      Assert.Equal(node.Point.X, duplicate.Point.X);
      Assert.Equal(node.Point.Y, duplicate.Point.Y);
      Assert.Equal(node.Point.Z, duplicate.Point.Z);

      Assert.Equal(duplicate.LocalAxis.OriginX, node.LocalAxis.OriginX, DoubleComparer.Default);
      Assert.Equal(duplicate.LocalAxis.OriginY, node.LocalAxis.OriginY, DoubleComparer.Default);
      Assert.Equal(duplicate.LocalAxis.OriginZ, node.LocalAxis.OriginZ, DoubleComparer.Default);
      Assert.Equal(duplicate.LocalAxis.Normal.X, node.LocalAxis.Normal.X, DoubleComparer.Default);
      Assert.Equal(duplicate.LocalAxis.Normal.Y, node.LocalAxis.Normal.Y, DoubleComparer.Default);
      Assert.Equal(duplicate.LocalAxis.Normal.Z, node.LocalAxis.Normal.Z, DoubleComparer.Default);

      Assert.Equal(node.Id, duplicate.Id);
      Assert.Equal(node.ApiNode.Colour, duplicate.ApiNode.Colour);
      Assert.Equal(node.ApiNode.Name, duplicate.ApiNode.Name);

      node.Point = new Point3d(3.3, 1, -1.5);
      node.LocalAxis = Plane.Unset;
      node.ApiNode.Colour = Color.Blue;
      node.Id = 2;
      node.ApiNode.Name = "Kristjan";

      Assert.NotEqual(node.Point.X, duplicate.Point.X);
      Assert.NotEqual(node.Point.Y, duplicate.Point.Y);
      Assert.NotEqual(node.Point.Y, duplicate.Point.Y);
      Assert.NotEqual(node.Point.Z, duplicate.Point.Z);

      Assert.NotEqual(duplicate.LocalAxis.OriginX, node.LocalAxis.OriginX);
      Assert.NotEqual(duplicate.LocalAxis.OriginY, node.LocalAxis.OriginY);
      Assert.NotEqual(duplicate.LocalAxis.OriginZ, node.LocalAxis.OriginZ);
      Assert.NotEqual(duplicate.LocalAxis.Normal.X, node.LocalAxis.Normal.X);
      Assert.NotEqual(duplicate.LocalAxis.Normal.Y, node.LocalAxis.Normal.Y);
      Assert.NotEqual(duplicate.LocalAxis.Normal.Z, node.LocalAxis.Normal.Z);

      Assert.NotEqual(node.Id, duplicate.Id);
      Assert.NotEqual(node.ApiNode.Colour, duplicate.ApiNode.Colour);
      Assert.NotEqual(node.ApiNode.Name, duplicate.ApiNode.Name);
    }
  }
}
