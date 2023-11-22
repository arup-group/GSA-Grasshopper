using GsaAPI;
using System.Collections.ObjectModel;

namespace GsaGH.Parameters.Results {
  public static partial class Entity1dResultsFactory {
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
