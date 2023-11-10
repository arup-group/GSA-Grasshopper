using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class NodeFootfallTransientA17 {
    // these are regression tests, the values are taken directly from GSA results
    // "footfall_steel.gwb" for nodes 200 to 206 for Transient analysis case A17
    internal static List<double> MaximumResponseFactor() {
      return new List<double>() {
        3.034,
3.768,
4.189,
4.194,
3.762,
2.848,
1.571
      };
    }

    internal static List<double> PeakVelocity() {
      return new List<double>() {
        813.3E-6,
992.5E-6,
0.001130,
0.001222,
0.001148,
931.6E-6,
521.5E-6
      };
    }

    internal static List<double> RMSVelocity() {
      return new List<double>() {
        374.0E-6,
464.3E-6,
515.4E-6,
510.8E-6,
451.5E-6,
336.8E-6,
179.6E-6
      };
    }

    internal static List<double> PeakAcceleration() {
      return new List<double>() {
        0.03449,
0.03927,
0.04096,
0.04920,
0.05647,
0.04957,
0.02894
      };
    }

    internal static List<double> RMSAcceleration() {
      return new List<double>() {
        0.01482,
0.01849,
0.02050,
0.02060,
0.01868,
0.01504,
0.008587
      };
    }

    internal static List<int> CriticalNode() {
      return new List<int>() {
        203,
202,
202,
203,
203,
203,
204
      };
    }

    internal static List<double> CriticalFrequency() {
      return new List<double>() {
        2.200,
2.200,
2.200,
2.200,
2.200,
2.200,
2.200
      };
    }
  }
}
