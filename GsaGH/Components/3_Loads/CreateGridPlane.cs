using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using GsaGH.Parameters;

namespace GsaGH.Components
{
  public class CreateGridPlane : GH_OasysComponent, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    public CreateGridPlane()
        : base("Create Grid Plane", "GridPlane", "Create GSA Grid Plane",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat3())
    { }
    public override Guid ComponentGuid => new Guid("675fd47a-890d-45b8-bdde-fb2e8c1d9cca");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;

    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.GridPlane;
    #endregion

    #region Custom UI
    //This region overrides the typical component layout
    public override void CreateAttributes()
    {
      if (first)
      {
        selecteditem = _mode.ToString();
        first = false;
      }

      m_attributes = new UI.DropDownComponentUI(this, SetSelected, dropdownitems, selecteditem, "Type");
    }

    public void SetSelected(string selected)
    {
      selecteditem = selected;
      switch (selected)
      {
        case "General":
          Mode1Clicked();
          break;
        case "Storey":
          Mode2Clicked();
          break;
      }
    }
    #endregion

    #region Input and output
    readonly List<string> dropdownitems = new List<string>(new string[]
    {
            "General",
            "Storey"
    });

    string selecteditem;

    #endregion

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter("Plane", "P", "Plane for Axis and Grid Plane definition. Note that an XY-plane will be created with an axis origin Z = 0 " +
          "and the height location will be controlled by Grid Plane elevation. For all none-XY plane inputs, the Grid Plane elevation will be 0", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Grid Plane ID", "ID", "GSA Grid Plane ID. Setting this will replace any existing Grid Planes in GSA model", GH_ParamAccess.item, 0);
      pManager.AddNumberParameter("Grid Elevation", "Ev", "Grid Elevation (Optional). Note that this value will be added to Plane origin location in the plane's normal axis direction.", GH_ParamAccess.item, 0);
      pManager.AddTextParameter("Name", "Na", "Grid Plane Name", GH_ParamAccess.item);

      pManager[0].Optional = true;
      pManager[1].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;

      _mode = FoldMode.General;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Grid Plane", "GP", "GSA Grid Plane", GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      Plane pln = Plane.WorldXY;

      // 0 Plane
      GH_Plane gh_pln = new GH_Plane();
      if (DA.GetData(0, ref gh_pln))
        GH_Convert.ToPlane(gh_pln, ref pln, GH_Conversion.Both);

      // create gsa gridplanesurface from plane
      GsaGridPlaneSurface gps = new GsaGridPlaneSurface(pln);

      // 1 Grid plane ID
      GH_Integer ghint = new GH_Integer();
      if (DA.GetData(1, ref ghint))
      {
        int id = 0;
        GH_Convert.ToInt32(ghint, out id, GH_Conversion.Both);
        gps.GridPlaneID = id;
      }

      // 2 Grid elevation
      GH_Number ghnum = new GH_Number();
      if (DA.GetData(2, ref ghnum))
      {
        double elev = 0;
        if (GH_Convert.ToDouble(ghnum, out elev, GH_Conversion.Both))
        {
          gps.GridPlane.Elevation = elev;

          // if elevation is set we want to move the plane in it's normal direction
          Vector3d vec = pln.Normal;
          vec.Unitize();
          vec.X *= elev;
          vec.Y *= elev;
          vec.Z *= elev;
          Transform xform = Transform.Translation(vec);
          pln.Transform(xform);
          gps.Plane = pln;
          // note this wont move the Grid Plane Axis gps.Axis
        }
      }

      // 3 Name
      GH_String ghtxt = new GH_String();
      if (DA.GetData(3, ref ghtxt))
      {
        string name = "";
        if (GH_Convert.ToString(ghtxt, out name, GH_Conversion.Both))
          gps.GridPlane.Name = name;
      }

      // set is story
      if (_mode == FoldMode.General)
        gps.GridPlane.IsStoreyType = false;
      else
      {
        gps.GridPlane.IsStoreyType = true;

        // 4 tolerance above
        GH_Number ghtola = new GH_Number();
        if (DA.GetData(4, ref ghtola))
        {
          double tola = 0;
          if (GH_Convert.ToDouble(ghtola, out tola, GH_Conversion.Both))
            gps.GridPlane.ToleranceAbove = tola;
        }

        // 5 tolerance above
        GH_Number ghtolb = new GH_Number();
        if (DA.GetData(5, ref ghtolb))
        {
          double tolb = 0;
          if (GH_Convert.ToDouble(ghtolb, out tolb, GH_Conversion.Both))
            gps.GridPlane.ToleranceBelow = tolb;
        }
      }

      DA.SetData(0, new GsaGridPlaneSurfaceGoo(gps));

    }

    #region menu override
    private enum FoldMode
    {
      General,
      Storey
    }
    private bool first = true;
    private FoldMode _mode = FoldMode.General;

    private void Mode1Clicked()
    {
      if (_mode == FoldMode.General)
        return;

      RecordUndoEvent("General Parameters");
      _mode = FoldMode.General;

      //remove input parameters
      while (Params.Input.Count > 4)
        Params.UnregisterInputParameter(Params.Input[4], true);

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();
      ExpireSolution(true);
    }
    private void Mode2Clicked()
    {
      if (_mode == FoldMode.Storey)
        return;

      RecordUndoEvent("Storey Parameters");
      _mode = FoldMode.Storey;

      //add input parameters
      Params.RegisterInputParam(new Param_Number());
      Params.RegisterInputParam(new Param_Number());

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
      selecteditem = reader.GetString("select");
      this.CreateAttributes();
      return base.Read(reader);
    }

    #endregion
    #region IGH_VariableParameterComponent null implementation
    void IGH_VariableParameterComponent.VariableParameterMaintenance()
    {
      if (_mode == FoldMode.Storey)
      {
        Params.Input[4].NickName = "tA";
        Params.Input[4].Name = "Tolerance Above (" + Units.LengthUnitGeometry + ")";
        Params.Input[4].Description = "Tolerance Above Grid Plane";
        Params.Input[4].Access = GH_ParamAccess.item;
        Params.Input[4].Optional = true;

        Params.Input[5].NickName = "tB";
        Params.Input[5].Name = "Tolerance Below (" + Units.LengthUnitGeometry + ")";
        Params.Input[5].Description = "Tolerance Above Grid Plane";
        Params.Input[5].Access = GH_ParamAccess.item;
        Params.Input[5].Optional = true;
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
