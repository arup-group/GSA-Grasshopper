using System.Collections.Generic;
using OasysUnits;

namespace GsaGH.Parameters.Results {
  public static partial class ExtremaKeyUtility {
    public static (ResultVector6<NodeExtremaKey> Max, ResultVector6<NodeExtremaKey> Min) GetResultVector6NodeExtremaKeys<T>(
      this IDictionary<int, IList<T>> subset) {

      var maxValue = new ResultVector6<double?>(double.MinValue);
      var minValue = new ResultVector6<double?>(double.MaxValue);

      var maxKey = new ResultVector6<NodeExtremaKey>();
      var minKey = new ResultVector6<NodeExtremaKey>();

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

    private static void UpdateExtrema(IReactionForce item, int nodeId, int permutation,
      ref ResultVector6<double?> maxValue, ref ResultVector6<double?> minValue,
      ref ResultVector6<NodeExtremaKey> maxKey, ref ResultVector6<NodeExtremaKey> minKey) {

      if (item.X != null && ((Force)item.X).Value > maxValue.X) {
        maxValue.X = ((Force)item.X).Value;
        maxKey.X = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.Y != null && ((Force)item.Y).Value > maxValue.Y) {
        maxValue.Y = ((Force)item.Y).Value;
        maxKey.Y = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.Z != null && ((Force)item.Z).Value > maxValue.Z) {
        maxValue.Z = ((Force)item.Z).Value;
        maxKey.Z = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.Xyz != null && ((Force)item.Xyz).Value > maxValue.Xyz) {
        maxValue.Xyz = ((Force)item.Xyz).Value;
        maxKey.Xyz = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.Xx != null && ((Moment)item.Xx).Value > maxValue.Xx) {
        maxValue.Xx = ((Moment)item.Xx).Value;
        maxKey.Xx = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.Yy != null && ((Moment)item.Yy).Value > maxValue.Yy) {
        maxValue.Yy = ((Moment)item.Yy).Value;
        maxKey.Yy = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.Zz != null && ((Moment)item.Zz).Value > maxValue.Zz) {
        maxValue.Zz = ((Moment)item.Zz).Value;
        maxKey.Zz = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.Xxyyzz != null && ((Moment)item.Xxyyzz).Value > maxValue.Xxyyzz) {
        maxValue.Xxyyzz = ((Moment)item.Xxyyzz).Value;
        maxKey.Xxyyzz = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.X != null && ((Force)item.X).Value < minValue.X) {
        minValue.X = ((Force)item.X).Value;
        minKey.X = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.X != null && ((Force)item.Y).Value < minValue.Y) {
        minValue.Y = ((Force)item.Y).Value;
        minKey.Y = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.Z != null && ((Force)item.Z).Value < minValue.Z) {
        minValue.Z = ((Force)item.Z).Value;
        minKey.Z = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.Xyz != null && ((Force)item.Xyz).Value < minValue.Xyz) {
        minValue.Xyz = ((Force)item.Xyz).Value;
        minKey.Xyz = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.Xx != null && ((Moment)item.Xx).Value < minValue.Xx) {
        minValue.Xx = ((Moment)item.Xx).Value;
        minKey.Xx = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.Yy != null && ((Moment)item.Yy).Value < minValue.Yy) {
        minValue.Yy = ((Moment)item.Yy).Value;
        minKey.Yy = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.Zz != null && ((Moment)item.Zz).Value < minValue.Zz) {
        minValue.Zz = ((Moment)item.Zz).Value;
        minKey.Zz = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.Xxyyzz != null && ((Moment)item.Xxyyzz).Value < minValue.Xxyyzz) {
        minValue.Xxyyzz = ((Moment)item.Xxyyzz).Value;
        minKey.Xxyyzz = new NodeExtremaKey(nodeId, permutation);
      }
    }
  }
}