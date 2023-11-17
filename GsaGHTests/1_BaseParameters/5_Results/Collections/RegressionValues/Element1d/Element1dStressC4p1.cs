using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class Element1dStressC4p1 {
    // these are regression tests, the values are taken directly from GSA results
    // "Steel_Design_Complex.gwb" for elements 2 to 3 for combination case C4p1
    internal static List<double> AxialInMPa() {
      return new List<double>() {
        -27.91,
-27.91,
-27.91,
-27.91,
-27.91,
-27.91,
-27.91,
-27.91
      };
    }

    internal static List<double> SyInMPa() {
      return new List<double>() {
        8.153,
8.153,
8.153,
8.153,
-8.153,
-8.153,
-8.153,
-8.153
      };
    }

    internal static List<double> SzInMPa() {
      return new List<double>() {
        27.02,
27.02,
27.02,
27.02,
27.02,
27.02,
27.02,
27.02
      };
    }

    internal static List<double> ByPosInMPa() {
      return new List<double>() {
        -119.8,
-36.36,
47.09,
130.6,
-119.8,
-36.36,
47.09,
130.6
      };
    }

    internal static List<double> ByNegInMPa() {
      return new List<double>() {
        119.8,
36.36,
-47.09,
-130.5,
119.8,
36.36,
-47.09,
-130.5
      };
    }

    internal static List<double> BzPosInMPa() {
      return new List<double>() {
        -37.22,
-12.04,
13.14,
38.32,
37.22,
12.04,
-13.14,
-38.32
      };
    }

    internal static List<double> BzNegInMPa() {
      return new List<double>() {
        37.22,
12.04,
-13.14,
-38.32,
-37.22,
-12.04,
13.14,
38.32
      };
    }

    internal static List<double> C1InMPa() {
      return new List<double>() {
        125.7,
19.39,
31.11,
137.4,
125.7,
19.39,
31.11,
137.4
      };
    }

    internal static List<double> C2InMPa() {
      return new List<double>() {
        -181.5,
-75.21,
-86.92,
-193.2,
-181.6,
-75.22,
-86.92,
-193.2
      };
    }
  }
}
