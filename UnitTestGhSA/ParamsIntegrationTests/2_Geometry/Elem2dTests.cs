using System;

using NUnit.Framework;
using GhSA;
using GhSA.Parameters;
using Rhino.Geometry;
using GsaAPI;
using System.Collections.Generic;

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
            for (int i = 0; i < elem.Elements.Count; i++)
            {
                elem.ID[i] = elid++;
                elem.Properties.Add(new GsaProp2d());
                elem.Properties[i].ID = secid++;
                elem.Elements[i].Group = 22;
                elem.Elements[i].IsDummy = true;
                elem.Elements[i].Name = "Shahin";
                elem.Elements[i].Offset.Z = 0.1;
                elem.Elements[i].Property = 3;
            }

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
            for (int i = 0; i < elem.Elements.Count; i++)
            {
                // check that element is tri or quad corrosponding to mesh face
                if (mesh.Faces[i].IsTriangle)
                    Assert.IsTrue(elem.Elements[i].Type == ElementType.TRI3);
                if (mesh.Faces[i].IsQuad)
                    Assert.IsTrue(elem.Elements[i].Type == ElementType.QUAD4);

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

                if (elem.Elements[i].Type == ElementType.QUAD4)
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
                Assert.AreEqual(22, elem.Elements[i].Group);
                Assert.IsTrue(elem.Elements[i].IsDummy);
                Assert.AreEqual("Shahin", elem.Elements[i].Name);
                Assert.AreEqual(0.1, elem.Elements[i].Offset.Z);
                Assert.AreEqual(3, elem.Elements[i].Property);
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
            for (int i = 0; i < origi.Elements.Count; i++)
            {
                origi.ID[i] = elid++;
                origi.Properties.Add(new GsaProp2d());
                origi.Properties[i].ID = secid++;
                origi.Elements[i].Group = 2;
                origi.Elements[i].IsDummy = false;
                origi.Elements[i].Name = "Esmaeil";
                origi.Elements[i].Offset.Z = -0.15;
                origi.Elements[i].Property = 1;
            }

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
            for (int i = 0; i < dup.Elements.Count; i++)
            {
                // check that element is tri or quad corrosponding to mesh face
                if (mesh.Faces[i].IsTriangle)
                    Assert.IsTrue(dup.Elements[i].Type == ElementType.TRI3);
                if (mesh.Faces[i].IsQuad)
                    Assert.IsTrue(dup.Elements[i].Type == ElementType.QUAD4);

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

                if (dup.Elements[i].Type == ElementType.QUAD4)
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
            for (int i = 0; i < origi.Elements.Count; i++)
            {
                origi.ID[i] = elid++;
                origi.Properties[i].ID = secid++;
                origi.Elements[i].Group = 4;
                origi.Elements[i].IsDummy = true;
                origi.Elements[i].Name = "Mani";
                origi.Elements[i].Offset.Z = -0.17;
                origi.Elements[i].Property = 2;
            }

            // check that duplicate maintains values
            int chelid = 3;
            int chsecid = 4;
            for (int i = 0; i < dup.Elements.Count; i++)
            {
                // check other members are valid
                Assert.AreEqual(chelid++, dup.ID[i]);
                Assert.AreEqual(chsecid++, dup.Properties[i].ID);
                Assert.AreEqual(2, dup.Elements[i].Group);
                Assert.IsFalse(dup.Elements[i].IsDummy);
                Assert.AreEqual("Esmaeil", dup.Elements[i].Name);
                Assert.AreEqual(-0.15, dup.Elements[i].Offset.Z);
                Assert.AreEqual(1, dup.Elements[i].Property);
            }

            // check that values in original are changed
            chelid = 15;
            chsecid = 16;
            for (int i = 0; i < origi.Elements.Count; i++)
            {
                // check other members are valid
                Assert.AreEqual(chelid++, origi.ID[i]);
                Assert.AreEqual(chsecid++, origi.Properties[i].ID);
                Assert.AreEqual(4, origi.Elements[i].Group);
                Assert.IsTrue(origi.Elements[i].IsDummy);
                Assert.AreEqual("Mani", origi.Elements[i].Name);
                Assert.AreEqual(-0.17, origi.Elements[i].Offset.Z);
                Assert.AreEqual(2, origi.Elements[i].Property);
            }
        }
    }
}