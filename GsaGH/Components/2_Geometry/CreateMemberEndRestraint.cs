using System;
using System.Collections.Generic;
using System.Drawing;

using Grasshopper.Kernel;

using GsaAPI;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {
  public class CreateMemberEndRestraint : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("ffd0e792-4dc9-49bd-88ef-48dc0e0a2178");
    public override GH_Exposure Exposure => GH_Exposure.senary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateMemberEndRestraint;
    private readonly List<string> _shortcuts = new List<string>(new[] {
        "Pinned",
        "Fixed",
        "Free",
        "FullRotational",
        "PartialRotational",
        "TopFlangeLateral",
      });
    public CreateMemberEndRestraint() : base("Create Member End Restraint", "EndRestraint",
      "Create Member End Restraint Settings for Effective Length Properties",
      CategoryName.Name(), SubCategoryName.Cat2()) {
      Hidden = true;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Shortcuts",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(_shortcuts);
      _selectedItems.Add(_dropDownItems[0][0]);

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
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
      for (int i = 0; i < pManager.ParamCount; i++) {
        pManager[i].Optional = true;
      }
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddTextParameter("Member End Restraint Syntax", "ER",
        "Restraint Description Syntax for Member End Restraint", GH_ParamAccess.item);
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      bool noInputs = true;
      foreach (IGH_Param param in Params.Input) {
        if (param.SourceCount > 0) {
          noInputs = false;
          break;
        }
      }

      if (noInputs) {
        da.SetData(0, _selectedItems[0]);
        return;
      }

      _selectedItems[0] = "Custom";
      this.AddRuntimeRemark("All inputs without input have been set to 'None (0)'");

      string f1 = "F1";
      string f2 = "F2";
      string t = "T";
      string maj = "MAJ";
      string min = "MIN";

      string f1V = string.Empty;
      if (da.GetData(5, ref f1V)) {
        TranslationalRestraint rot = MemberEndRestraintFactory.TranslationalRestraint(f1V);
        if (rot != TranslationalRestraint.None) {
          f1 += "L";
        }
      }

      string f2V = string.Empty;
      if (da.GetData(6, ref f2V)) {
        TranslationalRestraint rot = MemberEndRestraintFactory.TranslationalRestraint(f2V);
        if (rot != TranslationalRestraint.None) {
          f2 += "L";
        }
      }

      string majV = string.Empty;
      if (da.GetData(7, ref majV)) {
        TranslationalRestraint rot = MemberEndRestraintFactory.TranslationalRestraint(majV);
        if (rot != TranslationalRestraint.None) {
          maj += "V";
        }
      }

      string minV = string.Empty;
      if (da.GetData(8, ref minV)) {
        TranslationalRestraint rot = MemberEndRestraintFactory.TranslationalRestraint(minV);
        if (rot != TranslationalRestraint.None) {
          min += "V";
        }
      }

      string f1W = string.Empty;
      if (da.GetData(0, ref f1W)) {
        RotationalRestraint rot = MemberEndRestraintFactory.RotationalRestraint(f1W);
        if (rot != RotationalRestraint.None) {
          f1 += rot == RotationalRestraint.Partial ? "P" : "W";
        }
      }

      string f2W = string.Empty;
      if (da.GetData(1, ref f2W)) {
        RotationalRestraint rot = MemberEndRestraintFactory.RotationalRestraint(f2W);
        if (rot != RotationalRestraint.None) {
          f2 += rot == RotationalRestraint.Partial ? "P" : "W";
        }
      }

      string xx = string.Empty;
      if (da.GetData(2, ref xx)) {
        switch (MemberEndRestraintFactory.TorsionalRestraint(xx)) {
          case TorsionalRestraint.Friction:
            t += "F";
            break;

          case TorsionalRestraint.Partial:
            t += "P";
            break;

          case TorsionalRestraint.Full:
            t += "R";
            break;

          case TorsionalRestraint.None:
            t = string.Empty;
            break;
        }
      }

      string majR = string.Empty;
      if (da.GetData(3, ref majR)) {
        RotationalRestraint rot = MemberEndRestraintFactory.RotationalRestraint(majR);
        if (rot != RotationalRestraint.None) {
          maj += rot == RotationalRestraint.Partial ? "P" : "W";
        }
      }

      string minR = string.Empty;
      if (da.GetData(4, ref minR)) {
        RotationalRestraint rot = MemberEndRestraintFactory.RotationalRestraint(minR);
        if (rot != RotationalRestraint.None) {
          min += rot == RotationalRestraint.Partial ? "P" : "W";
        }
      }

      // create the restraint to trigger the MakeConsistent() check
      MemberEndRestraint res = MemberEndRestraintFactory.CreateFromStrings(f1, f2, t, maj, min);
      da.SetData(0, MemberEndRestraintFactory.MemberEndRestraintToString(res));
    }
  }
}
