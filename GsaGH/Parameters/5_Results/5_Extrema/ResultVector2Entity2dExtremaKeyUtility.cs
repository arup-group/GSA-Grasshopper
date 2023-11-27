using System.Collections.Generic;
using System.Collections.ObjectModel;
using OasysUnits;

namespace GsaGH.Parameters.Results {
  public static partial class ExtremaKeyUtility {
    public static (ResultVector2<Entity2dExtremaKey> Max, ResultVector2<Entity2dExtremaKey> Min) GetResultVector2Entity2dExtremaKeys<T>(
      this IDictionary<int, Collection<IEntity2dQuantity<T>>> subset)
      where T : IResultItem {

      var maxValue = new ResultVector2<double>(double.MinValue);
      var minValue = new ResultVector2<double>(double.MaxValue);

      var maxKey = new ResultVector2<Entity2dExtremaKey>();
      var minKey = new ResultVector2<Entity2dExtremaKey>();

      foreach (int elementId in subset.Keys) {
        Collection<IEntity2dQuantity<T>> values = subset[elementId];
        for (int permutation = 0; permutation < values.Count; permutation++) {
            switch (values[permutation]) {
              case IEntity2dQuantity<IShear2d> displacement:
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
        if (item[i].Vx.Value > maxValue.Vx) {
          maxValue.Vx = item[i].Vx.Value;
          maxKey.Vx = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].Vy.Value > maxValue.Vy) {
          maxValue.Vy = item[i].Vy.Value;
          maxKey.Vy = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].Vx.Value < minValue.Vx) {
          minValue.Vx = item[i].Vx.Value;
          minKey.Vx = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].Vy.Value < minValue.Vy) {
          minValue.Vy = item[i].Vy.Value;
          minKey.Vy = new Entity2dExtremaKey(elementId, i, permutation);
        }
      }
    }
  }
}