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
    public class GsaSection
    {
        public Section Section
        {
            get { return m_section; }
            set 
            {
                m_guid = Guid.NewGuid();
                m_section = value; 
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
                m_guid = Guid.NewGuid();
                m_material = value;
                m_section.MaterialType = Util.Gsa.ToGSA.Materials.ConvertType(m_material);
                m_section.MaterialAnalysisProperty = m_material.AnalysisProperty;
                m_section.MaterialGradeProperty = m_material.Grade;
            }
        }
        public Guid GUID
        {
            get { return m_guid; }
        }

        #region fields
        Section m_section;
        int m_idd = 0;
        GsaMaterial m_material; //to be added when GsaAPI supports materials
        private Guid m_guid;
        #endregion

        #region constructors
        public GsaSection()
        {
            m_section = new Section();
            m_guid = Guid.NewGuid();
        }
        public GsaSection(string profile)
        {
            m_section = new Section
            {
                Profile = profile
            };
            m_guid = Guid.NewGuid();
        }
        public GsaSection(string profile, int ID)
        {
            m_section = new Section
            {
                Profile = profile
            };
            m_idd = ID;
            m_guid = Guid.NewGuid();
        }

        public GsaSection Duplicate()
        {
            if (this == null) { return null; }
            GsaSection dup = new GsaSection();
            if (m_section != null)
            {
                dup.Section = new Section()
                {
                    MaterialAnalysisProperty = m_section.MaterialAnalysisProperty,
                    MaterialGradeProperty = m_section.MaterialGradeProperty,
                    MaterialType = m_section.MaterialType,
                    Name = m_section.Name,
                    Pool = m_section.Pool,
                    Profile = m_section.Profile
                };

                if ((System.Drawing.Color)m_section.Colour != System.Drawing.Color.FromArgb(0, 0, 0)) // workaround to handle that System.Drawing.Color is non-nullable type
                    dup.m_section.Colour = m_section.Colour;
            }
            else
                dup.Section = null;
            
            dup.ID = m_idd;

            if (Material != null)
                dup.Material = m_material.Duplicate();

            dup.m_guid = new Guid(m_guid.ToString());
            return dup;
        }

        public GsaSection Clone()
        {
            GsaSection clone = this.Duplicate();
            clone.m_guid = Guid.NewGuid();
            return clone;
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
            string prof = "";
            if (m_section != null)
                prof = m_section.Profile;
            string pb = "PB" + ID + " ";
            return "GSA Section " + ((ID > 0) ? pb : "") + ((m_section == null) ? "" : prof.Replace("%", " "));
        }

        #endregion
    }

    /// <summary>
    /// GsaSection Goo wrapper class, makes sure GsaSection can be used in Grasshopper.
    /// </summary>
    public class GsaSectionGoo : GH_Goo<GsaSection>
    {
        #region constructors
        public GsaSectionGoo()
        {
            this.Value = new GsaSection();
        }
        public GsaSectionGoo(GsaSection section)
        {
            if (section == null)
                section = new GsaSection();
            this.Value = section; //section.Duplicate();
        }

        public override IGH_Goo Duplicate()
        {
            return DuplicateGsaSection();
        }
        public GsaSectionGoo DuplicateGsaSection()
        {
            return new GsaSectionGoo(Value == null ? new GsaSection() : Value); //Value.Duplicate());
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
                return "Null GSA Section";
            else
                return Value.ToString();
        }
        public override string TypeName
        {
            get { return ("GSA Section"); }
        }
        public override string TypeDescription
        {
            get { return ("GSA Section"); }
        }


        #endregion

        #region casting methods
        public override bool CastTo<Q>(ref Q target)
        {
            // This function is called when Grasshopper needs to convert this 
            // instance of GsaSection into some other type Q.            


            if (typeof(Q).IsAssignableFrom(typeof(GsaSection)))
            {
                if (Value == null)
                    target = default;
                else
                    target = (Q)(object)Value;
                return true;
            }

            if (typeof(Q).IsAssignableFrom(typeof(Section)))
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
            // into GsaSection.


            if (source == null) { return false; }

            //Cast from GsaSection
            if (typeof(GsaSection).IsAssignableFrom(source.GetType()))
            {
                Value = (GsaSection)source;
                return true;
            }


            //Cast from string
            if (GH_Convert.ToString(source, out string name, GH_Conversion.Both))
            {
                Value.Section.Profile = name;
                return true;
            }

            //Cast from integer
            if (GH_Convert.ToInt32(source, out int idd, GH_Conversion.Both))
            {
                Value.ID = idd;
            }
            return false;
        }
        #endregion


    }

    /// <summary>
    /// This class provides a Parameter interface for the Data_GsaSection type.
    /// </summary>
    public class GsaSectionParameter : GH_PersistentParam<GsaSectionGoo>
    {
        public GsaSectionParameter()
          : base(new GH_InstanceDescription("Section", "PB", "GSA Section", GhSA.Components.Ribbon.CategoryName.Name(), GhSA.Components.Ribbon.SubCategoryName.Cat9()))
        {
        }

        public override Guid ComponentGuid => new Guid("8500f335-fad7-46a0-b1be-bdad22ab1474");

        public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.GsaSection;

        protected override GH_GetterResult Prompt_Plural(ref List<GsaSectionGoo> values)
        {
            return GH_GetterResult.cancel;
        }
        protected override GH_GetterResult Prompt_Singular(ref GsaSectionGoo value)
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
