using System;
using System.Collections.Generic;
using Eto.Forms;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using GsaGH.Parameters;
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
  public class CreateGridPlane : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("675fd47a-890d-45b8-bdde-fb2e8c1d9cca");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.GridPlane;

    public CreateGridPlane() : base("Create Grid Plane",
      "GridPlane",
      "Create GSA Grid Plane",
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat3())
    { }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter("Plane", "P", "Plane for Axis and Grid Plane definition. Note that an XY-plane will be created with an axis origin Z = 0 " +
          "and the height location will be controlled by Grid Plane elevation. For all none-XY plane inputs, the Grid Plane elevation will be 0", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Grid Plane ID", "ID", "GSA Grid Plane ID. Setting this will replace any existing Grid Planes in GSA model", GH_ParamAccess.item, 0);
      pManager.AddGenericParameter("Grid Elevation [" + Length.GetAbbreviation(this.LengthUnit) + "]", "Ev", "Grid Elevation (Optional). Note that this value will be added to Plane origin location in the plane's normal axis direction.", GH_ParamAccess.item);
      pManager.AddTextParameter("Name", "Na", "Grid Plane Name", GH_ParamAccess.item);

      pManager[0].Optional = true;
      pManager[1].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;

      _mode = FoldMode.General;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new GsaGridPlaneParameter(), "Grid Plane", "GP", "GSA Grid Plane", GH_ParamAccess.item);
    }
    #endregion

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
      if (this.Params.Input[2].SourceCount > 0)
      {
        double elev = Input.UnitNumber(this, DA, 2, this.LengthUnit, true).As(LengthUnit.Meter);
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
        if (this.Params.Input[4].SourceCount > 0)
          gps.GridPlane.ToleranceAbove = Input.UnitNumber(this, DA, 4, this.LengthUnit, true).As(LengthUnit.Meter);

        // 5 tolerance below
        if (this.Params.Input[5].SourceCount > 0)
          gps.GridPlane.ToleranceBelow = Input.UnitNumber(this, DA, 5, this.LengthUnit, true).As(LengthUnit.Meter);
      }

      DA.SetData(0, new GsaGridPlaneSurfaceGoo(gps));
    }

    #region Custom UI
    private enum FoldMode
    {
      General,
      Storey
    }
    readonly List<string> _type = new List<string>(new string[]
    {
      "General",
      "Storey"
    });
    FoldMode _mode = FoldMode.General;
    LengthUnit LengthUnit = DefaultUnits.LengthUnitGeometry;
    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[]
        {
          "Type", "Unit"
        });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // Type
      this.DropDownItems.Add(_type);
      this.SelectedItems.Add(this._mode.ToString());

      // Length
      this.DropDownItems.Add(FilteredUnits.FilteredLengthUnits);
      this.SelectedItems.Add(this.LengthUnit.ToString());

      this.IsInitialised = true;
    }
    public override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];
      if (i == 0)
      {
        switch (this.SelectedItems[0])
        {
          case "General":
            Mode1Clicked();
            break;
          case "Storey":
            Mode2Clicked();
            break;
        }
      }
      else
        this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[i]);

      base.UpdateUI();
    }
    public override void UpdateUIFromSelectedItems()
    {
      this._mode = (FoldMode)Enum.Parse(typeof(FoldMode), this.SelectedItems[0]);
      this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[1]);
      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance()
    {
      Params.Input[2].Name = "Grid Elevation[" + Length.GetAbbreviation(this.LengthUnit) + "]";

      if (_mode == FoldMode.Storey)
      {
        Params.Input[4].NickName = "tA";
        Params.Input[4].Name = "Tolerance Above [" + Length.GetAbbreviation(this.LengthUnit) + "]";
        Params.Input[4].Description = "Tolerance Above Grid Plane";
        Params.Input[4].Access = GH_ParamAccess.item;
        Params.Input[4].Optional = true;

        Params.Input[5].NickName = "tB";
        Params.Input[5].Name = "Tolerance Below [" + Length.GetAbbreviation(this.LengthUnit) + "]";
        Params.Input[5].Description = "Tolerance Below Grid Plane";
        Params.Input[5].Access = GH_ParamAccess.item;
        Params.Input[5].Optional = true;
      }
    }
    private void Mode1Clicked()
    {
      if (_mode == FoldMode.General)
        return;

      RecordUndoEvent("General Parameters");
      _mode = FoldMode.General;

      //remove input parameters
      while (Params.Input.Count > 4)
        Params.UnregisterInputParameter(Params.Input[4], true);
    }
    private void Mode2Clicked()
    {
      if (_mode == FoldMode.Storey)
        return;

      RecordUndoEvent("Storey Parameters");
      _mode = FoldMode.Storey;

      //add input parameters
      Params.RegisterInputParam(new Param_GenericObject());
      Params.RegisterInputParam(new Param_GenericObject());
    }
    #endregion

    #region (de)serialization
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      if (reader.ItemExists("Mode"))
      {
        _mode = (FoldMode)reader.GetInt32("Mode");
        this.InitialiseDropdowns();
      }

      return base.Read(reader);
    }
    #endregion
  }
}
