using System.Collections.Generic;

using OasysUnits;

namespace GsaGH.Parameters.Results {
  public static partial class ExtremaKeyUtility {
    public static (ResultVector3InAxis<Entity2dExtremaKey> Max, ResultVector3InAxis<Entity2dExtremaKey> Min) GetResultVector3Entity2dExtremaKeys<T>(
      this IDictionary<int, IList<IMeshQuantity<T>>> subset)
      where T : IResultItem {

      var maxValue = new ResultVector3InAxis<double>(double.MinValue);
      var minValue = new ResultVector3InAxis<double>(double.MaxValue);

      var maxKey = new ResultVector3InAxis<Entity2dExtremaKey>();
      var minKey = new ResultVector3InAxis<Entity2dExtremaKey>();

      foreach (int elementId in subset.Keys) {
        IList<IMeshQuantity<T>> values = subset[elementId];
        for (int permutation = 0; permutation < values.Count; permutation++) {
          switch (values[permutation]) {
            case IMeshQuantity<ITranslation> displacement:
              UpdateExtrema<ITranslation, Length>(displacement.Results(),
                elementId, permutation, ref maxValue, ref minValue, ref maxKey, ref minKey);
              break;

          }
        }
      }

      return (maxKey, minKey);
    }

    private static void UpdateExtrema<T, Q1>(IList<T> item, int elementId, int permutation,
      ref ResultVector3InAxis<double> maxValue, ref ResultVector3InAxis<double> minValue,
      ref ResultVector3InAxis<Entity2dExtremaKey> maxKey, ref ResultVector3InAxis<Entity2dExtremaKey> minKey)
      where T : IResultVector3InAxis<Q1> where Q1 : IQuantity {
      for (int i = 0; i < item.Count; i++) {
        if (item[i].X.Value > maxValue.X) {
          maxValue.X = item[i].X.Value;
          maxKey.X = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].Y.Value > maxValue.Y) {
          maxValue.Y = item[i].Y.Value;
          maxKey.Y = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].Z.Value > maxValue.Z) {
          maxValue.Z = item[i].Z.Value;
          maxKey.Z = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].Xyz.Value > maxValue.Xyz) {
          maxValue.Xyz = item[i].Xyz.Value;
          maxKey.Xyz = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].X.Value < minValue.X) {
          minValue.X = item[i].X.Value;
          minKey.X = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].Y.Value < minValue.Y) {
          minValue.Y = item[i].Y.Value;
          minKey.Y = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].Z.Value < minValue.Z) {
          minValue.Z = item[i].Z.Value;
          minKey.Z = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].Xyz.Value < minValue.Xyz) {
          minValue.Xyz = item[i].Xyz.Value;
          minKey.Xyz = new Entity2dExtremaKey(elementId, i, permutation);
        }
      }
    }
  }
}
