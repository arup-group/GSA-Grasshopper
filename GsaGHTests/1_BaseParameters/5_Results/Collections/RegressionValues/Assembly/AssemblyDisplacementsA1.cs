using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class AssemblyDisplacementsA1 {
    // these are regression tests, the values are taken directly from GSA results
    // "assembly-simple.gwb" for Assembly 2 for analysis case A1
    internal static List<double> XInMillimeter() {
      return new List<double>() {
        -676.2E-9,
        323.2E-9,
        1.208E-6,
        680.2E-9,
        748.9E-9,
        800.5E-9,
        806.1E-9,
        787.1E-9,
        782.8E-9,
        712.3E-9,
      };
    }

    internal static List<double> YInMillimeter() {
      return new List<double>() {
        0.001790,
        -0.003385,
        25.80E-6,
        0.001149,
        0.001002,
        796.4E-6,
        585.7E-6,
        385.7E-6,
        194.1E-6,
        28.31E-6,
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
        -0.06176,
      };
    }

    internal static List<double> XyzInMillimeter() {
      return new List<double>() {
        0.01344,
        0.03692,
        0.04245,
        0.04615,
        0.05081,
        0.05470,
        0.05776,
        0.05995,
        0.06131,
        0.06176,
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
        1.336E-9,
        1.575E-9,
        187.3E-12,
        231.1E-12,
        124.1E-12,
        47.66E-12,
        0.0,
        -35.90E-12,
        -68.54E-12,
        35.98E-12,
      };
    }

    internal static List<double> ZzInRadian() {
      return new List<double>() {
        0.0,
        -1.311E-9,
        0.0,
        276.7E-12,
        237.9E-12,
        131.4E-12,
        0.0,
        0.0,
        0.0,
        225.1E-12,
      };
    }

    internal static List<double> XxyyzzInRadian() {
      return new List<double>() {
        14.10E-6,
        3.360E-6,
        2.303E-6,
        96.59E-9,
        165.2E-9,
        196.0E-9,
        182.4E-9,
        180.7E-9,
        155.3E-9,
        167.3E-9,
      };
    }
  }
}
