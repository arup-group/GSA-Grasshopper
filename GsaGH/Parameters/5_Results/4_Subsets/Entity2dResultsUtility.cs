using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using GsaGH.Helpers;

using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Parameters.Results {
  internal static partial class ResultsUtility {
    internal static ConcurrentDictionary<int, IList<IQuantity>> GetResultComponent<T>(
      IDictionary<int, IList<IMeshQuantity<T>>> subset, Func<T, IQuantity> selector,
      List<int> permutations, EnvelopeMethod envelopeType)
      where T : IResultItem {
      var vals = new ConcurrentDictionary<int, IList<IQuantity>>();
      if (permutations.IsNullOrEmpty()) {
        permutations = new List<int>() {
          1
        };
      }

      if (permutations.Count == 1) {
        Parallel.ForEach(subset, kvp => {
          int idx = permutations[0] - 1;
          if (idx >= 0 && idx < kvp.Value.Count && kvp.Value[idx] != null) {
            vals.TryAdd(kvp.Key, kvp.Value[idx].Results().Select(selector).ToList());
          }
        });
        return vals;
      }

      Parallel.ForEach(subset, kvp => {
        IList<IQuantity> values = kvp.Value[permutations[0] - 1].Results().Select(selector).ToList();
        switch (envelopeType) {
          case EnvelopeMethod.Maximum:
            foreach (int permutation in permutations.Skip(1)) {
              var results = kvp.Value[permutation - 1].Results().Select(selector).ToList();
              for (int vertex = 0; vertex < values.Count; vertex++) {
                if (results[vertex].Value > values[vertex].Value) {
                  values[vertex] = results[vertex];
                }
              }
            }
            break;

          case EnvelopeMethod.Minimum:
            foreach (int permutation in permutations.Skip(1)) {
              var results = kvp.Value[permutation - 1].Results().Select(selector).ToList();
              for (int vertex = 0; vertex < values.Count; vertex++) {
                if (results[vertex].Value < values[vertex].Value) {
                  values[vertex] = results[vertex];
                }
              }
            }
            break;

          case EnvelopeMethod.Absolute:
            values = values.Select(x => x.Abs()).ToList();
            foreach (int permutation in permutations.Skip(1)) {
              var results = kvp.Value[permutation - 1].Results().Select(selector).ToList();
              for (int vertex = 0; vertex < values.Count; vertex++) {
                if (Math.Abs(results[vertex].Value) > values[vertex].Value) {
                  values[vertex] = results[vertex].Abs();
                }
              }
            }
            break;

          case EnvelopeMethod.SignedAbsolute:
            foreach (int permutation in permutations.Skip(1)) {
              var results = kvp.Value[permutation - 1].Results().Select(selector).ToList();
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
      IDictionary<int, IList<IMeshQuantity<T1>>> subset, LengthUnit unit, List<int> permutations,
      EnvelopeMethod envelopeType)
      where T1 : IResultVector3InAxis<Length>, IResultItem {
      var vals = new ConcurrentDictionary<int, (IList<double> x, IList<double> y, IList<double> z)>();
      if (permutations.IsNullOrEmpty()) {
        permutations = new List<int>() {
          1
        };
      }

      Parallel.ForEach(subset, kvp => {
        (IList<double> x, IList<double> y, IList<double> z, IList<double> xyz) = (
          kvp.Value[permutations[0] - 1].Results().Select(r => r.X.As(unit)).ToList(),
          kvp.Value[permutations[0] - 1].Results().Select(r => r.Y.As(unit)).ToList(),
          kvp.Value[permutations[0] - 1].Results().Select(r => r.Z.As(unit)).ToList(),
          kvp.Value[permutations[0] - 1].Results().Select(r => r.Xyz.As(unit)).ToList()
        );
        foreach (int permutation in permutations.Skip(1)) {
          var res = kvp.Value[permutation - 1].Results().Select(r => r.Xyz.As(unit)).ToList();
          for (int vertex = 0; vertex < xyz.Count; vertex++) {
            switch (envelopeType) {
              case EnvelopeMethod.Maximum:
              case EnvelopeMethod.SignedAbsolute:
              case EnvelopeMethod.Absolute:
                if (res[vertex] > xyz[vertex]) {
                  (x[vertex], y[vertex], z[vertex]) =
                    GetXyz(kvp.Value[permutation - 1].Results(), unit, vertex);
                  xyz[vertex] = res[vertex];
                } else if (res[vertex] == xyz[vertex]) {
                  (double xt, double yt, double zt) =
                    GetXyz(kvp.Value[permutation - 1].Results(), unit, vertex);
                  if (xt + yt + zt > x[vertex] + y[vertex] + z[vertex]) {
                    (x[vertex], y[vertex], z[vertex]) = (xt, yt, zt);
                    xyz[vertex] = res[vertex];
                  }
                }

                break;

              case EnvelopeMethod.Minimum:
                if (res[vertex] < xyz[vertex]) {
                  (x[vertex], y[vertex], z[vertex]) =
                    GetXyz(kvp.Value[permutation - 1].Results(), unit, vertex);
                  xyz[vertex] = res[vertex];
                } else if (res[vertex] == xyz[vertex]) {
                  (double xt, double yt, double zt) =
                    GetXyz(kvp.Value[permutation - 1].Results(), unit, vertex);
                  if (xt + yt + zt < x[vertex] + y[vertex] + z[vertex]) {
                    (x[vertex], y[vertex], z[vertex]) = (xt, yt, zt);
                    xyz[vertex] = res[vertex];
                  }
                }

                break;
            }
          }
        }
        vals.TryAdd(kvp.Key, (x, y, z));
      });
      return vals;
    }
  }
}
