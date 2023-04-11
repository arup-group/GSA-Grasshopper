using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
  public class CreateGridLineLoad : GH_OasysDropDownComponent {
    protected override void SolveInstance(IGH_DataAccess da) {
      var gridlineload = new GsaGridLineLoad();
      int loadCase = 1;
      var ghLc = new GH_Integer();
      if (da.GetData(0, ref ghLc))
        GH_Convert.ToInt32(ghLc, out loadCase, GH_Conversion.Both);
      gridlineload.GridLineLoad.Case = loadCase;

      // Do plane input first as to see if we need to project polyline onto grid plane
      Plane plane = Plane.WorldXY;
      bool planeSet = false;
      var gridPlaneSurface = new GsaGridPlaneSurface();
      var ghTyp = new GH_ObjectWrapper();
      if (da.GetData(2, ref ghTyp))
        switch (ghTyp.Value) {
          case GsaGridPlaneSurfaceGoo _: {
            var temppln = new GsaGridPlaneSurface();
            ghTyp.CastTo(ref temppln);
            gridPlaneSurface = temppln.Duplicate();
            plane = gridPlaneSurface.Plane;
            planeSet = true;
            break;
          }
          case Plane _:
            ghTyp.CastTo(ref plane);
            gridPlaneSurface = new GsaGridPlaneSurface(plane);
            planeSet = true;
            break;
          default: {
            if (GH_Convert.ToInt32(ghTyp.Value, out int id, GH_Conversion.Both)) {
              gridlineload.GridLineLoad.GridSurface = id;
              gridlineload.GridPlaneSurface = null;
            }
            else {
              this.AddRuntimeError(
                "Error in GPS input. Accepted inputs are Grid Plane Surface or Plane. "
                + Environment.NewLine
                + "If no input here then the line's best-fit plane will be used");
              return;
            }

            break;
          }
        }

      // we wait setting the gridplanesurface until we have run the polyline input
      var ghCurve = new GH_Curve();
      if (da.GetData(1, ref ghCurve)) {
        Curve curve = null;
        GH_Convert.ToCurve(ghCurve, ref curve, GH_Conversion.Both);

        if (curve.TryGetPolyline(out Polyline ln)) {
          var controlPoints = ln.ToList();

          if (!planeSet) {
            plane = RhinoConversions.CreateBestFitUnitisedPlaneFromPts(controlPoints);

            gridPlaneSurface = new GsaGridPlaneSurface(plane, true);
          }
          else {
            curve = Curve.ProjectToPlane(curve, plane);

            curve.TryGetPolyline(out ln);

            controlPoints = ln.ToList();
          }

          string desc = "";

          for (int i = 0; i < controlPoints.Count; i++) {
            if (i > 0)
              desc += " ";

            plane.RemapToPlaneSpace(controlPoints[i], out Point3d temppt);
            // format accepted by GSA: (0,0) (0,1) (1,2) (3,4) (4,0)(m)
            desc += "(" + temppt.X + "," + temppt.Y + ")";
          }

          desc += "(" + DefaultUnits.LengthUnitGeometry + ")";

          gridlineload.GridLineLoad.Type = GridLineLoad.PolyLineType.EXPLICIT_POLYLINE;
          gridlineload.GridLineLoad.PolyLineDefinition = desc;
        }
        else
          this.AddRuntimeError("Could not convert Curve to Polyline");
      }

      if (gridlineload.GridPlaneSurface != null)
        if (gridlineload.GridPlaneSurface.GridSurfaceId == 0)
          gridlineload.GridPlaneSurface = gridPlaneSurface;

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

      gridlineload.GridLineLoad.Direction = direc;

      gridlineload.GridLineLoad.AxisProperty = 0;
      var ghAxis = new GH_Integer();
      if (da.GetData(4, ref ghAxis)) {
        GH_Convert.ToInt32(ghAxis, out int axis, GH_Conversion.Both);
        if (axis == 0 || axis == -1)
          gridlineload.GridLineLoad.AxisProperty = axis;
      }

      var ghProj = new GH_Boolean();
      if (da.GetData(5, ref ghProj))
        if (GH_Convert.ToBoolean(ghProj, out bool proj, GH_Conversion.Both))
          gridlineload.GridLineLoad.IsProjected = proj;

      var ghName = new GH_String();
      if (da.GetData(6, ref ghName))
        if (GH_Convert.ToString(ghName, out string name, GH_Conversion.Both))
          gridlineload.GridLineLoad.Name = name;

      double load1 = ((ForcePerLength)Input.UnitNumber(this, da, 7, _forcePerLengthUnit))
        .NewtonsPerMeter;
      gridlineload.GridLineLoad.ValueAtStart = load1;

      double load2 = load1;
      if (da.GetData(8, ref load2))
        load2 = ((ForcePerLength)Input.UnitNumber(this, da, 8, _forcePerLengthUnit, true))
          .NewtonsPerMeter;
      gridlineload.GridLineLoad.ValueAtEnd = load2;

      var gsaLoad = new GsaLoad(gridlineload);
      da.SetData(0, new GsaLoadGoo(gsaLoad));
    }

    #region Name and Ribbon Layout

    public override Guid ComponentGuid => new Guid("e1f22e6f-8550-4078-8613-ea5ed2ede2b9");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.LineLoad;

    public CreateGridLineLoad() : base("Create Grid Line Load",
      "LineLoad",
      "Create GSA Grid Line Load",
      CategoryName.Name(),
      SubCategoryName.Cat3())
      => Hidden = true;

    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      string unitAbbreviation = ForcePerLength.GetAbbreviation(_forcePerLengthUnit);

      pManager.AddIntegerParameter("Load case",
        "LC",
        "Load case number (default 1)",
        GH_ParamAccess.item,
        1);
      pManager.AddCurveParameter("PolyLine",
        "L",
        "PolyLine. If you input grid plane below only x and y coordinate positions will be used from this polyline, but if not a new Grid Plane Surface (best-fit plane) will be created from PolyLine control points.",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Grid Plane Surface",
        "GPS",
        "Grid Plane Surface or Plane (optional). If no input here then the line's best-fit plane will be used",
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
      pManager.AddBooleanParameter("Projected",
        "Pj",
        "Projected (default not)",
        GH_ParamAccess.item,
        false);
      pManager.AddTextParameter("Name", "Na", "Load Name", GH_ParamAccess.item);
      pManager.AddGenericParameter("Value Start [" + unitAbbreviation + "]",
        "V1",
        "Load Value at Start of Line",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Value End [" + unitAbbreviation + "]",
        "V2",
        "Load Value at End of Line (default : Start Value)",
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
      pManager[6]
        .Optional = true;
      pManager[8]
        .Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
      => pManager.AddParameter(new GsaLoadParameter(),
        "Grid Line Load",
        "Ld",
        "GSA Grid Line Load",
        GH_ParamAccess.item);

    #endregion

    #region Custom UI

    private ForcePerLengthUnit _forcePerLengthUnit = DefaultUnits.ForcePerLengthUnit;

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Unit",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations((EngineeringUnits.ForcePerLength)));
      _selectedItems.Add(ForcePerLength.GetAbbreviation(_forcePerLengthUnit));

      _isInitialised = true;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      _forcePerLengthUnit
        = (ForcePerLengthUnit)UnitsHelper.Parse(typeof(ForcePerLengthUnit), _selectedItems[i]);
      base.UpdateUI();
    }

    protected override void UpdateUIFromSelectedItems() {
      _forcePerLengthUnit
        = (ForcePerLengthUnit)UnitsHelper.Parse(typeof(ForcePerLengthUnit), _selectedItems[0]);
      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance() {
      string unitAbbreviation = ForcePerLength.GetAbbreviation(_forcePerLengthUnit);
      Params.Input[7]
        .Name = "Value Start [" + unitAbbreviation + "]";
      Params.Input[8]
        .Name = "Value End [" + unitAbbreviation + "]";
    }

    #endregion
  }
}
