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
    /// Component to create a new Section
    /// </summary>
    public class CreateSection : GH_Component
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("1167c4aa-b98b-47a7-ae85-1a3c976a1973");
        public CreateSection()
          : base("Create Section", "Section", "Create GSA Section",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat1())
        { this.Hidden = true; } // sets the initial state of the component to hidden
        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.CreateSection;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout

        #endregion

        #region Input and output

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Profile", "Pf", "Cross-Section Profile", GH_ParamAccess.item);
            pManager.AddNumberParameter("Number", "ID", "Section number PB# (default appended to model = 0). Will overwrite any existing section with same number", GH_ParamAccess.item, 0);
            pManager.AddGenericParameter("Material", "Ma", "Section Material or Reference ID for Material Property in Existing GSA Model", GH_ParamAccess.item);
            pManager.AddNumberParameter("Pool", "Po", "Section Pool", GH_ParamAccess.item, 0);
            pManager.AddTextParameter("Name", "Na", "Section name", GH_ParamAccess.item);
            pManager.AddColourParameter("Colour", "Co", "Section colour)", GH_ParamAccess.item);

            for (int i = 1; i < pManager.ParamCount; i++)
                pManager[i].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Section", "PB", "GSA Section", GH_ParamAccess.item);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GsaSection sect = new GsaSection();

            //profile
            GH_String gh_profile = new GH_String();
            if (DA.GetData(0, ref gh_profile))
            {
                if (GH_Convert.ToString(gh_profile, out string profile, GH_Conversion.Both))
                {
                    sect.Section = new Section();
                    sect.Section.Profile = profile;

                    // 1 ID
                    GH_Integer gh_id = new GH_Integer();
                    if (DA.GetData(1, ref gh_id))
                    {
                        if (GH_Convert.ToInt32(gh_id, out int idd, GH_Conversion.Both))
                            sect.ID = idd;
                    }

                    // 2 material
                    // to include GsaMaterial when this becomes available in GsaAPI
                    GH_Integer gh_mat = new GH_Integer();
                    if (DA.GetData(21, ref gh_mat))
                    {
                        if (GH_Convert.ToInt32(gh_mat, out int mat, GH_Conversion.Both))
                            sect.Section.MaterialAnalysisProperty = mat;
                    }

                    // 3 pool
                    GH_Integer gh_pool = new GH_Integer();
                    if (DA.GetData(3, ref gh_pool))
                    {
                        if (GH_Convert.ToInt32(gh_pool, out int pool, GH_Conversion.Both))
                            sect.Section.Pool = pool;
                    }

                    // 4 name
                    GH_String gh_n = new GH_String();
                    if (DA.GetData(4, ref gh_n))
                    {
                        if (GH_Convert.ToString(gh_n, out string name, GH_Conversion.Both))
                            sect.Section.Name = name;
                    }

                    // 5 colour
                    GH_Colour gh_Colour = new GH_Colour();
                    if (DA.GetData(5, ref gh_Colour))
                    {
                        if (GH_Convert.ToColor(gh_Colour, out System.Drawing.Color colour, GH_Conversion.Both))
                            sect.Section.Colour = colour;
                    }
                }
                DA.SetData(0, new GsaSectionGoo(sect));
            }
        }
    }
}

