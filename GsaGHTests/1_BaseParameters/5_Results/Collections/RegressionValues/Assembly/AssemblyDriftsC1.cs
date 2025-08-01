using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class AssemblyDriftsC1 {
    // these are regression tests, the values are taken directly from GSA results
    // "assembly-simple.gwb" for Assembly 2 for combination case C1
    internal static List<double> XInMillimeter() {
      return new List<double>() {
        0.0,
        -491.5E-6,
        -508.0E-6,
        -807.9E-6,
        -0.001087,
        -762.9E-6,
        -652.7E-6,
        -189.0E-6,
        -412.1E-6,
        -589.1E-6,
      };
    }

    internal static List<double> YInMillimeter() {
      return new List<double>() {
        0.0,
        -0.007555,
        -0.001465,
        -0.004633,
        -0.006591,
        -0.007108,
        -0.007520,
        -0.007701,
        -0.007864,
        -0.007195,
      };
    }

    internal static List<double> XyInMillimeter() {
      return new List<double>() {
        0.0,
        0.007606,
        0.008313,
        0.006935,
        0.006682,
        0.007150,
        0.007549,
        0.007703,
        0.007875,
        0.007219,
      };
    }
  }
}
