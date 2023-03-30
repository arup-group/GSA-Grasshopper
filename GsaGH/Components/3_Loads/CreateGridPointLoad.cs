﻿using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Components {
  public class CreateGridPointLoad : GH_OasysDropDownComponent {
    protected override void SolveInstance(IGH_DataAccess da) {
      var gsaGridPointLoad = new GsaGridPointLoad();
      int loadCase = 1;
      var ghLoadCase = new GH_Integer();
      if (da.GetData(0, ref ghLoadCase))
        GH_Convert.ToInt32(ghLoadCase, out loadCase, GH_Conversion.Both);
      gsaGridPointLoad.GridPointLoad.Case = loadCase;

      var point3d = new Point3d();
      var ghPt = new GH_Point();
      if (da.GetData(1, ref ghPt))
        GH_Convert.ToPoint3d(ghPt, ref point3d, GH_Conversion.Both);
      gsaGridPointLoad.GridPointLoad.X = point3d.X;
      gsaGridPointLoad.GridPointLoad.Y = point3d.Y;

      GsaGridPlaneSurface gridPlaneSurface;
      Plane plane = Plane.WorldXY;
      var ghTyp = new GH_ObjectWrapper();
      if (da.GetData(2, ref ghTyp))
        switch (ghTyp.Value) {
          case GsaGridPlaneSurfaceGoo _: {
            var temppln = new GsaGridPlaneSurface();
            ghTyp.CastTo(ref temppln);
            gridPlaneSurface = temppln.Duplicate();
            gsaGridPointLoad.GridPlaneSurface = gridPlaneSurface;
            break;
          }
          case Plane _:
            ghTyp.CastTo(ref plane);
            gridPlaneSurface = new GsaGridPlaneSurface(plane);
            gsaGridPointLoad.GridPlaneSurface = gridPlaneSurface;
            break;
          default: {
            if (GH_Convert.ToInt32(ghTyp.Value, out int id, GH_Conversion.Both)) {
              gsaGridPointLoad.GridPointLoad.GridSurface = id;
              gsaGridPointLoad.GridPlaneSurface = null;
            }
            else {
              this.AddRuntimeError(
                "Error in GPS input. Accepted inputs are Grid Plane Surface or Plane. "
                + Environment.NewLine
                + "If no input here then the point's z-coordinate will be used for an xy-plane at that elevation");
              return;
            }

            break;
          }
        }
      else {
        plane = Plane.WorldXY;
        plane.Origin = point3d;
        gridPlaneSurface = new GsaGridPlaneSurface(plane);
        gsaGridPointLoad.GridPlaneSurface = gridPlaneSurface;
      }

      string dir = "Z";
      Direction direc = Direction.Z;

      var ghDir = new GH_String();
      if (da.GetData(3, ref ghDir))
        GH_Convert.ToString(ghDir, out dir, GH_Conversion.Both);
      dir = dir.ToUpper();
      switch (dir) {
        case "X":
          direc = Direction.X;
          break;
        case "Y":
          direc = Direction.Y;
          break;
      }

      gsaGridPointLoad.GridPointLoad.Direction = direc;

      gsaGridPointLoad.GridPointLoad.AxisProperty = 0;
      var ghAx = new GH_Integer();
      if (da.GetData(4, ref ghAx)) {
        GH_Convert.ToInt32(ghAx, out int axis, GH_Conversion.Both);
        if (axis == 0 || axis == -1)
          gsaGridPointLoad.GridPointLoad.AxisProperty = axis;
      }

      var ghName = new GH_String();
      if (da.GetData(5, ref ghName))
        if (GH_Convert.ToString(ghName, out string name, GH_Conversion.Both))
          gsaGridPointLoad.GridPointLoad.Name = name;

      gsaGridPointLoad.GridPointLoad.Value
        = ((Force)Input.UnitNumber(this, da, 6, _forceUnit)).Newtons;

      var gsaLoad = new GsaLoad(gsaGridPointLoad);
      da.SetData(0, new GsaLoadGoo(gsaLoad));
    }

    #region Name and Ribbon Layout

    public override Guid ComponentGuid => new Guid("076f03c6-67ba-49d3-9462-cd4a4b5aff92");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.PointLoad;

    public CreateGridPointLoad() : base("Create Grid Point Load",
      "PointLoad",
      "Create GSA Grid Point Load",
      CategoryName.Name(),
      SubCategoryName.Cat3())
      => Hidden = true;

    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      string unitAbbreviation = Force.GetAbbreviation(_forceUnit);

      pManager.AddIntegerParameter("Load case",
        "LC",
        "Load case number (default 1)",
        GH_ParamAccess.item,
        1);
      pManager.AddPointParameter("Point",
        "Pt",
        "Point. If you input grid plane below only x and y coordinates will be used from this point, but if not a new Grid Plane Surface (xy-plane) will be created at the z-elevation of this point.",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Grid Plane Surface",
        "GPS",
        "Grid Plane Surface or Plane (optional). If no input here then the point's z-coordinate will be used for an xy-plane at that elevation.",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Direction",
        "Di",
        "Load direction (default z)."
        + Environment.NewLine
        + "Accepted inputs are:"
        + Environment.NewLine
        + "x"
        + Environment.NewLine
        + "y"
        + Environment.NewLine
        + "z",
        GH_ParamAccess.item,
        "z");
      pManager.AddIntegerParameter("Axis",
        "Ax",
        "Load axis (default Global). "
        + Environment.NewLine
        + "Accepted inputs are:"
        + Environment.NewLine
        + "0 : Global"
        + Environment.NewLine
        + "-1 : Local",
        GH_ParamAccess.item,
        0);
      pManager.AddTextParameter("Name", "Na", "Load Name", GH_ParamAccess.item);
      pManager.AddGenericParameter("Value [" + unitAbbreviation + "]",
        "V",
        "Load Value",
        GH_ParamAccess.item);

      pManager[0]
        .Optional = true;
      pManager[2]
        .Optional = true;
      pManager[3]
        .Optional = true;
      pManager[4]
        .Optional = true;
      pManager[5]
        .Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
      => pManager.AddParameter(new GsaLoadParameter(),
        "Grid Point Load",
        "Ld",
        "GSA Grid Point Load",
        GH_ParamAccess.item);

    #endregion

    #region Custom UI

    private ForceUnit _forceUnit = DefaultUnits.ForceUnit;

    public override void InitialiseDropdowns() {
      SpacerDescriptions = new List<string>(new[] {
        "Unit",
      });

      DropDownItems = new List<List<string>>();
      SelectedItems = new List<string>();

      DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Force));
      SelectedItems.Add(Force.GetAbbreviation(_forceUnit));

      IsInitialised = true;
    }

    public override void SetSelected(int i, int j) {
      SelectedItems[i] = DropDownItems[i][j];
      _forceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), SelectedItems[i]);
      base.UpdateUI();
    }

    public override void UpdateUIFromSelectedItems() {
      _forceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), SelectedItems[0]);
      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance() {
      string unitAbbreviation = Force.GetAbbreviation(_forceUnit);
      Params.Input[6]
        .Name = "Value [" + unitAbbreviation + "]";
    }

    #endregion
  }
}
