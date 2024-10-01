using System.Collections.Generic;

using OasysUnits;

namespace GsaGH.Parameters.Results {
  public static partial class ExtremaKeyUtility {
    public static (ResultTensor3<Entity2dExtremaKey> Max, ResultTensor3<Entity2dExtremaKey> Min) GetResultTensor3Entity2dExtremaKeys<T>(
      this IDictionary<int, IList<IMeshQuantity<T>>> subset)
      where T : IResultItem {

      var maxValue = new ResultTensor3<double>(double.MinValue);
      var minValue = new ResultTensor3<double>(double.MaxValue);

      var maxKey = new ResultTensor3<Entity2dExtremaKey>();
      var minKey = new ResultTensor3<Entity2dExtremaKey>();

      foreach (int elementId in subset.Keys) {
        IList<IMeshQuantity<T>> values = subset[elementId];
        for (int permutation = 0; permutation < values.Count; permutation++) {
          switch (values[permutation]) {
            case IMeshQuantity<IStress> stress:
              UpdateExtrema<IStress, Pressure>(stress.Results(),
                elementId, permutation, ref maxValue, ref minValue, ref maxKey, ref minKey);
              break;

          }
        }
      }

      return (maxKey, minKey);
    }

    private static void UpdateExtrema<T, Q>(IList<T> item, int elementId, int permutation,
      ref ResultTensor3<double> maxValue, ref ResultTensor3<double> minValue,
      ref ResultTensor3<Entity2dExtremaKey> maxKey, ref ResultTensor3<Entity2dExtremaKey> minKey)
      where T : IResultTensor3<Q> where Q : IQuantity {
      for (int i = 0; i < item.Count; i++) {
        if (item[i].Xx.Value > maxValue.Xx) {
          maxValue.Xx = item[i].Xx.Value;
          maxKey.Xx = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].Yy.Value > maxValue.Yy) {
          maxValue.Yy = item[i].Yy.Value;
          maxKey.Yy = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].Zz.Value > maxValue.Zz) {
          maxValue.Zz = item[i].Zz.Value;
          maxKey.Zz = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].Xy.Value > maxValue.Xy) {
          maxValue.Xy = item[i].Xy.Value;
          maxKey.Xy = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].Xy.Value > maxValue.Xy) {
          maxValue.Xy = item[i].Xy.Value;
          maxKey.Xy = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].Yz.Value > maxValue.Yz) {
          maxValue.Yz = item[i].Yz.Value;
          maxKey.Yz = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].Zx.Value > maxValue.Zx) {
          maxValue.Zx = item[i].Zx.Value;
          maxKey.Zx = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].Xx.Value < minValue.Xx) {
          minValue.Xx = item[i].Xx.Value;
          minKey.Xx = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].Yy.Value < minValue.Yy) {
          minValue.Yy = item[i].Yy.Value;
          minKey.Yy = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].Zz.Value < minValue.Zz) {
          minValue.Zz = item[i].Zz.Value;
          minKey.Zz = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].Xy.Value < minValue.Xy) {
          minValue.Xy = item[i].Xy.Value;
          minKey.Xy = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].Yz.Value < minValue.Yz) {
          minValue.Yz = item[i].Yz.Value;
          minKey.Yz = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].Zx.Value < minValue.Zx) {
          minValue.Zx = item[i].Zx.Value;
          minKey.Zx = new Entity2dExtremaKey(elementId, i, permutation);
        }
      }
    }
  }
}
