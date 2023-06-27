using System;
using System.Drawing;
using System.IO;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {
  /// <summary>
  /// Component to create GSA profile
  /// </summary>
  public class CreateProfile : CreateOasysProfile {
    public override Guid ComponentGuid => new Guid("ea1741e5-905e-4ecb-8270-a584e3f99aa3");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateProfile;

    public override string DataSource => Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3");

    public CreateProfile() : base("Create Profile", "Profile",
      "Create Profile text-string for a GSA Section", CategoryName.Name(), SubCategoryName.Cat1()) {
      Hidden = true;
    }

    protected override string HtmlHelp_Source() {
      string help
        = "GOTO:https://arup-group.github.io/oasys-combined/adsec-api/api/Oasys.Profiles.html";
      return help;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddGenericParameter("Profile", "Pf", "Profile for a GSA Section", GH_ParamAccess.tree);
    }
  }
}
