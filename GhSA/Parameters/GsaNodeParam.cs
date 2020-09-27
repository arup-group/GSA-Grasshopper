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
        #region fields
        public Node node { get; set; } = new Node();
        private Plane m_plane;
        private int m_id;
        private GsaSpring m_spring;
        #endregion

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

        #region constructors
        public GsaNode()
        {
            node = null;
        }

        public GsaNode(Point3d position)
        {
            node.Position.X = position.X;
            node.Position.Y = position.Y;
            node.Position.Z = position.Z;
            m_plane = Plane.WorldXY;
            m_plane.Origin = position;
        }

        public GsaNode(Point3d position, int ID)
        {
            node.Position.X = position.X;
            node.Position.Y = position.Y;
            node.Position.Z = position.Z;
            m_id = ID;
            m_plane = Plane.WorldXY;
            m_plane.Origin = position;
        }

        public GsaNode(Point3d position, GsaBool6 bool6)
        {
            node.Position.X = position.X;
            node.Position.Y = position.Y;
            node.Position.Z = position.Z;
            node.Restraint.X = bool6.X;
            node.Restraint.Y = bool6.Y;
            node.Restraint.Z = bool6.Z;
            node.Restraint.XX = bool6.XX;
            node.Restraint.YY = bool6.YY;
            node.Restraint.ZZ = bool6.ZZ;
            m_plane = Plane.WorldXY;
            m_plane.Origin = position;
        }

        public GsaNode(Point3d position, int ID, GsaBool6 bool6, Plane plane)
        {
            node.Position.X = position.X;
            node.Position.Y = position.Y;
            node.Position.Z = position.Z;
            m_id = ID;
            node.Restraint.X = bool6.X;
            node.Restraint.Y = bool6.Y;
            node.Restraint.Z = bool6.Z;
            node.Restraint.XX = bool6.XX;
            node.Restraint.YY = bool6.YY;
            node.Restraint.ZZ = bool6.ZZ;
            m_plane = plane;
            m_plane.Origin = position;
        }
        public GsaNode(Point3d position, bool restraintX, bool restraintY, bool restraintZ, bool restraintXX, bool restraintYY, bool restraintZZ)
        {
            node.Position.X = position.X;
            node.Position.Y = position.Y;
            node.Position.Z = position.Z;
            node.Restraint.X = restraintX; 
            node.Restraint.Y = restraintY;
            node.Restraint.Z = restraintZ;
            node.Restraint.XX = restraintXX; 
            node.Restraint.YY = restraintYY;
            node.Restraint.ZZ = restraintZZ;
            m_plane = Plane.WorldXY;
            m_plane.Origin = position;
        }
        public GsaNode(Point3d position, bool restraintX, bool restraintY, bool restraintZ, bool restraintXX, bool restraintYY, bool restraintZZ, Plane localPlane)
        {
            node.Position.X = position.X;
            node.Position.Y = position.Y;
            node.Position.Z = position.Z;
            node.Restraint.X = restraintX;
            node.Restraint.Y = restraintY;
            node.Restraint.Z = restraintZ;
            node.Restraint.XX = restraintXX;
            node.Restraint.YY = restraintYY;
            node.Restraint.ZZ = restraintZZ;
            m_plane = localPlane;
            m_plane.Origin = position;
        }
        public GsaNode(Point3d position, bool restraintX, bool restraintY, bool restraintZ, bool restraintXX, bool restraintYY, bool restraintZZ, Plane localPlane,
            string name, System.Drawing.Color colour, int damperProp, int massProp, int springProp)
        {
            node.Position.X = position.X;
            node.Position.Y = position.Y;
            node.Position.Z = position.Z;
            node.Restraint.X = restraintX;
            node.Restraint.Y = restraintY;
            node.Restraint.Z = restraintZ;
            node.Restraint.XX = restraintXX;
            node.Restraint.YY = restraintYY;
            node.Restraint.ZZ = restraintZZ;
            m_plane = localPlane;
            m_plane.Origin = position;
            node.Name = name;
            node.Colour = colour;
            node.DamperProperty = damperProp;
            node.MassProperty = massProp;
            node.SpringProperty = springProp;
        }

        public GsaNode Duplicate()
        {
            GsaNode dup = new GsaNode(); 
            dup.node = node; //add clone or duplicate if available
            Point3dList pt = new Point3dList(point);
            Point3dList duppt = pt.Duplicate();
            dup.point = duppt[0];
            if (m_id != 0)
                dup.ID = m_id;
            if(m_spring != null)
                dup.Spring = m_spring.Duplicate();
            dup.m_plane = localAxis.Clone();
            return dup;
        }


        #endregion

        #region properties
        public bool IsValid
        {
            get
            {
                if (node == null) { return false; }
                return true;
            }
        }
       
        
        public Plane localAxis
        {
            get { return m_plane; }
            set { m_plane = value; }
        }

        public Point3d point
        {
            get 
            { 
                if (node == null) { return new Point3d(); }
                return new Point3d(node.Position.X, node.Position.Y, node.Position.Z); 
            }
            set { node.Position.X = value.X; node.Position.Y = value.Y; node.Position.Z = value.Z; }
        }

        #endregion

        #region methods
        public override string ToString()
        {
            if (node == null) { return "Null Node"; }
            string idd = " " + ID.ToString() + " ";
            if (ID == 0) { idd = " "; }
            string nodeTxt = "Gsa Node" + idd + "(" + point.ToString() + ") " + System.Environment.NewLine;
            string localTxt = "";
            if (localAxis != Plane.WorldXY)
                localTxt = System.Environment.NewLine + "Local axis (" + localAxis.ToString() + ") ";
            string sptTxt = "Free";

            sptTxt = "X: " + (node.Restraint.X ? "Fix" : "Free") +
                   ", Y: " + (node.Restraint.Y ? "Fix" : "Free") +
                   ", Z: " + (node.Restraint.Z ? "Fix" : "Free") +
                   ", XX: " + (node.Restraint.XX ? "Fix" : "Free") +
                   ", YY: " + (node.Restraint.YY ? "Fix" : "Free") +
                   ", ZZ: " + (node.Restraint.ZZ ? "Fix" : "Free");
            if (!node.Restraint.X & !node.Restraint.Y & !node.Restraint.Z &
                !node.Restraint.XX & !node.Restraint.YY & !node.Restraint.ZZ)
                sptTxt = "Free";
            if (node.Restraint.X & node.Restraint.Y & node.Restraint.Z &
                !node.Restraint.XX & !node.Restraint.YY & !node.Restraint.ZZ)
                sptTxt = "Pinned";
            if (node.Restraint.X & node.Restraint.Y & node.Restraint.Z &
                node.Restraint.XX & node.Restraint.YY & node.Restraint.ZZ)
                sptTxt = "Fixed";

            return nodeTxt + "Restraint: " + sptTxt + localTxt;
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
                if (node.node == null)
                    node = null;
            }
            this.Value = node;
        }

        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return DuplicateGsaNode();
        }
        public GsaNodeGoo DuplicateGsaNode()
        {
            return new GsaNodeGoo(Value == null ? new GsaNode() : Value.Duplicate());
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
                return Value.point.IsValid.ToString(); //Todo: beef this up to be more informative.
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
                if (Value.point == null) { return BoundingBox.Empty; }
                Point3d pt = Value.point;
                pt.Z += 0.001;
                Line ln = new Line(Value.point, pt);
                LineCurve crv = new LineCurve(ln);
                return crv.GetBoundingBox(false); 
            }
        }
        public override BoundingBox GetBoundingBox(Transform xform)
        {
            if (Value == null) { return BoundingBox.Empty; }
            if (Value.point == null) { return BoundingBox.Empty; }
            Point3d pt = Value.point;
            pt.Z += 0.001;
            Line ln = new Line(Value.point, pt);
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
                    target = default(Q);
                else
                    target = (Q)(object)Value;
                return true;
            }

            if (typeof(Q).IsAssignableFrom(typeof(Node)))
            {
                if (Value == null)
                    target = default(Q);
                else
                    target = (Q)(object)Value.node;
                return true;
            }

            //Cast to Point3d
            if (typeof(Q).IsAssignableFrom(typeof(Point3d)))
            {
                if (Value == null)
                    target = default(Q);
                else
                    target = (Q)(object)Value.point;
                return true;
            }
            if (typeof(Q).IsAssignableFrom(typeof(GH_Point)))
            {
                if (Value == null)
                    target = default(Q);
                else
                    target = (Q)(object)new GH_Point(Value.point);
                return true;
            }

            if (typeof(Q).IsAssignableFrom(typeof(Point)))
            {
                if (Value == null)
                    target = default(Q);
                else
                    target = (Q)(object)new Point(Value.point);
                return true;
            }

            target = default(Q);
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
                Value.node = (Node)source;
                return true;
            }

            //Cast from Point3d
            Point3d pt = new Point3d();

            if (GH_Convert.ToPoint3d(source, ref pt, GH_Conversion.Both))
            {
                GsaNode node = new GsaNode(pt);
                this.Value = node;
                return true;
            }
            

            return false;
        }
        #endregion

        #region transformation methods
        public override IGH_GeometricGoo Transform(Transform xform)
        {
            if (Value == null) { return null; }
            if (Value.point == null) { return null; }

            Point3d pt = Value.point;
            pt.Transform(xform);
            GsaNode node = new GsaNode(pt);
            return new GsaNodeGoo(node);
        }

        public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
        {
            if (Value == null) { return null; }
            if (Value.point == null) { return null; }

            Point3d pt = Value.point;
            pt = xmorph.MorphPoint(pt);
            GsaNode node = new GsaNode(pt);
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


            if (Value.point.IsValid)
            {
                if (args.Color == System.Drawing.Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
                    args.Pipeline.DrawPoint(Value.point, Rhino.Display.PointStyle.RoundSimple, 3, UI.Colour.Node);
                else
                    args.Pipeline.DrawPoint(Value.point, Rhino.Display.PointStyle.RoundControlPoint, 3, UI.Colour.NodeSelected);
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
          : base(new GH_InstanceDescription("GSA Node", "Node", "Maintains a collection of GSA Node data.", GhSA.Components.Ribbon.CategoryName.name(), GhSA.Components.Ribbon.SubCategoryName.cat9()))
        {
        }

        public override Guid ComponentGuid => new Guid("8ebdc693-e882-494d-8177-b0bd9c3d84a3");

        public override GH_Exposure Exposure => GH_Exposure.tertiary;

        //protected override Bitmap Icon => Resources.CrossSections;

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
            System.Windows.Forms.ToolStripMenuItem item = new System.Windows.Forms.ToolStripMenuItem();
            item.Text = "Not available";
            item.Visible = false;
            return item;
        }
        protected override System.Windows.Forms.ToolStripMenuItem Menu_CustomMultiValueItem()
        {
            System.Windows.Forms.ToolStripMenuItem item = new System.Windows.Forms.ToolStripMenuItem();
            item.Text = "Not available";
            item.Visible = false;
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
