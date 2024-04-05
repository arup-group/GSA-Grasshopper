using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class AssemblyByStoreyDisplacementsC1 {
    // these are regression tests, the values are taken directly from GSA results
    // "assembly-simple.gwb" for Assembly 2 for combination case C1
    internal static List<double> XInMillimeter() {
      return new List<double>() {
        -0.002838,
        -0.02303,
        -0.02524,
        -0.05987,
        -0.06306,
        -0.1041
      };
    }

    internal static List<double> YInMillimeter() {
      return new List<double>() {
        -9.25E-04,
        -0.006854,
        -0.006538,
        -0.0224,
        -0.02228,
        -0.04538
      };
    }

    internal static List<double> ZInMillimeter() {
      return new List<double>() {
        -0.0153,
        -0.05774,
        -0.0589,
        -0.08967,
        -0.0902,
        -0.1069
      };
    }

    internal static List<double> XyzInMillimeter() {
      return new List<double>() {
        0.01558,
        0.06254,
        0.06441,
        0.1101,
        0.1123,
        0.156
      };
    }

    internal static List<double> XxInRadian() {
      return new List<double>() {
        8.94E-07,
        3.16E-06,
        3.41E-06,
        5.01E-06,
        5.15E-06,
        6.01E-06
      };
    }

    internal static List<double> YyInRadian() {
      return new List<double>() {
        -1.67E-06,
        -5.78E-06,
        -6.26E-06,
        -8.85E-06,
        -9.11E-06,
        -1.04E-05
      };
    }

    internal static List<double> ZzInRadian() {
      return new List<double>() {
        -4.99E-07,
        -2.96E-06,
        -2.45E-06,
        -4.58E-06,
        -3.77E-06,
        -4.88E-06
      };
    }

    internal static List<double> XxyyzzInRadian() {
      return new List<double>() {
        1.96E-06,
        7.22E-06,
        7.54E-06,
        1.12E-05,
        1.11E-05,
        1.29E-05
      };
    }
  }
}
