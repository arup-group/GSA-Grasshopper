﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
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
    /// Element2d class, this class defines the basic properties and methods for any Gsa Element 2d
    /// </summary>
    public class GsaElement2d

    {
        public List<Element> Elements
        {
            get { return m_elements; }
            set { m_elements = value; }
        }
        public Mesh Mesh
        {
            get { return m_mesh; }
            set { m_mesh = value; }
        }
        public List<Point3d> Topology
        {
            get { return m_topo; }
            set { m_topo = value; }
        }
        public List<int> ID
        {
            get { return m_id; }
            set { m_id = value; }
        }
        public List<List<int>> TopoInt
        {
            get { return m_topoInt; }
            set { m_topoInt = value; }
        }
        public List<GsaProp2d> Properties
        {
            get { return m_props; }
            set { m_props = value; }
        }
        public List<System.Drawing.Color> Colours
        {
            get
            {
                List<System.Drawing.Color> cols = new List<System.Drawing.Color>();
                for (int i = 0; i < m_elements.Count; i++)
                {
                    if ((System.Drawing.Color)m_elements[i].Colour == System.Drawing.Color.FromArgb(0, 0, 0))
                    {
                        m_elements[i].Colour = System.Drawing.Color.FromArgb(50, 150, 150, 150);
                    }
                    cols.Add((System.Drawing.Color)m_elements[i].Colour);

                    Mesh.VertexColors.SetColor(i, (System.Drawing.Color)m_elements[i].Colour);
                }
                return cols;
            }
            set 
            {
                for (int i = 0; i < m_elements.Count; i++)
                {
                    if (value[i] != null)
                    {
                        m_elements[i].Colour = value[i];
                        Mesh.VertexColors.SetColor(i, (System.Drawing.Color)m_elements[i].Colour);
                    }
                }
            }
        }
        #region fields
        private List<Element> m_elements; 
        private Mesh m_mesh;
        private List<List<int>> m_topoInt; // list of topology integers referring to the topo list of points
        private List<Point3d> m_topo; // list of topology points for visualisation
        private List<int> m_id;
        private List<GsaProp2d> m_props;
        #endregion

        #region constructors
        public GsaElement2d()
        {
            m_elements = new List<Element>();
            m_mesh = new Mesh();
            m_props = new List<GsaProp2d>();
        }

        public GsaElement2d(Mesh mesh, int prop = 0)
        {
            m_elements = new List<Element>();
            m_mesh = mesh;
            Tuple<List<Element>, List<Point3d>, List<List<int>>> convertMesh = Util.GH.Convert.ConvertMeshToElem2d(mesh, prop);
            m_elements = convertMesh.Item1;
            m_topo = convertMesh.Item2;
            m_topoInt = convertMesh.Item3;
            
            m_id = new List<int>(new int[m_mesh.Faces.Count()]);

            m_props = new List<GsaProp2d>();
            for (int i = 0; i < m_mesh.Faces.Count(); i++)
            {
                GsaProp2d property = new GsaProp2d();
                property.Prop2d = null;
                m_props.Add(property);
            }
        }

        public GsaElement2d(Brep brep, List<Curve> curves, List<Point3d> points, double meshSize, List<GsaMember1d> mem1ds, List<GsaNode> nodes, int prop = 0)
        {
            m_elements = new List<Element>();
            m_mesh = Util.GH.Convert.ConvertBrepToMesh(brep, curves, points, meshSize, mem1ds, nodes);
            Tuple<List<Element>, List<Point3d>, List<List<int>>> convertMesh = Util.GH.Convert.ConvertMeshToElem2d(m_mesh, prop, true);
            m_elements = convertMesh.Item1;
            m_topo = convertMesh.Item2;
            m_topoInt = convertMesh.Item3;
            
            m_id = new List<int>(new int[m_mesh.Faces.Count()]);

            m_props = new List<GsaProp2d>();
        }

        public GsaElement2d Duplicate()
        {
            if (this == null) { return null; }
            if (m_mesh == null) { return null; }

            GsaElement2d dup = new GsaElement2d();
            dup.m_mesh = (Mesh)m_mesh.Duplicate();
            dup.m_topo = m_topo.ToList();
            dup.m_topoInt = m_topoInt.ToList();

            dup.m_props = new List<GsaProp2d>();

            for (int i = 0; i < m_elements.Count; i++)
            {
                dup.m_elements.Add(new Element()
                {
                    Group = m_elements[i].Group,
                    IsDummy = m_elements[i].IsDummy,
                    Name = m_elements[i].Name.ToString(),
                    OrientationNode = m_elements[i].OrientationNode,
                    OrientationAngle = m_elements[i].OrientationAngle,
                    Offset = m_elements[i].Offset,
                    ParentMember = m_elements[i].ParentMember,
                    Property = m_elements[i].Property,
                    Topology = new ReadOnlyCollection<int>(m_elements[i].Topology.ToList()),
                    Type = m_elements[i].Type //GsaToModel.Element2dType((int)Elements[i].Type)
                });

                if ((System.Drawing.Color)m_elements[i].Colour != System.Drawing.Color.FromArgb(0, 0, 0)) // workaround to handle that System.Drawing.Color is non-nullable type
                    dup.m_elements[i].Colour = m_elements[i].Colour;

                dup.m_elements[i].Offset.X1 = m_elements[i].Offset.X1;
                dup.m_elements[i].Offset.X2 = m_elements[i].Offset.X2;
                dup.m_elements[i].Offset.Y = m_elements[i].Offset.Y;
                dup.m_elements[i].Offset.Z = m_elements[i].Offset.Z;

                if (m_props[i] != null)
                    dup.m_props.Add(m_props[i].Duplicate());
                else
                    dup.m_props.Add(null); //dup.m_props.Add(new GsaProp2d());
            }

            dup.Colours = new List<System.Drawing.Color>(Colours);

            if (m_id != null)
            {
                int[] dupids = new int[m_id.Count];
                m_id.CopyTo(dupids);
                dup.ID = new List<int>(dupids);
            }
            

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
            return "GSA 2D Element(s)";
        }

        #endregion
    }

    /// <summary>
    /// GsaMember Goo wrapper class, makes sure GsaMember can be used in Grasshopper.
    /// </summary>
    public class GsaElement2dGoo : GH_GeometricGoo<GsaElement2d>, IGH_PreviewData
    {
        #region constructors
        public GsaElement2dGoo()
        {
            this.Value = new GsaElement2d();
        }
        public GsaElement2dGoo(GsaElement2d element)
        {
            if (element == null)
                element = new GsaElement2d();
            this.Value = element; //element.Duplicate();
        }

        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return DuplicateGsaElement2d();
        }
        public GsaElement2dGoo DuplicateGsaElement2d()
        {
            return new GsaElement2dGoo(Value == null ? new GsaElement2d() : Value); //Value.Duplicate());
        }
        #endregion

        #region properties
        public override bool IsValid
        {
            get
            {
                if (Value == null) { return false; }
                if (Value.Mesh == null) { return false; }
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
                return "Null Element2D";
            else
                return Value.ToString();
        }
        public override string TypeName
        {
            get { return ("Element 2D"); }
        }
        public override string TypeDescription
        {
            get { return ("GSA 2D Element"); }
        }

        public override BoundingBox Boundingbox
        {
            get
            {
                if (Value == null) { return BoundingBox.Empty; }
                if (Value.Mesh == null) { return BoundingBox.Empty; }
                return Value.Mesh.GetBoundingBox(false) ;
            }
        }
        public override BoundingBox GetBoundingBox(Transform xform)
        {
            if (Value == null) { return BoundingBox.Empty; }
            if (Value.Mesh == null) { return BoundingBox.Empty; }
            return Value.Mesh.GetBoundingBox(xform);
        }
        #endregion

        #region casting methods
        public override bool CastTo<Q>(out Q target)
        {
            // This function is called when Grasshopper needs to convert this 
            // instance of GsaElement2D into some other type Q.            


            if (typeof(Q).IsAssignableFrom(typeof(GsaElement2d)))
            {
                if (Value == null)
                    target = default;
                else
                    target = (Q)(object)Value.Duplicate();
                return true;
            }
            
            if (typeof(Q).IsAssignableFrom(typeof(Element)))
            {
                if (Value == null)
                    target = default;
                else
                    target = (Q)(object)Value.Elements[0];
                return true;
            }

            //Cast to Mesh
            if (typeof(Q).IsAssignableFrom(typeof(Mesh)))
            {
                if (Value == null)
                    target = default;
                else
                    target = (Q)(object)Value.Mesh;
                return true;
            }

            if (typeof(Q).IsAssignableFrom(typeof(GH_Mesh)))
            {
                if (Value == null)
                    target = default;
                else
                {
                    target = (Q)(object)new GH_Mesh(Value.Mesh);
                }
                    
                return true;
            }

            if (typeof(Q).IsAssignableFrom(typeof(List<GH_Integer>)))
            {
                if (Value == null)
                    target = default;
                else
                {
                    List<GH_Integer> ints = new List<GH_Integer>();
                    
                    for (int i = 0; i < Value.ID.Count; i++)
                    {
                        GH_Integer ghint = new GH_Integer();
                        if (GH_Convert.ToGHInteger(Value.ID, GH_Conversion.Both, ref ghint))
                            ints.Add(ghint);
                    }
                    target = (Q)(object)ints;
                }
                return true;
            }


            target = default;
            return false;
        }
        public override bool CastFrom(object source)
        {
            // This function is called when Grasshopper needs to convert other data 
            // into GsaElement.


            if (source == null) { return false; }

            //Cast from GsaElement
            if (typeof(GsaElement2d).IsAssignableFrom(source.GetType()))
            {
                Value = (GsaElement2d)source;
                return true;
            }

            //Cast from GsaAPI Member
            if (typeof(List<Element>).IsAssignableFrom(source.GetType()))
            {
                Value.Elements = (List<Element>)source;
                return true;
            }

            if (typeof(Element).IsAssignableFrom(source.GetType()))
            {
                Value.Elements[0] = (Element)source; //If someone should want to just test if they can convert a Mesh face
                return true;
            }

            //Cast from Mesh
            Mesh mesh = new Mesh();

            if (GH_Convert.ToMesh(source, ref mesh, GH_Conversion.Both))
            {
                GsaElement2d elem = new GsaElement2d(mesh);
                this.Value = elem;
                return true;
            }
            
            return false;
        }
        #endregion

        #region transformation methods
        public override IGH_GeometricGoo Transform(Transform xform)
        {
            if (Value == null) { return null; }
            if (Value.Mesh == null) { return null; }

            GsaElement2d elem = Value.Duplicate();
            
            Mesh xMs = elem.Mesh;
            xMs.Transform(xform);
            elem.Mesh = xMs;
            Point3dList pts = new Point3dList(Value.Topology);
            pts.Transform(xform);
            elem.Topology = pts.ToList();

            return new GsaElement2dGoo(elem);
        }

        public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
        {
            if (Value == null) { return null; }
            if (Value.Mesh == null) { return null; }

            GsaElement2d elem = Value.Duplicate();
            Mesh xMs = elem.Mesh;
            xmorph.Morph(xMs);
            elem.Mesh = xMs;
            elem.TopoInt = Value.TopoInt;
            elem.Topology = Value.Topology;

            return new GsaElement2dGoo(elem);
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
            if (args.Material.Diffuse == System.Drawing.Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
            {
                args.Pipeline.DrawMeshShaded(Value.Mesh, UI.Colour.Element2dFace);
                //Mesh mesh = Value.Mesh.DuplicateMesh();
                //mesh.fac
                //for (int i = 0; i < Value.Mesh.Faces.Count; i++)
                //{
                //    int[] face = new int[] { i };
                //    args.Pipeline.DrawMeshShaded(Value.Mesh,
                //        UI.Colour.FaceCustom(Value.Colours[i]), face);
                //}
            }
            else
                args.Pipeline.DrawMeshShaded(Value.Mesh, UI.Colour.Element2dFaceSelected);
        }
        public void DrawViewportWires(GH_PreviewWireArgs args)
        {
            if (Value == null) { return; }
            if (Grasshopper.CentralSettings.PreviewMeshEdges == false) { return; }

            //Draw lines
            if (Value.Mesh != null)
            {
                if (args.Color == System.Drawing.Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
                {
                    args.Pipeline.DrawMeshWires(Value.Mesh, UI.Colour.Element2dEdge, 1);
                    
                    //List<Line> lines = new List<Line>();
                    //for (int i = 0; i < Value.Mesh.TopologyEdges.Count; i++)
                    //{
                    //    if(!Value.Mesh.TopologyEdges.IsNgonInterior(i))
                    //        lines.Add(Value.Mesh.TopologyEdges.EdgeLine(i));
                    //}
                    //args.Pipeline.DrawLines(lines, UI.Colour.Element2dEdge, 1);
                }
                else
                {
                    args.Pipeline.DrawMeshWires(Value.Mesh, UI.Colour.Element2dEdgeSelected, 2);

                    //List<Line> lines = new List<Line>();
                    //for (int i = 0; i < Value.Mesh.TopologyEdges.Count; i++)
                    //{
                    //    if (!Value.Mesh.TopologyEdges.IsNgonInterior(i))
                    //        lines.Add(Value.Mesh.TopologyEdges.EdgeLine(i));
                    //}
                    //args.Pipeline.DrawLines(lines, UI.Colour.Element2dEdgeSelected, 2);
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// This class provides a Parameter interface for the Data_GsaElement2d type.
    /// </summary>
    public class GsaElement2dParameter : GH_PersistentGeometryParam<GsaElement2dGoo>, IGH_PreviewObject
    {
        public GsaElement2dParameter()
          : base(new GH_InstanceDescription("2D Element", "E2D", "Maintains a collection of GSA 2D Element data.", GhSA.Components.Ribbon.CategoryName.Name(), GhSA.Components.Ribbon.SubCategoryName.Cat9()))
        {
        }

        public override Guid ComponentGuid => new Guid("bfaa6912-77b0-40b1-aa78-54e2b28614d0");

        public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.obscure;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.GsaElement2D;

        //We do not allow users to pick parameter, 
        //therefore the following 4 methods disable all this ui.
        protected override GH_GetterResult Prompt_Plural(ref List<GsaElement2dGoo> values)
        {
            return GH_GetterResult.cancel;
        }
        protected override GH_GetterResult Prompt_Singular(ref GsaElement2dGoo value)
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
