using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class NodalForcesAndMomentsC1 {
    // these are regression tests, the values are taken directly from GSA results
    // "Steel_Design_Complex.gwb" for nodes 1324 to 1327 for combination case C1
    internal static List<double> XInKiloNewtons() {
      return new List<double>() {
        68.070,
        466.100,
        -242.900,
        -291.300
      };
    }

    internal static List<double> YInKiloNewtons() {
      return new List<double>() {
        466.100,
        68.070,
        -242.900,
        -291.300
      };
    }

    internal static List<double> ZInKiloNewtons() {
      return new List<double>() {
        -225.600,
        -225.600,
        -381.500,
        832.700
      };
    }

    internal static List<double> YzInKiloNewtons() {
      return new List<double>() {
        522.300,
        522.300,
        513.400,
        929.000
      };
    }

    internal static List<double> XxInKiloNewtonsPerMeter() {
      return new List<double>() {
        -215.900,
        29.820,
        259.200,
        -73.100
      };
    }

    internal static List<double> YyInKiloNewtonsPerMeter() {
      return new List<double>() {
        -29.820,
        215.900,
        -259.200,
        73.100
      };
    }

    internal static List<double> ZzInKiloNewtonsPerMeter() {
      return new List<double>() {
        -67.080,
        67.080,
        0,
        0.000005779
      };
    }

    internal static List<double> XxyyzzInKiloNewtonsPerMeter() {
      return new List<double>() {
        228.100,
        228.100,
        366.600,
        103.400
      };
    }
  }
}
