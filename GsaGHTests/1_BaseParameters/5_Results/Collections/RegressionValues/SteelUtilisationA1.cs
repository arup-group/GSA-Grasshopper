using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel.Types;

using OasysGH.Parameters;

namespace GsaGHTests.Parameters.Results {
  public class SteelUtilisationsA1 {
    // these are regression tests, the values are taken directly from GSA results
    // "Steel_Design_Simple.gwb" for member id=all for analysis case A1

    internal static List<double?> Overall = new List<double?>() {
      0.1499,
    };
    internal static List<double?> LocalCombined = new List<double?>() {
      0.1499,
    };
    internal static List<double?> BucklingCombined = new List<double?>() {
      0.1421
    };
    internal static List<double?> LocalAxis = new List<double?>() {
      0,
    };
    internal static List<double?> LocalSu = new List<double?>() {
      0,
    };
    internal static List<double?> LocalSv = new List<double?>() {
      0.04847,
    };
    internal static List<double?> LocalTorsion = new List<double?>() {
      0,
    };
    internal static List<double?> LocalMuu = new List<double?>() {
      0.1499,
    };
    internal static List<double?> LocalMvv = new List<double?>() {
      0,
    };
    internal static List<double?> BucklingUu = new List<double?>() {
       0,
    };
    internal static List<double?> BucklingVv = new List<double?>() {
       0,
    };
    internal static List<double?> BucklingLt = new List<double?>() {
      0,
    };
    internal static List<double?> BucklingTor = new List<double?>() {
      null,
    };
    internal static List<double?> BucklingFt = new List<double?>() {
      null,
    };
  }
}
