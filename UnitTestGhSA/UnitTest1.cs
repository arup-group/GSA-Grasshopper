using System;

using NUnit.Framework;
using GhSA;
using GhSA.Parameters;
using Rhino.Geometry;

namespace UnitTestGhSA
{
    public class UnitTest1
    {
        
        [TestCase]
        public void TestCreateGsaNode()
        {
            UnitTestGhSA.Helper.LoadAPI();

            GsaNode node = new GsaNode(new Point3d(10, 15, 7.8));
            
            Assert.AreEqual(10, node.Point.X);
            Assert.AreEqual(15, node.Point.Y);
            Assert.AreEqual(7.8, node.Point.Z);
        }
        
        [TestCase]
        public void TestCreateGsaSectionCat()
        {
            UnitTestGhSA.Helper.LoadAPI();

            string profile = "CAT HE HE200.B";
            GsaSection section = new GsaSection(profile);

            double area = section.Section.Area * Math.Pow(10, 6);

            Assert.AreEqual(7808.12, area);
        }

        [TestCase]
        public void TestCreateGsaSectionRect()
        {
            UnitTestGhSA.Helper.LoadAPI();

            string profile = "STD R 15 20";
            GsaSection section = new GsaSection(profile);

            double area = section.Section.Area * Math.Pow(10, 6);

            Assert.AreEqual(300, area);
        }
    }
}
