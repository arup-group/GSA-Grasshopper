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
    /// Member2d class, this class defines the basic properties and methods for any Gsa Member 2d
    /// </summary>
    public class GsaMember2d

    {
        public Member Member
        {
            get { return m_member; }
            set { m_member = value; }
        }
        public PolyCurve PolyCurve
        {
            get { return m_crv; }
            set { m_crv = Util.GH.Convert.ConvertCurveMem2d(value); }
        }
        public int ID
        {
            get { return m_id; }
            set { m_id = value; }
        }
        public Brep Brep
        {
            get { return m_brep; }
            set { m_brep = Util.GH.Convert.ConvertBrepToArcLineEdges(value); }
        }

        public List<Point3d> Topology
        {
            get { return m_topo; }
            set { m_topo = value; }
        }

        public List<string> TopologyType
        {
            get { return m_topoType; }
            set { m_topoType = value; }
        }

        public List<List<Point3d>> VoidTopology
        {
            get { return void_topo; }
            set { void_topo = value; }
        }

        public List<List<string>> VoidTopologyType
        {
            get { return void_topoType; }
            set { void_topoType = value; }
        }

        public List<PolyCurve> InclusionLines
        {
            get { return incl_Lines; }
            set { incl_Lines = value; }
        }

        public List<List<Point3d>> IncLinesTopology
        {
            get { return incLines_topo; }
            set { incLines_topo = value; }
        }

        public List<List<string>> IncLinesTopologyType
        {
            get { return inclLines_topoType; }
            set { inclLines_topoType = value; }
        }

        public List<Point3d> InclusionPoints
        {
            get { return incl_pts; }
            set { incl_pts = value; }
        }
        public GsaProp2d Property
        {
            get { return m_prop; }
            set { m_prop = value; }
        }
        public System.Drawing.Color Colour
        {
            get
            {
                return (System.Drawing.Color)m_member.Colour;
            }
            set { m_member.Colour = value; }
        }

        #region fields
        private Member m_member;
        private int m_id = 0;
        private Brep m_brep; //brep for visualisation /member2d
        private List<PolyCurve> void_crvs; //converted edgecurve /member2d
        private List<List<Point3d>> void_topo; //list of lists of void points /member2d
        private List<List<string>> void_topoType; ////list of polyline curve type (arch or line) for void /member2d
        private List<PolyCurve> incl_Lines; //converted inclusion lines /member2d
        private List<List<Point3d>> incLines_topo; //list of lists of line inclusion topology points /member2d
        private List<List<string>> inclLines_topoType; ////list of polyline curve type (arch or line) for inclusion /member2d
        private List<Point3d> incl_pts; //list of points for inclusion /member2d

        private PolyCurve m_crv; //Polyline for visualisation /member1d/member2d
        private List<Point3d> m_topo; // list of topology points for visualisation /member1d/member2d
        private List<string> m_topoType; //list of polyline curve type (arch or line) for member1d/2d
        private GsaProp2d m_prop;
        #endregion

        #region constructors
        public GsaMember2d()
        {
            m_member = new Member();
            //m_prop = new GsaProp2d();
        }

        public GsaMember2d(Brep brep, List<Curve> includeCurves = null, List<Point3d> includePoints = null, int prop = 0)
        {
            m_member = new Member
            {
                Type = MemberType.GENERIC_2D,
                Property = prop
            };
            //m_prop = new GsaProp2d();

            Tuple<Tuple<PolyCurve, List<Point3d>, List<string>>, Tuple<List<PolyCurve>, List<List<Point3d>>, List<List<string>>>, Tuple<List<PolyCurve>, List<List<Point3d>>, List<List<string>>, List<Point3d>>>
                convertBrepInclusion = Util.GH.Convert.ConvertPolyBrepInclusion(brep, includeCurves, includePoints);

            Tuple<PolyCurve, List<Point3d>, List<string>> edgeTuple = convertBrepInclusion.Item1;
            Tuple<List<PolyCurve>, List<List<Point3d>>, List<List<string>>> voidTuple = convertBrepInclusion.Item2;
            Tuple<List<PolyCurve>, List<List<Point3d>>, List<List<string>>, List<Point3d>> inclTuple = convertBrepInclusion.Item3;

            m_crv = edgeTuple.Item1;
            m_topo = edgeTuple.Item2;
            m_topoType = edgeTuple.Item3;
            void_crvs = voidTuple.Item1;
            void_topo = voidTuple.Item2;
            void_topoType = voidTuple.Item3;
            incl_Lines = inclTuple.Item1;
            incLines_topo = inclTuple.Item2;
            inclLines_topoType = inclTuple.Item3;
            incl_pts = inclTuple.Item4;

            m_brep = Util.GH.Convert.BuildBrep(m_crv, void_crvs);
        }

        public GsaMember2d(List<Point3d> topology, 
            List<string> topology_type, 
            List<List<Point3d>> void_topology = null,
            List<List<string>> void_topology_type = null,
            List<List<Point3d>> inlcusion_lines_topology = null,
            List<List<string>> inclusion_topology_type = null,
            List<Point3d> includePoints = null,
            int prop = 1)
        {
            m_member = new Member
            {
                Type = MemberType.GENERIC_2D,
                Property = prop
            };

            if (topology.Count == 0)
            {
                m_brep = null;
                m_crv = null;
                incl_pts = null;
                return;
            }

            if (topology[0] != topology[topology.Count - 1])
            {
                topology.Add(topology[0]);
                topology_type.Add("");
            }
                
            m_crv = Util.GH.Convert.BuildArcLineCurveFromPtsAndTopoType(topology, topology_type);
            m_topo = topology;
            m_topoType = topology_type;

            if (void_topology != null)
            {
                if (void_crvs == null) { void_crvs = new List<PolyCurve>(); }
                for (int i = 0; i < void_topology.Count; i++)
                {
                    // void curves must be closed, check that topogylist is ending with start point
                    if (void_topology[i][0] != void_topology[i][void_topology[i].Count - 1])
                    {
                        void_topology[i].Add(void_topology[i][0]);
                        void_topology_type[i].Add("");
                    }
                    if (void_topology_type != null)
                        void_crvs.Add(Util.GH.Convert.BuildArcLineCurveFromPtsAndTopoType(void_topology[i], void_topology_type[i]));
                    else
                        void_crvs.Add(Util.GH.Convert.BuildArcLineCurveFromPtsAndTopoType(void_topology[i]));
                }
            }
            void_topo = void_topology;
            void_topoType = void_topology_type;

            if (inlcusion_lines_topology != null)
            {
                if (incl_Lines == null) { incl_Lines = new List<PolyCurve>(); }
                for (int i = 0; i < inlcusion_lines_topology.Count; i++)
                {
                    if (inclusion_topology_type != null)
                        incl_Lines.Add(Util.GH.Convert.BuildArcLineCurveFromPtsAndTopoType(inlcusion_lines_topology[i], inclusion_topology_type[i]));
                    else
                        incl_Lines.Add(Util.GH.Convert.BuildArcLineCurveFromPtsAndTopoType(inlcusion_lines_topology[i]));
                }
            }
            incLines_topo = inlcusion_lines_topology;
            inclLines_topoType = inclusion_topology_type;

            incl_pts = includePoints;

            m_brep = Util.GH.Convert.BuildBrep(m_crv, void_crvs);
        }
        
        public GsaMember2d Duplicate()
        {
            if (this == null) { return null; }
            GsaMember2d dup = new GsaMember2d()
            {
                Member = new Member()
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
                    Type = m_member.Type, // GsaToModel.Member2dType((int)Member.Type),
                    Type2D = m_member.Type2D //GsaToModel.AnalysisOrder((int) Member.Type2D)
                }
            };

            if ((System.Drawing.Color)m_member.Colour != System.Drawing.Color.FromArgb(0, 0, 0)) // workaround to handle that System.Drawing.Color is non-nullable type
                dup.m_member.Colour = m_member.Colour;

            dup.Member.Offset.X1 = m_member.Offset.X1;
            dup.Member.Offset.X2 = m_member.Offset.X2;
            dup.Member.Offset.Y = m_member.Offset.Y;
            dup.Member.Offset.Z = m_member.Offset.Z;

            if (m_brep == null) { return dup; }

            if (m_brep != null)
                dup.m_brep = Brep.DuplicateBrep();
            if (m_crv != null)
                dup.m_crv = PolyCurve.DuplicatePolyCurve();

            Point3dList point3Ds = new Point3dList(Topology);
            dup.Topology = new List<Point3d>(point3Ds.Duplicate());
            dup.TopologyType = TopologyType.ToList();

            if (void_crvs != null)
            {
                dup.void_crvs = new List<PolyCurve>();
                dup.void_topo = new List<List<Point3d>>();
                dup.void_topoType = new List<List<string>>();
                for (int i = 0; i < void_crvs.Count; i++)
                {
                    dup.void_crvs.Add(void_crvs[i].DuplicatePolyCurve());
                    Point3dList voidpoint3Ds = new Point3dList(void_topo[i]);
                    dup.void_topo.Add(new List<Point3d>(voidpoint3Ds.Duplicate()));
                    dup.void_topoType.Add(void_topoType[i].ToList());
                }
            }

            if (incl_Lines != null)
            {
                dup.incl_Lines = new List<PolyCurve>();
                dup.incLines_topo = new List<List<Point3d>>();
                dup.inclLines_topoType = new List<List<string>>();
                for (int i = 0; i < incl_Lines.Count; i++)
                {
                    dup.incl_Lines.Add(incl_Lines[i].DuplicatePolyCurve());
                    Point3dList inclLinepoint3Ds = new Point3dList(incLines_topo[i]);
                    dup.incLines_topo.Add(new List<Point3d>(inclLinepoint3Ds.Duplicate()));
                    dup.inclLines_topoType.Add(inclLines_topoType[i].ToList());
                }
            }
            if (m_prop != null)
                dup.Property = Property.Duplicate();

            Point3dList inclpoint3Ds = new Point3dList(incl_pts);
            dup.incl_pts = new List<Point3d>(inclpoint3Ds.Duplicate());

            dup.ID = ID;

            return dup;
        }


        #endregion

        #region properties
        public bool IsValid
        {
            get
            {
                if (m_brep == null | m_crv == null)
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
            typeTxt = typeTxt.Replace("2d", "2D");
            string incl = "";
            if (!(incl_Lines == null & incl_pts == null))
                if (incl_Lines.Count > 0 | incl_pts.Count > 0)
                    incl = System.Environment.NewLine + "Contains ";
            if (incl_Lines != null)
            {
                if (incl_Lines.Count > 0)
                    incl = incl + incl_Lines.Count + " inclusion line";
                if (incl_Lines.Count > 1)
                    incl += "s";
            }
            
            if (incl_Lines != null & incl_pts != null)
                if (incl_Lines.Count > 0 & incl_pts.Count > 0)
                    incl += " and ";
            
            if (incl_pts != null)
            {
                if (incl_pts.Count > 0)
                    incl = incl + incl_pts.Count + " inclusion point";
                if (incl_pts.Count > 1)
                    incl += "s";
            }

            return typeTxt + incl;
        }

        #endregion
    }

    /// <summary>
    /// GsaMember Goo wrapper class, makes sure GsaMember can be used in Grasshopper.
    /// </summary>
    public class GsaMember2dGoo : GH_GeometricGoo<GsaMember2d>, IGH_PreviewData
    {
        #region constructors
        public GsaMember2dGoo()
        {
            this.Value = new GsaMember2d();
        }
        public GsaMember2dGoo(GsaMember2d member)
        {
            if (member == null)
                member = new GsaMember2d();
            this.Value = member; //member.Duplicate();
        }

        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return DuplicateGsaMember2d();
        }
        public GsaMember2dGoo DuplicateGsaMember2d()
        {
            return new GsaMember2dGoo(Value == null ? new GsaMember2d() : Value); //Value.Duplicate());
        }
        #endregion

        #region properties
        public override bool IsValid
        {
            get
            {
                if (Value == null) { return false; }
                if (Value.Brep == null & Value.PolyCurve == null) { return false; }
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
                return "Null Member2D";
            else
                return Value.ToString();
        }
        public override string TypeName
        {
            get { return ("Member 2D"); }
        }
        public override string TypeDescription
        {
            get { return ("GSA 2D Member"); }
        }

        public override BoundingBox Boundingbox
        {
            get
            {
                if (Value == null) { return BoundingBox.Empty; }
                if (Value.Brep == null & Value.PolyCurve == null) { return BoundingBox.Empty; }
                if (Value.Brep != null) { return Value.Brep.GetBoundingBox(false); }
                return Value.PolyCurve.GetBoundingBox(false) ;
            }
        }
        public override BoundingBox GetBoundingBox(Transform xform)
        {
            if (Value == null) { return BoundingBox.Empty; }
            if (Value.Brep == null & Value.PolyCurve == null) { return BoundingBox.Empty; }
            if (Value.Brep != null) { return Value.Brep.GetBoundingBox(xform); }
            return Value.PolyCurve.GetBoundingBox(xform);
        }
        #endregion

        #region casting methods
        public override bool CastTo<Q>(out Q target)
        {
            // This function is called when Grasshopper needs to convert this 
            // instance of GsaMember into some other type Q.            


            if (typeof(Q).IsAssignableFrom(typeof(GsaMember2d)))
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
                    target = (Q)(object)Value.Member;
                return true;
            }

            //Cast to Curve
            if (typeof(Q).IsAssignableFrom(typeof(Curve)))
            {
                if (Value == null)
                    target = default;
                else
                    target = (Q)(object)Value.PolyCurve;
                return true;
            }
            if (typeof(Q).IsAssignableFrom(typeof(GH_Curve)))
            {
                if (Value == null)
                    target = default;
                else
                {
                    target = (Q)(object)new GH_Curve(Value.PolyCurve);
                    if (Value.PolyCurve == null)
                        return false;
                }
                   
                return true;
            }

            if (typeof(Q).IsAssignableFrom(typeof(PolyCurve)))
            {
                if (Value == null)
                    target = default;
                else
                {
                    target = (Q)(object)Value.PolyCurve;
                    if (Value.PolyCurve == null)
                        return false;
                }
                    
                return true;
            }

            if (typeof(Q).IsAssignableFrom(typeof(Polyline)))
            {
                if (Value == null)
                    target = default;
                else
                {
                    target = (Q)(object)Value.PolyCurve;
                    if (Value.PolyCurve == null)
                        return false;
                }
                    
                return true;
            }
            if (typeof(Q).IsAssignableFrom(typeof(Line)))
            {
                if (Value == null)
                    target = default;
                else
                {
                    target = (Q)(object)Value.PolyCurve.ToPolyline(0.05, 5, 0, 0);
                    if (Value.PolyCurve == null)
                        return false;
                }
                    
                return true;
            }

            //Cast to Brep
            if (typeof(Q).IsAssignableFrom(typeof(Brep)))
            {
                if (Value == null)
                    target = default;
                else
                {
                    target = (Q)(object)Value.Brep;
                    if (Value.Brep == null)
                        return false;
                }
                    
                return true;
            }
            if (typeof(Q).IsAssignableFrom(typeof(GH_Brep)))
            {
                if (Value == null)
                    target = default;
                else
                {
                    target = (Q)(object)new GH_Brep(Value.Brep);
                    if (Value.Brep == null)
                        return false;
                }
                return true;
            }

            //Cast to Points
            if (typeof(Q).IsAssignableFrom(typeof(List<Point3d>)))
            {
                if (Value == null)
                    target = default;
                else
                    target = (Q)(object)Value.Topology;
                return true;
            }
            if (typeof(Q).IsAssignableFrom(typeof(List<GH_Point>)))
            {
                if (Value == null)
                    target = default;
                else
                    target = (Q)(object)Value.Topology;
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
            if (typeof(GsaMember2d).IsAssignableFrom(source.GetType()))
            {
                Value = (GsaMember2d)source;
                return true;
            }

            //Cast from GsaAPI Member
            if (typeof(Member).IsAssignableFrom(source.GetType()))
            {
                Value.Member = (Member)source;
                return true;
            }

            //Cast from Brep
            Brep brep = new Brep();
            if (GH_Convert.ToBrep(source, ref brep, GH_Conversion.Both))
            {
                List<Point3d> pts = new List<Point3d>();
                List<Curve> crvs = new List<Curve>();
                GsaMember2d mem = new GsaMember2d(brep, crvs, pts);
                //GsaProp2d prop2d = new GsaProp2d();
                //prop2d.ID = 1;
                //mem.Property = prop2d;
                this.Value = mem;
                return true;
            }

            return false;
        }
        #endregion

        #region transformation methods
        public override IGH_GeometricGoo Transform(Transform xform)
        {
            if (Value == null) { return null; }
            if (Value.Brep == null & Value.PolyCurve == null) { return null; }

            GsaMember2d mem = Value.Duplicate();

            List<Point3d> pts = mem.Topology.ToList();
            Point3dList xpts = new Point3dList(pts);
            xpts.Transform(xform);
            mem.Topology = xpts.ToList();
            mem.TopologyType = Value.TopologyType.ToList();
            
            if (Value.VoidTopology != null)
            {
                mem.VoidTopology = new List<List<Point3d>>();
                for (int i = 0; i < Value.VoidTopology.Count; i++)
                {
                    xpts = new Point3dList(Value.VoidTopology[i]);
                    xpts.Transform(xform);
                    mem.VoidTopology.Add(xpts.ToList());
                }
                mem.VoidTopologyType = Value.VoidTopologyType;
            }

            if (Value.InclusionLines != null)
            {
                mem.InclusionLines = new List<PolyCurve>();
                for (int i = 0; i < Value.InclusionLines.Count; i++)
                {
                    PolyCurve xLn = Value.InclusionLines[i];
                    xLn.Transform(xform);
                    mem.InclusionLines.Add(xLn);
                }
                mem.IncLinesTopology = new List<List<Point3d>>();
                for (int i = 0; i < Value.IncLinesTopology.Count; i++)
                {
                    xpts = new Point3dList(Value.IncLinesTopology[i]);
                    xpts.Transform(xform);
                    mem.IncLinesTopology.Add(xpts.ToList());
                }
                mem.IncLinesTopologyType = Value.IncLinesTopologyType;
            }
            if (Value.InclusionPoints != null)
            {
                xpts = new Point3dList(Value.InclusionPoints);
                xpts.Transform(xform);
                mem.InclusionPoints = xpts.ToList();
            }

            if (Value.Brep != null)
            {
                Brep brep = Value.Brep.DuplicateBrep();
                brep.Transform(xform);
                mem.Brep = brep;
            }
            if (Value.PolyCurve != null)
            {
                PolyCurve crv = Value.PolyCurve.DuplicatePolyCurve();
                crv.Transform(xform);
                mem.PolyCurve = crv;
            }
            
            return new GsaMember2dGoo(mem);
        }

        public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
        {
            if (Value == null) { return null; }
            if (Value.Brep == null & Value.PolyCurve == null) { return null; }

            GsaMember2d mem = new GsaMember2d
            {
                Member = Value.Member
            };

            List<Point3d> pts = Value.Topology;
            for (int i = 0; i < pts.Count; i++)
                pts[i] = xmorph.MorphPoint(pts[i]);
            mem.Topology = pts;

            if (Value.VoidTopology != null)
            {
                mem.VoidTopology = new List<List<Point3d>>();
                for (int i = 0; i < Value.VoidTopology.Count; i++)
                {
                    pts = Value.VoidTopology[i];
                    for (int j = 0; j < pts.Count; j++)
                        pts[j] = xmorph.MorphPoint(pts[j]);
                    mem.VoidTopology.Add(pts);
                }
                mem.VoidTopologyType = Value.VoidTopologyType;
            }

            if (Value.InclusionLines != null)
            {
                mem.InclusionLines = new List<PolyCurve>();
                for (int i = 0; i < Value.InclusionLines.Count; i++)
                {
                    PolyCurve xLn = Value.InclusionLines[i];
                    xmorph.Morph(xLn);
                    mem.InclusionLines.Add(xLn);
                }
                mem.IncLinesTopology = new List<List<Point3d>>();
                for (int i = 0; i < Value.IncLinesTopology.Count; i++)
                {
                    pts = Value.IncLinesTopology[i];
                    for (int j = 0; j < pts.Count; j++)
                        pts[j] = xmorph.MorphPoint(pts[j]);
                    mem.IncLinesTopology.Add(pts);
                }
                mem.IncLinesTopologyType = Value.IncLinesTopologyType;
            }
            if (Value.InclusionPoints != null)
            {
                pts = Value.InclusionPoints;
                for (int i = 0; i < pts.Count; i++)
                    pts[i] = xmorph.MorphPoint(pts[i]);
                mem.InclusionPoints = pts;
            }


            if (Value.Brep != null)
            {
                Brep brep = Value.Brep.DuplicateBrep();
                xmorph.Morph(brep);
                mem.Brep = brep;
            }
            if (Value.PolyCurve != null)
            {
                PolyCurve crv = Value.PolyCurve.DuplicatePolyCurve();
                xmorph.Morph(crv);
                mem.PolyCurve = crv;
            }

            return new GsaMember2dGoo(mem);
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
            if (Value.Brep != null)
            {
                if (Value.Member.Type == MemberType.VOID_CUTTER_2D)
                {
                    
                }
                else
                {
                    if (args.Material.Diffuse == System.Drawing.Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
                        args.Pipeline.DrawBrepShaded(Value.Brep, UI.Colour.Member2dFace); //UI.Colour.Member2dFace
                    else
                        args.Pipeline.DrawBrepShaded(Value.Brep, UI.Colour.Member2dFaceSelected);
                }
            }
        }
        public void DrawViewportWires(GH_PreviewWireArgs args)
        {
            if (Value == null) { return; }

            // Draw shape
            if (Value.Brep != null)
            {
                if (args.Color == System.Drawing.Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
                {
                    if (!Value.Member.IsDummy)
                        args.Pipeline.DrawBrepWires(Value.Brep, (System.Drawing.Color)Value.Member.Colour, -1);
                }
                else
                    args.Pipeline.DrawBrepWires(Value.Brep, UI.Colour.Member2dEdgeSelected, -1);
            }

            //Draw lines
            if (Value.PolyCurve != null & Value.Brep == null)
            {
                if (args.Color == System.Drawing.Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
                {
                    if (Value.Member.IsDummy)
                        args.Pipeline.DrawDottedPolyline(Value.Topology, UI.Colour.Dummy1D, false);
                    else
                    {
                        if ((System.Drawing.Color)Value.Colour != System.Drawing.Color.FromArgb(0, 0, 0))
                            args.Pipeline.DrawCurve(Value.PolyCurve, Value.Colour, 2);
                        else
                        {
                            System.Drawing.Color col = UI.Colour.Member2dEdge;
                            args.Pipeline.DrawCurve(Value.PolyCurve, col, 2);
                        }
                    }
                }
                else
                {
                    if (Value.Member.IsDummy)
                        args.Pipeline.DrawDottedPolyline(Value.Topology, UI.Colour.Member1dSelected, false);
                    else
                        args.Pipeline.DrawCurve(Value.PolyCurve, UI.Colour.Member1dSelected, 2);
                }
            }

            if (Value.InclusionLines != null)
            {
                for (int i = 0; i < Value.InclusionLines.Count; i++)
                {
                    if (Value.Member.IsDummy)
                        args.Pipeline.DrawDottedPolyline(Value.IncLinesTopology[i], UI.Colour.Member1dSelected, false);
                    else
                        args.Pipeline.DrawCurve(Value.InclusionLines[i], UI.Colour.Member2dInclLn, 2);
                }
            }

            //Draw points.
            if (Value.Topology != null)
            {
                List<Point3d> pts = Value.Topology;
                for (int i = 0; i < pts.Count; i++)
                {
                    if (args.Color == System.Drawing.Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
                    {
                        if (Value.Brep == null & (i == 0 | i == pts.Count - 1)) // draw first point bigger
                            args.Pipeline.DrawPoint(pts[i], Rhino.Display.PointStyle.RoundSimple, 3, (Value.Member.IsDummy) ? UI.Colour.Dummy1D : UI.Colour.Member1dNode);
                        else
                            args.Pipeline.DrawPoint(pts[i], Rhino.Display.PointStyle.RoundSimple, 2, (Value.Member.IsDummy)? UI.Colour.Dummy1D : UI.Colour.Member1dNode);
                    }
                    else
                    {
                        if (Value.Brep == null & (i == 0 | i == pts.Count - 1)) // draw first point bigger
                            args.Pipeline.DrawPoint(pts[i], Rhino.Display.PointStyle.RoundControlPoint, 3, UI.Colour.Member1dNodeSelected);
                        else
                            args.Pipeline.DrawPoint(pts[i], Rhino.Display.PointStyle.RoundControlPoint, 2, UI.Colour.Member1dNodeSelected);
                    }
                }
            }
            if (Value.InclusionPoints != null)
            {
                for (int i = 0; i < Value.InclusionPoints.Count; i++)
                    args.Pipeline.DrawPoint(Value.InclusionPoints[i], Rhino.Display.PointStyle.RoundSimple, 3, (Value.Member.IsDummy) ? UI.Colour.Dummy1D : UI.Colour.Member2dInclPt);
            }
        }
        #endregion
    }

    /// <summary>
    /// This class provides a Parameter interface for the Data_GsaMember2d type.
    /// </summary>
    public class GsaMember2dParameter : GH_PersistentGeometryParam<GsaMember2dGoo>, IGH_PreviewObject
    {
        public GsaMember2dParameter()
          : base(new GH_InstanceDescription("2D Member", "M2D", "Maintains a collection of GSA 2D Member data.", GhSA.Components.Ribbon.CategoryName.Name(), GhSA.Components.Ribbon.SubCategoryName.Cat9()))
        {
        }

        public override Guid ComponentGuid => new Guid("fa512c2d-4767-49f1-a574-32bf66a66568");

        public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.obscure;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.GsaMem2D;

        //We do not allow users to pick parameter, 
        //therefore the following 4 methods disable all this ui.
        protected override GH_GetterResult Prompt_Plural(ref List<GsaMember2dGoo> values)
        {
            return GH_GetterResult.cancel;
        }
        protected override GH_GetterResult Prompt_Singular(ref GsaMember2dGoo value)
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
