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
    internal static ConcurrentDictionary<int, IList<IQuantity>> GetResultComponent<T1, T2>(
      IDictionary<int, IList<T1>> subset, Func<T2, IQuantity> selector, List<int> permutations,
      EnvelopeMethod envelopeType)
      where T1 : IEntity1dQuantity<T2> where T2 : IResultItem {
      var vals = new ConcurrentDictionary<int, IList<IQuantity>>();
      if (permutations.IsNullOrEmpty()) {
        permutations = new List<int>() {
          1
        };
      }

      if (permutations.Count == 1) {
        Parallel.ForEach(subset, kvp =>
          vals.TryAdd(kvp.Key, kvp.Value[permutations[0] - 1].Results.Values.Select(selector).ToList()));
        return vals;
      }

      Parallel.ForEach(subset, kvp => {
        IList<IQuantity> values = kvp.Value[permutations[0] - 1].Results.Values.Select(selector).ToList();
        switch (envelopeType) {
          case EnvelopeMethod.Maximum:
            foreach (int permutation in permutations.Skip(1)) {
              var results = kvp.Value[permutation - 1].Results.Values.Select(selector).ToList();
              for (int position = 0; position < values.Count; position++) {
                if (results[position].Value > values[position].Value) {
                  values[position] = results[position];
                }
              }
            }
            break;

          case EnvelopeMethod.Minimum:
            foreach (int permutation in permutations.Skip(1)) {
              var results = kvp.Value[permutation - 1].Results.Values.Select(selector).ToList();
              for (int position = 0; position < values.Count; position++) {
                if (results[position].Value < values[position].Value) {
                  values[position] = results[position];
                }
              }
            }
            break;

          case EnvelopeMethod.Absolute:
            values = values.Select(x => x.Abs()).ToList();
            foreach (int permutation in permutations.Skip(1)) {
              var results = kvp.Value[permutation - 1].Results.Values.Select(selector).ToList();
              for (int position = 0; position < values.Count; position++) {
                if (Math.Abs(results[position].Value) > values[position].Value) {
                  values[position] = results[position].Abs();
                }
              }
            }
            break;

          case EnvelopeMethod.SignedAbsolute:
            foreach (int permutation in permutations.Skip(1)) {
              var results = kvp.Value[permutation - 1].Results.Values.Select(selector).ToList();
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
      IDictionary<int, IList<IEntity1dQuantity<IDisplacement>>> subset, LengthUnit unit, List<int> permutations,
      EnvelopeMethod envelopeType) {
      var vals = new ConcurrentDictionary<int, (IList<double> x, IList<double> y, IList<double> z)>();
      if (permutations.IsNullOrEmpty()) {
        permutations = new List<int>() {
          1
        };
      }

      Parallel.ForEach(subset, kvp => {
        (IList<double> x, IList<double> y, IList<double> z, IList<double> xyz) = (
          kvp.Value[permutations[0] - 1].Results.Values.Select(r => r.X.As(unit)).ToList(),
          kvp.Value[permutations[0] - 1].Results.Values.Select(r => r.Y.As(unit)).ToList(),
          kvp.Value[permutations[0] - 1].Results.Values.Select(r => r.Z.As(unit)).ToList(),
          kvp.Value[permutations[0] - 1].Results.Values.Select(r => r.Xyz.As(unit)).ToList()
        );
        foreach (int permutation in permutations.Skip(1)) {
          var res = kvp.Value[permutation - 1].Results.Values.Select(r => r.Xyz.As(unit)).ToList();
          for (int position = 0; position < xyz.Count; position++) {
            switch (envelopeType) {
              case EnvelopeMethod.Maximum:
              case EnvelopeMethod.SignedAbsolute:
              case EnvelopeMethod.Absolute:
                if (res[position] > xyz[position]) {
                  (x[position], y[position], z[position]) =
                    GetXyz(kvp.Value[permutation - 1].Results.Values, unit, position);
                  xyz[position] = res[position];
                } else if (res[position] == xyz[position]) {
                  (double xt, double yt, double zt) =
                    GetXyz(kvp.Value[permutation - 1].Results.Values, unit, position);
                  if (xt + yt + zt > x[position] + y[position] + z[position]) {
                    (x[position], y[position], z[position]) = (xt, yt, zt);
                    xyz[position] = res[position];
                  }
                }

                break;

              case EnvelopeMethod.Minimum:
                if (res[position] < xyz[position]) {
                  (x[position], y[position], z[position]) =
                    GetXyz(kvp.Value[permutation - 1].Results.Values, unit, position);
                  xyz[position] = res[position];
                } else if (res[position] == xyz[position]) {
                  (double xt, double yt, double zt) =
                    GetXyz(kvp.Value[permutation - 1].Results.Values, unit, position);
                  if (xt + yt + zt < x[position] + y[position] + z[position]) {
                    (x[position], y[position], z[position]) = (xt, yt, zt);
                    xyz[position] = res[position];
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

    private static (double x, double y, double z) GetXyz(ICollection<IDisplacement> values, LengthUnit unit, int id) {
      return (values.Select(r => r.X.As(unit)).ElementAt(id),
              values.Select(r => r.Y.As(unit)).ElementAt(id),
              values.Select(r => r.Z.As(unit)).ElementAt(id));
    }
  }
}
