using GsaAPI;
using System.Collections.ObjectModel;

namespace GsaGH.Parameters.Results {
  public static partial class Entity2dResultsFactory {
    
    public static Collection<IEntity2dQuantity<IMoment2d>> CreateMoment(
      ReadOnlyCollection<Tensor2> results) {
      return new Collection<IEntity2dQuantity<IMoment2d>>() {
        CreateMomentFromApiCollection(results)
      };
    }

    public static Collection<IEntity2dQuantity<IMoment2d>> CreateMoment(
      ReadOnlyCollection<ReadOnlyCollection<Tensor2>> results) {
      var permutations = new Collection<IEntity2dQuantity<IMoment2d>>();
      foreach (ReadOnlyCollection<Tensor2> permutation in results) {
        permutations.Add(CreateMomentFromApiCollection(permutation));
      }
      return permutations;
    }

    private static IEntity2dQuantity<IMoment2d> CreateMomentFromApiCollection(
      ReadOnlyCollection<Tensor2> results) {
      return results.Count switch {
        4 => new Entity2dTriMoment(results),
        5 => new Entity2dQuadMoment(results),
        7 => new Entity2dTri6Moment(results),
        9 => new Entity2dQuad8Moment(results),
        _ => throw new System.Exception("Unable to create 2d result"),
      };
    }
  }
}
