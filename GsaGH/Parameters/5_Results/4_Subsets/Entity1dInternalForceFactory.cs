using GsaAPI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GsaGH.Parameters.Results {
  internal static partial class Entity1dResultsFactory {
    internal static IList<IEntity1dInternalForce> CreateForces(
      ReadOnlyCollection<Double6> results, ReadOnlyCollection<double> positions) {
      return new List<IEntity1dInternalForce> {
              new Entity1dInternalForce(results, positions)
             };
    }

    internal static IList<IEntity1dInternalForce> CreateForces(
      ReadOnlyCollection<ReadOnlyCollection<Double6>> results, ReadOnlyCollection<double> positions) {
      var permutations = new List<IEntity1dInternalForce>();
      foreach (ReadOnlyCollection<Double6> permutation in results) {
        permutations.Add(new Entity1dInternalForce(permutation, positions));
      }
      return permutations;
    }

    internal static IList<IEntity1dInternalForce> AddMissingPositions(
      this IList<IEntity1dInternalForce> existing,
      ReadOnlyCollection<Double6> results, ReadOnlyCollection<double> positions) {
      for (int i = 0; i < positions.Count; i++) {
        if (!existing[0].Results.ContainsKey(positions[i])) {
          existing[0].Results.Add(positions[i], new InternalForce(results[i]));
        }
      }

      return existing;
    }

    internal static IList<IEntity1dInternalForce> AddMissingPositions(
      this IList<IEntity1dInternalForce> existing,
      ReadOnlyCollection<ReadOnlyCollection<Double6>> results, ReadOnlyCollection<double> positions) {
      for (int i = 0; i < existing.Count; i++) {
        for (int j = 0; j < positions.Count; j++) {
          if (!existing[i].Results.ContainsKey(positions[j])) {
            existing[i].Results.Add(positions[j], new InternalForce(results[i][j]));
          }
        }
      }

      return existing;
    }
  }
}
