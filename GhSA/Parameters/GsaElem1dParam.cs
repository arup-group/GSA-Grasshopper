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
    /// Element1d class, this class defines the basic properties and methods for any Gsa Element 1d
    /// </summary>
    public class GsaElement1d
    {
        public LineCurve Line
        {
            get { return m_line; }
            set 
            { 
                m_line = value;
                UpdatePreview();
            }
        }
        public int ID
        {
            get { return m_id; }
            set { m_id = value; }
        }
        public GsaBool6 ReleaseStart
        {
            get 
            {
                Bool6 rel1 = m_element.Release(0);
                m_rel1 = new GsaBool6();
                m_rel1.X = rel1.X;
                m_rel1.Y = rel1.Y;
                m_rel1.Z = rel1.Z;
                m_rel1.XX = rel1.XX;
                m_rel1.YY = rel1.YY;
                m_rel1.ZZ = rel1.ZZ;
                return m_rel1; 
            } 
            set 
            { 
                m_rel1 = value;
                if (m_rel2 == null) { m_rel2 = new GsaBool6(); }
                UpdatePreview();
                //Bool6 bool6 = new Bool6();
                //bool6.X = m_rel1.X;
                //bool6.Y = m_rel1.Y;
                //bool6.Z = m_rel1.Z;
                //bool6.XX = m_rel1.XX;
                //bool6.YY = m_rel1.YY;
                //bool6.ZZ = m_rel1.ZZ;
                //m_element.SetRelease(0, Bool6);
            }
        }
        public GsaBool6 ReleaseEnd
        {
            get 
            {
                Bool6 rel2 = m_element.Release(1);
                m_rel2 = new GsaBool6();
                m_rel2.X = rel2.X;
                m_rel2.Y = rel2.Y;
                m_rel2.Z = rel2.Z;
                m_rel2.XX = rel2.XX;
                m_rel2.YY = rel2.YY;
                m_rel2.ZZ = rel2.ZZ;
                return m_rel2;
            }
            set 
            { 
                m_rel2 = value;
                if (m_rel1 == null) { m_rel1 = new GsaBool6(); }
                UpdatePreview();
                //Bool6 bool6 = new Bool6();
                //bool6.X = m_rel2.X;
                //bool6.Y = m_rel2.Y;
                //bool6.Z = m_rel2.Z;
                //bool6.XX = m_rel2.XX;
                //bool6.YY = m_rel2.YY;
                //bool6.ZZ = m_rel2.ZZ;
                //m_element.SetRelease(1, Bool6);
            }
        }
        public GsaSection Section
        {
            get { return m_section; }
            set { m_section = value; }
        }

        #region GsaAPI.Element members
        internal Element API_Element
        {
            get { return m_element; }
            set { m_element = value; }
        }
        internal Element GetAPI_ElementClone()
        {
            Element elem = new Element()
            {
            Group = m_element.Group,
                IsDummy = m_element.IsDummy,
                Name = m_element.Name.ToString(),
                Offset = m_element.Offset,
                OrientationAngle = m_element.OrientationAngle,
                OrientationNode = m_element.OrientationNode,
                ParentMember = m_element.ParentMember,
                Property = m_element.Property,
                Topology = new ReadOnlyCollection<int>(m_element.Topology.ToList()),
                Type = m_element.Type //GsaToModel.Element1dType((int)Element.Type)
            };
            if ((System.Drawing.Color) m_element.Colour != System.Drawing.Color.FromArgb(0, 0, 0)) // workaround to handle that System.Drawing.Color is non-nullable type
                elem.Colour = m_element.Colour;
            return elem;
        }
        public System.Drawing.Color Colour
        {
            get
            {
                return (System.Drawing.Color)m_element.Colour;
            }
            set 
            {
                CloneApiElement();
                m_element.Colour = value; 
            }
        }
        public int Group
        {
            get { return m_element.Group; }
            set 
            {
                CloneApiElement();
                m_element.Group = value; 
            }
        }
        public bool IsDummy
        {
            get { return m_element.IsDummy; }
            set
            {
                CloneApiElement();
                m_element.IsDummy = value;
            }
        }
        public string Name
        {
            get { return m_element.Name; }
            set
            {
                CloneApiElement();
                m_element.Name = value;
            }
        }
        public GsaOffset Offset
        {
            get 
            {
                return new GsaOffset(
                    m_element.Offset.X1,
                    m_element.Offset.X2,
                    m_element.Offset.Y,
                    m_element.Offset.Z); 
            }
            set
            {
                CloneApiElement();
                m_element.Offset.X1 = value.X1.Meters;
                m_element.Offset.X2 = value.X2.Meters;
                m_element.Offset.Y = value.Y.Meters;
                m_element.Offset.Z = value.Z.Meters;
            }
        }
        public double OrientationAngle
        {
            get { return m_element.OrientationAngle; }
            set
            {
                CloneApiElement();
                m_element.OrientationAngle = value;
            }
        }
        public GsaNode OrientationNode
        {
            get { return m_orientationNode; }
            set
            {
                CloneApiElement();
                m_orientationNode = value;
            }
        }
        public int ParentMember
        {
            get { return m_element.ParentMember.Member; }
        }
        public ElementType Type
        {
            get { return m_element.Type; }
            set
            {
                CloneApiElement();
                m_element.Type = value;
            }
        }
        internal void CloneApiElement()
        {
            Element elem = new Element()
            {
                Group = m_element.Group,
                IsDummy = m_element.IsDummy,
                Name = m_element.Name.ToString(),
                Offset = m_element.Offset,
                OrientationAngle = m_element.OrientationAngle,
                OrientationNode = m_element.OrientationNode,
                ParentMember = m_element.ParentMember,
                Property = m_element.Property,
                Topology = new ReadOnlyCollection<int>(m_element.Topology.ToList()),
                Type = m_element.Type //GsaToModel.Element1dType((int)Element.Type)
            };
            if ((System.Drawing.Color)m_element.Colour != System.Drawing.Color.FromArgb(0, 0, 0)) // workaround to handle that System.Drawing.Color is non-nullable type
                elem.Colour = m_element.Colour;
            m_element = elem;
        }
        
        #endregion
        #region preview
        internal Point3d previewPointStart;
        internal Point3d previewPointEnd;
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
        internal void UpdatePreview()
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
                    PolyCurve crv = new PolyCurve();
                    crv.Append(m_line);
                    GhSA.UI.Display.Preview1D(crv, m_element.OrientationAngle * Math.PI / 180.0, m_rel1, m_rel2,
                        ref previewGreenLines, ref previewRedLines);
                }
                else
                    previewGreenLines = null;
            }
            
            previewPointStart = m_line.PointAtStart;
            previewPointEnd = m_line.PointAtEnd;
        }
        #endregion
        #region fields
        private Element m_element; 
        private LineCurve m_line;
        private int m_id = 0;
        private GsaBool6 m_rel1;
        private GsaBool6 m_rel2;
        private GsaSection m_section;
        private GsaNode m_orientationNode;
        #endregion
        #region constructors
        public GsaElement1d()
        {
            m_element = new Element();
            m_line = new LineCurve();
            m_section = new GsaSection();
        }
        public GsaElement1d(LineCurve line, int prop = 0, int ID = 0, GsaNode orientationNode = null)
        {
            m_element = new Element
            {
                Type = ElementType.BEAM,
            };
            
            m_line = line;
            m_section = new GsaSection();
            m_id = ID;
            m_section.ID = prop;
            if (orientationNode != null)
                m_orientationNode = orientationNode;
            UpdatePreview();
        }
        internal GsaElement1d(Element elem, LineCurve line, int ID, GsaSection section, GsaNode orientationNode)
        {
            m_element = elem;
            m_line = line;

            m_rel1 = new GsaBool6()
            {
                X = elem.Release(0).X,
                Y = elem.Release(0).Y,
                Z = elem.Release(0).Z,
                XX = elem.Release(0).XX,
                YY = elem.Release(0).YY,
                ZZ = elem.Release(0).ZZ
            };
            m_rel2 = new GsaBool6()
            {
                X = elem.Release(1).X,
                Y = elem.Release(1).Y,
                Z = elem.Release(1).Z,
                XX = elem.Release(1).XX,
                YY = elem.Release(1).YY,
                ZZ = elem.Release(1).ZZ
            };
            
            m_id = ID;

            m_section = section;
            
            if (orientationNode != null)
                m_orientationNode = orientationNode;

            UpdatePreview();
        }
        public GsaElement1d Duplicate(bool cloneApiElement = false)
        {
            if (this == null) { return null; }
            GsaElement1d dup = new GsaElement1d();
            dup.m_id = m_id;
            dup.m_element = m_element;
            if (cloneApiElement)
                dup.CloneApiElement();
            dup.m_line = (LineCurve)m_line.DuplicateShallow();
            if (m_rel1 != null)
                dup.m_rel1 = m_rel1.Duplicate();
            if (m_rel2 != null)
                dup.m_rel2 = m_rel2.Duplicate();
            dup.m_section = m_section.Duplicate();
            if (m_orientationNode != null)
                dup.m_orientationNode = m_orientationNode.Duplicate();
            UpdatePreview();
            return dup;
        }
        #endregion

        #region properties
        public bool IsValid
        {
            get
            {
                if (m_line == null)
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
            return "GSA 1D Element" + idd;
        }
        #endregion
    }

    /// <summary>
    /// GsaMember Goo wrapper class, makes sure GsaMember can be used in Grasshopper.
    /// </summary>
    public class GsaElement1dGoo : GH_GeometricGoo<GsaElement1d>, IGH_PreviewData
    {
        #region constructors
        public GsaElement1dGoo()
        {
            this.Value = new GsaElement1d();
        }
        public GsaElement1dGoo(GsaElement1d element)
        {
            if (element == null)
                element = new GsaElement1d();
            this.Value = element; //element.Duplicat();
        }

        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return DuplicateGsaElement1d();
        }
        public GsaElement1dGoo DuplicateGsaElement1d()
        {
            return new GsaElement1dGoo(Value == null ? new GsaElement1d() : Value); //Value.Duplicate());
        }
        #endregion

        #region properties
        public override bool IsValid
        {
            get
            {
                if (Value == null) { return false; }
                if (Value.Line == null) { return false; }
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
                return "Null Element1D";
            else
                return Value.ToString();
        }
        public override string TypeName
        {
            get { return ("Element 1D"); }
        }
        public override string TypeDescription
        {
            get { return ("GSA 1D Element"); }
        }

        public override BoundingBox Boundingbox
        {
            get
            {
                if (Value == null) { return BoundingBox.Empty; }
                if (Value.Line == null) { return BoundingBox.Empty; }
                return Value.Line.GetBoundingBox(false) ;
            }
        }
        public override BoundingBox GetBoundingBox(Transform xform)
        {
            if (Value == null) { return BoundingBox.Empty; }
            if (Value.Line == null) { return BoundingBox.Empty; }
            return Value.Line.GetBoundingBox(xform);
        }
        #endregion

        #region casting methods
        public override bool CastTo<Q>(out Q target)
        {
            // This function is called when Grasshopper needs to convert this 
            // instance of GsaElement into some other type Q.            


            if (typeof(Q).IsAssignableFrom(typeof(GsaElement1d)))
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
                    target = (Q)(object)Value.GetAPI_ElementClone();
                return true;
            }

            //Cast to Curve
            if (typeof(Q).IsAssignableFrom(typeof(Line)))
            {
                if (Value == null)
                    target = default;
                else
                    target = (Q)(object)Value.Line;
                return true;
            }
            if (typeof(Q).IsAssignableFrom(typeof(GH_Line)))
            {
                if (Value == null)
                    target = default;
                else
                {
                    GH_Line ghLine = new GH_Line();
                    GH_Convert.ToGHLine(Value.Line, GH_Conversion.Both, ref ghLine);
                    target = (Q)(object)ghLine;
                }
                    
                return true;
            }
            if (typeof(Q).IsAssignableFrom(typeof(Curve)))
            {
                if (Value == null)
                    target = default;
                else
                    target = (Q)(object)Value.Line;
                return true;
            }
            if (typeof(Q).IsAssignableFrom(typeof(GH_Curve)))
            {
                if (Value == null)
                    target = default;
                else
                {
                    target = (Q)(object)new GH_Curve(Value.Line);
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
            // into GsaElement.


            if (source == null) { return false; }

            //Cast from GsaElement
            if (typeof(GsaElement1d).IsAssignableFrom(source.GetType()))
            {
                Value = (GsaElement1d)source;
                return true;
            }

            //Cast from GsaAPI Member
            // we shouldnt provide auto-convertion from GsaAPI.Element
            // as this cannot alone be used to create a line....
            //if (typeof(Element).IsAssignableFrom(source.GetType()))
            //{
            //    Value.Element = (Element)source;
            //    return true;
            //}

            //Cast from Curve
            Line ln = new Line();

            if (GH_Convert.ToLine(source, ref ln, GH_Conversion.Both))
            {
                LineCurve crv = new LineCurve(ln);
                GsaElement1d elem = new GsaElement1d(crv);
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
            if (Value.Line == null) { return null; }

            GsaElement1d elem = Value.Duplicate(true);
            LineCurve xLn = elem.Line;
            xLn.Transform(xform);
            elem.Line = xLn;

            return new GsaElement1dGoo(elem);
        }

        public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
        {
            if (Value == null) { return null; }
            if (Value.Line == null) { return null; }

            GsaElement1d elem = Value.Duplicate(true);
            LineCurve xLn = Value.Line;
            xmorph.Morph(xLn);
            elem.Line = xLn;

            return new GsaElement1dGoo(elem);
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
            //no meshes to be drawn
        }
        public void DrawViewportWires(GH_PreviewWireArgs args)
        {
            if (Value == null) { return; }

            //Draw lines
            if (Value.Line != null)
            {
                if (args.Color == System.Drawing.Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
                {
                    if (Value.IsDummy)
                        args.Pipeline.DrawDottedLine(Value.previewPointStart, Value.previewPointEnd, UI.Colour.Dummy1D);
                    else
                    {
                        if ((System.Drawing.Color)Value.Colour != System.Drawing.Color.FromArgb(0, 0, 0))
                            args.Pipeline.DrawCurve(Value.Line, Value.Colour, 2);
                        else
                        {
                            System.Drawing.Color col = UI.Colour.ElementType(Value.Type);
                            args.Pipeline.DrawCurve(Value.Line, col, 2);
                        }
                        //args.Pipeline.DrawPoint(Value.previewPointStart, Rhino.Display.PointStyle.RoundSimple, 3, UI.Colour.Element1dNode);
                        //args.Pipeline.DrawPoint(Value.previewPointEnd, Rhino.Display.PointStyle.RoundSimple, 3, UI.Colour.Element1dNode);
                    }
                }
                else
                {
                    if (Value.IsDummy)
                        args.Pipeline.DrawDottedLine(Value.previewPointStart, Value.previewPointEnd, UI.Colour.Element1dSelected);
                    else
                    {
                        args.Pipeline.DrawCurve(Value.Line, UI.Colour.Element1dSelected, 2);
                        //args.Pipeline.DrawPoint(Value.previewPointStart, Rhino.Display.PointStyle.RoundControlPoint, 3, UI.Colour.Element1dNodeSelected);
                        //args.Pipeline.DrawPoint(Value.previewPointEnd, Rhino.Display.PointStyle.RoundControlPoint, 3, UI.Colour.Element1dNodeSelected);
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
    /// This class provides a Parameter interface for the Data_GsaElement1d type.
    /// </summary>
    public class GsaElement1dParameter : GH_PersistentGeometryParam<GsaElement1dGoo>, IGH_PreviewObject
    {
        public GsaElement1dParameter()
          : base(new GH_InstanceDescription("1D Element", "E1D", "Maintains a collection of GSA 1D Element data.", GhSA.Components.Ribbon.CategoryName.Name(), GhSA.Components.Ribbon.SubCategoryName.Cat9()))
        {
        }

        public override Guid ComponentGuid => new Guid("9c045214-cab6-47d9-a158-ae1f4f494b66");

        public override GH_Exposure Exposure => GH_Exposure.tertiary;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.Elem1dParam;

        //We do not allow users to pick parameter, 
        //therefore the following 4 methods disable all this ui.
        protected override GH_GetterResult Prompt_Plural(ref List<GsaElement1dGoo> values)
        {
            return GH_GetterResult.cancel;
        }
        protected override GH_GetterResult Prompt_Singular(ref GsaElement1dGoo value)
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
