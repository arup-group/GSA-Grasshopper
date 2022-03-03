using System;
using System.Collections.Generic;
using System.Linq;

using GsaAPI;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using UnitsNet;
using UnitsNet.Units;

namespace GsaGH.Parameters
{
    /// <summary>
    /// Prop2d class, this class defines the basic properties and methods for any Gsa Prop2d
    /// </summary>
    public class GsaProp3d
    {
        internal Prop3D API_Prop3d
        {
            get { return m_prop3d; }
            set
            {
                m_guid = Guid.NewGuid();
                m_prop3d = value;
                m_material = new GsaMaterial(this);
            }
        }
        public int ID
        {
            get { return m_idd; }
            set 
            {
                m_guid = Guid.NewGuid(); 
                m_idd = value; 
            }
        }
        public GsaMaterial Material
        {
            get { return m_material; }
            set 
            {
                m_material = value;
                if (m_prop3d == null)
                    m_prop3d = new Prop3D();
                else
                    CloneProperty();
                m_prop3d.MaterialType = Util.Gsa.ToGSA.Materials.ConvertType(m_material);
                m_prop3d.MaterialAnalysisProperty = m_material.AnalysisProperty;
                m_prop3d.MaterialGradeProperty = m_material.GradeProperty;
            }
        }
        #region GsaAPI members
        public string Name
        {
            get { return m_prop3d.Name; }
            set
            {
                CloneProperty();
                m_prop3d.Name = value;
            }
        }
        public int MaterialID
        {
            get { return m_prop3d.MaterialAnalysisProperty; }
            set
            {
                CloneProperty();
                m_prop3d.MaterialAnalysisProperty = value;
                m_material.AnalysisProperty = m_prop3d.MaterialAnalysisProperty;
            }
        }
        
        
        public int AxisProperty
        {
            get { return m_prop3d.AxisProperty; }
            set
            {
                CloneProperty();
                value = Math.Min(1, value);
                value = Math.Max(0, value);
                m_prop3d.AxisProperty = value * -1;
            }
        }
        
        public System.Drawing.Color Colour
        {
            get { return (System.Drawing.Color)m_prop3d.Colour; }
            set
            {
                CloneProperty();
                m_prop3d.Colour = value;
            }
        }
        private void CloneProperty()
        {
            if (m_prop3d == null) 
            { 
                m_prop3d = new Prop3D();
                m_guid = Guid.NewGuid(); 
                return; 
            }
            Prop3D prop = new Prop3D
            {
                MaterialAnalysisProperty = m_prop3d.MaterialAnalysisProperty,
                MaterialGradeProperty = m_prop3d.MaterialGradeProperty,
                MaterialType = m_prop3d.MaterialType,
                Name = m_prop3d.Name.ToString(),
                AxisProperty = m_prop3d.AxisProperty
            };
            if ((System.Drawing.Color)m_prop3d.Colour != System.Drawing.Color.FromArgb(0, 0, 0)) // workaround to handle that System.Drawing.Color is non-nullable type
                prop.Colour = m_prop3d.Colour;

            m_prop3d = prop;
            m_guid = Guid.NewGuid();
        }
        #endregion
        public Guid GUID
        {
            get { return m_guid; }
        }

        #region fields
        Prop3D m_prop3d;
        int m_idd;
        GsaMaterial m_material = null;
        private Guid m_guid;
        #endregion

        #region constructors
        public GsaProp3d()
        {
            m_prop3d = null;
            m_guid = Guid.Empty;
            m_idd = 0;
        }
        public GsaProp3d(int id)
        {
            m_prop3d = null; 
            m_guid = Guid.Empty;
            m_idd = id;
        }
        public GsaProp3d(GsaMaterial material)
        {
            m_prop3d = new Prop3D();
            m_guid = Guid.Empty;
            m_idd = 0;
            this.Material = material;
        }


        public GsaProp3d Duplicate()
        {
            if (this == null) { return null; }
            GsaProp3d dup = new GsaProp3d();
            if (m_prop3d != null)
                dup.m_prop3d = m_prop3d;
            dup.m_idd = m_idd;
            if (m_material != null)
                dup.m_material = m_material.Duplicate();
            dup.m_guid = new Guid(m_guid.ToString());
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
            if (m_prop3d != null)
            {
                str = m_prop3d.MaterialType.ToString();
                str = Char.ToUpper(str[0]) + str.Substring(1).ToLower().Replace("_", " ");
            }
            string pa = (ID > 0) ? "PV" + ID + " " : ""; 
            return "GSA 3D Property " + ((ID > 0) ? pa : "") + ((m_prop3d == null) ? "" : str);
        }

        #endregion
    }

    /// <summary>
    /// GsaProp2d Goo wrapper class, makes sure GsaProp2d can be used in Grasshopper.
    /// </summary>
    public class GsaProp3dGoo : GH_Goo<GsaProp3d>
    {
        #region constructors
        public GsaProp3dGoo()
        {
            this.Value = new GsaProp3d();
        }
        public GsaProp3dGoo(GsaProp3d prop)
        {
            if (prop == null)
                prop = new GsaProp3d();
            this.Value = prop; //prop.Duplicate();
        }

        public override IGH_Goo Duplicate()
        {
            return DuplicateGsaProp2d();
        }
        public GsaProp3dGoo DuplicateGsaProp2d()
        {
            return new GsaProp3dGoo(Value == null ? new GsaProp3d() : Value); //Value.Duplicate());
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
                return "Null GSA Prop3d";
            else
                return Value.ToString();
        }
        public override string TypeName
        {
            get { return ("GSA 3D Property"); }
        }
        public override string TypeDescription
        {
            get { return ("GSA 3D Property"); }
        }


        #endregion

        #region casting methods
        public override bool CastTo<Q>(ref Q target)
        {
            // This function is called when Grasshopper needs to convert this 
            // instance of GsaProp2d into some other type Q.            

            if (typeof(Q).IsAssignableFrom(typeof(GsaProp3d)))
            {
                if (Value == null)
                    target = default;
                else
                    target = (Q)(object)Value.Duplicate();
                return true;
            }

            if (typeof(Q).IsAssignableFrom(typeof(Prop3D)))
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
            if (typeof(GsaProp3d).IsAssignableFrom(source.GetType()))
            {
                Value = (GsaProp3d)source;
                return true;
            }

            //Cast from GsaAPI Prop2d
            if (typeof(Prop3D).IsAssignableFrom(source.GetType()))
            {
                Value = new GsaProp3d();
                Value.API_Prop3d = (Prop3D)source;
                return true;
            }

            return false;
        }
        #endregion
    }

    /// <summary>
    /// This class provides a Parameter interface for the Data_GsaSection type.
    /// </summary>
    public class GsaProp3dParameter : GH_PersistentParam<GsaProp2dGoo>
    {
        public GsaProp3dParameter()
          : base(new GH_InstanceDescription("3D Property", "PV", "GSA 3D Property", GsaGH.Components.Ribbon.CategoryName.Name(), GsaGH.Components.Ribbon.SubCategoryName.Cat9()))
        {
        }

        public override Guid ComponentGuid => new Guid("277c96bb-8ea4-4d95-ab02-2954f14203f3");

        public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;

        protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.Prop3dParam;

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
