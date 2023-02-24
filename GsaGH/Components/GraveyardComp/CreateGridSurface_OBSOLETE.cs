using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Parameters;
using GsaGH.Helpers.GH;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Components
{
  public class CreateGridSurface_OBSOLETE : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("1052955c-cf97-4378-81d3-8491e0defad0");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.GridSurface;

    public CreateGridSurface_OBSOLETE() : base("Create Grid Surface",
      "GridSurface",
      "Create GSA Grid Surface",
      CategoryName.Name(),
      SubCategoryName.Cat3())
    { } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      IQuantity length = new Length(0, DefaultUnits.LengthUnitGeometry);
      string unitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

      pManager.AddGenericParameter("Grid Plane", "GP", "Grid Plane. If no input, Global XY-plane will be used", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Grid Surface ID", "ID", "GSA Grid Surface ID. Setting this will replace any existing Grid Surfaces in GSA model", GH_ParamAccess.item, 0);
      pManager.AddTextParameter("Element list", "El", "List of Elements for which load should be expanded to (by default 'all')." + System.Environment.NewLine +
         "Element list should take the form:" + System.Environment.NewLine +
         " 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)" + System.Environment.NewLine +
         "Refer to GSA help file for definition of lists and full vocabulary.", GH_ParamAccess.item, "All");
      pManager.AddTextParameter("Name", "Na", "Grid Surface Name", GH_ParamAccess.item);
      pManager.AddGenericParameter("Tolerance [" + unitAbbreviation + "]", "To", "Tolerance for Load Expansion (default 10mm)", GH_ParamAccess.item);
      pManager.AddAngleParameter("Span Direction", "Di", "Span Direction between -180 and 180 degrees", GH_ParamAccess.item, 0);
      pManager[5].Optional = true;
      angleInputParam = this.Params.Input[5];

      pManager[0].Optional = true;
      pManager[1].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;
      pManager[4].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new GsaGridPlaneParameter(), "Grid Surface", "GPS", "GSA Grid Surface", GH_ParamAccess.item);
    }

    protected override void BeforeSolveInstance()
    {
      base.BeforeSolveInstance();
      if (_mode == FoldMode.One_Dimensional_One_Way)
      {
        Param_Number angleParameter = Params.Input[5] as Param_Number;
        if (angleParameter != null)
        {
          if (angleParameter.UseDegrees)
            this.AngleUnit = AngleUnit.Degree;
          else
            this.AngleUnit = AngleUnit.Radian;
        }
      }
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      // 0 Plane
      Plane pln = Plane.Unset;
      GsaGridPlaneSurface gps;
      bool idSet = false;

      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(0, ref gh_typ))
      {
        if (gh_typ.Value is GsaGridPlaneSurfaceGoo)
        {
          GsaGridPlaneSurface temppln = new GsaGridPlaneSurface();
          gh_typ.CastTo(ref temppln);
          gps = temppln.Duplicate();
        }
        else
        {
          if (gh_typ.CastTo(ref pln))
            gps = new GsaGridPlaneSurface(pln);
          else
          {
            int id = 0;
            if (GH_Convert.ToInt32(gh_typ.Value, out id, GH_Conversion.Both))
            {
              gps = new GsaGridPlaneSurface();
              gps.GridSurface.GridPlane = id;
              gps.GridPlane = null;
              idSet = true;
            }
            else
            {
              this.AddRuntimeError("Cannot convert your input to GridPlaneSurface or Plane");
              return;
            }
          }
        }
      }
      else
      {
        pln = Plane.WorldXY;
        gps = new GsaGridPlaneSurface(pln);
      }

      // record if changes has been made from default type
      bool changeGS = false;
      GridSurface gs = new GridSurface(); // new GridSurface to make changes to, set it back to GPS in the end
      if (idSet)
        gs.GridPlane = gps.GridSurface.GridPlane;

      // 1 ID
      GH_Integer ghint = new GH_Integer();
      if (DA.GetData(1, ref ghint))
      {
        int id = 0;
        GH_Convert.ToInt32(ghint, out id, GH_Conversion.Both);
        gps.GridSurfaceId = id;
      }

      // 2 Elements
      // check that user has not inputted Gsa geometry elements here
      gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(2, ref gh_typ))
      {
        string type = gh_typ.Value.ToString().ToUpper();
        if (type.StartsWith("GSA "))
        {
          Params.Owner.AddRuntimeError(
            "You cannot input a Node/Element/Member in ElementList input!" + System.Environment.NewLine +
              "Element list should take the form:" + System.Environment.NewLine +
              "'1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)'" + System.Environment.NewLine +
              "Refer to GSA help file for definition of lists and full vocabulary.");
          return;
        }
      }
      GH_String ghelem = new GH_String();
      if (DA.GetData(2, ref ghelem))
      {
        string elem = "";
        if (GH_Convert.ToString(ghelem, out elem, GH_Conversion.Both))
        {
          gs.Elements = elem;
          changeGS = true;
        }
      }

      // 3 Name
      GH_String ghtxt = new GH_String();
      if (DA.GetData(3, ref ghtxt))
      {
        string name = "";
        if (GH_Convert.ToString(ghtxt, out name, GH_Conversion.Both))
        {
          gs.Name = name;
          changeGS = true;
        }
      }

      // 4 Tolerance
      if (this.Params.Input[4].SourceCount != 0)
      {
        gs.Tolerance = ((Length)Input.UnitNumber(this, DA, 4, this.LengthUnit, true)).Millimeters;
        changeGS = true;
      }

      switch (_mode)
      {
        case FoldMode.One_Dimensional_One_Way:
          gs.ElementType = GridSurface.Element_Type.ONE_DIMENSIONAL;
          gs.SpanType = GridSurface.Span_Type.ONE_WAY;

          // 5 span direction
          double dir = 0.0;
          if (DA.GetData(5, ref dir))
          {
            Angle direction = new Angle(dir, this.AngleUnit);

            if (direction.Degrees > 180 || direction.Degrees < -180)
              this.AddRuntimeWarning("Angle value must be between -180 and 180 degrees"); // to be updated when GsaAPI support units
            gs.Direction = direction.Degrees;
            if (dir != 0.0)
              changeGS = true;
          }
          break;

        case FoldMode.One_Dimensional_Two_Way:
          changeGS = true;
          gs.ElementType = GridSurface.Element_Type.ONE_DIMENSIONAL;

          // 5 expansion method
          int exp = 0;
          GH_Integer ghexp = new GH_Integer();
          if (DA.GetData(5, ref ghexp))
            GH_Convert.ToInt32_Primary(ghexp, ref exp);
          gs.ExpansionType = GridSurfaceExpansionType.PLANE_CORNER;
          if (exp == 1)
            gs.ExpansionType = GridSurfaceExpansionType.PLANE_SMOOTH;
          if (exp == 2)
            gs.ExpansionType = GridSurfaceExpansionType.PLANE_ASPECT;
          if (exp == 3)
            gs.ExpansionType = GridSurfaceExpansionType.LEGACY;

          // 6 simplify tributary area
          bool simple = true;
          GH_Boolean ghsim = new GH_Boolean();
          if (DA.GetData(6, ref ghsim))
            GH_Convert.ToBoolean(ghsim, out simple, GH_Conversion.Both);
          if (simple)
            gs.SpanType = GridSurface.Span_Type.TWO_WAY_SIMPLIFIED_TRIBUTARY_AREAS;
          else
            gs.SpanType = GridSurface.Span_Type.TWO_WAY;
          break;

        case FoldMode.Two_Dimensional:
          changeGS = true;
          gs.ElementType = GridSurface.Element_Type.TWO_DIMENSIONAL;
          break;
      }
      if (changeGS)
        gps.GridSurface = gs;

      DA.SetData(0, new GsaGridPlaneSurfaceGoo(gps));
    }


    #region Custom UI
    private enum FoldMode
    {
      One_Dimensional_One_Way,
      One_Dimensional_Two_Way,
      Two_Dimensional
    }
    readonly List<string> _type = new List<string>(new string[]
    {
      "1D, One-way span",
      "1D, Two-way span",
      "2D"
    });
    AngleUnit AngleUnit = AngleUnit.Radian;
    LengthUnit LengthUnit = DefaultUnits.LengthUnitGeometry;
    private FoldMode _mode = FoldMode.One_Dimensional_One_Way;
    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[]
        {
          "Type", "Unit"
        });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // Type
      this.DropDownItems.Add(this._type);
      this.SelectedItems.Add(this._type[0].ToString());

      // Length
      this.DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      this.SelectedItems.Add(Length.GetAbbreviation(this.LengthUnit));

      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];
      if (i == 0)
      {
        switch (this.SelectedItems[i])
        {
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
        this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), this.SelectedItems[i]);

      base.UpdateUI();
    }
    public override void UpdateUIFromSelectedItems()
    {
      switch (this.SelectedItems[0])
      {
        case "1D, One-way span":
          this._mode = FoldMode.One_Dimensional_One_Way;
          break;
        case "1D, Two-way span":
          this._mode = FoldMode.One_Dimensional_Two_Way;
          break;
        case "2D":
          this._mode = FoldMode.Two_Dimensional;
          break;
      }
      if (this.SelectedItems.Count > 1)
        this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), this.SelectedItems[1]);
      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance()
    {
      if (_mode == FoldMode.One_Dimensional_One_Way)
      {
        Params.Input[5].NickName = "Dir";
        Params.Input[5].Name = "Span Direction";
        Params.Input[5].Description = "Span Direction between -180 and 180 degrees";
        Params.Input[5].Access = GH_ParamAccess.item;
        Params.Input[5].Optional = true;
      }
      if (_mode == FoldMode.One_Dimensional_Two_Way)
      {
        Params.Input[5].NickName = "Exp";
        Params.Input[5].Name = "Load Expansion";
        Params.Input[5].Description = "Load Expansion: " + System.Environment.NewLine + "Accepted inputs are:" +
            System.Environment.NewLine + "0 : Corner (plane)" +
            System.Environment.NewLine + "1 : Smooth (plane)" +
            System.Environment.NewLine + "2 : Plane" +
            System.Environment.NewLine + "3 : Legacy";
        Params.Input[5].Access = GH_ParamAccess.item;
        Params.Input[5].Optional = true;

        Params.Input[6].NickName = "Sim";
        Params.Input[6].Name = "Simplify";
        Params.Input[6].Description = "Simplify Tributary Area (default: True)";
        Params.Input[6].Access = GH_ParamAccess.item;
        Params.Input[6].Optional = true;
      }
    }
    #endregion

    #region menu override
    private IGH_Param angleInputParam;
    private void Mode1Clicked()
    {
      if (_mode == FoldMode.One_Dimensional_One_Way)
        return;

      RecordUndoEvent("1D, one-way Parameters");
      _mode = FoldMode.One_Dimensional_One_Way;

      //remove input parameters
      while (Params.Input.Count > 5)
        Params.UnregisterInputParameter(Params.Input[5], true);

      //add input parameters
      Params.RegisterInputParam(angleInputParam);
    }
    private void Mode2Clicked()
    {
      if (_mode == FoldMode.One_Dimensional_Two_Way)
        return;
      if (_mode == FoldMode.One_Dimensional_One_Way)
        angleInputParam = Params.Input[5];

      RecordUndoEvent("1D, two-way Parameters");
      _mode = FoldMode.One_Dimensional_Two_Way;

      //remove input parameters
      while (Params.Input.Count > 5)
        Params.UnregisterInputParameter(Params.Input[5], true);

      //add input parameters
      Params.RegisterInputParam(new Param_Integer());
      Params.RegisterInputParam(new Param_Boolean());
    }
    private void Mode3Clicked()
    {
      if (_mode == FoldMode.Two_Dimensional)
        return;
      if (_mode == FoldMode.One_Dimensional_One_Way)
        angleInputParam = Params.Input[5];

      RecordUndoEvent("2D Parameters");
      _mode = FoldMode.Two_Dimensional;

      //remove input parameters
      while (Params.Input.Count > 5)
        Params.UnregisterInputParameter(Params.Input[5], true);
    }

    #endregion

    #region (de)serialization
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      if (reader.ItemExists("Mode"))
      {
        _mode = (FoldMode)reader.GetInt32("Mode");
        
        this.InitialiseDropdowns();

        this.SelectedItems = new List<string>();
        switch (_mode)
        {
          case FoldMode.One_Dimensional_One_Way:
            this.SelectedItems.Add("1D, One-way span");
            break;
          case FoldMode.One_Dimensional_Two_Way:
            this.SelectedItems.Add("1D, Two-way span");
            break;
          case FoldMode.Two_Dimensional:
            this.SelectedItems.Add("2D");
            break;
        }
        this.SelectedItems.Add(Length.GetAbbreviation(this.LengthUnit));
      }
      
      return base.Read(reader);
    }
    #endregion
  }
}
