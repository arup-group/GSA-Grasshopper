using System;
using System.Collections.Generic;
using System.Drawing;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
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
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Components {
  // ReSharper disable once InconsistentNaming
  public class CreateGridPlane_OBSOLETE : GH_OasysDropDownComponent {
    private enum FoldMode {
      General,
      Storey,
    }

    public override Guid ComponentGuid => new Guid("675fd47a-890d-45b8-bdde-fb2e8c1d9cca");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.GridPlane;
    private readonly List<string> _type = new List<string>(new string[] {
      "General",
      "Storey",
    });
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitGeometry;
    private FoldMode _mode = FoldMode.General;

    public CreateGridPlane_OBSOLETE() : base("Create Grid Plane",
                      "GridPlane",
      "Create GSA Grid Plane",
      CategoryName.Name(),
      SubCategoryName.Cat3()) { }

    public override bool Read(GH_IReader reader) {
      if (reader.ItemExists("Mode")) {
        _mode = (FoldMode)reader.GetInt32("Mode");
        InitialiseDropdowns();
      }

      if (_mode != FoldMode.Storey || Params.Input.Count >= 5)
        return base.Read(reader);

      Params.RegisterInputParam(new Param_GenericObject());
      Params.RegisterInputParam(new Param_GenericObject());

      return base.Read(reader);
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      if (i == 0)
        switch (_selectedItems[0]) {
          case "General":
            Mode1Clicked();
            break;

          case "Storey":
            Mode2Clicked();
            break;
        }
      else
        _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[i]);

      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      Params.Input[2]
        .Name = "Grid Elevation [" + Length.GetAbbreviation(_lengthUnit) + "]";

      if (_mode != FoldMode.Storey)
        return;

      if (Params.Input.Count < 5) {
        Params.RegisterInputParam(new Param_GenericObject());
        Params.RegisterInputParam(new Param_GenericObject());
      }

      Params.Input[4]
        .NickName = "tA";
      Params.Input[4]
        .Name = "Tolerance Above [" + Length.GetAbbreviation(_lengthUnit) + "]";
      Params.Input[4]
        .Description = "Tolerance Above Grid Plane";
      Params.Input[4]
        .Access = GH_ParamAccess.item;
      Params.Input[4]
        .Optional = true;

      Params.Input[5]
        .NickName = "tB";
      Params.Input[5]
        .Name = "Tolerance Below [" + Length.GetAbbreviation(_lengthUnit) + "]";
      Params.Input[5]
        .Description = "Tolerance Below Grid Plane";
      Params.Input[5]
        .Access = GH_ParamAccess.item;
      Params.Input[5]
        .Optional = true;
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new string[] {
        "Type",
        "Unit",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(_type);
      _selectedItems.Add(_mode.ToString());

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      _selectedItems.Add(Length.GetAbbreviation(_lengthUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddGenericParameter("Plane",
        "P",
        "Plane for Axis and Grid Plane definition. Note that an XY-plane will be created with an axis origin Z = 0 "
        + "and the height location will be controlled by Grid Plane elevation. For all none-XY plane inputs, the Grid Plane elevation will be 0",
        GH_ParamAccess.item);
      pManager.AddIntegerParameter("Grid Plane ID",
        "ID",
        "GSA Grid Plane ID. Setting this will replace any existing Grid Planes in GSA model",
        GH_ParamAccess.item,
        0);
      pManager.AddGenericParameter("Grid Elevation [" + Length.GetAbbreviation(_lengthUnit) + "]",
        "Ev",
        "Grid Elevation (Optional). Note that this value will be added to Plane origin location in the plane's normal axis direction.",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Name", "Na", "Grid Plane Name", GH_ParamAccess.item);

      pManager[0]
        .Optional = true;
      pManager[1]
        .Optional = true;
      pManager[2]
        .Optional = true;
      pManager[3]
        .Optional = true;

      _mode = FoldMode.General;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
      => pManager.AddParameter(new GsaGridPlaneParameter(),
        "Grid Plane",
        "GP",
        "GSA Grid Plane",
        GH_ParamAccess.item);

    protected override void SolveInstance(IGH_DataAccess da) {
      Plane plane = Plane.WorldXY;
      var ghPlane = new GH_Plane();
      if (da.GetData(0, ref ghPlane))
        GH_Convert.ToPlane(ghPlane, ref plane, GH_Conversion.Both);

      var gsaGridPlaneSurface = new GsaGridPlaneSurface(plane);

      var ghint = new GH_Integer();
      if (da.GetData(1, ref ghint)) {
        GH_Convert.ToInt32(ghint, out int id, GH_Conversion.Both);
        gsaGridPlaneSurface.GridPlaneId = id;
      }

      if (Params.Input[2]
          .SourceCount
        > 0) {
        double elev = Input.UnitNumber(this, da, 2, _lengthUnit, true)
          .As(LengthUnit.Meter);
        gsaGridPlaneSurface.GridPlane.Elevation = elev;

        Vector3d vec = plane.Normal;
        vec.Unitize();
        vec.X *= elev;
        vec.Y *= elev;
        vec.Z *= elev;
        var xform = Transform.Translation(vec);
        plane.Transform(xform);
        gsaGridPlaneSurface.Plane = plane;
      }

      var ghtxt = new GH_String();
      if (da.GetData(3, ref ghtxt))
        if (GH_Convert.ToString(ghtxt, out string name, GH_Conversion.Both))
          gsaGridPlaneSurface.GridPlane.Name = name;

      if (_mode == FoldMode.General)
        gsaGridPlaneSurface.GridPlane.IsStoreyType = false;
      else {
        gsaGridPlaneSurface.GridPlane.IsStoreyType = true;

        if (Params.Input[4]
            .SourceCount
          > 0)
          gsaGridPlaneSurface.GridPlane.ToleranceAbove = Input
            .UnitNumber(this, da, 4, _lengthUnit, true)
            .As(LengthUnit.Meter);

        if (Params.Input[5]
            .SourceCount
          > 0)
          gsaGridPlaneSurface.GridPlane.ToleranceBelow = Input
            .UnitNumber(this, da, 5, _lengthUnit, true)
            .As(LengthUnit.Meter);
      }

      da.SetData(0, new GsaGridPlaneSurfaceGoo(gsaGridPlaneSurface));
    }

    protected override void UpdateUIFromSelectedItems() {
      _mode = (FoldMode)Enum.Parse(typeof(FoldMode), _selectedItems[0]);
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[1]);
      base.UpdateUIFromSelectedItems();
    }

    private void Mode1Clicked() {
      if (_mode == FoldMode.General)
        return;

      RecordUndoEvent("General Parameters");
      _mode = FoldMode.General;

      while (Params.Input.Count > 4)
        Params.UnregisterInputParameter(Params.Input[4], true);
    }

    private void Mode2Clicked() {
      if (_mode == FoldMode.Storey)
        return;

      RecordUndoEvent("Storey Parameters");
      _mode = FoldMode.Storey;

      Params.RegisterInputParam(new Param_GenericObject());
      Params.RegisterInputParam(new Param_GenericObject());
    }
  }
}
