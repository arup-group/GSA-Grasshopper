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
    /// Element1d class, this class defines the basic properties and methods for any Gsa Element 1d
    /// </summary>
    public class GsaElement1d

    {
        public Element Element
        {
            get { return m_element; }
            set { m_element = value; } //update release etc when setting?
        }
        public LineCurve Line
        {
            get { return m_line; }
            set { m_line = value; }
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
                //m_rel1.X = m_element.Release(0).X;
                //m_rel1.Y = m_element.Release(0).Y;
                //m_rel1.Z = m_element.Release(0).Z;
                //m_rel1.XX = m_element.Release(0).XX;
                //m_rel1.YY = m_element.Release(0).YY;
                //m_rel1.ZZ = m_element.Release(0).ZZ;
                return m_rel1; 
            } 
            set 
            { 
                m_rel1 = value;
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
                //m_rel2.X = m_element.Release(1).X;
                //m_rel2.Y = m_element.Release(1).Y;
                //m_rel2.Z = m_element.Release(1).Z;
                //m_rel2.XX = m_element.Release(1).XX;
                //m_rel2.YY = m_element.Release(1).YY;
                //m_rel2.ZZ = m_element.Release(1).ZZ;
                return m_rel2; 
            }
            set 
            { 
                m_rel2 = value;
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

        #region fields
        private Element m_element; 
        private LineCurve m_line;
        private int m_id = 0;
        private GsaBool6 m_rel1;
        private GsaBool6 m_rel2;
        private GsaSection m_section;
        #endregion

        #region constructors
        public GsaElement1d()
        {
            m_element = new Element();
            m_line = new LineCurve();
        }


        public GsaElement1d(LineCurve line, int prop = 1)
        {
            m_element = new Element
            {
                Type = ElementType.BEAM,
                Property = prop
            };

            m_line = line;
        }

        //public GsaElement1d(Element element, LineCurve line)
        //{
        //    m_element = element;
        //    m_line = line;
        //}

        public GsaElement1d Duplicate()
        {
            GsaElement1d dup = new GsaElement1d();
            dup.m_element = m_element;
            
            if (m_line != null)
                dup.m_line = (LineCurve)m_line.Duplicate();
            dup.ID = m_id;

            if (m_rel1 != null)
                dup.ReleaseStart = m_rel1.Duplicate();
            if (m_rel2 != null)
                dup.ReleaseEnd = m_rel2.Duplicate();
            if (m_section != null)
                dup.Section = m_section.Duplicate();

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
            this.Value = element;
        }


        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return DuplicateGsaElement1d();
        }
        public GsaElement1dGoo DuplicateGsaElement1d()
        {
            return new GsaElement1dGoo(Value == null ? new GsaElement1d() : Value.Duplicate());
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
                    target = (Q)(object)Value;
                return true;
            }

            if (typeof(Q).IsAssignableFrom(typeof(Element)))
            {
                if (Value == null)
                    target = default;
                else
                    target = (Q)(object)Value.Element;
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
            if (typeof(Element).IsAssignableFrom(source.GetType()))
            {
                Value.Element = (Element)source;
                return true;
            }

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

            GsaElement1d elem = Value.Duplicate();
            LineCurve xLn = Value.Line;
            xLn.Transform(xform);
            elem.Line = xLn;

            return new GsaElement1dGoo(elem);
        }

        public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
        {
            if (Value == null) { return null; }
            if (Value.Line == null) { return null; }

            GsaElement1d elem = Value.Duplicate();
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
                    args.Pipeline.DrawCurve(Value.Line, UI.Colour.Element1d, 2);
                    args.Pipeline.DrawPoint(Value.Line.PointAtStart, Rhino.Display.PointStyle.RoundSimple, 3, UI.Colour.Element1dNode);
                    args.Pipeline.DrawPoint(Value.Line.PointAtEnd, Rhino.Display.PointStyle.RoundSimple, 3, UI.Colour.Element1dNode);
                }
                else
                {
                    args.Pipeline.DrawCurve(Value.Line, UI.Colour.Element1dSelected, 2);
                    args.Pipeline.DrawPoint(Value.Line.PointAtStart, Rhino.Display.PointStyle.RoundControlPoint, 3, UI.Colour.Element1dNodeSelected);
                    args.Pipeline.DrawPoint(Value.Line.PointAtEnd, Rhino.Display.PointStyle.RoundControlPoint, 3, UI.Colour.Element1dNodeSelected);
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
          : base(new GH_InstanceDescription("GSA 1D Element", "Element 1D", "Maintains a collection of GSA 1D Element data.", GhSA.Components.Ribbon.CategoryName.Name(), GhSA.Components.Ribbon.SubCategoryName.Cat9()))
        {
        }

        public override Guid ComponentGuid => new Guid("9c045214-cab6-47d9-a158-ae1f4f494b66");

        public override GH_Exposure Exposure => GH_Exposure.tertiary;

        protected override System.Drawing.Bitmap Icon => GSA.Properties.Resources.GsaElem1D;

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
