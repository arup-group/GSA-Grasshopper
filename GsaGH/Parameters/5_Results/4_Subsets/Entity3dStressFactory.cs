using GsaAPI;
using System.Collections.ObjectModel;

namespace GsaGH.Parameters.Results {
  public static partial class Entity3dResultsFactory {
    public static Collection<IEntity2dQuantity<IStress>> CreateStresses(
      ReadOnlyCollection<Tensor3> results) {
      return new Collection<IEntity2dQuantity<IStress>> {
              CreateFromApiCollection(results)
             };
    }

    public static Collection<IEntity2dQuantity<IStress>> CreateStresses(
      ReadOnlyCollection<ReadOnlyCollection<Tensor3>> results) {
      var permutations = new Collection<IEntity2dQuantity<IStress>>();
      foreach (ReadOnlyCollection<Tensor3> permutation in results) {
        permutations.Add(CreateFromApiCollection(permutation));
      }
      return permutations;
    }

    private static IEntity2dQuantity<IStress> CreateFromApiCollection(
      ReadOnlyCollection<Tensor3> results) {
      return results.Count switch {
        9 => new Entity3dTetra4Stress(results),
        11 => new Entity3dPyramid5Stress(results),
        12 => new Entity3dWedge6Stress(results),
        15 => new Entity3dBrick8Stress(results),
        _ => throw new System.ArgumentException("Unable to create 3d result"),
      };
    }
  }
}
