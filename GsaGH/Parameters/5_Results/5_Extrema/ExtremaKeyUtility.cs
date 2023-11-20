using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;
using OasysUnits;

namespace GsaGH.Parameters.Results {
  public static class ExtremaKeyUtility {
    public static (ResultVector6<Entity1dExtremaKey> Max, ResultVector6<Entity1dExtremaKey> Min) Extrema<T, U>(
      this IDictionary<int, Collection<T>> subset)
      where T : IEntity1dQuantity<U> where U : IResultItem {

      var maxValue = new ResultVector6<double>(double.MinValue);
      var minValue = new ResultVector6<double>(double.MaxValue);

      var maxKey = new ResultVector6<Entity1dExtremaKey>();
      var minKey = new ResultVector6<Entity1dExtremaKey>();

      foreach (int elementId in subset.Keys) {
        Collection<T> values = subset[elementId];
        for (int permutation = 0; permutation < values.Count; permutation++) {
          foreach (double position in values[permutation].Results.Keys) {
            switch (values[permutation]) {
              case IEntity1dDisplacement displacement:
                UpdateExtrema<IDisplacement, Length, Angle>(displacement.Results[position],
                  elementId, permutation, position,
                  ref maxValue, ref minValue, ref maxKey, ref minKey);
                break;

              case IEntity1dInternalForce force:
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

    public static (ResultStress1d<Entity1dExtremaKey> Max, ResultStress1d<Entity1dExtremaKey> Min) Extrema(
      this IDictionary<int, Collection<IEntity1dStress>> subset) {

      var maxValue = new ResultStress1d<double>(double.MinValue);
      var minValue = new ResultStress1d<double>(double.MaxValue);

      var maxKey = new ResultStress1d<Entity1dExtremaKey>();
      var minKey = new ResultStress1d<Entity1dExtremaKey>();

      foreach (int elementId in subset.Keys) {
        Collection<IEntity1dStress> values = subset[elementId];
        for (int permutation = 0; permutation < values.Count; permutation++) {
          foreach (double position in values[permutation].Results.Keys) {
            UpdateExtrema(values[permutation].Results[position],
              elementId, permutation, position,
              ref maxValue, ref minValue, ref maxKey, ref minKey);
          }
        }
      }

      return (maxKey, minKey);
    }

    public static (ResultDerivedStress1d<Entity1dExtremaKey> Max, ResultDerivedStress1d<Entity1dExtremaKey> Min) Extrema(
      this IDictionary<int, Collection<IEntity1dDerivedStress>> subset) {

      var maxValue = new ResultDerivedStress1d<double>(double.MinValue);
      var minValue = new ResultDerivedStress1d<double>(double.MaxValue);

      var maxKey = new ResultDerivedStress1d<Entity1dExtremaKey>();
      var minKey = new ResultDerivedStress1d<Entity1dExtremaKey>();

      foreach (int elementId in subset.Keys) {
        Collection<IEntity1dDerivedStress> values = subset[elementId];
        for (int permutation = 0; permutation < values.Count; permutation++) {
          foreach (double position in values[permutation].Results.Keys) {
            UpdateExtrema(values[permutation].Results[position],
              elementId, permutation, position,
              ref maxValue, ref minValue, ref maxKey, ref minKey);
          }
        }
      }

      return (maxKey, minKey);
    }

    public static (ResultFootfall<NodeExtremaKey> Max, ResultFootfall<NodeExtremaKey> Min) Extrema(
      this IDictionary<int, Collection<IFootfall>> subset) {

      var maxValue = new ResultFootfall<double>(double.MinValue);
      var minValue = new ResultFootfall<double>(double.MaxValue);

      var maxKey = new ResultFootfall<NodeExtremaKey>();
      var minKey = new ResultFootfall<NodeExtremaKey>();

      foreach (int nodeId in subset.Keys) {
        Collection<IFootfall> values = subset[nodeId];
        for (int permutation = 0; permutation < values.Count; permutation++) {
          UpdateExtrema(values[permutation], nodeId, permutation,
                  ref maxValue, ref minValue, ref maxKey, ref minKey);
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

    private static void UpdateExtrema(IFootfall item, int nodeId, int permutation,
      ref ResultFootfall<double> maxValue, ref ResultFootfall<double> minValue,
      ref ResultFootfall<NodeExtremaKey> maxKey, ref ResultFootfall<NodeExtremaKey> minKey) {

      if (item.CriticalFrequency.Value > maxValue.CriticalFrequency) {
        maxValue.CriticalFrequency = item.CriticalFrequency.Value;
        maxKey.CriticalFrequency = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.MaximumResponseFactor > maxValue.MaximumResponseFactor) {
        maxValue.MaximumResponseFactor = item.MaximumResponseFactor;
        maxKey.MaximumResponseFactor = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.PeakAcceleration.Value > maxValue.PeakAcceleration) {
        maxValue.PeakAcceleration = item.PeakAcceleration.Value;
        maxKey.PeakAcceleration = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.PeakVelocity.Value > maxValue.PeakVelocity) {
        maxValue.PeakVelocity = item.PeakVelocity.Value;
        maxKey.PeakVelocity = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.RmsAcceleration.Value > maxValue.RmsAcceleration) {
        maxValue.RmsAcceleration = item.RmsAcceleration.Value;
        maxKey.RmsAcceleration = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.RmsVelocity.Value > maxValue.RmsVelocity) {
        maxValue.RmsVelocity = item.RmsVelocity.Value;
        maxKey.RmsVelocity = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.CriticalFrequency.Value < minValue.CriticalFrequency) {
        minValue.CriticalFrequency = item.CriticalFrequency.Value;
        minKey.CriticalFrequency = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.MaximumResponseFactor < minValue.MaximumResponseFactor) {
        minValue.MaximumResponseFactor = item.MaximumResponseFactor;
        minKey.MaximumResponseFactor = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.PeakAcceleration.Value < minValue.PeakAcceleration) {
        minValue.PeakAcceleration = item.PeakAcceleration.Value;
        minKey.PeakAcceleration = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.PeakVelocity.Value < minValue.PeakVelocity) {
        minValue.PeakVelocity = item.PeakVelocity.Value;
        minKey.PeakVelocity = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.RmsAcceleration.Value < minValue.RmsAcceleration) {
        minValue.RmsAcceleration = item.RmsAcceleration.Value;
        minKey.RmsAcceleration = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.RmsVelocity.Value < minValue.RmsVelocity) {
        minValue.RmsVelocity = item.RmsVelocity.Value;
        minKey.RmsVelocity = new NodeExtremaKey(nodeId, permutation);
      }
    }

    private static void UpdateExtrema(IStress1d item, int elementId, int permutation, double position,
      ref ResultStress1d<double> maxValue, ref ResultStress1d<double> minValue,
      ref ResultStress1d<Entity1dExtremaKey> maxKey, ref ResultStress1d<Entity1dExtremaKey> minKey) {

      if (item.Axial.Value > maxValue.Axial) {
        maxValue.Axial = item.Axial.Value;
        maxKey.Axial = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.BendingYyNegativeZ.Value > maxValue.BendingYyNegativeZ) {
        maxValue.BendingYyNegativeZ = item.BendingYyNegativeZ.Value;
        maxKey.BendingYyNegativeZ = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.BendingYyPositiveZ.Value > maxValue.BendingYyPositiveZ) {
        maxValue.BendingYyPositiveZ = item.BendingYyPositiveZ.Value;
        maxKey.BendingYyPositiveZ = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.BendingZzNegativeY.Value > maxValue.BendingZzNegativeY) {
        maxValue.BendingZzNegativeY = item.BendingZzNegativeY.Value;
        maxKey.BendingZzNegativeY = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.BendingZzPositiveY.Value > maxValue.BendingZzPositiveY) {
        maxValue.BendingZzPositiveY = item.BendingZzPositiveY.Value;
        maxKey.BendingZzPositiveY = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.CombinedC1.Value > maxValue.CombinedC1) {
        maxValue.CombinedC1 = item.CombinedC1.Value;
        maxKey.CombinedC1 = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.CombinedC2.Value > maxValue.CombinedC2) {
        maxValue.CombinedC2 = item.CombinedC2.Value;
        maxKey.CombinedC2 = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.ShearY.Value > maxValue.ShearY) {
        maxValue.ShearY = item.ShearY.Value;
        maxKey.ShearY = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.ShearZ.Value > maxValue.ShearZ) {
        maxValue.ShearZ = item.ShearZ.Value;
        maxKey.ShearZ = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.Axial.Value < minValue.Axial) {
        minValue.Axial = item.Axial.Value;
        minKey.Axial = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.BendingYyNegativeZ.Value < minValue.BendingYyNegativeZ) {
        minValue.BendingYyNegativeZ = item.BendingYyNegativeZ.Value;
        minKey.BendingYyNegativeZ = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.BendingYyPositiveZ.Value < minValue.BendingYyPositiveZ) {
        minValue.BendingYyPositiveZ = item.BendingYyPositiveZ.Value;
        minKey.BendingYyPositiveZ = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.BendingZzNegativeY.Value < minValue.BendingZzNegativeY) {
        minValue.BendingZzNegativeY = item.BendingZzNegativeY.Value;
        minKey.BendingZzNegativeY = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.BendingZzPositiveY.Value < minValue.BendingZzPositiveY) {
        minValue.BendingZzPositiveY = item.BendingZzPositiveY.Value;
        minKey.BendingZzPositiveY = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.CombinedC1.Value < minValue.CombinedC1) {
        minValue.CombinedC1 = item.CombinedC1.Value;
        minKey.CombinedC1 = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.CombinedC2.Value < minValue.CombinedC2) {
        minValue.CombinedC2 = item.CombinedC2.Value;
        minKey.CombinedC2 = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.ShearY.Value < minValue.ShearY) {
        minValue.ShearY = item.ShearY.Value;
        minKey.ShearY = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.ShearZ.Value < minValue.ShearZ) {
        minValue.ShearZ = item.ShearZ.Value;
        minKey.ShearZ = new Entity1dExtremaKey(elementId, position, permutation);
      }
    }

    private static void UpdateExtrema(IStress1dDerived item, int elementId, int permutation, double position,
      ref ResultDerivedStress1d<double> maxValue, ref ResultDerivedStress1d<double> minValue,
      ref ResultDerivedStress1d<Entity1dExtremaKey> maxKey, ref ResultDerivedStress1d<Entity1dExtremaKey> minKey) {

      if (item.ElasticShearY.Value > maxValue.ElasticShearY) {
        maxValue.ElasticShearY = item.ElasticShearY.Value;
        maxKey.ElasticShearY = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.ElasticShearZ.Value > maxValue.ElasticShearZ) {
        maxValue.ElasticShearZ = item.ElasticShearZ.Value;
        maxKey.ElasticShearZ = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.Torsional.Value > maxValue.Torsional) {
        maxValue.Torsional = item.Torsional.Value;
        maxKey.Torsional = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.VonMises.Value > maxValue.VonMises) {
        maxValue.VonMises = item.VonMises.Value;
        maxKey.VonMises = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.ElasticShearY.Value < minValue.ElasticShearY) {
        minValue.ElasticShearY = item.ElasticShearY.Value;
        minKey.ElasticShearY = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.ElasticShearZ.Value < minValue.ElasticShearZ) {
        minValue.ElasticShearZ = item.ElasticShearZ.Value;
        minKey.ElasticShearZ = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.Torsional.Value < minValue.Torsional) {
        minValue.Torsional = item.Torsional.Value;
        minKey.Torsional = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.VonMises.Value < minValue.VonMises) {
        minValue.VonMises = item.VonMises.Value;
        minKey.VonMises = new Entity1dExtremaKey(elementId, position, permutation);
      }
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