﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaAPI;
using GsaGH.Helpers.Import;
using OasysUnits;
using Rhino.Geometry;
using LengthUnit = OasysUnits.Units.LengthUnit;
using NodeLoadType = GsaGH.Parameters.NodeLoadType;

namespace GsaGH.Parameters {
  public static class GsaLoadFactory {
    /// <summary>
    ///   Method to import Load Cases from a GSA model.
    ///   Will output a list of GsaLoadCase.
    /// </summary>
    /// <param name="loadCases"></param>
    /// <returns></returns>
    internal static List<GsaLoadCaseGoo> CreateLoadCasesFromApi(ReadOnlyDictionary<int, LoadCase> loadCases) {
      var cases = new List<GsaLoadCaseGoo>();
      foreach (KeyValuePair<int, LoadCase> kvp in loadCases) {
        cases.Add(new GsaLoadCaseGoo(new GsaLoadCase(kvp.Key, loadCases)));
      }

      return cases;
    }

    /// <summary>
    ///   Method to import Beam Loads from a GSA model.
    ///   Will output a list of GsaLoads.
    /// </summary>
    /// <param name="beamLoads">Collection of beams loads to be imported</param>
    /// <param name="loadCases"></param>
    /// <returns></returns>
    internal static List<GsaLoadGoo> CreateBeamLoadsFromApi(
      ReadOnlyCollection<BeamLoad> beamLoads, ReadOnlyDictionary<int, LoadCase> loadCases) {
      var loads = new List<GsaLoadGoo>();
      foreach (BeamLoad apiLoad in beamLoads) {
        var load = new GsaBeamLoad {
          ApiLoad = apiLoad,
          ReferenceList = new GsaList(apiLoad.Name, apiLoad.EntityList, apiLoad.EntityType)
        };
        load.LoadCase = new GsaLoadCase(load.ApiLoad.Case, loadCases);
        loads.Add(new GsaLoadGoo(load));
      }

      return loads;
    }

    /// <summary>
    ///   Method to import Beam Thermal Loads from a GSA model.
    ///   Will output a list of GsaLoads.
    /// </summary>
    /// <param name="beamThermalLoads">Collection of beam thermal loads to be imported</param>
    /// <param name="loadCases"></param>
    /// <returns></returns>
    internal static List<GsaLoadGoo> CreateBeamThermalLoadsFromApi(
      ReadOnlyCollection<BeamThermalLoad> beamThermalLoads, ReadOnlyDictionary<int, LoadCase> loadCases) {
      var loads = new List<GsaLoadGoo>();
      foreach (BeamThermalLoad apiLoad in beamThermalLoads) {
        var load = new GsaBeamThermalLoad {
          ApiLoad = apiLoad,
          ReferenceList = new GsaList(apiLoad.Name, apiLoad.EntityList, apiLoad.EntityType)
        };
        load.LoadCase = new GsaLoadCase(load.ApiLoad.Case, loadCases);
        loads.Add(new GsaLoadGoo(load));
      }

      return loads;
    }

    /// <summary>
    ///   Method to import Face Loads from a GSA model.
    ///   Will output a list of GsaLoads.
    /// </summary>
    /// <param name="faceLoads">Collection of Face loads to be imported</param>
    /// <param name="loadCases"></param>
    /// <returns></returns>
    internal static List<GsaLoadGoo> CreateFaceLoadsFromApi(ReadOnlyCollection<FaceLoad> faceLoads,
      ReadOnlyDictionary<int, LoadCase> loadCases) {
      var loads = new List<GsaLoadGoo>();
      foreach (FaceLoad apiLoad in faceLoads) {
        var load = new GsaFaceLoad {
          ApiLoad = apiLoad,
          ReferenceList = new GsaList(apiLoad.Name, apiLoad.EntityList, apiLoad.EntityType)
        };
        load.LoadCase = new GsaLoadCase(load.ApiLoad.Case, loadCases);
        loads.Add(new GsaLoadGoo(load));
      }

      return loads;
    }

    /// <summary>
    ///   Method to import Face Thermal Loads from a GSA model.
    ///   Will output a list of GsaLoads.
    /// </summary>
    /// <param name="faceThermalLoads">Collection of Face Thermal loads to be imported</param>
    /// <param name="loadCases"></param>
    /// <returns></returns>
    internal static List<GsaLoadGoo> CreateFaceThermalLoadsFromApi(
      ReadOnlyCollection<FaceThermalLoad> faceThermalLoads, ReadOnlyDictionary<int, LoadCase> loadCases) {
      var loads = new List<GsaLoadGoo>();
      foreach (FaceThermalLoad apiLoad in faceThermalLoads) {
        var load = new GsaFaceThermalLoad {
          ApiLoad = apiLoad,
          ReferenceList = new GsaList(apiLoad.Name, apiLoad.EntityList, apiLoad.EntityType)
        };
        load.LoadCase = new GsaLoadCase(load.ApiLoad.Case, loadCases);
        loads.Add(new GsaLoadGoo(load));
      }

      return loads;
    }

    /// <summary>
    ///   Method to import Gravity Loads from a GSA model.
    ///   Will output a list of GsaLoadsGoo.
    /// </summary>
    /// <param name="gravityLoads">Collection of gravity loads to import</param>
    /// <param name="loadCases"></param>
    /// <returns></returns>
    internal static List<GsaLoadGoo> CreateGravityLoadsFromApi(
      ReadOnlyCollection<GravityLoad> gravityLoads, ReadOnlyDictionary<int, LoadCase> loadCases) {
      var loads = new List<GsaLoadGoo>();
      foreach (GravityLoad apiLoad in gravityLoads) {
        var load = new GsaGravityLoad {
          ApiLoad = apiLoad,
          ReferenceList = new GsaList(apiLoad.Name, apiLoad.EntityList, apiLoad.EntityType)
        };
        load.LoadCase = new GsaLoadCase(load.ApiLoad.Case, loadCases);
        loads.Add(new GsaLoadGoo(load));
      }

      return loads;
    }

    /// <summary>
    ///   Method to import Grid Area Loads from a GSA model.
    ///   Will output a list of GsaLoads.
    /// </summary>
    /// <param name="areaLoads">Collection of Grid Area loads to be imported</param>
    /// <param name="srfDict">Grid Surface Dictionary</param>
    /// <param name="plnDict">Grid Plane Dictionary</param>
    /// <param name="axDict">Axes Dictionary</param>
    /// <param name="loadCases"></param>
    /// <param name="unit"></param>
    /// <returns></returns>
    internal static List<GsaLoadGoo> CreateGridAreaLoadsFromApi(
      ReadOnlyCollection<GridAreaLoad> areaLoads, IReadOnlyDictionary<int, GridSurface> srfDict,
      IReadOnlyDictionary<int, GridPlane> plnDict, IReadOnlyDictionary<int, Axis> axDict,
      ReadOnlyDictionary<int, LoadCase> loadCases, LengthUnit unit) {
      var loads = new List<GsaLoadGoo>();
      foreach (GridAreaLoad gridAreaLoad in areaLoads) {
        var load = new GsaGridAreaLoad {
          ApiLoad = gridAreaLoad,
          GridPlaneSurface
            = CreateGridPlaneSurfaceFromApi(srfDict, plnDict, axDict, gridAreaLoad.GridSurface, unit)
        };

        if (gridAreaLoad.PolyLineDefinition != string.Empty
          && gridAreaLoad.PolyLineDefinition.Contains('(')
          && load.GridPlaneSurface != null) {
          load.Points = GridLoadHelper.ConvertPoints(
            gridAreaLoad.PolyLineDefinition.ToString(), unit, load.GridPlaneSurface.Plane);
        }

        load.LoadCase = new GsaLoadCase(load.ApiLoad.Case, loadCases);
        loads.Add(new GsaLoadGoo(load));
      }

      return loads;
    }

    /// <summary>
    ///   Method to import Grid Line Loads from a GSA model.
    ///   Will output a list of GsaLoads.
    /// </summary>
    /// <param name="lineLoads">Collection of Grid Line loads to be imported</param>
    /// <param name="srfDict">Grid Surface Dictionary</param>
    /// <param name="plnDict">Grid Plane Dictionary</param>
    /// <param name="axDict">Axes Dictionary</param>
    /// <param name="loadCases"></param>
    /// <param name="unit"></param>
    /// <returns></returns>
    internal static List<GsaLoadGoo> CreateGridLineLoadsFromApi(
      ReadOnlyCollection<GridLineLoad> lineLoads, IReadOnlyDictionary<int, GridSurface> srfDict,
      IReadOnlyDictionary<int, GridPlane> plnDict, IReadOnlyDictionary<int, Axis> axDict,
      ReadOnlyDictionary<int, LoadCase> loadCases, LengthUnit unit) {
      var loads = new List<GsaLoadGoo>();
      foreach (GridLineLoad gridLineLoad in lineLoads) {
        var load = new GsaGridLineLoad {
          ApiLoad = gridLineLoad,
          GridPlaneSurface
            = CreateGridPlaneSurfaceFromApi(srfDict, plnDict, axDict, gridLineLoad.GridSurface, unit)
        };

        if (gridLineLoad.PolyLineDefinition != string.Empty
          && gridLineLoad.PolyLineDefinition.Contains('(')
          && load.GridPlaneSurface != null) {
          load.Points = GridLoadHelper.ConvertPoints(
          gridLineLoad.PolyLineDefinition.ToString(), unit, load.GridPlaneSurface.Plane);
        }

        load.LoadCase = new GsaLoadCase(load.ApiLoad.Case, loadCases);
        loads.Add(new GsaLoadGoo(load));
      }

      return loads;
    }

    /// <summary>
    ///   Method to create GsaGridPlaneSurface including
    ///   grid surface, grid plane and axis from GSA Model
    ///   Grid Surface references a Grid Plane
    ///   Grid Plane references an Axis
    ///   Only Grid Surface ID is required, the others will be found by ref
    ///   Will output a new GsaGridPlaneSurface.
    /// </summary>
    /// <param name="srfDict">Grid Surface Dictionary</param>
    /// <param name="plnDict">Grid Plane Dictionary</param>
    /// <param name="axDict">Axes Dictionary</param>
    /// <param name="gridSrfId">ID/Key/number of Grid Surface in GSA model to convert</param>
    /// <param name="unit"></param>
    /// <returns></returns>
    internal static GsaGridPlaneSurface CreateGridPlaneSurfaceFromApi(
      IReadOnlyDictionary<int, GridSurface> srfDict, IReadOnlyDictionary<int, GridPlane> plnDict,
      IReadOnlyDictionary<int, Axis> axDict, int gridSrfId, LengthUnit unit) {
      if (srfDict.Count == 0 || !srfDict.TryGetValue(gridSrfId, out GridSurface gs)) {
        return null;
      }

      var gps = new GsaGridPlaneSurface {
        GridSurface = gs,
        GridSurfaceId = gridSrfId,
        Tolerance = new Length(gs.Tolerance, LengthUnit.Meter).ToUnit(unit).ToString()
       .Replace(" ", string.Empty).Replace(",", string.Empty)
      };

      if (plnDict.TryGetValue(gs.GridPlane, out GridPlane gp)) {
        gps.GridPlane = gp;
        gps.GridPlaneId = gs.GridPlane;
        gps.SetElevation(new Length(gp.Elevation, LengthUnit.Meter));
        gps.StoreyToleranceAbove = gp.ToleranceAbove == 0 ? "auto" :
          new Length(gp.ToleranceAbove, LengthUnit.Meter).ToUnit(unit).ToString()
           .Replace(" ", string.Empty).Replace(",", string.Empty);
        gps.StoreyToleranceBelow = gp.ToleranceBelow == 0 ? "auto" :
          new Length(gp.ToleranceBelow, LengthUnit.Meter).ToUnit(unit).ToString()
           .Replace(" ", string.Empty).Replace(",", string.Empty);
        gps.AxisId = gp.AxisProperty;
        Plane plane;
        if (axDict.TryGetValue(gp.AxisProperty, out Axis ax)) {
          // for new origin Z-coordinate we add axis origin and grid plane elevation
          plane = new Plane(
            Nodes.Point3dFromXyzUnit(ax.Origin.X, ax.Origin.Y, ax.Origin.Z + gp.Elevation, unit),
            Nodes.Vector3dFromXyzUnit(ax.XVector.X, ax.XVector.Y, ax.XVector.Z, unit),
            Nodes.Vector3dFromXyzUnit(ax.XYPlane.X, ax.XYPlane.Y, ax.XYPlane.Z, unit));
        } else {
          plane = Plane.WorldXY;
          plane.OriginZ = new Length(gp.Elevation, LengthUnit.Meter).As(unit);
        }
        gps.Plane = plane;
      }

      return gps;
    }

    /// <summary>
    ///   Method to import Grid Point Loads from a GSA model.
    ///   Will output a list of GsaLoads.
    /// </summary>
    /// <param name="pointLoads">Collection of Grid Point loads to be imported</param>
    /// <param name="srfDict">Grid Surface Dictionary</param>
    /// <param name="plnDict">Grid Plane Dictionary</param>
    /// <param name="axDict">Axes Dictionary</param>
    /// <param name="loadCases"></param>
    /// <param name="unit"></param>
    /// <returns></returns>
    internal static List<GsaLoadGoo> CreateGridPointLoadsFromApi(
      ReadOnlyCollection<GridPointLoad> pointLoads, IReadOnlyDictionary<int, GridSurface> srfDict,
      IReadOnlyDictionary<int, GridPlane> plnDict, IReadOnlyDictionary<int, Axis> axDict,
      ReadOnlyDictionary<int, LoadCase> loadCases, LengthUnit unit) {
      var loads = new List<GsaLoadGoo>();
      foreach (GridPointLoad gridPointLoad in pointLoads) {
        var load = new GsaGridPointLoad {
          ApiLoad = gridPointLoad,
          GridPlaneSurface
            = CreateGridPlaneSurfaceFromApi(srfDict, plnDict, axDict, gridPointLoad.GridSurface, unit), };

        if (unit != LengthUnit.Meter) {
          load.ApiLoad.X = new Length(load.ApiLoad.X, LengthUnit.Meter).As(unit);
          load.ApiLoad.Y = new Length(load.ApiLoad.Y, LengthUnit.Meter).As(unit);
        }

        load.LoadCase = new GsaLoadCase(load.ApiLoad.Case, loadCases);
        loads.Add(new GsaLoadGoo(load));
      }

      return loads;
    }

    /// <summary>
    ///   Method to import all Node Loads from a GSA model.
    ///   GSA Node loads vary by type, to get all node loads easiest
    ///   method seems to be toogling through all enum types which
    ///   requeres the entire model to be inputted to this method.
    ///   Will output a list of GsaLoads.
    /// </summary>
    /// <param name="model">GSA model containing node loads</param>
    /// <param name="loadCases"></param>
    /// <returns></returns>
    internal static List<GsaLoadGoo> CreateNodeLoadsFromApi(Model model, ReadOnlyDictionary<int, LoadCase> loadCases) {
      var loads = new List<GsaLoadGoo>();

      // NodeLoads come in varioys types, depending on GsaAPI.NodeLoadType:
      foreach (GsaAPI.NodeLoadType typ in Enum.GetValues(typeof(GsaAPI.NodeLoadType))) {
        try // some GsaAPI.NodeLoadTypes are currently not supported in the API and throws an error
        {
          var gsaloads = model.NodeLoads(typ).ToList();
          NodeLoadType ntyp = NodeLoadType.NodeLoad;
          switch (typ) {
            case GsaAPI.NodeLoadType.APPL_DISP:
              ntyp = NodeLoadType.AppliedDisp;
              break;

            case GsaAPI.NodeLoadType.GRAVITY:
              ntyp = NodeLoadType.Gravity;
              break;

            case GsaAPI.NodeLoadType.NODE_LOAD:
              ntyp = NodeLoadType.NodeLoad;
              break;

            case GsaAPI.NodeLoadType.NUM_TYPES:
              ntyp = NodeLoadType.NumTypes;
              break;

            case GsaAPI.NodeLoadType.SETTLEMENT:
              ntyp = NodeLoadType.Settlement;
              break;
          }

          foreach (NodeLoad gsaLoad in gsaloads) {
            var load = new GsaNodeLoad {
              ApiLoad = gsaLoad,
              Type = ntyp,
            };
            load.LoadCase = new GsaLoadCase(load.ApiLoad.Case, loadCases);
            loads.Add(new GsaLoadGoo(load));
          }
        } catch (Exception) {
          // ignored
        }
      }

      return loads;
    }
  }
}