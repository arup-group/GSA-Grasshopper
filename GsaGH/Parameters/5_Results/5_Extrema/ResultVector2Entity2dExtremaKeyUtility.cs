using System.Collections.Generic;

using OasysUnits;

namespace GsaGH.Parameters.Results {
  public static partial class ExtremaKeyUtility {
    public static (ResultVector2<Entity2dExtremaKey> Max, ResultVector2<Entity2dExtremaKey> Min) GetResultVector2Entity2dExtremaKeys<T>(
      this IDictionary<int, IList<IMeshQuantity<T>>> subset)
      where T : IResultItem {

      var maxValue = new ResultVector2<double>(double.MinValue);
      var minValue = new ResultVector2<double>(double.MaxValue);

      var maxKey = new ResultVector2<Entity2dExtremaKey>();
      var minKey = new ResultVector2<Entity2dExtremaKey>();

      foreach (int elementId in subset.Keys) {
        IList<IMeshQuantity<T>> values = subset[elementId];
        for (int permutation = 0; permutation < values.Count; permutation++) {
          switch (values[permutation]) {
            case IMeshQuantity<IShear2d> displacement:
              UpdateExtrema<IShear2d, ForcePerLength>(displacement.Results(),
                elementId, permutation, ref maxValue, ref minValue, ref maxKey, ref minKey);
              break;
          }
        }
      }

      return (maxKey, minKey);
    }

    private static void UpdateExtrema<T, Q1>(IList<T> item, int elementId, int permutation,
      ref ResultVector2<double> maxValue, ref ResultVector2<double> minValue,
      ref ResultVector2<Entity2dExtremaKey> maxKey, ref ResultVector2<Entity2dExtremaKey> minKey)
      where T : IResultVector2<Q1> where Q1 : IQuantity {
      for (int i = 0; i < item.Count; i++) {
        if (item[i].Qx.Value > maxValue.Qx) {
          maxValue.Qx = item[i].Qx.Value;
          maxKey.Qx = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].Qy.Value > maxValue.Qy) {
          maxValue.Qy = item[i].Qy.Value;
          maxKey.Qy = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].Qx.Value < minValue.Qx) {
          minValue.Qx = item[i].Qx.Value;
          minKey.Qx = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].Qy.Value < minValue.Qy) {
          minValue.Qy = item[i].Qy.Value;
          minKey.Qy = new Entity2dExtremaKey(elementId, i, permutation);
        }
      }
    }
  }
}
