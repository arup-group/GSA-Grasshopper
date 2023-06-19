using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using GsaAPI;
using GsaGH.Parameters;

namespace GsaGH.Helpers.Export {
  internal partial class Loads {
    internal static void ConvertList(
      List<GsaList> lists, List<GsaLoad> loads, ref ModelAssembly model, GH_Component owner) {
      model.Lists = new GsaGuidDictionary<EntityList>(model.Model.Lists());

      // Add lists embedded in loads as they may have ID > 0 set
      if (lists == null && loads != null && loads.Count > 0) {
        lists = Loads.GetLoadLists(loads);
      } else if (loads != null && loads.Count > 0) {
        lists.AddRange(Loads.GetLoadLists(loads));
      }

      Lists.ConvertList(lists, ref model, owner);
    }
    internal static List<GsaList> GetLoadLists(List<GsaLoad> loads) {
      var loadLists = new List<GsaList>();
      foreach (GsaLoad load in loads.Where(gsaLoad => gsaLoad != null)) {
        GsaList list = GetLoadList(load);
        if (list != null) {
          loadLists.Add(list);
        }
      }
      return loadLists;
    }
    private static GsaList GetLoadList(GsaLoad load) {
      if (load == null) {
        return null;
      }
      switch (load.LoadType) {
        case GsaLoad.LoadTypes.Gravity:
          if (load.GravityLoad._referenceType == ReferenceType.List) {
            return load.GravityLoad._refList;
          }
          break;

        case GsaLoad.LoadTypes.Beam:
          if (load.BeamLoad._referenceType == ReferenceType.List) {
            return load.BeamLoad._refList;
          }
          break;

        case GsaLoad.LoadTypes.Face:
          if (load.FaceLoad._referenceType == ReferenceType.List) {
            return load.FaceLoad._refList;
          }
          break;

        case GsaLoad.LoadTypes.GridPoint:
          if (load.PointLoad.GridPlaneSurface._referenceType == ReferenceType.List) {
            return load.PointLoad.GridPlaneSurface._refList;
          }
          break;

        case GsaLoad.LoadTypes.GridLine:
          if (load.LineLoad.GridPlaneSurface._referenceType == ReferenceType.List) {
            return load.LineLoad.GridPlaneSurface._refList;
          }
          break;

        case GsaLoad.LoadTypes.GridArea:
          if (load.AreaLoad.GridPlaneSurface._referenceType == ReferenceType.List) {
            return load.AreaLoad.GridPlaneSurface._refList;
          }
          break;
      }
      return null;
    }
  }
}
