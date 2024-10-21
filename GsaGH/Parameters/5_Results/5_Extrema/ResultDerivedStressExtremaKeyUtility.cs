using System.Collections.Generic;

namespace GsaGH.Parameters.Results {
  public static partial class ExtremaKeyUtility {
    public static (ResultDerivedStress1d<Entity1dExtremaKey> Max, ResultDerivedStress1d<Entity1dExtremaKey> Min) GetResultDerivedStress1dExtremaKeys(
      this IDictionary<int, IList<IEntity1dQuantity<IStress1dDerived>>> subset) {

      var maxValue = new ResultDerivedStress1d<double>(double.MinValue);
      var minValue = new ResultDerivedStress1d<double>(double.MaxValue);

      var maxKey = new ResultDerivedStress1d<Entity1dExtremaKey>();
      var minKey = new ResultDerivedStress1d<Entity1dExtremaKey>();

      foreach (int elementId in subset.Keys) {
        IList<IEntity1dQuantity<IStress1dDerived>> values = subset[elementId];
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

    private static void UpdateExtrema(IStress1dDerived item, int elementId, int permutation, double position,
      ref ResultDerivedStress1d<double> maxValue, ref ResultDerivedStress1d<double> minValue,
      ref ResultDerivedStress1d<Entity1dExtremaKey> maxKey, ref ResultDerivedStress1d<Entity1dExtremaKey> minKey) {

      if (item.ElasticShearY.Value > maxValue.ElasticShearY) {
        maxValue.ElasticShearY = item.ElasticShearY.Value;
        maxKey.ElasticShearY = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.ElasticShearZ.Value > maxValue.ElasticShearZ) {
        maxValue.ElasticShearZ = item.ElasticShearZ.Value;
        maxKey.ElasticShearZ = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.Torsional.Value > maxValue.Torsional) {
        maxValue.Torsional = item.Torsional.Value;
        maxKey.Torsional = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.VonMises.Value > maxValue.VonMises) {
        maxValue.VonMises = item.VonMises.Value;
        maxKey.VonMises = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.ElasticShearY.Value < minValue.ElasticShearY) {
        minValue.ElasticShearY = item.ElasticShearY.Value;
        minKey.ElasticShearY = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.ElasticShearZ.Value < minValue.ElasticShearZ) {
        minValue.ElasticShearZ = item.ElasticShearZ.Value;
        minKey.ElasticShearZ = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.Torsional.Value < minValue.Torsional) {
        minValue.Torsional = item.Torsional.Value;
        minKey.Torsional = new Entity1dExtremaKey(elementId, position, permutation);
      }

      if (item.VonMises.Value < minValue.VonMises) {
        minValue.VonMises = item.VonMises.Value;
        minKey.VonMises = new Entity1dExtremaKey(elementId, position, permutation);
      }
    }
  }
}
