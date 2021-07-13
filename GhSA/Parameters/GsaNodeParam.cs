using System;
using System.Collections.Generic;

using GsaAPI;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Rhino.Display;
using Rhino.Collections;

namespace GhSA.Parameters
{
    /// <summary>
    /// Node class, this class defines the basic properties and methods for any Gsa Node
    /// </summary>
    public class GsaNode
    {
        public GsaSpring Spring
        {
            get { return m_spring; }
            set { m_spring = value; }
        }
        public int ID
        {
            get { return m_id; }
            set { m_id = value; }
        }
        public Plane LocalAxis
        {
            get { return m_plane; }
            set 
            { 
                m_plane = value;
                UpdatePreview();
            }
        }
        public Point3d Point
        {
            get
            {
                if (m_node == null) { return Point3d.Unset; }
                return new Point3d(m_node.Position.X, m_node.Position.Y, m_node.Position.Z);
            }
            set 
            {
                CloneNode();
                m_id = 0;
                m_node.Position.X = value.X; 
                m_node.Position.Y = value.Y; 
                m_node.Position.Z = value.Z;
                UpdatePreview();
            }
        }
        #region GsaAPI members
        internal Node API_Node
        {
            get { return m_node; }
            set 
            { 
                m_node = value; 
                UpdatePreview(); 
            }
        }
        public System.Drawing.Color Colour
        {
            get
            {
                return (System.Drawing.Color)m_node.Colour;
            }
            set 
            {
                CloneNode();
                m_node.Colour = value; 
            }
        }
        public GsaBool6 Restraint
        {
            get
            {
                return new GsaBool6(m_node.Restraint.X, m_node.Restraint.Y, m_node.Restraint.Z,
                    m_node.Restraint.XX, m_node.Restraint.YY, m_node.Restraint.ZZ);
            }
            set
            {
                CloneNode();
                m_node.Restraint = new NodalRestraint
                {
                    X = value.X,
                    Y = value.Y,
                    Z = value.Z,
                    XX = value.XX,
                    YY = value.YY,
                    ZZ = value.ZZ,
                };
                UpdatePreview();
            }
        }
        public string Name
        {
            get { return m_node.Name; }
            set
            {
                CloneNode();
                m_node.Name = value;
            }
        }
        private void CloneNode()
        {
            Node node = new Node
            {
                AxisProperty = m_node.AxisProperty,
                DamperProperty = m_node.DamperProperty,
                MassProperty = m_node.MassProperty,
                Name = m_node.Name.ToString(),
                Restraint = new NodalRestraint
                {
                    X = m_node.Restraint.X,
                    Y = m_node.Restraint.Y,
                    Z = m_node.Restraint.Z,
                    XX = m_node.Restraint.XX,
                    YY = m_node.Restraint.YY,
                    ZZ = m_node.Restraint.ZZ,
                },
                SpringProperty = m_node.SpringProperty,
                Position = new Vector3
                {
                    X = m_node.Position.X,
                    Y = m_node.Position.Y,
                    Z = m_node.Position.Z,
                }
            };

            if ((System.Drawing.Color)m_node.Colour != System.Drawing.Color.FromArgb(0, 0, 0)) // workaround to handle that System.Drawing.Color is non-nullable type
                node.Colour = m_node.Colour;

            m_node = node;
        }
        #endregion
        #region preview
        internal Line previewXaxis;
        internal Line previewYaxis;
        internal Line previewZaxis;
        internal Brep previewSupportSymbol;
        internal Rhino.Display.Text3d previewText;
        internal void UpdatePreview()
        {
            if (m_node.Restraint.X || m_node.Restraint.Y || m_node.Restraint.Z ||
                m_node.Restraint.XX || m_node.Restraint.YY || m_node.Restraint.ZZ)
            {
                GhSA.UI.Display.PreviewRestraint(Restraint, m_plane, Point, 
                    ref previewSupportSymbol, ref previewText);
            }
            else
            {
                previewSupportSymbol = null;
                previewText = null;
            }

            if (m_plane != null)
            {
                if (m_plane != Plane.WorldXY & m_plane != new Plane())
                {
                    Plane local = m_plane.Clone();
                    local.Origin = Point;
                    
                    previewXaxis = new Line(Point, local.XAxis, 0.5);
                    previewYaxis = new Line(Point, local.YAxis, 0.5);
                    previewZaxis = new Line(Point, local.ZAxis, 0.5);
                }
            }
        }
        #endregion
        #region fields
        private Plane m_plane; 
        private int m_id;
        private GsaSpring m_spring;
        private Node m_node; 
        #endregion
        
        #region constructors
        public GsaNode()
        {
            m_node = new Node();
        }

        public GsaNode(Point3d position, int ID = 0)
        {
            m_node = new Node();
            Point = position;
            m_id = ID;
            UpdatePreview();
        }
        public GsaNode Duplicate()
        {
            if (m_node == null) { return null; }
            GsaNode dup = new GsaNode();
            dup.m_id = m_id;
            dup.m_node = m_node;
            dup.m_plane = m_plane;
            dup.m_spring = m_spring;
            dup.UpdatePreview();
            return dup;
        }
        #endregion

        #region properties
        public bool IsValid
        {
            get
            {
                if (API_Node == null) { return false; }
                return true;
            }
        }

        #endregion

        #region methods
        public override string ToString()
        {
            if (API_Node == null) { return "Null Node"; }
            string idd = " " + ID.ToString() + " ";
            if (ID == 0) { idd = " "; }
            GH_Point gH_Point = new GH_Point(Point);
            string nodeTxt = "GSA Node" + idd + gH_Point.ToString();

            string localTxt = "";
            Plane noPlane = new Plane() { Origin = new Point3d(0, 0, 0), XAxis = new Vector3d(0, 0, 0), YAxis = new Vector3d(0, 0, 0), ZAxis = new Vector3d(0, 0, 0) };
            if (LocalAxis != noPlane)
            {
                if (LocalAxis != Plane.WorldXY)
                {
                    GH_Plane gH_Plane = new GH_Plane(LocalAxis);
                    localTxt = " Local axis: {" + gH_Plane.ToString() + "}";
                }
            }
            
            string sptTxt;
            if (API_Node.Restraint.X == false && API_Node.Restraint.Y == false && API_Node.Restraint.Z == false &&
                API_Node.Restraint.XX == false && API_Node.Restraint.YY == false && API_Node.Restraint.ZZ == false)
                sptTxt = "";
            else
            {
                sptTxt = " Restraint: " + "X: " + (API_Node.Restraint.X ? "\u2713" : "\u2610") +
                   ", Y: " + (API_Node.Restraint.Y ? "\u2713" : "\u2610") +
                   ", Z: " + (API_Node.Restraint.Z ? "\u2713" : "\u2610") +
                   ", XX: " + (API_Node.Restraint.XX ? "\u2713" : "\u2610") +
                   ", YY: " + (API_Node.Restraint.YY ? "\u2713" : "\u2610") +
                   ", ZZ: " + (API_Node.Restraint.ZZ ? "\u2713" : "\u2610");
                if (!API_Node.Restraint.X & !API_Node.Restraint.Y & !API_Node.Restraint.Z &
                    !API_Node.Restraint.XX & !API_Node.Restraint.YY & !API_Node.Restraint.ZZ)
                    sptTxt = "";
                if (API_Node.Restraint.X & API_Node.Restraint.Y & API_Node.Restraint.Z &
                    !API_Node.Restraint.XX & !API_Node.Restraint.YY & !API_Node.Restraint.ZZ)
                    sptTxt = " Restraint: Pinned";
                if (API_Node.Restraint.X & API_Node.Restraint.Y & API_Node.Restraint.Z &
                    API_Node.Restraint.XX & API_Node.Restraint.YY & API_Node.Restraint.ZZ)
                    sptTxt = " Restraint: Fixed";
            }

            return nodeTxt + sptTxt + localTxt;
        }
        #endregion
    }

    /// <summary>
    /// GsaNode Goo wrapper class, makes sure GsaNode can be used in Grasshopper.
    /// </summary>
    public class GsaNodeGoo : GH_GeometricGoo<GsaNode>, IGH_PreviewData
    {
        #region constructors
        public GsaNodeGoo()
        {
            this.Value = new GsaNode();
        }
        public GsaNodeGoo(GsaNode node)
        {
            if (node == null)
                node = null;
            else
            {
                if (node.API_Node == null)
                    node = null;
            }
            this.Value = node; //node.Duplicate();
        }

        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return DuplicateGsaNode();
        }
        public GsaNodeGoo DuplicateGsaNode()
        {
            return new GsaNodeGoo(Value == null ? new GsaNode() : Value); //Value.Duplicate());
        }
        #endregion

        #region properties
        public override bool IsValid
        {
            get
            {
                if (Value == null) { return false; }
                return Value.IsValid;
            }
        }
        public override string IsValidWhyNot
        {
            get
            {
                //if (Value == null) { return "No internal BoatShell instance"; }
                if (Value.IsValid) { return string.Empty; }
                return Value.Point.IsValid.ToString(); //Todo: beef this up to be more informative.
            }
        }
        public override string ToString()
        {
            if (Value == null)
                return "Null Node";
            else
                return Value.ToString();
        }
        public override string TypeName
        {
            get { return ("Node"); }
        }
        public override string TypeDescription
        {
            get { return ("GSA Node"); }
        }

        public override BoundingBox Boundingbox
        {
            get
            {
                if (Value == null) { return BoundingBox.Empty; }
                if (Value.Point == null) { return BoundingBox.Empty; }
                Point3d pt1 = Value.Point;
                pt1.Z += 0.25;
                Point3d pt2 = Value.Point;
                pt2.Z += -0.25;
                Line ln = new Line(pt1, pt2);
                LineCurve crv = new LineCurve(ln);
                return crv.GetBoundingBox(false); 
            }
        }
        public override BoundingBox GetBoundingBox(Transform xform)
        {
            if (Value == null) { return BoundingBox.Empty; }
            if (Value.Point == null) { return BoundingBox.Empty; }
            Point3d pt = new Point3d(Value.Point);
            pt.Z += 0.001;
            Line ln = new Line(Value.Point, pt);
            LineCurve crv = new LineCurve(ln);
            return crv.GetBoundingBox(xform); //BoundingBox.Empty; //Value.point.GetBoundingBox(xform);
        }
        #endregion

        #region casting methods
        public override bool CastTo<Q>(out Q target)
        {
            // This function is called when Grasshopper needs to convert this 
            // instance of GsaNode into some other type Q.            

            if (typeof(Q).IsAssignableFrom(typeof(GsaNode)))
            {
                if (Value == null)
                    target = default;
                else
                    target = (Q)(object)Value.Duplicate();
                return true;
            }

            if (typeof(Q).IsAssignableFrom(typeof(Node)))
            {
                if (Value == null)
                    target = default;
                else
                    target = (Q)(object)Value.API_Node;
                return true;
            }

            //Cast to Point3d
            if (typeof(Q).IsAssignableFrom(typeof(Point3d)))
            {
                if (Value == null)
                    target = default;
                else
                    target = (Q)(object)new Point3d(Value.Point);
                return true;
            }
            if (typeof(Q).IsAssignableFrom(typeof(GH_Point)))
            {
                if (Value == null)
                    target = default;
                else
                    target = (Q)(object)new GH_Point(Value.Point);
                return true;
            }

            if (typeof(Q).IsAssignableFrom(typeof(Point)))
            {
                if (Value == null)
                    target = default;
                else
                    target = (Q)(object)new Point(Value.Point);
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
            // into GsaNode.


            if (source == null) { return false; }

            //Cast from GsaNode
            if (typeof(GsaNode).IsAssignableFrom(source.GetType()))
            {
                Value = (GsaNode)source;
                return true;
            }

            //Cast from GsaAPI Node
            if (typeof(Node).IsAssignableFrom(source.GetType()))
            {
                Value = new GsaNode();
                Value.API_Node = (Node)source;
                return true;
            }

            //Cast from Point3d
            Point3d pt = new Point3d();
            if (GH_Convert.ToPoint3d(source, ref pt, GH_Conversion.Both))
            {
                Value = new GsaNode(pt);
                return true;
            }

            return false;
        }
        #endregion

        #region transformation methods
        public override IGH_GeometricGoo Transform(Transform xform)
        {
            if (Value == null) { return null; }
            if (Value.Point == null) { return null; }

            GsaNode node = Value.Duplicate();
            Point3d pt = new Point3d(node.Point);
            pt.Transform(xform);
            
            node.Point = pt;
            return new GsaNodeGoo(node);
        }

        public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
        {
            if (Value == null) { return null; }
            if (Value.Point == null) { return null; }

            GsaNode node = Value.Duplicate();

            Point3d pt = new Point3d(node.Point);
            pt = xmorph.MorphPoint(pt);

            node.Point = pt;

            return new GsaNodeGoo(node);
        }

        #endregion

        #region drawing methods
        public BoundingBox ClippingBox
        {
            get { return Boundingbox; }
        }
        public void DrawViewportMeshes(GH_PreviewMeshArgs args) 
        {
            //No meshes are drawn.   
        }
        public void DrawViewportWires(GH_PreviewWireArgs args)
        {
            if (Value == null) { return; }
            
            if (Value.Point.IsValid)
            {
                // draw the point
                if (args.Color == System.Drawing.Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
                {
                    if ((System.Drawing.Color)Value.Colour != System.Drawing.Color.FromArgb(0, 0, 0))
                    {
                        args.Pipeline.DrawPoint(Value.Point, Rhino.Display.PointStyle.RoundSimple, 3, (System.Drawing.Color)Value.Colour);
                    }
                    else
                    {
                        System.Drawing.Color col = UI.Colour.Node;
                        args.Pipeline.DrawPoint(Value.Point, Rhino.Display.PointStyle.RoundSimple, 3, col);
                    }
                    if (Value.previewSupportSymbol != null)
                        args.Pipeline.DrawBrepShaded(Value.previewSupportSymbol, UI.Colour.SupportSymbol);
                    if (Value.previewText != null)
                        args.Pipeline.Draw3dText(Value.previewText, UI.Colour.Support);
                }
                else
                {
                    args.Pipeline.DrawPoint(Value.Point, Rhino.Display.PointStyle.RoundControlPoint, 3, UI.Colour.NodeSelected);
                    if (Value.previewSupportSymbol != null)
                        args.Pipeline.DrawBrepShaded(Value.previewSupportSymbol, UI.Colour.SupportSymbolSelected);
                    if (Value.previewText != null)
                        args.Pipeline.Draw3dText(Value.previewText, UI.Colour.NodeSelected);
                }

                // local axis
                if (Value.LocalAxis != Plane.WorldXY & Value.LocalAxis != new Plane() & Value.LocalAxis != Plane.Unset)
                {
                    args.Pipeline.DrawLine(Value.previewXaxis, System.Drawing.Color.FromArgb(255, 244, 96, 96), 1);
                    args.Pipeline.DrawLine(Value.previewYaxis, System.Drawing.Color.FromArgb(255, 96, 244, 96), 1);
                    args.Pipeline.DrawLine(Value.previewZaxis, System.Drawing.Color.FromArgb(255, 96, 96, 234), 1);
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// This class provides a Parameter interface for the Data_GsaNode type.
    /// </summary>
    public class GsaNodeParameter : GH_PersistentGeometryParam<GsaNodeGoo>, IGH_PreviewObject
    {
        public GsaNodeParameter()
          : base(new GH_InstanceDescription("Node", "No", "Maintains a collection of GSA Node data.", GhSA.Components.Ribbon.CategoryName.Name(), GhSA.Components.Ribbon.SubCategoryName.Cat9()))
        {
        }

        public override Guid ComponentGuid => new Guid("8ebdc693-e882-494d-8177-b0bd9c3d84a3");

        public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.obscure;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.GsaNode;

        //We do not allow users to pick parameter, 
        //therefore the following 4 methods disable all this ui.
        protected override GH_GetterResult Prompt_Plural(ref List<GsaNodeGoo> values)
        {
            return GH_GetterResult.cancel;
        }
        protected override GH_GetterResult Prompt_Singular(ref GsaNodeGoo value)
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
            //Meshes aren't drawn.
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
