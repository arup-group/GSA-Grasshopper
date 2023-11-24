using GsaAPI;
using System.Collections.ObjectModel;

namespace GsaGH.Parameters.Results {
  public static partial class Entity2dResultsFactory {
    
    public static Collection<IEntity2dQuantity<IForce2d>> CreateForce(
      ReadOnlyCollection<Tensor2> results) {
      return new Collection<IEntity2dQuantity<IForce2d>>() {
              CreateFromApiCollection(results)
             };
    }

    public static Collection<IEntity2dQuantity<IForce2d>> CreateForce(
      ReadOnlyCollection<ReadOnlyCollection<Tensor2>> results) {
      var permutations = new Collection<IEntity2dQuantity<IForce2d>>();
      foreach (ReadOnlyCollection<Tensor2> permutation in results) {
        permutations.Add(CreateFromApiCollection(permutation));
      }
      return permutations;
    }

    private static IEntity2dQuantity<IForce2d> CreateFromApiCollection(
      ReadOnlyCollection<Tensor2> results) {
      return results.Count switch {
        4 => new Entity2dTriForce(results),
        5 => new Entity2dQuadForce(results),
        7 => new Entity2dTri6Force(results),
        9 => new Entity2dQuad8Force(results),
        _ => throw new System.Exception("Unable to create 2d result"),
      };
    }
  }
}
