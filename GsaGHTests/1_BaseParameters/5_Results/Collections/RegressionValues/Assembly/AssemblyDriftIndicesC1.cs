using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class AssemblyDriftIndicesC1 {
    // these are regression tests, the values are taken directly from GSA results
    // "assembly-simple.gwb" for Assembly 2 for combination case C1
    internal static List<double> X() {
      return new List<double>() {
        0.0,
        -486.5E-9,
        -457.2E-9,
        -727.1E-9,
        -978.2E-9,
        -686.6E-9,
        -587.4E-9,
        -170.1E-9,
        -370.9E-9,
        -583.2E-9,
      };
    }

    internal static List<double> Y() {
      return new List<double>() {
        0.0,
        -7.480E-6,
        -1.319E-6,
        -4.170E-6,
        -5.932E-6,
        -6.397E-6,
        -6.768E-6,
        -6.931E-6,
        -7.077E-6,
        -7.123E-6,
      };
    }

    internal static List<double> Xy() {
      return new List<double>() {
        0.0,
        -20.10E-6,
        -3.614E-6,
        -2.268E-6,
        -3.343E-6,
        -2.860E-6,
        -2.262E-6,
        -1.757E-6,
        -912.1E-9,
        -629.5E-9,
      };
    }
  }
}
