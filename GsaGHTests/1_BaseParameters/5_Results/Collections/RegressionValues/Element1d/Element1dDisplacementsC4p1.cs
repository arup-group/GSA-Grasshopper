using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class Element1dDisplacementsC4p1 {
    // these are regression tests, the values are taken directly from GSA results
    // "Steel_Design_Complex.gwb" for nodes 442 to 468 for combination case C4p1
    internal static List<double> XInMillimeter() {
      return new List<double>() {
        2.674,
2.985,
3.296,
3.607,
-0.9809,
-0.9736,
-0.9663,
-0.9590,
3.607,
3.928,
4.248,
4.569,
-1.299,
-1.294,
-1.290,
-1.286,
4.569,
4.873,
5.176,
5.480
      };
    }

    internal static List<double> YInMillimeter() {
      return new List<double>() {
        -0.7855,
-0.8397,
-0.9050,
-0.9809,
-3.607,
-3.673,
-3.726,
-3.783,
-0.9809,
-1.077,
-1.187,
-1.299,
-4.569,
-4.648,
-4.707,
-4.772,
-1.299,
-1.423,
-1.561,
-1.692
      };
    }

    internal static List<double> ZInMillimeter() {
      return new List<double>() {
        -95.37,
-97.84,
-99.99,
-101.5,
-101.5,
-103.0,
-104.7,
-106.3,
-101.5,
-102.3,
-102.6,
-102.5,
-102.5,
-103.8,
-105.2,
-106.6,
-102.5,
-101.7,
-100.4,
-98.78
      };
    }

    internal static List<double> XyzInMillimeter() {
      return new List<double>() {
        95.41,
97.89,
100.0,
101.6,
101.6,
103.1,
104.7,
106.4,
101.6,
102.4,
102.7,
102.6,
102.6,
104.0,
105.4,
106.8,
102.6,
101.9,
100.5,
98.94
      };
    }

    internal static List<double> XxInRadian() {
      return new List<double>() {
        -0.002413,
-0.002306,
-0.002199,
-0.002093,
0.001596,
0.001505,
0.001413,
0.001322,
-0.002093,
-0.002024,
-0.001955,
-0.001886,
-412.4E-6,
-492.2E-6,
-572.1E-6,
-651.9E-6,
-0.001886,
-0.001834,
-0.001783,
-0.001732
      };
    }

    internal static List<double> YyInRadian() {
      return new List<double>() {
        0.003813,
0.003534,
0.002832,
0.001707,
0.002139,
0.002387,
0.002459,
0.002356,
0.001571,
804.4E-6,
135.0E-6,
-437.8E-6,
0.001914,
0.002069,
0.002117,
0.002057,
-556.3E-6,
-0.001705,
-0.002306,
-0.002358
      };
    }

    internal static List<double> ZzInRadian() {
      return new List<double>() {
        -72.83E-6,
-89.75E-6,
-106.0E-6,
-121.5E-6,
-115.1E-6,
-85.03E-6,
-78.59E-6,
-95.77E-6,
-126.4E-6,
-157.9E-6,
-169.9E-6,
-162.4E-6,
-147.1E-6,
-97.01E-6,
-86.06E-6,
-114.2E-6,
-165.4E-6,
-201.6E-6,
-206.9E-6,
-181.4E-6
      };
    }

    internal static List<double> XxyyzzInRadian() {
      return new List<double>() {
        0.004513,
0.004221,
0.003587,
0.002703,
0.002671,
0.002823,
0.002837,
0.002703,
0.002620,
0.002183,
0.001967,
0.001942,
0.001963,
0.002129,
0.002195,
0.002161,
0.001973,
0.002513,
0.002922,
0.002931
      };
    }
  }
}
