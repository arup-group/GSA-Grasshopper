using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class AssemblyByStoreyDisplacementsA1 {
    // these are regression tests, the values are taken directly from GSA results
    // "assembly-by-storey.gwb" for Assembly 2 for analysis case A1
    internal static List<double> XInMillimeter() {
      return new List<double>() {
        0.01373,
        0.08220,
        0.07943,
        0.1596,
        0.1552,
        0.2258,
      };
    }

    internal static List<double> YInMillimeter() {
      return new List<double>() {
        -706.2E-6,
        -0.004427,
        -0.004281,
        -0.01704,
        -0.01700,
        -0.03640,
      };
    }

    internal static List<double> ZInMillimeter() {
      return new List<double>() {
        -0.01436,
        -0.05438,
        -0.05530,
        -0.08472,
        -0.08521,
        -0.1014,
      };
    }

    internal static List<double> XyzInMillimeter() {
      return new List<double>() {
        0.01988,
        0.09866,
        0.09688,
        0.1815,
        0.1778,
        0.2502,
      };
    }

    internal static List<double> XxInRadian() {
      return new List<double>() {
        602.7E-9,
        2.084E-6,
        2.297E-6,
        3.418E-6,
        3.598E-6,
        4.244E-6,
      };
    }

    internal static List<double> YyInRadian() {
      return new List<double>() {
        3.258E-6,
        10.71E-6,
        10.50E-6,
        13.87E-6,
        13.71E-6,
        14.37E-6,
      };
    }

    internal static List<double> ZzInRadian() {
      return new List<double>() {
        -471.7E-9,
        -2.940E-6,
        -2.157E-6,
        -4.251E-6,
        -2.959E-6,
        -4.075E-6,
      };
    }

    internal static List<double> XxyyzzInRadian() {
      return new List<double>() {
        3.347E-6,
        11.30E-6,
        10.96E-6,
        14.90E-6,
        14.48E-6,
        15.53E-6,
      };
    }
  }
}
