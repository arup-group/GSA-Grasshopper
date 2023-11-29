using GsaAPI;
using System.Collections.ObjectModel;

namespace GsaGH.Parameters.Results {
  public static partial class Entity2dResultsFactory {
    public static Collection<IMeshQuantity<IDisplacement>> CreateDisplacements(
      ReadOnlyCollection<Double6> results) {
      return new Collection<IMeshQuantity<IDisplacement>> {
              CreateFromApiCollection(results)
             };
    }

    public static Collection<IMeshQuantity<IDisplacement>> CreateDisplacements(
      ReadOnlyCollection<ReadOnlyCollection<Double6>> results) {
      var permutations = new Collection<IMeshQuantity<IDisplacement>>();
      foreach (ReadOnlyCollection<Double6> permutation in results) {
        permutations.Add(CreateFromApiCollection(permutation));
      }
      return permutations;
    }

    private static IMeshQuantity<IDisplacement> CreateFromApiCollection(
      ReadOnlyCollection<Double6> results) {
      return results.Count switch {
        4 => new Entity2dTriDisplacement(results),
        5 => new Entity2dQuadDisplacement(results),
        7 => new Entity2dTri6Displacement(results),
        9 => new Entity2dQuad8Displacement(results),
        _ => throw new System.ArgumentException("Unable to create 2d result"),
      };
    }
  }
}
