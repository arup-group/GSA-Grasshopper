using GsaAPI;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GsaGH.Parameters.Results {
  internal static partial class Entity1dResultsFactory {
    internal static IList<IEntity1dDerivedStress> CreateDerivedStresses(
      ReadOnlyCollection<DerivedStressResult1d> results, ReadOnlyCollection<double> positions) {
      return new List<IEntity1dDerivedStress> {
              new Entity1dDerivedStress(results, positions)
             };
    }

    internal static IList<IEntity1dDerivedStress> CreateDerivedStresses(
      ReadOnlyCollection<ReadOnlyCollection<DerivedStressResult1d>> results, ReadOnlyCollection<double> positions) {
      var permutations = new List<IEntity1dDerivedStress>();
      foreach (ReadOnlyCollection<DerivedStressResult1d> permutation in results) {
        permutations.Add(new Entity1dDerivedStress(permutation, positions));
      }
      return permutations;
    }

    internal static IList<IEntity1dDerivedStress> AddMissingPositions(
      this IList<IEntity1dDerivedStress> existing,
      ReadOnlyCollection<DerivedStressResult1d> results, ReadOnlyCollection<double> positions) {
      for (int i = 0; i < results.Count; i++) {
        if (!existing[0].Results.ContainsKey(positions[i])) {
          existing[0].Results.Add(positions[i], new Stress1dDerived(results[i]));
        }
      }

      return existing;
    }

    internal static IList<IEntity1dDerivedStress> AddMissingPositions(
      this IList<IEntity1dDerivedStress> existing,
      ReadOnlyCollection<ReadOnlyCollection<DerivedStressResult1d>> results, ReadOnlyCollection<double> positions) {
      for (int i = 0; i < existing.Count; i++) {
        for (int j = 0; j < results[i].Count; j++) {
          if (!existing[i].Results.ContainsKey(positions[j])) {
            existing[i].Results.Add(positions[j], new Stress1dDerived(results[i][j]));
          }
        }
      }

      return existing;
    }
  }
}
