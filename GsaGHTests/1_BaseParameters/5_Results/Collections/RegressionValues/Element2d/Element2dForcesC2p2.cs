using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class Element2dForcesC2p2 {
    // these are regression tests, the values are taken directly from GSA results
    // "Element2d_Simple.gwb" for elements 420 430 440 445 for combination case C2p2
    internal static List<double> NxInKiloNewtonPerMeter() {
      return new List<double>() {
        2.591,
        3.180,
        3.622,
        3.305,
        3.174,
        26.29,
        30.92,
        29.34,
        28.85,
        -1.022,
        0.8535,
        2.666,
        -0.08416,
        1.760,
        0.8219,
        0.8324,
        -6.878,
        1.832,
        -1.237,
        -3.128,
        0.02121,
        0.07361,
        0.2672,
        -5.157,
        -0.04439,
      };
    }
    internal static List<double> NyInKiloNewtonPerMeter() {
      return new List<double>() {
        0.1011,
        0.1093,
        -0.1661,
        -0.1196,
        -0.01886,
        15.38,
        25.67,
        27.90,
        22.98,
        0.6779,
        -0.3023,
        0.7155,
        0.1878,
        0.2066,
        0.6967,
        0.3637,
        -2.226,
        0.7496,
        -2.058,
        1.339,
        -0.03103,
        -1.283,
        0.3629,
        -0.9279,
        -0.3877,
      };
    }
    internal static List<double> NxyInKiloNewtonPerMeter() {
      return new List<double>() {
        0.6308,
        0.7585,
        -0.1427,
        -0.1746,
        0.2680,
        21.29,
        24.64,
        21.17,
        22.36,
        -1.384,
        1.265,
        0.1036,
        -0.05963,
        0.6843,
        -0.6403,
        -0.005220,
        2.348,
        -0.5277,
        2.257,
        -0.2395,
        0.7886,
        0.5948,
        0.8671,
        0.7582,
        0.5442,
      };
    }
  }
}
