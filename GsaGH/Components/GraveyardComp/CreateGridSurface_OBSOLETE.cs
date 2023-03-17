using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
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
  public class CreateGridSurface_OBSOLETE : GH_OasysDropDownComponent {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("1052955c-cf97-4378-81d3-8491e0defad0");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.GridSurface;

    public CreateGridSurface_OBSOLETE() : base("Create Grid Surface",
      "GridSurface",
      "Create GSA Grid Surface",
      CategoryName.Name(),
      SubCategoryName.Cat3()) { } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      IQuantity length = new Length(0, DefaultUnits.LengthUnitGeometry);
      string unitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

      pManager.AddGenericParameter("Grid Plane", "GP", "Grid Plane. If no input, Global XY-plane will be used", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Grid Surface ID", "ID", "GSA Grid Surface ID. Setting this will replace any existing Grid Surfaces in GSA model", GH_ParamAccess.item, 0);
      pManager.AddTextParameter("Element list", "El", "List of Elements for which load should be expanded to (by default 'all')." + Environment.NewLine +
         "Element list should take the form:" + Environment.NewLine +
         " 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)" + Environment.NewLine +
         "Refer to GSA help file for definition of lists and full vocabulary.", GH_ParamAccess.item, "All");
      pManager.AddTextParameter("Name", "Na", "Grid Surface Name", GH_ParamAccess.item);
      pManager.AddGenericParameter("Tolerance [" + unitAbbreviation + "]", "To", "Tolerance for Load Expansion (default 10mm)", GH_ParamAccess.item);
      pManager.AddAngleParameter("Span Direction", "Di", "Span Direction between -180 and 180 degrees", GH_ParamAccess.item, 0);
      pManager[5].Optional = true;
      _angleInputParam = Params.Input[5];

      pManager[0].Optional = true;
      pManager[1].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;
      pManager[4].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaGridPlaneParameter(), "Grid Surface", "GPS", "GSA Grid Surface", GH_ParamAccess.item);
    }

    protected override void BeforeSolveInstance() {
      base.BeforeSolveInstance();
      if (_mode != FoldMode.OneDimensionalOneWay) {
        return;
      }

      if (Params.Input[5] is Param_Number angleParameter) {
        _angleUnit = angleParameter.UseDegrees
          ? AngleUnit.Degree
          : AngleUnit.Radian;
      }
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess da) {
      Plane plane = Plane.Unset;
      GsaGridPlaneSurface gsaGridPlaneSurface;
      bool idSet = false;

      var ghTyp = new GH_ObjectWrapper();
      if (da.GetData(0, ref ghTyp)) {
        if (ghTyp.Value is GsaGridPlaneSurfaceGoo) {
          var temppln = new GsaGridPlaneSurface();
          ghTyp.CastTo(ref temppln);
          gsaGridPlaneSurface = temppln.Duplicate();
        }
        else {
          if (ghTyp.CastTo(ref plane))
            gsaGridPlaneSurface = new GsaGridPlaneSurface(plane);
          else {
            if (GH_Convert.ToInt32(ghTyp.Value, out int id, GH_Conversion.Both)) {
              gsaGridPlaneSurface = new GsaGridPlaneSurface {
                GridSurface = {
                  GridPlane = id,
                },
                GridPlane = null,
              };
              idSet = true;
            }
            else {
              this.AddRuntimeError("Cannot convert your input to GridPlaneSurface or Plane");
              return;
            }
          }
        }
      }
      else {
        plane = Plane.WorldXY;
        gsaGridPlaneSurface = new GsaGridPlaneSurface(plane);
      }

      bool changeGs = false;
      var gs = new GridSurface(); // new GridSurface to make changes to, set it back to GPS in the end
      if (idSet)
        gs.GridPlane = gsaGridPlaneSurface.GridSurface.GridPlane;

      var ghint = new GH_Integer();
      if (da.GetData(1, ref ghint)) {
        GH_Convert.ToInt32(ghint, out int id, GH_Conversion.Both);
        gsaGridPlaneSurface.GridSurfaceId = id;
      }

      ghTyp = new GH_ObjectWrapper();
      if (da.GetData(2, ref ghTyp)) {
        string type = ghTyp.Value.ToString().ToUpper();
        if (type.StartsWith("GSA ")) {
          Params.Owner.AddRuntimeError(
            "You cannot input a Node/Element/Member in ElementList input!" + Environment.NewLine +
              "Element list should take the form:" + Environment.NewLine +
              "'1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)'" + Environment.NewLine +
              "Refer to GSA help file for definition of lists and full vocabulary.");
          return;
        }
      }
      var ghString = new GH_String();
      if (da.GetData(2, ref ghString)) {
        if (GH_Convert.ToString(ghString, out string elem, GH_Conversion.Both)) {
          gs.Elements = elem;
          changeGs = true;
        }
      }

      var ghtxt = new GH_String();
      if (da.GetData(3, ref ghtxt)) {
        if (GH_Convert.ToString(ghtxt, out string name, GH_Conversion.Both)) {
          gs.Name = name;
          changeGs = true;
        }
      }

      if (Params.Input[4].SourceCount != 0) {
        gs.Tolerance = ((Length)Input.UnitNumber(this, da, 4, _lengthUnit, true)).Millimeters;
        changeGs = true;
      }

      switch (_mode) {
        case FoldMode.OneDimensionalOneWay:
          gs.ElementType = GridSurface.Element_Type.ONE_DIMENSIONAL;
          gs.SpanType = GridSurface.Span_Type.ONE_WAY;

          double dir = 0.0;
          if (da.GetData(5, ref dir)) {
            var direction = new Angle(dir, _angleUnit);

            if (direction.Degrees > 180 || direction.Degrees < -180)
              this.AddRuntimeWarning("Angle value must be between -180 and 180 degrees"); // to be updated when GsaAPI support units
            gs.Direction = direction.Degrees;
            if (dir != 0.0)
              changeGs = true;
          }
          break;

        case FoldMode.OneDimensionalTwoWay:
          changeGs = true;
          gs.ElementType = GridSurface.Element_Type.ONE_DIMENSIONAL;

          int exp = 0;
          var ghexp = new GH_Integer();
          if (da.GetData(5, ref ghexp))
            GH_Convert.ToInt32_Primary(ghexp, ref exp);
          gs.ExpansionType = GridSurfaceExpansionType.PLANE_CORNER;
          switch (exp)
          {
            case 1:
              gs.ExpansionType = GridSurfaceExpansionType.PLANE_SMOOTH;
              break;
            case 2:
              gs.ExpansionType = GridSurfaceExpansionType.PLANE_ASPECT;
              break;
            case 3:
              gs.ExpansionType = GridSurfaceExpansionType.LEGACY;
              break;
          }

          bool simple = true;
          var ghsim = new GH_Boolean();
          if (da.GetData(6, ref ghsim))
            GH_Convert.ToBoolean(ghsim, out simple, GH_Conversion.Both);
          gs.SpanType = simple
            ? GridSurface.Span_Type.TWO_WAY_SIMPLIFIED_TRIBUTARY_AREAS
            : GridSurface.Span_Type.TWO_WAY;
          break;

        case FoldMode.TwoDimensional:
          changeGs = true;
          gs.ElementType = GridSurface.Element_Type.TWO_DIMENSIONAL;
          break;
      }
      if (changeGs)
        gsaGridPlaneSurface.GridSurface = gs;

      da.SetData(0, new GsaGridPlaneSurfaceGoo(gsaGridPlaneSurface));
    }

    #region Custom UI
    private enum FoldMode {
      OneDimensionalOneWay,
      OneDimensionalTwoWay,
      TwoDimensional,
    }

    private readonly List<string> _type = new List<string>(new []
    {
      "1D, One-way span",
      "1D, Two-way span",
      "2D",
    });
    private AngleUnit _angleUnit = AngleUnit.Radian;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitGeometry;
    private FoldMode _mode = FoldMode.OneDimensionalOneWay;
    public override void InitialiseDropdowns() {
      SpacerDescriptions = new List<string>(new []
        {
          "Type", "Unit",
        });

      DropDownItems = new List<List<string>>();
      SelectedItems = new List<string>();

      // Type
      DropDownItems.Add(_type);
      SelectedItems.Add(_type[0]);

      // Length
      DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      SelectedItems.Add(Length.GetAbbreviation(_lengthUnit));

      IsInitialised = true;
    }

    public override void SetSelected(int i, int j) {
      SelectedItems[i] = DropDownItems[i][j];
      if (i == 0) {
        switch (SelectedItems[i]) {
          case "1D, One-way span":
            Mode1Clicked();
            break;
          case "1D, Two-way span":
            Mode2Clicked();
            break;
          case "2D":
            Mode3Clicked();
            break;
        }
      }
      else
        _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), SelectedItems[i]);

      base.UpdateUI();
    }
    public override void UpdateUIFromSelectedItems() {
      switch (SelectedItems[0]) {
        case "1D, One-way span":
          _mode = FoldMode.OneDimensionalOneWay;
          break;
        case "1D, Two-way span":
          _mode = FoldMode.OneDimensionalTwoWay;
          break;
        case "2D":
          _mode = FoldMode.TwoDimensional;
          break;
      }
      if (SelectedItems.Count > 1)
        _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), SelectedItems[1]);
      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance() {
      switch (_mode)
      {
        case FoldMode.OneDimensionalOneWay:
          Params.Input[5].NickName = "Dir";
          Params.Input[5].Name = "Span Direction";
          Params.Input[5].Description = "Span Direction between -180 and 180 degrees";
          Params.Input[5].Access = GH_ParamAccess.item;
          Params.Input[5].Optional = true;
          break;
        case FoldMode.OneDimensionalTwoWay:
          Params.Input[5].NickName = "Exp";
          Params.Input[5].Name = "Load Expansion";
          Params.Input[5].Description = "Load Expansion: " +
                                        Environment.NewLine + "Accepted inputs are:" +
                                        Environment.NewLine + "0 : Corner (plane)" +
                                        Environment.NewLine + "1 : Smooth (plane)" +
                                        Environment.NewLine + "2 : Plane" +
                                        Environment.NewLine + "3 : Legacy";
          Params.Input[5].Access = GH_ParamAccess.item;
          Params.Input[5].Optional = true;

          Params.Input[6].NickName = "Sim";
          Params.Input[6].Name = "Simplify";
          Params.Input[6].Description = "Simplify Tributary Area (default: True)";
          Params.Input[6].Access = GH_ParamAccess.item;
          Params.Input[6].Optional = true;
          break;
      }
    }
    #endregion

    #region menu override
    private IGH_Param _angleInputParam;
    private void Mode1Clicked() {
      if (_mode == FoldMode.OneDimensionalOneWay)
        return;

      RecordUndoEvent("1D, one-way Parameters");
      _mode = FoldMode.OneDimensionalOneWay;

      while (Params.Input.Count > 5)
        Params.UnregisterInputParameter(Params.Input[5], true);

      Params.RegisterInputParam(_angleInputParam);
    }
    private void Mode2Clicked() {
      switch (_mode)
      {
        case FoldMode.OneDimensionalTwoWay:
          return;
        case FoldMode.OneDimensionalOneWay:
          _angleInputParam = Params.Input[5];
          break;
      }

      RecordUndoEvent("1D, two-way Parameters");
      _mode = FoldMode.OneDimensionalTwoWay;

      while (Params.Input.Count > 5)
        Params.UnregisterInputParameter(Params.Input[5], true);

      Params.RegisterInputParam(new Param_Integer());
      Params.RegisterInputParam(new Param_Boolean());
    }
    private void Mode3Clicked() {
      switch (_mode)
      {
        case FoldMode.TwoDimensional:
          return;
        case FoldMode.OneDimensionalOneWay:
          _angleInputParam = Params.Input[5];
          break;
      }

      RecordUndoEvent("2D Parameters");
      _mode = FoldMode.TwoDimensional;

      while (Params.Input.Count > 5)
        Params.UnregisterInputParameter(Params.Input[5], true);
    }

    #endregion

    #region (de)serialization
    public override bool Read(GH_IO.Serialization.GH_IReader reader) {
      if (!reader.ItemExists("Mode")) {
        return base.Read(reader);
      }

      _mode = (FoldMode)reader.GetInt32("Mode");

      InitialiseDropdowns();

      SelectedItems = new List<string>();
      switch (_mode) {
        case FoldMode.OneDimensionalOneWay:
          SelectedItems.Add("1D, One-way span");
          break;
        case FoldMode.OneDimensionalTwoWay:
          SelectedItems.Add("1D, Two-way span");
          break;
        case FoldMode.TwoDimensional:
          SelectedItems.Add("2D");
          break;
      }
      SelectedItems.Add(Length.GetAbbreviation(_lengthUnit));

      return base.Read(reader);
    }
    #endregion
  }
}
