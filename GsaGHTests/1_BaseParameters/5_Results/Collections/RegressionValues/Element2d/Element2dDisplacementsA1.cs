using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class Element2dDisplacementsA1 {
    // these are regression tests, the values are taken directly from GSA results
    // "Element2d_Simple.gwb" for elements 420 430 440 445 for analysis case A1
    internal static List<double> XInMillimeter() {
      return new List<double>() {
        0.004927,
        0.005043,
        0.005144,
        0.005009,
        0.005031,
        0.002778,
        0.002517,
        0.001731,
        0.002342,
        0.005000,
        0.004944,
        0.004880,
        0.004931,
        0.004910,
        0.004945,
        0.004924,
        0.003667,
        0.004520,
        0.004570,
        0.003906,
        0.004160,
        0.004529,
        0.004180,
        0.003806,
        0.004171,
      };
    }

    internal static List<double> YInMillimeter() {
      return new List<double>() {
        0.01530,
        0.01537,
        0.01546,
        0.01541,
        0.01538,
        0.009554,
        0.008507,
        0.007409,
        0.008490,
        0.01553,
        0.01554,
        0.01548,
        0.01554,
        0.01552,
        0.01551,
        0.01553,
        0.01408,
        0.01417,
        0.01478,
        0.01475,
        0.01412,
        0.01449,
        0.01479,
        0.01446,
        0.01448,
      };
    }

    internal static List<double> ZInMillimeter() {
      return new List<double>() {
        -0.4254,
        -0.3156,
        -0.5081,
        -0.6427,
        -0.4729,
        -0.2177,
        -0.2067,
        -0.1490,
        -0.1911,
        -1.766,
        -1.795,
        -1.711,
        -1.784,
        -1.755,
        -1.738,
        -1.760,
        -0.4028,
        -0.5694,
        -0.5583,
        -0.4067,
        -0.4866,
        -0.5645,
        -0.4829,
        -0.4056,
        -0.4855,
      };
    }

    internal static List<double> XyzInMillimeter() {
      return new List<double>() {
        0.4257,
        0.3160,
        0.5083,
        0.6429,
        0.4732,
        0.2179,
        0.2069,
        0.1492,
        0.1913,
        1.766,
        1.795,
        1.711,
        1.784,
        1.755,
        1.738,
        1.760,
        0.4030,
        0.5695,
        0.5585,
        0.4070,
        0.4868,
        0.5647,
        0.4831,
        0.4058,
        0.4857,
      };
    }
  }
}
