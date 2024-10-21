using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using GsaAPI;

using GsaGH.Helpers.Import;

using OasysUnits;

using Rhino.Geometry;

using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Parameters {
  public static class GsaLoadFactory {
    /// <summary>
    ///   Method to import Load Cases from a GSA model.
    ///   Will output a list of GsaLoadCase.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    internal static List<GsaLoadCaseGoo> CreateLoadCasesFromApi(Model model) {
      ReadOnlyDictionary<int, LoadCase> loadCases = model.LoadCases();

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
    /// <param name="model"></param>
    /// <returns></returns>
    internal static List<GsaLoadGoo> CreateBeamLoadsFromApi(Model model) {
      ReadOnlyCollection<BeamLoad> beamLoads = model.BeamLoads();
      ReadOnlyDictionary<int, LoadCase> loadCases = model.LoadCases();

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
    /// <param name="model"></param>
    /// <returns></returns>
    internal static List<GsaLoadGoo> CreateBeamThermalLoadsFromApi(Model model) {
      ReadOnlyCollection<BeamThermalLoad> beamThermalLoads = model.BeamThermalLoads();
      ReadOnlyDictionary<int, LoadCase> loadCases = model.LoadCases();

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
    /// <param name="model"></param>
    /// <returns></returns>
    internal static List<GsaLoadGoo> CreateFaceLoadsFromApi(Model model) {
      ReadOnlyCollection<FaceLoad> faceLoads = model.FaceLoads();
      ReadOnlyDictionary<int, LoadCase> loadCases = model.LoadCases();

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
    /// <param name="model"></param>
    /// <returns></returns>
    internal static List<GsaLoadGoo> CreateFaceThermalLoadsFromApi(Model model) {
      ReadOnlyCollection<FaceThermalLoad> faceThermalLoads = model.FaceThermalLoads();
      ReadOnlyDictionary<int, LoadCase> loadCases = model.LoadCases();

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
    /// <param name="model"></param>
    /// <returns></returns>
    internal static List<GsaLoadGoo> CreateGravityLoadsFromApi(Model model) {
      ReadOnlyCollection<GravityLoad> gravityLoads = model.GravityLoads();
      ReadOnlyDictionary<int, LoadCase> loadCases = model.LoadCases();

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
    /// <param name="model"></param>
    /// <param name="unit"></param>
    /// <returns></returns>
    internal static List<GsaLoadGoo> CreateGridAreaLoadsFromApi(Model model, LengthUnit unit) {
      ReadOnlyCollection<GridAreaLoad> areaLoads = model.GridAreaLoads();
      IReadOnlyDictionary<int, GridSurface> srfDict = model.GridSurfaces();
      IReadOnlyDictionary<int, GridPlane> plnDict = model.GridPlanes();
      IReadOnlyDictionary<int, Axis> axDict = model.Axes();
      ReadOnlyDictionary<int, LoadCase> loadCases = model.LoadCases();
      ReadOnlyDictionary<int, GsaAPI.Polyline> polylines = model.Polylines();

      var loads = new List<GsaLoadGoo>();
      foreach (GridAreaLoad gridAreaLoad in areaLoads) {
        var load = new GsaGridAreaLoad {
          ApiLoad = gridAreaLoad,
          GridPlaneSurface = CreateGridPlaneSurfaceFromApi(model, gridAreaLoad.GridSurface, unit)
        };

        if (load.GridPlaneSurface != null) {
          if (gridAreaLoad.PolyLineDefinition != string.Empty && gridAreaLoad.PolyLineDefinition.Contains('(')) {
            load.Points = GridLoadHelper.ConvertPoints(gridAreaLoad.PolyLineDefinition.ToString(), unit, load.GridPlaneSurface.Plane);
          } else if (polylines.ContainsKey(gridAreaLoad.PolyLineReference)) {
            GsaAPI.Polyline polyline = polylines[gridAreaLoad.PolyLineReference];
            load.Points = GridLoadHelper.ConvertPoints(polyline.Points, unit, load.GridPlaneSurface.Plane);
            load.ApiPolyline = polyline;
          }
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
    /// <param name="model"></param>
    /// <param name="unit"></param>
    /// <returns></returns>
    internal static List<GsaLoadGoo> CreateGridLineLoadsFromApi(Model model, LengthUnit unit) {
      ReadOnlyCollection<GridLineLoad> lineLoads = model.GridLineLoads();
      ReadOnlyCollection<GridAreaLoad> areaLoads = model.GridAreaLoads();
      IReadOnlyDictionary<int, GridSurface> srfDict = model.GridSurfaces();
      IReadOnlyDictionary<int, GridPlane> plnDict = model.GridPlanes();
      IReadOnlyDictionary<int, Axis> axDict = model.Axes();
      ReadOnlyDictionary<int, LoadCase> loadCases = model.LoadCases();
      ReadOnlyDictionary<int, GsaAPI.Polyline> polylines = model.Polylines();

      var loads = new List<GsaLoadGoo>();
      foreach (GridLineLoad gridLineLoad in lineLoads) {
        var load = new GsaGridLineLoad {
          ApiLoad = gridLineLoad,
          GridPlaneSurface = CreateGridPlaneSurfaceFromApi(model, gridLineLoad.GridSurface, unit)
        };

        if (load.GridPlaneSurface != null) {
          if (gridLineLoad.PolyLineDefinition != string.Empty && gridLineLoad.PolyLineDefinition.Contains('(')) {
            load.Points = GridLoadHelper.ConvertPoints(gridLineLoad.PolyLineDefinition.ToString(), unit, load.GridPlaneSurface.Plane);
          } else if (polylines.ContainsKey(gridLineLoad.PolyLineReference)) {
            GsaAPI.Polyline polyline = polylines[gridLineLoad.PolyLineReference];
            load.Points = GridLoadHelper.ConvertPoints(polyline.Points, unit, load.GridPlaneSurface.Plane);
            load.ApiPolyline = polyline;
          }
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
    /// <param name="model"></param>
    /// <param name="gridSrfId">ID/Key/number of Grid Surface in GSA model to convert</param>
    /// <param name="unit"></param>
    /// <returns></returns>
    internal static GsaGridPlaneSurface CreateGridPlaneSurfaceFromApi(Model model, int gridSrfId, LengthUnit unit) {
      ReadOnlyCollection<GridAreaLoad> areaLoads = model.GridAreaLoads();
      IReadOnlyDictionary<int, GridSurface> srfDict = model.GridSurfaces();
      IReadOnlyDictionary<int, GridPlane> plnDict = model.GridPlanes();
      IReadOnlyDictionary<int, Axis> axDict = model.Axes();
      ReadOnlyDictionary<int, LoadCase> loadCases = model.LoadCases();

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
        }
        gps.Plane = plane;
      }

      return gps;
    }

    /// <summary>
    ///   Method to import Grid Point Loads from a GSA model.
    ///   Will output a list of GsaLoads.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="unit"></param>
    /// <returns></returns>
    internal static List<GsaLoadGoo> CreateGridPointLoadsFromApi(Model model, LengthUnit unit) {
      ReadOnlyCollection<GridPointLoad> pointLoads = model.GridPointLoads();
      ReadOnlyCollection<GridAreaLoad> areaLoads = model.GridAreaLoads();
      IReadOnlyDictionary<int, GridSurface> srfDict = model.GridSurfaces();
      IReadOnlyDictionary<int, GridPlane> plnDict = model.GridPlanes();
      IReadOnlyDictionary<int, Axis> axDict = model.Axes();
      ReadOnlyDictionary<int, LoadCase> loadCases = model.LoadCases();

      var loads = new List<GsaLoadGoo>();
      foreach (GridPointLoad gridPointLoad in pointLoads) {
        var load = new GsaGridPointLoad {
          ApiLoad = gridPointLoad,
          GridPlaneSurface = CreateGridPlaneSurfaceFromApi(model, gridPointLoad.GridSurface, unit)
        };

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
    /// <param name="model"></param>
    /// <returns></returns>
    internal static List<GsaLoadGoo> CreateNodeLoadsFromApi(Model model) {
      ReadOnlyDictionary<int, LoadCase> loadCases = model.LoadCases();

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
