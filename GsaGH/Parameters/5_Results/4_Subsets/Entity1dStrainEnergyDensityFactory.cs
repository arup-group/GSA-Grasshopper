using GsaAPI;
using System.Collections.ObjectModel;

namespace GsaGH.Parameters.Results {
  public static partial class Entity1dResultsFactory {
    public static Collection<IEntity1dStrainEnergyDensity> CreateStrainEnergyDensities(
      ReadOnlyCollection<double> results, ReadOnlyCollection<double> positions) {
      return new Collection<IEntity1dStrainEnergyDensity> {
              new Entity1dStrainEnergyDensity(results, positions)
             };
    }

    public static Collection<IEntity1dStrainEnergyDensity> CreateStrainEnergyDensities(
      ReadOnlyCollection<ReadOnlyCollection<double>> results, ReadOnlyCollection<double> positions) {
      var permutations = new Collection<IEntity1dStrainEnergyDensity>();
      foreach (ReadOnlyCollection<double> permutation in results) {
        permutations.Add(new Entity1dStrainEnergyDensity(permutation, positions));
      }
      return permutations;
    }

    public static Collection<IEntity1dStrainEnergyDensity> AddMissingPositions(
      this Collection<IEntity1dStrainEnergyDensity> existing,
      ReadOnlyCollection<double> results, ReadOnlyCollection<double> positions) {
      for (int i = 0; i < results.Count; i++) {
        if (!existing[0].Results.ContainsKey(positions[i])) {
          existing[0].Results.Add(positions[i], new StrainEnergyDensity(results[i]));
        }
      }

      return existing;
    }

    public static Collection<IEntity1dStrainEnergyDensity> AddMissingPositions(
      this Collection<IEntity1dStrainEnergyDensity> existing,
      ReadOnlyCollection<ReadOnlyCollection<double>> results, ReadOnlyCollection<double> positions) {
      for (int i = 0; i < existing.Count; i++) {
        for (int j = 0; j < results.Count; j++) {
          if (!existing[i].Results.ContainsKey(positions[j])) {
            existing[i].Results.Add(positions[j], new StrainEnergyDensity(results[i][j]));
          }
        }
      }

      return existing;
    }
  }
}
