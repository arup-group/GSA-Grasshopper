using System;
using System.Drawing;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;

using GsaGH.Helpers.GH;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;

using Rhino;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to create a new Section
  /// </summary>
  public class EditProfile : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("980f43b3-5d3d-445a-9a52-b82e1cf4b27f");
    public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.EditProfile;
    private bool _useDegrees = false;
    public EditProfile() : base("Edit Profile", "EditPf",
      "Transform a Profile by rotation or reflection.",
      CategoryName.Name(), SubCategoryName.Cat1()) {
      Hidden = true;
    }
    protected override void BeforeSolveInstance() {
      base.BeforeSolveInstance();
      _useDegrees = false;
      if (Params.Input[1] is Param_Number angleParameter) {
        _useDegrees = angleParameter.UseDegrees;
      }
    }
    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddTextParameter("Profile", "Pf", "Profile to edit", GH_ParamAccess.item);
      pManager.AddAngleParameter("Orientation Angle", "⭮A",
        "Set Profile Orientation Angle in counter-clockwise direction", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Reflect Horizontal", "Ry",
        "True to reflect the profile about the local y-axis", GH_ParamAccess.item, false);
      pManager.AddBooleanParameter("Reflect Vertical", "Rz",
        "True to reflect the profile about the local z-axis", GH_ParamAccess.item, false);
      pManager[1].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddTextParameter("Profile", "Pf",
        "Edited Profile with applied transformations", GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      string profile = string.Empty;
      da.GetData(0, ref profile);
      string transformation = " [";
      double angle = 0;
      if (da.GetData(1, ref angle)) {
        if (!_useDegrees) {
          angle = RhinoMath.ToDegrees(angle);
        }

        if (angle != 0) {
          transformation += $"R({angle})";
        }
      }

      bool ghHorizontal = false;
      if (da.GetData(2, ref ghHorizontal)) {
        if (ghHorizontal) {
          transformation += "H";
        }
      }
      bool ghVertical = false;
      if (da.GetData(3, ref ghVertical)) {
        if (ghVertical) {
          transformation += "V";
        }
      }

      transformation += "]";

      if (transformation == " []") {
        transformation = string.Empty;
      }

      // remove any existing transformations
      profile = profile.Split('[')[0].TrimEnd();

      da.SetData(0, profile + transformation);
    }
  }
}
