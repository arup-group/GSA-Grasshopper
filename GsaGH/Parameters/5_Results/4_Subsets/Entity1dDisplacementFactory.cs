using GsaAPI;
using System.Collections.ObjectModel;

namespace GsaGH.Parameters.Results {
  public static partial class Entity1dResultsFactory {
    public static Collection<IEntity1dDisplacement> CreateDisplacements(
      ReadOnlyCollection<Double6> results, ReadOnlyCollection<double> positions) {
      return new Collection<IEntity1dDisplacement> {
              new Entity1dDisplacement(results, positions)
             };
    }

    public static Collection<IEntity1dDisplacement> CreateDisplacements(
      ReadOnlyCollection<ReadOnlyCollection<Double6>> results,  ReadOnlyCollection<double> positions) {
      var permutations = new Collection<IEntity1dDisplacement>();
      foreach (ReadOnlyCollection<Double6> permutation in results) {
        permutations.Add(new Entity1dDisplacement(permutation, positions));
      }
      return permutations;
    }

    public static Collection<IEntity1dDisplacement> AddMissingPositions(
      this Collection<IEntity1dDisplacement> existing,
      ReadOnlyCollection<Double6> results, ReadOnlyCollection<double> positions) {
      for (int i = 0; i < results.Count; i++) {
        if (!existing[0].Results.ContainsKey(positions[i])) {
          existing[0].Results.Add(positions[i], new Displacement(results[i]));
        }
      }

      return existing;
    }

    public static Collection<IEntity1dDisplacement> AddMissingPositions(
      this Collection<IEntity1dDisplacement> existing,
      ReadOnlyCollection<ReadOnlyCollection<Double6>> results, ReadOnlyCollection<double> positions) {
      for (int i = 0; i < existing.Count; i++) {
        for (int j = 0; j < results.Count; j++) {
          if (!existing[i].Results.ContainsKey(positions[j])) {
            existing[i].Results.Add(positions[j], new Displacement(results[i][j]));
          }
        }
      }

      return existing;
    }
  }
}
