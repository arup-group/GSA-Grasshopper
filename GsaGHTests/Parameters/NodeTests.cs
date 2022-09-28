using GsaGH.Parameters;
using Rhino.Geometry;
using Xunit;

namespace ParamsIntegrationTests
{
  public class NodeTests
  {
    [Fact]
    public void TestCreateGsaNodeFromPt()
    {
      GsaNode node = new GsaNode(new Point3d(10, 15, 7.8));

      Assert.Equal(10, node.Point.X);
      Assert.Equal(15, node.Point.Y);
      Assert.Equal(7.8, node.Point.Z);

      // node should maintain syncronisation of Point and API_Node.Position:
      Assert.Equal(10, node.Point.X);
      Assert.Equal(15, node.Point.Y);
      Assert.Equal(7.8, node.Point.Z);
    }
    [Fact]

    public void TestCreateGsaNodeRestrained()
    {
      // create new Bool6
      GsaBool6 bool6 = new GsaBool6
      {
        X = true,
        Y = false,
        Z = true,
        XX = false,
        YY = true,
        ZZ = false
      };

      // create new node from point with bool6 restraint
      GsaNode node = new GsaNode(new Point3d(5.3, 9.9, 2017));
      node.Restraint = bool6;

      Assert.Equal(5.3, node.Point.X);
      Assert.Equal(9.9, node.Point.Y);
      Assert.Equal(2017, node.Point.Z);
      Assert.True(node.Restraint.X);
      Assert.False(node.Restraint.Y);
      Assert.True(node.Restraint.Z);
      Assert.False(node.Restraint.XX);
      Assert.True(node.Restraint.YY);
      Assert.False(node.Restraint.ZZ);
    }

    [Fact]
    public void TestCreateGsaNodeIdRestrainLocAxis()
    {
      // create new Bool6
      GsaBool6 bool6 = new GsaBool6
      {
        X = false,
        Y = true,
        Z = false,
        XX = true,
        YY = false,
        ZZ = true
      };
      // create new rhino plane for local axis
      Plane pln = Plane.WorldZX;

      // set ID 
      int id = 44;

      // create new node from point with id, bool6 restraint and plane local axis:
      GsaNode node = new GsaNode(new Point3d(-40, -3.654, -99));
      node.Restraint = bool6;
      pln.Origin = node.Point;
      node.LocalAxis = pln;
      node.ID = id;

      Assert.Equal(-40, node.Point.X);
      Assert.Equal(-3.654, node.Point.Y);
      Assert.Equal(-99, node.Point.Z);

      Assert.Equal(44, node.ID);

      Assert.False(node.Restraint.X);
      Assert.True(node.Restraint.Y);
      Assert.False(node.Restraint.Z);
      Assert.True(node.Restraint.XX);
      Assert.False(node.Restraint.YY);
      Assert.True(node.Restraint.ZZ);

      // the local plane origin point should be moved to the node position
      Assert.Equal(-40, node.LocalAxis.OriginX);
      Assert.Equal(-3.654, node.LocalAxis.OriginY);
      Assert.Equal(-99, node.LocalAxis.OriginZ);
      Assert.Equal(0, node.LocalAxis.Normal.X);
      Assert.Equal(1, node.LocalAxis.Normal.Y);
      Assert.Equal(0, node.LocalAxis.Normal.Z);
    }

    [Fact]
    public void TestDuplicateNode()
    {
      // create new node with some properties
      GsaNode node = new GsaNode(new Point3d(-3.3, 0, 1.5));
      node.Colour = System.Drawing.Color.Red;
      node.LocalAxis = Plane.WorldYZ;
      node.ID = 3;
      node.Name = "Mariam";

      // duplicate node
      GsaNode duplicate = node.Duplicate();

      // check that original and duplicate are the same
      Assert.Equal(node.Point.X, duplicate.Point.X);
      Assert.Equal(node.Point.Y, duplicate.Point.Y);
      Assert.Equal(node.Point.Z, duplicate.Point.Z);

      Assert.Equal(duplicate.LocalAxis.OriginX, node.LocalAxis.OriginX);
      Assert.Equal(duplicate.LocalAxis.OriginY, node.LocalAxis.OriginY);
      Assert.Equal(duplicate.LocalAxis.OriginZ, node.LocalAxis.OriginZ);
      Assert.Equal(duplicate.LocalAxis.Normal.X, node.LocalAxis.Normal.X);
      Assert.Equal(duplicate.LocalAxis.Normal.Y, node.LocalAxis.Normal.Y);
      Assert.Equal(duplicate.LocalAxis.Normal.Z, node.LocalAxis.Normal.Z);

      Assert.Equal(node.ID, duplicate.ID);
      Assert.Equal(node.Colour, duplicate.Colour);
      Assert.Equal(node.Name, duplicate.Name);


      // make changes to original node
      node.Point = new Point3d(3.3, 1, -1.5);
      node.LocalAxis = Plane.Unset;
      node.Colour = System.Drawing.Color.Blue;
      node.ID = 2;
      node.Name = "Kristjan";

      // check that original and duplicate are not the same
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

      Assert.NotEqual(node.ID, duplicate.ID);
      Assert.NotEqual(node.Colour, duplicate.Colour);
      Assert.NotEqual(node.Name, duplicate.Name);
    }
  }
}
