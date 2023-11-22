using System.Collections.Generic;
using System.Collections.ObjectModel;
using OasysUnits;

namespace GsaGH.Parameters.Results {
  public static partial class ExtremaKeyUtility {
    public static (ResultFootfall<NodeExtremaKey> Max, ResultFootfall<NodeExtremaKey> Min) GetResultFootfallExtremaKeys(
      this IDictionary<int, Collection<IFootfall>> subset) {

      var maxValue = new ResultFootfall<double>(double.MinValue);
      var minValue = new ResultFootfall<double>(double.MaxValue);

      var maxKey = new ResultFootfall<NodeExtremaKey>();
      var minKey = new ResultFootfall<NodeExtremaKey>();

      foreach (int nodeId in subset.Keys) {
        Collection<IFootfall> values = subset[nodeId];
        for (int permutation = 0; permutation < values.Count; permutation++) {
          UpdateExtrema(values[permutation], nodeId, permutation,
                  ref maxValue, ref minValue, ref maxKey, ref minKey);
        }
      }

      return (maxKey, minKey);
    }

    private static void UpdateExtrema(IFootfall item, int nodeId, int permutation,
      ref ResultFootfall<double> maxValue, ref ResultFootfall<double> minValue,
      ref ResultFootfall<NodeExtremaKey> maxKey, ref ResultFootfall<NodeExtremaKey> minKey) {

      if (item.CriticalFrequency.Value > maxValue.CriticalFrequency) {
        maxValue.CriticalFrequency = item.CriticalFrequency.Value;
        maxKey.CriticalFrequency = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.MaximumResponseFactor > maxValue.MaximumResponseFactor) {
        maxValue.MaximumResponseFactor = item.MaximumResponseFactor;
        maxKey.MaximumResponseFactor = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.PeakAcceleration.Value > maxValue.PeakAcceleration) {
        maxValue.PeakAcceleration = item.PeakAcceleration.Value;
        maxKey.PeakAcceleration = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.PeakVelocity.Value > maxValue.PeakVelocity) {
        maxValue.PeakVelocity = item.PeakVelocity.Value;
        maxKey.PeakVelocity = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.RmsAcceleration.Value > maxValue.RmsAcceleration) {
        maxValue.RmsAcceleration = item.RmsAcceleration.Value;
        maxKey.RmsAcceleration = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.RmsVelocity.Value > maxValue.RmsVelocity) {
        maxValue.RmsVelocity = item.RmsVelocity.Value;
        maxKey.RmsVelocity = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.CriticalFrequency.Value < minValue.CriticalFrequency) {
        minValue.CriticalFrequency = item.CriticalFrequency.Value;
        minKey.CriticalFrequency = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.MaximumResponseFactor < minValue.MaximumResponseFactor) {
        minValue.MaximumResponseFactor = item.MaximumResponseFactor;
        minKey.MaximumResponseFactor = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.PeakAcceleration.Value < minValue.PeakAcceleration) {
        minValue.PeakAcceleration = item.PeakAcceleration.Value;
        minKey.PeakAcceleration = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.PeakVelocity.Value < minValue.PeakVelocity) {
        minValue.PeakVelocity = item.PeakVelocity.Value;
        minKey.PeakVelocity = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.RmsAcceleration.Value < minValue.RmsAcceleration) {
        minValue.RmsAcceleration = item.RmsAcceleration.Value;
        minKey.RmsAcceleration = new NodeExtremaKey(nodeId, permutation);
      }

      if (item.RmsVelocity.Value < minValue.RmsVelocity) {
        minValue.RmsVelocity = item.RmsVelocity.Value;
        minKey.RmsVelocity = new NodeExtremaKey(nodeId, permutation);
      }
    }
  }
}