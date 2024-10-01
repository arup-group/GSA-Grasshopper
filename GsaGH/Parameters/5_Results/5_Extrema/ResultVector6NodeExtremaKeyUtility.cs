using System.Collections.Generic;
using System.Linq;

using OasysUnits;

namespace GsaGH.Parameters.Results {
  public static partial class ExtremaKeyUtility {
    public static (ResultVector6<Entity0dExtremaKey> Max, ResultVector6<Entity0dExtremaKey> Min) GetResultVector6NodeExtremaKeys<T>(
      this IDictionary<int, IList<T>> subset) {

      var maxValue = new ResultVector6<double?>(double.MinValue);
      var minValue = new ResultVector6<double?>(double.MaxValue);

      var maxKey = new ResultVector6<Entity0dExtremaKey>();
      var minKey = new ResultVector6<Entity0dExtremaKey>();

      if (subset.Keys.Count > 0) {
        int nodeId = subset.Keys.First();
        maxKey.X = new Entity0dExtremaKey(nodeId, 0);
        maxKey.Y = new Entity0dExtremaKey(nodeId, 0);
        maxKey.Z = new Entity0dExtremaKey(nodeId, 0);
        maxKey.Xyz = new Entity0dExtremaKey(nodeId, 0);
        maxKey.Xx = new Entity0dExtremaKey(nodeId, 0);
        maxKey.Yy = new Entity0dExtremaKey(nodeId, 0);
        maxKey.Zz = new Entity0dExtremaKey(nodeId, 0);
        maxKey.Xxyyzz = new Entity0dExtremaKey(nodeId, 0);
        minKey.X = new Entity0dExtremaKey(nodeId, 0);
        minKey.Y = new Entity0dExtremaKey(nodeId, 0);
        minKey.Z = new Entity0dExtremaKey(nodeId, 0);
        minKey.Xyz = new Entity0dExtremaKey(nodeId, 0);
        minKey.Xx = new Entity0dExtremaKey(nodeId, 0);
        minKey.Yy = new Entity0dExtremaKey(nodeId, 0);
        minKey.Zz = new Entity0dExtremaKey(nodeId, 0);
        minKey.Xxyyzz = new Entity0dExtremaKey(nodeId, 0);
      }

      foreach (int nodeId in subset.Keys) {
        IList<T> values = subset[nodeId];
        for (int permutation = 0; permutation < values.Count; permutation++) {
          switch (values[permutation]) {
            case IDisplacement displacement:
              UpdateExtrema<IDisplacement, Length, Angle>(displacement, nodeId, permutation,
                ref maxValue, ref minValue, ref maxKey, ref minKey);
              break;

            case IInternalForce internalForce:
              UpdateExtrema<IInternalForce, Force, Moment>(internalForce, nodeId, permutation,
                ref maxValue, ref minValue, ref maxKey, ref minKey);
              break;

            case IReactionForce reactionForce:
              UpdateExtrema(reactionForce, nodeId, permutation, ref maxValue, ref minValue, ref maxKey, ref minKey);
              break;
          }
        }
      }

      return (maxKey, minKey);
    }

    private static void UpdateExtrema<T, Q1, Q2>(T item, int nodeId, int permutation,
      ref ResultVector6<double?> maxValue, ref ResultVector6<double?> minValue,
      ref ResultVector6<Entity0dExtremaKey> maxKey, ref ResultVector6<Entity0dExtremaKey> minKey)
      where T : IResultVector6<Q1, Q2> where Q1 : IQuantity where Q2 : IQuantity {

      if (item.X.Value > maxValue.X) {
        maxValue.X = item.X.Value;
        maxKey.X = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.Y.Value > maxValue.Y) {
        maxValue.Y = item.Y.Value;
        maxKey.Y = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.Z.Value > maxValue.Z) {
        maxValue.Z = item.Z.Value;
        maxKey.Z = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.Xyz.Value > maxValue.Xyz) {
        maxValue.Xyz = item.Xyz.Value;
        maxKey.Xyz = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.Xx.Value > maxValue.Xx) {
        maxValue.Xx = item.Xx.Value;
        maxKey.Xx = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.Yy.Value > maxValue.Yy) {
        maxValue.Yy = item.Yy.Value;
        maxKey.Yy = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.Zz.Value > maxValue.Zz) {
        maxValue.Zz = item.Zz.Value;
        maxKey.Zz = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.Xxyyzz.Value > maxValue.Xxyyzz) {
        maxValue.Xxyyzz = item.Xxyyzz.Value;
        maxKey.Xxyyzz = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.X.Value < minValue.X) {
        minValue.X = item.X.Value;
        minKey.X = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.Y.Value < minValue.Y) {
        minValue.Y = item.Y.Value;
        minKey.Y = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.Z.Value < minValue.Z) {
        minValue.Z = item.Z.Value;
        minKey.Z = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.Xyz.Value < minValue.Xyz) {
        minValue.Xyz = item.Xyz.Value;
        minKey.Xyz = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.Xx.Value < minValue.Xx) {
        minValue.Xx = item.Xx.Value;
        minKey.Xx = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.Yy.Value < minValue.Yy) {
        minValue.Yy = item.Yy.Value;
        minKey.Yy = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.Zz.Value < minValue.Zz) {
        minValue.Zz = item.Zz.Value;
        minKey.Zz = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.Xxyyzz.Value < minValue.Xxyyzz) {
        minValue.Xxyyzz = item.Xxyyzz.Value;
        minKey.Xxyyzz = new Entity0dExtremaKey(nodeId, permutation);
      }
    }

    private static void UpdateExtrema(IReactionForce item, int nodeId, int permutation,
      ref ResultVector6<double?> maxValue, ref ResultVector6<double?> minValue,
      ref ResultVector6<Entity0dExtremaKey> maxKey, ref ResultVector6<Entity0dExtremaKey> minKey) {

      if (item.X.HasValue && !double.IsNaN(item.X.Value.Value) && item.X.Value.Value > maxValue.X) {
        maxValue.X = item.X.Value.Value;
        maxKey.X = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.Y.HasValue && !double.IsNaN(item.Y.Value.Value) && item.Y.Value.Value > maxValue.Y) {
        maxValue.Y = item.Y.Value.Value;
        maxKey.Y = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.Z.HasValue && !double.IsNaN(item.Z.Value.Value) && item.Z.Value.Value > maxValue.Z) {
        maxValue.Z = item.Z.Value.Value;
        maxKey.Z = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.Xyz.HasValue && !double.IsNaN(item.Xyz.Value.Value) && item.Xyz.Value.Value > maxValue.Xyz) {
        maxValue.Xyz = item.Xyz.Value.Value;
        maxKey.Xyz = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.Xx.HasValue && !double.IsNaN(item.Xx.Value.Value) && item.Xx.Value.Value > maxValue.Xx) {
        maxValue.Xx = item.Xx.Value.Value;
        maxKey.Xx = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.Yy.HasValue && !double.IsNaN(item.Yy.Value.Value) && item.Yy.Value.Value > maxValue.Yy) {
        maxValue.Yy = item.Yy.Value.Value;
        maxKey.Yy = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.Zz.HasValue && !double.IsNaN(item.Zz.Value.Value) && item.Zz.Value.Value > maxValue.Zz) {
        maxValue.Zz = item.Zz.Value.Value;
        maxKey.Zz = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.Xxyyzz.HasValue && !double.IsNaN(item.Xxyyzz.Value.Value) && item.Xxyyzz.Value.Value > maxValue.Xxyyzz) {
        maxValue.Xxyyzz = item.Xxyyzz.Value.Value;
        maxKey.Xxyyzz = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.X.HasValue && !double.IsNaN(item.X.Value.Value) && item.X.Value.Value < minValue.X) {
        minValue.X = item.X.Value.Value;
        minKey.X = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.Y.HasValue && !double.IsNaN(item.Y.Value.Value) && item.Y.Value.Value < minValue.Y) {
        minValue.Y = item.Y.Value.Value;
        minKey.Y = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.Z.HasValue && !double.IsNaN(item.Z.Value.Value) && item.Z.Value.Value < minValue.Z) {
        minValue.Z = item.Z.Value.Value;
        minKey.Z = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.Xyz.HasValue && !double.IsNaN(item.Xyz.Value.Value) && item.Xyz.Value.Value < minValue.Xyz) {
        minValue.Xyz = item.Xyz.Value.Value;
        minKey.Xyz = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.Xx.HasValue && !double.IsNaN(item.Xx.Value.Value) && item.Xx.Value.Value < minValue.Xx) {
        minValue.Xx = item.Xx.Value.Value;
        minKey.Xx = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.Yy.HasValue && !double.IsNaN(item.Yy.Value.Value) && item.Yy.Value.Value < minValue.Yy) {
        minValue.Yy = item.Yy.Value.Value;
        minKey.Yy = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.Zz.HasValue && !double.IsNaN(item.Zz.Value.Value) && item.Zz.Value.Value < minValue.Zz) {
        minValue.Zz = item.Zz.Value.Value;
        minKey.Zz = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.Xxyyzz.HasValue && !double.IsNaN(item.Xxyyzz.Value.Value) && item.Xxyyzz.Value.Value < minValue.Xxyyzz) {
        minValue.Xxyyzz = item.Xxyyzz.Value.Value;
        minKey.Xxyyzz = new Entity0dExtremaKey(nodeId, permutation);
      }
    }
  }
}
