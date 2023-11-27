using GsaAPI;
using System.Collections.ObjectModel;

namespace GsaGH.Parameters.Results {
  public static partial class Entity2dResultsFactory {
    
    public static Collection<IEntity2dQuantity<IShear2d>> CreateShearForce(
      ReadOnlyCollection<Vector2> results) {
      return new Collection<IEntity2dQuantity<IShear2d>>() {
        CreateShearForceFromApiCollection(results)
      };
    }

    public static Collection<IEntity2dQuantity<IShear2d>> CreateShearForce(
      ReadOnlyCollection<ReadOnlyCollection<Vector2>> results) {
      var permutations = new Collection<IEntity2dQuantity<IShear2d>>();
      foreach (ReadOnlyCollection<Vector2> permutation in results) {
        permutations.Add(CreateShearForceFromApiCollection(permutation));
      }
      return permutations;
    }

    private static IEntity2dQuantity<IShear2d> CreateShearForceFromApiCollection(
      ReadOnlyCollection<Vector2> results) {
      return results.Count switch {
        4 => new Entity2dTriShear(results),
        5 => new Entity2dQuadShear(results),
        7 => new Entity2dTri6Shear(results),
        9 => new Entity2dQuad8Shear(results),
        _ => throw new System.Exception("Unable to create 2d result"),
      };
    }
  }
}
