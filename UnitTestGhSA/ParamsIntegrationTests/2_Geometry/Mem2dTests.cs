using System;

using NUnit.Framework;
using GhSA;
using GhSA.Parameters;
using Rhino.Geometry;
using GsaAPI;
using System.Collections.Generic;

namespace ParamsIntegrationTests
{
    public class Mem2dTests
    {
        [TestCase]
        public void TestCreateGsaMem2dFromBrep()
        {
            // create a 4 point surface
            NurbsSurface surf = NurbsSurface.CreateFromCorners(
                new Point3d(-3, -4, 0),
                new Point3d(5, -2, 0),
                new Point3d(6, 7, 0),
                new Point3d(-1, 2, 0));
            // create new brep type
            Brep brep = new Brep();
            // add surface to brep object
            brep.AddSurface(surf);

            // empty lists for inclusion points and lines
            List<Point3d> pts = new List<Point3d>();
            List<Curve> crvs = new List<Curve>();
            
            // create 2d member from brep
            GsaMember2d mem = new GsaMember2d(brep, crvs, pts);

            Assert.AreEqual(-3, mem.Topology[0].X);
        }
        
        [TestCase]
        public void TestDuplicateNode()
        {
            
        }
    }
}