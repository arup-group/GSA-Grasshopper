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
    public class gsaSectionEdit : GH_Component, IGH_PreviewObject
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("27dcadbd-4735-4110-8c30-931b37ec5f5a");
        public gsaSectionEdit()
          : base("Edit Section", "SectionEdit", "Modify GSA Section",
                Ribbon.CategoryName.name(),
                Ribbon.SubCategoryName.cat1())
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.tertiary;

        //protected override Bitmap Icon => Resources.CrossSections;
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
            pManager.AddIntegerParameter("Analysis Type", "Typ", "Set Material Analysis Type", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Section Pool", "Pool", "Set Section pool", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Section Offset", "Offs", "Set Section offset", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Section Number", "ID", "Set 2D Property Number. If ID is set it will replace any existing 2D Property in the model", GH_ParamAccess.item);
            pManager.AddTextParameter("Section Name", "Name", "Set Section name", GH_ParamAccess.item);
            pManager.AddColourParameter("Section Colour", "Col", "Set Section colour", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Section", "Sect", "GSA Section with changes", GH_ParamAccess.item);
            pManager.AddTextParameter("Section Profile", "Prfl", "Profile describtion", GH_ParamAccess.item);

            pManager.AddGenericParameter("Material", "Mat", "Material Property", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Analysis Type", "Typ", "Material Analysis Type", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Section Pool", "Pool", "Section pool (default none)", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Section Number", "ID", "Original Section number (ID) if Section ever belonged to a GSA Model", GH_ParamAccess.item);
            pManager.AddTextParameter("Section Name", "Name", "Section name (default profile name)", GH_ParamAccess.item);
            pManager.AddColourParameter("Section Colour", "Col", "Section colour (default none)", GH_ParamAccess.item);

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
                    // to be implemented

                    // 3 analysis type
                    int analtype = 0; //prop.Prop2d.Thickness;
                    if (DA.GetData(3, ref analtype))
                    {
                        gsaSection.Section.MaterialAnalysisProperty = analtype;
                    }

                    // 4 section pool
                    int pool = 0; //prop.Prop2d.Thickness;
                    if (DA.GetData(4, ref pool))
                    {
                        gsaSection.Section.Pool = pool;
                    }

                    // 5 offset
                    
                    int offset = 0;
                    if (DA.GetData(5, ref offset))
                    {
                        //prop.Prop2d.Offeset = offset;
                    }

                    // 6 ID
                    GH_Integer ghID = new GH_Integer();
                    if (DA.GetData(6, ref ghID))
                    {
                        int id = new int();
                        if (GH_Convert.ToInt32(ghID, out id, GH_Conversion.Both))
                        {
                            gsaSection.ID = id;
                        }
                    }

                    // 7 name
                    GH_String ghnm = new GH_String();
                    if (DA.GetData(7, ref ghnm))
                    {
                        string name = "";
                        if (GH_Convert.ToString(ghnm, out name, GH_Conversion.Both))
                        {
                            gsaSection.Section.Name = name;
                        }
                    }

                    // 8 Colour
                    GH_Colour ghcol = new GH_Colour();
                    if (DA.GetData(8, ref ghcol))
                    {
                        System.Drawing.Color col = new System.Drawing.Color();
                        if (GH_Convert.ToColor(ghcol, out col, GH_Conversion.Both))
                        {
                            gsaSection.Section.Colour = col;
                        }
                    }

                    // #### outputs ####
                    DA.SetData(0, new GsaSectionGoo(gsaSection));

                    DA.SetData(1, gsaSection.Section.Profile); 
                    //DA.SetData(2, gsaProp2d.Prop2d.Material); // to be implemented
                    DA.SetData(3, gsaSection.Section.MaterialAnalysisProperty);
                    DA.SetData(4, gsaSection.Section.Pool);
                    //DA.SetData(5, gsaSection.Section.Offset);
                    DA.SetData(6, gsaSection.ID);
                    DA.SetData(7, gsaSection.Section.Name);
                    DA.SetData(8, gsaSection.Section.Colour);

                }
            }
        }
    }
}

