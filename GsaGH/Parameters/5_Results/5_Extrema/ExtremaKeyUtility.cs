using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using OasysUnits;

namespace GsaGH.Parameters.Results {
  public static class ExtremaKeyUtility {
    public static (ResultVector6<Element1dExtremaKey> Max, ResultVector6<Element1dExtremaKey> Min) Extrema<T, U>(
      this IDictionary<int, Collection<T>> subset)
      where T : IElement1dQuantity<U> where U : IResultItem {

      var maxValue = new ResultVector6<double>(double.MinValue);
      var minValue = new ResultVector6<double>(double.MaxValue);

      var maxKey = new ResultVector6<Element1dExtremaKey>();
      var minKey = new ResultVector6<Element1dExtremaKey>();

      foreach (int elementId in subset.Keys) {
        Collection<T> values = subset[elementId];
        for (int permutation = 0; permutation < values.Count; permutation++) {
          foreach (double position in values[permutation].Results.Keys) {
            switch (values[permutation]) {
              case IElement1dDisplacement displacement:
                UpdateExtrema<IDisplacement, Length, Angle>(displacement.Results[position], 
                  elementId, permutation, position,
                  ref maxValue, ref minValue, ref maxKey, ref minKey);
                break;

              case IElement1dInternalForce displacement:
                UpdateExtrema<IInternalForce, Force, Moment>(displacement.Results[position],
                  elementId, permutation, position,
                  ref maxValue, ref minValue, ref maxKey, ref minKey);
                break;

            }
          }
        }
      }

      return (maxKey, minKey);
    }

    public static (ResultVector6<NodeExtremaKey> Max, ResultVector6<NodeExtremaKey> Min) Extrema<T>(
      this IDictionary<int, Collection<T>> subset) {

      var maxValue = new ResultVector6<double>(double.MinValue);
      var minValue = new ResultVector6<double>(double.MaxValue);

      var maxKey = new ResultVector6<NodeExtremaKey>();
      var minKey = new ResultVector6<NodeExtremaKey>();

      foreach (int nodeId in subset.Keys) {
        Collection<T> values = subset[nodeId];
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
          }
        }
      }

      return (maxKey, minKey);
    }

    private static void UpdateExtrema<T, Q1, Q2>(T item, int nodeId, int permutation,
      ref ResultVector6<double> maxValue, ref ResultVector6<double> minValue,
      ref ResultVector6<NodeExtremaKey> maxKey, ref ResultVector6<NodeExtremaKey> minKey)
      where T : IResultVector6<Q1, Q2> where Q1 : IQuantity where Q2 : IQuantity {

      if (item.X.Value > maxValue.X) {
        maxValue.X = item.X.Value;
        maxKey.X = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.Y.Value > maxValue.Y) {
        maxValue.Y = item.Y.Value;
        maxKey.Y = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.Z.Value > maxValue.Z) {
        maxValue.Z = item.Z.Value;
        maxKey.Z = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.Xyz.Value > maxValue.Xyz) {
        maxValue.Xyz = item.Xyz.Value;
        maxKey.Xyz = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.Xx.Value > maxValue.Xx) {
        maxValue.Xx = item.Xx.Value;
        maxKey.Xx = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.Yy.Value > maxValue.Yy) {
        maxValue.Yy = item.Yy.Value;
        maxKey.Yy = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.Zz.Value > maxValue.Zz) {
        maxValue.Zz = item.Zz.Value;
        maxKey.Zz = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.Xxyyzz.Value > maxValue.Xxyyzz) {
        maxValue.Xxyyzz = item.Xxyyzz.Value;
        maxKey.Xxyyzz = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.X.Value < minValue.X) {
        minValue.X = item.X.Value;
        minKey.X = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.Y.Value < minValue.Y) {
        minValue.Y = item.Y.Value;
        minKey.Y = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.Z.Value < minValue.Z) {
        minValue.Z = item.Z.Value;
        minKey.Z = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.Xyz.Value < minValue.Xyz) {
        minValue.Xyz = item.Xyz.Value;
        minKey.Xyz = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.Xx.Value < minValue.Xx) {
        minValue.Xx = item.Xx.Value;
        minKey.Xx = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.Yy.Value < minValue.Yy) {
        minValue.Yy = item.Yy.Value;
        minKey.Yy = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.Zz.Value < minValue.Zz) {
        minValue.Zz = item.Zz.Value;
        minKey.Zz = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.Xxyyzz.Value < minValue.Xxyyzz) {
        minValue.Xxyyzz = item.Xxyyzz.Value;
        minKey.Xxyyzz = new NodeExtremaKey(nodeId, permutation);
      }
    }

    private static void UpdateExtrema<T, Q1, Q2>(T item, int elementId, int permutation, double position,
      ref ResultVector6<double> maxValue, ref ResultVector6<double> minValue,
      ref ResultVector6<Element1dExtremaKey> maxKey, ref ResultVector6<Element1dExtremaKey> minKey)
      where T : IResultVector6<Q1, Q2> where Q1 : IQuantity where Q2 : IQuantity {

      if (item.X.Value > maxValue.X) {
        maxValue.X = item.X.Value;
        maxKey.X = new Element1dExtremaKey(elementId, position, permutation);
      }

      if (item.Y.Value > maxValue.Y) {
        maxValue.Y = item.Y.Value;
        maxKey.Y = new Element1dExtremaKey(elementId, position, permutation);
      }

      if (item.Z.Value > maxValue.Z) {
        maxValue.Z = item.Z.Value;
        maxKey.Z = new Element1dExtremaKey(elementId, position, permutation);
      }

      if (item.Xyz.Value > maxValue.Xyz) {
        maxValue.Xyz = item.Xyz.Value;
        maxKey.Xyz = new Element1dExtremaKey(elementId, position, permutation);
      }

      if (item.Xx.Value > maxValue.Xx) {
        maxValue.Xx = item.Xx.Value;
        maxKey.Xx = new Element1dExtremaKey(elementId, position, permutation);
      }

      if (item.Yy.Value > maxValue.Yy) {
        maxValue.Yy = item.Yy.Value;
        maxKey.Yy = new Element1dExtremaKey(elementId, position, permutation);
      }

      if (item.Zz.Value > maxValue.Zz) {
        maxValue.Zz = item.Zz.Value;
        maxKey.Zz = new Element1dExtremaKey(elementId, position, permutation);
      }

      if (item.Xxyyzz.Value > maxValue.Xxyyzz) {
        maxValue.Xxyyzz = item.Xxyyzz.Value;
        maxKey.Xxyyzz = new Element1dExtremaKey(elementId, position, permutation);
      }

      if (item.X.Value < minValue.X) {
        minValue.X = item.X.Value;
        minKey.X = new Element1dExtremaKey(elementId, position, permutation);
      }

      if (item.Y.Value < minValue.Y) {
        minValue.Y = item.Y.Value;
        minKey.Y = new Element1dExtremaKey(elementId, position, permutation);
      }

      if (item.Z.Value < minValue.Z) {
        minValue.Z = item.Z.Value;
        minKey.Z = new Element1dExtremaKey(elementId, position, permutation);
      }

      if (item.Xyz.Value < minValue.Xyz) {
        minValue.Xyz = item.Xyz.Value;
        minKey.Xyz = new Element1dExtremaKey(elementId, position, permutation);
      }

      if (item.Xx.Value < minValue.Xx) {
        minValue.Xx = item.Xx.Value;
        minKey.Xx = new Element1dExtremaKey(elementId, position, permutation);
      }

      if (item.Yy.Value < minValue.Yy) {
        minValue.Yy = item.Yy.Value;
        minKey.Yy = new Element1dExtremaKey(elementId, position, permutation);
      }

      if (item.Zz.Value < minValue.Zz) {
        minValue.Zz = item.Zz.Value;
        minKey.Zz = new Element1dExtremaKey(elementId, position, permutation);
      }

      if (item.Xxyyzz.Value < minValue.Xxyyzz) {
        minValue.Xxyyzz = item.Xxyyzz.Value;
        minKey.Xxyyzz = new Element1dExtremaKey(elementId, position, permutation);
      }
    }
  }
}
