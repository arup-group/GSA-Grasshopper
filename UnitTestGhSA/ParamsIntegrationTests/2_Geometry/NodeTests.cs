using System;
using NUnit.Framework;
using GsaGH;
using GsaGH.Parameters;
using Rhino.Geometry;
using GsaAPI;

namespace ParamsIntegrationTests
{
  public class NodeTests
  {
    [TestCase]
    public void TestCreateGsaNodeFromPt()
    {
      GsaNode node = new GsaNode(new Point3d(10, 15, 7.8));

      Assert.AreEqual(10, node.Point.X);
      Assert.AreEqual(15, node.Point.Y);
      Assert.AreEqual(7.8, node.Point.Z);

      // node should maintain syncronisation of Point and API_Node.Position:
      Assert.AreEqual(10, node.Point.X);
      Assert.AreEqual(15, node.Point.Y);
      Assert.AreEqual(7.8, node.Point.Z);
    }
    [TestCase]
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

      Assert.AreEqual(5.3, node.Point.X);
      Assert.AreEqual(9.9, node.Point.Y);
      Assert.AreEqual(2017, node.Point.Z);
      Assert.IsTrue(node.Restraint.X);
      Assert.IsFalse(node.Restraint.Y);
      Assert.IsTrue(node.Restraint.Z);
      Assert.IsFalse(node.Restraint.XX);
      Assert.IsTrue(node.Restraint.YY);
      Assert.IsFalse(node.Restraint.ZZ);
    }
    [TestCase]
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

      Assert.AreEqual(-40, node.Point.X);
      Assert.AreEqual(-3.654, node.Point.Y);
      Assert.AreEqual(-99, node.Point.Z);

      Assert.AreEqual(44, node.ID);

      Assert.IsFalse(node.Restraint.X);
      Assert.IsTrue(node.Restraint.Y);
      Assert.IsFalse(node.Restraint.Z);
      Assert.IsTrue(node.Restraint.XX);
      Assert.IsFalse(node.Restraint.YY);
      Assert.IsTrue(node.Restraint.ZZ);

      // the local plane origin point should be moved to the node position
      Assert.AreEqual(-40, node.LocalAxis.OriginX);
      Assert.AreEqual(-3.654, node.LocalAxis.OriginY);
      Assert.AreEqual(-99, node.LocalAxis.OriginZ);
      Assert.AreEqual(0, node.LocalAxis.Normal.X);
      Assert.AreEqual(1, node.LocalAxis.Normal.Y);
      Assert.AreEqual(0, node.LocalAxis.Normal.Z);
    }
    [TestCase]
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
      Assert.AreEqual(node.Point.X, duplicate.Point.X);
      Assert.AreEqual(node.Point.Y, duplicate.Point.Y);
      Assert.AreEqual(node.Point.Z, duplicate.Point.Z);

      Assert.AreEqual(duplicate.LocalAxis.OriginX, node.LocalAxis.OriginX);
      Assert.AreEqual(duplicate.LocalAxis.OriginY, node.LocalAxis.OriginY);
      Assert.AreEqual(duplicate.LocalAxis.OriginZ, node.LocalAxis.OriginZ);
      Assert.AreEqual(duplicate.LocalAxis.Normal.X, node.LocalAxis.Normal.X);
      Assert.AreEqual(duplicate.LocalAxis.Normal.Y, node.LocalAxis.Normal.Y);
      Assert.AreEqual(duplicate.LocalAxis.Normal.Z, node.LocalAxis.Normal.Z);

      Assert.AreEqual(node.ID, duplicate.ID);
      Assert.AreEqual(node.Colour, duplicate.Colour);
      Assert.AreEqual(node.Name, duplicate.Name);


      // make changes to original node
      node.Point = new Point3d(3.3, 1, -1.5);
      node.LocalAxis = Plane.Unset;
      node.Colour = System.Drawing.Color.Blue;
      node.ID = 2;
      node.Name = "Kristjan";

      // check that original and duplicate are not the same
      Assert.AreNotEqual(node.Point.X, duplicate.Point.X);
      Assert.AreNotEqual(node.Point.Y, duplicate.Point.Y);
      Assert.AreNotEqual(node.Point.Z, duplicate.Point.Z);

      Assert.AreNotEqual(duplicate.LocalAxis.OriginX, node.LocalAxis.OriginX);
      Assert.AreNotEqual(duplicate.LocalAxis.OriginY, node.LocalAxis.OriginY);
      Assert.AreNotEqual(duplicate.LocalAxis.OriginZ, node.LocalAxis.OriginZ);
      Assert.AreNotEqual(duplicate.LocalAxis.Normal.X, node.LocalAxis.Normal.X);
      Assert.AreNotEqual(duplicate.LocalAxis.Normal.Y, node.LocalAxis.Normal.Y);
      Assert.AreNotEqual(duplicate.LocalAxis.Normal.Z, node.LocalAxis.Normal.Z);

      Assert.AreNotEqual(node.ID, duplicate.ID);
      Assert.AreNotEqual(node.Colour, duplicate.Colour);
      Assert.AreNotEqual(node.Name, duplicate.Name);
    }
  }
}