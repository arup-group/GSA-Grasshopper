using System.Collections.Generic;

using OasysUnits;

namespace GsaGH.Parameters.Results {
  public static partial class ExtremaKeyUtility {
    public static (ResultTensor2InAxis<Entity2dExtremaKey> Max, ResultTensor2InAxis<Entity2dExtremaKey> Min) GetResultTensor2InAxisEntity2dExtremaKeys<T>(
      this IDictionary<int, IList<IMeshQuantity<T>>> subset)
      where T : IResultItem {

      var maxValue = new ResultTensor2InAxis<double>(double.MinValue);
      var minValue = new ResultTensor2InAxis<double>(double.MaxValue);

      var maxKey = new ResultTensor2InAxis<Entity2dExtremaKey>();
      var minKey = new ResultTensor2InAxis<Entity2dExtremaKey>();

      foreach (int elementId in subset.Keys) {
        IList<IMeshQuantity<T>> values = subset[elementId];
        for (int permutation = 0; permutation < values.Count; permutation++) {
          switch (values[permutation]) {
            case IMeshQuantity<IForce2d> force:
              UpdateExtrema<IForce2d, ForcePerLength>(force.Results(),
                elementId, permutation, ref maxValue, ref minValue, ref maxKey, ref minKey);
              break;

          }
        }
      }

      return (maxKey, minKey);
    }

    private static void UpdateExtrema<T, Q>(IList<T> item, int elementId, int permutation,
      ref ResultTensor2InAxis<double> maxValue, ref ResultTensor2InAxis<double> minValue,
      ref ResultTensor2InAxis<Entity2dExtremaKey> maxKey, ref ResultTensor2InAxis<Entity2dExtremaKey> minKey)
      where T : IResultTensor2InAxis<Q> where Q : IQuantity {
      for (int i = 0; i < item.Count; i++) {
        if (item[i].Nx.Value > maxValue.Nx) {
          maxValue.Nx = item[i].Nx.Value;
          maxKey.Nx = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].Ny.Value > maxValue.Ny) {
          maxValue.Ny = item[i].Ny.Value;
          maxKey.Ny = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].Nxy.Value > maxValue.Nxy) {
          maxValue.Nxy = item[i].Nxy.Value;
          maxKey.Nxy = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].Nx.Value < minValue.Nx) {
          minValue.Nx = item[i].Nx.Value;
          minKey.Nx = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].Ny.Value < minValue.Ny) {
          minValue.Ny = item[i].Ny.Value;
          minKey.Ny = new Entity2dExtremaKey(elementId, i, permutation);
        }

        if (item[i].Nxy.Value < minValue.Nxy) {
          minValue.Nxy = item[i].Nxy.Value;
          minKey.Nxy = new Entity2dExtremaKey(elementId, i, permutation);
        }
      }
    }
  }
}
