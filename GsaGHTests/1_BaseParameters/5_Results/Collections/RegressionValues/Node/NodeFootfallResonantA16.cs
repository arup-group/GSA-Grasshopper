using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class NodeFootfallResonantA16 {
    // these are regression tests, the values are taken directly from GSA results
    // "footfall_steel.gwb" for nodes 200 to 206 for Resonant analysis case A16
    internal static List<double> MaximumResponseFactor() {
      return new List<double>() {
        5.927,
        7.414,
        8.261,
        8.206,
        7.191,
        5.321,
        2.820
      };
    }

    internal static List<double> PeakVelocity() {
      return new List<double>() {
        0.001104,
        0.001382,
        0.001541,
        0.001531,
        0.001341,
        992.5E-6,
        526.0E-6
      };
    }

    internal static List<double> RMSVelocity() {
      return new List<double>() {
        780.4E-6,
        977.6E-6,
        0.001090,
        0.001082,
        948.5E-6,
        701.8E-6,
        371.9E-6
      };
    }

    internal static List<double> PeakAcceleration() {
      return new List<double>() {
        0.04192,
        0.05244,
        0.05843,
        0.05804,
        0.05086,
        0.03764,
        0.01995
      };
    }

    internal static List<double> RMSAcceleration() {
      return new List<double>() {
        0.02964,
        0.03708,
        0.04132,
        0.04104,
        0.03597,
        0.02661,
        0.01411
      };
    }

    internal static List<int> CriticalNode() {
      return new List<int>() {
        202,
202,
202,
202,
202,
202,
202
      };
    }

    internal static List<double> CriticalFrequency() {
      return new List<double>() {
        2.025,
2.025,
2.025,
2.025,
2.025,
2.025,
2.025
      };
    }
  }
}
