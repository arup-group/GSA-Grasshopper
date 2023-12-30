﻿using OasysUnits;
using OasysUnits.Units;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GsaGH.Parameters.Results {
  internal static partial class ResultsUtility {
    internal static ConcurrentDictionary<int, IList<IQuantity>> GetResultComponent<T1, T2>(
      IDictionary<int, IList<T1>> subset, Func<T2, IQuantity> selector, EnvelopeMethod envelopeType)
      where T1 : IEntity1dQuantity<T2> where T2 : IResultItem {
      var vals = new ConcurrentDictionary<int, IList<IQuantity>>();
      if (subset.Values.FirstOrDefault().Count < 2) {
        Parallel.ForEach(subset, kvp =>
          vals.TryAdd(kvp.Key, kvp.Value[0].Results.Values.Select(selector).ToList()));
        return vals;
      }

      Parallel.ForEach(subset, kvp => {
        IList<IQuantity> values = kvp.Value[0].Results.Values.Select(selector).ToList();
        switch (envelopeType) {
          case EnvelopeMethod.Maximum:
            for (int permutation = 1; permutation < kvp.Value.Count; permutation++) {
              var results = kvp.Value[permutation].Results.Values.Select(selector).ToList();
              for (int position = 0; position < values.Count; position++) {
                if (results[position].Value > values[position].Value) {
                  values[position] = results[position];
                }
              }
            }
            break;

          case EnvelopeMethod.Minimum:
            for (int permutation = 1; permutation < kvp.Value.Count; permutation++) {
              var results = kvp.Value[permutation].Results.Values.Select(selector).ToList();
              for (int position = 0; position < values.Count; position++) {
                if (results[position].Value < values[position].Value) {
                  values[position] = results[position];
                }
              }
            }
            break;

          case EnvelopeMethod.Absolute:
            values = values.Select(x => x.Abs()).ToList();
            for (int permutation = 1; permutation < kvp.Value.Count; permutation++) {
              var results = kvp.Value[permutation].Results.Values.Select(selector).ToList();
              for (int position = 0; position < values.Count; position++) {
                if (Math.Abs(results[position].Value) > values[position].Value) {
                  values[position] = results[position].Abs();
                }
              }
            }
            break;

          case EnvelopeMethod.SignedAbsolute:
            for (int permutation = 1; permutation < kvp.Value.Count; permutation++) {
              var results = kvp.Value[permutation].Results.Values.Select(selector).ToList();
              for (int position = 0; position < values.Count; position++) {
                if (Math.Abs(results[position].Value) > Math.Abs(values[position].Value)) {
                  values[position] = results[position];
                }
              }
            }
            break;
        }

        vals.TryAdd(kvp.Key, values);
      });
      return vals;
    }

    internal static ConcurrentDictionary<int, (IList<double> x, IList<double> y, IList<double> z)> GetResultResultantTranslation(
      IDictionary<int, IList<IEntity1dDisplacement>> subset, LengthUnit unit, EnvelopeMethod envelopeType) {
      var vals = new ConcurrentDictionary<int, (IList<double> x, IList<double> y, IList<double> z)>();
      Parallel.ForEach(subset, kvp => {
        (IList<double> x, IList<double> y, IList<double> z, IList<double> xyz) = (
          kvp.Value[0].Results.Values.Select(r => r.X.As(unit)).ToList(),
          kvp.Value[0].Results.Values.Select(r => r.Y.As(unit)).ToList(),
          kvp.Value[0].Results.Values.Select(r => r.Z.As(unit)).ToList(),
          kvp.Value[0].Results.Values.Select(r => r.Xyz.As(unit)).ToList()
        );
        for (int permutation = 1; permutation < kvp.Value.Count; permutation++) {
          var res = kvp.Value[permutation].Results.Values.Select(r => r.Xyz.As(unit)).ToList();
          for (int positionId = 0; positionId < xyz.Count; positionId++) {
            switch (envelopeType) {
              case EnvelopeMethod.Maximum:
              case EnvelopeMethod.SignedAbsolute:
              case EnvelopeMethod.Absolute:
                if (res[positionId] > xyz[positionId]) {
                  (x[positionId], y[positionId], z[positionId]) =
                    GetXyz(kvp.Value[permutation].Results.Values, unit, positionId);
                  xyz[positionId] = res[positionId];
                }

                break;

              case EnvelopeMethod.Minimum:
                if (res[positionId] < xyz[positionId]) {
                  (x[positionId], y[positionId], z[positionId]) =
                    GetXyz(kvp.Value[permutation].Results.Values, unit, positionId);
                  xyz[positionId] = res[positionId];
                }

                break;
            }
          }
        }
        vals.TryAdd(kvp.Key, (x, y, z));
      });
      return vals;
    }

    private static (double x, double y, double z) GetXyz(ICollection<IDisplacement> values, LengthUnit unit, int id) {
      return (values.Select(r => r.X.As(unit)).ElementAt(id),
              values.Select(r => r.Y.As(unit)).ElementAt(id),
              values.Select(r => r.Z.As(unit)).ElementAt(id));
    }
  }
}
