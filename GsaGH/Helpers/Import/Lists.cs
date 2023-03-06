using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using GsaGH.Parameters;

namespace GsaGH.Helpers.Import
{
  /// <summary>
  /// Class containing functions to import various object types from GSA
  /// </summary>
  internal class Lists
  {
    internal static List<GsaList> GetLists(GsaModel gsaModel)
    {
      ReadOnlyDictionary<int, GsaApi.EntityList> apiLists = gsaModel.Model.Lists();
    }
  }
}
