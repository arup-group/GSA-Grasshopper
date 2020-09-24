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
    public class gsaSection : GH_Component
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("1167c4aa-b98b-47a7-ae85-1a3c976a1973");
        public gsaSection()
          : base("Create Section", "Section", "Create GSA Section",
                Ribbon.CategoryName.name(),
                Ribbon.SubCategoryName.cat1())
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        //protected override Bitmap Icon => Resources.CrossSections;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout

        #endregion

        #region Input and output

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Profile", "Prfl", "Cross-Section Profile", GH_ParamAccess.item);
            pManager.AddTextParameter("Material", "Mat", "Section Material", GH_ParamAccess.item);
            pManager.AddNumberParameter("Pool", "Pool", "Section Pool", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("Number", "ID", "Section number PB# (default appended to model = 0). Will overwrite any existing section with same number", GH_ParamAccess.item, 0);
            pManager.AddTextParameter("Name", "Na", "Section name", GH_ParamAccess.item);
            pManager.AddColourParameter("Colour", "Col", "Section colour)", GH_ParamAccess.item);
            
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
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
                string profile = "";
                if (GH_Convert.ToString(gh_profile, out profile, GH_Conversion.Both))
                {
                    
                    sect.Section.Profile = profile;

                    // 1 material
                    // to be implemented

                    // 2 pool
                    GH_Integer gh_pool = new GH_Integer();
                    if (DA.GetData(2, ref gh_pool))
                    {
                        int pool = 0;
                        if (GH_Convert.ToInt32(gh_pool, out pool, GH_Conversion.Both))
                            sect.Section.Pool = pool;
                    }

                    // 3 ID
                    GH_Integer gh_id = new GH_Integer();
                    if (DA.GetData(3, ref gh_id))
                    {
                        int idd = 0;
                        if (GH_Convert.ToInt32(gh_id, out idd, GH_Conversion.Both))
                            sect.ID = idd;
                    }

                    // 4 name
                    GH_String gh_n = new GH_String();
                    if (DA.GetData(4, ref gh_n))
                    {
                        string name = "";
                        if (GH_Convert.ToString(gh_n, out name, GH_Conversion.Both))
                            sect.Section.Name = name;
                    }

                    // 5 colour
                    GH_Colour gh_Colour = new GH_Colour();
                    if (DA.GetData(5, ref gh_Colour))
                    {
                        System.Drawing.Color colour = new System.Drawing.Color();
                        if (GH_Convert.ToColor(gh_Colour, out colour, GH_Conversion.Both))
                            sect.Section.Colour = colour;
                    }
                }
                DA.SetData(0, new GsaSectionGoo(sect));
            }
        }
    }
}

