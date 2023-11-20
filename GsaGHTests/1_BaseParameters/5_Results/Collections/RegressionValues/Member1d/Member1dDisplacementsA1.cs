using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class Member1dDisplacementsA1 {
    // these are regression tests, the values are taken directly from GSA results
    // "Steel_Design_Simplex.gwb" for all members for analysis case A1 with 5 positions
    internal static List<double> XInMillimeter() {
      return new List<double>() {
        0,
        0,
        0,
        0,
        0,
      };
    }

    internal static List<double> YInMillimeter() {
      return new List<double>() {
        0,
        0,
        0,
        0,
        0,
      };
    }

    internal static List<double> ZInMillimeter() {
      return new List<double>() {
        0.0,
        -3.639,
        -5.097,
        -3.639,
        0.0,
      };
    }

    internal static List<double> XyzInMillimeter() {
      return new List<double>() {
        0.0,
        3.639,
        5.097,
        3.639,
        0.0,
      };
    }

    internal static List<double> XxInRadian() {
      return new List<double>() {
        0,
        0,
        0,
        0,
        0,
      };
    }

    internal static List<double> YyInRadian() {
      return new List<double>() {
        0.002196,
        0.001490,
        0.0,
        -0.001490,
        -0.002196,
      };
    }

    internal static List<double> ZzInRadian() {
      return new List<double>() {
        0,
        0,
        0,
        0,
        0,
      };
    }

    internal static List<double> XxyyzzInRadian() {
      return new List<double>() {
        0.002196,
        0.001490,
        0.0,
        0.001490,
        0.002196,
      };
    }
  }
}
