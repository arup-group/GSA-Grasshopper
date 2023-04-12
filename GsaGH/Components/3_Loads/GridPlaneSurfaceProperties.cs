using System;
using System.Drawing;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Components {
  public class GridPlaneSurfaceProperties : GH_OasysComponent,
    IGH_VariableParameterComponent {
    protected override void SolveInstance(IGH_DataAccess da) {
      var gsaGridPlaneSurface = new GsaGridPlaneSurface();
      if (!da.GetData(0, ref gsaGridPlaneSurface))
        return;

      if (gsaGridPlaneSurface == null) {
        this.AddRuntimeWarning("Null GridPlaneSurface");
        return;
      }

      da.SetData(0,
        gsaGridPlaneSurface == null
          ? Plane.Unset
          : gsaGridPlaneSurface.Plane);
      da.SetData(1,
        gsaGridPlaneSurface.GridPlane == null
          ? 0
          : gsaGridPlaneSurface.GridPlaneId);
      da.SetData(2, gsaGridPlaneSurface.GridPlane?.Name);
      da.SetData(3, gsaGridPlaneSurface.GridPlane?.IsStoreyType ?? false);
      var axis = new Plane();
      if (gsaGridPlaneSurface.GridPlane != null) {
        axis = new Plane(gsaGridPlaneSurface.Plane);
        if (gsaGridPlaneSurface.Elevation != "0") {
          var elevation = new Length();
          try {
            elevation = Length.Parse(gsaGridPlaneSurface.Elevation);
          }
          catch (Exception) {
            if (double.TryParse(gsaGridPlaneSurface.Elevation, out double elev))
              elevation = new Length(elev, _lengthUnit);
          }

          axis.OriginZ -= elevation.As(_lengthUnit);
        }
      }

      da.SetData(4,
        gsaGridPlaneSurface.GridPlane == null
          ? Plane.Unset
          : axis);
      da.SetData(5, gsaGridPlaneSurface.AxisId);
      da.SetData(6,
        gsaGridPlaneSurface.GridPlane == null
          ? "0"
          : gsaGridPlaneSurface.Elevation);
      da.SetData(7,
        gsaGridPlaneSurface.GridPlane == null
          ? ""
          : gsaGridPlaneSurface.StoreyToleranceAbove);
      da.SetData(8,
        gsaGridPlaneSurface.GridPlane == null
          ? ""
          : gsaGridPlaneSurface.StoreyToleranceBelow);
      da.SetData(9, gsaGridPlaneSurface.GridSurfaceId);
      da.SetData(10, gsaGridPlaneSurface.GridSurface.Name);
      da.SetData(11, gsaGridPlaneSurface.GridSurface.Elements);
      string elemtype = gsaGridPlaneSurface.GridSurface.ElementType.ToString();
      da.SetData(12,
        char.ToUpper(elemtype[0])
        + elemtype.Substring(1)
          .ToLower()
          .Replace("_", " "));
      da.SetData(13, gsaGridPlaneSurface.Tolerance);
      string spantype = gsaGridPlaneSurface.GridSurface.SpanType.ToString();
      da.SetData(14,
        char.ToUpper(spantype[0])
        + spantype.Substring(1)
          .ToLower()
          .Replace("_", " "));
      da.SetData(15, gsaGridPlaneSurface.GridSurface.Direction);
      string expantype = gsaGridPlaneSurface.GridSurface.ExpansionType.ToString();
      da.SetData(16,
        char.ToUpper(expantype[0])
        + expantype.Substring(1)
          .ToLower()
          .Replace("_", " "));
      bool simple = gsaGridPlaneSurface.GridSurface.SpanType
        == GridSurface.Span_Type.TWO_WAY_SIMPLIFIED_TRIBUTARY_AREAS;
      da.SetData(17, simple);
    }

    #region Name and Ribbon Layout

    public override Guid ComponentGuid => new Guid("cb5c1d72-e414-447b-b5db-ce18d76e2f4d");
    public override GH_Exposure Exposure => GH_Exposure.quarternary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.GridPlaneProperties;

    public GridPlaneSurfaceProperties() : base("Grid Plane Surface Properties",
      "GridPlaneSurfaceProp",
      "Get GSA Grid Plane Surface Properties",
      CategoryName.Name(),
      SubCategoryName.Cat3()) { }

    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager)
      => pManager.AddParameter(new GsaGridPlaneParameter(),
        "Grid Plane Surface",
        "GPS",
        "Grid Plane Surface to get a bit more info out of.",
        GH_ParamAccess.item);

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddPlaneParameter("Grid Plane",
        "P",
        "Grid Plane (Axis + Elevation)",
        GH_ParamAccess.item); //0
      pManager.AddIntegerParameter("Grid Plane ID",
        "IdG",
        " Grid Plane ID",
        GH_ParamAccess.item); //1
      pManager.AddTextParameter("Grid Plane Name",
        "NaP",
        "Grid Plane Name",
        GH_ParamAccess.item); //2
      pManager.AddBooleanParameter("is Storey?",
        "St",
        "Grid Plane is Storey type",
        GH_ParamAccess.item); //3
      pManager.AddPlaneParameter("Axis", "Ax", "Grid Plane Axis as plane", GH_ParamAccess.item); //4
      pManager.AddIntegerParameter("Axis ID", "IdA", "Axis ID", GH_ParamAccess.item); //5
      pManager.AddGenericParameter("Elevation",
        "Ev",
        "Grid Plane Elevation",
        GH_ParamAccess.item); //6
      pManager.AddGenericParameter("Grid Plane Tolerance Above",
        "tA",
        "Grid Plane Tolerance Above (for Storey Type)",
        GH_ParamAccess.item); //7
      pManager.AddGenericParameter("Grid Plane Tolerance Below",
        "tB",
        "Grid Plane Tolerance Below (for Storey Type)",
        GH_ParamAccess.item); //8

      pManager.AddIntegerParameter("Grid Surface ID",
        "IdS",
        "Grid Surface ID",
        GH_ParamAccess.item); //9
      pManager.AddTextParameter("Grid Surface Name",
        "NaS",
        "Grid Surface Name",
        GH_ParamAccess.item); //10
      pManager.AddTextParameter("Elements",
        "El",
        "Elements that Grid Surface will try to expand load to",
        GH_ParamAccess.item); //11
      pManager.AddTextParameter("Element Type",
        "Ty",
        "Grid Surface Element Type",
        GH_ParamAccess.item); //12
      pManager.AddGenericParameter("Grid Surface Tolerance",
        "To",
        "Grid Surface Tolerance",
        GH_ParamAccess.item); //13
      pManager.AddTextParameter("Span Type",
        "Sp",
        "Grid Surface Span Type",
        GH_ParamAccess.item); //14
      pManager.AddNumberParameter("Span Direction",
        "Di",
        "Grid Surface Span Direction",
        GH_ParamAccess.item); //15
      pManager.AddTextParameter("Expansion Type",
        "Ex",
        "Grid Surface Expansion Type",
        GH_ParamAccess.item); //16
      pManager.AddBooleanParameter("Simplified Tributary Area",
        "Sf",
        "Grid Surface Simplified Tributary Area",
        GH_ParamAccess.item); //17
    }

    #endregion

    #region Custom UI

    protected override void BeforeSolveInstance() => Message = Length.GetAbbreviation(_lengthUnit);

    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitGeometry;

    public override void AppendAdditionalMenuItems(ToolStripDropDown menu) {
      Menu_AppendSeparator(menu);

      var unitsMenu = new ToolStripMenuItem("Select unit", Resources.Units) {
        Enabled = true,
        ImageScaling = ToolStripItemImageScaling.SizeToFit,
      };
      foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length)) {
        var toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => { Update(unit); }) {
          Checked = unit == Length.GetAbbreviation(_lengthUnit),
          Enabled = true,
        };

        unitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

      menu.Items.Add(unitsMenu);

      Menu_AppendSeparator(menu);
    }

    private void Update(string unit) {
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), unit);
      Message = unit;
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
    }

    public override bool Write(GH_IWriter writer) {
      writer.SetString("LengthUnit", _lengthUnit.ToString());
      return base.Write(writer);
    }

    public override bool Read(GH_IReader reader) {
      if (reader.ItemExists("LengthUnit"))
        _lengthUnit
          = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("LengthUnit"));
      else
        _lengthUnit = LengthUnit.Meter;
      return base.Read(reader);
    }

    #region IGH_VariableParameterComponent null implementation

    public virtual void VariableParameterMaintenance() {
      string unit = Length.GetAbbreviation(_lengthUnit);

      Params.Output[6]
        .Name = "Elevation [" + unit + "]";
      Params.Output[7]
        .Name = "Grid Plane Tolerance Above [" + unit + "]";
      Params.Output[8]
        .Name = "Grid Plane Tolerance Below [" + unit + "]";
      Params.Output[13]
        .Name = "Grid Surface Tolerance [" + unit + "]";
    }

    bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index)
      => false;

    bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index)
      => false;

    IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index)
      => null;

    bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index) => false;

    #endregion

    #endregion
  }
}
