using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class Element1dDisplacementsC4p2 {
    // these are regression tests, the values are taken directly from GSA results
    // "Steel_Design_Complex.gwb" for elements 24 to 30 for combination case C4p2
    internal static List<double> XInMillimeter() {
      return new List<double>() {
        2.298,
2.598,
2.897,
3.197,
4.451,
4.458,
4.466,
4.473,
3.197,
3.500,
3.804,
4.108,
4.345,
4.351,
4.356,
4.361,
4.108,
4.390,
4.672,
4.955
      };
    }

    internal static List<double> YInMillimeter() {
      return new List<double>() {
        4.425,
4.441,
4.456,
4.451,
-3.197,
-3.233,
-3.276,
-3.313,
4.451,
4.424,
4.390,
4.345,
-4.108,
-4.154,
-4.194,
-4.233,
4.345,
4.285,
4.215,
4.145
      };
    }

    internal static List<double> ZInMillimeter() {
      return new List<double>() {
        -84.86,
-86.77,
-88.36,
-89.37,
-89.37,
-90.47,
-91.68,
-92.88,
-89.37,
-89.65,
-89.45,
-88.88,
-88.88,
-89.76,
-90.71,
-91.65,
-88.88,
-87.65,
-85.86,
-83.91
      };
    }

    internal static List<double> XyzInMillimeter() {
      return new List<double>() {
       85.00,
86.93,
88.52,
89.54,
89.54,
90.64,
91.85,
93.04,
89.54,
89.83,
89.64,
89.08,
89.08,
89.96,
90.91,
91.85,
89.08,
87.87,
86.09,
84.16
      };
    }

    internal static List<double> XxInRadian() {
      return new List<double>() {
        -0.001909,
-0.001767,
-0.001625,
-0.001483,
882.9E-6,
772.9E-6,
662.9E-6,
552.9E-6,
-0.001483,
-0.001386,
-0.001290,
-0.001193,
-0.001040,
-0.001126,
-0.001212,
-0.001298,
-0.001193,
-0.001127,
-0.001061,
-995.3E-6
      };
    }

    internal static List<double> YyInRadian() {
      return new List<double>() {
        0.003008,
0.002681,
0.002004,
975.1E-6,
0.001527,
0.001763,
0.001830,
0.001727,
840.2E-6,
36.31E-6,
-604.8E-6,
-0.001083,
0.001225,
0.001391,
0.001435,
0.001357,
-0.001205,
-0.002371,
-0.002912,
-0.002827
      };
    }

    internal static List<double> ZzInRadian() {
      return new List<double>() {
        14.79E-6,
28.21E-6,
12.50E-6,
-32.35E-6,
-44.74E-6,
-62.70E-6,
-62.59E-6,
-44.42E-6,
-38.39E-6,
-44.93E-6,
-57.59E-6,
-76.38E-6,
-76.33E-6,
-64.15E-6,
-58.26E-6,
-58.66E-6,
-81.02E-6,
-99.53E-6,
-106.5E-6,
-101.8E-6
      };
    }

    internal static List<double> XxyyzzInRadian() {
      return new List<double>() {
       0.003562,
0.003211,
0.002580,
0.001775,
0.001765,
0.001926,
0.001947,
0.001814,
0.001705,
0.001387,
0.001426,
0.001613,
0.001609,
0.001791,
0.001879,
0.001878,
0.001697,
0.002627,
0.003101,
0.002999
      };
    }
  }
}
