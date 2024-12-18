using System.Collections.Generic;
using System.Linq;

namespace GsaGH.Parameters.Results {
  public static partial class ExtremaKeyUtility {
    public static (SteelUtilisationExtremaKeys Max, SteelUtilisationExtremaKeys Min) GetSteelUtilisationExtremaKeys<T>(
      this IDictionary<int, IList<T>> subset) {
      var maxValue = new SteelUtilisation(double.MinValue);
      var minValue = new SteelUtilisation(double.MaxValue);

      var maxKeys = new SteelUtilisationExtremaKeys();
      var minKeys = new SteelUtilisationExtremaKeys();

      if (subset.Keys.Count > 0) {
        int nodeId = subset.Keys.First();
        maxKeys.Overall = new Entity0dExtremaKey(nodeId, 0);
        maxKeys.LocalCombined = new Entity0dExtremaKey(nodeId, 0);
        maxKeys.BucklingCombined = new Entity0dExtremaKey(nodeId, 0);
        maxKeys.LocalAxial = new Entity0dExtremaKey(nodeId, 0);
        maxKeys.LocalShearU = new Entity0dExtremaKey(nodeId, 0);
        maxKeys.LocalShearV = new Entity0dExtremaKey(nodeId, 0);
        maxKeys.LocalTorsion = new Entity0dExtremaKey(nodeId, 0);
        maxKeys.LocalMajorMoment = new Entity0dExtremaKey(nodeId, 0);
        maxKeys.LocalMinorMoment = new Entity0dExtremaKey(nodeId, 0);
        maxKeys.MajorBuckling = new Entity0dExtremaKey(nodeId, 0);
        maxKeys.MinorBuckling = new Entity0dExtremaKey(nodeId, 0);
        maxKeys.LateralTorsionalBuckling = new Entity0dExtremaKey(nodeId, 0);
        maxKeys.TorsionalBuckling = new Entity0dExtremaKey(nodeId, 0);
        maxKeys.FlexuralBuckling = new Entity0dExtremaKey(nodeId, 0);
        minKeys.Overall = new Entity0dExtremaKey(nodeId, 0);
        minKeys.LocalCombined = new Entity0dExtremaKey(nodeId, 0);
        minKeys.BucklingCombined = new Entity0dExtremaKey(nodeId, 0);
        minKeys.LocalAxial = new Entity0dExtremaKey(nodeId, 0);
        minKeys.LocalShearU = new Entity0dExtremaKey(nodeId, 0);
        minKeys.LocalShearV = new Entity0dExtremaKey(nodeId, 0);
        minKeys.LocalTorsion = new Entity0dExtremaKey(nodeId, 0);
        minKeys.LocalMajorMoment = new Entity0dExtremaKey(nodeId, 0);
        minKeys.LocalMinorMoment = new Entity0dExtremaKey(nodeId, 0);
        minKeys.MajorBuckling = new Entity0dExtremaKey(nodeId, 0);
        minKeys.MinorBuckling = new Entity0dExtremaKey(nodeId, 0);
        minKeys.LateralTorsionalBuckling = new Entity0dExtremaKey(nodeId, 0);
        minKeys.TorsionalBuckling = new Entity0dExtremaKey(nodeId, 0);
        minKeys.FlexuralBuckling = new Entity0dExtremaKey(nodeId, 0);
      }

      foreach (int nodeId in subset.Keys) {
        IList<T> values = subset[nodeId];
        for (int permutation = 0; permutation < values.Count; permutation++) {
          switch (values[permutation]) {
            case ISteelUtilisation utilisation:
              UpdateExtrema(utilisation, nodeId, permutation,
                  ref maxValue, ref minValue, ref maxKeys, ref minKeys);
              break;
          }
        }
      }

      return (maxKeys, minKeys);
    }

    internal static void UpdateExtrema(ISteelUtilisation item, int nodeId, int permutation,
      ref SteelUtilisation maxValue, ref SteelUtilisation minValue,
      ref SteelUtilisationExtremaKeys maxKeys, ref SteelUtilisationExtremaKeys minKeys) {

      if (item.Overall > maxValue.Overall) {
        maxValue.Overall = item.Overall;
        maxKeys.Overall = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.LocalCombined > maxValue.LocalCombined) {
        maxValue.LocalCombined = item.LocalCombined;
        maxKeys.LocalCombined = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.BucklingCombined > maxValue.BucklingCombined) {
        maxValue.BucklingCombined = item.BucklingCombined;
        maxKeys.BucklingCombined = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.LocalAxial > maxValue.LocalAxial) {
        maxValue.LocalAxial = item.LocalAxial;
        maxKeys.LocalAxial = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.LocalShearU > maxValue.LocalShearU) {
        maxValue.LocalShearU = item.LocalShearU;
        maxKeys.LocalShearU = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.LocalShearV > maxValue.LocalShearV) {
        maxValue.LocalShearV = item.LocalShearV;
        maxKeys.LocalShearV = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.LocalTorsion > maxValue.LocalTorsion) {
        maxValue.LocalTorsion = item.LocalTorsion;
        maxKeys.LocalTorsion = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.LocalMajorMoment > maxValue.LocalMajorMoment) {
        maxValue.LocalMajorMoment = item.LocalMajorMoment;
        maxKeys.LocalMajorMoment = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.LocalMinorMoment > maxValue.LocalMinorMoment) {
        maxValue.LocalMinorMoment = item.LocalMinorMoment;
        maxKeys.LocalMinorMoment = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.MajorBuckling > maxValue.MajorBuckling) {
        maxValue.MajorBuckling = item.MajorBuckling;
        maxKeys.MajorBuckling = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.MinorBuckling > maxValue.MinorBuckling) {
        maxValue.MinorBuckling = item.MinorBuckling;
        maxKeys.MinorBuckling = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.LateralTorsionalBuckling > maxValue.LateralTorsionalBuckling) {
        maxValue.LateralTorsionalBuckling = item.LateralTorsionalBuckling;
        maxKeys.LateralTorsionalBuckling = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.TorsionalBuckling > maxValue.TorsionalBuckling) {
        maxValue.TorsionalBuckling = item.TorsionalBuckling;
        maxKeys.TorsionalBuckling = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.FlexuralBuckling > maxValue.FlexuralBuckling) {
        maxValue.FlexuralBuckling = item.FlexuralBuckling;
        maxKeys.FlexuralBuckling = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.Overall < minValue.Overall) {
        minValue.Overall = item.Overall;
        minKeys.Overall = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.LocalCombined < minValue.LocalCombined) {
        minValue.LocalCombined = item.LocalCombined;
        minKeys.LocalCombined = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.BucklingCombined < minValue.BucklingCombined) {
        minValue.BucklingCombined = item.BucklingCombined;
        minKeys.BucklingCombined = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.LocalAxial < minValue.LocalAxial) {
        minValue.LocalAxial = item.LocalAxial;
        minKeys.LocalAxial = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.LocalShearU < minValue.LocalShearU) {
        minValue.LocalShearU = item.LocalShearU;
        minKeys.LocalShearU = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.LocalShearV < minValue.LocalShearV) {
        minValue.LocalShearV = item.LocalShearV;
        minKeys.LocalShearV = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.LocalTorsion < minValue.LocalTorsion) {
        minValue.LocalTorsion = item.LocalTorsion;
        minKeys.LocalTorsion = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.LocalMajorMoment < minValue.LocalMajorMoment) {
        minValue.LocalMajorMoment = item.LocalMajorMoment;
        minKeys.LocalMajorMoment = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.LocalMinorMoment < minValue.LocalMinorMoment) {
        minValue.LocalMinorMoment = item.LocalMinorMoment;
        minKeys.LocalMinorMoment = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.MajorBuckling < minValue.MajorBuckling) {
        minValue.MajorBuckling = item.MajorBuckling;
        minKeys.MajorBuckling = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.MinorBuckling < minValue.MinorBuckling) {
        minValue.MinorBuckling = item.MinorBuckling;
        minKeys.MinorBuckling = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.LateralTorsionalBuckling < minValue.LateralTorsionalBuckling) {
        minValue.LateralTorsionalBuckling = item.LateralTorsionalBuckling;
        minKeys.LateralTorsionalBuckling = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.TorsionalBuckling < minValue.TorsionalBuckling) {
        minValue.TorsionalBuckling = item.TorsionalBuckling;
        minKeys.TorsionalBuckling = new Entity0dExtremaKey(nodeId, permutation);
      }

      if (item.FlexuralBuckling < minValue.FlexuralBuckling) {
        minValue.FlexuralBuckling = item.FlexuralBuckling;
        minKeys.FlexuralBuckling = new Entity0dExtremaKey(nodeId, permutation);
      }
    }
  }
}
