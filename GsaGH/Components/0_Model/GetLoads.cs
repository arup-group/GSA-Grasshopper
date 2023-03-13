// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

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

namespace GsaGH.Components {
  /// <summary>
  /// Component to retrieve non-geometric objects from a GSA model
  /// </summary>
  public class GetLoads : GH_OasysDropDownComponent {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("87ff28e5-a1a6-4d78-ba71-e930e01dca13");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.GetLoads;

    public GetLoads() : base("Get Model Loads",
      "GetLoads",
      "Get Loads and Grid Planes/Surfaces from GSA model",
      CategoryName.Name(),
      SubCategoryName.Cat0()) {
        Hidden = true;
    } // sets the initial state of the component to hidden
    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaModelParameter(), "GSA Model", "GSA", "GSA model containing some loads", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);

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

    protected override void SolveInstance(IGH_DataAccess da) {
      var gsaModel = new GsaModel();
      if (!da.GetData(0, ref gsaModel))
        return;

      Model model = gsaModel.Model;

      List<GsaLoadGoo> gravity = Helpers.Import.Loads.GetGravityLoads(model.GravityLoads());
      List<GsaLoadGoo> node = Helpers.Import.Loads.GetNodeLoads(model);
      List<GsaLoadGoo> beam = Helpers.Import.Loads.GetBeamLoads(model.BeamLoads());
      List<GsaLoadGoo> face = Helpers.Import.Loads.GetFaceLoads(model.FaceLoads());

      IReadOnlyDictionary<int, GridSurface> srfDict = model.GridSurfaces();
      IReadOnlyDictionary<int, GridPlane> plnDict = model.GridPlanes();
      IReadOnlyDictionary<int, Axis> axDict = model.Axes();
      List<GsaLoadGoo> point = Helpers.Import.Loads.GetGridPointLoads(model.GridPointLoads(), srfDict, plnDict, axDict, _lengthUnit);
      List<GsaLoadGoo> line = Helpers.Import.Loads.GetGridLineLoads(model.GridLineLoads(), srfDict, plnDict, axDict, _lengthUnit);
      List<GsaLoadGoo> area = Helpers.Import.Loads.GetGridAreaLoads(model.GridAreaLoads(), srfDict, plnDict, axDict, _lengthUnit);

      var gps = srfDict.Keys.Select(key => new GsaGridPlaneSurfaceGoo(Helpers.Import.Loads.GetGridPlaneSurface(srfDict, plnDict, axDict, key, _lengthUnit))).ToList();

      da.SetDataList(0, gravity);
      da.SetDataList(1, node);
      da.SetDataList(2, beam);
      da.SetDataList(3, face);
      da.SetDataList(4, point);
      da.SetDataList(5, line);
      da.SetDataList(6, area);
      da.SetDataList(7, gps);
    }

    #region Custom UI
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitGeometry;

    public override void InitialiseDropdowns() {
      SpacerDescriptions = new List<string>(new[]
        {
          "Unit",
        });

      DropDownItems = new List<List<string>>();
      SelectedItems = new List<string>();

      // Length
      DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      SelectedItems.Add(Length.GetAbbreviation(_lengthUnit));

      IsInitialised = true;
    }

    public override void SetSelected(int i, int j) {
      SelectedItems[i] = DropDownItems[i][j];
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), SelectedItems[i]);
      base.UpdateUI();
    }
    public override void UpdateUIFromSelectedItems() {
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), SelectedItems[0]);
      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance() {
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);
      int i = 4;
      Params.Output[i++].Name = "Grid Point Loads [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "Grid Line Loads [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "Grid Area Loads [" + unitAbbreviation + "]";
      Params.Output[i].Name = "Grid Plane Surfaces [" + unitAbbreviation + "]";
    }
    #endregion
  }
}

