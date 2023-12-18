using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class NodeSpringForcesC2p1 {
    // these are regression tests, the values are taken directly from GSA results
    // "Spring Forces and Moments Output.gwb" for nodes 1 to 4 for combination case C2p1
    internal static List<double?> XInKiloNewtons() {
      return new List<double?>() {
        0,
        0,
        0,
        0,
      };
    }

    internal static List<double?> YInKiloNewtons() {
      return new List<double?>() {
        0,
        0,
        0,
        0,
      };
    }

    internal static List<double?> ZInKiloNewtons() {
      return new List<double?>() {
        -99290,
        -99290,
        -99290,
        -99290,
      };
    }

    internal static List<double?> XyzInKiloNewtons() {
      return new List<double?>() {
        99290,
        99290,
        99290,
        99290,
      };
    }

    internal static List<double?> XxInKiloNewtonsPerMeter() {
      return new List<double?>() {
        -0.5710,
        -0.5710,
        0.5710,
        0.5710,
      };
    }

    internal static List<double?> YyInKiloNewtonsPerMeter() {
      return new List<double?>() {
        0.5710,
        -0.5710,
        -0.5710,
        0.5710,
      };
    }

    internal static List<double?> ZzInKiloNewtonsPerMeter() {
      return new List<double?>() {
        0,
        0,
        0,
        0,
      };
    }

    internal static List<double?> XxyyzzInKiloNewtonsPerMeter() {
      return new List<double?>() {
        0.8075,
        0.8075,
        0.8075,
        0.8075,
      };
    }
  }
}
