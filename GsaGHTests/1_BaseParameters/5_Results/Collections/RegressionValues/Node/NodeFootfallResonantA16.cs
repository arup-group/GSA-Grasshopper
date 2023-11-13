using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class NodeFootfallResonantA16 {
    // these are regression tests, the values are taken directly from GSA results
    // "footfall_steel.gwb" for nodes 200 to 206 for Resonant analysis case A16
    internal static List<double> MaximumResponseFactor() {
      return new List<double>() {
        5.928,
7.416,
8.262,
8.207,
7.192,
5.322,
2.821
      };
    }

    internal static List<double> PeakVelocity() {
      return new List<double>() {
        0.001104,
0.001383,
0.001541,
0.001531,
0.001342,
992.7E-6,
526.1E-6
      };
    }

    internal static List<double> RMSVelocity() {
      return new List<double>() {
        928.3E-6,
0.001163,
0.001296,
0.001288,
0.001128,
834.7E-6,
442.4E-6
      };
    }

    internal static List<double> PeakAcceleration() {
      return new List<double>() {
        0.04193,
0.05245,
0.05844,
0.05805,
0.05087,
0.03764,
0.01995
      };
    }

    internal static List<double> RMSAcceleration() {
      return new List<double>() {
        0.03526,
0.04411,
0.04915,
0.04882,
0.04278,
0.03165,
0.01678
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
