using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class Member1dDisplacementsC1 {
    // these are regression tests, the values are taken directly from GSA results
    // "Steel_Design_Simple.gwb" for all members for combination case C1p1
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
        -8.670,
        -12.40,
        -8.670,
        0.0,
      };
    }

    internal static List<double> XyzInMillimeter() {
      return new List<double>() {
        0.0,
        8.670,
        12.40,
        8.670,
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
        0.005148,
        0.003658,
        67.33E-6,
        -0.003658,
        -0.005148,
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
        0.005148,
        0.003658,
        67.33E-6,
        0.003658,
        0.005148,
      };
    }
  }
}
