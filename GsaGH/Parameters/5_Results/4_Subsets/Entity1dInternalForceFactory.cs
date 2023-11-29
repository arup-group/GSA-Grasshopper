using GsaAPI;
using System.Collections.ObjectModel;

namespace GsaGH.Parameters.Results {
  public static partial class Entity1dResultsFactory {
    public static Collection<IEntity1dInternalForce> CreateForces(
      ReadOnlyCollection<Double6> results, ReadOnlyCollection<double> positions) {
      return new Collection<IEntity1dInternalForce> {
              new Entity1dInternalForce(results, positions)
             };
    }

    public static Collection<IEntity1dInternalForce> CreateForces(
      ReadOnlyCollection<ReadOnlyCollection<Double6>> results, ReadOnlyCollection<double> positions) {
      var permutations = new Collection<IEntity1dInternalForce>();
      foreach (ReadOnlyCollection<Double6> permutation in results) {
        permutations.Add(new Entity1dInternalForce(permutation, positions));
      }
      return permutations;
    }

    public static Collection<IEntity1dInternalForce> AddMissingPositions(
      this Collection<IEntity1dInternalForce> existing,
      ReadOnlyCollection<Double6> results, ReadOnlyCollection<double> positions) {
      for (int i = 0; i < results.Count; i++) {
        if (!existing[0].Results.ContainsKey(positions[i])) {
          existing[0].Results.Add(positions[i], new ReactionForce(results[i]));
        }
      }

      return existing;
    }

    public static Collection<IEntity1dInternalForce> AddMissingPositions(
      this Collection<IEntity1dInternalForce> existing,
      ReadOnlyCollection<ReadOnlyCollection<Double6>> results, ReadOnlyCollection<double> positions) {
      for (int i = 0; i < existing.Count; i++) {
        for (int j = 0; j < results[i].Count; j++) {
          if (!existing[i].Results.ContainsKey(positions[j])) {
            existing[i].Results.Add(positions[j], new ReactionForce(results[i][j]));
          }
        }
      }

      return existing;
    }
  }
}
