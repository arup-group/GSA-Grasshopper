using System.Collections.Generic;

namespace GsaGH.Parameters.Results {
  public static partial class ExtremaKeyUtility {
    public static (Entity1dExtremaKey Max, Entity1dExtremaKey Min) GetEntity1dExtremaKeys<T1, T2>(
      this IDictionary<int, IList<T1>> subset)
      where T1 : IEntity1dQuantity<T2>
      where T2 : IResultItem {

      double maxValue = double.MinValue;
      double minValue = double.MaxValue;

      Entity1dExtremaKey maxKey = null;
      Entity1dExtremaKey minKey = null;

      foreach (int elementId in subset.Keys) {
        IList<T1> values = subset[elementId];
        for (int permutation = 0; permutation < values.Count; permutation++) {
          foreach (double position in values[permutation].Results.Keys) {
            switch (values[permutation].Results[position]) {
              case IEnergyDensity energyDensity:
                UpdateExtrema(energyDensity,
              elementId, permutation, position,
              ref maxValue, ref minValue, ref maxKey, ref minKey);
                break;
            }
          }
        }
      }

      return (maxKey, minKey);
    }

    private static void UpdateExtrema(IEnergyDensity item, int elementId, int permutation, double position,
      ref double maxValue, ref double minValue,
      ref Entity1dExtremaKey maxKey, ref Entity1dExtremaKey minKey) {

      if (item.EnergyDensity.Value > maxValue) {
        maxValue = item.EnergyDensity.Value;
        maxKey = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.EnergyDensity.Value < minValue) {
        minValue = item.EnergyDensity.Value;
        minKey = new Entity1dExtremaKey(elementId, position, permutation);
      }
    }
  }
}
