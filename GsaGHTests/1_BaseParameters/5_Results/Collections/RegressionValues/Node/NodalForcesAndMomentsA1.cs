using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class NodalForcesAndMomentsA1 {
    // these are regression tests, the values are taken directly from GSA results
    // "Steel_Design_Complex.gwb" for node 442 for analysis case A1
    internal static List<double> XInKiloNewtons() {
      return new List<double>() {
        60.910,
        337.200,
        -177.800,
        -220.400
      };
    }

    internal static List<double> YInKiloNewtons() {
      return new List<double>() {
        343.600,
        53.070,
        -194.300,
        -202.400
      };
    }

    internal static List<double> ZInKiloNewtons() {
      return new List<double>() {
        -189.100,
        -167.900,
        -274.000,
        631.000
      };
    }

    internal static List<double> XyzInKiloNewtons() {
      return new List<double>() {
        396.900,
        380.400,
        380.100,
        698.400
      };
    }

    internal static List<double> XxInKiloNewtonsPerMeter() {
      return new List<double>() {
        -188.300,
        21.160,
        202.900,
        -35.760
      };
    }

    internal static List<double> YyInKiloNewtonsPerMeter() {
      return new List<double>() {
        -23.010,
        159.800,
        -189.700,
        52.9
      };
    }

    internal static List<double> ZzInKiloNewtonsPerMeter() {
      return new List<double>() {
        -63.400,
        50.020,
        -0.8961,
        14.280
      };
    }

    internal static List<double> XxyyzzInKiloNewtonsPerMeter() {
      return new List<double>() {
        200.000,
        168.800,
        277.700,
        65.390
      };
    }
  }
}
