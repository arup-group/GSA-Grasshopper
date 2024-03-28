using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class NodeReactionForcesC4p1 {
    // these are regression tests, the values are taken directly from GSA results
    // "Steel_Design_Complex.gwb" for nodes 1324 to 1327 for combination case C4p1
    internal static List<double?> XInKiloNewtons() {
      return new List<double?>() {
        2351,
        -1051,
        1051,
        -2351,
      };
    }

    internal static List<double?> YInKiloNewtons() {
      return new List<double?>() {
        2351,
        1051,
        -1051,
        -2351,
      };
    }

    internal static List<double?> ZInKiloNewtons() {
      return new List<double?>() {
        7631,
        14420,
        14420,
        7631,
      };
    }

    internal static List<double?> YzInKiloNewtons() {
      return new List<double?>() {
        8324,
        14500,
        14500,
        8324,
      };
    }

    internal static List<double?> XxInKiloNewtonsPerMeter() {
      return new List<double?>() {
        -294.1,
        null,
        null,
        294.1,
      };
    }

    internal static List<double?> YyInKiloNewtonsPerMeter() {
      return new List<double?>() {
        294.1,
        null,
        null,
        -294.1,
      };
    }

    internal static List<double?> ZzInKiloNewtonsPerMeter() {
      return new List<double?>() {
        2.930E-6,
        null,
        null,
        -2.441E-6,
      };
    }

    internal static List<double?> XxyyzzInKiloNewtonsPerMeter() {
      return new List<double?>() {
        415.9,
        null,
        null,
        415.9,
      };
    }
  }
}
