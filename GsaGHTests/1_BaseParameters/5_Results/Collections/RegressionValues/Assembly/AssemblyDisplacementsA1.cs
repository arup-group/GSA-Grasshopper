using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class AssemblyDisplacementsA1 {
    // these are regression tests, the values are taken directly from GSA results
    // "assembly-simple.gwb" for Assembly 2 for analysis case A1
    internal static List<double> XInMillimeter() {
      return new List<double>() {
        -5.20E-07,
        5.00E-07,
        1.50E-06,
        9.84E-07,
        1.04E-06,
        1.08E-06,
        1.08E-06,
        1.05E-06,
        1.03E-06,
        9.67E-07
      };
    }

    internal static List<double> YInMillimeter() {
      return new List<double>() {
        0.001790,
        -0.003385,
        25.56E-06,
        0.001149,
        0.001002,
        796.2E-06,
        585.5E-06,
        385.5E-06,
        194.0E-06,
        28.21E-06
      };
    }

    internal static List<double> ZInMillimeter() {
      return new List<double>() {
        -0.01332,
        -0.03676,
        -0.04245,
        -0.04614,
        -0.05080,
        -0.05469,
        -0.05776,
        -0.05995,
        -0.06131,
        -0.06176
      };
    }

    internal static List<double> XyzInMillimeter() {
      return new List<double>() {
        0.01344,
        0.03692,
        0.04245,
        0.04615,
        0.05081,
        0.0547,
        0.05776,
        0.05995,
        0.06131,
        0.06176
      };
    }

    internal static List<double> XxInRadian() {
      return new List<double>() {
        14.10E-06,
        -3.36E-06,
        -2.30E-06,
        96.57E-09,
        165.2E-09,
        195.9E-09,
        182.4E-09,
        180.7E-09,
        155.3E-09,
        167.2E-09
      };
    }

    internal static List<double> YyInRadian() {
      return new List<double>() {
        1.31E-09,
        1.52E-09,
        1.62E-10,
        2.12E-10,
        1.09E-10,
        3.36E-11,
        -1.18E-11,
        -4.55E-11,
        -6.90E-11,
        4.41E-11
      };
    }

    internal static List<double> ZzInRadian() {
      return new List<double>() {
        0.0,
        -1.34E-09,
        -120.7E-12,
        176.5E-12,
        128.2E-12,
        0.0,
        0.0,
        0.0,
        0.0,
        0.0
      };
    }

    internal static List<double> XxyyzzInRadian() {
      return new List<double>() {
        1.41E-05,
        3.36E-06,
        2.30E-06,
        9.66E-08,
        1.65E-07,
        1.96E-07,
        1.82E-07,
        1.81E-07,
        1.55E-07,
        1.67E-07
      };
    }
  }
}
