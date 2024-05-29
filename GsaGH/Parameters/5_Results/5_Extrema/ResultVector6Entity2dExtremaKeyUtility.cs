using System.Collections.Generic;
using OasysUnits;

namespace GsaGH.Parameters.Results {
  public static partial class ExtremaKeyUtility {
    public static (ResultVector6<Entity2dExtremaKey> Max, ResultVector6<Entity2dExtremaKey> Min) GetResultVector6Entity2dExtremaKeys<T>(
      this IDictionary<int, IList<IMeshQuantity<T>>> subset)
      where T : IResultItem {

      var maxValue = new ResultVector6<double>(double.MinValue);
      var minValue = new ResultVector6<double>(double.MaxValue);

      var maxKey = new ResultVector6<Entity2dExtremaKey>();
      var minKey = new ResultVector6<Entity2dExtremaKey>();

      foreach (int elementId in subset.Keys) {
        IList<IMeshQuantity<T>> values = subset[elementId];
        for (int permutation = 0; permutation < values.Count; permutation++) {
          switch (values[permutation]) {
            case IMeshQuantity<IDisplacement> displacement:
              UpdateExtrema<IDisplacement, Length, Angle>(displacement.Results(),
                elementId, permutation, ref maxValue, ref minValue, ref maxKey, ref minKey);
              break;

          }
        }
      }

      return (maxKey, minKey);
    }

    private static void UpdateExtrema<T, Q1, Q2>(IList<T> item, int elementId, int permutation,
      ref ResultVector6<double> maxValue, ref ResultVector6<double> minValue,
      ref ResultVector6<Entity2dExtremaKey> maxKey, ref ResultVector6<Entity2dExtremaKey> minKey)
      where T : IResultVector6<Q1, Q2> where Q1 : IQuantity where Q2 : IQuantity {
      for (int i = 0; i < item.Count; i++) {
        if (item[i].X.Value > maxValue.X) {
          maxValue.X = (double)item[i].X.Value;
          maxKey.X = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].Y.Value > maxValue.Y) {
          maxValue.Y = (double)item[i].Y.Value;
          maxKey.Y = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].Z.Value > maxValue.Z) {
          maxValue.Z = (double)item[i].Z.Value;
          maxKey.Z = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].Xyz.Value > maxValue.Xyz) {
          maxValue.Xyz = (double)item[i].Xyz.Value;
          maxKey.Xyz = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].Xx.Value > maxValue.Xx) {
          maxValue.Xx = (double)item[i].Xx.Value;
          maxKey.Xx = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].Yy.Value > maxValue.Yy) {
          maxValue.Yy = (double)item[i].Yy.Value;
          maxKey.Yy = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].Zz.Value > maxValue.Zz) {
          maxValue.Zz = (double)item[i].Zz.Value;
          maxKey.Zz = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].Xxyyzz.Value > maxValue.Xxyyzz) {
          maxValue.Xxyyzz = (double)item[i].Xxyyzz.Value;
          maxKey.Xxyyzz = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].X.Value < minValue.X) {
          minValue.X = (double)item[i].X.Value;
          minKey.X = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].Y.Value < minValue.Y) {
          minValue.Y = (double)item[i].Y.Value;
          minKey.Y = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].Z.Value < minValue.Z) {
          minValue.Z = (double)item[i].Z.Value;
          minKey.Z = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].Xyz.Value < minValue.Xyz) {
          minValue.Xyz = (double)item[i].Xyz.Value;
          minKey.Xyz = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].Xx.Value < minValue.Xx) {
          minValue.Xx = (double)item[i].Xx.Value;
          minKey.Xx = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].Yy.Value < minValue.Yy) {
          minValue.Yy = (double)item[i].Yy.Value;
          minKey.Yy = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].Zz.Value < minValue.Zz) {
          minValue.Zz = (double)item[i].Zz.Value;
          minKey.Zz = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].Xxyyzz.Value < minValue.Xxyyzz) {
          minValue.Xxyyzz = (double)item[i].Xxyyzz.Value;
          minKey.Xxyyzz = new Entity2dExtremaKey(elementId, i, permutation);
        }
      }
    }
  }
}