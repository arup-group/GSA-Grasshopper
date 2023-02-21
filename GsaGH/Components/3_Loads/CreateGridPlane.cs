using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysUnits;
using Rhino.Geometry;

namespace GsaGH.Components
{
  public class CreateGridPlane : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("95c9281a-739b-4480-a2d0-8b04ab0250bd");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.GridPlane;

    public CreateGridPlane() : base("Create Grid Plane",
      "GridPlane",
      "Create GSA Grid Plane",
      CategoryName.Name(),
      SubCategoryName.Cat3())
    { }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter("Plane", "P", "Plane for Axis and Grid Plane definition. Note that an XY-plane will be created with an axis origin Z = 0 " +
          "and the height location will be controlled by Grid Plane elevation. For all none-XY plane inputs, the Grid Plane elevation will be 0", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Grid Plane ID", "ID", "GSA Grid Plane ID. Setting this will replace any existing Grid Planes in GSA model", GH_ParamAccess.item, 0);
      pManager.AddGenericParameter("Grid Elevation in model units", "Ev", "Grid Elevation (Optional). Note that this value will be added to Plane origin location in the plane's normal axis direction.", GH_ParamAccess.item);
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
        gps.GridPlaneId = id;
      }

      // 2 Grid elevation
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(2, ref gh_typ))
      {
        string elevation_in = gh_typ.Value.ToString();
        double elevation = 0;
        if (elevation_in != "" && elevation_in.ToLower() != "0")
        {
          try
          {
            Length newElevation = Length.Parse(elevation_in);
            gps.Elevation = elevation_in;
            elevation = newElevation.Value;
          }
          catch (Exception e)
          {
            if (double.TryParse(elevation_in, out elevation))
              gps.Elevation = elevation_in;
            else
              AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, e.Message);
          }
        }
        if (elevation != 0)
        {
          // if elevation is set we want to move the plane in it's normal direction
          Vector3d vec = pln.Normal;
          vec.Unitize();
          vec.X *= elevation;
          vec.Y *= elevation;
          vec.Z *= elevation;
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
        gh_typ = new GH_ObjectWrapper();
        if (DA.GetData(4, ref gh_typ))
        {
          string tol_in = gh_typ.Value.ToString();
          if (tol_in != "" && tol_in.ToLower() != "auto")
          {
            try
            {
              Length newTolerance = Length.Parse(tol_in);
              gps.StoreyToleranceAbove = tol_in;
            }
            catch (Exception e)
            {
              if (double.TryParse(tol_in, out double tolerance))
                gps.StoreyToleranceAbove = tol_in;
              else
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, e.Message);
            }
          }
        }

        // 5 tolerance below
        gh_typ = new GH_ObjectWrapper();
        if (DA.GetData(5, ref gh_typ))
        {
          string tol_in = gh_typ.Value.ToString();
          if (tol_in != "" && tol_in.ToLower() != "auto")
          {
            try
            {
              Length newTolerance = Length.Parse(tol_in);
              gps.StoreyToleranceBelow = tol_in;
            }
            catch (Exception e)
            {
              if (double.TryParse(tol_in, out double tolerance))
                gps.StoreyToleranceBelow = tol_in;
              else
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, e.Message);
            }
          }
        }
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

    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[]
        {
          "Type"
        });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // Type
      this.DropDownItems.Add(_type);
      this.SelectedItems.Add(this._mode.ToString());

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

      base.UpdateUI();
    }
    public override void UpdateUIFromSelectedItems()
    {
      this._mode = (FoldMode)Enum.Parse(typeof(FoldMode), this.SelectedItems[0]);
      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance()
    {
      Params.Input[2].Name = "Grid Elevation in model units";

      if (_mode == FoldMode.Storey)
      {
        if (Params.Input.Count < 5)
        {
          Params.RegisterInputParam(new Param_GenericObject());
          Params.RegisterInputParam(new Param_GenericObject());
        }

        Params.Input[4].NickName = "tA";
        Params.Input[4].Name = "Tolerance Above";
        Params.Input[4].Description = "Tolerance Above Grid Plane";
        Params.Input[4].Access = GH_ParamAccess.item;
        Params.Input[4].Optional = true;

        Params.Input[5].NickName = "tB";
        Params.Input[5].Name = "Tolerance Below";
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

      if (_mode == FoldMode.Storey && this.Params.Input.Count < 5)
      {
        Params.RegisterInputParam(new Param_GenericObject());
        Params.RegisterInputParam(new Param_GenericObject());
      }

      return base.Read(reader);
    }
    #endregion
  }
}
