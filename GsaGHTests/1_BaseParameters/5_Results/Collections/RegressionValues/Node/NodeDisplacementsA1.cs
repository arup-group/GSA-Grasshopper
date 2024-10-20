﻿using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class NodeDisplacementsA1 {
    // these are regression tests, the values are taken directly from GSA results
    // "Steel_Design_Complex.gwb" for nodes 442 to 468 for analysis case A1
    internal static List<double> XInMillimeter() {
      return new List<double>() {
        1.108,
0.9107,
0.6290,
0.2078,
0.1886,
0.5299,
1.140,
1.923,
2.786,
3.646,
4.429,
5.087,
5.601,
5.973,
6.214,
6.349,
6.409,
6.426,
6.423,
6.419,
6.417,
0.3396,
0.2390,
0.01895,
-0.1426,
0.01656,
0.4770
      };
    }

    internal static List<double> YInMillimeter() {
      return new List<double>() {
        10.27,
9.467,
9.513,
8.731,
8.510,
8.808,
9.253,
9.635,
9.883,
9.990,
9.983,
9.913,
9.797,
9.663,
9.507,
9.229,
8.746,
8.559,
8.498,
8.570,
8.800,
10.07,
9.357,
9.772,
8.715,
8.513,
8.833
      };
    }

    internal static List<double> ZInMillimeter() {
      return new List<double>() {
        4.700,
4.609,
1.052,
-16.58,
-36.44,
-53.62,
-66.46,
-74.35,
-77.21,
-75.25,
-69.05,
-60.16,
-49.70,
-38.47,
-27.14,
-16.26,
-6.207,
2.840,
11.03,
18.72,
26.23,
1.362,
-2.120,
-7.704,
-24.96,
-44.39,
-60.25,
      };
    }

    internal static List<double> XyzInMillimeter() {
      return new List<double>() {
        11.35,
10.57,
9.592,
18.74,
37.42,
54.35,
67.11,
74.99,
77.89,
76.00,
69.90,
61.18,
50.97,
40.11,
29.42,
19.75,
12.49,
11.07,
15.33,
21.56,
28.40,
10.16,
9.597,
12.44,
26.44,
45.20,
60.90
      };
    }

    internal static List<double> XxInRadian() {
      return new List<double>() {
        -0.002972,
-0.004069,
-0.004135,
-0.004172,
-0.003659,
-0.002896,
-0.002095,
-0.001404,
-872.6E-6,
-500.7E-6,
-258.7E-6,
-12.80E-6,
182.4E-6,
353.0E-6,
489.5E-6,
499.1E-6,
284.1E-6,
-14.73E-6,
-278.4E-6,
-439.8E-6,
-516.8E-6,
-0.002999,
-0.003962,
-0.003166,
-0.004169,
-0.004045,
-0.003267
      };
    }

    internal static List<double> YyInRadian() {
      return new List<double>() {
        0.001090,
0.001338,
0.003758,
0.007726,
0.007265,
0.005896,
0.004087,
0.002129,
169.9E-6,
-0.001669,
-0.003111,
-0.004044,
-0.004612,
-0.004887,
-0.004910,
-0.004734,
-0.004432,
-0.004114,
-0.003888,
-0.003780,
-0.003758,
0.002283,
0.002228,
0.004772,
0.007613,
0.007004,
0.005378
      };
    }

    internal static List<double> ZzInRadian() {
      return new List<double>() {
        -78.27E-6,
16.27E-6,
-251.5E-6,
-122.1E-6,
25.22E-6,
123.7E-6,
124.5E-6,
86.95E-6,
41.32E-6,
1.368E-6,
-24.23E-6,
-37.86E-6,
-43.57E-6,
-42.59E-6,
-56.57E-6,
-110.8E-6,
-80.44E-6,
7.425E-6,
48.68E-6,
102.6E-6,
136.0E-6,
-1.473E-6,
-806.3E-9,
-115.1E-6,
-208.4E-6,
-20.80E-6,
53.26E-6
      };
    }

    internal static List<double> XxyyzzInRadian() {
      return new List<double>() {
        0.003166,
0.004283,
0.005593,
0.008781,
0.008135,
0.006570,
0.004594,
0.002552,
890.0E-6,
0.001742,
0.003122,
0.004044,
0.004616,
0.004900,
0.004934,
0.004762,
0.004442,
0.004114,
0.003898,
0.003807,
0.003796,
0.003769,
0.004546,
0.005728,
0.008682,
0.008089,
0.006293
      };
    }
  }
}
