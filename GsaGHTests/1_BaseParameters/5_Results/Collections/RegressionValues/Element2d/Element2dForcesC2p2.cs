using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class Element2dForcesC2p2 {
    // these are regression tests, the values are taken directly from GSA results
    // "Element2d_Simple.gwb" for elements 420 430 440 445 for combination case C2p2
    internal static List<double> NxInKiloNewtonPerMeter() {
      return new List<double>() {
        2.588,
        3.179,
        3.621,
        3.301,
        3.172,
        26.31,
        30.81,
        29.44,
        28.85,
        -1.021,
        0.8527,
        2.666,
        -0.08420,
        1.760,
        0.8226,
        0.8326,
        -6.870,
        1.836,
        -1.228,
        -3.126,
        0.02048,
        0.08096,
        0.2664,
        -5.152,
        -0.04445,
      };
    }
    internal static List<double> NyInKiloNewtonPerMeter() {
      return new List<double>() {
        0.1024,
        0.1058,
        -0.1665,
        -0.1180,
        -0.01907,
        15.41,
        25.68,
        27.98,
        23.03,
        0.6749,
        -0.3066,
        0.7197,
        0.1841,
        0.2065,
        0.6973,
        0.3627,
        -2.224,
        0.7462,
        -2.054,
        1.338,
        -0.03347,
        -1.280,
        0.3628,
        -0.9254,
        -0.3867,
      };
    }
    internal static List<double> NxyInKiloNewtonPerMeter() {
      return new List<double>() {
        0.6352,
        0.7588,
        -0.1413,
        -0.1686,
        0.2710,
        21.31,
        24.60,
        21.10,
        22.34,
        -1.385,
        1.265,
        0.1033,
        -0.05994,
        0.6844,
        -0.6410,
        -0.005515,
        2.344,
        -0.5266,
        2.252,
        -0.2382,
        0.7872,
        0.5934,
        0.8657,
        0.7573,
        0.5432,
      };
    }
  }
}
