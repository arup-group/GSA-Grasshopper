using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class Element1dStressA1 {
    // these are regression tests, the values are taken directly from GSA results
    // "Steel_Design_Complex.gwb" for elements 2 to 3 for analysis case A1
    internal static List<double> AxialInMPa() {
      return new List<double>() {
        -20.57,
-20.57,
-20.57,
-20.57,
-20.19,
-20.19,
-20.19,
-20.19
      };
    }

    internal static List<double> SyInMPa() {
      return new List<double>() {
        7.295,
7.295,
7.295,
7.295,
-6.356,
-6.356,
-6.356,
-6.356
      };
    }

    internal static List<double> SzInMPa() {
      return new List<double>() {
        22.65,
22.65,
22.65,
22.65,
20.10,
20.10,
20.10,
20.10
      };
    }

    internal static List<double> ByPosInMPa() {
      return new List<double>() {
        -104.5,
-34.50,
35.46,
105.4,
-88.69,
-26.59,
35.50,
97.59
      };
    }

    internal static List<double> ByNegInMPa() {
      return new List<double>() {
        104.5,
34.50,
-35.46,
-105.4,
88.68,
26.59,
-35.50,
-97.58
      };
    }

    internal static List<double> BzPosInMPa() {
      return new List<double>() {
        -35.17,
-12.64,
9.888,
32.42,
27.75,
8.119,
-11.51,
-31.14
      };
    }

    internal static List<double> BzNegInMPa() {
      return new List<double>() {
        35.18,
12.64,
-9.889,
-32.42,
-27.75,
-8.120,
11.51,
31.14
      };
    }

    internal static List<double> C1InMPa() {
      return new List<double>() {
        115.9,
25.44,
23.86,
114.3,
93.68,
13.77,
25.77,
105.7
      };
    }

    internal static List<double> C2InMPa() {
      return new List<double>() {
        -157.0,
-66.59,
-65.00,
-155.4,
-134.1,
-54.16,
-66.14,
-146.1
      };
    }
  }
}
