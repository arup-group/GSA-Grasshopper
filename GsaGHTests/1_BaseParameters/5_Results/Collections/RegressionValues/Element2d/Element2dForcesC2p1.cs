using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class Element2dForcesC2p1 {
    // these are regression tests, the values are taken directly from GSA results
    // "Element2d_Simple.gwb" for elements 420 430 440 445 for combination case C2p1
    internal static List<double> NxInKiloNewtonPerMeter() {
      return new List<double>() {
        3.494,
        4.292,
        4.889,
        4.456,
        4.283,
        35.52,
        41.59,
        39.74,
        38.95,
        -1.379,
        1.151,
        3.600,
        -0.1137,
        2.375,
        1.111,
        1.124,
        -9.275,
        2.478,
        -1.658,
        -4.221,
        0.02765,
        0.1093,
        0.3597,
        -6.955,
        -0.06000,
      };
    }

    internal static List<double> NyInKiloNewtonPerMeter() {
      return new List<double>() {
        0.1383,
        0.1428,
        -0.2248,
        -0.1593,
        -0.02575,
        20.81,
        34.67,
        37.78,
        31.09,
        0.9111,
        -0.4139,
        0.9716,
        0.2486,
        0.2788,
        0.9414,
        0.4896,
        -3.002,
        1.007,
        -2.773,
        1.806,
        -0.04518,
        -1.728,
        0.4897,
        -1.249,
        -0.5221,
      };
    }

    internal static List<double> NxyInKiloNewtonPerMeter() {
      return new List<double>() {
        0.8575,
        1.024,
        -0.1907,
        -0.2276,
        0.3659,
        28.77,
        33.21,
        28.49,
        30.16,
        -1.870,
        1.708,
        0.1395,
        -0.08092,
        0.9239,
        -0.8653,
        -0.007445,
        3.165,
        -0.7110,
        3.041,
        -0.3216,
        1.063,
        0.8011,
        1.169,
        1.022,
        0.7333,
      };
    }
  }
}
