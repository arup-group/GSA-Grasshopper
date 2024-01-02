using GsaAPI;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GsaGH.Parameters.Results {
  internal static partial class Entity1dResultsFactory {
    internal static IList<IEntity1dDisplacement> CreateDisplacements(
      ReadOnlyCollection<Double6> results, ReadOnlyCollection<double> positions) {
      return new List<IEntity1dDisplacement> {
              new Entity1dDisplacement(results, positions)
             };
    }

    internal static IList<IEntity1dDisplacement> CreateDisplacements(
      ReadOnlyCollection<ReadOnlyCollection<Double6>> results, ReadOnlyCollection<double> positions) {
      var permutations = new List<IEntity1dDisplacement>();
      foreach (ReadOnlyCollection<Double6> permutation in results) {
        permutations.Add(new Entity1dDisplacement(permutation, positions));
      }
      return permutations;
    }

    internal static IList<IEntity1dDisplacement> AddMissingPositions(
      this IList<IEntity1dDisplacement> existing,
      ReadOnlyCollection<Double6> results, ReadOnlyCollection<double> positions) {
      for (int i = 0; i < positions.Count; i++) {
        if (!existing[0].Results.ContainsKey(positions[i])) {
          existing[0].Results.Add(positions[i], new Displacement(results[i]));
        }
      }

      return existing;
    }

    internal static IList<IEntity1dDisplacement> AddMissingPositions(
      this IList<IEntity1dDisplacement> existing,
      ReadOnlyCollection<ReadOnlyCollection<Double6>> results, ReadOnlyCollection<double> positions) {
      for (int i = 0; i < existing.Count; i++) {
        for (int j = 0; j < positions.Count; j++) {
          if (!existing[i].Results.ContainsKey(positions[j])) {
            existing[i].Results.Add(positions[j], new Displacement(results[i][j]));
          }
        }
      }

      return existing;
    }
  }
}
