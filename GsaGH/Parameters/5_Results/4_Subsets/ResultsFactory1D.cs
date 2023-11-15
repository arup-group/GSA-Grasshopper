using System.Collections.ObjectModel;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public static class ResultsFactory1D {
    public static Collection<IDisplacement1D> CreateBeamDisplacements(
      ReadOnlyCollection<Double6> results, ReadOnlyCollection<double> positions) {
      return new Collection<IDisplacement1D> {
        new Displacement1D(results, positions),
      };
    }

    public static Collection<IDisplacement1D> CreateBeamDisplacements(
      ReadOnlyCollection<ReadOnlyCollection<Double6>> results,
      ReadOnlyCollection<double> positions) {
      var permutations = new Collection<IDisplacement1D>();
      foreach (ReadOnlyCollection<Double6> permutation in results) {
        permutations.Add(new Displacement1D(permutation, positions));
      }

      return permutations;
    }

    public static Collection<IInternalForce1D> CreateBeamForces(
      ReadOnlyCollection<Double6> results, ReadOnlyCollection<double> positions) {
      return new Collection<IInternalForce1D> {
        new InternalForce1D(results, positions),
      };
    }

    public static Collection<IInternalForce1D> CreateBeamForces(
      ReadOnlyCollection<ReadOnlyCollection<Double6>> results,
      ReadOnlyCollection<double> positions) {
      var permutations = new Collection<IInternalForce1D>();
      foreach (ReadOnlyCollection<Double6> permutation in results) {
        permutations.Add(new InternalForce1D(permutation, positions));
      }

      return permutations;
    }

    public static Collection<IInternalForce1D> AddMissingPositions(
      this Collection<IInternalForce1D> existing, ReadOnlyCollection<Double6> results,
      ReadOnlyCollection<double> positions) {
      for (int i = 0; i < results.Count; i++) {
        if (!existing[0].Results.ContainsKey(positions[i])) {
          existing[0].Results.Add(positions[i], new ReactionForce(results[i]));
        }
      }

      return existing;
    }

    public static Collection<IInternalForce1D> AddMissingPositions(
      this Collection<IInternalForce1D> existing,
      ReadOnlyCollection<ReadOnlyCollection<Double6>> results,
      ReadOnlyCollection<double> positions) {
      for (int i = 0; i < existing.Count; i++) {
        for (int j = 0; j < results.Count; j++) {
          if (!existing[i].Results.ContainsKey(positions[j])) {
            existing[i].Results.Add(positions[j], new ReactionForce(results[i][j]));
          }
        }
      }

      return existing;
    }

    public static Collection<IDisplacement1D> AddMissingPositions(
      this Collection<IDisplacement1D> existing, ReadOnlyCollection<Double6> results,
      ReadOnlyCollection<double> positions) {
      for (int i = 0; i < results.Count; i++) {
        if (!existing[0].Results.ContainsKey(positions[i])) {
          existing[0].Results.Add(positions[i], new Displacement(results[i]));
        }
      }

      return existing;
    }

    public static Collection<IDisplacement1D> AddMissingPositions(
      this Collection<IDisplacement1D> existing,
      ReadOnlyCollection<ReadOnlyCollection<Double6>> results,
      ReadOnlyCollection<double> positions) {
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
