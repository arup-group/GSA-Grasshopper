using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class NodeSpringForcesC2p2 {
    // these are regression tests, the values are taken directly from GSA results
    // "Spring Forces and Moments Output.gwb" for nodes 1 to 4 for combination case C2p2
    internal static List<double> XInKiloNewtons() {
      return new List<double>() {
        0,
        0,
        0,
        0,
      };
    }

    internal static List<double> YInKiloNewtons() {
      return new List<double>() {
        0,
        0,
        0,
        0,
      };
    }

    internal static List<double> ZInKiloNewtons() {
      return new List<double>() {
        -148900,
        -148900,
        -148900,
        -148900,
      };
    }

    internal static List<double> XyzInKiloNewtons() {
      return new List<double>() {
        148900,
        148900,
        148900,
        148900,
      };
    }

    internal static List<double> XxInKiloNewtonsPerMeter() {
      return new List<double>() {
        -0.8565,
        -0.8565,
        0.8565,
        0.8565,
      };
    }

    internal static List<double> YyInKiloNewtonsPerMeter() {
      return new List<double>() {
        0.8565,
        -0.8565,
        -0.8565,
        0.8565,
      };
    }

    internal static List<double> ZzInKiloNewtonsPerMeter() {
      return new List<double>() {
        0,
        0,
        0,
        0,
      };
    }

    internal static List<double> XxyyzzInKiloNewtonsPerMeter() {
      return new List<double>() {
        1.211,
        1.211,
        1.211,
        1.211,
      };
    }
  }
}
