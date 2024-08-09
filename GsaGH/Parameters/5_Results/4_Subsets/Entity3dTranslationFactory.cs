using System.Collections.Generic;
using System.Collections.ObjectModel;

using GsaAPI;

namespace GsaGH.Parameters.Results {
  internal static partial class Entity3dResultsFactory {
    internal static IList<IMeshQuantity<ITranslation>> CreateTranslations(
      ReadOnlyCollection<Double3> results) {
      return new List<IMeshQuantity<ITranslation>> {
              CreateFromApiCollection(results)
             };
    }

    internal static IList<IMeshQuantity<ITranslation>> CreateTranslations(
      ReadOnlyCollection<ReadOnlyCollection<Double3>> results) {
      var permutations = new List<IMeshQuantity<ITranslation>>();
      foreach (ReadOnlyCollection<Double3> permutation in results) {
        permutations.Add(CreateFromApiCollection(permutation));
      }
      return permutations;
    }

    private static IMeshQuantity<ITranslation> CreateFromApiCollection(
      ReadOnlyCollection<Double3> results) {
      return results.Count switch {
        9 => new Entity3dTetra4Translation(results),
        11 => new Entity3dPyramid5Translation(results),
        12 => new Entity3dWedge6Translation(results),
        15 => new Entity3dBrick8Translation(results),
        _ => throw new System.ArgumentException("Unable to create 3d result"),
      };
    }
  }
}
