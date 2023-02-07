using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to retrieve non-geometric objects from a GSA model
  /// </summary>
  public class GetLoads : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("87ff28e5-a1a6-4d78-ba71-e930e01dca13");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.GetLoads;

    public GetLoads() : base("Get Model Loads",
      "GetLoads",
      "Get Loads and Grid Planes/Surfaces from GSA model",
      CategoryName.Name(),
      SubCategoryName.Cat0())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddParameter(new GsaModelParameter(), "GSA Model", "GSA", "GSA model containing some loads", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      string unitAbbreviation = Length.GetAbbreviation(this.LengthUnit);

      pManager.AddParameter(new GsaLoadParameter(), "Gravity Loads", "Gr", "Gravity Loads from GSA Model", GH_ParamAccess.list);
      pManager.AddParameter(new GsaLoadParameter(), "Node Loads", "No", "Node Loads from GSA Model", GH_ParamAccess.list);
      pManager.AddParameter(new GsaLoadParameter(), "Beam Loads", "Be", "Beam Loads from GSA Model", GH_ParamAccess.list);
      pManager.AddParameter(new GsaLoadParameter(), "Face Loads", "Fa", "Face Loads from GSA Model", GH_ParamAccess.list);
      pManager.AddParameter(new GsaLoadParameter(), "Grid Point Loads [" + unitAbbreviation + "]", "Pt", "Grid Point Loads from GSA Model", GH_ParamAccess.list);
      pManager.AddParameter(new GsaLoadParameter(), "Grid Line Loads [" + unitAbbreviation + "]", "Ln", "Grid Line Loads from GSA Model", GH_ParamAccess.list);
      pManager.AddParameter(new GsaLoadParameter(), "Grid Area Loads [" + unitAbbreviation + "]", "Ar", "Grid Area Loads from GSA Model", GH_ParamAccess.list);
      pManager.AddParameter(new GsaGridPlaneParameter(), "Grid Plane Surfaces [" + unitAbbreviation + "]", "GPS", "Grid Plane Surfaces from GSA Model", GH_ParamAccess.list);
      pManager.HideParameter(7);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GsaModel gsaModel = new GsaModel();
      if (DA.GetData(0, ref gsaModel))
      {
        Model model = new Model();
        model = gsaModel.Model;

        List<GsaLoadGoo> gravity = Helpers.Import.Loads.GetGravityLoads(model.GravityLoads());
        List<GsaLoadGoo> node = Helpers.Import.Loads.GetNodeLoads(model);
        List<GsaLoadGoo> beam = Helpers.Import.Loads.GetBeamLoads(model.BeamLoads());
        List<GsaLoadGoo> face = Helpers.Import.Loads.GetFaceLoads(model.FaceLoads());

        IReadOnlyDictionary<int, GridSurface> srfDict = model.GridSurfaces();
        IReadOnlyDictionary<int, GridPlane> plnDict = model.GridPlanes();
        IReadOnlyDictionary<int, Axis> axDict = model.Axes();
        List<GsaLoadGoo> point = Helpers.Import.Loads.GetGridPointLoads(model.GridPointLoads(), srfDict, plnDict, axDict, this.LengthUnit);
        List<GsaLoadGoo> line = Helpers.Import.Loads.GetGridLineLoads(model.GridLineLoads(), srfDict, plnDict, axDict, this.LengthUnit);
        List<GsaLoadGoo> area = Helpers.Import.Loads.GetGridAreaLoads(model.GridAreaLoads(), srfDict, plnDict, axDict, this.LengthUnit);

        List<GsaGridPlaneSurfaceGoo> gps = new List<GsaGridPlaneSurfaceGoo>();

        foreach (int key in srfDict.Keys)
          gps.Add(new GsaGridPlaneSurfaceGoo(Helpers.Import.Loads.GetGridPlaneSurface(srfDict, plnDict, axDict, key, this.LengthUnit)));

        DA.SetDataList(0, gravity);
        DA.SetDataList(1, node);
        DA.SetDataList(2, beam);
        DA.SetDataList(3, face);
        DA.SetDataList(4, point);
        DA.SetDataList(5, line);
        DA.SetDataList(6, area);
        DA.SetDataList(7, gps);
      }
    }

    #region Custom UI
    private LengthUnit LengthUnit = DefaultUnits.LengthUnitGeometry;

    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[]
        {
          "Unit"
        });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // Length
      this.DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      this.SelectedItems.Add(Length.GetAbbreviation(this.LengthUnit));

      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];
      this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), this.SelectedItems[i]);
      base.UpdateUI();
    }
    public override void UpdateUIFromSelectedItems()
    {
      this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), this.SelectedItems[0]);
      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance()
    {
      string unitAbbreviation = Length.GetAbbreviation(this.LengthUnit);
      int i = 4;
      Params.Output[i++].Name = "Grid Point Loads [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "Grid Line Loads [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "Grid Area Loads [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "Grid Plane Surfaces [" + unitAbbreviation + "]";
    }
    #endregion
  }
}

