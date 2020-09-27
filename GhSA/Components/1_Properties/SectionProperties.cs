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
    /// Component to get geometric properties of a section
    /// </summary>
    public class GetSectionProperties : GH_Component, IGH_PreviewObject
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("6504a99f-a4e2-4e30-8251-de31ea83e8cb");
        public GetSectionProperties()
          : base("Section Properties", "SectProp", "Get GSA Section Properties",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat1())
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
            pManager.AddGenericParameter("Section", "PB", "GSA Section to get a bit more info out of", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Area", "A", "GSA Section Area (" + Util.Unit.Length_Section + "\xB2)", GH_ParamAccess.item);
            pManager.AddNumberParameter("Moment of Inertia y-y", "Iyy", "GSA Section Moment of Intertia around local y-y axis (" + Util.Unit.Length_Section + "\x2074)", GH_ParamAccess.item);
            pManager.AddNumberParameter("Moment of Inertia z-z", "Izz", "GSA Section Moment of Intertia around local z-z axis (" + Util.Unit.Length_Section + "\x2074)", GH_ParamAccess.item);
            pManager.AddNumberParameter("Moment of Inertia y-z", "Iyz", "GSA Section Moment of Intertia around local y-z axis (" + Util.Unit.Length_Section + "\x2074)", GH_ParamAccess.item);
            pManager.AddNumberParameter("Torsion constant", "J", "GSA Section Torsion constant J (" + Util.Unit.Length_Section + "\x2074)", GH_ParamAccess.item);
            pManager.AddNumberParameter("Shear Area Factor in y", "Ky", "GSA Section Shear Area Factor in local y-direction (-)", GH_ParamAccess.item);
            pManager.AddNumberParameter("Shear Area Factor in z", "Kz", "GSA Section Shear Area Factor in local z-direction (-)", GH_ParamAccess.item);
            pManager.AddNumberParameter("Surface A/Length", "S/L", "GSA Section Surface Area per Unit Length (" + Util.Unit.Length_Section + "\xB2/"+ Util.Unit.Length_Large + ")", GH_ParamAccess.item);
            pManager.AddNumberParameter("Volume/Length", "V/L", "GSA Section Volume per Unit Length (" + Util.Unit.Length_Section + "\xB3/"+ Util.Unit.Length_Large + ")", GH_ParamAccess.item);

        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GsaSection gsaSection = new GsaSection();
            GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
            if (DA.GetData(0, ref gh_typ))
            {
                if (gh_typ.Value is GsaSection)
                    gh_typ.CastTo(ref gsaSection);
                else
                {
                    string profile = "";
                    gh_typ.CastTo(ref profile);
                    gsaSection = new GsaSection(profile);
                }
            }
            if (gsaSection != null)
            {
                double conversionfactor = 1;
                if (Util.Unit.Length_Section != "m")
                {
                    switch (Util.Unit.Length_Section)
                    {
                        case "mm":
                            conversionfactor = 1000;
                            break;
                        case "cm":
                            conversionfactor = 100;
                            break;
                        case "in":
                            conversionfactor = 1000 / 25.4;
                            break;
                        case "ft":
                            conversionfactor = 1000 / (12 * 25.4);
                            break;
                    }
                }
                DA.SetData(0, gsaSection.Section.Area * Math.Pow(conversionfactor, 2));
                DA.SetData(1, gsaSection.Section.Iyy * Math.Pow(conversionfactor, 4));
                DA.SetData(2, gsaSection.Section.Izz * Math.Pow(conversionfactor, 4));
                DA.SetData(3, gsaSection.Section.Iyz * Math.Pow(conversionfactor, 4));
                DA.SetData(4, gsaSection.Section.J * Math.Pow(conversionfactor, 4));
                DA.SetData(5, gsaSection.Section.Ky);
                DA.SetData(6, gsaSection.Section.Kz);
                DA.SetData(7, gsaSection.Section.SurfaceAreaPerLength * Math.Pow(conversionfactor, 2));
                DA.SetData(8, gsaSection.Section.VolumePerLength * Math.Pow(conversionfactor, 3));
            }
        }
    }
}

