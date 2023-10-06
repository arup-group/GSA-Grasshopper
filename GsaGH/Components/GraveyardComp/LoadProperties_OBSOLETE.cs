using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;
using OasysGH.Parameters;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Components {
  public class LoadProperties_OBSOLETE : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("0df96bee-3440-4699-b08d-d805220d1f68");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.LoadProperties;
    private ForceUnit _forceUnit = DefaultUnits.ForceUnit;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitGeometry;

    public LoadProperties_OBSOLETE() : base("Load Properties", "LoadProp", "Get properties of a GSA Load",
      CategoryName.Name(), SubCategoryName.Cat3()) {
      Hidden = true;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      switch (i) {
        case 0:
          _forceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), _selectedItems[0]);
          break;

        case 1:
          _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[1]);
          break;
      }

      _selectedItems[1] = Length.GetAbbreviation(_lengthUnit);
      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      try {
        string forceUnitAbbreviation = Force.GetAbbreviation(_forceUnit);
        string forcePerLengthUnit
          = ForcePerLength.GetAbbreviation(
            UnitsHelper.GetForcePerLengthUnit(_forceUnit, _lengthUnit));
        string forcePerAreaUnit
          = Pressure.GetAbbreviation(UnitsHelper.GetForcePerAreaUnit(_forceUnit, _lengthUnit));
        string unitAbbreviation = string.Join(", ", new {
          forceUnitAbbreviation,
          forcePerLengthUnit,
          forcePerAreaUnit,
        });

        Params.Output[6].Name = "Load Value or Factor X [" + unitAbbreviation + "]";
        Params.Output[7].Name = "Load Value or Factor X [" + unitAbbreviation + "]";
        Params.Output[8].Name = "Load Value or Factor X [" + unitAbbreviation + "]";
        Params.Output[9].Name = "Load Value [" + unitAbbreviation + "]";
      } catch (Exception e) {
        this.AddRuntimeError(e.Message);
      }
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Force Unit",
        "Length Unit",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Force));
      _dropDownItems[0].RemoveAt(_dropDownItems[0].Count - 1);
      _selectedItems.Add(Force.GetAbbreviation(_forceUnit));

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      _selectedItems.Add(Length.GetAbbreviation(_lengthUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaLoadParameter(), "Load", "Ld", "Load to get some info out of.",
        GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      string forceUnitAbbreviation = Force.GetAbbreviation(_forceUnit);
      string lengthUnitAbbreviation = Length.GetAbbreviation(_lengthUnit);
      string unitAbbreviation = forceUnitAbbreviation + "/" + lengthUnitAbbreviation;

      pManager.AddIntegerParameter("Load case", "LC", "Load case number", GH_ParamAccess.item);
      pManager.AddTextParameter("Name", "Na", "Load name", GH_ParamAccess.item);
      pManager.AddGenericParameter("Elements/Nodes/Definition", "Def",
        "Element/Node list that load is applied to or Grid point / polygon definition",
        GH_ParamAccess.item);
      pManager.AddIntegerParameter("Axis", "Ax", "Axis Property (0 : Global // -1 : Local",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Direction", "Di", "Load direction", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Projected", "Pj", "Projected", GH_ParamAccess.item);
      pManager.AddGenericParameter(
        "Load Value or Factor X [" + forceUnitAbbreviation + ", " + unitAbbreviation + "]", "V1",
        "Value at Start, Point 1 or Factor X", GH_ParamAccess.item);
      pManager.AddGenericParameter(
        "Load Value or Factor Y [" + forceUnitAbbreviation + ", " + unitAbbreviation + "]", "V2",
        "Value at End, Point 2 or Factor Y", GH_ParamAccess.item);
      pManager.AddGenericParameter(
        "Load Value or Factor Z [" + forceUnitAbbreviation + ", " + unitAbbreviation + "]", "V3",
        "Value at Point 3 or Factor Z", GH_ParamAccess.item);
      pManager.AddGenericParameter(
        "Load Value [" + forceUnitAbbreviation + ", " + unitAbbreviation + "]", "V4",
        "Value at Point 4", GH_ParamAccess.item);
      pManager.AddGenericParameter("Grid Plane Surface", "GPS", "Grid Plane Surface",
        GH_ParamAccess.item);
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      ForcePerLengthUnit forcePerLengthUnit
        = UnitsHelper.GetForcePerLengthUnit(_forceUnit, _lengthUnit);
      PressureUnit forcePerAreaUnit = UnitsHelper.GetForcePerAreaUnit(_forceUnit, _lengthUnit);

      var ghTyp = new GH_ObjectWrapper();
      if (!da.GetData(0, ref ghTyp)) {
        return;
      }

      if (ghTyp.Value is GsaLoadGoo loadGoo) {
        IGsaLoad gsaLoad = loadGoo.Value;
        switch (gsaLoad) {
          case GsaGravityLoad gravityLoad:
            da.SetData(0, gravityLoad.ApiLoad.Case);
            da.SetData(1, gravityLoad.ApiLoad.Name);
            da.SetData(2, gravityLoad.ApiLoad.EntityList);
            da.SetData(6, gravityLoad.ApiLoad.Factor.X);
            da.SetData(7, gravityLoad.ApiLoad.Factor.Y);
            da.SetData(8, gravityLoad.ApiLoad.Factor.Z);
            return;

          case GsaNodeLoad nodeLoad:
            da.SetData(0, nodeLoad.ApiLoad.Case);
            da.SetData(1, nodeLoad.ApiLoad.Name);
            da.SetData(2, nodeLoad.ApiLoad.Nodes);
            da.SetData(3, nodeLoad.ApiLoad.AxisProperty);
            da.SetData(4, nodeLoad.ApiLoad.Direction);
            var apiNodeForce = new Force(nodeLoad.ApiLoad.Value, ForceUnit.Newton);
            var outNodeForce = new Force(apiNodeForce.As(_forceUnit), _forceUnit);
            da.SetData(6, new GH_UnitNumber(outNodeForce));
            return;

          case GsaBeamLoad beamLoad:
            da.SetData(0, beamLoad.ApiLoad.Case);
            da.SetData(1, beamLoad.ApiLoad.Name);
            da.SetData(2, beamLoad.ApiLoad.EntityList);
            da.SetData(3, beamLoad.ApiLoad.AxisProperty);
            da.SetData(4, beamLoad.ApiLoad.Direction);
            da.SetData(5, beamLoad.ApiLoad.IsProjected);
            var apiBeamForce1 = new ForcePerLength(beamLoad.ApiLoad.Value(0),
              ForcePerLengthUnit.NewtonPerMeter);
            var outBeamForce1
              = new ForcePerLength(apiBeamForce1.As(forcePerLengthUnit), forcePerLengthUnit);
            da.SetData(6, new GH_UnitNumber(outBeamForce1));
            var apiBeamForce2 = new ForcePerLength(beamLoad.ApiLoad.Value(1),
              ForcePerLengthUnit.NewtonPerMeter);
            var outBeamForce2
              = new ForcePerLength(apiBeamForce2.As(forcePerLengthUnit), forcePerLengthUnit);
            da.SetData(7, new GH_UnitNumber(outBeamForce2));
            return;

          case GsaFaceLoad faceLoad:
            da.SetData(0, faceLoad.ApiLoad.Case);
            da.SetData(1, faceLoad.ApiLoad.Name);
            da.SetData(2, faceLoad.ApiLoad.EntityList);
            da.SetData(3, faceLoad.ApiLoad.AxisProperty);
            da.SetData(4, faceLoad.ApiLoad.Direction);
            da.SetData(5, faceLoad.ApiLoad.IsProjected);
            var apiFaceForce1 = new Pressure(faceLoad.ApiLoad.Value(0),
              PressureUnit.NewtonPerSquareMeter);
            var outFaceForce1 = new Pressure(apiFaceForce1.As(forcePerAreaUnit), forcePerAreaUnit);
            da.SetData(6, new GH_UnitNumber(outFaceForce1));
            var apiFaceForce2 = new Pressure(faceLoad.ApiLoad.Value(1),
              PressureUnit.NewtonPerSquareMeter);
            var outFaceForce2 = new Pressure(apiFaceForce2.As(forcePerAreaUnit), forcePerAreaUnit);
            da.SetData(7, new GH_UnitNumber(outFaceForce2));
            var apiFaceForce3 = new Pressure(faceLoad.ApiLoad.Value(2),
              PressureUnit.NewtonPerSquareMeter);
            var outFaceForce3 = new Pressure(apiFaceForce3.As(forcePerAreaUnit), forcePerAreaUnit);
            da.SetData(8, new GH_UnitNumber(outFaceForce3));
            var apiFaceForce4 = new Pressure(faceLoad.ApiLoad.Value(3),
              PressureUnit.NewtonPerSquareMeter);
            var outFaceForce4 = new Pressure(apiFaceForce4.As(forcePerAreaUnit), forcePerAreaUnit);
            da.SetData(9, new GH_UnitNumber(outFaceForce4));
            return;

          case GsaGridPointLoad gridPointLoad:
            da.SetData(0, gridPointLoad.ApiLoad.Case);
            da.SetData(1, gridPointLoad.ApiLoad.Name);
            da.SetData(2, new GH_Point(gridPointLoad.GetPoint(_lengthUnit)));
            da.SetData(3, gridPointLoad.ApiLoad.AxisProperty);
            da.SetData(4, gridPointLoad.ApiLoad.Direction);
            var apiPointForce = new Force(gridPointLoad.ApiLoad.Value, ForceUnit.Newton);
            var outPointForce = new Force(apiPointForce.As(_forceUnit), _forceUnit);
            da.SetData(6, new GH_UnitNumber(outPointForce));
            da.SetData(10, new GsaGridPlaneSurfaceGoo(gridPointLoad.GridPlaneSurface));
            return;

          case GsaGridLineLoad gridLineLoad:
            da.SetData(0, gridLineLoad.ApiLoad.Case);
            da.SetData(1, gridLineLoad.ApiLoad.Name);
            da.SetData(2, new Polyline(gridLineLoad.Points));
            da.SetData(3, gridLineLoad.ApiLoad.AxisProperty);
            da.SetData(4, gridLineLoad.ApiLoad.Direction);
            var apiLineForce1 = new ForcePerLength(gridLineLoad.ApiLoad.ValueAtStart,
              ForcePerLengthUnit.NewtonPerMeter);
            var outLineForce1
              = new ForcePerLength(apiLineForce1.As(forcePerLengthUnit), forcePerLengthUnit);
            da.SetData(6, new GH_UnitNumber(outLineForce1));
            var apiLineForce2 = new ForcePerLength(gridLineLoad.ApiLoad.ValueAtEnd,
              ForcePerLengthUnit.NewtonPerMeter);
            var outLineForce2
              = new ForcePerLength(apiLineForce2.As(forcePerLengthUnit), forcePerLengthUnit);
            da.SetData(7, new GH_UnitNumber(outLineForce2));
            da.SetData(10, new GsaGridPlaneSurfaceGoo(gridLineLoad.GridPlaneSurface));
            return;

          case GsaGridAreaLoad gridAreaLoad:
            da.SetData(0, gridAreaLoad.ApiLoad.Case);
            da.SetData(1, gridAreaLoad.ApiLoad.Name);
            var polyline = new Polyline(gridAreaLoad.Points);
            if (!polyline.IsClosed) {
              var pts = gridAreaLoad.Points.ToList();
              pts.Add(pts[0]);
              polyline = new Polyline(pts);
            }

            da.SetData(2, polyline);
            da.SetData(3, gridAreaLoad.ApiLoad.AxisProperty);
            da.SetData(4, gridAreaLoad.ApiLoad.Direction);
            var apiAreaForce = new Pressure(gridAreaLoad.ApiLoad.Value,
              PressureUnit.NewtonPerSquareMeter);
            var outAreaForce = new Pressure(apiAreaForce.As(forcePerAreaUnit), forcePerAreaUnit);
            da.SetData(6, new GH_UnitNumber(outAreaForce));
            da.SetData(10, new GsaGridPlaneSurfaceGoo(gridAreaLoad.GridPlaneSurface));
            return;
        }
      } else {
        this.AddRuntimeError("Error converting input to GSA Load");
      }
    }

    protected override void UpdateUIFromSelectedItems() {
      _forceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), _selectedItems[0]);
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[1]);
      base.UpdateUIFromSelectedItems();
    }
  }
}
