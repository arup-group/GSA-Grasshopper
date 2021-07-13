﻿using System;
using System.Collections.Generic;
using System.Linq;

using GsaAPI;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Rhino;
using GhSA.Util.Gsa;
using Grasshopper.Documentation;
using Rhino.Collections;

namespace GhSA.Parameters
{
    /// <summary>
    /// Member3d class, this class defines the basic properties and methods for any Gsa Member 3d
    /// </summary>
    public class GsaMember3d

    {
        public Member API_Member
        {
            get { return m_member; }
        }
        public Mesh SolidMesh
        {
            get { return m_mesh; }
            set { m_mesh = Util.GH.Convert.ConvertMeshToTriMeshSolid(value); }
        }
        public int ID
        {
            get { return m_id; }
            set { m_id = value; }
        }

        //public GsaSection Section
        //{
        //    get { return m_section; }
        //    set 
        //    {
        //        if (m_section == null)
        //            PropertyID = 0;
        //        m_section = value; 
        //    }
        //}
        
        #region GsaAPI members
        public System.Drawing.Color Colour
        {
            get
            {
                //if ((System.Drawing.Color)m_member.Colour == System.Drawing.Color.FromArgb(0, 0, 0))
                //    m_member.Colour = UI.Colour.Member1d;
                return (System.Drawing.Color)m_member.Colour;
            }
            set
            {
                CloneMember();
                m_member.Colour = value;
            }
        }
        public int Group
        {
            get { return m_member.Group; }
            set
            {
                CloneMember();
                m_member.Group = value;
            }
        }
        public bool IsDummy
        {
            get { return m_member.IsDummy; }
            set
            {
                CloneMember();
                m_member.IsDummy = value;
            }
        }
        public string Name
        {
            get { return m_member.Name; }
            set
            {
                CloneMember();
                m_member.Name = value;
            }
        }
        public double MeshSize
        {
            get { return m_member.MeshSize; }
            set
            {
                CloneMember();
                m_member.MeshSize = value;
            }
        }
        public int PropertyID
        {
            get { return m_member.Property; }
            set
            {
                CloneMember();
                m_member.Property = value;
                //m_section = null;
            }
        }
        private void CloneMember()
        {
            Member mem = new Member
            {
                Group = m_member.Group,
                IsDummy = m_member.IsDummy,
                MeshSize = m_member.MeshSize,
                Name = m_member.Name.ToString(),
                Offset = m_member.Offset,
                OrientationAngle = m_member.OrientationAngle,
                OrientationNode = m_member.OrientationNode,
                Property = m_member.Property,
                Topology = m_member.Topology.ToString(),
                Type = m_member.Type,
            };

            if ((System.Drawing.Color)m_member.Colour != System.Drawing.Color.FromArgb(0, 0, 0)) // workaround to handle that System.Drawing.Color is non-nullable type
                mem.Colour = m_member.Colour;

            m_member = mem;
        }
        #endregion
        #region fields
        private Member m_member;
        private int m_id = 0;

        private Mesh m_mesh; 
        //private GsaSection m_section;
        #endregion

        #region constructors
        public GsaMember3d()
        {
            m_member = new Member();
            m_member.Type = MemberType.GENERIC_3D;
            m_mesh = new Mesh();
        }

        public GsaMember3d(Mesh mesh)
        {
            m_member = new Member
            {
                Type = MemberType.GENERIC_3D
            };

            m_mesh = GhSA.Util.GH.Convert.ConvertMeshToTriMeshSolid(mesh);
        }
        public GsaMember3d(Member apiMember, int id, Mesh mesh)
        {
            m_member = apiMember;
            m_mesh = GhSA.Util.GH.Convert.ConvertMeshToTriMeshSolid(mesh);
            m_id = id;
        }
        public GsaMember3d(Brep brep)
        {
            m_member = new Member
            {
                Type = MemberType.GENERIC_3D
            };

            m_mesh = GhSA.Util.GH.Convert.ConvertBrepToTriMeshSolid(brep);
        }
        public GsaMember3d Duplicate()
        {
            if (this == null) { return null; }
            GsaMember3d dup = new GsaMember3d();
            dup.m_mesh = (Mesh)m_mesh.DuplicateShallow();
            dup.m_member = m_member;
            //dup.m_section = m_section;
            dup.m_id = m_id;

            return dup;
        }
        public GsaMember3d UpdateGeometry(Brep brep)
        {
            if (this == null) { return null; }
            GsaMember3d dup = this.Duplicate();
            dup.m_mesh = GhSA.Util.GH.Convert.ConvertBrepToTriMeshSolid(brep);

            return dup;
        }
        public GsaMember3d UpdateGeometry(Mesh mesh)
        {
            if (this == null) { return null; }
            GsaMember3d dup = this.Duplicate();
            dup.m_mesh = GhSA.Util.GH.Convert.ConvertMeshToTriMeshSolid(mesh);

            return dup;
        }
        #endregion

        #region properties
        public bool IsValid
        {
            get
            {
                if (m_mesh == null)
                    return false;
                return true;
            }
        }
        #endregion

        #region methods
        public override string ToString()
        {
            string idd = " " + ID.ToString();
            if (ID == 0) { idd = ""; }
            string mes = m_member.Type.ToString();
            string typeTxt = "GSA " + Char.ToUpper(mes[0]) + mes.Substring(1).ToLower().Replace("_", " ") + " Member" + idd;
            typeTxt = typeTxt.Replace("3d", "3D");
            return typeTxt;
        }

        #endregion
    }

    /// <summary>
    /// GsaMember Goo wrapper class, makes sure GsaMember can be used in Grasshopper.
    /// </summary>
    public class GsaMember3dGoo : GH_GeometricGoo<GsaMember3d>, IGH_PreviewData
    {
        #region constructors
        public GsaMember3dGoo()
        {
            this.Value = new GsaMember3d();
        }
        public GsaMember3dGoo(GsaMember3d member)
        {
            if (member == null)
                member = new GsaMember3d();
            this.Value = member; //member.Duplicate();
        }

        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return DuplicateGsaMember3d();
        }
        public GsaMember3dGoo DuplicateGsaMember3d()
        {
            return new GsaMember3dGoo(Value == null ? new GsaMember3d() : Value); //Value.Duplicate());
        }
        #endregion

        #region properties
        public override bool IsValid
        {
            get
            {
                if (Value == null) { return false; }
                if (Value.SolidMesh == null) { return false; }
                return true;
            }
        }
        public override string IsValidWhyNot
        {
            get
            {
                //if (Value == null) { return "No internal GsaMember instance"; }
                if (Value.IsValid) { return string.Empty; }
                return Value.IsValid.ToString(); //Todo: beef this up to be more informative.
            }
        }
        public override string ToString()
        {
            if (Value == null)
                return "Null Member3D";
            else
                return Value.ToString();
        }
        public override string TypeName
        {
            get { return ("Member 3D"); }
        }
        public override string TypeDescription
        {
            get { return ("GSA 3D Member"); }
        }

        public override BoundingBox Boundingbox
        {
            get
            {
                if (Value == null) { return BoundingBox.Empty; }
                if (Value.SolidMesh == null) { return BoundingBox.Empty; }
                return Value.SolidMesh.GetBoundingBox(false) ;
            }
        }
        public override BoundingBox GetBoundingBox(Transform xform)
        {
            if (Value == null) { return BoundingBox.Empty; }
            if (Value.SolidMesh == null) { return BoundingBox.Empty; }
            return Value.SolidMesh.GetBoundingBox(xform);
        }
        #endregion

        #region casting methods
        public override bool CastTo<Q>(out Q target)
        {
            // This function is called when Grasshopper needs to convert this 
            // instance of GsaMember into some other type Q.            


            if (typeof(Q).IsAssignableFrom(typeof(GsaMember3d)))
            {
                if (Value == null)
                    target = default;
                else
                    target = (Q)(object)Value.Duplicate();
                return true;
            }

            if (typeof(Q).IsAssignableFrom(typeof(Member)))
            {
                if (Value == null)
                    target = default;
                else
                    target = (Q)(object)Value.API_Member;
                return true;
            }

            //Cast to Mesh
            if (typeof(Q).IsAssignableFrom(typeof(Mesh)))
            {
                if (Value == null)
                    target = default;
                else
                    target = (Q)(object)Value.SolidMesh;
                return true;
            }
            if (typeof(Q).IsAssignableFrom(typeof(GH_Mesh)))
            {
                if (Value == null)
                    target = default;
                else
                {
                    target = (Q)(object)new GH_Mesh(Value.SolidMesh);
                    if (Value.SolidMesh == null)
                        return false;
                }
                   
                return true;
            }

            if (typeof(Q).IsAssignableFrom(typeof(GH_Integer)))
            {
                if (Value == null)
                    target = default;
                else
                {
                    GH_Integer ghint = new GH_Integer();
                    if (GH_Convert.ToGHInteger(Value.ID, GH_Conversion.Both, ref ghint))
                        target = (Q)(object)ghint;
                    else
                        target = default;
                }
                return true;
            }

            target = default;
            return false;
        }
        public override bool CastFrom(object source)
        {
            // This function is called when Grasshopper needs to convert other data 
            // into GsaMember.


            if (source == null) { return false; }

            //Cast from GsaMember
            if (typeof(GsaMember3d).IsAssignableFrom(source.GetType()))
            {
                Value = (GsaMember3d)source;
                return true;
            }

            ////Cast from GsaAPI Member
            //if (typeof(Member).IsAssignableFrom(source.GetType()))
            //{
            //    Value.Member = (Member)source;
            //    return true;
            //}

            //Cast from Brep
            Brep brep = new Brep();
            if (GH_Convert.ToBrep(source, ref brep, GH_Conversion.Both))
            {
                GsaMember3d member = new GsaMember3d(brep);
                this.Value = member;
                return true;
            }
            
            //Cast from Mesh
            Mesh mesh = new Mesh();

            if (GH_Convert.ToMesh(source, ref mesh, GH_Conversion.Both))
            {
                GsaMember3d member = new GsaMember3d(mesh);
                this.Value = member;
                return true;
            }

            return false;
        }
        #endregion

        #region transformation methods
        public override IGH_GeometricGoo Transform(Transform xform)
        {
            if (Value == null) { return null; }
            if (Value.SolidMesh == null) { return null; }

            GsaMember3d elem = Value.Duplicate();
            elem.SolidMesh.Transform(xform);

            return new GsaMember3dGoo(elem);
        }

        public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
        {
            if (Value == null) { return null; }
            if (Value.SolidMesh == null) { return null; }

            GsaMember3d elem = Value.Duplicate();
            xmorph.Morph(elem.SolidMesh.Duplicate());

            return new GsaMember3dGoo(elem);
        }

        #endregion

        #region drawing methods
        public BoundingBox ClippingBox
        {
            get { return Boundingbox; }
        }
        public void DrawViewportMeshes(GH_PreviewMeshArgs args)
        {
            //Draw shape.
            if (Value.SolidMesh != null)
            {
                if (!Value.IsDummy)
                {
                    if (args.Material.Diffuse == System.Drawing.Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
                        args.Pipeline.DrawMeshShaded(Value.SolidMesh, UI.Colour.Element2dFace); //UI.Colour.Member2dFace
                    else
                        args.Pipeline.DrawMeshShaded(Value.SolidMesh, UI.Colour.Element2dFaceSelected);
                }
                else
                    args.Pipeline.DrawMeshShaded(Value.SolidMesh, UI.Colour.Dummy2D);
            }
        }
        public void DrawViewportWires(GH_PreviewWireArgs args)
        {
            if (Value == null) { return; }

            //Draw lines
            if (Value.SolidMesh != null)
            {
                // Draw edges
                Rhino.Geometry.Collections.MeshTopologyEdgeList edges = Value.SolidMesh.TopologyEdges;
                if (Value.SolidMesh.FaceNormals.Count < Value.SolidMesh.Faces.Count)
                    Value.SolidMesh.FaceNormals.ComputeFaceNormals();
                if (Value.IsDummy)
                {
                    for (int i = 0; i < edges.Count; i++)
                    {
                        int[] faceID = edges.GetConnectedFaces(i);
                        Vector3d vec1 = Value.SolidMesh.FaceNormals[faceID[0]];
                        Vector3d vec2 = Value.SolidMesh.FaceNormals[faceID[1]];
                        vec1.Unitize(); vec2.Unitize();
                        if (!vec1.Equals(vec2) || faceID.Length > 2)
                        {
                            if (args.Color == System.Drawing.Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
                            {
                                Polyline hidden = new Polyline();
                                hidden.Add(edges.EdgeLine(i).PointAt(0));
                                hidden.Add(edges.EdgeLine(i).PointAt(1));
                                args.Pipeline.DrawDottedPolyline(hidden, UI.Colour.Dummy1D, false);
                            }
                            else
                            {
                                Polyline hidden = new Polyline();
                                hidden.Add(edges.EdgeLine(i).PointAt(0));
                                hidden.Add(edges.EdgeLine(i).PointAt(1));
                                args.Pipeline.DrawDottedPolyline(hidden, UI.Colour.Member2dEdgeSelected, false);
                            }
                        }
                        else
                        {
                            
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < edges.Count; i++)
                    {
                        int[] faceID = edges.GetConnectedFaces(i);
                        Vector3d vec1 = Value.SolidMesh.FaceNormals[faceID[0]];
                        Vector3d vec2 = Value.SolidMesh.FaceNormals[faceID[1]];
                        vec1.Unitize(); vec2.Unitize();
                        if (!vec1.Equals(vec2) || faceID.Length > 2)
                        {
                            if (args.Color == System.Drawing.Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
                            {
                                if ((System.Drawing.Color)Value.Colour != System.Drawing.Color.FromArgb(0, 0, 0))
                                    args.Pipeline.DrawLine(edges.EdgeLine(i), (System.Drawing.Color)Value.Colour, 2);
                                else
                                {
                                    System.Drawing.Color col = UI.Colour.Member2dEdge;
                                    args.Pipeline.DrawLine(edges.EdgeLine(i), col, 2);
                                }
                            }
                            else
                                args.Pipeline.DrawLine(edges.EdgeLine(i), UI.Colour.Element2dEdgeSelected, 2);
                        }
                        else
                        {
                            Polyline hidden = new Polyline();
                            hidden.Add(edges.EdgeLine(i).PointAt(0));
                            hidden.Add(edges.EdgeLine(i).PointAt(1));
                            args.Pipeline.DrawDottedPolyline(hidden, UI.Colour.Dummy1D, false);
                        }
                    }
                }
                // draw points
                List<Point3d> pts = new List<Point3d>(Value.SolidMesh.Vertices.ToPoint3dArray());
                for (int i = 0; i < pts.Count; i++)
                {
                    if (args.Color == System.Drawing.Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
                        args.Pipeline.DrawPoint(pts[i], Rhino.Display.PointStyle.RoundSimple, 2, (Value.IsDummy) ? UI.Colour.Dummy1D : UI.Colour.Member1dNode);
                    else
                        args.Pipeline.DrawPoint(pts[i], Rhino.Display.PointStyle.RoundControlPoint, 3, UI.Colour.Member1dNodeSelected);
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// This class provides a Parameter interface for the Data_GsaMember3d type.
    /// </summary>
    public class GsaMember3dParameter : GH_PersistentGeometryParam<GsaMember3dGoo>, IGH_PreviewObject
    {
        public GsaMember3dParameter()
          : base(new GH_InstanceDescription("3D Member", "M3D", "Maintains a collection of GSA 3D Member data.", GhSA.Components.Ribbon.CategoryName.Name(), GhSA.Components.Ribbon.SubCategoryName.Cat9()))
        {
        }

        public override Guid ComponentGuid => new Guid("7608a5a0-7762-4214-8c30-fb395365056e");

        public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.obscure;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.GsaMem3D;

        //We do not allow users to pick parameter, 
        //therefore the following 4 methods disable all this ui.
        protected override GH_GetterResult Prompt_Plural(ref List<GsaMember3dGoo> values)
        {
            return GH_GetterResult.cancel;
        }
        protected override GH_GetterResult Prompt_Singular(ref GsaMember3dGoo value)
        {
            return GH_GetterResult.cancel;
        }
        protected override System.Windows.Forms.ToolStripMenuItem Menu_CustomSingleValueItem()
        {
            System.Windows.Forms.ToolStripMenuItem item = new System.Windows.Forms.ToolStripMenuItem
            {
                Text = "Not available",
                Visible = false
            };
            return item;
        }
        protected override System.Windows.Forms.ToolStripMenuItem Menu_CustomMultiValueItem()
        {
            System.Windows.Forms.ToolStripMenuItem item = new System.Windows.Forms.ToolStripMenuItem
            {
                Text = "Not available",
                Visible = false
            };
            return item;
        }

        #region preview methods
        public BoundingBox ClippingBox
        {
            get
            {
                return Preview_ComputeClippingBox();
            }
        }
        public void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            //Use a standard method to draw gunk, you don't have to specifically implement this.
            Preview_DrawMeshes(args);
        }
        public void DrawViewportWires(IGH_PreviewArgs args)
        {
            //Use a standard method to draw gunk, you don't have to specifically implement this.
            Preview_DrawWires(args);
        }

        private bool m_hidden = false;
        public bool Hidden
        {
            get { return m_hidden; }
            set { m_hidden = value; }
        }
        public bool IsPreviewCapable
        {
            get { return true; }
        }
        #endregion
    }

}
