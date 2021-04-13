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
    /// Offset class, this class defines the basic properties and methods for any Gsa Offset
    /// </summary>
    public class GsaOffset

    {
        public double X1
        {
            get { return m_x1; }
            set { m_x1 = value; } //m_offset.X1 = m_x1; }
        }

        public double Y
        {
            get { return m_y; }
            set { m_y = value; } //m_offset.Y = m_y; }
        }

        public double Z
        {
            get { return m_z; }
            set { m_z = value; } //m_offset.Z = m_z; }
        }

        public double X2
        {
            get { return m_x2; }
            set { m_x2 = value; } //m_offset.X2 = m_x2; }
        }


        #region fields
        private double m_x1;
        private double m_y;
        private double m_z;
        private double m_x2;
        //private Offset m_offset;
        #endregion

        #region constructors
        public GsaOffset()
        {
            m_x1 = 0;
            m_x2 = 0;
            m_y = 0;
            m_z = 0;
            //m_offset.X1 = m_x1;
            //m_offset.X2 = m_x2;
            //m_offset.Y = m_y;
            //m_offset.Z = m_z;
        }

        public GsaOffset(double x1, double x2, double y, double z)
        {
            m_x1 = x1;
            m_x2 = x2;
            m_y = y;
            m_z = z;
            //m_offset.X1 = m_x1;
            //m_offset.X2 = m_x2;
            //m_offset.Y = m_y;
            //m_offset.Z = m_z;
        }


        public GsaOffset Duplicate()
        {
            if (this == null) { return null; }
            GsaOffset dup = new GsaOffset
            {
                X1 = m_x1,
                Y = m_y,
                Z = m_z,
                X2 = m_x2
            };

            return dup;
        }
        #endregion

        #region properties
        public bool IsValid
        {
            get
            {
                if (this == null) { return false; }
                return true;
            }
        }


        #endregion

        #region methods
        public override string ToString()
        {
            string str = "{X1:" + X1.ToString()
                + ", X2:" + X2.ToString()
                + ", Y:" + Y.ToString()
                + ", Z:" + Z.ToString() + "}";
            return "GSA Offset " + str;
        }

        #endregion
    }

    /// <summary>
    /// GsaOffset Goo wrapper class, makes sure GsaOffset can be used in Grasshopper.
    /// </summary>
    public class GsaOffsetGoo : GH_Goo<GsaOffset>
    {
        #region constructors
        public GsaOffsetGoo()
        {
            this.Value = new GsaOffset();
        }
        public GsaOffsetGoo(GsaOffset offset)
        {
            if (offset == null)
                offset = new GsaOffset();
            this.Value = offset; //offset.Duplicate();
        }

        public override IGH_Goo Duplicate()
        {
            return DuplicateGsaOffset();
        }
        public GsaOffsetGoo DuplicateGsaOffset()
        {
            return new GsaOffsetGoo(Value == null ? new GsaOffset() : Value); //Value.Duplicate());
        }
        #endregion

        #region properties
        public override bool IsValid
        {
            get
            {
                if (Value == null) { return false; }
                return true;
            }
        }
        public override string IsValidWhyNot
        {
            get
            {
                //if (Value == null) { return "No internal GsaOffset instance"; }
                if (Value.IsValid) { return string.Empty; }
                return Value.IsValid.ToString(); //Todo: beef this up to be more informative.
            }
        }
        public override string ToString()
        {
            if (Value == null)
                return "Null GSA Offset";
            else
                return Value.ToString();
        }
        public override string TypeName
        {
            get { return ("GSA Offset"); }
        }
        public override string TypeDescription
        {
            get { return ("GSA Offset"); }
        }


        #endregion

        #region casting methods
        public override bool CastTo<Q>(ref Q target)
        {
            // This function is called when Grasshopper needs to convert this 
            // instance of GsaOffset into some other type Q.            


            if (typeof(Q).IsAssignableFrom(typeof(GsaOffset)))
            {
                if (Value == null)
                    target = default;
                else
                    target = (Q)(object)Value.Duplicate();
                return true;
            }

            if (typeof(Q).IsAssignableFrom(typeof(Offset)))
            {
                if (Value == null)
                    target = default;
                else
                    target = (Q)(object)Value;
                return true;
            }


            target = default;
            return false;
        }
        public override bool CastFrom(object source)
        {
            // This function is called when Grasshopper needs to convert other data 
            // into GsaOffset.


            if (source == null) { return false; }

            //Cast from GsaOffset
            if (typeof(GsaOffset).IsAssignableFrom(source.GetType()))
            {
                Value = (GsaOffset)source;
                return true;
            }


            //Cast from double
            if (GH_Convert.ToDouble(source, out double myval, GH_Conversion.Both))
            {
                Value.Z = myval;
                // if input to parameter is a single number convert it to the most common Z-offset
                return true;
            }

            return false;
        }
        #endregion


    }

    /// <summary>
    /// This class provides a Parameter interface for the Data_GsaOffset type.
    /// </summary>
    public class GsaOffsetParameter : GH_PersistentParam<GsaOffsetGoo>
    {
        public GsaOffsetParameter()
          : base(new GH_InstanceDescription("Offset", "Of", "GSA Offset", GhSA.Components.Ribbon.CategoryName.Name(), GhSA.Components.Ribbon.SubCategoryName.Cat9()))
        {
        }

        public override Guid ComponentGuid => new Guid("0b14f16e-bd6a-4da7-991a-359f64aa28fd");

        public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.GsaOffset;

        //We do not allow users to pick parameter, 
        //therefore the following 4 methods disable all this ui.
        protected override GH_GetterResult Prompt_Plural(ref List<GsaOffsetGoo> values)
        {
            return GH_GetterResult.cancel;
        }
        protected override GH_GetterResult Prompt_Singular(ref GsaOffsetGoo value)
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

        public bool Hidden
        {
            get { return true; }
            //set { m_hidden = value; }
        }
        public bool IsPreviewCapable
        {
            get { return false; }
        }
        #endregion
    }

}
