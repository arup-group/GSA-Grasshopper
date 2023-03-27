using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
using Rhino.Geometry;

namespace GsaGH.Components {
  public class CreateGridAreaLoad : GH_OasysDropDownComponent {
    protected override void SolveInstance(IGH_DataAccess da) {
      var gridareaload = new GsaGridAreaLoad();
      int loadCase = 1;
      var ghLc = new GH_Integer();
      if (da.GetData(0, ref ghLc))
        GH_Convert.ToInt32(ghLc, out loadCase, GH_Conversion.Both);
      gridareaload.GridAreaLoad.Case = loadCase;

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
            _expansionType = ExpansionType.UseGpsSettings;
            UpdateMessage();
            break;
          }
          case Plane _:
            ghTyp.CastTo(ref plane);
            gridPlaneSurface = new GsaGridPlaneSurface(plane);
            planeSet = true;
            break;
          default: {
            if (GH_Convert.ToInt32(ghTyp.Value, out int id, GH_Conversion.Both)) {
              gridareaload.GridAreaLoad.GridSurface = id;
              gridareaload.GridPlaneSurface = null;
            }
            else {
              this.AddRuntimeError(
                "Error in GPS input. Accepted inputs are Grid Plane Surface or Plane. "
                + Environment.NewLine
                + "If no input here then the brep's best-fit plane will be used");
              return;
            }

            break;
          }
        }

      var brep = new Brep();

      var ghBrep = new GH_Brep();
      if (da.GetData(1, ref ghBrep)) {
        GH_Convert.ToBrep(ghBrep, ref brep, GH_Conversion.Both);

        Curve[] edgeSegments = brep.DuplicateEdgeCurves();
        Curve[] edges = Curve.JoinCurves(edgeSegments);
        Curve curve = edges[0];

        if (curve.TryGetPolyline(out Polyline polyline)) {
          var ctrlPts = polyline.ToList();

          if (!planeSet) {
            plane = RhinoConversions.CreateBestFitUnitisedPlaneFromPts(ctrlPts);

            if (Params.Input[2]
                .SourceCount
              == 0
              && _expansionType == ExpansionType.UseGpsSettings) {
              _expansionType = ExpansionType.To1D;
              this.AddRuntimeRemark(
                "Input Brep has automatically been converted to a GridPlaneSurface."
                + Environment.NewLine
                + "The default expansion type is set to be onto 1D Elements."
                + Environment.NewLine
                + "You can change this by right-clicking the component.");
              UpdateMessage();
            }

            gridPlaneSurface = new GsaGridPlaneSurface(plane, true);
          }

          curve = Curve.ProjectToPlane(curve, plane);
          curve.TryGetPolyline(out polyline);
          ctrlPts = polyline.ToList();
          string desc = "";
          for (int i = 0; i < ctrlPts.Count - 1; i++) {
            if (i > 0)
              desc += " ";

            plane.RemapToPlaneSpace(ctrlPts[i], out Point3d temppt);

            // format accepted by GSA: (0,0) (0,1) (1,2) (3,4) (4,0)(m)
            desc += "(" + temppt.X + "," + temppt.Y + ")";
          }

          gridareaload.GridAreaLoad.Type = GridAreaPolyLineType.POLYGON;
          gridareaload.GridAreaLoad.PolyLineDefinition = desc;
        }
        else
          this.AddRuntimeError("Could not convert Brep edge to Polyline");
      }

      if (gridareaload.GridPlaneSurface != null) {
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

        if (gridareaload.GridPlaneSurface.GridSurfaceId == 0)
          gridareaload.GridPlaneSurface = gridPlaneSurface;
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

      gridareaload.GridAreaLoad.Direction = direc;
      gridareaload.GridAreaLoad.AxisProperty = 0;
      var ghAxis = new GH_Integer();
      if (da.GetData(4, ref ghAxis)) {
        GH_Convert.ToInt32(ghAxis, out int axis, GH_Conversion.Both);
        if (axis == 0 || axis == -1)
          gridareaload.GridAreaLoad.AxisProperty = axis;
      }

      var ghProj = new GH_Boolean();
      if (da.GetData(5, ref ghProj))
        if (GH_Convert.ToBoolean(ghProj, out bool proj, GH_Conversion.Both))
          gridareaload.GridAreaLoad.IsProjected = proj;

      var ghName = new GH_String();
      if (da.GetData(6, ref ghName))
        if (GH_Convert.ToString(ghName, out string name, GH_Conversion.Both))
          gridareaload.GridAreaLoad.Name = name;

      gridareaload.GridAreaLoad.Value = ((Pressure)Input.UnitNumber(this, da, 7, _forcePerAreaUnit))
        .NewtonsPerSquareMeter;

      var gsaLoad = new GsaLoad(gridareaload);
      da.SetData(0, new GsaLoadGoo(gsaLoad));
    }

    #region Name and Ribbon Layout

    public override Guid ComponentGuid => new Guid("146f1bf8-8d2b-468f-bdb8-0237bee75262");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.AreaLoad;

    public CreateGridAreaLoad() : base("Create Grid Area Load",
      "AreaLoad",
      "Create GSA Grid Area Load",
      CategoryName.Name(),
      SubCategoryName.Cat3())
      => Hidden = true;

    #endregion

    #region input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      string unitAbbreviation = Pressure.GetAbbreviation(_forcePerAreaUnit);

      pManager.AddIntegerParameter("Load case",
        "LC",
        "Load case number (default 1)",
        GH_ParamAccess.item,
        1);
      pManager.AddBrepParameter("Brep",
        "B",
        "(Optional) Brep. If no input the whole plane method will be used. If both Grid Plane Surface and Brep are inputted, this Brep will be projected onto the Grid Plane.",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Grid Plane Surface",
        "GPS",
        "Grid Plane Surface or Plane (optional). If no input here then the brep's best-fit plane will be used",
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
      pManager.AddNumberParameter("Value [" + unitAbbreviation + "]",
        "V",
        "Load Value",
        GH_ParamAccess.item);

      pManager[0]
        .Optional = true;
      pManager[1]
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
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
      => pManager.AddParameter(new GsaLoadParameter(),
        "Grid Area Load",
        "Ld",
        "GSA Grid Area Load",
        GH_ParamAccess.item);

    #endregion

    #region Custom UI

    private PressureUnit _forcePerAreaUnit = DefaultUnits.ForcePerAreaUnit;

    private enum ExpansionType {
      UseGpsSettings = 0,
      To1D = 1,
      To2D = 2,
    }

    private ExpansionType _expansionType = ExpansionType.UseGpsSettings;

    protected override void BeforeSolveInstance() {
      base.BeforeSolveInstance();
      UpdateMessage();
    }

    public override void InitialiseDropdowns() {
      SpacerDescriptions = new List<string>(new[] {
        "Unit",
      });

      DropDownItems = new List<List<string>>();
      SelectedItems = new List<string>();

      DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations((EngineeringUnits.ForcePerArea)));
      SelectedItems.Add(Pressure.GetAbbreviation(_forcePerAreaUnit));

      IsInitialised = true;
    }

    public override void SetSelected(int i, int j) {
      SelectedItems[i] = DropDownItems[i][j];
      _forcePerAreaUnit = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), SelectedItems[i]);
      base.UpdateUI();
    }

    public override void UpdateUIFromSelectedItems() {
      _forcePerAreaUnit = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), SelectedItems[0]);
      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance() {
      string unitAbbreviation = Pressure.GetAbbreviation(_forcePerAreaUnit);
      Params.Input[7]
        .Name = "Value [" + unitAbbreviation + "]";
    }

    protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu) {
      bool noGps = Params.Input[2]
          .SourceCount
        == 0;
      Menu_AppendItem(menu,
        "Use GridPlaneSurface",
        SetUseGps,
        !noGps,
        _expansionType == ExpansionType.UseGpsSettings);
      Menu_AppendItem(menu,
        "Expand to 1D Elements",
        SetUse1D,
        noGps,
        _expansionType == ExpansionType.To1D);
      Menu_AppendItem(menu,
        "Expand to 2D Elements",
        SetUse2D,
        noGps,
        _expansionType == ExpansionType.To2D);
    }

    private void UpdateMessage()
      => Message = "Expansion: "
        + _expansionType.ToString()
          .Replace("_", " ");

    private void SetUseGps(object s, EventArgs e) {
      _expansionType = ExpansionType.UseGpsSettings;
      UpdateMessage();
      base.UpdateUI();
    }

    private void SetUse1D(object s, EventArgs e) {
      _expansionType = ExpansionType.To1D;
      UpdateMessage();
      base.UpdateUI();
    }

    private void SetUse2D(object s, EventArgs e) {
      _expansionType = ExpansionType.To2D;
      UpdateMessage();
      base.UpdateUI();
    }

    #endregion

    #region deserialization

    public override bool Write(GH_IWriter writer) {
      writer.SetString("Mode", _expansionType.ToString());
      return base.Write(writer);
    }

    public override bool Read(GH_IReader reader) {
      if (reader.ItemExists("Mode"))
        _expansionType = (ExpansionType)Enum.Parse(typeof(ExpansionType), reader.GetString("Mode"));
      return base.Read(reader);
    }

    #endregion
  }
}
