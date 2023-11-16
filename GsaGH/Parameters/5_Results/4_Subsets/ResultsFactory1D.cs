using System.Collections.ObjectModel;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public static class ResultsFactory1d {
    public static Collection<IDisplacement1d> CreateBeamDisplacements(
      ReadOnlyCollection<Double6> results, ReadOnlyCollection<double> positions) {
      return new Collection<IDisplacement1d> {
        new Displacement1d(results, positions),
      };
    }

    public static Collection<IDisplacement1d> CreateBeamDisplacements(
      ReadOnlyCollection<ReadOnlyCollection<Double6>> results,
      ReadOnlyCollection<double> positions) {
      var permutations = new Collection<IDisplacement1d>();
      foreach (ReadOnlyCollection<Double6> permutation in results) {
        permutations.Add(new Displacement1d(permutation, positions));
      }

      return permutations;
    }

    public static Collection<IInternalForce1d> CreateBeamForces(
      ReadOnlyCollection<Double6> results, ReadOnlyCollection<double> positions) {
      return new Collection<IInternalForce1d> {
        new InternalForce1d(results, positions),
      };
    }

    public static Collection<IInternalForce1d> CreateBeamForces(
      ReadOnlyCollection<ReadOnlyCollection<Double6>> results,
      ReadOnlyCollection<double> positions) {
      var permutations = new Collection<IInternalForce1d>();
      foreach (ReadOnlyCollection<Double6> permutation in results) {
        permutations.Add(new InternalForce1d(permutation, positions));
      }

      return permutations;
    }

    public static Collection<IInternalForce1d> AddMissingPositions(
      this Collection<IInternalForce1d> existing, ReadOnlyCollection<Double6> results,
      ReadOnlyCollection<double> positions) {
      for (int i = 0; i < results.Count; i++) {
        if (!existing[0].Results.ContainsKey(positions[i])) {
          existing[0].Results.Add(positions[i], new ReactionForce(results[i]));
        }
      }

      return existing;
    }

    public static Collection<IInternalForce1d> AddMissingPositions(
      this Collection<IInternalForce1d> existing,
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

    public static Collection<IDisplacement1d> AddMissingPositions(
      this Collection<IDisplacement1d> existing, ReadOnlyCollection<Double6> results,
      ReadOnlyCollection<double> positions) {
      for (int i = 0; i < results.Count; i++) {
        if (!existing[0].Results.ContainsKey(positions[i])) {
          existing[0].Results.Add(positions[i], new Displacement(results[i]));
        }
      }

      return existing;
    }

    public static Collection<IDisplacement1d> AddMissingPositions(
      this Collection<IDisplacement1d> existing,
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
