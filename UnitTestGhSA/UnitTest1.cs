using System;

using NUnit.Framework;
using GhSA;
using GhSA.Parameters;
using Rhino.Geometry;
using GsaAPI;

namespace UnitTestGhSA
{
    public class UnitTest1
    {

        [TestCase]
        public void TestCreateGsaNode()
        {
            UnitTestGhSA.Helper.LoadRefs();

            GsaNode node = new GsaNode(new Point3d(10, 15, 7.8));

            Assert.AreEqual(10, node.Point.X);
            Assert.AreEqual(15, node.Point.Y);
            Assert.AreEqual(7.8, node.Point.Z);
        }

        [TestCase]
        public void TestCreateGsaSectionCat()
        {
            UnitTestGhSA.Helper.LoadRefs();
            Model model = new Model();
            string installPath = GhSA.Util.Gsa.GsaPath.GetPath;
            model.Open(installPath + "\\Samples\\Steel\\Steel_Design_Simple.gwb");

            string profile = "CAT HE HE200.B";
            GsaSection section = new GsaSection(profile);

            double area = section.Section.Area * Math.Pow(10, 6);

            Assert.AreEqual(7808.121, area);
        }

        [TestCase]
        public void TestCreateGsaSectionRect()
        {
            UnitTestGhSA.Helper.LoadRefs();

            string profile = "STD R 15 20";
            GsaSection section = new GsaSection(profile);

            double area = section.Section.Area * Math.Pow(10, 6);

            Assert.AreEqual(300, area);
        }
    }
}