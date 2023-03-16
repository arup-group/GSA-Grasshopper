using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Parameters;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components {
  public class LoadProp : GH_OasysDropDownComponent {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("0df96bee-3440-4699-b08d-d805220d1f68");
    public override GH_Exposure Exposure => GH_Exposure.quarternary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.LoadInfo;

    public LoadProp() : base("Load Properties",
      "LoadProp",
      "Get properties of a GSA Load",
      CategoryName.Name(),
      SubCategoryName.Cat3()) {
        Hidden = true;
    } // sets the initial state of the component to hidden
    #endregion

    #region Input and Output
    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaLoadParameter(), "Load", "Ld", "Load to get some info out of.", GH_ParamAccess.item);
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      string forceUnitAbbreviation = Force.GetAbbreviation(_forceUnit);
      string lengthUnitAbbreviation = Length.GetAbbreviation(_lengthUnit);
      string unitAbbreviation = forceUnitAbbreviation + "/" + lengthUnitAbbreviation;

      pManager.AddIntegerParameter("Load case", "LC", "Load case number", GH_ParamAccess.item);
      pManager.AddTextParameter("Name", "Na", "Load name", GH_ParamAccess.item);
      pManager.AddGenericParameter("Elements/Nodes/Definition", "Def", "Element/Node list that load is applied to or Grid point / polygon definition", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Axis", "Ax", "Axis Property (0 : Global // -1 : Local", GH_ParamAccess.item);
      pManager.AddTextParameter("Direction", "Di", "Load direction", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Projected", "Pj", "Projected", GH_ParamAccess.item);
      pManager.AddGenericParameter("Load Value or Factor X [" + forceUnitAbbreviation + ", " + unitAbbreviation + "]", "V1", "Value at Start, Point 1 or Factor X", GH_ParamAccess.item);
      pManager.AddGenericParameter("Load Value or Factor Y [" + forceUnitAbbreviation + ", " + unitAbbreviation + "]", "V2", "Value at End, Point 2 or Factor Y", GH_ParamAccess.item);
      pManager.AddGenericParameter("Load Value or Factor Z [" + forceUnitAbbreviation + ", " + unitAbbreviation + "]", "V3", "Value at Point 3 or Factor Z", GH_ParamAccess.item);
      pManager.AddGenericParameter("Load Value [" + forceUnitAbbreviation + ", " + unitAbbreviation + "]", "V4", "Value at Point 4", GH_ParamAccess.item);
      pManager.AddGenericParameter("Grid Plane Surface", "GPS", "Grid Plane Surface", GH_ParamAccess.item);
    }
    #endregion
    protected override void SolveInstance(IGH_DataAccess da) {
      ForcePerLengthUnit forcePerLengthUnit = UnitsHelper.GetForcePerLengthUnit(_forceUnit, _lengthUnit);
      PressureUnit forcePerAreaUnit = UnitsHelper.GetForcePerAreaUnit(_forceUnit, _lengthUnit);

      // Get Loads input
      var ghTyp = new GH_ObjectWrapper();
      if (!da.GetData(0, ref ghTyp)) {
        return;
      }

      GsaLoad gsaLoad = null;
      if (ghTyp.Value is GsaLoadGoo) {
        ghTyp.CastTo(ref gsaLoad);
        switch (gsaLoad.LoadType) {
          case GsaLoad.LoadTypes.Gravity:
            da.SetData(0, gsaLoad.GravityLoad.GravityLoad.Case);
            da.SetData(1, gsaLoad.GravityLoad.GravityLoad.Name);
            da.SetData(2, gsaLoad.GravityLoad.GravityLoad.Elements);
            da.SetData(6, gsaLoad.GravityLoad.GravityLoad.Factor.X);
            da.SetData(7, gsaLoad.GravityLoad.GravityLoad.Factor.Y);
            da.SetData(8, gsaLoad.GravityLoad.GravityLoad.Factor.Z);
            return;
          case GsaLoad.LoadTypes.Node:
            da.SetData(0, gsaLoad.NodeLoad.NodeLoad.Case);
            da.SetData(1, gsaLoad.NodeLoad.NodeLoad.Name);
            da.SetData(2, gsaLoad.NodeLoad.NodeLoad.Nodes);
            da.SetData(3, gsaLoad.NodeLoad.NodeLoad.AxisProperty);
            da.SetData(4, gsaLoad.NodeLoad.NodeLoad.Direction);
            var apiNodeForce = new Force(gsaLoad.NodeLoad.NodeLoad.Value, ForceUnit.Newton);
            var outNodeForce = new Force(apiNodeForce.As(_forceUnit), _forceUnit);
            da.SetData(6, new GH_UnitNumber(outNodeForce));
            return;
          case GsaLoad.LoadTypes.Beam:
            da.SetData(0, gsaLoad.BeamLoad.BeamLoad.Case);
            da.SetData(1, gsaLoad.BeamLoad.BeamLoad.Name);
            da.SetData(2, gsaLoad.BeamLoad.BeamLoad.Elements);
            da.SetData(3, gsaLoad.BeamLoad.BeamLoad.AxisProperty);
            da.SetData(4, gsaLoad.BeamLoad.BeamLoad.Direction);
            da.SetData(5, gsaLoad.BeamLoad.BeamLoad.IsProjected);
            var apiBeamForce1 = new ForcePerLength(gsaLoad.BeamLoad.BeamLoad.Value(0), ForcePerLengthUnit.NewtonPerMeter);
            var outBeamForce1 = new ForcePerLength(apiBeamForce1.As(forcePerLengthUnit), forcePerLengthUnit);
            da.SetData(6, new GH_UnitNumber(outBeamForce1));
            var apiBeamForce2 = new ForcePerLength(gsaLoad.BeamLoad.BeamLoad.Value(1), ForcePerLengthUnit.NewtonPerMeter);
            var outBeamForce2 = new ForcePerLength(apiBeamForce2.As(forcePerLengthUnit), forcePerLengthUnit);
            da.SetData(7, new GH_UnitNumber(outBeamForce2));
            return;
          case GsaLoad.LoadTypes.Face:
            da.SetData(0, gsaLoad.FaceLoad.FaceLoad.Case);
            da.SetData(1, gsaLoad.FaceLoad.FaceLoad.Name);
            da.SetData(2, gsaLoad.FaceLoad.FaceLoad.Elements);
            da.SetData(3, gsaLoad.FaceLoad.FaceLoad.AxisProperty);
            da.SetData(4, gsaLoad.FaceLoad.FaceLoad.Direction);
            da.SetData(5, gsaLoad.FaceLoad.FaceLoad.IsProjected);
            var apiFaceForce1 = new Pressure(gsaLoad.FaceLoad.FaceLoad.Value(0), PressureUnit.NewtonPerSquareMeter);
            var outFaceForce1 = new Pressure(apiFaceForce1.As(forcePerAreaUnit), forcePerAreaUnit);
            da.SetData(6, new GH_UnitNumber(outFaceForce1));
            var apiFaceForce2 = new Pressure(gsaLoad.FaceLoad.FaceLoad.Value(1), PressureUnit.NewtonPerSquareMeter);
            var outFaceForce2 = new Pressure(apiFaceForce2.As(forcePerAreaUnit), forcePerAreaUnit);
            da.SetData(7, new GH_UnitNumber(outFaceForce2));
            var apiFaceForce3 = new Pressure(gsaLoad.FaceLoad.FaceLoad.Value(2), PressureUnit.NewtonPerSquareMeter);
            var outFaceForce3 = new Pressure(apiFaceForce3.As(forcePerAreaUnit), forcePerAreaUnit);
            da.SetData(8, new GH_UnitNumber(outFaceForce3));
            var apiFaceForce4 = new Pressure(gsaLoad.FaceLoad.FaceLoad.Value(3), PressureUnit.NewtonPerSquareMeter);
            var outFaceForce4 = new Pressure(apiFaceForce4.As(forcePerAreaUnit), forcePerAreaUnit);
            da.SetData(9, new GH_UnitNumber(outFaceForce4));
            return;
          case GsaLoad.LoadTypes.GridPoint:
            da.SetData(0, gsaLoad.PointLoad.GridPointLoad.Case);
            da.SetData(1, gsaLoad.PointLoad.GridPointLoad.Name);
            da.SetData(2, "(" + gsaLoad.PointLoad.GridPointLoad.X + "," + gsaLoad.PointLoad.GridPointLoad.Y + ")");
            da.SetData(3, gsaLoad.PointLoad.GridPointLoad.AxisProperty);
            da.SetData(4, gsaLoad.PointLoad.GridPointLoad.Direction);
            var apiPointForce = new Force(gsaLoad.PointLoad.GridPointLoad.Value, ForceUnit.Newton);
            var outPointForce = new Force(apiPointForce.As(_forceUnit), _forceUnit);
            da.SetData(6, new GH_UnitNumber(outPointForce));
            da.SetData(10, new GsaGridPlaneSurfaceGoo(gsaLoad.PointLoad.GridPlaneSurface));
            return;
          case GsaLoad.LoadTypes.GridLine:
            da.SetData(0, gsaLoad.LineLoad.GridLineLoad.Case);
            da.SetData(1, gsaLoad.LineLoad.GridLineLoad.Name);
            da.SetData(2, gsaLoad.LineLoad.GridLineLoad.PolyLineDefinition);
            da.SetData(3, gsaLoad.LineLoad.GridLineLoad.AxisProperty);
            da.SetData(4, gsaLoad.LineLoad.GridLineLoad.Direction);
            var apiLineForce1 = new ForcePerLength(gsaLoad.LineLoad.GridLineLoad.ValueAtStart, ForcePerLengthUnit.NewtonPerMeter);
            var outLineForce1 = new ForcePerLength(apiLineForce1.As(forcePerLengthUnit), forcePerLengthUnit);
            da.SetData(6, new GH_UnitNumber(outLineForce1));
            var apiLineForce2 = new ForcePerLength(gsaLoad.LineLoad.GridLineLoad.ValueAtEnd, ForcePerLengthUnit.NewtonPerMeter);
            var outLineForce2 = new ForcePerLength(apiLineForce2.As(forcePerLengthUnit), forcePerLengthUnit);
            da.SetData(7, new GH_UnitNumber(outLineForce2));
            da.SetData(10, new GsaGridPlaneSurfaceGoo(gsaLoad.LineLoad.GridPlaneSurface));
            return;
          case GsaLoad.LoadTypes.GridArea:
            da.SetData(0, gsaLoad.AreaLoad.GridAreaLoad.Case);
            da.SetData(1, gsaLoad.AreaLoad.GridAreaLoad.Name);
            da.SetData(2, gsaLoad.AreaLoad.GridAreaLoad.PolyLineDefinition);
            da.SetData(3, gsaLoad.AreaLoad.GridAreaLoad.AxisProperty);
            da.SetData(4, gsaLoad.AreaLoad.GridAreaLoad.Direction);
            var apiAreaForce = new Pressure(gsaLoad.AreaLoad.GridAreaLoad.Value, PressureUnit.NewtonPerSquareMeter);
            var outAreaForce = new Pressure(apiAreaForce.As(forcePerAreaUnit), forcePerAreaUnit);
            da.SetData(6, new GH_UnitNumber(outAreaForce));
            da.SetData(10, new GsaGridPlaneSurfaceGoo(gsaLoad.AreaLoad.GridPlaneSurface));
            return;
        }
      }
      else {
        this.AddRuntimeError("Error converting input to GSA Load");
      }
    }

    #region Custom UI
    private ForceUnit _forceUnit = DefaultUnits.ForceUnit;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitGeometry;
    public override void InitialiseDropdowns() {
      SpacerDescriptions = new List<string>(new []
        {
          "Force Unit",
          "Length Unit",
        });

      DropDownItems = new List<List<string>>();
      SelectedItems = new List<string>();

      // Force
      DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Force));
      DropDownItems[0].RemoveAt(DropDownItems[0].Count - 1); // remove Tonneforce
      SelectedItems.Add(Force.GetAbbreviation(_forceUnit));

      // Length
      DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      SelectedItems.Add(Length.GetAbbreviation(_lengthUnit));

      IsInitialised = true;
    }

    public override void SetSelected(int i, int j) {
      SelectedItems[i] = DropDownItems[i][j];
      switch (i) {
        case 0:
          _forceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), SelectedItems[0]);
          break;
        case 1:
          _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), SelectedItems[1]);
          break;
      }
      SelectedItems[1] = Length.GetAbbreviation(_lengthUnit);
      base.UpdateUI();
    }

    public override void UpdateUIFromSelectedItems() {
      _forceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), SelectedItems[0]);
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), SelectedItems[1]);
      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance() {
      try {
        string forceUnitAbbreviation = Force.GetAbbreviation(_forceUnit);
        string forcePerLengthUnit = ForcePerLength.GetAbbreviation(UnitsHelper.GetForcePerLengthUnit(_forceUnit, _lengthUnit));
        string forcePerAreaUnit = Pressure.GetAbbreviation(UnitsHelper.GetForcePerAreaUnit(_forceUnit, _lengthUnit));
        string unitAbbreviation = string.Join(", ", new { forceUnitAbbreviation, forcePerLengthUnit, forcePerAreaUnit });

        Params.Output[6].Name = "Load Value or Factor X [" + unitAbbreviation + "]";
        Params.Output[7].Name = "Load Value or Factor X [" + unitAbbreviation + "]";
        Params.Output[8].Name = "Load Value or Factor X [" + unitAbbreviation + "]";
        Params.Output[9].Name = "Load Value [" + unitAbbreviation + "]";
      }
      catch (Exception e) {
        this.AddRuntimeError(e.Message);
      }
    }
    #endregion
  }
}
