using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class Element1dStrainEnergyDensity {
    // these are regression tests, the values are taken directly from GSA results
    // "Steel_Design_Complex.gwb" for elements 2 to 3
    internal static List<double> A1EnergyInkJ() {
      return new List<double>() {
        21.85,
8.145,
3.579,
8.156,
21.88,
16.00,
5.939,
3.050,
7.336,
18.80
      };
    }

    internal static List<double> C4p1EnergyInkJ() {
      return new List<double>() {
       29.27,
11.02,
5.633,
13.10,
33.43,
29.27,
11.02,
5.633,
13.10,
33.43
      };
    }

    internal static List<double> C4p2EnergyInkJ() {
      return new List<double>() {
       25.41,
9.512,
4.535,
10.48,
27.34,
22.14,
8.284,
4.242,
10.01,
25.59
      };
    }
  }
}
