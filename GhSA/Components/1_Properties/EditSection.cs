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
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.tertiary;

        protected override System.Drawing.Bitmap Icon => GSA.Properties.Resources.EditSection;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout


        #endregion

        #region Input and output

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Section", "PB", "GSA Section to get or set information for", GH_ParamAccess.item);
            pManager.AddTextParameter("Section Profile", "Prfl", "Profile name following GSA naming convetion (eg 'STD I 1000 500 15 25')", GH_ParamAccess.item);

            pManager.AddGenericParameter("Material", "Mat", "Set Material Property", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Section Pool", "Pool", "Set Section pool", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Section Number", "ID", "Set 2D Property Number. If ID is set it will replace any existing 2D Property in the model", GH_ParamAccess.item);
            pManager.AddTextParameter("Section Name", "Name", "Set Section name", GH_ParamAccess.item);
            pManager.AddColourParameter("Section Colour", "Col", "Set Section colour", GH_ParamAccess.item);

            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
            pManager[6].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Section", "PB", "GSA Section with changes", GH_ParamAccess.item);
            pManager.AddTextParameter("Section Profile", "Prfl", "Profile describtion", GH_ParamAccess.item);

            pManager.AddGenericParameter("Material", "Mat", "Section Material or Reference ID for Material Property in Existing GSA Model", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Section Pool", "Pool", "Section pool", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Section Number", "ID", "Original Section number (ID) if Section ever belonged to a GSA Model", GH_ParamAccess.item);
            pManager.AddTextParameter("Section Name", "Name", "Section name", GH_ParamAccess.item);
            pManager.AddColourParameter("Section Colour", "Col", "Section colour", GH_ParamAccess.item);

        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GsaSection gsaSection = new GsaSection();
            if (DA.GetData(0, ref gsaSection))
            {
                if (gsaSection != null)
                {
                    // #### input ####
                    // 1 profile
                    string profile = "";
                    if (DA.GetData(1, ref profile))
                        gsaSection.Section.Profile = profile;

                    // 2 Material
                    // to include GsaMaterial when this becomes available in GsaAPI
                    GH_Integer gh_mat = new GH_Integer();
                    if (DA.GetData(2, ref gh_mat))
                    {
                        if (GH_Convert.ToInt32(gh_mat, out int mat, GH_Conversion.Both))
                            gsaSection.Section.MaterialAnalysisProperty = mat;
                    }

                    // 3 section pool
                    int pool = 0; //prop.Prop2d.Thickness;
                    if (DA.GetData(3, ref pool))
                    {
                        gsaSection.Section.Pool = pool;
                    }

                    // 4 ID
                    GH_Integer ghID = new GH_Integer();
                    if (DA.GetData(4, ref ghID))
                    {
                        if (GH_Convert.ToInt32(ghID, out int id, GH_Conversion.Both))
                            gsaSection.ID = id;
                    }

                    // 5 name
                    GH_String ghnm = new GH_String();
                    if (DA.GetData(5, ref ghnm))
                    {
                        if (GH_Convert.ToString(ghnm, out string name, GH_Conversion.Both))
                            gsaSection.Section.Name = name;
                    }

                    // 6 Colour
                    GH_Colour ghcol = new GH_Colour();
                    if (DA.GetData(6, ref ghcol))
                    {
                        if (GH_Convert.ToColor(ghcol, out System.Drawing.Color col, GH_Conversion.Both))
                            gsaSection.Section.Colour = col;
                    }

                    // #### outputs ####
                    DA.SetData(0, new GsaSectionGoo(gsaSection));

                    DA.SetData(1, gsaSection.Section.Profile.Replace("%", " ")); 
                    DA.SetData(2, gsaSection.Section.MaterialAnalysisProperty); // to implemented GsaMaterial
                    DA.SetData(3, gsaSection.Section.Pool);
                    DA.SetData(4, gsaSection.ID);
                    DA.SetData(5, gsaSection.Section.Name);
                    DA.SetData(6, gsaSection.Section.Colour);

                }
            }
        }
    }
}

