using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Parameters;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components
{
  public class LoadProp : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("0df96bee-3440-4699-b08d-d805220d1f68");
    public override GH_Exposure Exposure => GH_Exposure.quarternary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.LoadInfo;

    public LoadProp() : base("Load Properties",
      "LoadProp",
      "Get properties of a GSA Load",
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat3())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and Output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddParameter(new GsaLoadParameter(), "Load", "Ld", "Load to get some info out of.", GH_ParamAccess.item);
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      string forceUnitAbbreviation = Force.GetAbbreviation(this.ForceUnit);
      string lengthUnitAbbreviation = Length.GetAbbreviation(this.LengthUnit);
      string unitAbbreviation = forceUnitAbbreviation + "/" + lengthUnitAbbreviation;

      pManager.AddIntegerParameter("Load case", "LC", "Load case number)", GH_ParamAccess.item);
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
    protected override void SolveInstance(IGH_DataAccess DA)
    {
      ForcePerLengthUnit forcePerLengthUnit = UnitsHelper.GetForcePerLengthUnit(this.ForceUnit, this.LengthUnit);
      PressureUnit forcePerAreaUnit = UnitsHelper.GetForcePerAreaUnit(this.ForceUnit, this.LengthUnit);

      // Get Loads input
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(0, ref gh_typ))
      {
        GsaLoad gsaload = null;
        if (gh_typ.Value is GsaLoadGoo)
        {
          gh_typ.CastTo(ref gsaload);
          switch (gsaload.LoadType)
          {
            case GsaLoad.LoadTypes.Gravity:
              DA.SetData(0, gsaload.GravityLoad.GravityLoad.Case);
              DA.SetData(1, gsaload.GravityLoad.GravityLoad.Name);
              DA.SetData(2, gsaload.GravityLoad.GravityLoad.Elements);
              DA.SetData(6, gsaload.GravityLoad.GravityLoad.Factor.X);
              DA.SetData(7, gsaload.GravityLoad.GravityLoad.Factor.Y);
              DA.SetData(8, gsaload.GravityLoad.GravityLoad.Factor.Z);
              return;
            case GsaLoad.LoadTypes.Node:
              DA.SetData(0, gsaload.NodeLoad.NodeLoad.Case);
              DA.SetData(1, gsaload.NodeLoad.NodeLoad.Name);
              DA.SetData(2, gsaload.NodeLoad.NodeLoad.Nodes);
              DA.SetData(3, gsaload.NodeLoad.NodeLoad.AxisProperty);
              DA.SetData(4, gsaload.NodeLoad.NodeLoad.Direction);
              Force apiNodeForce = new Force(gsaload.NodeLoad.NodeLoad.Value, ForceUnit.Newton);
              Force outNodeForce = new Force(apiNodeForce.As(ForceUnit), ForceUnit);
              DA.SetData(6, new GH_UnitNumber(outNodeForce));
              return;
            case GsaLoad.LoadTypes.Beam:
              DA.SetData(0, gsaload.BeamLoad.BeamLoad.Case);
              DA.SetData(1, gsaload.BeamLoad.BeamLoad.Name);
              DA.SetData(2, gsaload.BeamLoad.BeamLoad.Elements);
              DA.SetData(3, gsaload.BeamLoad.BeamLoad.AxisProperty);
              DA.SetData(4, gsaload.BeamLoad.BeamLoad.Direction);
              DA.SetData(5, gsaload.BeamLoad.BeamLoad.IsProjected);
              ForcePerLength apiBeamForce1 = new ForcePerLength(gsaload.BeamLoad.BeamLoad.Value(0), ForcePerLengthUnit.NewtonPerMeter);
              ForcePerLength outBeamForce1 = new ForcePerLength(apiBeamForce1.As(forcePerLengthUnit), forcePerLengthUnit);
              DA.SetData(6, new GH_UnitNumber(outBeamForce1));
              ForcePerLength apiBeamForce2 = new ForcePerLength(gsaload.BeamLoad.BeamLoad.Value(1), ForcePerLengthUnit.NewtonPerMeter);
              ForcePerLength outBeamForce2 = new ForcePerLength(apiBeamForce2.As(forcePerLengthUnit), forcePerLengthUnit);
              DA.SetData(7, new GH_UnitNumber(outBeamForce2));
              return;
            case GsaLoad.LoadTypes.Face:
              DA.SetData(0, gsaload.FaceLoad.FaceLoad.Case);
              DA.SetData(1, gsaload.FaceLoad.FaceLoad.Name);
              DA.SetData(2, gsaload.FaceLoad.FaceLoad.Elements);
              DA.SetData(3, gsaload.FaceLoad.FaceLoad.AxisProperty);
              DA.SetData(4, gsaload.FaceLoad.FaceLoad.Direction);
              DA.SetData(5, gsaload.FaceLoad.FaceLoad.IsProjected);
              Pressure apiFaceForce1 = new Pressure(gsaload.FaceLoad.FaceLoad.Value(0), PressureUnit.NewtonPerSquareMeter);
              Pressure outFaceForce1 = new Pressure(apiFaceForce1.As(forcePerAreaUnit), forcePerAreaUnit);
              DA.SetData(6, new GH_UnitNumber(outFaceForce1));
              Pressure apiFaceForce2 = new Pressure(gsaload.FaceLoad.FaceLoad.Value(1), PressureUnit.NewtonPerSquareMeter);
              Pressure outFaceForce2 = new Pressure(apiFaceForce2.As(forcePerAreaUnit), forcePerAreaUnit);
              DA.SetData(7, new GH_UnitNumber(outFaceForce2));
              Pressure apiFaceForce3 = new Pressure(gsaload.FaceLoad.FaceLoad.Value(2), PressureUnit.NewtonPerSquareMeter);
              Pressure outFaceForce3 = new Pressure(apiFaceForce3.As(forcePerAreaUnit), forcePerAreaUnit);
              DA.SetData(8, new GH_UnitNumber(outFaceForce3));
              Pressure apiFaceForce4 = new Pressure(gsaload.FaceLoad.FaceLoad.Value(3), PressureUnit.NewtonPerSquareMeter);
              Pressure outFaceForce4 = new Pressure(apiFaceForce4.As(forcePerAreaUnit), forcePerAreaUnit);
              DA.SetData(9, new GH_UnitNumber(outFaceForce4));
              return;
            case GsaLoad.LoadTypes.GridPoint:
              DA.SetData(0, gsaload.PointLoad.GridPointLoad.Case);
              DA.SetData(1, gsaload.PointLoad.GridPointLoad.Name);
              DA.SetData(2, "(" + gsaload.PointLoad.GridPointLoad.X + "," + gsaload.PointLoad.GridPointLoad.Y + ")");
              DA.SetData(3, gsaload.PointLoad.GridPointLoad.AxisProperty);
              DA.SetData(4, gsaload.PointLoad.GridPointLoad.Direction);
              Force apiPointForce = new Force(gsaload.PointLoad.GridPointLoad.Value, ForceUnit.Newton);
              Force outPointForce = new Force(apiPointForce.As(ForceUnit), ForceUnit);
              DA.SetData(6, new GH_UnitNumber(outPointForce));
              DA.SetData(10, new GsaGridPlaneSurfaceGoo(gsaload.PointLoad.GridPlaneSurface));
              return;
            case GsaLoad.LoadTypes.GridLine:
              DA.SetData(0, gsaload.LineLoad.GridLineLoad.Case);
              DA.SetData(1, gsaload.LineLoad.GridLineLoad.Name);
              DA.SetData(2, gsaload.LineLoad.GridLineLoad.PolyLineDefinition);
              DA.SetData(3, gsaload.LineLoad.GridLineLoad.AxisProperty);
              DA.SetData(4, gsaload.LineLoad.GridLineLoad.Direction);
              ForcePerLength apiLineForce1 = new ForcePerLength(gsaload.LineLoad.GridLineLoad.ValueAtStart, ForcePerLengthUnit.NewtonPerMeter);
              ForcePerLength outLineForce1 = new ForcePerLength(apiLineForce1.As(forcePerLengthUnit), forcePerLengthUnit);
              DA.SetData(6, new GH_UnitNumber(outLineForce1));
              ForcePerLength apiLineForce2 = new ForcePerLength(gsaload.LineLoad.GridLineLoad.ValueAtEnd, ForcePerLengthUnit.NewtonPerMeter);
              ForcePerLength outLineForce2 = new ForcePerLength(apiLineForce2.As(forcePerLengthUnit), forcePerLengthUnit);
              DA.SetData(7, new GH_UnitNumber(outLineForce2));
              DA.SetData(10, new GsaGridPlaneSurfaceGoo(gsaload.LineLoad.GridPlaneSurface));
              return;
            case GsaLoad.LoadTypes.GridArea:
              DA.SetData(0, gsaload.AreaLoad.GridAreaLoad.Case);
              DA.SetData(1, gsaload.AreaLoad.GridAreaLoad.Name);
              DA.SetData(2, gsaload.AreaLoad.GridAreaLoad.PolyLineDefinition);
              DA.SetData(3, gsaload.AreaLoad.GridAreaLoad.AxisProperty);
              DA.SetData(4, gsaload.AreaLoad.GridAreaLoad.Direction);
              Pressure apiAreaForce = new Pressure(gsaload.AreaLoad.GridAreaLoad.Value, PressureUnit.NewtonPerSquareMeter);
              Pressure outAreaForce = new Pressure(apiAreaForce.As(forcePerAreaUnit), forcePerAreaUnit);
              DA.SetData(6, new GH_UnitNumber(outAreaForce));
              DA.SetData(10, new GsaGridPlaneSurfaceGoo(gsaload.AreaLoad.GridPlaneSurface));
              return;
          }
        }
        else
        {
          AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error converting input to GSA Load");
          return;
        }
      }
    }

    #region Custom UI
    private ForceUnit ForceUnit = DefaultUnits.ForceUnit;
    private LengthUnit LengthUnit = DefaultUnits.LengthUnitGeometry;
    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[]
        {
          "Force Unit",
          "Length Unit"
        });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // Force
      this.DropDownItems.Add(FilteredUnits.FilteredForceUnits);
      this.SelectedItems.Add(this.ForceUnit.ToString());

      // Length
      this.DropDownItems.Add(FilteredUnits.FilteredLengthUnits);
      this.SelectedItems.Add(this.LengthUnit.ToString());

      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];
      switch (i)
      {
        case 0:
          this.ForceUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), this.SelectedItems[0]);
          break;
        case 1:
          this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[1]);
          break;
      }
      base.UpdateUI();
    }

    public override void UpdateUIFromSelectedItems()
    {
      this.ForceUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), this.SelectedItems[1]);
      this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[2]);
      base.UpdateUIFromSelectedItems();
    }
    public override void VariableParameterMaintenance()
    {
      string forceUnitAbbreviation = Force.GetAbbreviation(this.ForceUnit);
      string forcePerLengthUnit = ForcePerLength.GetAbbreviation(UnitsHelper.GetForcePerLengthUnit(this.ForceUnit, this.LengthUnit));
      string forcePerAreaUnit = Pressure.GetAbbreviation(UnitsHelper.GetForcePerAreaUnit(this.ForceUnit, this.LengthUnit));
      string unitAbbreviation = string.Join(", ", new string[] {forceUnitAbbreviation, forcePerLengthUnit, forcePerAreaUnit});

      Params.Output[6].Name = "Load Value or Factor X [" + unitAbbreviation + "]";
      Params.Output[7].Name = "Load Value or Factor X [" + unitAbbreviation + "]";
      Params.Output[8].Name = "Load Value or Factor X [" + unitAbbreviation + "]";
      Params.Output[9].Name = "Load Value [" + unitAbbreviation + "]";
    }
    #endregion
  }
}
