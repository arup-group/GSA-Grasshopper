using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class AssemblyDriftsA1 {
    // these are regression tests, the values are taken directly from GSA results
    // "assembly-simple.gwb" for Assembly 2 for analysis case A1
    internal static List<double> XInMillimeter() {
      return new List<double>() {
        0.0,
        999.4E-9,
        885.3E-9,
        -528.2E-9,
        68.68E-9,
        51.64E-9,
        5.589E-9,
        -19.06E-9,
        -4.245E-9,
        -70.53E-9,
      };
    }

    internal static List<double> YInMillimeter() {
      return new List<double>() {
        0.0,
        -0.005175,
        0.003411,
        0.001123,
        -146.8E-6,
        -205.7E-6,
        -210.7E-6,
        -200.0E-6,
        -191.6E-6,
        -165.8E-6,
      };
    }

    internal static List<double> XyInMillimeter() {
      return new List<double>() {
        0,
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
