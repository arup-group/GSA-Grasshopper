using System;

using NUnit.Framework;
using GhSA;
using GhSA.Parameters;
using Rhino.Geometry;
using GsaAPI;

namespace UnitTestGhSA
{
    public class NodeTests
    {
        [TestCase]
        // each test class much first initiate / load the GsaAPI using reflection
        public void InitiateAPI()
        {
            Assert.IsTrue(UnitTestGhSA.Helper.LoadAPI());
        }

        [TestCase]
        public void TestCreateGsaNodeFromPt()
        {
            GsaNode node = new GsaNode(new Point3d(10, 15, 7.8));

            Assert.AreEqual(10, node.Point.X);
            Assert.AreEqual(15, node.Point.Y);
            Assert.AreEqual(7.8, node.Point.Z);

            // node should maintain syncronisation of Point and Node.Position:
            Assert.AreEqual(10, node.Node.Position.X);
            Assert.AreEqual(15, node.Node.Position.Y);
            Assert.AreEqual(7.8, node.Node.Position.Z);
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
            GsaNode node = new GsaNode(new Point3d(5.3, 9.9, 2017), bool6);

            Assert.AreEqual(5.3, node.Point.X);
            Assert.AreEqual(9.9, node.Point.Y);
            Assert.AreEqual(2017, node.Point.Z);
            Assert.IsTrue(node.Node.Restraint.X);
            Assert.IsFalse(node.Node.Restraint.Y);
            Assert.IsTrue(node.Node.Restraint.Z);
            Assert.IsFalse(node.Node.Restraint.XX);
            Assert.IsTrue(node.Node.Restraint.YY);
            Assert.IsFalse(node.Node.Restraint.ZZ);
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
            GsaNode node = new GsaNode(new Point3d(-40, -3.654, -99), id, bool6, pln);

            Assert.AreEqual(-40, node.Point.X);
            Assert.AreEqual(-3.654, node.Point.Y);
            Assert.AreEqual(-99, node.Point.Z);

            Assert.AreEqual(44, node.ID);

            Assert.IsFalse(node.Node.Restraint.X);
            Assert.IsTrue(node.Node.Restraint.Y);
            Assert.IsFalse(node.Node.Restraint.Z);
            Assert.IsTrue(node.Node.Restraint.XX);
            Assert.IsFalse(node.Node.Restraint.YY);
            Assert.IsTrue(node.Node.Restraint.ZZ);

            // the local plane origin point should be moved to the node position
            Assert.AreEqual(-40, node.LocalAxis.OriginX);
            Assert.AreEqual(-3.654, node.LocalAxis.OriginY);
            Assert.AreEqual(-99, node.LocalAxis.OriginZ);
            Assert.AreEqual(1, node.LocalAxis.Normal.Y);
        }
    }
}