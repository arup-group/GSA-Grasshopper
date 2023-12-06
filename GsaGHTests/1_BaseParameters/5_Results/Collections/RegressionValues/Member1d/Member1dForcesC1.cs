using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class Member1dForcesC1 {
    // these are regression tests, the values are taken directly from GSA results
    // "Steel_Design_Simplex.gwb" for all members for analysis case C1 with 5 positions
    internal static List<double> XInKiloNewton() {
      return new List<double>() {
        0,
        0,
        0,
        0,
        0,
      };
    }

    internal static List<double> YInKiloNewton() {
      return new List<double>() {
        0,
        0,
        0,
        0,
        0,
      };
    }

    internal static List<double> ZInKiloNewton() {
      return new List<double>() {
        -76.50,
-50.25,
-24.00,
50.25,
76.50
      };
    }

    internal static List<double> XyzInKiloNewton() {
      return new List<double>() {
        76.50,
50.25,
24.00,
50.25,
76.50
      };
    }

    internal static List<double> XxInKiloNewtonMeter() {
      return new List<double>() {
        0,
        0,
        0,
        0,
        0,
      };
    }

    internal static List<double> YyInKiloNewtonMeter() {
      return new List<double>() {
        -1.112E-6,
-118.8,
-188.4,
-118.8,
-1.112E-6
      };
    }

    internal static List<double> ZzInKiloNewtonMeter() {
      return new List<double>() {
        0,
        0,
        0,
        0,
        0,
      };
    }

    internal static List<double> XxyyzzInKiloNewtonMeter() {
      return new List<double>() {
        1.112E-6,
118.8,
188.4,
118.8,
1.112E-6
      };
    }
  }
}
