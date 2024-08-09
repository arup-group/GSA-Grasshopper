using System.Collections.Generic;
using System.Collections.ObjectModel;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal static partial class Entity2dResultsFactory {

    internal static IList<IMeshQuantity<IMoment2d>> CreateMoment(
      ReadOnlyCollection<Tensor2> results) {
      return new List<IMeshQuantity<IMoment2d>>() {
        CreateMomentFromApiCollection(results)
      };
    }

    internal static IList<IMeshQuantity<IMoment2d>> CreateMoment(
      ReadOnlyCollection<ReadOnlyCollection<Tensor2>> results) {
      var permutations = new List<IMeshQuantity<IMoment2d>>();
      foreach (ReadOnlyCollection<Tensor2> permutation in results) {
        permutations.Add(CreateMomentFromApiCollection(permutation));
      }
      return permutations;
    }

    private static IMeshQuantity<IMoment2d> CreateMomentFromApiCollection(
      ReadOnlyCollection<Tensor2> results) {
      return results.Count switch {
        1 => new MeshCentreOnlyMoment(results),
        4 => new Entity2dTriMoment(results),
        5 => new Entity2dQuadMoment(results),
        7 => new Entity2dTri6Moment(results),
        9 => new Entity2dQuad8Moment(results),
        _ => throw new System.Exception("Unable to create 2d result"),
      };
    }
  }
}
