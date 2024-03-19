using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class Element1dForcesAndMomentsC4p2 {
    // these are regression tests, the values are taken directly from GSA results
    // "Steel_Design_Complex.gwb" for elements 2 to 6 for analysis case C4p2 with 5 positions
    internal static List<double> XInKiloNewton() {
      return new List<double>() {
        -404.8,
-404.8,
-404.8,
-404.8,
-404.8,
-401.6,
-401.6,
-401.6,
-401.6,
-401.6,
-215.7,
-215.7,
-215.7,
-215.7,
-215.7,
-581.2,
-581.2,
-581.2,
-581.2,
-581.2
      };
    }

    internal static List<double> YInKiloNewton() {
      return new List<double>() {
        64.49,
64.49,
64.49,
64.49,
64.49,
-60.57,
-60.57,
-60.57,
-60.57,
-60.57,
50.73,
50.73,
50.73,
50.73,
50.73,
25.77,
25.77,
25.77,
25.77,
25.77
      };
    }

    internal static List<double> ZInKiloNewton() {
      return new List<double>() {
        207.4,
207.4,
207.4,
207.4,
207.4,
196.7,
196.7,
196.7,
196.7,
196.7,
104.9,
104.9,
104.9,
104.9,
104.9,
126.8,
126.8,
126.8,
126.8,
126.8
      };
    }

    internal static List<double> YzInKiloNewton() {
      return new List<double>() {
        217.2,
217.2,
217.2,
217.2,
217.2,
205.8,
205.8,
205.8,
205.8,
205.8,
116.5,
116.5,
116.5,
116.5,
116.5,
129.4,
129.4,
129.4,
129.4,
129.4,
      };
    }

    internal static List<double> XxInKiloNewtonMeter() {
      return new List<double>() {
        26.42,
26.42,
26.42,
26.42,
26.42,
-25.49,
-25.49,
-25.49,
-25.49,
-25.49,
19.43,
19.43,
19.43,
19.43,
19.43,
-1.419,
-1.419,
-1.419,
-1.419,
-1.419,
      };
    }

    internal static List<double> YyInKiloNewtonMeter() {
      return new List<double>() {
        -202.1,
-98.42,
5.266,
109.0,
212.6,
-187.9,
-89.52,
8.847,
107.2,
205.6,
-108.6,
-56.16,
-3.721,
48.72,
101.2,
-42.71,
20.69,
84.09,
147.5,
210.9
      };
    }

    internal static List<double> ZzInKiloNewtonMeter() {
      return new List<double>() {
        65.24,
32.99,
0.7445,
-31.50,
-63.75,
-58.55,
-28.26,
2.025,
32.31,
62.60,
49.96,
24.59,
-0.7730,
-26.14,
-51.51,
13.96,
1.076,
-11.81,
-24.69,
-37.57,
      };
    }

    internal static List<double> YyzzInKiloNewtonMeter() {
      return new List<double>() {
        212.4,
103.8,
5.319,
113.4,
222.0,
196.8,
93.87,
9.076,
112.0,
214.9,
119.5,
61.31,
3.800,
55.29,
113.5,
44.93,
20.72,
84.91,
149.5,
214.2
      };
    }
  }
}
