using System.Collections.Generic;

namespace GsaGH.Parameters.Results {
  public static partial class ExtremaKeyUtility {
    public static (Entity0dExtremaKey Max, Entity0dExtremaKey Min) GetEntity0dExtremaKeys<T>(
      this IDictionary<int, IList<T>> subset) {

      double maxValue = double.MinValue;
      double minValue = double.MaxValue;

      Entity0dExtremaKey maxKey = null;
      Entity0dExtremaKey minKey = null;

      foreach (int nodeId in subset.Keys) {
        IList<T> values = subset[nodeId];
        for (int permutation = 0; permutation < values.Count; permutation++) {
          switch (values[permutation]) {
            case IEnergyDensity energyDensity:
              UpdateExtrema(energyDensity, nodeId, permutation,
                  ref maxValue, ref minValue, ref maxKey, ref minKey);
              break;
          }
        }
      }

      return (maxKey, minKey);
    }

    private static void UpdateExtrema(IEnergyDensity item, int nodeId, int permutation,
      ref double maxValue, ref double minValue,
      ref Entity0dExtremaKey maxKey, ref Entity0dExtremaKey minKey) {

      if (item.EnergyDensity.Value > maxValue) {
        maxValue = item.EnergyDensity.Value;
        maxKey = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.EnergyDensity.Value < minValue) {
        minValue = item.EnergyDensity.Value;
        minKey = new Entity0dExtremaKey(nodeId, permutation);
      }
    }
  }
}
