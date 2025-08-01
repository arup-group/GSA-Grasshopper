using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class AssemblyDriftIndicesC1 {
    // these are regression tests, the values are taken directly from GSA results
    // "assembly-simple.gwb" for Assembly 2 for combination case C1
    internal static List<double> X() {
      return new List<double>() {
        0,
        -4.86E-07,
        -4.56E-07,
        -7.27E-07,
        -9.785E-07,
        -6.87E-07,
        -5.88E-07,
        -1.71E-07,
        -3.71E-07,
        -5.84E-07,
      };
    }

    internal static List<double> Y() {
      return new List<double>() {
        0,
        -7.475E-06,
        -1.32E-06,
        -4.17E-06,
        -5.94E-06,
        -6.40E-06,
        -6.77E-06,
        -6.94E-06,
        -7.08E-06,
        -7.13E-06,
      };
    }

    internal static List<double> Xy() {
      return new List<double>() {
        0,
        7.525E-06,
        7.48E-06,
        6.25E-06,
        6.02E-06,
        6.44E-06,
        6.80E-06,
        6.94E-06,
        7.09E-06,
        7.15E-06,
      };
    }
  }
}
