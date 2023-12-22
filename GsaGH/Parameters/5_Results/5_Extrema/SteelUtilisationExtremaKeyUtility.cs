using System.Collections.Generic;
using System.Linq;

namespace GsaGH.Parameters.Results {
  public static partial class ExtremaKeyUtility {
    public static (Entity0dExtremaKey Max, Entity0dExtremaKey Min) GetSteelUtilisationExtremaKeys<T>(
      this IDictionary<int, IList<T>> subset) {

      var maxValue = new SteelUtilisation(double.MaxValue);
      var minValue = new SteelUtilisation(0.0);

      Entity0dExtremaKey maxKey = null;
      Entity0dExtremaKey minKey = null;

      if (subset.Keys.Count > 0) {
        int nodeId = subset.Keys.First();
        maxKey.X = new Entity0dExtremaKey(nodeId, 0);
        maxKey.Y = new Entity0dExtremaKey(nodeId, 0);
        maxKey.Z = new Entity0dExtremaKey(nodeId, 0);
        maxKey.Xyz = new Entity0dExtremaKey(nodeId, 0);
        maxKey.Xx = new Entity0dExtremaKey(nodeId, 0);
        maxKey.Yy = new Entity0dExtremaKey(nodeId, 0);
        maxKey.Zz = new Entity0dExtremaKey(nodeId, 0);
        maxKey.Xxyyzz = new Entity0dExtremaKey(nodeId, 0);
        minKey.X = new Entity0dExtremaKey(nodeId, 0);
        minKey.Y = new Entity0dExtremaKey(nodeId, 0);
        minKey.Z = new Entity0dExtremaKey(nodeId, 0);
        minKey.Xyz = new Entity0dExtremaKey(nodeId, 0);
        minKey.Xx = new Entity0dExtremaKey(nodeId, 0);
        minKey.Yy = new Entity0dExtremaKey(nodeId, 0);
        minKey.Zz = new Entity0dExtremaKey(nodeId, 0);
        minKey.Xxyyzz = new Entity0dExtremaKey(nodeId, 0);
      }

      foreach (int nodeId in subset.Keys) {
        IList<T> values = subset[nodeId];
        for (int permutation = 0; permutation < values.Count; permutation++) {
          switch (values[permutation]) {
            case ISteelUtilisation utilisation:
              UpdateExtrema(utilisation, nodeId, permutation,
                  ref maxValue, ref minValue, ref maxKey, ref minKey);
              break;
          }
        }
      }

      return (maxKey, minKey);
    }

    private static void UpdateExtrema(ISteelUtilisation item, int nodeId, int permutation,
      ref SteelUtilisation maxValue, ref SteelUtilisation minValue,
      ref Entity0dExtremaKey maxKey, ref Entity0dExtremaKey minKey) {

      if (item.Overall > maxValue.Overall) {
        maxValue.Overall = item.Overall;
        maxKey = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.LocalCombined > maxValue.LocalCombined) {
        maxValue.LocalCombined = item.LocalCombined;
        maxKey = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.BucklingCombined > maxValue.BucklingCombined) {
        maxValue.BucklingCombined = item.BucklingCombined;
        maxKey = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.LocalAxial > maxValue.LocalAxial) {
        maxValue.LocalAxial = item.LocalAxial;
        maxKey = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.LocalShearU > maxValue.LocalShearU) {
        maxValue.LocalShearU = item.LocalShearU;
        maxKey = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.LocalShearV > maxValue.LocalShearV) {
        maxValue.LocalShearV = item.LocalShearV;
        maxKey = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.LocalTorsion > maxValue.LocalTorsion) {
        maxValue.LocalTorsion = item.LocalTorsion;
        maxKey = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.LocalMajorMoment > maxValue.LocalMajorMoment) {
        maxValue.LocalMajorMoment = item.LocalMajorMoment;
        maxKey = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.LocalMinorMoment > maxValue.LocalMinorMoment) {
        maxValue.LocalMinorMoment = item.LocalMinorMoment;
        maxKey = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.MajorBuckling > maxValue.MajorBuckling) {
        maxValue.MajorBuckling = item.MajorBuckling;
        maxKey = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.MinorBuckling > maxValue.MinorBuckling) {
        maxValue.MinorBuckling = item.MinorBuckling;
        maxKey = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.LateralTorsionalBuckling > maxValue.LateralTorsionalBuckling) {
        maxValue.LateralTorsionalBuckling = item.LateralTorsionalBuckling;
        maxKey = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.TorsionalBuckling > maxValue.TorsionalBuckling) {
        maxValue.TorsionalBuckling = item.TorsionalBuckling;
        maxKey = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.FlexuralBuckling > maxValue.FlexuralBuckling) {
        maxValue.FlexuralBuckling = item.FlexuralBuckling;
        maxKey = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.Overall > maxValue.Overall) {
        minValue.Overall = item.Overall;
        minKey = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.LocalCombined > minValue.LocalCombined) {
        minValue.LocalCombined = item.LocalCombined;
        minKey = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.BucklingCombined > minValue.BucklingCombined) {
        minValue.BucklingCombined = item.BucklingCombined;
        minKey = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.LocalAxial > minValue.LocalAxial) {
        minValue.LocalAxial = item.LocalAxial;
        minKey = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.LocalShearU > minValue.LocalShearU) {
        minValue.LocalShearU = item.LocalShearU;
        minKey = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.LocalShearV > minValue.LocalShearV) {
        minValue.LocalShearV = item.LocalShearV;
        minKey = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.LocalTorsion > minValue.LocalTorsion) {
        minValue.LocalTorsion = item.LocalTorsion;
        minKey = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.LocalMajorMoment > minValue.LocalMajorMoment) {
        minValue.LocalMajorMoment = item.LocalMajorMoment;
        minKey = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.LocalMinorMoment > minValue.LocalMinorMoment) {
        minValue.LocalMinorMoment = item.LocalMinorMoment;
        minKey = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.MajorBuckling > minValue.MajorBuckling) {
        minValue.MajorBuckling = item.MajorBuckling;
        minKey = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.MinorBuckling > minValue.MinorBuckling) {
        minValue.MinorBuckling = item.MinorBuckling;
        minKey = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.LateralTorsionalBuckling > minValue.LateralTorsionalBuckling) {
        minValue.LateralTorsionalBuckling = item.LateralTorsionalBuckling;
        minKey = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.TorsionalBuckling > minValue.TorsionalBuckling) {
        minValue.TorsionalBuckling = item.TorsionalBuckling;
        minKey = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.FlexuralBuckling > minValue.FlexuralBuckling) {
        minValue.FlexuralBuckling = item.FlexuralBuckling;
        minKey = new Entity0dExtremaKey(nodeId, permutation);
      }
    }
  }
}
