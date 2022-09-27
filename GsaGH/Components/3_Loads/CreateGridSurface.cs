using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Units;
using OasysUnits;
using Rhino;
using Rhino.Geometry;

namespace GsaGH.Components
{
  public class CreateGridSurface : GH_OasysComponent, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("1052955c-cf97-4378-81d3-8491e0defad0");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.GridSurface;

    public CreateGridSurface() : base("Create Grid Surface",
      "GridSurface",
      "Create GSA Grid Surface",
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat3())
    { } // sets the initial state of the component to hidden
    #endregion

    #region Custom UI
    //This region overrides the typical component layout
    public override void CreateAttributes()
    {
      //if (first)
      //{
      //    selecteditem = "1D, One-way span";
      //    
      //}

      m_attributes = new UI.DropDownComponentUI(this, SetSelected, dropdownitems, selecteditem, "Type");
    }

    public void SetSelected(string selected)
    {
      selecteditem = selected;
      switch (selected)
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
    #endregion

    #region Input and output
    readonly List<string> dropdownitems = new List<string>(new string[]
    {
            "1D, One-way span",
            "1D, Two-way span",
            "2D"
    });

    string selecteditem = "1D, One-way span";
    private bool _useDegrees = false;
    #endregion

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string angleUnit = "rad";
      if (_useDegrees)
        angleUnit = "deg";

      IQuantity length = new Length(0, DefaultUnits.LengthUnitGeometry);
      string unitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

      pManager.AddGenericParameter("Grid Plane", "GP", "Grid Plane. If no input, Global XY-plane will be used", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Grid Surface ID", "ID", "GSA Grid Surface ID. Setting this will replace any existing Grid Surfaces in GSA model", GH_ParamAccess.item, 0);
      pManager.AddTextParameter("Element list", "El", "List of Elements for which load should be expanded to (by default 'all')." + System.Environment.NewLine +
         "Element list should take the form:" + System.Environment.NewLine +
         " 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)" + System.Environment.NewLine +
         "Refer to GSA help file for definition of lists and full vocabulary.", GH_ParamAccess.item, "All");
      pManager.AddTextParameter("Name", "Na", "Grid Surface Name", GH_ParamAccess.item);
      pManager.AddNumberParameter("Tolerance [" + unitAbbreviation + "]", "To", "Tolerance for Load Expansion (default 10mm)", GH_ParamAccess.item, 0.01);

      pManager[0].Optional = true;
      pManager[1].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;
      pManager[4].Optional = true;

      if (first)
      {
        _mode = FoldMode.One_Dimensional_One_Way;
        pManager.AddAngleParameter("Span Direction [" + angleUnit + "]", "Di", "Span Direction between -180 and 180 degrees", GH_ParamAccess.item, 0);
        pManager[5].Optional = true;
        first = false;
        angleInputParam = this.Params.Input[5];
      }
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Grid Surface", "GPS", "GSA Grid Surface", GH_ParamAccess.item);
    }
    protected override void BeforeSolveInstance()
    {
      if (_mode == FoldMode.One_Dimensional_One_Way)
      {
        _useDegrees = false;
        Param_Number angleParameter = Params.Input[5] as Param_Number;
        if (angleParameter != null)
          _useDegrees = angleParameter.UseDegrees;
      }
    }
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
              AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Cannot convert your input to GridPlaneSurface or Plane");
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
        gps.GridSurfaceID = id;
      }

      // 2 Elements
      // check that user has not inputted Gsa geometry elements here
      gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(2, ref gh_typ))
      {
        string type = gh_typ.Value.ToString().ToUpper();
        if (type.StartsWith("GSA "))
        {
          Params.Owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error,
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
        gs.Tolerance = ((Length)Input.LengthOrRatio(this, DA, 4, DefaultUnits.LengthUnitGeometry, true)).Millimeters;
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
            if (!_useDegrees)
              dir = RhinoMath.ToDegrees(dir);

            if (dir > 180 || dir < -180)
              AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Angle value must be between -180 and 180 degrees"); // to be updated when GsaAPI support units
            gs.Direction = dir;
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

    #region menu override
    private enum FoldMode
    {
      One_Dimensional_One_Way,
      One_Dimensional_Two_Way,
      Two_Dimensional
    }
    private bool first = true;
    private FoldMode _mode = FoldMode.One_Dimensional_One_Way;
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

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();
      ExpireSolution(true);
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

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();
      ExpireSolution(true);
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

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();
      ExpireSolution(true);
    }

    #endregion

    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      writer.SetInt32("Mode", (int)_mode);
      writer.SetString("select", selecteditem);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      _mode = (FoldMode)reader.GetInt32("Mode");
      first = false;
      selecteditem = reader.GetString("select");
      this.CreateAttributes();
      return base.Read(reader);
    }

    #endregion
    #region IGH_VariableParameterComponent null implementation
    void IGH_VariableParameterComponent.VariableParameterMaintenance()
    {
      if (_mode == FoldMode.One_Dimensional_One_Way)
      {
        string angleUnit = "rad";
        if (_useDegrees)
          angleUnit = "deg";
        Params.Input[5].NickName = "Dir";
        Params.Input[5].Name = "Span Direction [" + angleUnit + "]";
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

    bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index)
    {
      return false;
    }
    bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index)
    {
      return false;
    }
    IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index)
    {
      return null;
    }
    bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index)
    {
      return false;
    }
    #endregion

  }
}
