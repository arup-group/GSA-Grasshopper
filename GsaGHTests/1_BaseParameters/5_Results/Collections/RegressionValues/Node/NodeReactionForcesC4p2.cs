using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class NodeReactionForcesC4p2 {
    // these are regression tests, the values are taken directly from GSA results
    // "Steel_Design_Complex.gwb" for nodes 1324 to 1327 for combination case C4p2
    internal static List<double?> XInKiloNewtons() {
      return new List<double?>() {
        2123,
        -1148,
        423.3 - 1398,
      };
    }

    internal static List<double?> YInKiloNewtons() {
      return new List<double?>() {
        2158,
        394.9 - 1184,
        -1368,
      };
    }

    internal static List<double?> ZInKiloNewtons() {
      return new List<double?>() {
        7325,
        8408,
        13200,
        4112,
      };
    }

    internal static List<double?> XyzInKiloNewtons() {
      return new List<double?>() {
        7926,
        8496,
        13260,
        4554,
      };
    }

    internal static List<double?> XxInKiloNewtonsPerMeter() {
      return new List<double?>() {
        -82.10,
        null,
        null,
        358.5,
      };
    }

    internal static List<double?> YyInKiloNewtonsPerMeter() {
      return new List<double?>() {
        278.5,
        null,
        null,
        -162.1,
      };
    }

    internal static List<double?> ZzInKiloNewtonsPerMeter() {
      return new List<double?>() {
        -1.240,
        null,
        null,
        1.249,
      };
    }

    internal static List<double?> XxyyzzInKiloNewtonsPerMeter() {
      return new List<double?>() {
        290.3,
        null,
        null,
        239.5,
      };
    }
  }
}
