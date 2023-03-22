using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaAPI;
using GsaGH.Parameters;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Helpers.Import {
  /// <summary>
  /// Class containing functions to import various object types from GSA
  /// </summary>
  internal class Loads {
    /// <summary>
    /// Method to import Gravity Loads from a GSA model.
    /// Will output a list of GsaLoadsGoo.
    /// </summary>
    /// <param name="gravityLoads">Collection of gravity loads to import</param>
    /// <returns></returns>
    internal static List<GsaLoadGoo> GetGravityLoads(ReadOnlyCollection<GravityLoad> gravityLoads) {
      var loads = new List<GsaLoadGoo>();

      var gloads = gravityLoads.ToList();

      // Loop through all loads in list and create new GsaLoads
      for (int i = 0; i < gloads.Count; i++) {
        var myload = new GsaGravityLoad();
        myload.GravityLoad = gloads[i];
        var load = new GsaLoad(myload);
        loads.Add(new GsaLoadGoo(load));
      }
      return loads;
    }
    /// <summary>
    /// Method to import all Node Loads from a GSA model.
    /// 
    /// GSA Node loads vary by type, to get all node loads easiest
    /// method seems to be toogling through all enum types which
    /// requeres the entire model to be inputted to this method.
    /// 
    /// Will output a list of GsaLoads.
    /// </summary>
    /// <param name="model">GSA model containing node loads</param>
    /// <returns></returns>
    internal static List<GsaLoadGoo> GetNodeLoads(Model model) {
      var loads = new List<GsaLoadGoo>();

      // NodeLoads come in varioys types, depending on GsaAPI.NodeLoadType:
      foreach (NodeLoadType typ in Enum.GetValues(typeof(NodeLoadType))) {
        try // some GsaAPI.NodeLoadTypes are currently not supported in the API and throws an error
        {
          var gsaloads = model.NodeLoads(typ).ToList();
          GsaNodeLoad.NodeLoadTypes ntyp = GsaNodeLoad.NodeLoadTypes.NodeLoad;
          switch (typ) {
            case NodeLoadType.APPL_DISP:
              ntyp = GsaNodeLoad.NodeLoadTypes.AppliedDisp;
              break;
            case NodeLoadType.GRAVITY:
              ntyp = GsaNodeLoad.NodeLoadTypes.Gravity;
              break;
            case NodeLoadType.NODE_LOAD:
              ntyp = GsaNodeLoad.NodeLoadTypes.NodeLoad;
              break;
            case NodeLoadType.NUM_TYPES:
              ntyp = GsaNodeLoad.NodeLoadTypes.NumTypes;
              break;
            case NodeLoadType.SETTLEMENT:
              ntyp = GsaNodeLoad.NodeLoadTypes.Settlement;
              break;
          }

          // Loop through all loads in list and create new GsaLoads
          for (int i = 0; i < gsaloads.Count; i++) {
            var myload = new GsaNodeLoad();
            myload.NodeLoad = gsaloads[i];
            myload.Type = ntyp;
            var load = new GsaLoad(myload);
            loads.Add(new GsaLoadGoo(load));
          }
        }
        catch (Exception) {

        }
      }
      return loads;
    }
    /// <summary>
    /// Method to import Beam Loads from a GSA model.
    /// Will output a list of GsaLoads.
    /// </summary>
    /// <param name="beamLoads">Collection of beams loads to be imported</param>
    /// <returns></returns>
    internal static List<GsaLoadGoo> GetBeamLoads(ReadOnlyCollection<BeamLoad> beamLoads) {
      var loads = new List<GsaLoadGoo>();

      var gsaloads = beamLoads.ToList();

      // Loop through all loads in list and create new GsaLoads
      for (int i = 0; i < gsaloads.Count; i++) {
        var myload = new GsaBeamLoad();
        myload.BeamLoad = gsaloads[i];
        var load = new GsaLoad(myload);
        loads.Add(new GsaLoadGoo(load));
      }
      return loads;
    }
    /// <summary>
    /// Method to import Face Loads from a GSA model.
    /// Will output a list of GsaLoads.
    /// </summary>
    /// <param name="faceLoads">Collection of Face loads to be imported</param>
    /// <returns></returns>
    internal static List<GsaLoadGoo> GetFaceLoads(ReadOnlyCollection<FaceLoad> faceLoads) {
      var loads = new List<GsaLoadGoo>();

      var gsaloads = faceLoads.ToList();

      // Loop through all loads in list and create new GsaLoads
      for (int i = 0; i < gsaloads.Count; i++) {
        var myload = new GsaFaceLoad();
        myload.FaceLoad = gsaloads[i];
        var load = new GsaLoad(myload);
        loads.Add(new GsaLoadGoo(load));
      }
      return loads;
    }
    /// <summary>
    /// Method to import Grid Point Loads from a GSA model.
    /// Will output a list of GsaLoads.
    /// </summary>
    /// <param name="pointLoads">Collection of Grid Point loads to be imported</param>
    /// <param name="srfDict">Grid Surface Dictionary</param>
    /// <param name="plnDict">Grid Plane Dictionary</param>
    /// <param name="axDict">Axes Dictionary</param>
    /// <returns></returns>
    internal static List<GsaLoadGoo> GetGridPointLoads(ReadOnlyCollection<GridPointLoad> pointLoads,
        IReadOnlyDictionary<int, GridSurface> srfDict, IReadOnlyDictionary<int, GridPlane> plnDict, IReadOnlyDictionary<int, Axis> axDict, LengthUnit unit) {
      var loads = new List<GsaLoadGoo>();

      var gsaloads = pointLoads.ToList();

      // Loop through all loads in list and create new GsaLoads
      for (int i = 0; i < gsaloads.Count; i++) {
        // Get Grid Point Load
        var myload = new GsaGridPointLoad();
        myload.GridPointLoad = gsaloads[i];

        // Get GridPlaneSurface
        myload.GridPlaneSurface = GetGridPlaneSurface(srfDict, plnDict, axDict, gsaloads[i].GridSurface, unit);

        // Add load to list
        var load = new GsaLoad(myload);
        loads.Add(new GsaLoadGoo(load));
      }
      return loads;
    }
    /// <summary>
    /// Method to import Grid Line Loads from a GSA model.
    /// Will output a list of GsaLoads.
    /// </summary>
    /// <param name="lineLoads">Collection of Grid Line loads to be imported</param>
    /// <param name="srfDict">Grid Surface Dictionary</param>
    /// <param name="plnDict">Grid Plane Dictionary</param>
    /// <param name="axDict">Axes Dictionary</param>
    /// <returns></returns>
    internal static List<GsaLoadGoo> GetGridLineLoads(ReadOnlyCollection<GridLineLoad> lineLoads,
        IReadOnlyDictionary<int, GridSurface> srfDict, IReadOnlyDictionary<int, GridPlane> plnDict, IReadOnlyDictionary<int, Axis> axDict, LengthUnit unit) {
      var loads = new List<GsaLoadGoo>();

      var gsaloads = lineLoads.ToList();

      // Loop through all loads in list and create new GsaLoads
      for (int i = 0; i < gsaloads.Count; i++) {
        // Get Grid Point Load
        var myload = new GsaGridLineLoad();
        myload.GridLineLoad = gsaloads[i];

        // Get GridPlaneSurface
        myload.GridPlaneSurface = GetGridPlaneSurface(srfDict, plnDict, axDict, gsaloads[i].GridSurface, unit);

        // Add load to list
        var load = new GsaLoad(myload);
        loads.Add(new GsaLoadGoo(load));
      }
      return loads;
    }
    /// <summary>
    /// Method to import Grid Area Loads from a GSA model.
    /// Will output a list of GsaLoads.
    /// </summary>
    /// <param name="areaLoads">Collection of Grid Area loads to be imported</param>
    /// <param name="srfDict">Grid Surface Dictionary</param>
    /// <param name="plnDict">Grid Plane Dictionary</param>
    /// <param name="axDict">Axes Dictionary</param>
    /// <returns></returns>
    internal static List<GsaLoadGoo> GetGridAreaLoads(ReadOnlyCollection<GridAreaLoad> areaLoads,
        IReadOnlyDictionary<int, GridSurface> srfDict, IReadOnlyDictionary<int, GridPlane> plnDict, IReadOnlyDictionary<int, Axis> axDict, LengthUnit unit) {
      var loads = new List<GsaLoadGoo>();

      var gsaloads = areaLoads.ToList();

      // Loop through all loads in list and create new GsaLoads
      for (int i = 0; i < gsaloads.Count; i++) {
        // Get Grid Point Load
        var myload = new GsaGridAreaLoad();
        myload.GridAreaLoad = gsaloads[i];

        // Get GridPlaneSurface
        myload.GridPlaneSurface = GetGridPlaneSurface(srfDict, plnDict, axDict, gsaloads[i].GridSurface, unit);

        // Add load to list
        var load = new GsaLoad(myload);
        loads.Add(new GsaLoadGoo(load));
      }
      return loads;
    }

    /// <summary>
    /// Method to create GsaGridPlaneSurface including 
    /// grid surface, grid plane and axis from GSA Model
    /// 
    /// Grid Surface references a Grid Plane
    /// Grid Plane references an Axis
    /// Only Grid Surface ID is required, the others will be found by ref
    /// 
    /// Will output a new GsaGridPlaneSurface.
    /// </summary>
    /// <param name="srfDict">Grid Surface Dictionary</param>
    /// <param name="plnDict">Grid Plane Dictionary</param>
    /// <param name="axDict">Axes Dictionary</param>
    /// <param name="gridSrfId">ID/Key/number of Grid Surface in GSA model to convert</param>
    /// <returns></returns>
    internal static GsaGridPlaneSurface GetGridPlaneSurface(IReadOnlyDictionary<int, GridSurface> srfDict,
        IReadOnlyDictionary<int, GridPlane> plnDict, IReadOnlyDictionary<int, Axis> axDict, int gridSrfId, LengthUnit unit) {
      // GridPlaneSurface
      var gps = new GsaGridPlaneSurface();

      // Get Grid Surface
      if (srfDict.Count > 0) {
        srfDict.TryGetValue(gridSrfId, out GridSurface gs);
        gps.GridSurface = gs;
        gps.GridSurfaceId = gridSrfId;
        gps.Tolerance = new Length(gs.Tolerance, LengthUnit.Meter).ToUnit(unit).ToString().Replace(" ", string.Empty).Replace(",", string.Empty);

        // Get Grid Plane
        plnDict.TryGetValue(gs.GridPlane, out GridPlane gp);
        gps.GridPlane = gp;
        gps.GridPlaneId = gs.GridPlane;
        gps.Elevation = new Length(gp.Elevation, LengthUnit.Meter).ToUnit(unit).ToString().Replace(" ", string.Empty).Replace(",", string.Empty);
        gps.StoreyToleranceAbove = gp.ToleranceAbove == 0 ? "auto" : new Length(gp.ToleranceAbove, LengthUnit.Meter).ToUnit(unit).ToString().Replace(" ", string.Empty).Replace(",", string.Empty);
        gps.StoreyToleranceBelow = gp.ToleranceBelow == 0 ? "auto" : new Length(gp.ToleranceBelow, LengthUnit.Meter).ToUnit(unit).ToString().Replace(" ", string.Empty).Replace(",", string.Empty);

        // Get Axis
        axDict.TryGetValue(gp.AxisProperty, out Axis ax);

        gps.AxisId = gp.AxisProperty;

        // Construct Plane from Axis
        Plane pln;
        if (ax != null) {
          // for new origin Z-coordinate we add axis origin and grid plane elevation
          pln = new Plane(Nodes.Point3dFromXYZUnit(ax.Origin.X, ax.Origin.Y, ax.Origin.Z + gp.Elevation, unit),
            Nodes.Vector3dFromXYZUnit(ax.XVector.X, ax.XVector.Y, ax.XVector.Z, unit),
            Nodes.Vector3dFromXYZUnit(ax.XYPlane.X, ax.XYPlane.Y, ax.XYPlane.Z, unit)
            );
        }
        else {
          pln = Plane.WorldXY;
          pln.OriginZ = new Length(gp.Elevation, LengthUnit.Meter).As(unit);
        }
        gps.Plane = pln;
      }
      else
        return null;

      return gps;
    }
  }
}
