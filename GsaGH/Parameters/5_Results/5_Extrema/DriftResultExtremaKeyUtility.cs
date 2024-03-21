﻿using System.Collections.Generic;
using OasysUnits;

namespace GsaGH.Parameters.Results {
  public static partial class ExtremaKeyUtility {
    public static (ResultVector6<Entity1dExtremaKey> Max, ResultVector6<Entity1dExtremaKey> Min) GetResultVector6AssemblyExtremaKeys<T, U>(
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

    public static (DriftResultVector<Entity1dExtremaKey> Max, DriftResultVector<Entity1dExtremaKey> Min) GetDriftResultExtremaKeys<T, U>(
      this IDictionary<int, IList<T>> subset)
      where T : IEntity1dQuantity<U> where U : IResultItem {

      var maxValue = new DriftResultVector<double>(double.MinValue);
      var minValue = new DriftResultVector<double>(double.MaxValue);

      var maxKey = new DriftResultVector<Entity1dExtremaKey>();
      var minKey = new DriftResultVector<Entity1dExtremaKey>();

      foreach (int assemblyId in subset.Keys) {
        IList<T> values = subset[assemblyId];
        for (int permutation = 0; permutation < values.Count; permutation++) {
          foreach (double position in values[permutation].Results.Keys) {
            switch (values[permutation]) {
              case IEntity1dQuantity<IDrift<Length>> drift:
                UpdateExtrema<IDrift<Length>, Length>(drift.Results[position],
                  assemblyId, permutation, position,
                  ref maxValue, ref minValue, ref maxKey, ref minKey);
                break;

              case IEntity1dQuantity<IDrift<double>> driftIndex:
                UpdateExtrema(driftIndex.Results[position],
                  assemblyId, permutation, position,
                  ref maxValue, ref minValue, ref maxKey, ref minKey);
                break;

            }
          }
        }
      }

      return (maxKey, minKey);
    }

    private static void UpdateExtrema<T, Q>(T item, int assemblyId, int permutation, double position,
     ref DriftResultVector<double> maxValue, ref DriftResultVector<double> minValue,
     ref DriftResultVector<Entity1dExtremaKey> maxKey, ref DriftResultVector<Entity1dExtremaKey> minKey)
     where T : IDrift<Q> where Q : IQuantity {

      if (item.X.Value > maxValue.X) {
        maxValue.X = item.X.Value;
        maxKey.X = new Entity1dExtremaKey(assemblyId, position, permutation);
      }

      if (item.Y.Value > maxValue.Y) {
        maxValue.Y = item.Y.Value;
        maxKey.Y = new Entity1dExtremaKey(assemblyId, position, permutation);
      }

      if (item.Xy.Value > maxValue.Xy) {
        maxValue.Xy = item.Xy.Value;
        maxKey.Xy = new Entity1dExtremaKey(assemblyId, position, permutation);
      }

      if (item.X.Value < minValue.X) {
        minValue.X = item.X.Value;
        minKey.X = new Entity1dExtremaKey(assemblyId, position, permutation);
      }

      if (item.Y.Value < minValue.Y) {
        minValue.Y = item.Y.Value;
        minKey.Y = new Entity1dExtremaKey(assemblyId, position, permutation);
      }

      if (item.Xy.Value < minValue.Xy) {
        minValue.Xy = item.Xy.Value;
        minKey.Xy = new Entity1dExtremaKey(assemblyId, position, permutation);
      }
    }

    private static void UpdateExtrema(IDrift<double> item, int assemblyId, int permutation, double position,
      ref DriftResultVector<double> maxValue, ref DriftResultVector<double> minValue,
      ref DriftResultVector<Entity1dExtremaKey> maxKey, ref DriftResultVector<Entity1dExtremaKey> minKey) {

      if (item.X > maxValue.X) {
        maxValue.X = item.X;
        maxKey.X = new Entity1dExtremaKey(assemblyId, position, permutation);
      }

      if (item.Y > maxValue.Y) {
        maxValue.Y = item.Y;
        maxKey.Y = new Entity1dExtremaKey(assemblyId, position, permutation);
      }

      if (item.Xy > maxValue.Xy) {
        maxValue.Xy = item.Xy;
        maxKey.Xy = new Entity1dExtremaKey(assemblyId, position, permutation);
      }

      if (item.X < minValue.X) {
        minValue.X = item.X;
        minKey.X = new Entity1dExtremaKey(assemblyId, position, permutation);
      }

      if (item.Y < minValue.Y) {
        minValue.Y = item.Y;
        minKey.Y = new Entity1dExtremaKey(assemblyId, position, permutation);
      }

      if (item.Xy < minValue.Xy) {
        minValue.Xy = item.Xy;
        minKey.Xy = new Entity1dExtremaKey(assemblyId, position, permutation);
      }
    }
  }
}
