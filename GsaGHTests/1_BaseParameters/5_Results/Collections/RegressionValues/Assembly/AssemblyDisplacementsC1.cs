using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class AssemblyDisplacementsC1 {
    // these are regression tests, the values are taken directly from GSA results
    // "assembly-simple.gwb" for Assembly 2 for combination case C1
    internal static List<double> XInMillimeter() {
      return new List<double>() {
        -2.76E-05,
        -5.19E-04,
        -0.001026,
        -0.001834,
        -0.002921,
        -0.003684,
        -0.004337,
        -0.004527,
        -0.004939,
        -0.005529
      };
    }

    internal static List<double> YInMillimeter() {
      return new List<double>() {
        0.002188,
        -0.005362,
        -0.006828,
        -0.01146,
        -0.01806,
        -0.02517,
        -0.0327,
        -0.0404,
        -0.04827,
        -0.05547
      };
    }

    internal static List<double> ZInMillimeter() {
      return new List<double>() {
        -0.01039,
        -0.03068,
        -0.0347,
        -0.03722,
        -0.04093,
        -0.04411,
        -0.04662,
        -0.04857,
        -0.04959,
        -0.05022
      };
    }

    internal static List<double> XyzInMillimeter() {
      return new List<double>() {
        0.01062,
        0.03115,
        0.03538,
        0.03898,
        0.04483,
        0.05092,
        0.05711,
        0.06334,
        0.06938,
        0.07503
      };
    }

    internal static List<double> XxInRadian() {
      return new List<double>() {
        1.51E-05,
        4.34E-07,
        2.58E-06,
        5.58E-06,
        6.19E-06,
        6.60E-06,
        6.92E-06,
        6.96E-06,
        7.28E-06,
        7.22E-06
      };
    }

    internal static List<double> YyInRadian() {
      return new List<double>() {
        -63.0E-09,
        -1.47E-07,
        -2.05E-07,
        -3.01E-07,
        -4.24E-07,
        -5.15E-07,
        -5.76E-07,
        -5.77E-07,
        -5.74E-07,
        -5.73E-07
      };
    }

    internal static List<double> ZzInRadian() {
      return new List<double>() {
        4.66E-08,
        4.22E-07,
        7.02E-07,
        1.16E-06,
        1.70E-06,
        1.89E-06,
        1.89E-06,
        1.50E-06,
        1.29E-06,
        1.26E-06
      };
    }

    internal static List<double> XxyyzzInRadian() {
      return new List<double>() {
        1.51E-05,
        6.23E-07,
        2.68E-06,
        5.71E-06,
        6.43E-06,
        6.88E-06,
        7.20E-06,
        7.14E-06,
        7.41E-06,
        7.35E-06,
      };
    }
  }
}
