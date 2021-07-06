using System;
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
            set 
            {
                if (m_props == null)
                    PropertyIDs = new List<int> { 0 };
                m_props = value; 
            }
        }
        public List<int> ParentMembers
        {
            get 
            {
                List<int> pMems = new List<int>();
                for (int i = 0; i < m_elements.Count; i++)
                    try { pMems.Add(m_elements[i].ParentMember.Member); } catch (Exception) { pMems.Add(0); }
                return pMems; 
            }
        }
        #region GsaAPI members
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
                CloneElements(apiObjectMember.colour, null, null, null, null, null, null, null, value);
                //for (int i = 0; i < m_elements.Count; i++)
                //{
                //    if (value[i] != null)
                //    {
                //        m_elements[i].Colour = value[i];
                //        Mesh.VertexColors.SetColor(i, (System.Drawing.Color)m_elements[i].Colour);
                //    }
                //}
            }
        }
        public List<int> Groups
        {
            get 
            {
                List<int> groups = new List<int>();
                for (int i = 0; i < m_elements.Count; i++)
                {
                    if (m_elements[i] != null)
                        groups.Add(m_elements[i].Group);
                }
                return groups; 
            }
            set 
            {
                CloneElements(apiObjectMember.group, value);
            }
        }
        public List<bool> isDummies
        {
            get
            {
                List<bool> dums = new List<bool>();
                for (int i = 0; i < m_elements.Count; i++)
                {
                    if (m_elements[i] != null)
                        dums.Add(m_elements[i].IsDummy);
                }
                return dums;
            }
            set
            {
                CloneElements(apiObjectMember.dummy, null, value);
            }
        }
        public List<string> Names
        {
            get
            {
                List<string> names = new List<string>();
                for (int i = 0; i < m_elements.Count; i++)
                {
                    if (m_elements[i] != null)
                        names.Add(m_elements[i].Name);
                }
                return names;
            }
            set
            {
                CloneElements(apiObjectMember.dummy, null, null, value);
            }
        }
        public List<double> OrientationAngles
        {
            get
            {
                List<double> angles = new List<double>();
                for (int i = 0; i < m_elements.Count; i++)
                {
                    if (m_elements[i] != null)
                        angles.Add(m_elements[i].OrientationAngle);
                }
                return angles;
            }
            set
            {
                CloneElements(apiObjectMember.dummy, null, null, null, value);
            }
        }
        public List<GsaOffset> Offsets
        {
            get
            {
                List<GsaOffset> offs = new List<GsaOffset>();
                for (int i = 0; i < m_elements.Count; i++)
                {
                    if (m_elements[i] != null)
                    {
                        GsaOffset off = new GsaOffset(
                            m_elements[i].Offset.X1,
                            m_elements[i].Offset.X2,
                            m_elements[i].Offset.Y,
                            m_elements[i].Offset.Z);
                        offs.Add(off);
                    }
                }
                return offs;
            }
            set
            {
                CloneElements(apiObjectMember.dummy, null, null, null, null, value);
            }
        }
        public List<int> PropertyIDs
        {
            get
            {
                List<int> propids = new List<int>();
                for (int i = 0; i < m_elements.Count; i++)
                {
                    if (m_elements[i] != null)
                        propids.Add(m_elements[i].Property);
                }
                return propids;
            }
            set
            {
                CloneElements(apiObjectMember.dummy, null, null, null, null, null, value);
            }
        }
        public List<ElementType> Types
        {
            get
            {
                List<ElementType> typs = new List<ElementType>();
                for (int i = 0; i < m_elements.Count; i++)
                {
                    if (m_elements[i] != null)
                        typs.Add(m_elements[i].Type);
                }
                return typs;
            }
            set
            {
                CloneElements(apiObjectMember.dummy, null, null, null, null, null, null, value);
            }
        }

        private void CloneElements(apiObjectMember memType, List<int> grp = null, List<bool> dum = null, List<string> nm = null,  
            List<double> oriA = null, List<GsaOffset> off = null, List<int> prop = null, List<ElementType> typ = null, List<System.Drawing.Color> col = null)
        {
            List<Element> elems = new List<Element>();
            for (int i = 0; i < m_elements.Count; i++)
            {
                elems.Add(new Element()
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

                //if ((System.Drawing.Color)m_elements[i].Colour != System.Drawing.Color.FromArgb(0, 0, 0)) // workaround to handle that System.Drawing.Color is non-nullable type
                //    elems[i].Colour = m_elements[i].Colour;

                switch (memType)
                {
                    case apiObjectMember.group:
                        if (grp.Count > i)
                            elems[i].Group = grp[i];
                        else
                            elems[i].Group = grp.Last();
                        break;
                    case apiObjectMember.dummy:
                        if (dum.Count > i)
                            elems[i].IsDummy = dum[i];
                        else
                            elems[i].IsDummy = dum.Last();
                        break;
                    case apiObjectMember.name:
                        if (nm.Count > i)
                            elems[i].Name = nm[i];
                        else
                            elems[i].Name = nm.Last();
                        break;
                    case apiObjectMember.orientationAngle:
                        if (oriA.Count > i)
                            elems[i].OrientationAngle = oriA[i];
                        else
                            elems[i].OrientationAngle = oriA.Last();
                        break;
                    case apiObjectMember.offset:
                        if (off.Count > i)
                        {
                            elems[i].Offset.X1 = off[i].X1;
                            elems[i].Offset.X2 = off[i].X2;
                            elems[i].Offset.Y = off[i].Y;
                            elems[i].Offset.Z = off[i].Z;
                        }
                        else
                        {
                            elems[i].Offset.X1 = off.Last().X1;
                            elems[i].Offset.X2 = off.Last().X2;
                            elems[i].Offset.Y = off.Last().Y;
                            elems[i].Offset.Z = off.Last().Z;
                        }
                        break;
                    case apiObjectMember.property:
                        if (prop.Count > i)
                            elems[i].Property = prop[i];
                        else
                            elems[i].Property = prop.Last();
                        break;
                    case apiObjectMember.type:
                        if (typ.Count > i)
                            elems[i].Type = typ[i];
                        else
                            elems[i].Type = typ.Last();
                        break;
                    case apiObjectMember.colour:
                        if (col.Count > i)
                            elems[i].Colour = col[i];
                        else
                            elems[i].Colour = col.Last();
                        
                        m_mesh.VertexColors.SetColor(i, (System.Drawing.Color)elems[i].Colour);
                        break;
                }
            }
            m_elements = elems;
        }
        private enum apiObjectMember
        {
            group,
            dummy,
            name,
            orientationAngle,
            offset,
            property,
            type,
            colour
        }
        #endregion

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
            dup.m_elements = m_elements;
            dup.m_id = m_id;
            dup.m_mesh = (Mesh)m_mesh.DuplicateShallow();
            dup.m_props = m_props;
            dup.m_topo = m_topo;
            dup.m_topoInt = m_topoInt;
            return dup;
        }
        public GsaElement2d Clone()
        {
            if (this == null) { return null; }
            if (m_mesh == null) { return null; }

            GsaElement2d dup = new GsaElement2d();
            dup.m_mesh = (Mesh)m_mesh.Duplicate();
            dup.m_topo = m_topo.ToList();
            dup.m_topoInt = m_topoInt.ToList();

            dup.m_props = new List<GsaProp2d>();
            dup.m_props = m_props;

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

                //if (m_props[i] != null)
                //    dup.m_props.Add(m_props[i].Duplicate());
                //else
                //    dup.m_props.Add(null); //dup.m_props.Add(new GsaProp2d());
                
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
