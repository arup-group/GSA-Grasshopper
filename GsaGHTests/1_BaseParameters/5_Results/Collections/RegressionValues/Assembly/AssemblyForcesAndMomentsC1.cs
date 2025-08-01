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
        -59.20,
        -46.40,
        -33.31,
        -20.63,
        -7.182,
      };
    }

    internal static List<double> YInKiloNewton() {
      return new List<double>() {
        -2.873,
        -1.643,
        -0.6729,
        -0.4921,
        -0.4347,
        -0.3019,
        -0.1875,
        -0.09167,
        -0.06605,
        0.1182,
      };
    }

    internal static List<double> ZInKiloNewton() {
      return new List<double>() {
        4.530,
        1.829,
        -0.1974,
        -0.3925,
        -0.2645,
        -0.2496,
        -0.1670,
        -0.08497,
        0.001116,
        -0.2582,
      };
    }

    internal static List<double> YzInKiloNewton() {
      return new List<double>() {
        5.375,
        2.471,
        0.6996,
        0.6293,
        0.5088,
        0.3918,
        0.2514,
        0.1261,
        0.06606,
        0.2839,
      };
    }

    internal static List<double> XxInKiloNewtonMeter() {
      return new List<double>() {
        -0.4309,
        -0.5430,
        -0.5256,
        -0.1830,
        -0.3466,
        -0.2286,
        -0.1017,
        -0.2824,
        574.5E-6,
        -0.01808,
      };
    }

    internal static List<double> YyInKiloNewtonMeter() {
      return new List<double>() {
        77.55,
        75.61,
        66.15,
        57.87,
        49.30,
        40.63,
        31.75,
        22.50,
        13.95,
        4.908,
      };
    }

    internal static List<double> ZzInKiloNewtonMeter() {
      return new List<double>() {
        35.98,
        35.33,
        30.44,
        25.69,
        21.94,
        18.17,
        14.47,
        10.94,
        6.717,
        2.367,
      };
    }

    internal static List<double> YyzzInKiloNewtonMeter() {
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
