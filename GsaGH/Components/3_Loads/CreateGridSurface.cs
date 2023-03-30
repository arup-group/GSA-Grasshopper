﻿using System;
using System.Collections.Generic;
using System.Drawing;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Components {

  public class CreateGridSurface : GH_OasysDropDownComponent {

    #region Enums
    private enum FoldMode {
      OneDimensionalOneWay,
      OneDimensionalTwoWay,
      TwoDimensional,
    }
    #endregion Enums

    #region Properties + Fields
    public override Guid ComponentGuid => new Guid("b9405f78-317b-474f-b258-4a178a70bc02");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.GridSurface;

    private readonly List<string> _type = new List<string>(new[] {
      "1D, One-way span",
      "1D, Two-way span",
      "2D",
    });

    private IGH_Param _angleInputParam;
    private AngleUnit _angleUnit = AngleUnit.Radian;
    private bool _duringLoad = false;
    private FoldMode _mode = FoldMode.OneDimensionalOneWay;
    #endregion Properties + Fields

    #region Public Constructors
    public CreateGridSurface() : base("Create Grid Surface",
      "GridSurface",
      "Create GSA Grid Surface",
      CategoryName.Name(),
      SubCategoryName.Cat3()) { }

    #endregion Public Constructors

    #region Public Methods
    public override void InitialiseDropdowns() {
      SpacerDescriptions = new List<string>(new[] {
        "Type",
      });

      DropDownItems = new List<List<string>>();
      SelectedItems = new List<string>();

      DropDownItems.Add(_type);
      SelectedItems.Add(_type[0]);

      IsInitialised = true;
    }

    public override bool Read(GH_IReader reader) {
      if (reader.ItemExists("Mode")) {
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
      }

      return base.Read(reader);
    }

    public override void SetSelected(int i, int j) {
      SelectedItems[i] = DropDownItems[i][j];
      if (i == 0)
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

      base.UpdateUI();
    }

    public override void UpdateUIFromSelectedItems() {
      _duringLoad = true;
      switch (SelectedItems[0]) {
        case "1D, One-way span":
          _mode = FoldMode.OneDimensionalOneWay;
          Mode1Clicked();
          break;

        case "1D, Two-way span":
          _mode = FoldMode.OneDimensionalTwoWay;
          Mode2Clicked();
          break;

        case "2D":
          _mode = FoldMode.TwoDimensional;
          Mode3Clicked();
          break;
      }

      _duringLoad = false;
      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance() {
      switch (_mode) {
        case FoldMode.OneDimensionalOneWay:
          Params.Input[5]
            .NickName = "Dir";
          Params.Input[5]
            .Name = "Span Direction";
          Params.Input[5]
            .Description = "Span Direction between -180 and 180 degrees";
          Params.Input[5]
            .Access = GH_ParamAccess.item;
          Params.Input[5]
            .Optional = true;
          break;

        case FoldMode.OneDimensionalTwoWay:
          Params.Input[5]
            .NickName = "Exp";
          Params.Input[5]
            .Name = "Load Expansion";
          Params.Input[5]
              .Description = "Load Expansion: "
            + Environment.NewLine
            + "Accepted inputs are:"
            + Environment.NewLine
            + "0 : Corner (plane)"
            + Environment.NewLine
            + "1 : Smooth (plane)"
            + Environment.NewLine
            + "2 : Plane"
            + Environment.NewLine
            + "3 : Legacy";
          Params.Input[5]
            .Access = GH_ParamAccess.item;
          Params.Input[5]
            .Optional = true;

          Params.Input[6]
            .NickName = "Sim";
          Params.Input[6]
            .Name = "Simplify";
          Params.Input[6]
            .Description = "Simplify Tributary Area (default: True)";
          Params.Input[6]
            .Access = GH_ParamAccess.item;
          Params.Input[6]
            .Optional = true;
          break;
      }
    }

    #endregion Public Methods

    #region Protected Methods
    protected override void BeforeSolveInstance() {
      base.BeforeSolveInstance();
      if (_mode != FoldMode.OneDimensionalOneWay)
        return;

      if (Params.Input[5] is Param_Number angleParameter)
        _angleUnit = angleParameter.UseDegrees
          ? AngleUnit.Degree
          : AngleUnit.Radian;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddGenericParameter("Grid Plane",
        "GP",
        "Grid Plane. If no input, Global XY-plane will be used",
        GH_ParamAccess.item);
      pManager.AddIntegerParameter("Grid Surface ID",
        "ID",
        "GSA Grid Surface ID. Setting this will replace any existing Grid Surfaces in GSA model",
        GH_ParamAccess.item,
        0);
      pManager.AddGenericParameter("Element list",
        "El",
        "Properties, Elements or Members to which load should be expanded to (by default 'All'); either input Section, Prop2d, Prop3d, Element1d, Element2d, Member1d, Member2d or Member3d, or a text string."
        + Environment.NewLine
        + "Element list should take the form:"
        + Environment.NewLine
        + " 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)"
        + Environment.NewLine
        + "Refer to GSA help file for definition of lists and full vocabulary.",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Name", "Na", "Grid Surface Name", GH_ParamAccess.item);
      pManager.AddGenericParameter("Tolerance in model units",
        "To",
        "Tolerance for Load Expansion (default 10mm)",
        GH_ParamAccess.item);
      pManager.AddAngleParameter("Span Direction",
        "Di",
        "Span Direction between -180 and 180 degrees",
        GH_ParamAccess.item,
        0);
      pManager[5]
        .Optional = true;
      _angleInputParam = Params.Input[5];

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
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
      => pManager.AddParameter(new GsaGridPlaneParameter(),
        "Grid Surface",
        "GPS",
        "GSA Grid Surface",
        GH_ParamAccess.item);

    protected override void SolveInstance(IGH_DataAccess da) {
      Plane plane = Plane.Unset;
      GsaGridPlaneSurface gps;
      bool idSet = false;

      var ghTyp = new GH_ObjectWrapper();
      if (da.GetData(0, ref ghTyp)) {
        if (ghTyp.Value is GsaGridPlaneSurfaceGoo) {
          var temppln = new GsaGridPlaneSurface();
          ghTyp.CastTo(ref temppln);
          gps = temppln.Duplicate();
        }
        else {
          if (ghTyp.CastTo(ref plane))
            gps = new GsaGridPlaneSurface(plane);
          else {
            if (GH_Convert.ToInt32(ghTyp.Value, out int id, GH_Conversion.Both)) {
              gps = new GsaGridPlaneSurface {
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
        gps = new GsaGridPlaneSurface(plane);
      }

      bool changeGs = false;
      var gs = new GridSurface();
      if (idSet)
        gs.GridPlane = gps.GridSurface.GridPlane;

      var ghInteger = new GH_Integer();
      if (da.GetData(1, ref ghInteger)) {
        GH_Convert.ToInt32(ghInteger, out int id, GH_Conversion.Both);
        gps.GridSurfaceId = id;
      }

      ghTyp = new GH_ObjectWrapper();
      if (da.GetData(2, ref ghTyp)) {
        gps.GridSurface.Elements = "";
        if (ghTyp.Value is GsaElement1dGoo element1dGoo) {
          gps._refObjectGuid = element1dGoo.Value.Guid;
          gps._referenceType = ReferenceType.Element;
        }

        switch (ghTyp.Value) {
          case GsaElement2dGoo value: {
              gps._refObjectGuid = value.Value.Guid;
              gps._referenceType = ReferenceType.Element;
              break;
            }
          case GsaMember1dGoo value: {
              gps._refObjectGuid = value.Value.Guid;
              gps._referenceType = ReferenceType.Member;
              this.AddRuntimeRemark(
                "Member loading in GsaGH will automatically find child elements created from parent member with the load still being applied to elements. If you save the file and continue working in GSA please note that the member-loading relationship will be lost.");
              break;
            }
          case GsaMember2dGoo value: {
              gps._refObjectGuid = value.Value.Guid;
              gps._referenceType = ReferenceType.Member;
              this.AddRuntimeRemark(
                "Member loading in GsaGH will automatically find child elements created from parent member with the load still being applied to elements. If you save the file and continue working in GSA please note that the member-loading relationship will be lost.");
              break;
            }
          case GsaMember3dGoo value: {
              gps._refObjectGuid = value.Value.Guid;
              gps._referenceType = ReferenceType.Member;
              this.AddRuntimeRemark(
                "Member loading in GsaGH will automatically find child elements created from parent member with the load still being applied to elements. If you save the file and continue working in GSA please note that the member-loading relationship will be lost.");
              break;
            }
          case GsaSectionGoo value: {
              gps._refObjectGuid = value.Value.Guid;
              gps._referenceType = ReferenceType.Section;
              break;
            }
          case GsaProp2dGoo value: {
              gps._refObjectGuid = value.Value.Guid;
              gps._referenceType = ReferenceType.Prop2d;
              break;
            }
          case GsaProp3dGoo value: {
              gps._refObjectGuid = value.Value.Guid;
              gps._referenceType = ReferenceType.Prop3d;
              break;
            }
          default: {
              if (GH_Convert.ToString(ghTyp.Value, out string elemList, GH_Conversion.Both))
                gps.GridSurface.Elements = elemList;
              break;
            }
        }
      }
      else
        gps.GridSurface.Elements = "All";

      var ghString = new GH_String();
      if (da.GetData(3, ref ghString))
        if (GH_Convert.ToString(ghString, out string name, GH_Conversion.Both)) {
          gs.Name = name;
          changeGs = true;
        }

      ghTyp = new GH_ObjectWrapper();
      if (da.GetData(4, ref ghTyp)) {
        string tolIn = ghTyp.Value.ToString();

        if (tolIn != "")
          try {
            Length.Parse(tolIn);
            gps.Tolerance = tolIn;
          }
          catch (Exception e) {
            if (double.TryParse(tolIn, out double _))
              gps.Tolerance = tolIn;
            else
              this.AddRuntimeWarning(e.Message);
          }
      }

      switch (_mode) {
        case FoldMode.OneDimensionalOneWay:
          gs.ElementType = GridSurface.Element_Type.ONE_DIMENSIONAL;
          gs.SpanType = GridSurface.Span_Type.ONE_WAY;

          double dir = 0.0;
          if (da.GetData(5, ref dir)) {
            var direction = new Angle(dir, _angleUnit);

            if (direction.Degrees > 180 || direction.Degrees < -180)
              this.AddRuntimeWarning("Angle value must be between -180 and 180 degrees");
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
          switch (exp) {
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
        gps.GridSurface = gs;

      da.SetData(0, new GsaGridPlaneSurfaceGoo(gps));
    }

    #endregion Protected Methods

    #region Private Methods
    private void Mode1Clicked() {
      if (!_duringLoad && _mode == FoldMode.OneDimensionalOneWay)
        return;

      RecordUndoEvent("1D, one-way Parameters");
      _mode = FoldMode.OneDimensionalOneWay;

      while (Params.Input.Count > 5)
        Params.UnregisterInputParameter(Params.Input[5], true);

      Params.RegisterInputParam(_angleInputParam);
    }

    private void Mode2Clicked() {
      if (!_duringLoad && _mode == FoldMode.OneDimensionalTwoWay)
        return;
      if (_mode == FoldMode.OneDimensionalOneWay)
        _angleInputParam = Params.Input[5];

      RecordUndoEvent("1D, two-way Parameters");
      _mode = FoldMode.OneDimensionalTwoWay;

      while (Params.Input.Count > 5)
        Params.UnregisterInputParameter(Params.Input[5], true);

      Params.RegisterInputParam(new Param_Integer());
      Params.RegisterInputParam(new Param_Boolean());
    }

    private void Mode3Clicked() {
      if (!_duringLoad && _mode == FoldMode.TwoDimensional)
        return;
      if (_mode == FoldMode.OneDimensionalOneWay)
        _angleInputParam = Params.Input[5];

      RecordUndoEvent("2D Parameters");
      _mode = FoldMode.TwoDimensional;

      while (Params.Input.Count > 5)
        Params.UnregisterInputParameter(Params.Input[5], true);
    }

    #endregion Private Methods
  }
}
