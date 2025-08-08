using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class AssemblyDriftIndicesA1 {
    // these are regression tests, the values are taken directly from GSA results
    // "assembly-simple.gwb" for Assembly 2 for analysis case A1
    internal static List<double> X() {
      return new List<double>() {
        0,
        1.01E-09,
        9.00E-10,
        -4.642E-10,
        5.01E-11,
        3.65E-11,
        -4.01E-12,
        -2.67E-11,
        -1.43E-11,
        -6.26E-11,
      };
    }

    internal static List<double> Y() {
      return new List<double>() {
        0,
        -5.124E-06,
        3.07E-06,
        1.01E-06,
        -1.32E-07,
        -1.85E-07,
        -1.90E-07,
        -1.80E-07,
        -1.72E-07,
        -1.64E-07,
      };
    }

    internal static List<double> Xy() {
      return new List<double>() {
        0,
        5.124E-06,
        3.07E-06,
        1.01E-06,
        1.32E-07,
        1.85E-07,
        1.90E-07,
        1.80E-07,
        1.72E-07,
        1.64E-07,
      };
    }
  }
}
