using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class Element1dDerivedStressC4p1 {
    // these are regression tests, the values are taken directly from GSA results
    // "Steel_Design_Complex.gwb" for elements 2 to 3 for combination case C4p1
    internal static List<double> SEyInMPa() {
      return new List<double>() {
        9.077,
9.077,
9.077,
9.077,
-9.077,
-9.077,
-9.077,
-9.077
      };
    }

    internal static List<double> SEzInMPa() {
      return new List<double>() {
        30.08,
30.08,
30.08,
30.08,
30.08,
30.08,
30.08,
30.08
      };
    }

    internal static List<double> StInMPa() {
      return new List<double>() {
        11.24,
11.24,
11.24,
11.24,
-11.24,
-11.24,
-11.24,
-11.24
      };
    }

    internal static List<double> VonMisesInMPa() {
      return new List<double>() {
        192.6,
100.8,
106.7,
198.4,
192.6,
100.8,
106.7,
198.4
      };
    }
  }
}
