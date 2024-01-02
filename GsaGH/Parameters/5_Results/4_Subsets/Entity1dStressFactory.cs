using GsaAPI;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GsaGH.Parameters.Results {
  internal static partial class Entity1dResultsFactory {
    internal static IList<IEntity1dStress> CreateStresses(
      ReadOnlyCollection<StressResult1d> results, ReadOnlyCollection<double> positions) {
      return new List<IEntity1dStress> {
              new Entity1dStress(results, positions)
             };
    }

    internal static IList<IEntity1dStress> CreateStresses(
      ReadOnlyCollection<ReadOnlyCollection<StressResult1d>> results, ReadOnlyCollection<double> positions) {
      var permutations = new List<IEntity1dStress>();
      foreach (ReadOnlyCollection<StressResult1d> permutation in results) {
        permutations.Add(new Entity1dStress(permutation, positions));
      }
      return permutations;
    }

    internal static IList<IEntity1dStress> AddMissingPositions(
      this IList<IEntity1dStress> existing,
      ReadOnlyCollection<StressResult1d> results, ReadOnlyCollection<double> positions) {
      for (int i = 0; i < results.Count; i++) {
        if (!existing[0].Results.ContainsKey(positions[i])) {
          existing[0].Results.Add(positions[i], new Stress1d(results[i]));
        }
      }

      return existing;
    }

    internal static IList<IEntity1dStress> AddMissingPositions(
      this IList<IEntity1dStress> existing,
      ReadOnlyCollection<ReadOnlyCollection<StressResult1d>> results, ReadOnlyCollection<double> positions) {
      for (int i = 0; i < existing.Count; i++) {
        for (int j = 0; j < positions.Count; j++) {
          if (!existing[i].Results.ContainsKey(positions[j])) {
            existing[i].Results.Add(positions[j], new Stress1d(results[i][j]));
          }
        }
      }

      return existing;
    }
  }
}
