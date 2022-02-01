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
    /// Component to edit a Material and ouput the information
    /// </summary>
    public class EditMaterial : GH_Component, IGH_PreviewObject
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("865f73c7-a057-481a-834b-c7e12873dd39");
        public EditMaterial()
          : base("Edit Material", "MaterialEdit", "Modify GSA Material",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat1())
        { this.Hidden = true; } // sets the initial state of the component to hidden
        public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.obscure;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.EditMaterial;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout


        #endregion

        #region Input and output

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            
            pManager.AddGenericParameter("Material", "Ma", "GSA Material to get or set information for", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Analysis Property", "An", "Set Material Analysis Property Number (0 -> 'from Grade'", GH_ParamAccess.item);
            pManager.AddTextParameter("Material Type", "mT", "Set Material Type" + System.Environment.NewLine +
                "Input either text string or integer:"
                + System.Environment.NewLine + "Generic : 0"
                + System.Environment.NewLine + "Steel : 1"
                + System.Environment.NewLine + "Concrete : 2"
                + System.Environment.NewLine + "Aluminium : 3"
                + System.Environment.NewLine + "Glass : 4"
                + System.Environment.NewLine + "FRP : 5"
                + System.Environment.NewLine + "Timber : 7"
                + System.Environment.NewLine + "Fabric : 8", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Material Grade", "Gr", "Set Material Grade", GH_ParamAccess.item);

            for (int i = 0; i < pManager.ParamCount; i++)
                pManager[i].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Material", "Ma", "GSA Material with changes", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Analysis Property", "An", "Get Material Analysis Property (0 -> 'from Grade')", GH_ParamAccess.item);
            pManager.AddTextParameter("Material Type", "mT", "Get Material Type", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Material Grade", "Gr", "Get Material Grade", GH_ParamAccess.item);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GsaMaterial gsaMaterial = new GsaMaterial();
            GsaMaterial material = new GsaMaterial();
            if (DA.GetData(0, ref gsaMaterial))
            {
                material = gsaMaterial.Duplicate();
            }

            // #### inputs ####
            // 1 Analysis Property
            GH_Integer ghID = new GH_Integer();
            if (DA.GetData(1, ref ghID))
            {
                if (GH_Convert.ToInt32(ghID, out int id, GH_Conversion.Both))
                    material.AnalysisProperty = id;
            }

            // 2 Material type
            MaterialType matType = MaterialType.CONCRETE;
            GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
            if (DA.GetData(2, ref gh_typ))
            {
                if (gh_typ.Value is MaterialType)
                    gh_typ.CastTo(ref matType);
                if (gh_typ.Value is GH_Integer)
                {
                    int typ = 2;
                    GH_Convert.ToInt32(gh_typ, out typ, GH_Conversion.Both);
                    if (typ == 1)
                        material.MaterialType = GsaMaterial.MatType.STEEL;
                    if (typ == 2)
                        material.MaterialType = GsaMaterial.MatType.CONCRETE;
                    if (typ == 5)
                        material.MaterialType = GsaMaterial.MatType.FRP;
                    if (typ == 3)
                        material.MaterialType = GsaMaterial.MatType.ALUMINIUM;
                    if (typ == 7)
                        material.MaterialType = GsaMaterial.MatType.TIMBER;
                    if (typ == 4)
                        material.MaterialType = GsaMaterial.MatType.GLASS;
                    if (typ == 8)
                        material.MaterialType = GsaMaterial.MatType.FABRIC;
                    if (typ == 0)
                        material.MaterialType = GsaMaterial.MatType.GENERIC;
                }
                else if (gh_typ.Value is GH_String)
                {
                    string typ = "CONCRETE";
                    GH_Convert.ToString(gh_typ, out typ, GH_Conversion.Both);
                    if (typ.ToUpper() == "STEEL")
                        material.MaterialType = GsaMaterial.MatType.STEEL;
                    if (typ.ToUpper() == "CONCRETE")
                        material.MaterialType = GsaMaterial.MatType.CONCRETE;
                    if (typ.ToUpper() == "FRP")
                        material.MaterialType = GsaMaterial.MatType.FRP;
                    if (typ.ToUpper() == "ALUMINIUM")
                        material.MaterialType = GsaMaterial.MatType.ALUMINIUM;
                    if (typ.ToUpper() == "TIMBER")
                        material.MaterialType = GsaMaterial.MatType.TIMBER;
                    if (typ.ToUpper() == "GLASS")
                        material.MaterialType = GsaMaterial.MatType.GLASS;
                    if (typ.ToUpper() == "FABRIC")
                        material.MaterialType = GsaMaterial.MatType.FABRIC;
                    if (typ.ToUpper() == "GENERIC")
                        material.MaterialType = GsaMaterial.MatType.GENERIC;
                }
                else
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert Material Type input");
                    return;
                }
            }

            // 3 grade
            int grd = 0;
            if (DA.GetData(3, ref grd))
            {
                material.GradeProperty = grd;
            }

            //#### outputs ####
            DA.SetData(0, new GsaMaterialGoo(material));
            DA.SetData(1, material.AnalysisProperty);
            string mate = material.MaterialType.ToString();
            mate = Char.ToUpper(mate[0]) + mate.Substring(1).ToLower().Replace("_", " ");
            DA.SetData(2, mate);
            DA.SetData(3, material.GradeProperty);
        }
    }
}

