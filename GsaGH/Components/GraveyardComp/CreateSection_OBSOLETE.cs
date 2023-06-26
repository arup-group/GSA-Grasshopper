using System;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to create a new Section
  /// </summary>
  public class CreateSection_OBSOLETE : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("1167c4aa-b98b-47a7-ae85-1a3c976a1973");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    protected override Bitmap Icon => Resources.CreateSection;

    public CreateSection_OBSOLETE() : base("Create Section", "Section", "Create GSA Section",
      CategoryName.Name(), SubCategoryName.Cat1()) {
      Hidden = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddTextParameter("Profile", "Pf",
        "Cross-Section Profile defined using the GSA Profile string syntax", GH_ParamAccess.item);
      pManager.AddParameter(new GsaMaterialParameter());
      pManager[1].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaSectionParameter());
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var gsaSection = new GsaSection();
      var ghProfile = new GH_String();
      if (!da.GetData(0, ref ghProfile)) {
        return;
      }

      if (GH_Convert.ToString(ghProfile, out string profile, GH_Conversion.Both)) {
        if (GsaSection.ValidProfile(profile)) {
          gsaSection = new GsaSection(profile);
        } else {
          this.AddRuntimeWarning($"Invalid profile syntax: {profile}");
          return;
        }

        GsaMaterialGoo materialGoo = null;
        if (da.GetData(1, ref materialGoo)) {
          gsaSection.Material = materialGoo.Value;
        }
      }

      da.SetData(0, new GsaSectionGoo(gsaSection));
    }
  }
}
