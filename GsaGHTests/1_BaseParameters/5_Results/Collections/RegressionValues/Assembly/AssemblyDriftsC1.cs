using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class AssemblyDriftsC1 {
    // these are regression tests, the values are taken directly from GSA results
    // "assembly-simple.gwb" for Assembly 2 for combination case C1
    internal static List<double> XInMillimeter() {
      return new List<double>() {
        0,
        -4.91E-04,
        -5.07E-04,
        -8.08E-04,
        -0.001087,
        -7.63E-04,
        -6.53E-04,
        -1.89E-04,
        -4.13E-04,
        -5.89E-04
      };
    }

    internal static List<double> YInMillimeter() {
      return new List<double>() {
        0,
        -0.007551,
        -0.001465,
        -0.004637,
        -0.006596,
        -0.007112,
        -0.007524,
        -0.007705,
        -0.007867,
        -0.007199
      };
    }

    internal static List<double> XyInMillimeter() {
      return new List<double>() {
        0.00E+00,
        7.60E-03,
        8.31E-03,
        6.94E-03,
        6.69E-03,
        7.15E-03,
        7.55E-03,
        0.007708,
        0.007878,
        0.007223
      };
    }
  }
}
