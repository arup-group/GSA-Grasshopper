using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class Element2dForcesC2p1 {
    // these are regression tests, the values are taken directly from GSA results
    // "Element2d_Simple.gwb" for elements 420 430 440 445 for combination case C2p1
    internal static List<double> NxInKiloNewtonPerMeter() {
      return new List<double>() {
        3.497,
        4.293,
        4.890,
        4.461,
        4.285,
        35.49,
        41.74,
        39.61,
        38.94,
        -1.379,
        1.152,
        3.599,
        -0.1136,
        2.375,
        1.110,
        1.124,
        -9.285,
        2.473,
        -1.670,
        -4.223,
        0.02863,
        0.09937,
        0.3607,
        -6.961,
        -0.05993,
      };
    }

    internal static List<double> NyInKiloNewtonPerMeter() {
      return new List<double>() {
        0.1364,
        0.1475,
        -0.2242,
        -0.1615,
        -0.02545,
        20.76,
        34.66,
        37.67,
        31.03,
        0.9151,
        -0.4081,
        0.9660,
        0.2535,
        0.2789,
        0.9406,
        0.4910,
        -3.005,
        1.012,
        -2.779,
        1.807,
        -0.04190,
        -1.732,
        0.4899,
        -1.253,
        -0.5234,
      };
    }

    internal static List<double> NxyInKiloNewtonPerMeter() {
      return new List<double>() {
        0.8515,
        1.024,
        -0.1927,
        -0.2357,
        0.3618,
        28.74,
        33.26,
        28.58,
        30.19,
        -1.869,
        1.708,
        0.1399,
        -0.08051,
        0.9238,
        -0.8644,
        -0.007046,
        3.170,
        -0.7124,
        3.047,
        -0.3234,
        1.065,
        0.8030,
        1.171,
        1.024,
        0.7346,
      };
    }
  }
}
