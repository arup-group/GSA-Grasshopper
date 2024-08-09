using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using GH_IO.Serialization;

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

using Rhino.Collections;
using Rhino.Geometry;

using ExpansionType = GsaGH.Parameters.ExpansionType;

namespace GsaGH.Components {
  public class CreateGridLineLoad : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("e1f22e6f-8550-4078-8613-ea5ed2ede2b9");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateGridLineLoad;
    private ExpansionType _expansionType = ExpansionType.UseGpsSettings;
    private bool _expansionTypeChanged = false;
    private ForcePerLengthUnit _forcePerLengthUnit = DefaultUnits.ForcePerLengthUnit;

    public CreateGridLineLoad() : base("Create Grid Line Load", "LineLoad",
      "Create GSA Grid Line Load", CategoryName.Name(), SubCategoryName.Cat3()) {
      Hidden = true;
    }
    public override bool Read(GH_IReader reader) {
      if (reader.ItemExists("Mode")) {
        _expansionType = reader.TryGetEnum("Mode", typeof(ExpansionType), out int value) ?
          (ExpansionType)value : (ExpansionType)reader.GetInt32("Mode");
      }

      if (reader.ItemExists("ExpansionChanged")) {
        _expansionTypeChanged = reader.GetBoolean("ExpansionChanged");
      }

      return base.Read(reader);
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      _forcePerLengthUnit
        = (ForcePerLengthUnit)UnitsHelper.Parse(typeof(ForcePerLengthUnit), _selectedItems[i]);
      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      string unitAbbreviation = ForcePerLength.GetAbbreviation(_forcePerLengthUnit);
      Params.Input[7].Name = "Value Start [" + unitAbbreviation + "]";
      Params.Input[8].Name = "Value End [" + unitAbbreviation + "]";
    }

    public override bool Write(GH_IWriter writer) {
      writer.SetInt32("Mode", (int)_expansionType);
      writer.SetBoolean("ExpansionChanged", _expansionTypeChanged);
      return base.Write(writer);
    }

    protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu) {
      if (!(menu is ContextMenuStrip)) {
        return; // this method is also called when clicking EWR balloon
      }

      bool noGps = Params.Input[2].SourceCount == 0;
      Menu_AppendItem(menu, "Use GridPlaneSurface", SetUseGps, !noGps,
        _expansionType == ExpansionType.UseGpsSettings);
      Menu_AppendItem(menu, "Expand to 1D Elements", SetUse1D, noGps,
        _expansionType == ExpansionType.To1D);
      Menu_AppendItem(menu, "Expand to 2D Elements", SetUse2D, noGps,
        _expansionType == ExpansionType.To2D);
    }

    protected override void BeforeSolveInstance() {
      base.BeforeSolveInstance();
      UpdateMessage();
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Unit",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.ForcePerLength));
      _selectedItems.Add(ForcePerLength.GetAbbreviation(_forcePerLengthUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      string unitAbbreviation = ForcePerLength.GetAbbreviation(_forcePerLengthUnit);

      pManager.AddParameter(new GsaLoadCaseParameter());
      pManager.AddCurveParameter("PolyLine", "L",
        "PolyLine. If you input grid plane below only x and y coordinate positions will be used from this polyline, but if not a new Grid Plane Surface (best-fit plane) will be created from PolyLine control points.",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Grid Plane Surface", "GPS",
        "Grid Plane Surface or Plane [Optional]. If no input here then the line's best-fit plane will be used",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Direction", "Di",
        "Load direction (default z)." + Environment.NewLine + "Accepted inputs are:"
        + Environment.NewLine + "x" + Environment.NewLine + "y" + Environment.NewLine + "z",
        GH_ParamAccess.item, "z");
      pManager.AddIntegerParameter("Axis", "Ax",
        "Load axis (default Global). " + Environment.NewLine + "Accepted inputs are:"
        + Environment.NewLine + "0 : Global" + Environment.NewLine + "-1 : Local",
        GH_ParamAccess.item, 0);
      pManager.AddBooleanParameter("Projected", "Pj", "Projected (default not)",
        GH_ParamAccess.item, false);
      pManager.AddTextParameter("Name", "Na", "Load Name", GH_ParamAccess.item);
      pManager.AddGenericParameter("Value Start [" + unitAbbreviation + "]", "V1",
        "Load Value at Start of Line", GH_ParamAccess.item);
      pManager.AddGenericParameter("Value End [" + unitAbbreviation + "]", "V2",
        "Load Value at End of Line (default : Start Value)", GH_ParamAccess.item);

      pManager[0].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;
      pManager[4].Optional = true;
      pManager[5].Optional = true;
      pManager[6].Optional = true;
      pManager[8].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaLoadParameter(), "Grid Line Load", "Ld", "GSA Grid Line Load",
        GH_ParamAccess.item);
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      var gridlineload = new GsaGridLineLoad();

      var loadcase = new GsaLoadCase(1);
      GsaLoadCaseGoo loadCaseGoo = null;
      if (da.GetData(0, ref loadCaseGoo)) {
        if (loadCaseGoo.Value != null) {
          loadcase = loadCaseGoo.Value;
        }
      }

      gridlineload.LoadCase = loadcase;

      // Do plane input first as to see if we need to project polyline onto grid plane
      Plane plane = Plane.WorldXY;
      bool planeSet = false;
      var gridPlaneSurface = new GsaGridPlaneSurface();
      var ghTyp = new GH_ObjectWrapper();
      if (da.GetData(2, ref ghTyp)) {
        switch (ghTyp.Value) {
          case GsaGridPlaneSurfaceGoo gridplanesurfacegoo: {
              gridPlaneSurface = gridplanesurfacegoo.Value.Duplicate();
              plane = gridPlaneSurface.Plane;
              planeSet = true;
              _expansionType = ExpansionType.UseGpsSettings;
              UpdateMessage(gridPlaneSurface.GridSurface.ElementType
                == GridSurface.Element_Type.ONE_DIMENSIONAL ? "1D" : "2D");
              break;
            }
          case GH_Plane pln:
            plane = pln.Value;
            gridPlaneSurface = new GsaGridPlaneSurface(plane);
            planeSet = true;
            UpdateMessage(gridPlaneSurface.GridSurface.ElementType
                == GridSurface.Element_Type.ONE_DIMENSIONAL ? "1D" : "2D");
            break;

          default: {
              if (GH_Convert.ToInt32(ghTyp.Value, out int id, GH_Conversion.Both)) {
                gridlineload.ApiLoad.GridSurface = id;
                gridlineload.GridPlaneSurface = null;
              } else {
                this.AddRuntimeError(
                  "Error in GPS input. Accepted inputs are Grid Plane Surface or Plane. "
                  + Environment.NewLine
                  + "If no input here then the line's best-fit plane will be used");
                return;
              }

              break;
            }
        }
      }

      // we wait setting the gridplanesurface until we have run the polyline input
      var ghCurve = new GH_Curve();
      if (da.GetData(1, ref ghCurve)) {
        Curve curve = null;
        GH_Convert.ToCurve(ghCurve, ref curve, GH_Conversion.Both);

        if (curve.TryGetPolyline(out Rhino.Geometry.Polyline ln)) {
          var controlPoints = new Point3dList(ln);
          gridlineload.Points = controlPoints;

          if (!planeSet) {
            plane = RhinoConversions.CreateBestFitUnitisedPlaneFromPts(controlPoints);

            if (Params.Input[2].SourceCount == 0
              && _expansionType == ExpansionType.UseGpsSettings) {
              _expansionType = ExpansionType.To1D;
              this.AddRuntimeRemark(
                "Input Polyline plane has automatically been converted to a GridPlaneSurface."
                + Environment.NewLine + "The default expansion type is set to be onto 1D Elements."
                + Environment.NewLine + "You can change this by right-clicking the component.");
              UpdateMessage();
            }

            gridPlaneSurface = new GsaGridPlaneSurface(plane, true);
          } else {
            curve = Curve.ProjectToPlane(curve, plane);
            curve.TryGetPolyline(out ln);
            controlPoints = new Point3dList(ln);
          }

          gridlineload.ApiLoad.Type = GridLineLoad.PolyLineType.EXPLICIT_POLYLINE;
          string definition = GridLoadHelper.CreateDefinition(controlPoints, plane);
          gridlineload.ApiLoad.PolyLineDefinition = definition;
        } else {
          this.AddRuntimeError("Could not convert Curve to Polyline");
        }
      }

      if (gridlineload.GridPlaneSurface != null) {
        switch (_expansionType) {
          case ExpansionType.To1D:
            gridPlaneSurface.GridSurface.ElementType = GridSurface.Element_Type.ONE_DIMENSIONAL;
            break;

          case ExpansionType.To2D:
            gridPlaneSurface.GridSurface.ElementType = GridSurface.Element_Type.TWO_DIMENSIONAL;
            break;

          case ExpansionType.UseGpsSettings:
          default:
            break;
        }

        if (gridlineload.GridPlaneSurface.GridSurfaceId == 0) {
          gridlineload.GridPlaneSurface = gridPlaneSurface;
        }
      }

      string dir = "Z";
      Direction direc = Direction.Z;

      var ghDir = new GH_String();
      if (da.GetData(3, ref ghDir)) {
        GH_Convert.ToString(ghDir, out dir, GH_Conversion.Both);
      }

      dir = dir.ToUpper();
      switch (dir) {
        case "X":
          direc = Direction.X;
          break;

        case "Y":
          direc = Direction.Y;
          break;
      }

      gridlineload.ApiLoad.Direction = direc;

      gridlineload.ApiLoad.AxisProperty = 0;
      var ghAxis = new GH_Integer();
      if (da.GetData(4, ref ghAxis)) {
        GH_Convert.ToInt32(ghAxis, out int axis, GH_Conversion.Both);
        if (axis == 0 || axis == -1) {
          gridlineload.ApiLoad.AxisProperty = axis;
        }
      }

      var ghProj = new GH_Boolean();
      if (da.GetData(5, ref ghProj)) {
        if (GH_Convert.ToBoolean(ghProj, out bool proj, GH_Conversion.Both)) {
          gridlineload.ApiLoad.IsProjected = proj;
        }
      }

      var ghName = new GH_String();
      if (da.GetData(6, ref ghName)) {
        if (GH_Convert.ToString(ghName, out string name, GH_Conversion.Both)) {
          gridlineload.ApiLoad.Name = name;
        }
      }

      double load1 = ((ForcePerLength)Input.UnitNumber(this, da, 7, _forcePerLengthUnit))
       .NewtonsPerMeter;
      gridlineload.ApiLoad.ValueAtStart = load1;

      double load2 = load1;
      if (Params.Input[8].SourceCount > 0) {
        load2 = ((ForcePerLength)Input.UnitNumber(this, da, 8, _forcePerLengthUnit))
         .NewtonsPerMeter;
      }

      gridlineload.ApiLoad.ValueAtEnd = load2;

      da.SetData(0, new GsaLoadGoo(gridlineload));
    }

    protected override void UpdateUIFromSelectedItems() {
      _forcePerLengthUnit
        = (ForcePerLengthUnit)UnitsHelper.Parse(typeof(ForcePerLengthUnit), _selectedItems[0]);
      base.UpdateUIFromSelectedItems();
    }

    private void SetUse1D(object s, EventArgs e) {
      _expansionType = ExpansionType.To1D;
      UpdateMessage("1D");
      _expansionTypeChanged = true;
      base.UpdateUI();
    }

    private void SetUse2D(object s, EventArgs e) {
      _expansionType = ExpansionType.To2D;
      UpdateMessage("2D");
      _expansionTypeChanged = true;
      base.UpdateUI();
    }

    private void SetUseGps(object s, EventArgs e) {
      _expansionType = ExpansionType.UseGpsSettings;
      Message = string.Empty;
      _expansionTypeChanged = true;
      base.UpdateUI();
    }

    private void UpdateMessage(string type = "") {
      if (string.IsNullOrEmpty(type)) {
        if (_expansionType == ExpansionType.To1D) {
          Message = "Expand to 1D";
        } else if (_expansionType == ExpansionType.To2D) {
          Message = "Expand to 2D";
        }
      } else {
        Message = $"Expand to {type}";
      }
    }
  }
}
