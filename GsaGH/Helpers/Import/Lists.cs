using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using GsaAPI;
using GsaGH.Parameters;
using OasysUnits.Units;
using System.Collections.Concurrent;

namespace GsaGH.Helpers.Import
{
  /// <summary>
  /// Class containing functions to import various object types from GSA
  /// </summary>
  internal class Lists
  {
    internal static List<GsaList> GetLists(GsaModel gsaModel)
    {
      var lists = new List<GsaList>();
      //foreach (KeyValuePair<int, EntityList> apiList in gsaModel.Model.Lists())
      //  lists.Add(new GsaList(apiList.Key, apiList.Value, gsaModel));
      return lists;
    }
  }
}
