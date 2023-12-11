using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class Element1dStressC4p2 {
    // these are regression tests, the values are taken directly from GSA results
    // "Steel_Design_Complex.gwb" for elements 2 to 3 for combination case C4p2
    internal static List<double> AxialInMPa() {
      return new List<double>() {
        -24.24,
-24.24,
-24.24,
-24.24,
-24.05,
-24.05,
-24.05,
-24.05
      };
    }

    internal static List<double> SyInMPa() {
      return new List<double>() {
        7.724,
7.724,
7.724,
7.724,
-7.254,
-7.254,
-7.254,
-7.254
      };
    }

    internal static List<double> SzInMPa() {
      return new List<double>() {
        24.84,
24.84,
24.84,
24.84,
23.56,
23.56,
23.56,
23.56
      };
    }

    internal static List<double> ByPosInMPa() {
      return new List<double>() {
        -112.1,
-35.43,
41.28,
118.0,
-104.3,
-31.48,
41.30,
114.1
      };
    }

    internal static List<double> ByNegInMPa() {
      return new List<double>() {
        112.1,
35.43,
-41.27,
-118.0,
104.2,
31.48,
-41.29,
-114.1
      };
    }

    internal static List<double> BzPosInMPa() {
      return new List<double>() {
        -36.20,
-12.34,
11.51,
35.37,
32.48,
10.08,
-12.33,
-34.73
      };
    }

    internal static List<double> BzNegInMPa() {
      return new List<double>() {
        36.20,
12.34,
-11.52,
-35.37,
-32.49,
-10.08,
12.33,
34.73
      };
    }

    internal static List<double> C1InMPa() {
      return new List<double>() {
        120.8,
22.41,
27.48,
125.9,
109.7,
16.58,
28.44,
121.6
      };
    }

    internal static List<double> C2InMPa() {
      return new List<double>() {
        -169.3,
-70.89,
-75.96,
-174.3,
-157.8,
-64.69,
-76.53,
-169.6
      };
    }
  }
}
