﻿using System;
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
using UnitsNet;
using UnitsNet.Units;
using Oasys.Units;

namespace GhSA.Parameters
{
    /// <summary>
    /// Section class, this class defines the basic properties and methods for any Gsa Section
    /// </summary>
    public class GsaSection
    {
        internal Section API_Section
        {
            get { return m_section; }
            set
            {
                m_guid = Guid.NewGuid();
                m_section = value;
                m_material = new GsaMaterial(this);
            }
        }
        #region section properties
        public Area Area
        {
            get 
            {
                Area area = new Area(m_section.Area, UnitsNet.UnitSystem.SI);
                return new Area(area.As(Units.SectionAreaUnit), Units.SectionAreaUnit); 
            }
        }
        public AreaMomentOfInertia Iyy
        {
            get 
            {
                AreaMomentOfInertia inertia = new AreaMomentOfInertia(m_section.Iyy, UnitsNet.UnitSystem.SI);
                return new AreaMomentOfInertia(inertia.As(Units.SectionAreaMomentOfInertiaUnit), Units.SectionAreaMomentOfInertiaUnit);
            }
        }
        public AreaMomentOfInertia Iyz
        {
            get
            {
                AreaMomentOfInertia inertia = new AreaMomentOfInertia(m_section.Iyz, UnitsNet.UnitSystem.SI);
                return new AreaMomentOfInertia(inertia.As(Units.SectionAreaMomentOfInertiaUnit), Units.SectionAreaMomentOfInertiaUnit);
            }
        }
        public AreaMomentOfInertia Izz
        {
            get
            {
                AreaMomentOfInertia inertia = new AreaMomentOfInertia(m_section.Izz, UnitsNet.UnitSystem.SI);
                return new AreaMomentOfInertia(inertia.As(Units.SectionAreaMomentOfInertiaUnit), Units.SectionAreaMomentOfInertiaUnit);
            }
        }
        public AreaMomentOfInertia J
        {
            get
            {
                AreaMomentOfInertia inertia = new AreaMomentOfInertia(m_section.J, UnitsNet.UnitSystem.SI);
                return new AreaMomentOfInertia(inertia.As(Units.SectionAreaMomentOfInertiaUnit), Units.SectionAreaMomentOfInertiaUnit);
            }
        }
        public double Ky
        {
            get { return m_section.Ky; }
        }
        public double Kz
        {
            get { return m_section.Kz; }
        }
        public IQuantity SurfaceAreaPerLength
        {
            get 
            {
                Area area = new Area(m_section.SurfaceAreaPerLength, UnitsNet.UnitSystem.SI);
                Length len = new Length(1, Units.LengthUnitSection);
                Area unitArea = len * len;
                Area areaOut = new Area(area.As(unitArea.Unit), unitArea.Unit);
                return areaOut / len; 
            }
        }
        public VolumePerLength VolumePerLength
        {
            get { return new VolumePerLength(m_section.VolumePerLength, UnitsNet.UnitSystem.SI); }
        }
        #endregion
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
                if (m_section == null)
                    m_section = new Section();
                else
                    CloneSection();
                m_section.MaterialType = Util.Gsa.ToGSA.Materials.ConvertType(m_material);
                m_section.MaterialAnalysisProperty = m_material.AnalysisProperty;
                m_section.MaterialGradeProperty = m_material.GradeProperty;
            }
        }
        #region GsaAPI members
        public string Name
        {
            get { return m_section.Name; }
            set
            {
                m_guid = Guid.NewGuid();
                CloneSection();
                m_section.Name = value;
            }
        }
        public int Pool
        {
            get { return m_section.Pool; }
            set
            {
                m_guid = Guid.NewGuid();
                CloneSection();
                m_section.Pool = value;
            }
        }
        public int MaterialID
        {
            get { return m_section.MaterialAnalysisProperty; }
            set
            {
                m_guid = Guid.NewGuid();
                CloneSection();
                m_section.MaterialAnalysisProperty = value;
                m_material.AnalysisProperty = m_section.MaterialAnalysisProperty;
            }
        }
        public string Profile
        {
            get { return m_section.Profile.Replace("%", " "); }
            set
            {
                m_guid = Guid.NewGuid();
                CloneSection();
                m_section.Profile = value;
            }
        }
        public System.Drawing.Color Colour
        {
            get { return (System.Drawing.Color)m_section.Colour; }
            set
            {
                m_guid = Guid.NewGuid();
                CloneSection();
                m_section.Colour = value;
            }
        }
        private void CloneSection()
        {
            if (m_section == null)
            {
                m_section = new Section();
                m_guid = Guid.NewGuid();
                return;
            }

            Section sec = new Section()
            {
                MaterialAnalysisProperty = m_section.MaterialAnalysisProperty,
                MaterialGradeProperty = m_section.MaterialGradeProperty,
                MaterialType = m_section.MaterialType,
                Name = m_section.Name.ToString(),
                Pool = m_section.Pool,
                Profile = m_section.Profile.ToString()
            };
            if ((System.Drawing.Color)m_section.Colour != System.Drawing.Color.FromArgb(0, 0, 0)) // workaround to handle that System.Drawing.Color is non-nullable type
                sec.Colour = m_section.Colour;
            
            m_section = sec;
            m_guid = Guid.NewGuid();
        }
        #endregion
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
            m_section = null;
            m_guid = Guid.Empty;
            m_idd = 0;
        }
        public GsaSection(int id)
        {
            m_section = null;
            m_guid = Guid.Empty;
            m_idd = id;
        }
        public GsaSection(string profile)
        {
            m_section = new Section
            {
                Profile = profile
            };
            m_material = new GsaMaterial();
            m_guid = Guid.NewGuid();
        }
        public GsaSection(string profile, int ID = 0)
        {
            m_section = new Section
            {
                Profile = profile
            };
            m_idd = ID;
            m_material = new GsaMaterial();
            m_guid = Guid.NewGuid();
        }
        public GsaSection Duplicate()
        {
            if (this == null) { return null; }
            GsaSection dup = new GsaSection();
            if (m_section != null)
                dup.m_section = m_section;
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

            //Cast from GsaAPI Prop2d
            if (typeof(Section).IsAssignableFrom(source.GetType()))
            {
                Value = new GsaSection();
                Value.API_Section = (Section)source;
                return true;
            }

            //Cast from string
            if (GH_Convert.ToString(source, out string name, GH_Conversion.Both))
            {
                Value = new GsaSection(name);
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

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.SectionParam;

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
