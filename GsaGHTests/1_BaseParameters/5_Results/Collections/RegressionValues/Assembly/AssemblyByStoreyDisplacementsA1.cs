using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class AssemblyByStoreyDisplacementsA1 {
    // these are regression tests, the values are taken directly from GSA results
    // "assembly-simple.gwb" for Assembly 2 for analysis case A1
    internal static List<double> XInMillimeter() {
      return new List<double>() {
        0.01373,
        0.0822,
        0.07943,
        0.1596,
        0.1552,
        0.2258
      };
    }

    internal static List<double> YInMillimeter() {
      return new List<double>() {
        -7.06E-04,
        -0.004427,
        -0.004281,
        -0.01704,
        -0.01699,
        -0.03639
      };
    }

    internal static List<double> ZInMillimeter() {
      return new List<double>() {
        -0.01436,
        -0.05438,
        -0.0553,
        -0.08472,
        -0.08521,
        -0.1014
      };
    }

    internal static List<double> XyzInMillimeter() {
      return new List<double>() {
        0.01988,
        0.09866,
        0.09688,
        0.1815,
        0.1778,
        0.2502
      };
    }

    internal static List<double> XxInRadian() {
      return new List<double>() {
        6.03E-07,
        2.08E-06,
        2.30E-06,
        3.42E-06,
        3.60E-06,
        4.24E-06
      };
    }

    internal static List<double> YyInRadian() {
      return new List<double>() {
        3.26E-06,
        1.07E-05,
        1.05E-05,
        1.39E-05,
        1.37E-05,
        1.44E-05
      };
    }

    internal static List<double> ZzInRadian() {
      return new List<double>() {
        -4.72E-07,
        -2.94E-06,
        -2.16E-06,
        -4.25E-06,
        -2.96E-06,
        -4.08E-06
      };
    }

    internal static List<double> XxyyzzInRadian() {
      return new List<double>() {
        3.35E-06,
        1.13E-05,
        1.10E-05,
        1.49E-05,
        1.45E-05,
        1.55E-05
      };
    }
  }
}
