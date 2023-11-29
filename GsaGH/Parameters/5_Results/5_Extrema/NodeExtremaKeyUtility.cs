using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GsaGH.Parameters.Results {
  public static partial class ExtremaKeyUtility {
    public static (NodeExtremaKey Max, NodeExtremaKey Min) GetNodeExtremaKeys<T>(
      this IDictionary<int, IList<T>> subset) {

      double maxValue = double.MinValue;
      double minValue = double.MaxValue;

      NodeExtremaKey maxKey = null;
      NodeExtremaKey minKey = null;

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
      ref NodeExtremaKey maxKey, ref NodeExtremaKey minKey) {

      if (item.EnergyDensity.Value > maxValue) {
        maxValue = item.EnergyDensity.Value;
        maxKey = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.EnergyDensity.Value < minValue) {
        minValue = item.EnergyDensity.Value;
        minKey = new NodeExtremaKey(nodeId, permutation);
      }
    }
  }
}
