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
    /// Member1d class, this class defines the basic properties and methods for any Gsa Member 1d
    /// </summary>
    public class GsaMember1d
    {
        public PolyCurve PolyCurve
        {
            get { return m_crv; }
            set 
            {
                Tuple<PolyCurve, List<Point3d>, List<string>> convertCrv = Util.GH.Convert.ConvertMem1dCrv(value);
                m_crv = convertCrv.Item1;
                m_topo = convertCrv.Item2;
                m_topoType = convertCrv.Item3;
                UpdatePreview();
            }
        }
        public int ID
        {
            get { return m_id; }
            set { m_id = value; }
        }

        public List<Point3d> Topology
        {
            get { return m_topo; }
        }

        public List<string> TopologyType
        {
            get { return m_topoType; }
        }

        public GsaBool6 ReleaseStart
        {
            get { return m_rel1; }
            set 
            { 
                m_rel1 = value;
                if (m_rel2 == null)
                    m_rel2 = new GsaBool6();
                UpdatePreview();
            }
        }
        public GsaBool6 ReleaseEnd
        {
            get { return m_rel2; }
            set 
            { 
                m_rel2 = value;
                if (m_rel1 == null)
                    m_rel1 = new GsaBool6();
                UpdatePreview();
            }
        }

        public GsaSection Section
        {
            get { return m_section; }
            set { m_section = value; }
        }
        #region GsaAPI members
        internal Member API_Member
        {
            get { return m_member; }
            set { m_member = value; }
        }
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
        public GsaOffset Offset
        {
            get
            {
                return new GsaOffset(
                    m_member.Offset.X1,
                    m_member.Offset.X2,
                    m_member.Offset.Y,
                    m_member.Offset.Z);
            }
            set
            {
                CloneMember();
                m_member.Offset.X1 = value.X1;
                m_member.Offset.X2 = value.X2;
                m_member.Offset.Y = value.Y;
                m_member.Offset.Z = value.Z;
            }
        }
        public double OrientationAngle
        {
            get { return m_member.OrientationAngle; }
            set
            {
                CloneMember();
                m_member.OrientationAngle = value;
            }
        }
        public GsaNode OrientationNode
        {
            get { return m_orientationNode; }
            set
            {
                CloneMember();
                m_orientationNode = value;
            }
        }
        public MemberType Type
        {
            get { return m_member.Type; }
            set
            {
                CloneMember();
                m_member.Type = value;
            }
        }
        public ElementType Type1D
        {
            get { return m_member.Type1D; }
            set
            {
                CloneMember();
                m_member.Type1D = value;
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
                Type1D = m_member.Type1D
            };

            if ((System.Drawing.Color)m_member.Colour != System.Drawing.Color.FromArgb(0, 0, 0)) // workaround to handle that System.Drawing.Color is non-nullable type
                mem.Colour = m_member.Colour;
            
            m_member = mem;
        }
        #endregion
        #region preview
        #region preview lines
        private Line previewSX1;
        private Line previewSX2;
        private Line previewSY1;
        private Line previewSY2;
        private Line previewSY3;
        private Line previewSY4;
        private Line previewSZ1;
        private Line previewSZ2;
        private Line previewSZ3;
        private Line previewSZ4;
        private Line previewEX1;
        private Line previewEX2;
        private Line previewEY1;
        private Line previewEY2;
        private Line previewEY3;
        private Line previewEY4;
        private Line previewEZ1;
        private Line previewEZ2;
        private Line previewEZ3;
        private Line previewEZ4;
        private Line previewSXX;
        private Line previewSYY1;
        private Line previewSYY2;
        private Line previewSZZ1;
        private Line previewSZZ2;
        private Line previewEXX;
        private Line previewEYY1;
        private Line previewEYY2;
        private Line previewEZZ1;
        private Line previewEZZ2;

        internal List<Line> previewGreenLines;
        internal List<Line> previewRedLines;
        #endregion
        private void UpdatePreview()
        {
            if (m_rel1 != null & m_rel2 != null)
            {
                if (m_rel1.X || m_rel1.Y || m_rel1.Z || m_rel1.XX || m_rel1.YY || m_rel1.ZZ ||
                m_rel2.X || m_rel2.Y || m_rel2.Z || m_rel2.XX || m_rel2.YY || m_rel2.ZZ)
                {
                    #region add lines
                    previewGreenLines = new List<Line>();
                    previewGreenLines.Add(previewSX1);
                    previewGreenLines.Add(previewSX2);
                    previewGreenLines.Add(previewSY1);
                    previewGreenLines.Add(previewSY2);
                    previewGreenLines.Add(previewSY3);
                    previewGreenLines.Add(previewSY4);
                    previewGreenLines.Add(previewSZ1);
                    previewGreenLines.Add(previewSZ2);
                    previewGreenLines.Add(previewSZ3);
                    previewGreenLines.Add(previewSZ4);
                    previewGreenLines.Add(previewEX1);
                    previewGreenLines.Add(previewEX2);
                    previewGreenLines.Add(previewEY1);
                    previewGreenLines.Add(previewEY2);
                    previewGreenLines.Add(previewEY3);
                    previewGreenLines.Add(previewEY4);
                    previewGreenLines.Add(previewEZ1);
                    previewGreenLines.Add(previewEZ2);
                    previewGreenLines.Add(previewEZ3);
                    previewGreenLines.Add(previewEZ4);
                    previewRedLines = new List<Line>();
                    previewRedLines.Add(previewSXX);
                    previewRedLines.Add(previewSYY1);
                    previewRedLines.Add(previewSYY2);
                    previewRedLines.Add(previewSZZ1);
                    previewRedLines.Add(previewSZZ2);
                    previewRedLines.Add(previewEXX);
                    previewRedLines.Add(previewEYY1);
                    previewRedLines.Add(previewEYY2);
                    previewRedLines.Add(previewEZZ1);
                    previewRedLines.Add(previewEZZ2);
                    #endregion
                    GhSA.UI.Display.Preview1D(m_crv, m_member.OrientationAngle, m_rel1, m_rel2,
                    ref previewGreenLines, ref previewRedLines);
                }
                else
                    previewGreenLines = null;
            }
        }
        #endregion
        #region fields
        private Member m_member;
        private int m_id = 0;

        private PolyCurve m_crv; //Polyline for visualisation /member1d/member2d
        private List<Point3d> m_topo; // list of topology points for visualisation /member1d/member2d
        private List<string> m_topoType; //list of polyline curve type (arch or line) for member1d/2d
        private GsaBool6 m_rel1;
        private GsaBool6 m_rel2;
        private GsaSection m_section;
        private GsaNode m_orientationNode;
        #endregion

        #region constructors
        public GsaMember1d()
        {
            m_member = new Member();
            m_crv = new PolyCurve();
            m_section = new GsaSection();
        }

        internal GsaMember1d(Member member, int id, List<Point3d> topology, List<string> topo_type, GsaSection section, GsaNode orientationNode)
        {
            m_member = member;
            m_id = id;
            m_crv = Util.GH.Convert.BuildArcLineCurveFromPtsAndTopoType(topology, topo_type);
            m_topo = topology;
            m_topoType = topo_type;
            m_section = section;
            if (orientationNode != null)
                m_orientationNode = orientationNode;
            UpdatePreview();
        }

        public GsaMember1d(Curve crv, int prop = 0)
        {
            m_member = new Member
            {
                Type = MemberType.GENERIC_1D,
                Property = prop
            };
            Tuple<PolyCurve, List<Point3d>, List<string>> convertCrv = Util.GH.Convert.ConvertMem1dCrv(crv);
            m_crv = convertCrv.Item1;
            m_topo = convertCrv.Item2;
            m_topoType = convertCrv.Item3;

            m_section = new GsaSection();
            UpdatePreview();
        }
        public GsaMember1d Duplicate()
        {
            if (this == null) { return null; }
            
            GsaMember1d dup = new GsaMember1d();
            dup.m_id = m_id;
            dup.m_member = m_member;
            dup.m_crv = (PolyCurve)m_crv.DuplicateShallow();
            if (m_rel1 != null)
                dup.m_rel1 = m_rel1.Duplicate();
            if (m_rel2 != null)
                dup.m_rel2 = m_rel2.Duplicate();
            dup.m_section = m_section.Duplicate();
            dup.m_topo = m_topo;
            dup.m_topoType = m_topoType;
            if (m_orientationNode != null)
                dup.m_orientationNode = m_orientationNode.Duplicate();
            dup.UpdatePreview();
            return dup;
        }
        public GsaMember1d Transform(Transform xform)
        {
            if (this == null) { return null; }

            GsaMember1d dup = this.Duplicate();

            List<Point3d> pts = m_topo.ToList();
            Point3dList xpts = new Point3dList(pts);
            xpts.Transform(xform);
            dup.m_topo = xpts.ToList();

            if (m_crv != null)
            {
                PolyCurve crv = m_crv.DuplicatePolyCurve();
                crv.Transform(xform);
                dup.m_crv = crv;
            }
            dup.UpdatePreview();
            return dup;
        }
        public GsaMember1d Morph(SpaceMorph xmorph)
        {
            if (this == null) { return null; }

            GsaMember1d dup = this.Duplicate();

            List<Point3d> pts = m_topo.ToList();
            for (int i = 0; i < pts.Count; i++)
                pts[i] = xmorph.MorphPoint(pts[i]);
            dup.m_topo = pts;

            if (m_crv != null)
            {
                PolyCurve crv = m_crv.DuplicatePolyCurve();
                xmorph.Morph(crv);
                dup.m_crv = crv;
            }
            dup.UpdatePreview();
            return dup;
        }
        #endregion

        #region properties
        public bool IsValid
        {
            get
            {
                if (m_crv == null)
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
            typeTxt = typeTxt.Replace("1d", "1D");
            return typeTxt;
        }

        #endregion
    }

    /// <summary>
    /// GsaMember Goo wrapper class, makes sure GsaMember can be used in Grasshopper.
    /// </summary>
    public class GsaMember1dGoo : GH_GeometricGoo<GsaMember1d>, IGH_PreviewData
    {
        #region constructors
        public GsaMember1dGoo()
        {
            this.Value = new GsaMember1d();
        }
        public GsaMember1dGoo(GsaMember1d member)
        {
            if (member == null)
                member = new GsaMember1d();
            this.Value = member; //member.Duplicate();
        }

        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return DuplicateGsaMember1d();
        }
        public GsaMember1dGoo DuplicateGsaMember1d()
        {
            return new GsaMember1dGoo(Value == null ? new GsaMember1d() : Value); //Value.Duplicate());
        }
        #endregion

        #region properties
        public override bool IsValid
        {
            get
            {
                if (Value == null) { return false; }
                if (Value.PolyCurve == null) { return false; }
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
                return "Null Member1D";
            else
                return Value.ToString();
        }
        public override string TypeName
        {
            get { return ("Member 1D"); }
        }
        public override string TypeDescription
        {
            get { return ("GSA 1D Member"); }
        }

        public override BoundingBox Boundingbox
        {
            get
            {
                if (Value == null) { return BoundingBox.Empty; }
                if (Value.PolyCurve == null) { return BoundingBox.Empty; }
                return Value.PolyCurve.GetBoundingBox(false) ;
            }
        }
        public override BoundingBox GetBoundingBox(Transform xform)
        {
            if (Value == null) { return BoundingBox.Empty; }
            if (Value.PolyCurve == null) { return BoundingBox.Empty; }
            return Value.PolyCurve.GetBoundingBox(xform);
        }
        #endregion

        #region casting methods
        public override bool CastTo<Q>(out Q target)
        {
            // This function is called when Grasshopper needs to convert this 
            // instance of GsaMember into some other type Q.            


            if (typeof(Q).IsAssignableFrom(typeof(GsaMember1d)))
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

            //Cast to Curve
            if (typeof(Q).IsAssignableFrom(typeof(Curve)))
            {
                if (Value == null)
                    target = default;
                else
                    target = (Q)(object)Value.PolyCurve.DuplicatePolyCurve();
                return true;
            }
            if (typeof(Q).IsAssignableFrom(typeof(GH_Curve)))
            {
                if (Value == null)
                    target = default;
                else
                {
                    target = (Q)(object)new GH_Curve(Value.PolyCurve.DuplicatePolyCurve());
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
                    target = (Q)(object)Value.PolyCurve.DuplicatePolyCurve();
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
                    target = (Q)(object)Value.PolyCurve.DuplicatePolyCurve();
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
            if (typeof(GsaMember1d).IsAssignableFrom(source.GetType()))
            {
                Value = (GsaMember1d)source;
                return true;
            }

            //Cast from GsaAPI Member
            //if (typeof(Member).IsAssignableFrom(source.GetType()))
            //{
            //    Value.API_Member = (Member)source;
            //    return true;
            //}
            
            //Cast from Curve
            Curve crv = null;

            if (GH_Convert.ToCurve(source, ref crv, GH_Conversion.Both))
            {
                GsaMember1d member = new GsaMember1d(crv);
                this.Value = member;
                return true;
            }
            

            return false;
        }
        #endregion

        #region transformation methods
        public override IGH_GeometricGoo Transform(Transform xform)
        {
            return new GsaMember1dGoo(Value.Transform(xform));
        }

        public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
        {
            return new GsaMember1dGoo(Value.Morph(xmorph));
        }

        #endregion

        #region drawing methods
        public BoundingBox ClippingBox
        {
            get { return Boundingbox; }
        }
        public void DrawViewportMeshes(GH_PreviewMeshArgs args)
        {
            // no meshes to be drawn
        }
        public void DrawViewportWires(GH_PreviewWireArgs args)
        {
            if (Value == null) { return; }

            //Draw lines
            if (Value.PolyCurve != null)
            {
                if (args.Color == System.Drawing.Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
                {
                    if (Value.IsDummy)
                        args.Pipeline.DrawDottedPolyline(Value.Topology, UI.Colour.Dummy1D, false);
                    else
                    {
                        if ((System.Drawing.Color)Value.Colour != System.Drawing.Color.FromArgb(0, 0, 0))
                            args.Pipeline.DrawCurve(Value.PolyCurve, Value.Colour, 2);
                        else
                        {
                            System.Drawing.Color col = UI.Colour.ElementType(Value.Type1D);
                            args.Pipeline.DrawCurve(Value.PolyCurve, col, 2);
                        }
                    }
                }
                else
                {
                    if (Value.IsDummy)
                        args.Pipeline.DrawDottedPolyline(Value.Topology, UI.Colour.Member1dSelected, false);
                    else
                        args.Pipeline.DrawCurve(Value.PolyCurve, UI.Colour.Member1dSelected, 2);
                }
            }

            //Draw points.
            if (Value.Topology != null)
            {
                if (!Value.IsDummy)
                {
                    List<Point3d> pts = Value.Topology;
                    for (int i = 0; i < pts.Count; i++)
                    {
                        if (args.Color == System.Drawing.Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
                        {
                            if (i == 0 | i == pts.Count - 1) // draw first point bigger
                                args.Pipeline.DrawPoint(pts[i], Rhino.Display.PointStyle.RoundSimple, 2, UI.Colour.Member1dNode);
                            else
                                args.Pipeline.DrawPoint(pts[i], Rhino.Display.PointStyle.RoundSimple, 1, UI.Colour.Member1dNode);
                        }
                        else
                        {
                            if (i == 0 | i == pts.Count - 1) // draw first point bigger
                                args.Pipeline.DrawPoint(pts[i], Rhino.Display.PointStyle.RoundControlPoint, 2, UI.Colour.Member1dNodeSelected);
                            else
                                args.Pipeline.DrawPoint(pts[i], Rhino.Display.PointStyle.RoundControlPoint, 1, UI.Colour.Member1dNodeSelected);
                        }
                    }
                }
            }

            //Draw releases
            if (!Value.IsDummy)
            {
                if (Value.previewGreenLines != null)
                {
                    foreach (Line ln1 in Value.previewGreenLines)
                        args.Pipeline.DrawLine(ln1, UI.Colour.Support);
                    foreach (Line ln2 in Value.previewRedLines)
                        args.Pipeline.DrawLine(ln2, UI.Colour.Release);
                }
            }
        }
        #endregion
    }

    
    /// <summary>
    /// This class provides a Parameter interface for the Data_GsaMember1d type.
    /// </summary>
    public class GsaMember1dParameter : GH_PersistentGeometryParam<GsaMember1dGoo>, IGH_PreviewObject
    {
        public GsaMember1dParameter()
          : base(new GH_InstanceDescription("1D Member", "M1D", "Maintains a collection of GSA 1D Member data.", GhSA.Components.Ribbon.CategoryName.Name(), GhSA.Components.Ribbon.SubCategoryName.Cat9()))
        {
        }

        public override Guid ComponentGuid => new Guid("0392a5a0-7762-4214-8c30-fb395365056e");

        public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.obscure;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.GsaMem1D;

        //We do not allow users to pick parameter, 
        //therefore the following 4 methods disable all this ui.
        protected override GH_GetterResult Prompt_Plural(ref List<GsaMember1dGoo> values)
        {
            return GH_GetterResult.cancel;
        }
        protected override GH_GetterResult Prompt_Singular(ref GsaMember1dGoo value)
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
