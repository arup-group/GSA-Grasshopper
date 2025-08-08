using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class AssemblyDisplacementsC1 {
    // these are regression tests, the values are taken directly from GSA results
    // "assembly-simple.gwb" for Assembly 2 for combination case C1
    internal static List<double> XInMillimeter() {
      return new List<double>() {
        -28.16E-6,
        -519.6E-6,
        -0.001028,
        -0.001836,
        -0.002922,
        -0.003685,
        -0.004338,
        -0.004527,
        -0.004939,
        -0.005528,
      };
    }

    internal static List<double> YInMillimeter() {
      return new List<double>() {
        0.002188,
        -0.005367,
        -0.006832,
        -0.01147,
        -0.01806,
        -0.02516,
        -0.03268,
        -0.04038,
        -0.04825,
        -0.05544,
      };
    }

    internal static List<double> ZInMillimeter() {
      return new List<double>() {
        -0.01038,
        -0.03069,
        -0.03470,
        -0.03722,
        -0.04094,
        -0.04412,
        -0.04663,
        -0.04858,
        -0.04960,
        -0.05023,
      };
    }

    internal static List<double> XyzInMillimeter() {
      return new List<double>() {
        0.01061,
        0.03116,
        0.03538,
        0.03899,
        0.04484,
        0.05092,
        0.05711,
        0.06334,
        0.06937,
        0.07502,
      };
    }

    internal static List<double> XxInRadian() {
      return new List<double>() {
        15.12E-6,
        430.8E-9,
        2.574E-6,
        5.577E-6,
        6.182E-6,
        6.594E-6,
        6.916E-6,
        6.952E-6,
        7.277E-6,
        7.216E-6,
      };
    }

    internal static List<double> YyInRadian() {
      return new List<double>() {
        -62.79E-9,
        -146.9E-9,
        -204.6E-9,
        -300.3E-9,
        -423.2E-9,
        -514.2E-9,
        -575.3E-9,
        -576.2E-9,
        -573.2E-9,
        -572.5E-9,
      };
    }

    internal static List<double> ZzInRadian() {
      return new List<double>() {
        46.56E-9,
        422.3E-9,
        702.8E-9,
        1.157E-6,
        1.705E-6,
        1.886E-6,
        1.889E-6,
        1.500E-6,
        1.288E-6,
        1.262E-6,
      };
    }

    internal static List<double> XxyyzzInRadian() {
      return new List<double>() {
        15.12E-6,
        620.9E-9,
        2.676E-6,
        5.703E-6,
        6.427E-6,
        6.878E-6,
        7.192E-6,
        7.135E-6,
        7.413E-6,
        7.348E-6,
      };
    }
  }
}
