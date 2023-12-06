using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class Element1dDerivedStressC4p2 {
    // these are regression tests, the values are taken directly from GSA results
    // "Steel_Design_Complex.gwb" for elements 2 to 3 for combination case C4p2
    internal static List<double> SEyInMPa() {
      return new List<double>() {
        8.599,
8.599,
8.599,
8.599,
-8.076,
-8.076,
-8.076,
-8.076
      };
    }

    internal static List<double> SEzInMPa() {
      return new List<double>() {
        27.65,
27.65,
27.65,
27.65,
26.23,
26.23,
26.23,
26.23
      };
    }

    internal static List<double> StInMPa() {
      return new List<double>() {
        9.954,
9.954,
9.954,
9.954,
-9.604,
-9.604,
-9.604,
-9.604
      };
    }

    internal static List<double> VonMisesInMPa() {
      return new List<double>() {
        179.3,
93.64,
94.91,
179.1,
167.5,
87.07,
93.46,
174.1
      };
    }
  }
}
