using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class Member1dForcesA1 {
    // these are regression tests, the values are taken directly from GSA results
    // "Steel_Design_Simplex.gwb" for all members for analysis case A1 with 5 positions
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
        -37.50,
-18.75,
0.0,
18.75,
37.50
      };
    }

    internal static List<double> YzInKiloNewton() {
      return new List<double>() {
        37.50,
18.75,
0.0,
18.75,
37.50
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
        236.7E-9,
-52.73,
-70.31,
-52.73,
236.7E-9
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

    internal static List<double> YyzzInKiloNewtonMeter() {
      return new List<double>() {
        236.7E-9,
52.73,
70.31,
52.73,
236.7E-9
      };
    }
  }
}
