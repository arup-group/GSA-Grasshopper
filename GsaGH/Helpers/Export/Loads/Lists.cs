using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using GsaAPI;
using GsaGH.Parameters;

namespace GsaGH.Helpers.Export {
  internal partial class Loads {
    internal static void ConvertList(
      List<GsaList> lists, List<IGsaLoad> loads, ref ModelAssembly model, GH_Component owner) {
      model.Lists = new GsaGuidDictionary<EntityList>(model.Model.Lists());

      // Add lists embedded in loads as they may have ID > 0 set
      if (lists == null && !loads.IsNullOrEmpty()) {
        lists = Loads.GetLoadLists(loads);
      } else if (!loads.IsNullOrEmpty()) {
        lists.AddRange(Loads.GetLoadLists(loads));
      }

      Lists.ConvertList(lists, ref model, owner);
    }

    internal static List<GsaList> GetLoadLists(List<IGsaLoad> loads) {
      var loadLists = new List<GsaList>();
      foreach (IGsaLoad load in loads.Where(gsaLoad => gsaLoad != null)) {
        GsaList list = GetLoadList(load);
        if (list != null) {
          loadLists.Add(list);
        }
      }
      return loadLists;
    }

    private static GsaList GetLoadList(IGsaLoad load) {
      if (load == null) {
        return null;
      }

      switch (load.LoadType) {
        case LoadType.Gravity:
        case LoadType.Beam:
        case LoadType.Face:
        case LoadType.GridPoint:
        case LoadType.GridLine:
        case LoadType.GridArea:
          if (load.ReferenceType == ReferenceType.List) {
            return load.ReferenceList;
          }
          break;
      }

      return null;
    }
  }
}
