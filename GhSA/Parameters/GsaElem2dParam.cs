using System;
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
        }


        public GsaElement2d(Mesh mesh, int prop = 1)
        {
            m_elements = new List<Element>();
            m_mesh = mesh;
            Tuple<List<Element>, List<Point3d>, List<List<int>>> convertMesh = Util.GH.Convert.ConvertMesh(mesh, prop);
            m_elements = convertMesh.Item1;
            m_topo = convertMesh.Item2;
            m_topoInt = convertMesh.Item3;
            m_id = new List<int>(new int[m_mesh.Faces.Count()]);
        }

        public GsaElement2d Duplicate()
        {
            GsaElement2d dup = new GsaElement2d
            {
                m_elements = m_elements //add clone or duplicate if available
            };
            if (m_mesh != null)
            {
                dup.m_mesh = (Mesh)m_mesh.Duplicate();
                Point3dList point3Ds = new Point3dList(m_topo);
                dup.Topology = new List<Point3d>(point3Ds.Duplicate());
                dup.m_topoInt = m_topoInt.ToList();
            }
            if (m_id != null)
            {
                int[] dupids = new int[m_id.Count];
                m_id.CopyTo(dupids);
                dup.ID = new List<int>(dupids);
            }
            if (m_props != null)
            {
                GsaProp2d[] dupprop = new GsaProp2d[m_props.Count];
                m_props.CopyTo(dupprop);
                dup.Properties = new List<GsaProp2d>(dupprop);
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
            this.Value = element;
        }

        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return DuplicateGsaElement2d();
        }
        public GsaElement2dGoo DuplicateGsaElement2d()
        {
            return new GsaElement2dGoo(Value == null ? new GsaElement2d() : Value.Duplicate());
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
            // instance of GsaMember into some other type Q.            


            if (typeof(Q).IsAssignableFrom(typeof(GsaElement2d)))
            {
                if (Value == null)
                    target = default;
                else
                    target = (Q)(object)Value;
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
            
            Mesh xMs = Value.Mesh;
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

            GsaElement2d elem = new GsaElement2d
            {
                Elements = Value.Elements
            };
            Mesh xMs = Value.Mesh;
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
                args.Pipeline.DrawMeshShaded(Value.Mesh, UI.Colour.Member2dFace);
            else
                args.Pipeline.DrawMeshShaded(Value.Mesh, UI.Colour.Member2dFaceSelected);
        }
        public void DrawViewportWires(GH_PreviewWireArgs args)
        {
            if (Value == null) { return; }

            //Draw lines
            if (Value.Mesh != null)
            {
                if (args.Color == System.Drawing.Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
                {
                    for (int i = 0; i < Value.Mesh.TopologyEdges.Count; i++)
                        args.Pipeline.DrawLine(Value.Mesh.TopologyEdges.EdgeLine(i), UI.Colour.Element2dEdge, 2);
                }
                else
                {
                    for (int i = 0; i < Value.Mesh.TopologyEdges.Count; i++)
                        args.Pipeline.DrawLine(Value.Mesh.TopologyEdges.EdgeLine(i), UI.Colour.Element2dEdgeSelected, 3);
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
          : base(new GH_InstanceDescription("GSA 2D Element", "Element 2D", "Maintains a collection of GSA 2D Element data.", GhSA.Components.Ribbon.CategoryName.Name(), GhSA.Components.Ribbon.SubCategoryName.Cat9()))
        {
        }

        public override Guid ComponentGuid => new Guid("bfaa6912-77b0-40b1-aa78-54e2b28614d0");

        public override GH_Exposure Exposure => GH_Exposure.tertiary;

        protected override System.Drawing.Bitmap Icon => GSA.Properties.Resources.GsaElem2D;

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
