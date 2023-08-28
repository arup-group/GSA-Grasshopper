using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaAPI;
using GsaGH.Parameters;
using OasysUnits;
using Rhino.Geometry;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Helpers.Import {
  /// <summary>
  ///   Class containing functions to import various object types from GSA
  /// </summary>
  internal class Loads {
    /// <summary>
    ///   Method to import Load Cases from a GSA model.
    ///   Will output a list of GsaLoadCase.
    /// </summary>
    /// <param name="loadCases"></param>
    /// <returns></returns>
    internal static List<GsaLoadCaseGoo> GetLoadCases(ReadOnlyDictionary<int, LoadCase> loadCases) {
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
    internal static List<GsaLoadGoo> GetBeamLoads(
      ReadOnlyCollection<BeamLoad> beamLoads, ReadOnlyDictionary<int, LoadCase> loadCases) {
      var loads = new List<GsaLoadGoo>();
      foreach (BeamLoad gsaLoad in beamLoads) {
        var load = new GsaBeamLoad {
          BeamLoad = gsaLoad,
        };
        load.LoadCase = new GsaLoadCase(load.BeamLoad.Case, loadCases);
        loads.Add(new GsaLoadGoo(load));
      }

      return loads;
    }

    /// <summary>
    ///   Method to import Beam Thermal Loads from a GSA model.
    ///   Will output a list of GsaLoads.
    /// </summary>
    /// <param name="beamThermalLoads">Collection of beam thermal loads to be imported</param>
    /// <returns></returns>
    internal static List<GsaLoadGoo> GetBeamThermalLoads(
      ReadOnlyCollection<BeamThermalLoad> beamThermalLoads, ReadOnlyDictionary<int, LoadCase> loadCases) {
      var loads = new List<GsaLoadGoo>();
      foreach (BeamThermalLoad gsaLoad in beamThermalLoads) {
        var load = new GsaBeamThermalLoad {
          BeamThermalLoad = gsaLoad,
        };
        load.LoadCase = new GsaLoadCase(load.BeamThermalLoad.Case, loadCases);
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
    internal static List<GsaLoadGoo> GetFaceLoads(
      ReadOnlyCollection<FaceLoad> faceLoads, ReadOnlyDictionary<int, LoadCase> loadCases) {
      var loads = new List<GsaLoadGoo>();
      foreach (FaceLoad faceLoad in faceLoads) {
        var load = new GsaFaceLoad {
          FaceLoad = faceLoad,
        };
        load.LoadCase = new GsaLoadCase(load.FaceLoad.Case, loadCases);
        loads.Add(new GsaLoadGoo(load));
      }

      return loads;
    }

    /// <summary>
    ///   Method to import Face Thermal Loads from a GSA model.
    ///   Will output a list of GsaLoads.
    /// </summary>
    /// <param name="faceThermalLoads">Collection of Face Thermal loads to be imported</param>
    /// <returns></returns>
    internal static List<GsaLoadGoo> GetFaceThermalLoads(
      ReadOnlyCollection<FaceThermalLoad> faceThermalLoads, ReadOnlyDictionary<int, LoadCase> loadCases) {
      var loads = new List<GsaLoadGoo>();
      foreach (FaceThermalLoad faceThermalLoad in faceThermalLoads) {
        var load = new GsaFaceThermalLoad {
          FaceThermalLoad = faceThermalLoad,
        };
        load.LoadCase = new GsaLoadCase(load.FaceThermalLoad.Case, loadCases);
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
    internal static List<GsaLoadGoo> GetGravityLoads(
      ReadOnlyCollection<GravityLoad> gravityLoads, ReadOnlyDictionary<int, LoadCase> loadCases) {
      var loads = new List<GsaLoadGoo>();
      foreach (GravityLoad gload in gravityLoads) {
        var load = new GsaGravityLoad {
          GravityLoad = gload,
        };
        load.LoadCase = new GsaLoadCase(load.GravityLoad.Case, loadCases);
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
    internal static List<GsaLoadGoo> GetGridAreaLoads(
      ReadOnlyCollection<GridAreaLoad> areaLoads, IReadOnlyDictionary<int, GridSurface> srfDict,
      IReadOnlyDictionary<int, GridPlane> plnDict, IReadOnlyDictionary<int, Axis> axDict,
      ReadOnlyDictionary<int, LoadCase> loadCases, LengthUnit unit) {
      var loads = new List<GsaLoadGoo>();
      foreach (GridAreaLoad gridAreaLoad in areaLoads) {
        var load = new GsaGridAreaLoad {
          GridAreaLoad = gridAreaLoad,
          GridPlaneSurface
            = GetGridPlaneSurface(srfDict, plnDict, axDict, gridAreaLoad.GridSurface, unit)
        };

        if (gridAreaLoad.PolyLineDefinition != string.Empty &&
          gridAreaLoad.PolyLineDefinition.Contains('(')) {
          load.Points = GridLoadHelper.ConvertPoints(
            gridAreaLoad.PolyLineDefinition.ToString(), unit, load.GridPlaneSurface.Plane);
        }

        load.LoadCase = new GsaLoadCase(load.GridAreaLoad.Case, loadCases);
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
    internal static List<GsaLoadGoo> GetGridLineLoads(
      ReadOnlyCollection<GridLineLoad> lineLoads, IReadOnlyDictionary<int, GridSurface> srfDict,
      IReadOnlyDictionary<int, GridPlane> plnDict, IReadOnlyDictionary<int, Axis> axDict,
      ReadOnlyDictionary<int, LoadCase> loadCases, LengthUnit unit) {
      var loads = new List<GsaLoadGoo>();
      foreach (GridLineLoad gridLineLoad in lineLoads) {
        var load = new GsaGridLineLoad {
          GridLineLoad = gridLineLoad,
          GridPlaneSurface
            = GetGridPlaneSurface(srfDict, plnDict, axDict, gridLineLoad.GridSurface, unit)
        };

        if (gridLineLoad.PolyLineDefinition != string.Empty &&
          gridLineLoad.PolyLineDefinition.Contains('(')) {
          load.Points = GridLoadHelper.ConvertPoints(
          gridLineLoad.PolyLineDefinition.ToString(), unit, load.GridPlaneSurface.Plane);
        }

        load.LoadCase = new GsaLoadCase(load.GridLineLoad.Case, loadCases);
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
    internal static GsaGridPlaneSurface GetGridPlaneSurface(
      IReadOnlyDictionary<int, GridSurface> srfDict, IReadOnlyDictionary<int, GridPlane> plnDict,
      IReadOnlyDictionary<int, Axis> axDict, int gridSrfId, LengthUnit unit) {
      var gps = new GsaGridPlaneSurface();

      if (srfDict.Count > 0) {
        srfDict.TryGetValue(gridSrfId, out GridSurface gs);
        gps.GridSurface = gs;
        gps.GridSurfaceId = gridSrfId;
        gps.Tolerance = new Length(gs.Tolerance, LengthUnit.Meter).ToUnit(unit).ToString()
         .Replace(" ", string.Empty).Replace(",", string.Empty);

        plnDict.TryGetValue(gs.GridPlane, out GridPlane gp);
        gps.GridPlane = gp;
        gps.GridPlaneId = gs.GridPlane;
        gps.SetElevation(new Length(gp.Elevation, LengthUnit.Meter));
        gps.StoreyToleranceAbove = gp.ToleranceAbove == 0 ? "auto" :
          new Length(gp.ToleranceAbove, LengthUnit.Meter).ToUnit(unit).ToString()
           .Replace(" ", string.Empty).Replace(",", string.Empty);
        gps.StoreyToleranceBelow = gp.ToleranceBelow == 0 ? "auto" :
          new Length(gp.ToleranceBelow, LengthUnit.Meter).ToUnit(unit).ToString()
           .Replace(" ", string.Empty).Replace(",", string.Empty);

        axDict.TryGetValue(gp.AxisProperty, out Axis ax);

        gps.AxisId = gp.AxisProperty;

        Plane plane;
        if (ax != null) {
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
      } else {
        return null;
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
    internal static List<GsaLoadGoo> GetGridPointLoads(
      ReadOnlyCollection<GridPointLoad> pointLoads, IReadOnlyDictionary<int, GridSurface> srfDict,
      IReadOnlyDictionary<int, GridPlane> plnDict, IReadOnlyDictionary<int, Axis> axDict,
      ReadOnlyDictionary<int, LoadCase> loadCases, LengthUnit unit) {
      var loads = new List<GsaLoadGoo>();
      foreach (GridPointLoad gridPointLoad in pointLoads) {
        var load = new GsaGridPointLoad {
          GridPointLoad = gridPointLoad,
          GridPlaneSurface
            = GetGridPlaneSurface(srfDict, plnDict, axDict, gridPointLoad.GridSurface, unit), };

        if (unit != LengthUnit.Meter) {
          load.GridPointLoad.X = new Length(load.GridPointLoad.X, LengthUnit.Meter).As(unit);
          load.GridPointLoad.Y = new Length(load.GridPointLoad.Y, LengthUnit.Meter).As(unit);
        }

        load.LoadCase = new GsaLoadCase(load.GridPointLoad.Case, loadCases);
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
    internal static List<GsaLoadGoo> GetNodeLoads(Model model, ReadOnlyDictionary<int, LoadCase> loadCases) {
      var loads = new List<GsaLoadGoo>();

      // NodeLoads come in varioys types, depending on GsaAPI.NodeLoadType:
      foreach (NodeLoadType typ in Enum.GetValues(typeof(NodeLoadType))) {
        try // some GsaAPI.NodeLoadTypes are currently not supported in the API and throws an error
        {
          var gsaloads = model.NodeLoads(typ).ToList();
          GsaNodeLoad.NodeLoadType ntyp = GsaNodeLoad.NodeLoadType.NodeLoad;
          switch (typ) {
            case NodeLoadType.APPL_DISP:
              ntyp = GsaNodeLoad.NodeLoadType.AppliedDisp;
              break;

            case NodeLoadType.GRAVITY:
              ntyp = GsaNodeLoad.NodeLoadType.Gravity;
              break;

            case NodeLoadType.NODE_LOAD:
              ntyp = GsaNodeLoad.NodeLoadType.NodeLoad;
              break;

            case NodeLoadType.NUM_TYPES:
              ntyp = GsaNodeLoad.NodeLoadType.NumTypes;
              break;

            case NodeLoadType.SETTLEMENT:
              ntyp = GsaNodeLoad.NodeLoadType.Settlement;
              break;
          }

          foreach (NodeLoad gsaLoad in gsaloads) {
            var load = new GsaNodeLoad {
              NodeLoad = gsaLoad,
              Type = ntyp,
            };
            load.LoadCase = new GsaLoadCase(load.NodeLoad.Case, loadCases);
            loads.Add(new GsaLoadGoo(load));
          }
        } catch (Exception) {
          // ignored
        }
      }

      return loads;
    }

    internal static List<int> GetLoadCases(Model model) {
      var caseIDs = new List<int>();
      ReadOnlyCollection<GravityLoad> gravities = model.GravityLoads();
      caseIDs.AddRange(gravities.Select(x => x.Case));

      foreach (NodeLoadType typ in Enum.GetValues(typeof(NodeLoadType))) {
        ReadOnlyCollection<NodeLoad> nodeLoads;
        try // some GsaAPI.NodeLoadTypes are currently not supported in the API and throws an error
        {
          nodeLoads = model.NodeLoads(typ);
          caseIDs.AddRange(nodeLoads.Select(x => x.Case));
        } catch (Exception) {
          // ignored
        }
      }

      ReadOnlyCollection<BeamLoad> beamLoads = model.BeamLoads();
      caseIDs.AddRange(beamLoads.Select(x => x.Case));

      ReadOnlyCollection<BeamThermalLoad> beamThermalLoads = model.BeamThermalLoads();
      caseIDs.AddRange(beamThermalLoads.Select(x => x.Case));

      ReadOnlyCollection<FaceLoad> faceLoads = model.FaceLoads();
      caseIDs.AddRange(faceLoads.Select(x => x.Case));

      ReadOnlyCollection<FaceThermalLoad> faceThermalLoads = model.FaceThermalLoads();
      caseIDs.AddRange(faceThermalLoads.Select(x => x.Case));

      ReadOnlyCollection<GridPointLoad> gridPointLoads = model.GridPointLoads();
      caseIDs.AddRange(gridPointLoads.Select(x => x.Case));

      ReadOnlyCollection<GridLineLoad> gridLineLoads = model.GridLineLoads();
      caseIDs.AddRange(gridLineLoads.Select(x => x.Case));

      ReadOnlyCollection<GridAreaLoad> gridAreaLoads = model.GridAreaLoads();
      caseIDs.AddRange(gridAreaLoads.Select(x => x.Case));

      return caseIDs.GroupBy(x => x).Select(y => y.First()).OrderBy(z => z).ToList();
    }
  }
}
