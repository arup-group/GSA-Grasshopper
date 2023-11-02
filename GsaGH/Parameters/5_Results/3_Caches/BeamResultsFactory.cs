using GsaAPI;
using System.Collections.ObjectModel;

namespace GsaGH.Parameters.Results {
  public static class BeamResultsFactory {
    public static Collection<IBeamDisplacement> CreateBeamDisplacements(
      ReadOnlyCollection<Double6> results, ReadOnlyCollection<double> positions) {
      return new Collection<IBeamDisplacement> {
              new BeamDisplacement(results, positions)
             };
    }

    public static Collection<IBeamDisplacement> CreateBeamDisplacements(
      ReadOnlyCollection<ReadOnlyCollection<Double6>> results,  ReadOnlyCollection<double> positions) {
      var permutations = new Collection<IBeamDisplacement>();
      foreach (ReadOnlyCollection<Double6> permutation in results) {
        permutations.Add(new BeamDisplacement(permutation, positions));
      }
      return permutations;
    }

    public static Collection<IBeamDisplacement> AddMissingPositions(
      this Collection<IBeamDisplacement> existing,
      ReadOnlyCollection<Double6> results, ReadOnlyCollection<double> positions) {
      for (int i = 0; i < results.Count; i++) {
        if (!existing[0].Displacements.ContainsKey(positions[i])) {
          existing[0].Displacements.Add(positions[i], new Displacement(results[i]));
        }
      }

      return existing;
    }

    public static Collection<IBeamDisplacement> AddMissingPositions(
      this Collection<IBeamDisplacement> existing,
      ReadOnlyCollection<ReadOnlyCollection<Double6>> results, ReadOnlyCollection<double> positions) {
      for (int i = 0; i < existing.Count; i++) {
        for (int j = 0; j < results.Count; j++) {
          if (!existing[i].Displacements.ContainsKey(positions[j])) {
            existing[i].Displacements.Add(positions[j], new Displacement(results[i][j]));
          }
        }
      }

      return existing;
    }
  }
}
