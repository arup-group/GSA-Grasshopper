using System;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;
using OasysUnits.Units;
using OasysUnits;
using Grasshopper.Kernel.Parameters;
using Rhino;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to create a new Section
  /// </summary>
  public class EditProfile : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("980f43b3-5d3d-445a-9a52-b82e1cf4b27f");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.EditProfile;
    private bool _useDegrees = false;
    public EditProfile() : base("Edit Profile", "EditProfile",
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
        "Set Profile Orientation Angle in counter-clockwise direction.", GH_ParamAccess.item);
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
      var ghProfile = new GH_String();
      da.GetData(0, ref ghProfile);
      GH_Convert.ToString(ghProfile, out string profile, GH_Conversion.Both);

      string transformation = " [";

      var ghangle = new GH_Number();
      if (da.GetData(1, ref ghangle)) {
        if (GH_Convert.ToDouble(ghangle, out double angleInDegrees, GH_Conversion.Both)) {
          if (!_useDegrees) {
            angleInDegrees = RhinoMath.ToDegrees(angleInDegrees);
          }
          if (angleInDegrees != 0) {
            transformation += $"R({angleInDegrees})";
          }
        }
      }

      var ghHorizontal = new GH_Boolean();
      if (da.GetData(2, ref ghHorizontal)) {
        if (GH_Convert.ToBoolean(ghHorizontal, out bool horizontal, GH_Conversion.Both)) {
          if (horizontal) {
            transformation += "H";
          }
        }
      }
      var ghVertical = new GH_Boolean();
      if (da.GetData(3, ref ghVertical)) {
        if (GH_Convert.ToBoolean(ghVertical, out bool vertical, GH_Conversion.Both)) {
          if (vertical) {
            transformation += "V";
          }
        }
      }

      transformation += "]";

      if (transformation == " []") {
        transformation = string.Empty;
      }
      
      // remove any existing transformations
      profile = profile.Split('[')[0].Replace("  ", " ");

      da.SetData(0, profile + transformation);
    }
  }
}
