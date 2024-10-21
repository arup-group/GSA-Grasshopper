using System.Collections.Generic;

namespace GsaGH.Parameters.Results {
  public static partial class ExtremaKeyUtility {
    public static (ResultFootfall<Entity0dExtremaKey> Max, ResultFootfall<Entity0dExtremaKey> Min) GetResultFootfallExtremaKeys(
      this IDictionary<int, IList<IFootfall>> subset) {

      var maxValue = new ResultFootfall<double>(double.MinValue);
      var minValue = new ResultFootfall<double>(double.MaxValue);

      var maxKey = new ResultFootfall<Entity0dExtremaKey>();
      var minKey = new ResultFootfall<Entity0dExtremaKey>();

      foreach (int nodeId in subset.Keys) {
        IList<IFootfall> values = subset[nodeId];
        for (int permutation = 0; permutation < values.Count; permutation++) {
          UpdateExtrema(values[permutation], nodeId, permutation,
                  ref maxValue, ref minValue, ref maxKey, ref minKey);
        }
      }

      return (maxKey, minKey);
    }

    private static void UpdateExtrema(IFootfall item, int nodeId, int permutation,
      ref ResultFootfall<double> maxValue, ref ResultFootfall<double> minValue,
      ref ResultFootfall<Entity0dExtremaKey> maxKey, ref ResultFootfall<Entity0dExtremaKey> minKey) {

      if (item.CriticalFrequency.Value > maxValue.CriticalFrequency) {
        maxValue.CriticalFrequency = item.CriticalFrequency.Value;
        maxKey.CriticalFrequency = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.MaximumResponseFactor > maxValue.MaximumResponseFactor) {
        maxValue.MaximumResponseFactor = item.MaximumResponseFactor;
        maxKey.MaximumResponseFactor = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.PeakAcceleration.Value > maxValue.PeakAcceleration) {
        maxValue.PeakAcceleration = item.PeakAcceleration.Value;
        maxKey.PeakAcceleration = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.PeakVelocity.Value > maxValue.PeakVelocity) {
        maxValue.PeakVelocity = item.PeakVelocity.Value;
        maxKey.PeakVelocity = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.RmsAcceleration.Value > maxValue.RmsAcceleration) {
        maxValue.RmsAcceleration = item.RmsAcceleration.Value;
        maxKey.RmsAcceleration = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.RmsVelocity.Value > maxValue.RmsVelocity) {
        maxValue.RmsVelocity = item.RmsVelocity.Value;
        maxKey.RmsVelocity = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.CriticalFrequency.Value < minValue.CriticalFrequency) {
        minValue.CriticalFrequency = item.CriticalFrequency.Value;
        minKey.CriticalFrequency = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.MaximumResponseFactor < minValue.MaximumResponseFactor) {
        minValue.MaximumResponseFactor = item.MaximumResponseFactor;
        minKey.MaximumResponseFactor = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.PeakAcceleration.Value < minValue.PeakAcceleration) {
        minValue.PeakAcceleration = item.PeakAcceleration.Value;
        minKey.PeakAcceleration = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.PeakVelocity.Value < minValue.PeakVelocity) {
        minValue.PeakVelocity = item.PeakVelocity.Value;
        minKey.PeakVelocity = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.RmsAcceleration.Value < minValue.RmsAcceleration) {
        minValue.RmsAcceleration = item.RmsAcceleration.Value;
        minKey.RmsAcceleration = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.RmsVelocity.Value < minValue.RmsVelocity) {
        minValue.RmsVelocity = item.RmsVelocity.Value;
        minKey.RmsVelocity = new Entity0dExtremaKey(nodeId, permutation);
      }
    }
  }
}
