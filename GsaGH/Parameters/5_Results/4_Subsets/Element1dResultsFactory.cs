using GsaAPI;
using System.Collections.ObjectModel;

namespace GsaGH.Parameters.Results {
  public static class Element1dResultsFactory {
    public static Collection<IElement1dDisplacement> CreateBeamDisplacements(
      ReadOnlyCollection<Double6> results, ReadOnlyCollection<double> positions) {
      return new Collection<IElement1dDisplacement> {
              new Element1dDisplacement(results, positions)
             };
    }

    public static Collection<IElement1dDisplacement> CreateBeamDisplacements(
      ReadOnlyCollection<ReadOnlyCollection<Double6>> results,  ReadOnlyCollection<double> positions) {
      var permutations = new Collection<IElement1dDisplacement>();
      foreach (ReadOnlyCollection<Double6> permutation in results) {
        permutations.Add(new Element1dDisplacement(permutation, positions));
      }
      return permutations;
    }

    public static Collection<IElement1dInternalForce> CreateBeamForces(
      ReadOnlyCollection<Double6> results, ReadOnlyCollection<double> positions) {
      return new Collection<IElement1dInternalForce> {
              new Element1dInternalForce(results, positions)
             };
    }

    public static Collection<IElement1dInternalForce> CreateBeamForces(
      ReadOnlyCollection<ReadOnlyCollection<Double6>> results, ReadOnlyCollection<double> positions) {
      var permutations = new Collection<IElement1dInternalForce>();
      foreach (ReadOnlyCollection<Double6> permutation in results) {
        permutations.Add(new Element1dInternalForce(permutation, positions));
      }
      return permutations;
    }

    public static Collection<IElement1dInternalForce> AddMissingPositions(
      this Collection<IElement1dInternalForce> existing,
      ReadOnlyCollection<Double6> results, ReadOnlyCollection<double> positions) {
      for (int i = 0; i < results.Count; i++) {
        if (!existing[0].Results.ContainsKey(positions[i])) {
          existing[0].Results.Add(positions[i], new ReactionForce(results[i]));
        }
      }

      return existing;
    }

    public static Collection<IElement1dInternalForce> AddMissingPositions(
      this Collection<IElement1dInternalForce> existing,
      ReadOnlyCollection<ReadOnlyCollection<Double6>> results, ReadOnlyCollection<double> positions) {
      for (int i = 0; i < existing.Count; i++) {
        for (int j = 0; j < results.Count; j++) {
          if (!existing[i].Results.ContainsKey(positions[j])) {
            existing[i].Results.Add(positions[j], new ReactionForce(results[i][j]));
          }
        }
      }

      return existing;
    }

    public static Collection<IElement1dDisplacement> AddMissingPositions(
      this Collection<IElement1dDisplacement> existing,
      ReadOnlyCollection<Double6> results, ReadOnlyCollection<double> positions) {
      for (int i = 0; i < results.Count; i++) {
        if (!existing[0].Results.ContainsKey(positions[i])) {
          existing[0].Results.Add(positions[i], new Displacement(results[i]));
        }
      }

      return existing;
    }

    public static Collection<IElement1dDisplacement> AddMissingPositions(
      this Collection<IElement1dDisplacement> existing,
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
