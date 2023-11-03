using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class Element1dForcesAndMomentsA1 {
    // these are regression tests, the values are taken directly from GSA results
    // "Steel_Design_Complex.gwb" for elements 2 to 6 for analysis case A1 with 5 positions
    internal static List<double> XInKiloNewton() {
      return new List<double>() {
        -343.6,
-343.6,
-343.6,
-343.6,
-343.6,
-337.2,
-337.2,
-337.2,
-337.2,
-337.2,
-187.9,
-187.9,
-187.9,
-187.9,
-187.9,
-482.2,
-482.2,
-482.2,
-482.2,
-482.2
      };
    }

    internal static List<double> YInKiloNewton() {
      return new List<double>() {
        60.91,
60.91,
60.91,
60.91,
60.91,
-53.07,
-53.07,
-53.07,
-53.07,
-53.07,
47.15,
47.15,
47.15,
47.15,
47.15,
20.25,
20.25,
20.25,
20.25,
20.25,
      };
    }

    internal static List<double> ZInKiloNewton() {
      return new List<double>() {
        189.1,
189.1,
189.1,
189.1,
189.1,
167.9,
167.9,
167.9,
167.9,
167.9,
93.54,
93.54,
93.54,
93.54,
93.54,
110.5,
110.5,
110.5,
110.5,
110.5
      };
    }

    internal static List<double> XyzInKiloNewton() {
      return new List<double>() {
        198.7,
198.7,
198.7,
198.7,
198.7,
176.0,
176.0,
176.0,
176.0,
176.0,
104.8,
104.8,
104.8,
104.8,
104.8,
112.3,
112.3,
112.3,
112.3,
112.3
      };
    }

    internal static List<double> XxInKiloNewtonMeter() {
      return new List<double>() {
        23.01,
23.01,
23.01,
23.01,
23.01,
-21.16,
-21.16,
-21.16,
-21.16,
-21.16,
17.18,
17.18,
17.18,
17.18,
17.18,
-1.283,
-1.283,
-1.283,
-1.283,
-1.283,
      };
    }

    internal static List<double> YyInKiloNewtonMeter() {
      return new List<double>() {
        -188.3,
-93.71,
0.8640,
95.44,
190.0,
-159.8,
-75.90,
8.025,
91.95,
175.9,
-97.00,
-50.23,
-3.458,
43.31,
90.08,
-32.27,
22.98,
78.23,
133.5,
188.7
      };
    }

    internal static List<double> ZzInKiloNewtonMeter() {
      return new List<double>() {
        63.40,
32.94,
2.483,
-27.97,
-58.43,
-50.02,
-23.48,
3.056,
29.59,
56.13,
46.59,
23.02,
-0.5521,
-24.13,
-47.70,
11.60,
1.471,
-8.655,
-18.78,
-28.91
      };
    }

    internal static List<double> XxyyzzInKiloNewtonMeter() {
      return new List<double>() {
        198.7,
99.33,
2.629,
99.45,
198.8,
167.5,
79.45,
8.587,
96.60,
184.6,
107.6,
55.25,
3.502,
49.58,
101.9,
34.29,
23.03,
78.71,
134.8,
190.9
      };
    }
  }
}
