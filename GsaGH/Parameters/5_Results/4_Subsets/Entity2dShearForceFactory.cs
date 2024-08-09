using System.Collections.Generic;
using System.Collections.ObjectModel;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal static partial class Entity2dResultsFactory {
    public static IList<IMeshQuantity<IShear2d>> CreateShearForce(
      ReadOnlyCollection<Vector2> results) {
      return new List<IMeshQuantity<IShear2d>>() {
        CreateShearForceFromApiCollection(results)
      };
    }

    internal static IList<IMeshQuantity<IShear2d>> CreateShearForce(
      ReadOnlyCollection<ReadOnlyCollection<Vector2>> results) {
      var permutations = new List<IMeshQuantity<IShear2d>>();
      foreach (ReadOnlyCollection<Vector2> permutation in results) {
        permutations.Add(CreateShearForceFromApiCollection(permutation));
      }
      return permutations;
    }

    private static IMeshQuantity<IShear2d> CreateShearForceFromApiCollection(
      ReadOnlyCollection<Vector2> results) {
      return results.Count switch {
        1 => new MeshCentreOnlyShear(results),
        4 => new Entity2dTriShear(results),
        5 => new Entity2dQuadShear(results),
        7 => new Entity2dTri6Shear(results),
        9 => new Entity2dQuad8Shear(results),
        _ => throw new System.Exception("Unable to create 2d result"),
      };
    }
  }
}
