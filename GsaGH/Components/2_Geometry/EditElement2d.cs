using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;
using OasysGH.Units;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to edit a 2D Element
  /// </summary>
  public class EditElement2d : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("0b4ecb0e-ef8f-4b42-bcf2-de940594fada");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.EditElem2d;
    private AngleUnit _angleUnit = AngleUnit.Radian;

    public EditElement2d() : base("Edit 2D Element", "Elem2dEdit", "Modify GSA 2D Element",
      CategoryName.Name(), SubCategoryName.Cat2()) { }

    protected override void BeforeSolveInstance() {
      base.BeforeSolveInstance();
      if (Params.Input[5] is Param_Number angleParameter) {
        _angleUnit = angleParameter.UseDegrees ? AngleUnit.Degree : AngleUnit.Radian;
      }
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaElement2dParameter(), GsaElement2dGoo.Name,
        GsaElement2dGoo.NickName,
        GsaElement2dGoo.Description + " to get or set information for." + GsaElement2dGoo.Name,
        GH_ParamAccess.item);
      pManager.AddIntegerParameter("Element2d Number", "ID",
        "Set Element Number. If ID is set it will replace any existing 2D Element in the model",
        GH_ParamAccess.list);
      pManager.AddParameter(new GsaProp2dParameter(), "2D Property", "PA",
        "Change 2D Property. Input either a GSA 2D Property or an Integer to use a Property already defined in model",
        GH_ParamAccess.list);
      pManager.AddIntegerParameter("Element2d Group", "Gr", "Set Element Group",
        GH_ParamAccess.list);
      pManager.AddParameter(new GsaOffsetParameter(), "Offset", "Of", "Set Element Offset",
        GH_ParamAccess.list);
      pManager.AddAngleParameter("Orientation Angle", "⭮A", "Set Element Orientation Angle",
        GH_ParamAccess.list);
      pManager.AddTextParameter("Element2d Name", "Na", "Set Name of Element", GH_ParamAccess.list);
      pManager.AddColourParameter("Element2d Colour", "Co", "Set Element Colour",
        GH_ParamAccess.list);
      pManager.AddBooleanParameter("Dummy Element", "Dm", "Set Element to Dummy",
        GH_ParamAccess.list);

      for (int i = 1; i < pManager.ParamCount; i++) {
        pManager[i].Optional = true;
      }

      pManager.HideParameter(0);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaElement2dParameter(), GsaElement2dGoo.Name,
        GsaElement2dGoo.NickName, GsaElement2dGoo.Description + " with applied changes.",
        GH_ParamAccess.item);
      pManager.AddIntegerParameter("Number", "ID", "Get Element Number", GH_ParamAccess.list);
      pManager.AddMeshParameter("Analysis Mesh", "M", "Get Analysis Mesh", GH_ParamAccess.item);
      pManager.HideParameter(2);
      pManager.AddParameter(new GsaProp2dParameter(), "2D Property", "PA",
        "Get 2D Property. Input either a GSA 2D Property or an Integer to use a Property already defined in model",
        GH_ParamAccess.list);
      pManager.AddIntegerParameter("Group", "Gr", "Get Element Group", GH_ParamAccess.list);
      pManager.AddTextParameter("Element Type", "eT",
        "Get Element 2D Type." + Environment.NewLine
        + "Type can not be set; it is either Tri3 or Quad4" + Environment.NewLine
        + "depending on Rhino/Grasshopper mesh face type", GH_ParamAccess.list);
      pManager.AddParameter(new GsaOffsetParameter(), "Offset", "Of", "Get Element Offset",
        GH_ParamAccess.list);
      pManager.AddNumberParameter("Orientation Angle", "⭮A",
        "Get Element Orientation Angle in radians", GH_ParamAccess.list);
      pManager.AddTextParameter("Name", "Na", "Set Element Name", GH_ParamAccess.list);
      pManager.AddColourParameter("Colour", "Co", "Get Element Colour", GH_ParamAccess.list);
      pManager.AddBooleanParameter("Dummy Element", "Dm", "Get if Element is Dummy",
        GH_ParamAccess.list);
      pManager.AddIntegerParameter("Parent Members", "pM",
        "Get Parent Member IDs in Model that Element was created from", GH_ParamAccess.list);
      pManager.AddIntegerParameter("Topology", "Tp",
        "Get the Element's original topology list referencing node IDs in Model that Element was created from",
        GH_ParamAccess.list);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var gsaElement2d = new GsaElement2d();
      if (!da.GetData(0, ref gsaElement2d)) {
        return;
      }

      if (gsaElement2d == null) {
        this.AddRuntimeWarning("Element2D input is null");
      }

      GsaElement2d elem = gsaElement2d.Duplicate(true);

      // #### inputs ####

      // no good way of updating location of mesh on the fly //
      // suggest users re-create from scratch //

      // 1 ID
      var ghId = new List<GH_Integer>();
      var inIds = new List<int>();
      if (da.GetDataList(1, ghId)) {
        for (int i = 0; i < ghId.Count; i++) {
          if (i > elem.ApiElements.Count - 1) {
            this.AddRuntimeWarning("ID input List Length is longer than number of elements."
              + Environment.NewLine + "Excess ID's have been ignored");
            continue;
          }

          if (!GH_Convert.ToInt32(ghId[i], out int id, GH_Conversion.Both)) {
            continue;
          }

          if (inIds.Contains(id)) {
            if (id > 0) {
              this.AddRuntimeWarning("ID input(" + i + ") = " + id
                + " already exist in your input list." + Environment.NewLine
                + "You must provide a list of unique IDs, or set ID = 0 if you want to let GSA handle the numbering");
              continue;
            }
          }

          inIds.Add(id);
        }

        elem.Ids = inIds;
      }

      // 2 section
      var ghTypes = new List<GH_ObjectWrapper>();
      if (da.GetDataList(2, ghTypes)) {
        var prop2Ds = new List<GsaProp2d>();
        for (int i = 0; i < ghTypes.Count; i++) {
          if (i > elem.ApiElements.Count) {
            this.AddRuntimeWarning("PA input List Length is longer than number of elements."
              + Environment.NewLine + "Excess PA's have been ignored");
          }

          GH_ObjectWrapper ghTyp = ghTypes[i];
          var prop2d = new GsaProp2d();
          if (ghTyp.Value is GsaProp2dGoo) {
            ghTyp.CastTo(ref prop2d);
            prop2Ds.Add(prop2d);
          } else {
            if (GH_Convert.ToInt32(ghTyp.Value, out int id, GH_Conversion.Both)) {
              prop2Ds.Add(new GsaProp2d(id));
            } else {
              this.AddRuntimeError(
                "Unable to convert PA input to a 2D Property of reference integer");
              return;
            }
          }
        }

        elem.Properties = prop2Ds;
      }

      // 3 Group
      var ghgrp = new List<GH_Integer>();
      if (da.GetDataList(3, ghgrp)) {
        var inGroups = new List<int>();
        for (int i = 0; i < ghgrp.Count; i++) {
          if (i > elem.ApiElements.Count) {
            this.AddRuntimeWarning("Group input List Length is longer than number of elements."
              + Environment.NewLine + "Excess Group numbers have been ignored");
            continue;
          }

          if (GH_Convert.ToInt32(ghgrp[i], out int grp, GH_Conversion.Both)) {
            inGroups.Add(grp);
          }
        }

        elem.Groups = inGroups;
      }

      // 4 offset
      ghTypes = new List<GH_ObjectWrapper>();
      if (da.GetDataList(4, ghTypes)) {
        var inOffsets = new List<GsaOffset>();
        for (int i = 0; i < ghTypes.Count; i++) {
          if (i > elem.ApiElements.Count) {
            this.AddRuntimeWarning("Offset input List Length is longer than number of elements."
              + Environment.NewLine + "Excess Offsets have been ignored");
          }

          GH_ObjectWrapper ghTyp = ghTypes[i];
          var offset = new GsaOffset();
          if (ghTyp.Value is GsaOffsetGoo) {
            ghTyp.CastTo(ref offset);
          } else {
            if (GH_Convert.ToDouble(ghTyp.Value, out double z, GH_Conversion.Both)) {
              offset.Z = new Length(z, DefaultUnits.LengthUnitGeometry);
              string unitAbbreviation = string.Concat(offset.Z.ToString().Where(char.IsLetter));
              this.AddRuntimeRemark("Offset input converted to Z-offset in [" + unitAbbreviation
                + "]" + Environment.NewLine
                + "Note that this is based on your unit settings and may be changed to a different unit if you share this file or change your 'Length - geometry' unit settings");
            } else {
              this.AddRuntimeError("Unable to convert Offset input to Offset or double");
              return;
            }
          }

          inOffsets.Add(offset);
        }

        elem.Offsets = inOffsets;
      }

      // 5 orientation angle
      var ghangles = new List<GH_Number>();
      if (da.GetDataList(5, ghangles)) {
        var inAngles = new List<Angle>();
        for (int i = 0; i < ghangles.Count; i++) {
          if (i > elem.ApiElements.Count) {
            this.AddRuntimeWarning(
              "Orientation Angle input List Length is longer than number of elements."
              + Environment.NewLine + "Excess Angles have been ignored");
            continue;
          }

          if (GH_Convert.ToDouble(ghangles[i], out double angle, GH_Conversion.Both)) {
            inAngles.Add(new Angle(angle, _angleUnit));
          }
        }

        elem.OrientationAngles = inAngles;
      }

      // 6 name
      var ghnm = new List<GH_String>();
      if (da.GetDataList(6, ghnm)) {
        var inNames = new List<string>();
        for (int i = 0; i < ghnm.Count; i++) {
          if (i > elem.ApiElements.Count) {
            this.AddRuntimeWarning("Name input List Length is longer than number of elements."
              + Environment.NewLine + "Excess Names have been ignored");
            continue;
          }

          if (GH_Convert.ToString(ghnm[i], out string name, GH_Conversion.Both)) {
            inNames.Add(name);
          }
        }

        elem.Names = inNames;
      }

      // 7 Colour
      var ghcol = new List<GH_Colour>();
      if (da.GetDataList(7, ghcol)) {
        var inColours = new List<Color>();
        for (int i = 0; i < ghcol.Count; i++) {
          if (i > elem.ApiElements.Count) {
            this.AddRuntimeWarning("Colour input List Length is longer than number of elements."
              + Environment.NewLine + "Excess Colours have been ignored");
            continue;
          }

          if (GH_Convert.ToColor(ghcol[i], out Color col, GH_Conversion.Both)) {
            inColours.Add(col);
          }
        }

        elem.Colours = inColours;
      }

      // 8 Dummy
      var ghdum = new List<GH_Boolean>();

      if (da.GetDataList(8, ghdum)) {
        var inDummies = new List<bool>();
        for (int i = 0; i < ghdum.Count; i++) {
          if (i > elem.ApiElements.Count) {
            this.AddRuntimeWarning("Dummy input List Length is longer than number of elements."
              + Environment.NewLine + "Excess Dummy booleans have been ignored");
            continue;
          }

          if (GH_Convert.ToBoolean(ghdum[i], out bool dum, GH_Conversion.Both)) {
            inDummies.Add(dum);
          }
        }

        elem.IsDummies = inDummies;
      }

      // #### outputs ####
      da.SetData(0, new GsaElement2dGoo(elem));
      da.SetDataList(1, elem.Ids);
      da.SetData(2, elem.Mesh);
      da.SetDataList(3,
        new List<GsaProp2dGoo>(elem.Properties.ConvertAll(prop2d => new GsaProp2dGoo(prop2d))));
      da.SetDataList(4, elem.Groups);
      da.SetDataList(5, elem.Types);
      da.SetDataList(6,
        new List<GsaOffsetGoo>(elem.Offsets.ConvertAll(offset => new GsaOffsetGoo(offset))));
      da.SetDataList(7, elem.OrientationAngles.ConvertAll(angle => angle.Radians));
      da.SetDataList(8, elem.Names);
      da.SetDataList(9, elem.Colours);
      da.SetDataList(10, elem.IsDummies);
      da.SetDataList(11, elem.ParentMembers);
      da.SetDataTree(12, elem.TopologyIDs);
    }
  }
}
