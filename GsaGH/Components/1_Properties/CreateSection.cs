using System;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to create a new Section
  /// </summary>
  public class CreateSection : GH_OasysComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("1167c4aa-b98b-47a7-ae85-1a3c976a1973");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.CreateSection;

    public CreateSection() : base("Create Section",
      "Section",
      "Create GSA Section",
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat1())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddTextParameter("Profile", "Pf", "Cross-Section Profile defined using the GSA Profile string syntax", GH_ParamAccess.item);
      pManager.AddParameter(new GsaMaterialParameter());
      pManager[1].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new GsaSectionParameter());
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GsaSection gsaSection = new GsaSection();

      // profile
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
