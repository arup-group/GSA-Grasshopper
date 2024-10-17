using System.Collections.Generic;
using System.Linq;

using Grasshopper.Kernel;

using GsaAPI;

using GsaGH.Parameters;
using GsaGH.Parameters.Enums;

namespace GsaGH.Helpers.Assembly {
  internal partial class ModelAssembly {
    private void ConvertList(List<GsaList> lists, List<IGsaLoad> loads, List<IGsaDesignTask> designTasks, GH_Component owner) {
      _lists = new GsaGuidDictionary<EntityList>(_model.Lists());

      // Add lists embedded in loads as they may have ID > 0 set
      if (lists == null && !loads.IsNullOrEmpty()) {
        lists = GetLoadLists(loads);
      } else if (!loads.IsNullOrEmpty()) {
        lists.AddRange(GetLoadLists(loads));
      }

      // Add lists embedded in designTasks as they may have ID > 0 set
      if (lists == null && !designTasks.IsNullOrEmpty()) {
        lists = GetDesignTaskList(designTasks);
      } else if (!designTasks.IsNullOrEmpty()) {
        lists.AddRange(GetDesignTaskList(designTasks));
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

    private static List<GsaList> GetDesignTaskList(List<IGsaDesignTask> tasks) {
      var taskLists = new List<GsaList>();
      foreach (IGsaDesignTask task in tasks.Where(gsaTask => gsaTask != null)) {
        GsaList list = task?.List;
        if (list != null) {
          taskLists.Add(list);
        }
      }
      return taskLists;
    }
  }
}
