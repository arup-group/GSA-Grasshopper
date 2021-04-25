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
        public Node Node { get; set; } = new Node();
        private Plane m_plane; // = Plane.WorldXY;
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
        public Plane LocalAxis
        {
            get { return m_plane; }
            set { m_plane = value; }
        }

        public Point3d Point
        {
            get
            {
                if (Node == null) { return new Point3d(); }
                return new Point3d(Node.Position.X, Node.Position.Y, Node.Position.Z);
            }
            set { Node.Position.X = value.X; Node.Position.Y = value.Y; Node.Position.Z = value.Z; }
        }

        public System.Drawing.Color Colour
        {
            get
            {
                return (System.Drawing.Color)Node.Colour;
            }
            set { Node.Colour = value; }
        }

        #region constructors
        public GsaNode()
        {
            Node = null;
        }

        public GsaNode(Point3d position)
        {
            Node.Position.X = position.X;
            Node.Position.Y = position.Y;
            Node.Position.Z = position.Z;
        }

        
        public GsaNode Duplicate()
        {
            if (this.Node == null) { return null; }
            GsaNode dup = new GsaNode
            {
                Node = new Node
                {
                    AxisProperty = Node.AxisProperty,
                    DamperProperty = Node.DamperProperty,
                    MassProperty = Node.MassProperty,
                    Name = Node.Name,
                    Restraint = new NodalRestraint
                    {
                        X = Node.Restraint.X,
                        Y = Node.Restraint.Y,
                        Z = Node.Restraint.Z,
                        XX = Node.Restraint.XX,
                        YY = Node.Restraint.YY,
                        ZZ = Node.Restraint.ZZ,
                    },
                    SpringProperty = Node.SpringProperty,
                }
            };

            if ((System.Drawing.Color)Node.Colour != System.Drawing.Color.FromArgb(0, 0, 0)) // workaround to handle that System.Drawing.Color is non-nullable type
                dup.Node.Colour = Node.Colour;

            dup.Point = new Point3d
            {
                X = Node.Position.X,
                Y = Node.Position.Y,
                Z = Node.Position.Z,
            };

            if (m_id != 0)
                dup.ID = m_id;
            if(m_spring != null)
                dup.Spring = m_spring.Duplicate();
            if (m_plane != null)
                dup.LocalAxis = LocalAxis.Clone();
            return dup;
        }


        #endregion

        #region properties
        public bool IsValid
        {
            get
            {
                if (Node == null) { return false; }
                return true;
            }
        }

        #endregion

        #region methods
        public override string ToString()
        {
            if (Node == null) { return "Null Node"; }
            string idd = " " + ID.ToString() + " ";
            if (ID == 0) { idd = " "; }
            GH_Point gH_Point = new GH_Point(Point);
            string nodeTxt = "Gsa Node" + idd + gH_Point.ToString();

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
            if (Node.Restraint.X == false && Node.Restraint.Y == false && Node.Restraint.Z == false &&
                Node.Restraint.XX == false && Node.Restraint.YY == false && Node.Restraint.ZZ == false)
                sptTxt = "";
            else
            {
                sptTxt = " Restraint: " + "X: " + (Node.Restraint.X ? "\u2713" : "\u2610") +
                   ", Y: " + (Node.Restraint.Y ? "\u2713" : "\u2610") +
                   ", Z: " + (Node.Restraint.Z ? "\u2713" : "\u2610") +
                   ", XX: " + (Node.Restraint.XX ? "\u2713" : "\u2610") +
                   ", YY: " + (Node.Restraint.YY ? "\u2713" : "\u2610") +
                   ", ZZ: " + (Node.Restraint.ZZ ? "\u2713" : "\u2610");
                if (!Node.Restraint.X & !Node.Restraint.Y & !Node.Restraint.Z &
                    !Node.Restraint.XX & !Node.Restraint.YY & !Node.Restraint.ZZ)
                    sptTxt = "";
                if (Node.Restraint.X & Node.Restraint.Y & Node.Restraint.Z &
                    !Node.Restraint.XX & !Node.Restraint.YY & !Node.Restraint.ZZ)
                    sptTxt = " Restraint: Pinned";
                if (Node.Restraint.X & Node.Restraint.Y & Node.Restraint.Z &
                    Node.Restraint.XX & Node.Restraint.YY & Node.Restraint.ZZ)
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
                if (node.Node == null)
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
            Point3d pt = Value.Point;
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
                    target = (Q)(object)Value.Node;
                return true;
            }

            //Cast to Point3d
            if (typeof(Q).IsAssignableFrom(typeof(Point3d)))
            {
                if (Value == null)
                    target = default;
                else
                    target = (Q)(object)Value.Point;
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
                Value.Node = (Node)source;
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
            if (Value.Point == null) { return null; }

            GsaNode node = Value.Duplicate();
            Point3d pt = node.Point;
            pt.Transform(xform);
            
            node.Point = pt;
            return new GsaNodeGoo(node);
        }

        public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
        {
            if (Value == null) { return null; }
            if (Value.Point == null) { return null; }

            GsaNode node = Value.Duplicate();

            Point3d pt = node.Point;
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
                    DrawRestraint(args, Value);
                }
                else
                {
                    args.Pipeline.DrawPoint(Value.Point, Rhino.Display.PointStyle.RoundControlPoint, 3, UI.Colour.NodeSelected);
                    DrawRestraint(args, Value, true);
                }

                // local axis
                if (Value.LocalAxis != Plane.WorldXY)
                {
                    Plane plane = Value.LocalAxis.Clone();
                    plane.Origin = Value.Point;
                    args.Pipeline.DrawLine(new Line(Value.Point, plane.XAxis), System.Drawing.Color.FromArgb(255, 244, 96, 96), 1);
                    args.Pipeline.DrawLine(new Line(Value.Point, plane.YAxis), System.Drawing.Color.FromArgb(255, 96, 244, 96), 1);
                    args.Pipeline.DrawLine(new Line(Value.Point, plane.ZAxis), System.Drawing.Color.FromArgb(255, 96, 96, 234), 1);
                }
            }
        }

        private void DrawRestraint(GH_PreviewWireArgs args, GsaNode node, bool selected = false)
        {
            if (node.Node.Restraint.X == false & node.Node.Restraint.Y == false & node.Node.Restraint.Z == false &
                node.Node.Restraint.XX == false & node.Node.Restraint.YY == false & node.Node.Restraint.ZZ == false)
                return;

            // colour
            System.Drawing.Color col;
            if (selected)
                col = UI.Colour.NodeSelected;
            else
            {
                if ((System.Drawing.Color)node.Colour != System.Drawing.Color.FromArgb(0, 0, 0))
                {
                    col = node.Colour;
                }
                else
                    col = UI.Colour.Support;
            }

            // pin
            if (node.Node.Restraint.X == true & node.Node.Restraint.Y == true & node.Node.Restraint.Z == true &
                node.Node.Restraint.XX == false & node.Node.Restraint.YY == false & node.Node.Restraint.ZZ == false)
            {
                Plane plane = node.LocalAxis.Clone();
                if (!plane.IsValid) { plane = Plane.WorldXY; }
                plane.Origin = node.Point;
                Cone pin = new Cone(plane, -0.4, 0.4);
                DisplayMaterial material = new DisplayMaterial()
                {
                    Diffuse = System.Drawing.Color.FromArgb(255, col.R, col.G, col.B),
                    Emission = System.Drawing.Color.FromArgb(255, 50, 50, 50),
                    Transparency = 0.2
                };
                args.Pipeline.DrawBrepShaded(pin.ToBrep(true), material);
                args.Pipeline.DrawCone(pin, col);
            }
            else if (node.Node.Restraint.X == true & node.Node.Restraint.Y == true & node.Node.Restraint.Z == true &
                    node.Node.Restraint.XX == true & node.Node.Restraint.YY == true & node.Node.Restraint.ZZ == true)
            {
                Plane plane = node.LocalAxis.Clone();
                plane.Origin = node.Point;
                Box fix = new Box(plane, new Interval(-0.3, 0.3), new Interval(-0.3, 0.3), new Interval(-0.2, 0));
                DisplayMaterial material = new DisplayMaterial()
                {
                    Diffuse = System.Drawing.Color.FromArgb(255, col.R, col.G, col.B),
                    Emission = System.Drawing.Color.FromArgb(255, 50, 50, 50),
                    Transparency = 0.2
                };
                args.Pipeline.DrawBrepShaded(fix.ToBrep(), material);
                args.Pipeline.DrawBox(fix, col);
            }
            else
            {
                Plane plane = node.LocalAxis.Clone();
                plane.Origin = node.Point;
                string rest = "";
                if (node.Node.Restraint.X == true)
                    rest += "X";
                if (node.Node.Restraint.Y == true)
                    rest += "Y";
                if (node.Node.Restraint.Z == true)
                    rest += "Z";
                if (node.Node.Restraint.XX == true)
                    rest += "XX";
                if (node.Node.Restraint.YY == true)
                    rest += "YY";
                if (node.Node.Restraint.ZZ == true)
                    rest += "ZZ";
                Rhino.Display.Text3d text = new Text3d(rest, plane, 0.3);
                text.HorizontalAlignment = Rhino.DocObjects.TextHorizontalAlignment.Left;
                text.VerticalAlignment = Rhino.DocObjects.TextVerticalAlignment.Top;
                args.Pipeline.Draw3dText(text, col);
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
