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
            set { m_section = value; }
        }

        public int ID
        {
            get { return m_idd; }
            set { m_idd = value; }
        }

        //public GsaMaterial Material
        //{
        //    get { return m_material; }
        //    set { m_material = value; }
        //}

        #region fields
        Section m_section;
        int m_idd;
        //GsaMaterial m_material;
        #endregion

        #region constructors
        public GsaSection()
        {
            m_section = new Section();
            m_idd = 0;
        }
        public GsaSection(string profile)
        {
            m_section = new Section();
            m_section.Profile = profile;
            m_idd = 0;
        }
        public GsaSection(string profile, int ID)
        {
            m_section = new Section();
            m_section.Profile = profile;
            m_idd = ID;
        }

        public GsaSection Duplicate()
        {
            GsaSection dup = new GsaSection();
            dup.Section = m_section;
            dup.ID = m_idd;
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
            string str = m_section.Profile;
            return "GSA Section " + str;
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
            this.Value = section.Duplicate();
        }

        public override IGH_Goo Duplicate()
        {
            return DuplicateGsaSection();
        }
        public GsaSectionGoo DuplicateGsaSection()
        {
            return new GsaSectionGoo(Value == null ? new GsaSection() : Value.Duplicate());
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
                    target = default(Q);
                else
                    target = (Q)(object)Value;
                return true;
            }

            if (typeof(Q).IsAssignableFrom(typeof(Section)))
            {
                if (Value == null)
                    target = default(Q);
                else
                    target = (Q)(object)Value;
                return true;
            }

            target = default(Q);
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
            string name = "";
            if (GH_Convert.ToString(source, out name, GH_Conversion.Both))
            {
                Value.Section.Profile = name;
                return true;
            }

            //Cast from integer
            int idd = 0;
            if (GH_Convert.ToInt32(source, out idd, GH_Conversion.Both))
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
          : base(new GH_InstanceDescription("GSA Section", "Section", "GSA Section with profile", GhSA.Components.Ribbon.CategoryName.name(), GhSA.Components.Ribbon.SubCategoryName.cat9()))
        {
        }

        public override Guid ComponentGuid => new Guid("8500f335-fad7-46a0-b1be-bdad22ab1474");

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        //protected override Bitmap Icon => Resources.CrossSections;

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
            System.Windows.Forms.ToolStripMenuItem item = new System.Windows.Forms.ToolStripMenuItem();
            item.Text = "Not available";
            item.Visible = false;
            return item;
        }
        protected override System.Windows.Forms.ToolStripMenuItem Menu_CustomMultiValueItem()
        {
            System.Windows.Forms.ToolStripMenuItem item = new System.Windows.Forms.ToolStripMenuItem();
            item.Text = "Not available";
            item.Visible = false;
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
