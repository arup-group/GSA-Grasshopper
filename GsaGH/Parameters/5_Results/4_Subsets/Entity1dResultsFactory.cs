using GsaAPI;
using System.Collections.ObjectModel;

namespace GsaGH.Parameters.Results {
  public static class Entity1dResultsFactory {
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

    public static Collection<IEntity1dStress> CreateStresses(
      ReadOnlyCollection<StressResult1d> results, ReadOnlyCollection<double> positions) {
      return new Collection<IEntity1dStress> {
              new Entity1dStress(results, positions)
             };
    }

    public static Collection<IEntity1dStress> CreateStresses(
      ReadOnlyCollection<ReadOnlyCollection<StressResult1d>> results, ReadOnlyCollection<double> positions) {
      var permutations = new Collection<IEntity1dStress>();
      foreach (ReadOnlyCollection<StressResult1d> permutation in results) {
        permutations.Add(new Entity1dStress(permutation, positions));
      }
      return permutations;
    }

    public static Collection<IEntity1dDerivedStress> CreateDerivedStresses(
      ReadOnlyCollection<DerivedStressResult1d> results, ReadOnlyCollection<double> positions) {
      return new Collection<IEntity1dDerivedStress> {
              new Entity1dDerivedStress(results, positions)
             };
    }

    public static Collection<IEntity1dDerivedStress> CreateDerivedStresses(
      ReadOnlyCollection<ReadOnlyCollection<DerivedStressResult1d>> results, ReadOnlyCollection<double> positions) {
      var permutations = new Collection<IEntity1dDerivedStress>();
      foreach (ReadOnlyCollection<DerivedStressResult1d> permutation in results) {
        permutations.Add(new Entity1dDerivedStress(permutation, positions));
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
        for (int j = 0; j < results.Count; j++) {
          if (!existing[i].Results.ContainsKey(positions[j])) {
            existing[i].Results.Add(positions[j], new ReactionForce(results[i][j]));
          }
        }
      }

      return existing;
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

    public static Collection<IEntity1dStress> AddMissingPositions(
      this Collection<IEntity1dStress> existing,
      ReadOnlyCollection<StressResult1d> results, ReadOnlyCollection<double> positions) {
      for (int i = 0; i < results.Count; i++) {
        if (!existing[0].Results.ContainsKey(positions[i])) {
          existing[0].Results.Add(positions[i], new Stress1d(results[i]));
        }
      }

      return existing;
    }

    public static Collection<IEntity1dStress> AddMissingPositions(
      this Collection<IEntity1dStress> existing,
      ReadOnlyCollection<ReadOnlyCollection<StressResult1d>> results, ReadOnlyCollection<double> positions) {
      for (int i = 0; i < existing.Count; i++) {
        for (int j = 0; j < results.Count; j++) {
          if (!existing[i].Results.ContainsKey(positions[j])) {
            existing[i].Results.Add(positions[j], new Stress1d(results[i][j]));
          }
        }
      }

      return existing;
    }

    public static Collection<IEntity1dDerivedStress> AddMissingPositions(
      this Collection<IEntity1dDerivedStress> existing,
      ReadOnlyCollection<DerivedStressResult1d> results, ReadOnlyCollection<double> positions) {
      for (int i = 0; i < results.Count; i++) {
        if (!existing[0].Results.ContainsKey(positions[i])) {
          existing[0].Results.Add(positions[i], new Stress1dDerived(results[i]));
        }
      }

      return existing;
    }

    public static Collection<IEntity1dDerivedStress> AddMissingPositions(
      this Collection<IEntity1dDerivedStress> existing,
      ReadOnlyCollection<ReadOnlyCollection<DerivedStressResult1d>> results, ReadOnlyCollection<double> positions) {
      for (int i = 0; i < existing.Count; i++) {
        for (int j = 0; j < results.Count; j++) {
          if (!existing[i].Results.ContainsKey(positions[j])) {
            existing[i].Results.Add(positions[j], new Stress1dDerived(results[i][j]));
          }
        }
      }

      return existing;
    }
  }
}
