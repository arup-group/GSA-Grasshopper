using System;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Parameters;
using UnitsNet.GH;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to get geometric properties of a section
  /// </summary>
  public class GetSectionProperties : GH_OasysComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("6504a99f-a4e2-4e30-8251-de31ea83e8cb");
    public GetSectionProperties()
      : base("Section Properties", "SectProp", "Get GSA Section Properties",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat1())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    public override GH_Exposure Exposure => GH_Exposure.quarternary | GH_Exposure.obscure;

    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.SectionProperties;
    #endregion

    #region Custom UI
    //This region overrides the typical component layout

    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter("Section", "PB", "Profile or GSA Section to get a bit more info out of", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Area", "A", "Section Area", GH_ParamAccess.item);
      pManager.AddGenericParameter("Moment of Inertia y-y", "Iyy", "Section Moment of Intertia around local y-y axis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Moment of Inertia z-z", "Izz", "Section Moment of Intertia around local z-z axis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Moment of Inertia y-z", "Iyz", "Section Moment of Intertia around local y-z axis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Torsion constant", "J", "Section Torsion constant J", GH_ParamAccess.item);
      pManager.AddGenericParameter("Shear Area Factor in y", "Ky", "Section Shear Area Factor in local y-direction", GH_ParamAccess.item);
      pManager.AddGenericParameter("Shear Area Factor in z", "Kz", "Section Shear Area Factor in local z-direction", GH_ParamAccess.item);
      pManager.AddGenericParameter("Surface A/Length", "S/L", "Section Surface Area per Unit Length", GH_ParamAccess.item);
      pManager.AddGenericParameter("Volume/Length", "V/L", "Section Volume per Unit Length", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GsaSection gsaSection = new GsaSection();
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(0, ref gh_typ))
      {
        if (gh_typ.Value is GsaSectionGoo)
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

        DA.SetData(0, new GH_UnitNumber(gsaSection.Area));
        DA.SetData(1, new GH_UnitNumber(gsaSection.Iyy));
        DA.SetData(2, new GH_UnitNumber(gsaSection.Izz));
        DA.SetData(3, new GH_UnitNumber(gsaSection.Iyz));
        DA.SetData(4, new GH_UnitNumber(gsaSection.J));
        DA.SetData(5, gsaSection.Ky);
        DA.SetData(6, gsaSection.Kz);
        DA.SetData(7, new GH_UnitNumber(gsaSection.SurfaceAreaPerLength));
        DA.SetData(8, new GH_UnitNumber(gsaSection.VolumePerLength));
      }
    }
  }
}

