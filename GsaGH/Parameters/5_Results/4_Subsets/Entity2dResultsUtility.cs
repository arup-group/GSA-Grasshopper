using OasysUnits;
using OasysUnits.Units;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GsaGH.Parameters.Results {
  internal static partial class ResultsUtility {
    internal static ConcurrentDictionary<int, IList<IQuantity>> GetResultComponent<T>(
      IDictionary<int, IList<IMeshQuantity<T>>> subset, Func<T, IQuantity> selector, EnvelopeMethod envelopeType)
      where T : IResultItem {
      var vals = new ConcurrentDictionary<int, IList<IQuantity>>();
      if (subset.Values.FirstOrDefault().Count < 2) {
        Parallel.ForEach(subset, kvp =>
        vals.TryAdd(kvp.Key, kvp.Value[0].Results().Select(selector).ToList()));
        return vals;
      }

      Parallel.ForEach(subset, kvp => {
        IList<IQuantity> values = kvp.Value[0].Results().Select(selector).ToList();
        switch (envelopeType) {
          case EnvelopeMethod.Maximum:
            for (int permutation = 1; permutation < kvp.Value.Count; permutation++) {
              var results = kvp.Value[permutation].Results().Select(selector).ToList();
              for (int vertex = 0; vertex < values.Count; vertex++) {
                if (results[vertex].Value > values[vertex].Value) {
                  values[vertex] = results[vertex];
                }
              }
            }
            break;

          case EnvelopeMethod.Minimum:
            for (int permutation = 1; permutation < kvp.Value.Count; permutation++) {
              var results = kvp.Value[permutation].Results().Select(selector).ToList();
              for (int vertex = 0; vertex < values.Count; vertex++) {
                if (results[vertex].Value < values[vertex].Value) {
                  values[vertex] = results[vertex];
                }
              }
            }
            break;

          case EnvelopeMethod.Absolute:
            values = values.Select(x => x.Abs()).ToList();
            for (int permutation = 1; permutation < kvp.Value.Count; permutation++) {
              var results = kvp.Value[permutation].Results().Select(selector).ToList();
              for (int vertex = 0; vertex < values.Count; vertex++) {
                if (Math.Abs(results[vertex].Value) > values[vertex].Value) {
                  values[vertex] = results[vertex].Abs();
                }
              }
            }
            break;

          case EnvelopeMethod.SignedAbsolute:
            for (int permutation = 1; permutation < kvp.Value.Count; permutation++) {
              var results = kvp.Value[permutation].Results().Select(selector).ToList();
              for (int vertex = 0; vertex < values.Count; vertex++) {
                if (Math.Abs(results[vertex].Value) > Math.Abs(values[vertex].Value)) {
                  values[vertex] = results[vertex];
                }
              }
            }
            break;
        }

        vals.TryAdd(kvp.Key, values);
      });
      return vals;
    }

    internal static ConcurrentDictionary<int, (IList<double> x, IList<double> y, IList<double> z)> GetResultResultantTranslation<T1>(
      IDictionary<int, IList<IMeshQuantity<T1>>> subset, LengthUnit unit, EnvelopeMethod envelopeType)
      where T1 : IResultVector3InAxis<Length>, IResultItem {
      var vals = new ConcurrentDictionary<int, (IList<double> x, IList<double> y, IList<double> z)>();
      Parallel.ForEach(subset, kvp => {
        (IList<double> x, IList<double> y, IList<double> z, IList<double> xyz) = (
          kvp.Value[0].Results().Select(r => r.X.As(unit)).ToList(),
          kvp.Value[0].Results().Select(r => r.Y.As(unit)).ToList(),
          kvp.Value[0].Results().Select(r => r.Z.As(unit)).ToList(),
          kvp.Value[0].Results().Select(r => r.Xyz.As(unit)).ToList()
        );
        for (int permutation = 1; permutation < kvp.Value.Count; permutation++) {
          var res = kvp.Value[permutation].Results().Select(r => r.Xyz.As(unit)).ToList();
          for (int positionId = 0; positionId < xyz.Count; positionId++) {
            switch (envelopeType) {
              case EnvelopeMethod.Maximum:
              case EnvelopeMethod.SignedAbsolute:
              case EnvelopeMethod.Absolute:
                if (res[positionId] > xyz[positionId]) {
                  (x[positionId], y[positionId], z[positionId]) =
                    GetXyz(kvp.Value[permutation].Results(), unit, positionId);
                  xyz[positionId] = res[positionId];
                }

                break;

              case EnvelopeMethod.Minimum:
                if (res[positionId] < xyz[positionId]) {
                  (x[positionId], y[positionId], z[positionId]) =
                    GetXyz(kvp.Value[permutation].Results(), unit, positionId);
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

    private static (double x, double y, double z) GetXyz<T>(IList<T> values, LengthUnit unit, int id)
      where T : IResultVector3InAxis<Length> {
      return (values.Select(r => r.X.As(unit)).ElementAt(id),
              values.Select(r => r.Y.As(unit)).ElementAt(id),
              values.Select(r => r.Z.As(unit)).ElementAt(id));
    }
  }
}
