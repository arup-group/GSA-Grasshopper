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
    /// Section class, this class defines the basic properties and methods for any Gsa Section
    /// </summary>
    public class GsaMaterial

    {
        public enum MatType
        {
            UNDEF = -2,
            NONE = -1,
            GENERIC = 0,
            STEEL = 1,
            CONCRETE = 2,
            ALUMINIUM = 3,
            GLASS = 4,
            FRP = 5,
            REBAR = 6,
            TIMBER = 7,
            FABRIC = 8,
            SOIL = 9,
            NUM_MT = 10,
            COMPOUND = 256,
            BAR = 4096,
            TENDON = 4352,
            FRPBAR = 4608,
            CFRP = 4864,
            GFRP = 5120,
            AFRP = 5376,
            ARGFRP = 5632,
            BARMAT = 65280
        }

        //public int ID
        //{
        //    get { return m_idd; }
        //    set { m_idd = value; }
        //}
        public int GradeProperty
        {
            get { return m_grade; }
            set { m_grade = value; }
        }
        public int AnalysisProperty
        {
            get { return m_analProp; }
            set { m_analProp = value; }
        }

        public MatType MaterialType;

        #region fields
        //int m_idd = 0;
        int m_grade = 1;
        int m_analProp = 0;
        
        #endregion

        #region constructors
        public GsaMaterial()
        {
            MaterialType = MatType.GENERIC;
        }
        private MatType getType(MaterialType materialType)
        {
            MatType m_type = MatType.NONE;
            if (materialType == GsaAPI.MaterialType.GENERIC)
                m_type = MatType.GENERIC;
            if (materialType == GsaAPI.MaterialType.STEEL)
                m_type = MatType.STEEL;
            if (materialType == GsaAPI.MaterialType.CONCRETE)
                m_type = MatType.CONCRETE;
            if (materialType == GsaAPI.MaterialType.TIMBER)
                m_type = MatType.TIMBER;
            if (materialType == GsaAPI.MaterialType.ALUMINIUM)
                m_type = MatType.ALUMINIUM;
            if (materialType == GsaAPI.MaterialType.FRP)
                m_type = MatType.FRP;
            if (materialType == GsaAPI.MaterialType.GLASS)
                m_type = MatType.GLASS;
            if (materialType == GsaAPI.MaterialType.FABRIC)
                m_type = MatType.FABRIC;
            return m_type;
        }
        /// <summary>
        /// 0 : Generic
        /// 1 : Steel
        /// 2 : Concrete
        /// 3 : Aluminium
        /// 4 : Glass
        /// 5 : FRP
        /// 7 : Timber
        /// 8 : Fabric
        /// </summary>
        /// <param name="material_id"></param>
        public GsaMaterial (int material_id)
        {
            MaterialType = (MatType)material_id;
        }
        public GsaMaterial(GsaSection section)
        {
            if (section == null) { return; }
            if (section.API_Section == null) { return; }

            MaterialType = getType(section.API_Section.MaterialType);
            AnalysisProperty = section.API_Section.MaterialAnalysisProperty;
            GradeProperty = section.API_Section.MaterialGradeProperty;
        }
        public GsaMaterial(GsaProp2d prop)
        {
            if (prop.Material == null) { return;  }

            MaterialType = getType(prop.API_Prop2d.MaterialType);
            AnalysisProperty = prop.API_Prop2d.MaterialAnalysisProperty;
            GradeProperty = prop.API_Prop2d.MaterialGradeProperty;
        }
        public GsaMaterial(Prop2D prop)
        {
            if (prop == null) { return; }

            MaterialType = getType(prop.MaterialType);
            AnalysisProperty = prop.MaterialAnalysisProperty;
            GradeProperty = prop.MaterialGradeProperty;
        }

        public GsaMaterial Duplicate()
        {
            if (this == null) { return null; }
            GsaMaterial dup = new GsaMaterial();
            dup.MaterialType = MaterialType;
            //dup.ID = m_idd;
            dup.GradeProperty = m_grade;
            dup.AnalysisProperty = m_analProp;
            
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
            string mate = MaterialType.ToString();
            mate = Char.ToUpper(mate[0]) + mate.Substring(1).ToLower().Replace("_", " ");
            
            return "GSA Material " + mate;
        }

        #endregion
    }

    /// <summary>
    /// GsaSection Goo wrapper class, makes sure GsaSection can be used in Grasshopper.
    /// </summary>
    public class GsaMaterialGoo : GH_Goo<GsaMaterial>
    {
        #region constructors
        public GsaMaterialGoo()
        {
            this.Value = new GsaMaterial();
        }
        public GsaMaterialGoo(GsaMaterial material)
        {
            if (material == null)
                material = new GsaMaterial();
            this.Value = material; //material.Duplicate();
        }

        public override IGH_Goo Duplicate()
        {
            return DuplicateGsaSection();
        }
        public GsaMaterialGoo DuplicateGsaSection()
        {
            return new GsaMaterialGoo(Value == null ? new GsaMaterial() : Value); //Value.Duplicate());
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
                return "Null GSA Material";
            else
                return Value.ToString();
        }
        public override string TypeName
        {
            get { return ("GSA Material"); }
        }
        public override string TypeDescription
        {
            get { return ("GSA Material"); }
        }


        #endregion

        #region casting methods
        public override bool CastTo<Q>(ref Q target)
        {
            // This function is called when Grasshopper needs to convert this 
            // instance of GsaMaterial into some other type Q.            


            if (typeof(Q).IsAssignableFrom(typeof(GsaMaterial)))
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
            // into GsaMaterial.

            if (source == null) { return false; }

            //Cast from GsaMaterial
            if (typeof(GsaMaterial).IsAssignableFrom(source.GetType()))
            {
                Value = (GsaMaterial)source;
                return true;
            }

            //Cast from string
            if (GH_Convert.ToString(source, out string mat, GH_Conversion.Both))
            {
                if (mat.ToUpper() == "STEEL")
                {
                    Value.MaterialType = GsaMaterial.MatType.STEEL;
                    return true;
                }
                    
                if (mat.ToUpper() == "CONCRETE")
                {
                    Value.MaterialType = GsaMaterial.MatType.CONCRETE;
                    return true;
                }
                    
                if (mat.ToUpper() == "FRP")
                {
                    Value.MaterialType = GsaMaterial.MatType.FRP;
                    return true;
                }
                    
                if (mat.ToUpper() == "ALUMINIUM")
                {
                    Value.MaterialType = GsaMaterial.MatType.ALUMINIUM;
                    return true;
                }
                    
                if (mat.ToUpper() == "TIMBER")
                {
                    Value.MaterialType = GsaMaterial.MatType.TIMBER;
                    return true;
                }
                    
                if (mat.ToUpper() == "GLASS")
                {
                    Value.MaterialType = GsaMaterial.MatType.GLASS;
                    return true;
                }
                    
                if (mat.ToUpper() == "FABRIC")
                {
                    Value.MaterialType = GsaMaterial.MatType.FABRIC;
                    return true;
                }
                    
                if (mat.ToUpper() == "GENERIC")
                {
                    Value.MaterialType = GsaMaterial.MatType.GENERIC;
                    return true;
                }
                    
                return false;
            }

            //Cast from integer
            if (GH_Convert.ToInt32(source, out int idd, GH_Conversion.Both))
            {
                Value.AnalysisProperty = idd;
            }
            return false;
        }
        #endregion
    }

    /// <summary>
    /// This class provides a Parameter interface for the Data_GsaSection type.
    /// </summary>
    public class GsaMaterialParameter : GH_PersistentParam<GsaMaterialGoo>
    {
        public GsaMaterialParameter()
          : base(new GH_InstanceDescription("Material", "Ma", "GSA Material", GhSA.Components.Ribbon.CategoryName.Name(), GhSA.Components.Ribbon.SubCategoryName.Cat9()))
        {
        }

        public override Guid ComponentGuid => new Guid("f13d079b-f7d1-4d8a-be7c-3b7e1e59c5ab");

        public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.GsaMaterial;

        protected override GH_GetterResult Prompt_Plural(ref List<GsaMaterialGoo> values)
        {
            return GH_GetterResult.cancel;
        }
        protected override GH_GetterResult Prompt_Singular(ref GsaMaterialGoo value)
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
