using System;
using System.Collections.Generic;
using GsaAPI;
using GsaGH;
using GsaGH.Parameters;
using NUnit.Framework;
using Rhino.Geometry;
using UnitsNet;
using UnitsNet.Units;

namespace ParamsIntegrationTests
{
    public class Elem2dTests
    {
        [TestCase]
        public void TestCreateGsaElem2dFromMesh()
        {
            // create a list of corner points
            List<Point3d> pts = new List<Point3d>();
            pts.Add(new Point3d(-3, -4, 0));
            pts.Add(new Point3d(5, -2, 0));
            pts.Add(new Point3d(6, 7, 0));
            pts.Add(new Point3d(-1, 2, 0));
            pts.Add(pts[0]); // add initial point to close curve
            Polyline pol = new Polyline(pts); // create edge-crv from pts

            // create mesh from boundary curve
            Mesh mesh = Mesh.CreateFromPlanarBoundary(pol.ToPolylineCurve(), MeshingParameters.DefaultAnalysisMesh, 0.001);
            
            // create element
            GsaElement2d elem = new GsaElement2d(mesh);

            // set some element class members
            int elid = 14;
            int secid = 3;
            List<int> grps = new List<int>();
            List<bool> dum = new List<bool>();
            List<string> nms = new List<string>();
            List<GsaOffset> off = new List<GsaOffset>();
            for (int i = 0; i < elem.Count; i++)
            {
                elem.ID[i] = elid++;
                elem.Properties[i].ID = secid++;
                grps.Add(22);
                dum.Add(true);
                nms.Add("Shahin");
                off.Add(new GsaOffset(0, 0, 0, 0.1));
            }
            elem.Groups = grps;
            elem.isDummies = dum;
            elem.Names = nms;
            elem.Offsets = off;

            // check that topology responds to mesh verticies
            for (int i = 0; i < elem.Topology.Count; i++)
            {
                Assert.AreEqual(mesh.Vertices[i].X, elem.Topology[i].X);
                Assert.AreEqual(mesh.Vertices[i].Y, elem.Topology[i].Y);
                Assert.AreEqual(mesh.Vertices[i].Z, elem.Topology[i].Z);
            }

            // loop through all elements and make checks
            int chelid = 14;
            int chsecid = 3;
            for (int i = 0; i < elem.Count; i++)
            {
                // check that element is tri or quad corrosponding to mesh face
                if (mesh.Faces[i].IsTriangle)
                    Assert.IsTrue(elem.Types[i] == ElementType.TRI3);
                if (mesh.Faces[i].IsQuad)
                    Assert.IsTrue(elem.Types[i] == ElementType.QUAD4);

                // check topology int indexing references the right topology points
                Point3d mPt = mesh.Vertices[mesh.Faces[i].A]; // face corner A pt
                Point3d ePt = elem.Topology[elem.TopoInt[i][0]]; // topology first pt
                Assert.AreEqual(mPt.X, ePt.X);
                Assert.AreEqual(mPt.Y, ePt.Y);
                Assert.AreEqual(mPt.Z, ePt.Z);

                mPt = mesh.Vertices[mesh.Faces[i].B]; // face corner B pt
                ePt = elem.Topology[elem.TopoInt[i][1]]; // topology second pt
                Assert.AreEqual(mPt.X, ePt.X);
                Assert.AreEqual(mPt.Y, ePt.Y);
                Assert.AreEqual(mPt.Z, ePt.Z);

                mPt = mesh.Vertices[mesh.Faces[i].C]; // face corner C pt
                ePt = elem.Topology[elem.TopoInt[i][2]]; // topology third pt
                Assert.AreEqual(mPt.X, ePt.X);
                Assert.AreEqual(mPt.Y, ePt.Y);
                Assert.AreEqual(mPt.Z, ePt.Z);

                if (elem.Types[i] == ElementType.QUAD4)
                {
                    mPt = mesh.Vertices[mesh.Faces[i].D]; // face corner D pt
                    ePt = elem.Topology[elem.TopoInt[i][3]]; // topology fourth pt
                    Assert.AreEqual(mPt.X, ePt.X);
                    Assert.AreEqual(mPt.Y, ePt.Y);
                    Assert.AreEqual(mPt.Z, ePt.Z);
                }

                // check other members are valid
                Assert.AreEqual(chelid++, elem.ID[i]);
                Assert.AreEqual(chsecid++, elem.Properties[i].ID);
                Assert.AreEqual(22, elem.Groups[i]);
                Assert.IsTrue(elem.isDummies[i]);
                Assert.AreEqual("Shahin", elem.Names[i]);
                Assert.AreEqual(0.1, elem.Offsets[i].Z);
            }
        }
        
        [TestCase]
        public void TestDuplicateElem2d()
        {
            // create a list of corner points
            List<Point3d> pts = new List<Point3d>();
            pts.Add(new Point3d(0, 0, 0));
            pts.Add(new Point3d(0, 5, 0));
            pts.Add(new Point3d(5, 5, 0));
            pts.Add(new Point3d(5, 0, 0));
            pts.Add(pts[0]); // add initial point to close curve
            Polyline pol = new Polyline(pts); // create edge-crv from pts

            // create mesh from boundary curve
            Mesh mesh = Mesh.CreateFromPlanarBoundary(pol.ToPolylineCurve(), MeshingParameters.DefaultAnalysisMesh, 0.001);

            // create element
            GsaElement2d origi = new GsaElement2d(mesh);

            // set some element class members
            int elid = 3;
            int secid = 4;
            List<int> grps = new List<int>();
            List<bool> dum = new List<bool>();
            List<string> nms = new List<string>();
            List<GsaOffset> off = new List<GsaOffset>();
            for (int i = 0; i < origi.Count; i++)
            {
                origi.ID[i] = elid++;
                origi.Properties.Add(new GsaProp2d());
                origi.Properties[i].ID = secid++;
                grps.Add(2);
                dum.Add(false);
                nms.Add("Esmaeil");
                off.Add(new GsaOffset(0, 0, 0, -0.15));
            }
            origi.Groups = grps;
            origi.isDummies = dum;
            origi.Names = nms;
            origi.Offsets = off;


            // create duplicate
            GsaElement2d dup = origi.Duplicate();

            // check that topology responds to duplicated mesh verticies
            for (int i = 0; i < dup.Topology.Count; i++)
            {
                Assert.AreEqual(mesh.Vertices[i].X, dup.Topology[i].X);
                Assert.AreEqual(mesh.Vertices[i].Y, dup.Topology[i].Y);
                Assert.AreEqual(mesh.Vertices[i].Z, origi.Topology[i].Z);
            }

            // loop through all elements and make on duplicated geometry
            for (int i = 0; i < dup.Count; i++)
            {
                // check that element is tri or quad corrosponding to mesh face
                if (mesh.Faces[i].IsTriangle)
                    Assert.IsTrue(dup.Types[i] == ElementType.TRI3);
                if (mesh.Faces[i].IsQuad)
                    Assert.IsTrue(dup.Types[i] == ElementType.QUAD4);

                // check topology int indexing references the right topology points
                Point3d mPt = mesh.Vertices[mesh.Faces[i].A]; // face corner A pt
                Point3d ePt = dup.Topology[dup.TopoInt[i][0]]; // topology first pt
                Assert.AreEqual(mPt.X, ePt.X);
                Assert.AreEqual(mPt.Y, ePt.Y);
                Assert.AreEqual(mPt.Z, ePt.Z);

                mPt = mesh.Vertices[mesh.Faces[i].B]; // face corner B pt
                ePt = dup.Topology[dup.TopoInt[i][1]]; // topology second pt
                Assert.AreEqual(mPt.X, ePt.X);
                Assert.AreEqual(mPt.Y, ePt.Y);
                Assert.AreEqual(mPt.Z, ePt.Z);

                mPt = mesh.Vertices[mesh.Faces[i].C]; // face corner C pt
                ePt = dup.Topology[dup.TopoInt[i][2]]; // topology third pt
                Assert.AreEqual(mPt.X, ePt.X);
                Assert.AreEqual(mPt.Y, ePt.Y);
                Assert.AreEqual(mPt.Z, ePt.Z);

                if (dup.Types[i] == ElementType.QUAD4)
                {
                    mPt = mesh.Vertices[mesh.Faces[i].D]; // face corner D pt
                    ePt = dup.Topology[dup.TopoInt[i][3]]; // topology fourth pt
                    Assert.AreEqual(mPt.X, ePt.X);
                    Assert.AreEqual(mPt.Y, ePt.Y);
                    Assert.AreEqual(mPt.Z, ePt.Z);
                }
            }

            // make some changes to original
            elid = 15;
            secid = 16;
            List<int> grps2 = new List<int>();
            List<bool> dum2 = new List<bool>();
            List<string> nms2 = new List<string>();
            List<GsaOffset> off2 = new List<GsaOffset>();
            for (int i = 0; i < origi.Count; i++)
            {
                origi.ID[i] = elid++;
                origi.Properties[i].ID = secid++;
                origi.Groups[i] = 4;
                origi.isDummies[i] = true;
                origi.Names[i] = "Mani";
                origi.Offsets[i].Z = new Length(-0.17, LengthUnit.Meter);
                grps2.Add(4);
                dum2.Add(true);
                nms2.Add("Mani");
                off2.Add(new GsaOffset(0, 0, 0, -0.17));
            }
            origi.Groups = grps2;
            origi.isDummies = dum2;
            origi.Names = nms2;
            origi.Offsets = off2;

            // check that duplicate maintains values
            int chelid = 3;
            int chsecid = 4;
            for (int i = 0; i < dup.Count; i++)
            {
                // check other members are valid
                Assert.AreEqual(chelid++, dup.ID[i]);
                Assert.AreEqual(chsecid++, dup.Properties[i].ID);
                Assert.AreEqual(2, dup.Groups[i]);
                Assert.IsFalse(dup.isDummies[i]);
                Assert.AreEqual("Esmaeil", dup.Names[i]);
                Assert.AreEqual(-0.15, dup.Offsets[i].Z);
            }

            // check that values in original are changed
            chelid = 15;
            chsecid = 16;
            for (int i = 0; i < origi.Count; i++)
            {
                // check other members are valid
                Assert.AreEqual(chelid++, origi.ID[i]);
                Assert.AreEqual(chsecid++, origi.Properties[i].ID);
                Assert.AreEqual(4, origi.Groups[i]);
                Assert.IsTrue(origi.isDummies[i]);
                Assert.AreEqual("Mani", origi.Names[i]);
                Assert.AreEqual(-0.17, origi.Offsets[i].Z);
            }
        }
    }
}