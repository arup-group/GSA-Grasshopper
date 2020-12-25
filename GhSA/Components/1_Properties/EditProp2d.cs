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
    /// Component to edit a Prop2d and ouput the information
    /// </summary>
    public class EditProp2d : GH_Component, IGH_PreviewObject
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("4cfdee19-451b-4ee3-878b-93a86767ffef");
        public EditProp2d()
          : base("Edit 2D Property", "Prop2dEdit", "Modify GSA 2D Property",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat1())
        { this.Hidden = true; } // sets the initial state of the component to hidden
        public override GH_Exposure Exposure => GH_Exposure.tertiary;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.EditProp2D;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout


        #endregion

        #region Input and output

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            
            pManager.AddGenericParameter("2D Property", "PA", "GSA 2D Property to get or set information for", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Prop2d Number", "ID", "Set 2D Property Number. If ID is set it will replace any existing 2D Property in the model", GH_ParamAccess.item);
            pManager.AddTextParameter("Prop2d Name", "Na", "Set Name of 2D Proerty", GH_ParamAccess.item);
            pManager.AddColourParameter("Prop2d Colour", "Col", "Set 2D Property Colour", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Axis", "Ax", "Set Axis as integer: Global (0) or Topological (1)", GH_ParamAccess.item);
            pManager.AddGenericParameter("Material", "Mat", "Set Material Property", GH_ParamAccess.item);
            pManager.AddTextParameter("Thickness", "Thk", "Set Property Thickness", GH_ParamAccess.item);

            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
            pManager[6].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("2D Property", "PA", "GSA 2D Property to get or set information for", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Prop2d Number", "ID", " 2D Property Number. If ID is set it will replace any existing 2D Property in the model", GH_ParamAccess.item);
            pManager.AddTextParameter("Prop2d Name", "Na", "Name of 2D Proerty", GH_ParamAccess.item);
            pManager.AddColourParameter("Prop2d Colour", "Col", "2D Property Colour", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Axis", "Ax", "Axis as integer: Global (0) or Topological (1)", GH_ParamAccess.item);
            pManager.AddGenericParameter("Material", "Mat", "Material Property", GH_ParamAccess.item);
            pManager.AddTextParameter("Thickness", "Thk", "Property Thickness", GH_ParamAccess.item);
            pManager.AddTextParameter("Type", "Typ", "2D Property Type", GH_ParamAccess.item);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GsaProp2d gsaProp2d = new GsaProp2d();
            if (DA.GetData(0, ref gsaProp2d))
            {
                GsaProp2d prop = gsaProp2d.Duplicate();

                // #### inputs ####
                // 1 ID
                GH_Integer ghID = new GH_Integer();
                if (DA.GetData(1, ref ghID))
                {
                    if (GH_Convert.ToInt32(ghID, out int id, GH_Conversion.Both))
                        prop.ID = id;
                }

                // 2 name
                GH_String ghnm = new GH_String();
                if (DA.GetData(2, ref ghnm))
                {
                    if (GH_Convert.ToString(ghnm, out string name, GH_Conversion.Both))
                        prop.Prop2d.Name = name;
                }

                // 3 Colour
                GH_Colour ghcol = new GH_Colour();
                if (DA.GetData(3, ref ghcol))
                {
                    if (GH_Convert.ToColor(ghcol, out System.Drawing.Color col, GH_Conversion.Both))
                        prop.Prop2d.Colour = col;
                }

                // 4 Axis
                GH_Integer ghax = new GH_Integer();
                if (DA.GetData(4, ref ghax))
                {
                    if (GH_Convert.ToInt32(ghax, out int axis, GH_Conversion.Both))
                    {
                        axis = Math.Min(1, axis);
                        axis = Math.Max(0, axis);
                        prop.Prop2d.AxisProperty = axis;
                    }
                }

                // 5 Material
                // to include GsaMaterial when this becomes available in GsaAPI
                GH_Integer gh_mat = new GH_Integer();
                if (DA.GetData(5, ref gh_mat))
                {
                    if (GH_Convert.ToInt32(gh_mat, out int mat, GH_Conversion.Both))
                        prop.Prop2d.MaterialAnalysisProperty = mat;
                }

                // 6 thickness
                string thk = ""; //prop.Prop2d.Thickness;
                if (DA.GetData(6, ref thk))
                {
                    //prop.Prop2d.Thickness = thk;
                }

                

                //#### outputs ####
                DA.SetData(0, new GsaProp2dGoo(prop));
                DA.SetData(1, prop.ID);
                DA.SetData(2, prop.Prop2d.Name);
                DA.SetData(3, prop.Prop2d.Colour);
                DA.SetData(4, gsaProp2d.Prop2d.AxisProperty);
                DA.SetData(5, gsaProp2d.Prop2d.MaterialAnalysisProperty);
                //DA.SetData(6, gsaProp2d.Thickness); // GsaAPI to be updated

                string str = gsaProp2d.Prop2d.Type.ToString();
                str = Char.ToUpper(str[0]) + str.Substring(1).ToLower().Replace("_", " ");
                DA.SetData(7, str);

            }
        }
    }
}

