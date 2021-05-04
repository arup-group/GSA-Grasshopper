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
    /// Prop2d class, this class defines the basic properties and methods for any Gsa Prop2d
    /// </summary>
    public class GsaProp2d

    {
        public Prop2D Prop2d
        {
            get { return m_prop2d; }
            set { m_prop2d = value; }
        }

        public int ID
        {
            get { return m_idd; }
            set { m_idd = value; }
        }

        public GsaMaterial Material
        {
            get { return m_material; }
            set 
            { 
                m_material = value;
                m_prop2d.MaterialType = Util.Gsa.ToGSA.Materials.ConvertType(m_material);
                m_prop2d.MaterialAnalysisProperty = m_material.AnalysisProperty;
                m_prop2d.MaterialGradeProperty = m_material.Grade;
            }
        }

        #region fields
        Prop2D m_prop2d;
        int m_idd = 0;
        GsaMaterial m_material = null;
        #endregion

        #region constructors
        public GsaProp2d()
        {
            m_prop2d = new Prop2D();
        }
        
        public GsaProp2d Duplicate()
        {
            if (this == null) { return null; }
            GsaProp2d dup = new GsaProp2d();
            if (m_prop2d != null)
            {
                dup.Prop2d = new Prop2D
                {
                    MaterialAnalysisProperty = m_prop2d.MaterialAnalysisProperty,
                    MaterialGradeProperty = m_prop2d.MaterialGradeProperty,
                    MaterialType = m_prop2d.MaterialType,
                    Name = m_prop2d.Name.ToString(),
                    Description = m_prop2d.Description.ToString(),
                    Type = m_prop2d.Type, //GsaToModel.Prop2dType((int)m_prop2d.Type),
                    AxisProperty = m_prop2d.AxisProperty
                };
                if ((System.Drawing.Color)m_prop2d.Colour != System.Drawing.Color.FromArgb(0, 0, 0)) // workaround to handle that System.Drawing.Color is non-nullable type
                    dup.m_prop2d.Colour = m_prop2d.Colour;
            }
            else
                dup.Prop2d = null;

            dup.ID = m_idd;
            
            if (Material != null)
                dup.Material = m_material.Duplicate();
            
            return dup;
        }
        #endregion

        #region properties
        public bool IsValid
        {
            get
            {
                return true;
            }
        }
        #endregion

        #region methods
        public override string ToString()
        {
            string str = "";
            if (m_prop2d != null)
            {
                str = m_prop2d.Type.ToString();
                str = Char.ToUpper(str[0]) + str.Substring(1).ToLower().Replace("_", " ");
            }
            string pa = (ID > 0) ? "PA" + ID + " " : ""; 
            return "GSA 2D Property " + ((ID > 0) ? pa : "") + ((m_prop2d == null) ? "" : str);
        }

        #endregion
    }

    /// <summary>
    /// GsaProp2d Goo wrapper class, makes sure GsaProp2d can be used in Grasshopper.
    /// </summary>
    public class GsaProp2dGoo : GH_Goo<GsaProp2d>
    {
        #region constructors
        public GsaProp2dGoo()
        {
            this.Value = new GsaProp2d();
        }
        public GsaProp2dGoo(GsaProp2d prop)
        {
            if (prop == null)
                prop = new GsaProp2d();
            this.Value = prop; //prop.Duplicate();
        }

        public override IGH_Goo Duplicate()
        {
            return DuplicateGsaProp2d();
        }
        public GsaProp2dGoo DuplicateGsaProp2d()
        {
            return new GsaProp2dGoo(Value == null ? new GsaProp2d() : Value); //Value.Duplicate());
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
                //if (Value == null) { return "No internal GsaMember instance"; }
                if (Value.IsValid) { return string.Empty; }
                return Value.IsValid.ToString(); //Todo: beef this up to be more informative.
            }
        }
        public override string ToString()
        {
            if (Value == null)
                return "Null GSA Prop2d";
            else
                return Value.ToString();
        }
        public override string TypeName
        {
            get { return ("GSA 2D Property"); }
        }
        public override string TypeDescription
        {
            get { return ("GSA 2D Property"); }
        }


        #endregion

        #region casting methods
        public override bool CastTo<Q>(ref Q target)
        {
            // This function is called when Grasshopper needs to convert this 
            // instance of GsaProp2d into some other type Q.            


            if (typeof(Q).IsAssignableFrom(typeof(GsaProp2d)))
            {
                if (Value == null)
                    target = default;
                else
                    target = (Q)(object)Value.Duplicate();
                return true;
            }

            if (typeof(Q).IsAssignableFrom(typeof(Prop2D)))
            {
                if (Value == null)
                    target = default;
                else
                    target = (Q)(object)Value;
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
            // into GsaProp2d.


            if (source == null) { return false; }

            //Cast from GsaProp2d
            if (typeof(GsaProp2d).IsAssignableFrom(source.GetType()))
            {
                Value = (GsaProp2d)source;
                return true;
            }

            //Cast from double
            if (GH_Convert.ToDouble(source, out double thk, GH_Conversion.Both))
            {
                //Value.Prop2d.Thickness = thk; // To be added; GsaAPI bug
            }
            return false;
        }
        #endregion


    }

    /// <summary>
    /// This class provides a Parameter interface for the Data_GsaSection type.
    /// </summary>
    public class GsaProp2dParameter : GH_PersistentParam<GsaProp2dGoo>
    {
        public GsaProp2dParameter()
          : base(new GH_InstanceDescription("2D Property", "PA", "GSA 2D Property", GhSA.Components.Ribbon.CategoryName.Name(), GhSA.Components.Ribbon.SubCategoryName.Cat9()))
        {
        }

        public override Guid ComponentGuid => new Guid("05a034ad-683d-479b-9768-5c04379c0606");

        public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.GsaProp2D;

        protected override GH_GetterResult Prompt_Plural(ref List<GsaProp2dGoo> values)
        {
            return GH_GetterResult.cancel;
        }
        protected override GH_GetterResult Prompt_Singular(ref GsaProp2dGoo value)
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
