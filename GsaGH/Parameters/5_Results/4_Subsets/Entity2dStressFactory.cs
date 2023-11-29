using GsaAPI;
using System.Collections.ObjectModel;

namespace GsaGH.Parameters.Results {
  public static partial class Entity2dResultsFactory {
    public static Collection<IMeshQuantity<IStress>> CreateStresses(
      ReadOnlyCollection<Tensor3> results) {
      return new Collection<IMeshQuantity<IStress>> {
              CreateFromApiCollection(results)
             };
    }

    public static Collection<IMeshQuantity<IStress>> CreateStresses(
      ReadOnlyCollection<ReadOnlyCollection<Tensor3>> results) {
      var permutations = new Collection<IMeshQuantity<IStress>>();
      foreach (ReadOnlyCollection<Tensor3> permutation in results) {
        permutations.Add(CreateFromApiCollection(permutation));
      }
      return permutations;
    }

    private static IMeshQuantity<IStress> CreateFromApiCollection(
      ReadOnlyCollection<Tensor3> results) {
      return results.Count switch {
        1 => new MeshCentreOnlyStress(results),
        4 => new Entity2dTriStress(results),
        5 => new Entity2dQuadStress(results),
        7 => new Entity2dTri6Stress(results),
        9 => new Entity2dQuad8Stress(results),
        _ => throw new System.ArgumentException("Unable to create 2d result"),
      };
    }
  }
}
