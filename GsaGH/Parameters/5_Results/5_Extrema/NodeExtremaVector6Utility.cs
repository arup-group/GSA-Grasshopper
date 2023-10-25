using OasysUnits;
using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace GsaGH.Parameters.Results {
  public static class NodeExtremaVector6Utility {
    public static (NodeExtremaVector6 Max, NodeExtremaVector6 Min) Extrema<T, Q1, Q2>(
      this ConcurrentDictionary<int, Collection<T>> subset)
      where T : IResultVector6<Q1, Q2>
      where Q1 : IQuantity where Q2 : IQuantity {
      if (subset.First().Value.Count > 1) {
        return ExtremaWithPermutations<T, Q1, Q2>(subset);
      }

      int maxXId = subset.OrderBy(kvp => kvp.Value[0].X).Last().Key;
      int maxYId = subset.OrderBy(kvp => kvp.Value[0].Y).Last().Key;
      int maxZId = subset.OrderBy(kvp => kvp.Value[0].Z).Last().Key;
      int maxXyzId = subset.OrderBy(kvp => kvp.Value[0].Xyz).Last().Key;
      int maxXxId = subset.OrderBy(kvp => kvp.Value[0].Xx).Last().Key;
      int maxYyId = subset.OrderBy(kvp => kvp.Value[0].Yy).Last().Key;
      int maxZzId = subset.OrderBy(kvp => kvp.Value[0].Zz).Last().Key;
      int maxXxyyzzId = subset.OrderBy(kvp => kvp.Value[0].Xxyyzz).Last().Key;

      var max = new NodeExtremaVector6() {
        X = new NodeExtremaKey(maxXId),
        Y = new NodeExtremaKey(maxYId),
        Z = new NodeExtremaKey(maxZId),
        Xyz = new NodeExtremaKey(maxXyzId),
        Xx = new NodeExtremaKey(maxXxId),
        Yy = new NodeExtremaKey(maxYyId),
        Zz = new NodeExtremaKey(maxZzId),
        Xxyyzz = new NodeExtremaKey(maxXxyyzzId, 0)
      };

      int minXId = subset.OrderBy(kvp => kvp.Value[0].X).First().Key;
      int minYId = subset.OrderBy(kvp => kvp.Value[0].Y).First().Key;
      int minZId = subset.OrderBy(kvp => kvp.Value[0].Z).First().Key;
      int minXyzId = subset.OrderBy(kvp => kvp.Value[0].Xyz).First().Key;
      int minXxId = subset.OrderBy(kvp => kvp.Value[0].Xx).First().Key;
      int minYyId = subset.OrderBy(kvp => kvp.Value[0].Yy).First().Key;
      int minZzId = subset.OrderBy(kvp => kvp.Value[0].Zz).First().Key;
      int minXxyyzzId = subset.OrderBy(kvp => kvp.Value[0].Xxyyzz).First().Key;

      var min = new NodeExtremaVector6() {
        X = new NodeExtremaKey(minXId),
        Y = new NodeExtremaKey(minYId),
        Z = new NodeExtremaKey(minZId),
        Xyz = new NodeExtremaKey(minXyzId),
        Xx = new NodeExtremaKey(minXxId),
        Yy = new NodeExtremaKey(minYyId),
        Zz = new NodeExtremaKey(minZzId),
        Xxyyzz = new NodeExtremaKey(minXxyyzzId)
      };

      return (max, min);
    }


    internal static (NodeExtremaVector6 Max, NodeExtremaVector6 Min) ExtremaWithPermutations<T, Q1, Q2>(
      this ConcurrentDictionary<int, Collection<T>> subset)
      where T : IResultVector6<Q1, Q2>
      where Q1 : IQuantity where Q2 : IQuantity {
      var permIdsX = new ConcurrentDictionary<int, (int Max, int Min)>();
      var permIdsY = new ConcurrentDictionary<int, (int Max, int Min)>();
      var permIdsZ = new ConcurrentDictionary<int, (int Max, int Min)>();
      var permIdsXyz = new ConcurrentDictionary<int, (int Max, int Min)>();
      var permIdsXx = new ConcurrentDictionary<int, (int Max, int Min)>();
      var permIdsYy = new ConcurrentDictionary<int, (int Max, int Min)>();
      var permIdsZz = new ConcurrentDictionary<int, (int Max, int Min)>();
      var permIdsXxyyzz = new ConcurrentDictionary<int, (int Max, int Min)>();

      Parallel.ForEach(subset, kvp => {
        permIdsX.TryAdd(kvp.Key, ExtremaUtility.PermExtrema(kvp.Value, new Func<T, Q1>(x => x.X)));
        permIdsY.TryAdd(kvp.Key, ExtremaUtility.PermExtrema(kvp.Value, new Func<T, Q1>(x => x.Y)));
        permIdsZ.TryAdd(kvp.Key, ExtremaUtility.PermExtrema(kvp.Value, new Func<T, Q1>(x => x.Z)));
        permIdsXyz.TryAdd(kvp.Key, ExtremaUtility.PermExtrema(kvp.Value, new Func<T, Q1>(x => x.Xyz)));
        permIdsXx.TryAdd(kvp.Key, ExtremaUtility.PermExtrema(kvp.Value, new Func<T, Q2>(x => x.Xx)));
        permIdsYy.TryAdd(kvp.Key, ExtremaUtility.PermExtrema(kvp.Value, new Func<T, Q2>(x => x.Yy)));
        permIdsZz.TryAdd(kvp.Key, ExtremaUtility.PermExtrema(kvp.Value, new Func<T, Q2>(x => x.Zz)));
        permIdsXxyyzz.TryAdd(kvp.Key, ExtremaUtility.PermExtrema(kvp.Value, new Func<T, Q2>(x => x.Xxyyzz)));
      });

      int maxXId = subset.OrderBy(kvp => kvp.Value[permIdsX[kvp.Key].Max].X).Last().Key;
      int maxYId = subset.OrderBy(kvp => kvp.Value[permIdsY[kvp.Key].Max].Y).Last().Key;
      int maxZId = subset.OrderBy(kvp => kvp.Value[permIdsZ[kvp.Key].Max].Z).Last().Key;
      int maxXyzId = subset.OrderBy(kvp => kvp.Value[permIdsXyz[kvp.Key].Max].Xyz).Last().Key;
      int maxXxId = subset.OrderBy(kvp => kvp.Value[permIdsXx[kvp.Key].Max].Xx).Last().Key;
      int maxYyId = subset.OrderBy(kvp => kvp.Value[permIdsYy[kvp.Key].Max].Yy).Last().Key;
      int maxZzId = subset.OrderBy(kvp => kvp.Value[permIdsZz[kvp.Key].Max].Zz).Last().Key;
      int maxXxyyzzId = subset.OrderBy(kvp => kvp.Value[permIdsXxyyzz[kvp.Key].Max].Xxyyzz).Last().Key;

      var max = new NodeExtremaVector6() {
        X = new NodeExtremaKey(maxXId, permIdsX[maxXId].Max),
        Y = new NodeExtremaKey(maxYId, permIdsY[maxYId].Max),
        Z = new NodeExtremaKey(maxZId, permIdsZ[maxZId].Max),
        Xyz = new NodeExtremaKey(maxXyzId, permIdsXyz[maxXyzId].Max),
        Xx = new NodeExtremaKey(maxXxId, permIdsXx[maxXxId].Max),
        Yy = new NodeExtremaKey(maxYyId, permIdsYy[maxYyId].Max),
        Zz = new NodeExtremaKey(maxZzId, permIdsZz[maxZzId].Max),
        Xxyyzz = new NodeExtremaKey(maxXxyyzzId, permIdsXyz[maxXyzId].Max)
      };

      int minXId = subset.OrderBy(kvp => kvp.Value[permIdsX[kvp.Key].Min].X).First().Key;
      int minYId = subset.OrderBy(kvp => kvp.Value[permIdsY[kvp.Key].Min].Y).First().Key;
      int minZId = subset.OrderBy(kvp => kvp.Value[permIdsZ[kvp.Key].Min].Z).First().Key;
      int minXyzId = subset.OrderBy(kvp => kvp.Value[permIdsXyz[kvp.Key].Min].Xyz).First().Key;
      int minXxId = subset.OrderBy(kvp => kvp.Value[permIdsXx[kvp.Key].Min].Xx).First().Key;
      int minYyId = subset.OrderBy(kvp => kvp.Value[permIdsYy[kvp.Key].Min].Yy).First().Key;
      int minZzId = subset.OrderBy(kvp => kvp.Value[permIdsZz[kvp.Key].Min].Zz).First().Key;
      int minXxyyzzId = subset.OrderBy(kvp => kvp.Value[permIdsXxyyzz[kvp.Key].Min].Xxyyzz).First().Key;

      var min = new NodeExtremaVector6() {
        X = new NodeExtremaKey(minXId, permIdsX[minXId].Min),
        Y = new NodeExtremaKey(minYId, permIdsY[minYId].Min),
        Z = new NodeExtremaKey(minZId, permIdsZ[minZId].Min),
        Xyz = new NodeExtremaKey(minXyzId, permIdsXyz[minXyzId].Min),
        Xx = new NodeExtremaKey(minXxId, permIdsXx[minXxId].Min),
        Yy = new NodeExtremaKey(minYyId, permIdsYy[minYyId].Min),
        Zz = new NodeExtremaKey(minZzId, permIdsZz[minZzId].Min),
        Xxyyzz = new NodeExtremaKey(minXxyyzzId, permIdsXxyyzz[minXxyyzzId].Min)
      };

      return (max, min);
    }
  }
}
