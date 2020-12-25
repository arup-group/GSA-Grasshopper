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
    /// Spring class, this class defines the basic properties and methods for any Gsa Spring
    /// </summary>
    public class GsaSpring

    {
        public double X
        {
            get { return m_x; }
            set { m_x = value; }
        }

        public double Y
        {
            get { return m_y; }
            set { m_y = value; }
        }

        public double Z
        {
            get { return m_z; }
            set { m_z = value; }
        }

        public double XX
        {
            get { return m_xx; }
            set { m_xx = value; }
        }

        public double YY
        {
            get { return m_yy; }
            set { m_yy = value; }
        }

        public double ZZ
        {
            get { return m_zz; }
            set { m_zz = value; }
        }

        #region fields
        private double m_x;
        private double m_y;
        private double m_z;
        private double m_xx;
        private double m_yy;
        private double m_zz;
        #endregion

        #region constructors
        public GsaSpring()
        {
            
        }

        //public GsaSpring(Double6 double6)
        //{
        //    m_x = double6.X;
        //    m_y = double6.Y;
        //    m_z = double6.Z;
        //    m_xx = double6.XX;
        //    m_yy = double6.YY;
        //    m_zz = double6.ZZ;
        //}


        public GsaSpring Duplicate()
        {
            if (this == null) { return null; }
            GsaSpring dup = new GsaSpring
            {
                X = m_x,
                Y = m_y,
                Z = m_z,
                XX = m_xx,
                YY = m_yy,
                ZZ = m_zz
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
            string str = System.Environment.NewLine
                + "X: " + X.ToString()
                + ", Y: " + Y.ToString()
                + ", Z: " + Z.ToString()
                + ", XX: " + XX.ToString()
                + ", YY: " + YY.ToString()
                + ", ZZ: " + ZZ.ToString();
            return "GSA General Spring" + str;
        }

        #endregion
    }

    /// <summary>
    /// GsaSrping Goo wrapper class, makes sure GsaSpring can be used in Grasshopper.
    /// </summary>
    public class GsaSpringGoo : GH_Goo<GsaSpring>
    {
        #region constructors
        public GsaSpringGoo()
        {
            this.Value = new GsaSpring();
        }
        public GsaSpringGoo(GsaSpring spring)
        {
            if (spring == null)
                spring = new GsaSpring();
            this.Value = spring.Duplicate();
        }

        public override IGH_Goo Duplicate()
        {
            return DuplicateGsaSpring();
        }
        public GsaSpringGoo DuplicateGsaSpring()
        {
            return new GsaSpringGoo(Value == null ? new GsaSpring() : Value.Duplicate());
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
                //if (Value == null) { return "No internal GsaSpring instance"; }
                if (Value.IsValid) { return string.Empty; }
                return Value.IsValid.ToString(); //Todo: beef this up to be more informative.
            }
        }
        public override string ToString()
        {
            if (Value == null)
                return "Null GSA Spring";
            else
                return Value.ToString();
        }
        public override string TypeName
        {
            get { return ("GSA Spring"); }
        }
        public override string TypeDescription
        {
            get { return ("GSA Spring (Type: General)"); }
        }


        #endregion

        #region casting methods
        public override bool CastTo<Q>(ref Q target)
        {
            // This function is called when Grasshopper needs to convert this 
            // instance of GsaSpring into some other type Q.            


            if (typeof(Q).IsAssignableFrom(typeof(GsaSpring)))
            {
                if (Value == null)
                    target = default;
                else
                    target = (Q)(object)Value;
                return true;
            }

            if (typeof(Q).IsAssignableFrom(typeof(Double6)))
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
            // into GsaSpring.


            if (source == null) { return false; }

            //Cast from GsaSpring
            if (typeof(GsaSpring).IsAssignableFrom(source.GetType()))
            {
                Value = (GsaSpring)source;
                return true;
            }


            //Cast from double
            if (GH_Convert.ToDouble(source, out double myval, GH_Conversion.Both))
            {
                Value.X = myval;
                // if input to parameter is a single number convert it to an axial spring
                return true;
            }
            
            return false;
        }
        #endregion


    }

    /// <summary>
    /// This class provides a Parameter interface for the Data_GsaModel type.
    /// </summary>
    public class GsaSpringParameter : GH_PersistentParam<GsaSpringGoo>
    {
        public GsaSpringParameter()
          : base(new GH_InstanceDescription("GSA Spring", "Spring", "GSA Spring (Type: General)", GhSA.Components.Ribbon.CategoryName.Name(), GhSA.Components.Ribbon.SubCategoryName.Cat9()))
        {
        }

        public override Guid ComponentGuid => new Guid("74f4dbb6-78c5-40b3-a4c6-259e3c7b716c");

        public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.GsaSpring;

        //We do not allow users to pick parameter, 
        //therefore the following 4 methods disable all this ui.
        protected override GH_GetterResult Prompt_Plural(ref List<GsaSpringGoo> values)
        {
            return GH_GetterResult.cancel;
        }
        protected override GH_GetterResult Prompt_Singular(ref GsaSpringGoo value)
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
