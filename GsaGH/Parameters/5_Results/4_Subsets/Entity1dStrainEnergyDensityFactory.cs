using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GsaGH.Parameters.Results {
  internal static partial class Entity1dResultsFactory {
    internal static IList<IEntity1dStrainEnergyDensity> CreateStrainEnergyDensities(
      ReadOnlyCollection<double> results, ReadOnlyCollection<double> positions) {
      return new List<IEntity1dStrainEnergyDensity> {
              new Entity1dStrainEnergyDensity(results, positions)
             };
    }

    internal static IList<IEntity1dStrainEnergyDensity> CreateStrainEnergyDensities(
      ReadOnlyCollection<ReadOnlyCollection<double>> results, ReadOnlyCollection<double> positions) {
      var permutations = new List<IEntity1dStrainEnergyDensity>();
      foreach (ReadOnlyCollection<double> permutation in results) {
        permutations.Add(new Entity1dStrainEnergyDensity(permutation, positions));
      }
      return permutations;
    }

    internal static IList<IEntity1dStrainEnergyDensity> AddMissingPositions(
      this IList<IEntity1dStrainEnergyDensity> existing,
      ReadOnlyCollection<double> results, ReadOnlyCollection<double> positions) {
      for (int i = 0; i < positions.Count; i++) {
        if (!existing[0].Results.ContainsKey(positions[i])) {
          existing[0].Results.Add(positions[i], new StrainEnergyDensity(results[i]));
        }
      }

      return existing;
    }

    internal static IList<IEntity1dStrainEnergyDensity> AddMissingPositions(
      this IList<IEntity1dStrainEnergyDensity> existing,
      ReadOnlyCollection<ReadOnlyCollection<double>> results, ReadOnlyCollection<double> positions) {
      for (int i = 0; i < existing.Count; i++) {
        for (int j = 0; j < positions.Count; j++) {
          if (!existing[i].Results.ContainsKey(positions[j])) {
            existing[i].Results.Add(positions[j], new StrainEnergyDensity(results[i][j]));
          }
        }
      }

      return existing;
    }
  }
}
