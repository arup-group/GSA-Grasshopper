using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class NodeReactionForcesA1 {
    // these are regression tests, the values are taken directly from GSA results
    // "Steel_Design_Complex.gwb" for nodes 1324 to 1327 for analysis case A1
    internal static List<double?> XInKiloNewtons() {
      return new List<double?>() {
        1894,
        -1245,
        -204.1,
        -444.8,
      };
    }

    internal static List<double?> YInKiloNewtons() {
      return new List<double?>() {
        1964,
        -260.8,
        -1317,
        -385.5,
      };
    }

    internal static List<double?> ZInKiloNewtons() {
      return new List<double?>() {
        7019,
        2398,
        11990,
        593.4,
      };
    }

    internal static List<double?> YzInKiloNewtons() {
      return new List<double?>() {
        7530,
        2714,
        12060,
        835.8,
      };
    }

    internal static List<double?> XxInKiloNewtonsPerMeter() {
      return new List<double?>() {
        129.9,
        null,
        null,
        422.9,
      };
    }

    internal static List<double?> YyInKiloNewtonsPerMeter() {
      return new List<double?>() {
        262.8,
        null,
        null,
        -30.15,
      };
    }

    internal static List<double?> ZzInKiloNewtonsPerMeter() {
      return new List<double?>() {
        -2.481,
        null,
        null,
        2.498,
      };
    }

    internal static List<double?> XxyyzzInKiloNewtonsPerMeter() {
      return new List<double?>() {
        293.2,
        null,
        null,
        424.0,
      };
    }
  }
}
