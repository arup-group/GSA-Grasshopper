using System;
using System.Drawing;

using Grasshopper.Kernel;

using GsaGH.Helpers.GH;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Parameters;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to create an IProfile from a profile description string.
  /// </summary>
  public class ProfileFromString : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("b3e2d4f1-8a7c-4e5b-9d6f-2c1a0e3b7f8d");
    public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateProfile;

    public ProfileFromString() : base("Profile from String", "PfFromStr",
      "Create an IProfile from a profile description string (e.g. \"STD I(m) 0.6 0.3 0.012 0.02\" or \"CAT HE HE200.B\")",
      CategoryName.Name(), SubCategoryName.Cat1()) {
      Hidden = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddTextParameter("Profile Description", "Pf",
        "Profile description string in STD or CAT format (e.g. \"STD I(m) 0.6 0.3 0.012 0.02\" or \"CAT HE HE200.B\")",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Unit", "U",
        "Fallback length unit abbreviation used when the profile string does not contain an explicit unit "
        + "(e.g. \"m\", \"cm\", \"mm\", \"in\", \"ft\"). Leave unconnected to use the unit embedded in the string.",
        GH_ParamAccess.item);
      pManager[1].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddGenericParameter(OasysProfileGoo.Name, OasysProfileGoo.NickName,
        OasysProfileGoo.Description, GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      string description = string.Empty;
      if (!da.GetData(0, ref description)) {
        return;
      }

      string unitAbbrev = string.Empty;
      bool hasUnit = da.GetData(1, ref unitAbbrev);

      try {
        var profile = hasUnit
          ? ProfileHelper.ProfileFromString(description, unitAbbrev)
          : ProfileHelper.ProfileFromString(description);
        da.SetData(0, new OasysProfileGoo(profile));
      } catch (Exception ex) when (ex is ArgumentException || ex is FormatException || ex is NotSupportedException) {
        this.AddRuntimeError(ex.Message);
      }
    }
  }
}
