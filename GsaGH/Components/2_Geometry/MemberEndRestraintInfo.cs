using System;
using System.Drawing;

using Grasshopper.Kernel;

using GsaAPI;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to edit a Node
  /// </summary>
  public class MemberEndRestraintInfo : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("945c9221-6dd1-4d9e-8622-42bfd816b65d");
    public override GH_Exposure Exposure => GH_Exposure.senary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.MemberEndRestraintInfo;

    public MemberEndRestraintInfo() : base("Member End Restraint Info", "MemberEndRestraintInfo", "Get information of a 1D Member's End Restraint settings for Effective Length Properties", CategoryName.Name(),
      SubCategoryName.Cat2()) {
      Hidden = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddTextParameter("Member End Restraint Syntax", "ER",
       "Restraint Description Syntax for Member End Restraint", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddTextParameter("Top Flange (F1) Warping Restraint", "F1W",
       "Top Flange (F1) Warping Restraint\nAccepted inputs are:" +
       "\n  None (0)\n  Partial (1)\n  Full (2)", GH_ParamAccess.item);
      pManager.AddTextParameter("Bottom Flange (F2) Warping Restraint", "F2W",
        "Bottom Flange (F1) Warping Restraint\nAccepted inputs are:" +
        "\n  None (0)\n  Partial (1)\n  Full (2)", GH_ParamAccess.item);
      pManager.AddTextParameter("Torsional Restraint (xx)", "xx",
        "Torsional Restraint (xx)\nAccepted inputs are:" +
        "\n  None (0)\n  Frictional (1)\n  Partial (2)\n  Full (3)", GH_ParamAccess.item);
      pManager.AddTextParameter("Major Axis Rotation (yy)", "yy",
        "Major Axis Rotational Restraint (yy)\nAccepted inputs are:" +
        "\n  None (0)\n  Partial (1)\n  Full (2)", GH_ParamAccess.item);
      pManager.AddTextParameter("Minor Axis Rotation (zz)", "zz",
        "Minor Axis Rotational Restraint (zz)\nAccepted inputs are:" +
        "\n  None (0)\n  Partial (1)\n  Full (2)", GH_ParamAccess.item);
      pManager.AddTextParameter("Top Flange (F1) Lateral Restraint", "F1L",
        "Top Flange (F1) Lateral Restraint\nAccepted inputs are:" +
        "\n  None (0)\n  Full (1)", GH_ParamAccess.item);
      pManager.AddTextParameter("Bottom Flange (F2) Lateral Restraint", "F2L",
        "Bottom Flange (F1) Lateral Restraint\nAccepted inputs are:" +
        "\n  None (0)\n  Full (1)", GH_ParamAccess.item);
      pManager.AddTextParameter("Major Axis Translation (y)", "y",
        "Major Axis Translational Restraint (y)\nAccepted inputs are:" +
        "\n  None (0)\n  Full (1)", GH_ParamAccess.item);
      pManager.AddTextParameter("Minor Axis Translation (z)", "z",
        "Minor Axis Translational Restraint (z)\nAccepted inputs are:" +
        "\n  None (0)\n  Full (1)", GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      string er = string.Empty;
      da.GetData(0, ref er);
      var auto = new EffectiveLengthFromEndRestraintAndGeometry {
        End1 = MemberEndRestraintFactory.CreateFromStrings(er)
      };
      da.SetData(0, auto.End1.TopFlangeWarpingRestraint.ToString());
      da.SetData(1, auto.End1.BottomFlangeWarpingRestraint.ToString());
      da.SetData(2, auto.End1.TorsionalRestraint.ToString());
      da.SetData(3, auto.End1.MajorAxisRotationalRestraint.ToString());
      da.SetData(4, auto.End1.MinorAxisRotationalRestraint.ToString());
      da.SetData(5, auto.End1.TopFlangeLateralRestraint.ToString());
      da.SetData(6, auto.End1.BottomFlangeLateralRestraint.ToString());
      da.SetData(7, auto.End1.MajorAxisTranslationalRestraint.ToString());
      da.SetData(8, auto.End1.MinorAxisTranslationalRestraint.ToString());
    }
  }
}
