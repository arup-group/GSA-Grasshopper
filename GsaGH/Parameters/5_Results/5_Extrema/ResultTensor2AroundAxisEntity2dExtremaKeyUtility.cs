using System.Collections.Generic;
using OasysUnits;

namespace GsaGH.Parameters.Results {
  public static partial class ExtremaKeyUtility {
    public static (ResultTensor2AroundAxis<Entity2dExtremaKey> Max, ResultTensor2AroundAxis<Entity2dExtremaKey> Min) GetResultTensor2AroundAxisEntity2dExtremaKeys<T>(
      this IDictionary<int, IList<IMeshQuantity<T>>> subset)
      where T : IResultItem {

      var maxValue = new ResultTensor2AroundAxis<double>(double.MinValue);
      var minValue = new ResultTensor2AroundAxis<double>(double.MaxValue);

      var maxKey = new ResultTensor2AroundAxis<Entity2dExtremaKey>();
      var minKey = new ResultTensor2AroundAxis<Entity2dExtremaKey>();

      foreach (int elementId in subset.Keys) {
        IList<IMeshQuantity<T>> values = subset[elementId];
        for (int permutation = 0; permutation < values.Count; permutation++) {
          switch (values[permutation]) {
            case IMeshQuantity<IMoment2d> force:
              UpdateExtrema<IMoment2d, Force>(force.Results(),
                elementId, permutation, ref maxValue, ref minValue, ref maxKey, ref minKey);
              break;

          }
        }
      }

      return (maxKey, minKey);
    }

    private static void UpdateExtrema<T, Q>(IList<T> item, int elementId, int permutation,
      ref ResultTensor2AroundAxis<double> maxValue, ref ResultTensor2AroundAxis<double> minValue,
      ref ResultTensor2AroundAxis<Entity2dExtremaKey> maxKey, ref ResultTensor2AroundAxis<Entity2dExtremaKey> minKey)
      where T : IResultTensor2AroundAxis<Q> where Q : IQuantity {
      for (int i = 0; i < item.Count; i++) {
        if (item[i].Mx.Value > maxValue.Mx) {
          maxValue.Mx = (double)item[i].Mx.Value;
          maxKey.Mx = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].My.Value > maxValue.My) {
          maxValue.My = (double)item[i].My.Value;
          maxKey.My = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].Mxy.Value > maxValue.Mxy) {
          maxValue.Mxy = (double)item[i].Mxy.Value;
          maxKey.Mxy = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].WoodArmerX.Value > maxValue.WoodArmerX) {
          maxValue.WoodArmerX = (double)item[i].WoodArmerX.Value;
          maxKey.WoodArmerX = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].WoodArmerY.Value > maxValue.WoodArmerY) {
          maxValue.WoodArmerY = (double)item[i].WoodArmerY.Value;
          maxKey.WoodArmerY = new Entity2dExtremaKey(elementId, i, permutation);
        }
        if (item[i].Mx.Value < minValue.Mx) {
          minValue.Mx = (double)item[i].Mx.Value;
          minKey.Mx = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].My.Value < minValue.My) {
          minValue.My = (double)item[i].My.Value;
          minKey.My = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].Mxy.Value < minValue.Mxy) {
          minValue.Mxy = (double)item[i].Mxy.Value;
          minKey.Mxy = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].WoodArmerX.Value < minValue.WoodArmerX) {
          minValue.WoodArmerX = (double)item[i].WoodArmerX.Value;
          minKey.WoodArmerX = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].WoodArmerY.Value < minValue.WoodArmerY) {
          minValue.WoodArmerY = (double)item[i].WoodArmerY.Value;
          minKey.WoodArmerY = new Entity2dExtremaKey(elementId, i, permutation);
        }
      }
    }
  }
}