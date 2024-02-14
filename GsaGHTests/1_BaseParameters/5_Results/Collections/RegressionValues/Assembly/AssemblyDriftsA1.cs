using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class AssemblyDriftsA1 {
    // these are regression tests, the values are taken directly from GSA results
    // "assembly-simple.gwb" for Assembly 2 for analysis case A1
    internal static List<double> XInMillimeter() {
      return new List<double>() {
        1.02E-06,
        1.00E-06,
        -5.16E-07,
        5.56E-08,
        4.05E-08,
        -4.45E-09,
        -2.97E-08,
        -1.59E-08,
        -6.32E-08
      };
    }

    internal static List<double> YInMillimeter() {
      return new List<double>() {
        -0.005176,
        0.003411,
        0.001123,
        -1.47E-04,
        -2.06E-04,
        -2.11E-04,
        -2.00E-04,
        -1.92E-04,
        -1.66E-04
      };
    }

    internal static List<double> XyInMillimeter() {
      return new List<double>() {
        0.005176,
        0.003411,
        0.001123,
        1.47E-04,
        2.06E-04,
        2.11E-04,
        2.00E-04,
        1.92E-04,
        1.66E-04
      };
    }
  }
}
