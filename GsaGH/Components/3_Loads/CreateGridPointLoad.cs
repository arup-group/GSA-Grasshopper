using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using GH_IO.Serialization;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Units;
using OasysGH.Units.Helpers;

using OasysUnits;

using Rhino.Geometry;

using ExpansionType = GsaGH.Parameters.ExpansionType;
using ForceUnit = OasysUnits.Units.ForceUnit;

namespace GsaGH.Components {
  public class CreateGridPointLoad : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("076f03c6-67ba-49d3-9462-cd4a4b5aff92");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateGridPointLoad;
    private ExpansionType _expansionType = ExpansionType.UseGpsSettings;
    private bool _expansionTypeChanged = false;
    private ForceUnit _forceUnit = DefaultUnits.ForceUnit;

    public CreateGridPointLoad() : base("Create Grid Point Load", "PointLoad",
      "Create GSA Grid Point Load", CategoryName.Name(), SubCategoryName.Cat3()) {
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
      _forceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), _selectedItems[i]);
      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      string unitAbbreviation = Force.GetAbbreviation(_forceUnit);
      Params.Input[6].Name = "Value [" + unitAbbreviation + "]";
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

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Force));
      _selectedItems.Add(Force.GetAbbreviation(_forceUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      string unitAbbreviation = Force.GetAbbreviation(_forceUnit);

      pManager.AddParameter(new GsaLoadCaseParameter());
      pManager.AddPointParameter("Point", "Pt",
        "Point. If you input grid plane below only x and y coordinates will be used from this point, but if not a new Grid Plane Surface (xy-plane) will be created at the z-elevation of this point.",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Grid Plane Surface", "GPS",
        "Grid Plane Surface or Plane [Optional]. If no input here then the point's z-coordinate will be used for an xy-plane at that elevation.",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Direction", "Di",
        "Load direction (default z)." + Environment.NewLine + "Accepted inputs are:"
        + Environment.NewLine + "x" + Environment.NewLine + "y" + Environment.NewLine + "z",
        GH_ParamAccess.item, "z");
      pManager.AddIntegerParameter("Axis", "Ax",
        "Load axis (default Global). " + Environment.NewLine + "Accepted inputs are:"
        + Environment.NewLine + "0 : Global" + Environment.NewLine + "-1 : Local",
        GH_ParamAccess.item, 0);
      pManager.AddTextParameter("Name", "Na", "Load Name", GH_ParamAccess.item);
      pManager.AddGenericParameter("Value [" + unitAbbreviation + "]", "V", "Load Value",
        GH_ParamAccess.item);

      pManager[0].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;
      pManager[4].Optional = true;
      pManager[5].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaLoadParameter(), "Grid Point Load", "Ld", "GSA Grid Point Load",
        GH_ParamAccess.item);
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      var gridPointLoad = new GsaGridPointLoad();

      var loadcase = new GsaLoadCase(1);
      GsaLoadCaseGoo loadCaseGoo = null;
      if (da.GetData(0, ref loadCaseGoo)) {
        if (loadCaseGoo.Value != null) {
          loadcase = loadCaseGoo.Value;
        }
      }

      gridPointLoad.LoadCase = loadcase;

      var point3d = new Point3d();
      var ghPt = new GH_Point();
      if (da.GetData(1, ref ghPt)) {
        GH_Convert.ToPoint3d(ghPt, ref point3d, GH_Conversion.Both);
      }

      GsaGridPlaneSurface gridPlaneSurface;
      Plane plane = Plane.WorldXY;
      var ghTyp = new GH_ObjectWrapper();
      if (da.GetData(2, ref ghTyp)) {
        switch (ghTyp.Value) {
          case GsaGridPlaneSurfaceGoo gridplanesurfacegoo:
            gridPlaneSurface = gridplanesurfacegoo.Value.Duplicate();
            gridPointLoad.GridPlaneSurface = gridPlaneSurface;
            _expansionType = ExpansionType.UseGpsSettings;
            UpdateMessage(gridPlaneSurface.GridSurface.ElementType
              == GsaAPI.GridSurface.Element_Type.ONE_DIMENSIONAL ? "1D" : "2D");
            break;

          case GH_Plane pln:
            plane = pln.Value;
            gridPlaneSurface = new GsaGridPlaneSurface(plane);
            gridPointLoad.GridPlaneSurface = gridPlaneSurface;
            UpdateMessage(gridPlaneSurface.GridSurface.ElementType
                == GsaAPI.GridSurface.Element_Type.ONE_DIMENSIONAL ? "1D" : "2D");
            break;

          default:
            if (GH_Convert.ToInt32(ghTyp.Value, out int id, GH_Conversion.Both)) {
              gridPointLoad.ApiLoad.GridSurface = id;
              gridPointLoad.GridPlaneSurface = null;
            } else {
              this.AddRuntimeError(
                "Error in GPS input. Accepted inputs are Grid Plane Surface or Plane. "
                + Environment.NewLine
                + "If no input here then the point's z-coordinate will be used for an xy-plane at that elevation");
              return;
            }

            break;
        }
      } else {
        plane = Plane.WorldXY;
        plane.Origin = new Point3d(0, 0, point3d.Z);
        gridPlaneSurface = new GsaGridPlaneSurface(plane, true);
        gridPointLoad.GridPlaneSurface = gridPlaneSurface;

        if (!_expansionTypeChanged && _expansionType == ExpansionType.UseGpsSettings) {
          _expansionType = ExpansionType.To1D;
          this.AddRuntimeRemark(
            "Input Point has automatically been converted to a GridPlaneSurface."
            + Environment.NewLine + "The default expansion type is set to be onto 1D Elements."
            + Environment.NewLine + "You can change this by right-clicking the component.");
          UpdateMessage();
        }

        switch (_expansionType) {
          case ExpansionType.To1D:
            gridPlaneSurface.GridSurface.ElementType =
              GsaAPI.GridSurface.Element_Type.ONE_DIMENSIONAL;
            break;

          case ExpansionType.To2D:
            gridPlaneSurface.GridSurface.ElementType =
              GsaAPI.GridSurface.Element_Type.TWO_DIMENSIONAL;
            break;

          case ExpansionType.UseGpsSettings:
          default:
            break;
        }
      }

      var plnNormal = new Vector3d(plane.Normal);
      plnNormal.Unitize();
      if (plnNormal.Z != 1) {
        this.AddRuntimeRemark("The grid plane basis is not defined in world coordinates. \n" +
          "The input point´s X and Y coordinates are use as the grid plane´s local coordiantes.");
      }

      gridPointLoad.ApiLoad.X = point3d.X;
      gridPointLoad.ApiLoad.Y = point3d.Y;

      string dir = "Z";
      GsaAPI.Direction direc = GsaAPI.Direction.Z;

      var ghDir = new GH_String();
      if (da.GetData(3, ref ghDir)) {
        GH_Convert.ToString(ghDir, out dir, GH_Conversion.Both);
      }

      dir = dir.ToUpper();
      switch (dir) {
        case "X":
          direc = GsaAPI.Direction.X;
          break;

        case "Y":
          direc = GsaAPI.Direction.Y;
          break;
      }

      gridPointLoad.ApiLoad.Direction = direc;

      gridPointLoad.ApiLoad.AxisProperty = 0;
      var ghAx = new GH_Integer();
      if (da.GetData(4, ref ghAx)) {
        GH_Convert.ToInt32(ghAx, out int axis, GH_Conversion.Both);
        if (axis == 0 || axis == -1) {
          gridPointLoad.ApiLoad.AxisProperty = axis;
        }
      }

      var ghName = new GH_String();
      if (da.GetData(5, ref ghName)) {
        if (GH_Convert.ToString(ghName, out string name, GH_Conversion.Both)) {
          gridPointLoad.ApiLoad.Name = name;
        }
      }

      gridPointLoad.ApiLoad.Value
        = ((Force)Input.UnitNumber(this, da, 6, _forceUnit)).Newtons;

      da.SetData(0, new GsaLoadGoo(gridPointLoad));
    }

    protected override void UpdateUIFromSelectedItems() {
      _forceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), _selectedItems[0]);
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
