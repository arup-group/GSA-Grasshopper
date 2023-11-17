using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class Element1dDerivedStressA1 {
    // these are regression tests, the values are taken directly from GSA results
    // "Steel_Design_Complex.gwb" for elements 2 to 3 for analysis case A1
    internal static List<double> SEyInMPa() {
      return new List<double>() {
        8.122,
8.122,
8.122,
8.122,
-7.076,
-7.076,
-7.076,
-7.076
      };
    }

    internal static List<double> SEzInMPa() {
      return new List<double>() {
        25.22,
25.22,
25.22,
25.22,
22.38,
22.38,
22.38,
22.38
      };
    }

    internal static List<double> StInMPa() {
      return new List<double>() {
        8.671,
8.671,
8.671,
8.671,
-7.972,
-7.972,
-7.972,
-7.972
      };
    }

    internal static List<double> VonMisesInMPa() {
      return new List<double>() {
        165.9,
86.51,
83.17,
159.9,
142.3,
73.33,
80.20,
149.8
      };
    }
  }
}
