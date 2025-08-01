using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class AssemblyForcesAndMomentsA1 {
    // these are regression tests, the values are taken directly from GSA results
    // "assembly-simple.gwb" for Assembly 2 for analysis case A1
    internal static List<double> XInKiloNewton() {
      return new List<double>() {
        -128.3,
        -127.2,
        -111.0,
        -95.73,
        -80.91,
        -66.13,
        -51.38,
        -36.62,
        -21.79,
        -6.858,
      };
    }

    internal static List<double> YInKiloNewton() {
      return new List<double>() {
        -2.368,
        -1.071,
        0.01715,
        -0.01224,
        0.005943,
        0.007326,
        -0.003716,
        -0.002749,
        0.01087,
        0.001248,
      };
    }

    internal static List<double> ZInKiloNewton() {
      return new List<double>() {
        4.960,
        2.236,
        -0.04881,
        0.02225,
        -0.01349,
        -0.01587,
        0.007263,
        0.005497,
        -0.02248,
        -0.004678,
      };
    }

    internal static List<double> YzInKiloNewton() {
      return new List<double>() {
        5.496,
        2.48,
        0.05144,
        0.02544,
        0.01473,
        0.01748,
        0.00816,
        0.006146,
        0.02497,
        0.004849
      };
    }

    internal static List<double> XxInKiloNewtonMeter() {
      return new List<double>() {
        0.01236,
        0.002268,
        -0.004047,
        -928.9E-6,
        -261.4E-6,
        -179.3E-6,
        -112.6E-6,
        -103.5E-6,
        -162.3E-6,
        -848.5E-6,
      };
    }

    internal static List<double> YyInKiloNewtonMeter() {
      return new List<double>() {
        86.01,
        85.95,
        75.34,
        64.76,
        54.74,
        44.74,
        34.76,
        24.78,
        14.74,
        4.640,
      };
    }

    internal static List<double> ZzInKiloNewtonMeter() {
      return new List<double>() {
        41.38,
        41.30,
        36.21,
        31.12,
        26.31,
        21.50,
        16.70,
        11.91,
        7.083,
        2.229,
      };
    }

    internal static List<double> YyzzInKiloNewtonMeter() {
      return new List<double>() {
        95.44,
        95.36,
        83.59,
        71.85,
        60.73,
        49.64,
        38.56,
        27.49,
        16.35,
        5.148
      };
    }
  }
}
