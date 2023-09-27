using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Parameters.Enums;

namespace GsaGH.Helpers.Export {
  internal partial class ModelAssembly {
    private void ConvertList(List<GsaList> lists, List<IGsaLoad> loads, GH_Component owner) {
      _lists = new GsaGuidDictionary<EntityList>(_model.Lists());

      // Add lists embedded in loads as they may have ID > 0 set
      if (lists == null && !loads.IsNullOrEmpty()) {
        lists = GetLoadLists(loads);
      } else if (!loads.IsNullOrEmpty()) {
        lists.AddRange(GetLoadLists(loads));
      }

      ConvertList(lists, owner);
    }

    private static GsaList GetLoadList(IGsaLoad load) {
      return load == null ? null
        : load.ReferenceType == ReferenceType.List ? load.ReferenceList : null;
    }

    private static List<GsaList> GetLoadLists(List<IGsaLoad> loads) {
      var loadLists = new List<GsaList>();
      foreach (IGsaLoad load in loads.Where(gsaLoad => gsaLoad != null)) {
        GsaList list = GetLoadList(load);
        if (list != null) {
          loadLists.Add(list);
        }
      }
      return loadLists;
    }
  }
}
