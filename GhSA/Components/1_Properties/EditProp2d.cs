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
    public class gsaProp2dEdit : GH_Component, IGH_PreviewObject
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("4cfdee19-451b-4ee3-878b-93a86767ffef");
        public gsaProp2dEdit()
          : base("Edit 2D Property", "Prop2dEdit", "Modify GSA 2D Property",
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
            
            pManager.AddGenericParameter("2D Property", "PA", "GSA 2D Property to get or set information for", GH_ParamAccess.item);
            pManager.AddTextParameter("Thickness", "Thk", "Set Property Thickness", GH_ParamAccess.item);
            pManager.AddGenericParameter("Material", "Mat", "Set Material Property", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Analysis Type", "Typ", "Set Material Analysis Type", GH_ParamAccess.item);
            pManager.AddTextParameter("Alignment", "Alig", "Set Surface Reference", GH_ParamAccess.item);
            pManager.AddGenericParameter("Offset", "Offs", "Set Offset", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Prop2d Number", "ID", "Set 2D Property Number. If ID is set it will replace any existing 2D Property in the model", GH_ParamAccess.item);
            pManager.AddTextParameter("Prop2d Name", "Name", "Set Name of 2D Proerty", GH_ParamAccess.item);
            pManager.AddColourParameter("Prop2d Colour", "Col", "Set 2D Property Colour", GH_ParamAccess.item);

            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
            pManager[6].Optional = true;
            pManager[7].Optional = true;
            pManager[8].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("2D Property", "PA", "GSA 2D Property with changes", GH_ParamAccess.item);
            pManager.AddTextParameter("Thickness", "Thk", "Property Thickness", GH_ParamAccess.item);
            pManager.AddGenericParameter("Material", "Mat", "Material Property", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Analysis Type", "Typ", "Material Analysis Type", GH_ParamAccess.item);
            pManager.AddTextParameter("Alignment", "Alig", "Surface Reference", GH_ParamAccess.item);
            pManager.AddGenericParameter("Offset", "Offs", "Offset", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Prop2d Number", "ID", "Original 2D Property number (ID) if 2D Property ever belonged to a GSA Model", GH_ParamAccess.item);
            pManager.AddTextParameter("Prop2d Name", "Name", "Name of 2D Proerty", GH_ParamAccess.item);
            pManager.AddColourParameter("Prop2d Colour", "Colour", "2D Property Colour", GH_ParamAccess.item);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GsaProp2d gsaProp2d = new GsaProp2d();
            if (DA.GetData(0, ref gsaProp2d))
            {
                GsaProp2d prop = new GsaProp2d();
                prop = gsaProp2d.Duplicate();

                // #### inputs ####
                // 1 thickness
                string thk = ""; //prop.Prop2d.Thickness;
                if (DA.GetData(1, ref thk))
                {
                    //prop.Prop2d.Thickness = thk;
                }

                // 2 Material
                // to be implemented
                
                // 3 analysis type
                int analtype = 0; //prop.Prop2d.Thickness;
                if (DA.GetData(3, ref analtype))
                {
                    prop.Prop2d.MaterialAnalysisProperty = analtype;
                }

                // 4 alignment
                string ali = "";
                if (DA.GetData(4, ref ali))
                {
                    // to be implement / GsaAPI can handle alignment / reference surface
                }

                // 5 offset
                GsaOffset offsetGSA = new GsaOffset();
                double offset = 0;
                if (DA.GetData(5, ref offsetGSA))
                {
                    //prop.Prop2d.Offeset = offsetGSA.Z;
                }
                else if (DA.GetData(5, ref offset))
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
                        prop.ID = id;
                    }
                }

                // 7 name
                GH_String ghnm = new GH_String();
                if (DA.GetData(7, ref ghnm))
                {
                    string name = "";
                    if (GH_Convert.ToString(ghnm, out name, GH_Conversion.Both))
                    {
                        prop.Prop2d.Name = name;
                    }
                }

                // 8 Colour
                GH_Colour ghcol = new GH_Colour();
                if (DA.GetData(8, ref ghcol))
                {
                    System.Drawing.Color col = new System.Drawing.Color();
                    if (GH_Convert.ToColor(ghcol, out col, GH_Conversion.Both))
                    {
                        prop.Prop2d.Colour = col;
                    }
                }

                //#### outputs ####
                DA.SetData(0, new GsaProp2dGoo(gsaProp2d));

                //DA.SetData(1, gsaProp2d.Thickness); // GsaAPI to be updated
                //DA.SetData(2, gsaProp2d.Prop2d.Material); // to be implemented
                DA.SetData(3, gsaProp2d.Prop2d.MaterialAnalysisProperty); // GsaAPI to be updated
                                                                          //DA.SetData(4, gsaProp2d.??); GsaAPI to include alignment / reference surface

                GsaOffset gsaoffset = new GsaOffset();
                //offset.Z = gsaProp2d.Prop2d.Offset; // GsaAPI to include prop2d offset
                DA.SetData(5, gsaoffset);

                DA.SetData(6, gsaProp2d.ID);
                DA.SetData(7, gsaProp2d.Prop2d.Name);
                DA.SetData(8, gsaProp2d.Prop2d.Colour);
            }
        }
    }
}

