using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class AssemblyForcesAndMomentsA1 {
    // these are regression tests, the values are taken directly from GSA results
    // "assembly-simple.gwb" for Assembly 2 for analysis case A1
    internal static List<double> XInKiloNewton() {
      return new List<double>() {
        -128.3,
        -127.2,
        -111,
        -95.73,
        -80.91,
        -66.13,
        -51.38,
        -36.62,
        -21.79,
        -6.858
      };
    }

    internal static List<double> YInKiloNewton() {
      return new List<double>() {
        -2.371,
        -1.071,
        0.01729,
        -0.01222,
        0.005953,
        0.007334,
        -0.003715,
        -0.002769,
        0.01077,
        0.001208
      };
    }

    internal static List<double> ZInKiloNewton() {
      return new List<double>() {
        4.959,
        2.237,
        -0.04845,
        0.02231,
        -0.01347,
        -0.01587,
        0.007265,
        0.005487,
        -0.02253,
        -0.004697
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
        0.01025,
        0.002618,
        -0.003866,
        -8.90E-04,
        -2.49E-04,
        -1.71E-04,
        -1.08E-04,
        -9.88E-05,
        -1.28E-04,
        -7.80E-04
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
        4.64
      };
    }

    internal static List<double> ZzInKiloNewtonMeter() {
      return new List<double>() {
        41.39,
        41.29,
        36.21,
        31.12,
        26.31,
        21.5,
        16.7,
        11.91,
        7.083,
        2.229
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
