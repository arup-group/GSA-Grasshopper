using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class Element1dAverageStrainEnergyDensity {
    // these are regression tests, the values are taken directly from GSA results
    // "Steel_Design_Complex.gwb" for elements 2 to 38
    internal static List<double> A1EnergyInkJ() {
      return new List<double>() {
        10.44,
8.431,
3.041,
7.776,
6.091,
56.86,
0.5602,
28.05,
0.1944,
23.09,
0.4344,
20.90,
0.4773,
21.01,
0.3543,
21.43,
0.2165,
20.90,
0.1287,
19.13,
0.1202,
15.03,
0.02973,
10.78,
0.02866
      };
    }

    internal static List<double> C4p1EnergyInkJ() {
      return new List<double>() {
       15.28,
15.28,
4.637,
12.78,
7.629,
67.74,
1.319,
32.99,
0.3450,
26.51,
0.4352,
23.58,
0.4726,
24.03,
0.3515,
25.35,
0.2086,
25.71,
0.09512,
24.32,
0.03329,
21.26,
0.08649,
17.09,
0.4206
      };
    }

    internal static List<double> C4p2EnergyInkJ() {
      return new List<double>() {
       12.73,
11.60,
3.796,
10.11,
6.809,
62.05,
0.8827,
30.38,
0.2415,
24.71,
0.4155,
22.18,
0.4579,
22.47,
0.3388,
23.33,
0.2028,
23.21,
0.1054,
21.57,
0.05665,
17.98,
0.03421,
13.74,
0.1543
      };
    }
  }
}
