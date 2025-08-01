using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class AssemblyDriftIndicesA1 {
    // these are regression tests, the values are taken directly from GSA results
    // "assembly-simple.gwb" for Assembly 2 for analysis case A1
    internal static List<double> X() {
      return new List<double>() {
        0.0,
        989.4E-12,
        796.7E-12,
        -475.4E-12,
        61.81E-12,
        46.47E-12,
        5.030E-12,
        -17.16E-12,
        -3.821E-12,
        -69.82E-12,
      };
    }

    internal static List<double> Y() {
      return new List<double>() {
        0.0,
        -5.124E-6,
        3.070E-6,
        1.011E-6,
        -132.1E-9,
        -185.1E-9,
        -189.6E-9,
        -180.0E-9,
        -172.4E-9,
        -164.1E-9,
      };
    }

    internal static List<double> Xy() {
      return new List<double>() {
        0.0,
        -23.21E-6,
        -5.124E-6,
        -3.315E-6,
        -4.195E-6,
        -3.501E-6,
        -2.762E-6,
        -1.975E-6,
        -1.217E-6,
        -454.5E-9,
      };
    }
  }
}
