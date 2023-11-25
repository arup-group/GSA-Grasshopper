using GsaAPI;
using System.Collections.ObjectModel;

namespace GsaGH.Parameters.Results {
  public static partial class Entity3dResultsFactory {
    public static Collection<IEntity2dQuantity<ITranslation>> CreateTranslations(
      ReadOnlyCollection<Double3> results) {
      return new Collection<IEntity2dQuantity<ITranslation>> {
              CreateFromApiCollection(results)
             };
    }

    public static Collection<IEntity2dQuantity<ITranslation>> CreateTranslations(
      ReadOnlyCollection<ReadOnlyCollection<Double3>> results) {
      var permutations = new Collection<IEntity2dQuantity<ITranslation>>();
      foreach (ReadOnlyCollection<Double3> permutation in results) {
        permutations.Add(CreateFromApiCollection(permutation));
      }
      return permutations;
    }

    private static IEntity2dQuantity<ITranslation> CreateFromApiCollection(
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
