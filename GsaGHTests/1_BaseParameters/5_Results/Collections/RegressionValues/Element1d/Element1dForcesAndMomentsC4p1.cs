using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class Element1dForcesAndMomentsC4p1 {
    // these are regression tests, the values are taken directly from GSA results
    // "Steel_Design_Complex.gwb" for elements 2 to 6 for analysis case C4p1 with 5 positions
    internal static List<double> XInKiloNewton() {
      return new List<double>() {
        -466.1,
-466.1,
-466.1,
-466.1,
-466.1,
-466.1,
-466.1,
-466.1,
-466.1,
-466.1,
-243.6,
-243.6,
-243.6,
-243.6,
-243.6,
-680.2,
-680.2,
-680.2,
-680.2,
-680.2
      };
    }

    internal static List<double> YInKiloNewton() {
      return new List<double>() {
        68.07,
68.07,
68.07,
68.07,
68.07,
-68.07,
-68.07,
-68.07,
-68.07,
-68.07,
54.32,
54.32,
54.32,
54.32,
54.32,
31.28,
31.28,
31.28,
31.28,
31.28,
      };
    }

    internal static List<double> ZInKiloNewton() {
      return new List<double>() {
        225.6,
225.6,
225.6,
225.6,
225.6,
225.6,
225.6,
225.6,
225.6,
225.6,
116.2,
116.2,
116.2,
116.2,
116.2,
143.1,
143.1,
143.1,
143.1,
143.1
      };
    }

    internal static List<double> YzInKiloNewton() {
      return new List<double>() {
        235.7,
235.7,
235.7,
235.7,
235.7,
235.7,
235.7,
235.7,
235.7,
235.7,
128.3,
128.3,
128.3,
128.3,
128.3,
146.5,
146.5,
146.5,
146.5,
146.5
      };
    }

    internal static List<double> XxInKiloNewtonMeter() {
      return new List<double>() {
        29.82,
29.82,
29.82,
29.82,
29.82,
-29.82,
-29.82,
-29.82,
-29.82,
-29.82,
21.67,
21.67,
21.67,
21.67,
21.67,
-1.555,
-1.555,
-1.555,
-1.555,
-1.555
      };
    }

    internal static List<double> YyInKiloNewtonMeter() {
      return new List<double>() {
        -215.9,
-103.1,
9.669,
122.5,
235.3,
-215.9,
-103.1,
9.669,
122.5,
235.3,
-120.2,
-62.09,
-3.984,
54.12,
112.2,
-53.15,
18.40,
89.94,
161.5,
233.0
      };
    }

    internal static List<double> ZzInKiloNewtonMeter() {
      return new List<double>() {
        67.08,
33.04,
-0.9938,
-35.03,
-69.07,
-67.08,
-33.04,
0.9938,
35.03,
69.07,
53.32,
26.17,
-0.9938,
-28.15,
-55.31,
16.32,
0.6808,
-14.96,
-30.60,
-46.24
      };
    }

    internal static List<double> YyzzInKiloNewtonMeter() {
      return new List<double>() {
        226.1,
108.3,
9.720,
127.4,
245.2,
226.1,
108.3,
9.720,
127.4,
245.2,
131.5,
67.37,
4.106,
61.00,
125.1,
55.60,
18.41,
91.18,
164.4,
237.6
      };
    }
  }
}
