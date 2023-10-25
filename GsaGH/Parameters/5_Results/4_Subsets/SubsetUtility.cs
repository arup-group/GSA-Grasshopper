using OasysUnits;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace GsaGH.Parameters.Results {
  public static class SubsetUtility {
    public static (ExtremaSextet Max, ExtremaSextet Min) Extrema(
      this ConcurrentDictionary<int, Collection<IDisplacement>> subset) {
      if (subset.First().Value.Count > 1) {
        return ExtremaWithPermutations(subset);
      }

      int maxXId = subset.OrderBy(kvp => kvp.Value[0].X).Last().Key;
      int maxYId = subset.OrderBy(kvp => kvp.Value[0].Y).Last().Key;
      int maxZId = subset.OrderBy(kvp => kvp.Value[0].Z).Last().Key;
      int maxXyzId = subset.OrderBy(kvp => kvp.Value[0].Xyz).Last().Key;
      int maxXxId = subset.OrderBy(kvp => kvp.Value[0].Xx).Last().Key;
      int maxYyId = subset.OrderBy(kvp => kvp.Value[0].Yy).Last().Key;
      int maxZzId = subset.OrderBy(kvp => kvp.Value[0].Zz).Last().Key;
      int maxXxyyzzId = subset.OrderBy(kvp => kvp.Value[0].Xxyyzz).Last().Key;

      var max = new ExtremaSextet() {
        X = (maxXId, 0),
        Y = (maxYId, 0),
        Z = (maxZId, 0),
        Xyz = (maxXyzId, 0),
        Xx = (maxXxId, 0),
        Yy = (maxYyId, 0),
        Zz = (maxZzId, 0),
        Xxyyzz = (maxXxyyzzId, 0)
      };

      int minXId = subset.OrderBy(kvp => kvp.Value[0].X).First().Key;
      int minYId = subset.OrderBy(kvp => kvp.Value[0].Y).First().Key;
      int minZId = subset.OrderBy(kvp => kvp.Value[0].Z).First().Key;
      int minXyzId = subset.OrderBy(kvp => kvp.Value[0].Xyz).First().Key;
      int minXxId = subset.OrderBy(kvp => kvp.Value[0].Xx).First().Key;
      int minYyId = subset.OrderBy(kvp => kvp.Value[0].Yy).First().Key;
      int minZzId = subset.OrderBy(kvp => kvp.Value[0].Zz).First().Key;
      int minXxyyzzId = subset.OrderBy(kvp => kvp.Value[0].Xxyyzz).First().Key;

      var min = new ExtremaSextet() {
        X = (minXId, 0),
        Y = (minYId, 0),
        Z = (minZId, 0),
        Xyz = (minXyzId, 0),
        Xx = (minXxId, 0),
        Yy = (minYyId, 0),
        Zz = (minZzId, 0),
        Xxyyzz = (minXxyyzzId, 0)
      };

      return (max, min);
    }


    private static (ExtremaSextet Max, ExtremaSextet Min) ExtremaWithPermutations(
      this ConcurrentDictionary<int, Collection<IDisplacement>> subset) {
      var permIdsX = new ConcurrentDictionary<int, (int Max, int Min)>();
      var permIdsY = new ConcurrentDictionary<int, (int Max, int Min)>();
      var permIdsZ = new ConcurrentDictionary<int, (int Max, int Min)>();
      var permIdsXyz = new ConcurrentDictionary<int, (int Max, int Min)>();
      var permIdsXx = new ConcurrentDictionary<int, (int Max, int Min)>();
      var permIdsYy = new ConcurrentDictionary<int, (int Max, int Min)>();
      var permIdsZz = new ConcurrentDictionary<int, (int Max, int Min)>();
      var permIdsXxyyzz = new ConcurrentDictionary<int, (int Max, int Min)>();

      Parallel.ForEach(subset, kvp => {
        permIdsX.TryAdd(kvp.Key, PermExtrema(kvp.Value, new Func<IDisplacement, Length>(x => x.X)));
        permIdsY.TryAdd(kvp.Key, PermExtrema(kvp.Value, new Func<IDisplacement, Length>(x => x.Y)));
        permIdsZ.TryAdd(kvp.Key, PermExtrema(kvp.Value, new Func<IDisplacement, Length>(x => x.Z)));
        permIdsXyz.TryAdd(kvp.Key, PermExtrema(kvp.Value, new Func<IDisplacement, Length>(x => x.Xyz)));
        permIdsXx.TryAdd(kvp.Key, PermExtrema(kvp.Value, new Func<IDisplacement, Angle>(x => x.Xx)));
        permIdsYy.TryAdd(kvp.Key, PermExtrema(kvp.Value, new Func<IDisplacement, Angle>(x => x.Yy)));
        permIdsZz.TryAdd(kvp.Key, PermExtrema(kvp.Value, new Func<IDisplacement, Angle>(x => x.Zz)));
        permIdsXxyyzz.TryAdd(kvp.Key, PermExtrema(kvp.Value, new Func<IDisplacement, Angle>(x => x.Xxyyzz)));
      });

      int maxXId = subset.OrderBy(kvp => kvp.Value[permIdsX[kvp.Key].Max].X).Last().Key;
      int maxYId = subset.OrderBy(kvp => kvp.Value[permIdsY[kvp.Key].Max].Y).Last().Key;
      int maxZId = subset.OrderBy(kvp => kvp.Value[permIdsZ[kvp.Key].Max].Z).Last().Key;
      int maxXyzId = subset.OrderBy(kvp => kvp.Value[permIdsXyz[kvp.Key].Max].Xyz).Last().Key;
      int maxXxId = subset.OrderBy(kvp => kvp.Value[permIdsXx[kvp.Key].Max].Xx).Last().Key;
      int maxYyId = subset.OrderBy(kvp => kvp.Value[permIdsYy[kvp.Key].Max].Yy).Last().Key;
      int maxZzId = subset.OrderBy(kvp => kvp.Value[permIdsZz[kvp.Key].Max].Zz).Last().Key;
      int maxXxyyzzId = subset.OrderBy(kvp => kvp.Value[permIdsXxyyzz[kvp.Key].Max].Xxyyzz).Last().Key;

      var max = new ExtremaSextet() {
        X = (maxXId, permIdsX[maxXId].Max),
        Y = (maxYId, permIdsY[maxYId].Max),
        Z = (maxZId, permIdsZ[maxZId].Max),
        Xyz = (maxXyzId, permIdsXyz[maxXyzId].Max),
        Xx = (maxXxId, permIdsXx[maxXxId].Max),
        Yy = (maxYyId, permIdsYy[maxYyId].Max),
        Zz = (maxZzId, permIdsZz[maxZzId].Max),
        Xxyyzz = (maxXxyyzzId, permIdsXyz[maxXyzId].Max)
      };

      int minXId = subset.OrderBy(kvp => kvp.Value[permIdsX[kvp.Key].Min].X).First().Key;
      int minYId = subset.OrderBy(kvp => kvp.Value[permIdsY[kvp.Key].Min].Y).First().Key;
      int minZId = subset.OrderBy(kvp => kvp.Value[permIdsZ[kvp.Key].Min].Z).First().Key;
      int minXyzId = subset.OrderBy(kvp => kvp.Value[permIdsXyz[kvp.Key].Min].Xyz).First().Key;
      int minXxId = subset.OrderBy(kvp => kvp.Value[permIdsXx[kvp.Key].Min].Xx).First().Key;
      int minYyId = subset.OrderBy(kvp => kvp.Value[permIdsYy[kvp.Key].Min].Yy).First().Key;
      int minZzId = subset.OrderBy(kvp => kvp.Value[permIdsZz[kvp.Key].Min].Zz).First().Key;
      int minXxyyzzId = subset.OrderBy(kvp => kvp.Value[permIdsXxyyzz[kvp.Key].Min].Xxyyzz).First().Key;

      var min = new ExtremaSextet() {
        X = (minXId, permIdsX[minXId].Min),
        Y = (minYId, permIdsY[minYId].Min),
        Z = (minZId, permIdsZ[minZId].Min),
        Xyz = (minXyzId, permIdsXyz[minXyzId].Min),
        Xx = (minXxId, permIdsXx[minXxId].Min),
        Yy = (minYyId, permIdsYy[minYyId].Min),
        Zz = (minZzId, permIdsZz[minZzId].Min),
        Xxyyzz = (minXxyyzzId, permIdsXxyyzz[minXxyyzzId].Min)
      };

      return (max, min);
    }

    private static ConcurrentDictionary<int, (int Max, int Min)> PermExtremaDictionary
      (ICollection<int> ids) {
      var dict = new ConcurrentDictionary<int, (int Max, int Min)>();
      Parallel.ForEach(ids, id => dict.TryAdd(id, (0, 0)));
      return dict;
    }

    private static (int Max, int Min) PermExtrema<T, U>(
    Collection<T> collection, Func<T, U> sortFunction)
    where T : IResultQuantitySet
    where U : IQuantity {
      IOrderedEnumerable<T> sorted = collection.OrderBy(sortFunction);
      int max = collection.IndexOf(sorted.Last());
      int min = collection.IndexOf(sorted.First());
      return (max, min);
    }
  }
}
