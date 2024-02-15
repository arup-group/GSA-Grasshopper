using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class AssemblyForcesAndMomentsC1 {
    // these are regression tests, the values are taken directly from GSA results
    // "assembly-simple.gwb" for Assembly 2 for combination case C1
    internal static List<double> XInKiloNewton() {
      return new List<double>() {
        -114.8,
        -111.2,
        -96.58,
        -84.26,
        -71.78,
        -59.19,
        -46.4,
        -33.31,
        -20.61,
        -7.180
      };
    }

    internal static List<double> YInKiloNewton() {
      return new List<double>() {
        -2.888,
        -1.648,
        -0.6701,
        -0.4918,
        -0.4346,
        -0.3018,
        -0.1871,
        -0.09014,
        -0.0597,
        0.1231
      };
    }

    internal static List<double> ZInKiloNewton() {
      return new List<double>() {
        4.533,
        1.842,
        -0.2009,
        -0.3926,
        -0.2645,
        -0.2498,
        -0.1679,
        -0.08824,
        -0.01231,
        -0.2687
      };
    }

    internal static List<double> XyzInKiloNewton() {
      return new List<double>() {
        114.90,
        111.20,
        96.59,
        84.26,
        71.78,
        59.20,
        46.40,
        33.31,
        20.61,
        7.186
      };
    }

    internal static List<double> XxInKiloNewtonMeter() {
      return new List<double>() {
        -0.435,
        -0.5364,
        -0.5231,
        -0.1825,
        -0.3465,
        -0.2286,
        -0.1017,
        -0.2824,
        4.18E-04,
        -0.01825
      };
    }

    internal static List<double> YyInKiloNewtonMeter() {
      return new List<double>() {
        77.5,
        75.62,
        66.15,
        57.87,
        49.3,
        40.63,
        31.75,
        22.5,
        13.94,
        4.907
      };
    }

    internal static List<double> ZzInKiloNewtonMeter() {
      return new List<double>() {
        36.01,
        35.32,
        30.44,
        25.69,
        21.94,
        18.17,
        14.47,
        10.94,
        6.712,
        2.366
      };
    }

    internal static List<double> XxyyzzInKiloNewtonMeter() {
      return new List<double>() {
        85.46,
        83.46,
        72.82,
        63.32,
        53.97,
        44.51,
        34.89,
        25.02,
        15.47,
        5.448
      };
    }
  }
}
