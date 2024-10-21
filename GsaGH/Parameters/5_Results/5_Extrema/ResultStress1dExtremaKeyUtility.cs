using System.Collections.Generic;

namespace GsaGH.Parameters.Results {
  public static partial class ExtremaKeyUtility {
    public static (ResultStress1d<Entity1dExtremaKey> Max, ResultStress1d<Entity1dExtremaKey> Min) GetResultStress1dExtremaKeys(
      this IDictionary<int, IList<IEntity1dQuantity<IStress1d>>> subset) {

      var maxValue = new ResultStress1d<double>(double.MinValue);
      var minValue = new ResultStress1d<double>(double.MaxValue);

      var maxKey = new ResultStress1d<Entity1dExtremaKey>();
      var minKey = new ResultStress1d<Entity1dExtremaKey>();

      foreach (int elementId in subset.Keys) {
        IList<IEntity1dQuantity<IStress1d>> values = subset[elementId];
        for (int permutation = 0; permutation < values.Count; permutation++) {
          foreach (double position in values[permutation].Results.Keys) {
            UpdateExtrema(values[permutation].Results[position],
              elementId, permutation, position,
              ref maxValue, ref minValue, ref maxKey, ref minKey);
          }
        }
      }

      return (maxKey, minKey);
    }

    private static void UpdateExtrema(IStress1d item, int elementId, int permutation, double position,
      ref ResultStress1d<double> maxValue, ref ResultStress1d<double> minValue,
      ref ResultStress1d<Entity1dExtremaKey> maxKey, ref ResultStress1d<Entity1dExtremaKey> minKey) {

      if (item.Axial.Value > maxValue.Axial) {
        maxValue.Axial = item.Axial.Value;
        maxKey.Axial = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.BendingYyNegativeZ.Value > maxValue.BendingYyNegativeZ) {
        maxValue.BendingYyNegativeZ = item.BendingYyNegativeZ.Value;
        maxKey.BendingYyNegativeZ = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.BendingYyPositiveZ.Value > maxValue.BendingYyPositiveZ) {
        maxValue.BendingYyPositiveZ = item.BendingYyPositiveZ.Value;
        maxKey.BendingYyPositiveZ = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.BendingZzNegativeY.Value > maxValue.BendingZzNegativeY) {
        maxValue.BendingZzNegativeY = item.BendingZzNegativeY.Value;
        maxKey.BendingZzNegativeY = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.BendingZzPositiveY.Value > maxValue.BendingZzPositiveY) {
        maxValue.BendingZzPositiveY = item.BendingZzPositiveY.Value;
        maxKey.BendingZzPositiveY = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.CombinedC1.Value > maxValue.CombinedC1) {
        maxValue.CombinedC1 = item.CombinedC1.Value;
        maxKey.CombinedC1 = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.CombinedC2.Value > maxValue.CombinedC2) {
        maxValue.CombinedC2 = item.CombinedC2.Value;
        maxKey.CombinedC2 = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.ShearY.Value > maxValue.ShearY) {
        maxValue.ShearY = item.ShearY.Value;
        maxKey.ShearY = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.ShearZ.Value > maxValue.ShearZ) {
        maxValue.ShearZ = item.ShearZ.Value;
        maxKey.ShearZ = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.Axial.Value < minValue.Axial) {
        minValue.Axial = item.Axial.Value;
        minKey.Axial = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.BendingYyNegativeZ.Value < minValue.BendingYyNegativeZ) {
        minValue.BendingYyNegativeZ = item.BendingYyNegativeZ.Value;
        minKey.BendingYyNegativeZ = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.BendingYyPositiveZ.Value < minValue.BendingYyPositiveZ) {
        minValue.BendingYyPositiveZ = item.BendingYyPositiveZ.Value;
        minKey.BendingYyPositiveZ = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.BendingZzNegativeY.Value < minValue.BendingZzNegativeY) {
        minValue.BendingZzNegativeY = item.BendingZzNegativeY.Value;
        minKey.BendingZzNegativeY = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.BendingZzPositiveY.Value < minValue.BendingZzPositiveY) {
        minValue.BendingZzPositiveY = item.BendingZzPositiveY.Value;
        minKey.BendingZzPositiveY = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.CombinedC1.Value < minValue.CombinedC1) {
        minValue.CombinedC1 = item.CombinedC1.Value;
        minKey.CombinedC1 = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.CombinedC2.Value < minValue.CombinedC2) {
        minValue.CombinedC2 = item.CombinedC2.Value;
        minKey.CombinedC2 = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.ShearY.Value < minValue.ShearY) {
        minValue.ShearY = item.ShearY.Value;
        minKey.ShearY = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.ShearZ.Value < minValue.ShearZ) {
        minValue.ShearZ = item.ShearZ.Value;
        minKey.ShearZ = new Entity1dExtremaKey(elementId, position, permutation);
      }
    }
  }
}
