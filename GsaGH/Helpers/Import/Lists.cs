using System.Collections.Generic;
using GsaGH.Parameters;

namespace GsaGH.Helpers.Import {
  /// <summary>
  /// Class containing functions to import various object types from GSA
  /// </summary>
  internal class Lists {

    internal static List<GsaList> GetLists(GsaModel gsaModel) {
      var lists = new List<GsaList>();
      //foreach (KeyValuePair<int, EntityList> apiList in gsaModel.Model.Lists())
      //  lists.Add(new GsaList(apiList.Key, apiList.Value, gsaModel));
      return lists;
    }
  }
}
