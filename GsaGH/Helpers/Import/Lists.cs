using System.Collections.Generic;
using GsaAPI;
using GsaGH.Parameters;

namespace GsaGH.Helpers.Import {
  /// <summary>
  ///   Class containing functions to import various object types from GSA
  /// </summary>
  internal class Lists {

    internal static List<GsaList> GetLists(GsaModel model) {
      var lists = new List<GsaList>();
      foreach (KeyValuePair<int, EntityList> apiList in model.Model.Lists()) {
        lists.Add(new GsaList(apiList.Key, apiList.Value, model));
      }
      return lists;
    }
  }
}
