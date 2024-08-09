using System.Collections.Generic;
using System.Collections.ObjectModel;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal static partial class Entity2dResultsFactory {

    internal static IList<IMeshQuantity<IForce2d>> CreateForce(
      ReadOnlyCollection<Tensor2> results) {
      return new List<IMeshQuantity<IForce2d>>() {
              CreateFromApiCollection(results)
             };
    }

    internal static IList<IMeshQuantity<IForce2d>> CreateForce(
      ReadOnlyCollection<ReadOnlyCollection<Tensor2>> results) {
      var permutations = new List<IMeshQuantity<IForce2d>>();
      foreach (ReadOnlyCollection<Tensor2> permutation in results) {
        permutations.Add(CreateFromApiCollection(permutation));
      }
      return permutations;
    }

    private static IMeshQuantity<IForce2d> CreateFromApiCollection(
      ReadOnlyCollection<Tensor2> results) {
      return results.Count switch {
        1 => new MeshCentreOnlyForce(results),
        4 => new Entity2dTriForce(results),
        5 => new Entity2dQuadForce(results),
        7 => new Entity2dTri6Force(results),
        9 => new Entity2dQuad8Force(results),
        _ => throw new System.Exception("Unable to create 2d result"),
      };
    }
  }
}
