using System;
using System.Collections.Generic;
using Grasshopper.Kernel.Attributes;
using Grasshopper.GUI.Canvas;
using Grasshopper.GUI;
using Grasshopper.Kernel;
using Grasshopper;
using Rhino.Geometry;
using System.Windows.Forms;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GhSA.Parameters;
using System.Resources;

namespace GhSA.Components
{
    /// <summary>
    /// Component to edit a Section and ouput the information
    /// </summary>
    public class EditSection : GH_Component, IGH_PreviewObject
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("27dcadbd-4735-4110-8c30-931b37ec5f5a");
        public EditSection()
          : base("Edit Section", "SectionEdit", "Modify GSA Section",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat1())
        { this.Hidden = true; } // sets the initial state of the component to hidden
        public override GH_Exposure Exposure => GH_Exposure.tertiary;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.EditSection;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout


        #endregion

        #region Input and output

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Section", "PB", "GSA Section to get or set information for", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Section Number", "ID", "Set 2D Property Number. If ID is set it will replace any existing 2D Property in the model", GH_ParamAccess.item);
            pManager.AddTextParameter("Section Profile", "Pf", "Profile name following GSA naming convetion (eg 'STD I 1000 500 15 25')", GH_ParamAccess.item);
            pManager.AddGenericParameter("Material", "Ma", "Set GSA Material or reference existing material by ID", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Section Pool", "Po", "Set Section pool", GH_ParamAccess.item);
            pManager.AddTextParameter("Section Name", "Na", "Set Section name", GH_ParamAccess.item);
            pManager.AddColourParameter("Section Colour", "Co", "Set Section colour", GH_ParamAccess.item);

            for (int i = 0; i < pManager.ParamCount; i++)
                pManager[i].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Section", "PB", "GSA Section with changes", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Section Number", "ID", "Original Section number (ID) if Section ever belonged to a GSA Model", GH_ParamAccess.item);
            pManager.AddTextParameter("Section Profile", "Pf", "Profile describtion", GH_ParamAccess.item);
            pManager.AddGenericParameter("Material", "Ma", "GSA Material", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Section Pool", "Po", "Section pool", GH_ParamAccess.item);
            pManager.AddTextParameter("Section Name", "Na", "Section name", GH_ParamAccess.item);
            pManager.AddColourParameter("Section Colour", "Co", "Section colour", GH_ParamAccess.item);

        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GsaSection sect = new GsaSection();
            GsaSection gsaSection = new GsaSection();
            if (DA.GetData(0, ref sect))
            {
                gsaSection = sect.Duplicate();
            }

            if (gsaSection != null)
            {
                // #### input ####

                // 1 ID
                GH_Integer ghID = new GH_Integer();
                if (DA.GetData(1, ref ghID))
                {
                    if (GH_Convert.ToInt32(ghID, out int id, GH_Conversion.Both))
                        gsaSection.ID = id;
                }

                // 2 profile
                string profile = "";
                if (DA.GetData(2, ref profile))
                    gsaSection.Profile = profile;

                // 3 Material
                GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
                if (DA.GetData(3, ref gh_typ))
                {
                    GsaMaterial material = new GsaMaterial();
                    if (gh_typ.Value is GsaMaterialGoo)
                    {
                        gh_typ.CastTo(ref material);
                        gsaSection.Material = material;
                    }
                    else
                    {
                        if (GH_Convert.ToInt32(gh_typ.Value, out int idd, GH_Conversion.Both))
                            gsaSection.MaterialID = idd;
                        else
                        {
                            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert PB input to a Section Property of reference integer");
                            return;
                        }
                    }
                }

                // 4 section pool
                int pool = 0; //prop.Prop2d.Thickness;
                if (DA.GetData(4, ref pool))
                {
                    gsaSection.Pool = pool;
                }

                // 5 name
                GH_String ghnm = new GH_String();
                if (DA.GetData(5, ref ghnm))
                {
                    if (GH_Convert.ToString(ghnm, out string name, GH_Conversion.Both))
                        gsaSection.Name = name;
                }

                // 6 Colour
                GH_Colour ghcol = new GH_Colour();
                if (DA.GetData(6, ref ghcol))
                {
                    if (GH_Convert.ToColor(ghcol, out System.Drawing.Color col, GH_Conversion.Both))
                        gsaSection.Colour = col;
                }

                // #### outputs ####
                string prof = (gsaSection.Section == null) ? "--" : gsaSection.Profile;
                int poo = (gsaSection.Section == null) ? 0 : gsaSection.Pool;
                string nm = (gsaSection.Section == null) ? "--" : gsaSection.Name;
                ValueType colour = (gsaSection.Section == null) ? null : gsaSection.Section.Colour;

                DA.SetData(0, new GsaSectionGoo(gsaSection));
                DA.SetData(1, gsaSection.ID);
                DA.SetData(2, prof);
                DA.SetData(3, new GsaMaterialGoo(new GsaMaterial(gsaSection))); // to implemented GsaMaterial
                DA.SetData(4, poo);
                DA.SetData(5, nm);
                DA.SetData(6, colour);

            }
        }
    }
}

