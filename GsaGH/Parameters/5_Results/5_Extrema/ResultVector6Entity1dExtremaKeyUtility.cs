using System.Collections.Generic;

using OasysUnits;

namespace GsaGH.Parameters.Results {
  public static partial class ExtremaKeyUtility {
    public static (ResultVector6<Entity1dExtremaKey> Max, ResultVector6<Entity1dExtremaKey> Min) GetResultVector6Entity1dExtremaKeys<T, U>(
      this IDictionary<int, IList<T>> subset)
      where T : IEntity1dQuantity<U> where U : IResultItem {

      var maxValue = new ResultVector6<double?>(double.MinValue);
      var minValue = new ResultVector6<double?>(double.MaxValue);

      var maxKey = new ResultVector6<Entity1dExtremaKey>();
      var minKey = new ResultVector6<Entity1dExtremaKey>();

      foreach (int elementId in subset.Keys) {
        IList<T> values = subset[elementId];
        for (int permutation = 0; permutation < values.Count; permutation++) {
          foreach (double position in values[permutation].Results.Keys) {
            switch (values[permutation]) {
              case IEntity1dQuantity<IDisplacement> displacement:
                UpdateExtrema<IDisplacement, Length, Angle>(displacement.Results[position],
                  elementId, permutation, position,
                  ref maxValue, ref minValue, ref maxKey, ref minKey);
                break;

              case IEntity1dQuantity<IInternalForce> force:
                UpdateExtrema<IInternalForce, Force, Moment>(force.Results[position],
                  elementId, permutation, position,
                  ref maxValue, ref minValue, ref maxKey, ref minKey);
                break;

            }
          }
        }
      }

      return (maxKey, minKey);
    }

    private static void UpdateExtrema<T, Q1, Q2>(T item, int elementId, int permutation, double position,
      ref ResultVector6<double?> maxValue, ref ResultVector6<double?> minValue,
      ref ResultVector6<Entity1dExtremaKey> maxKey, ref ResultVector6<Entity1dExtremaKey> minKey)
      where T : IResultVector6<Q1, Q2> where Q1 : IQuantity where Q2 : IQuantity {

      if (item.X.Value > maxValue.X) {
        maxValue.X = item.X.Value;
        maxKey.X = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.Y.Value > maxValue.Y) {
        maxValue.Y = item.Y.Value;
        maxKey.Y = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.Z.Value > maxValue.Z) {
        maxValue.Z = item.Z.Value;
        maxKey.Z = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.Xyz.Value > maxValue.Xyz) {
        maxValue.Xyz = item.Xyz.Value;
        maxKey.Xyz = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.Xx.Value > maxValue.Xx) {
        maxValue.Xx = item.Xx.Value;
        maxKey.Xx = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.Yy.Value > maxValue.Yy) {
        maxValue.Yy = item.Yy.Value;
        maxKey.Yy = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.Zz.Value > maxValue.Zz) {
        maxValue.Zz = item.Zz.Value;
        maxKey.Zz = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.Xxyyzz.Value > maxValue.Xxyyzz) {
        maxValue.Xxyyzz = item.Xxyyzz.Value;
        maxKey.Xxyyzz = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.X.Value < minValue.X) {
        minValue.X = item.X.Value;
        minKey.X = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.Y.Value < minValue.Y) {
        minValue.Y = item.Y.Value;
        minKey.Y = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.Z.Value < minValue.Z) {
        minValue.Z = item.Z.Value;
        minKey.Z = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.Xyz.Value < minValue.Xyz) {
        minValue.Xyz = item.Xyz.Value;
        minKey.Xyz = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.Xx.Value < minValue.Xx) {
        minValue.Xx = item.Xx.Value;
        minKey.Xx = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.Yy.Value < minValue.Yy) {
        minValue.Yy = item.Yy.Value;
        minKey.Yy = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.Zz.Value < minValue.Zz) {
        minValue.Zz = item.Zz.Value;
        minKey.Zz = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.Xxyyzz.Value < minValue.Xxyyzz) {
        minValue.Xxyyzz = item.Xxyyzz.Value;
        minKey.Xxyyzz = new Entity1dExtremaKey(elementId, position, permutation);
      }
    }
  }
}
