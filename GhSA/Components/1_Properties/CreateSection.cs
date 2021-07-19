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
            pManager.AddGenericParameter("Material", "Ma", "GsaMaterial or Number for referring to a Material already in Existing GSA Model." + System.Environment.NewLine
                    + "Accepted inputs are: " + System.Environment.NewLine
                    + "0 : Generic" + System.Environment.NewLine
                    + "1 : Steel" + System.Environment.NewLine
                    + "2 : Concrete" + System.Environment.NewLine
                    + "3 : Aluminium" + System.Environment.NewLine
                    + "4 : Glass" + System.Environment.NewLine
                    + "5 : FRP" + System.Environment.NewLine
                    + "7 : Timber (default - because your Carbon Emissions matter!)" + System.Environment.NewLine
                    + "8 : Fabric", GH_ParamAccess.item);

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
            GsaSection gsaSection = new GsaSection();

            //profile
            GH_String gh_profile = new GH_String();
            if (DA.GetData(0, ref gh_profile))
            {
                if (GH_Convert.ToString(gh_profile, out string profile, GH_Conversion.Both))
                {
                    gsaSection = new GsaSection(profile);

                    // 3 Material
                    GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
                    if (DA.GetData(1, ref gh_typ))
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
                                gsaSection.Material = new GsaMaterial(idd);
                            else
                            {
                                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert PB input to a Section Property of reference integer");
                                return;
                            }
                        }
                    }
                    else
                        gsaSection.Material = new GsaMaterial(7); // because Timber

                }
                DA.SetData(0, new GsaSectionGoo(gsaSection));
            }
        }
    }
}

