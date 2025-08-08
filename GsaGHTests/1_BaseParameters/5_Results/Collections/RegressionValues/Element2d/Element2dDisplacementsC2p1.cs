using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class Element2dDisplacementsC2p1 {
    // these are regression tests, the values are taken directly from GSA results
    // "Element2d_Simple.gwb" for elements 420 430 440 445 for combination case C2p1
    internal static List<double> XInMillimeter() {
      return new List<double>() {
        0.006651,
        0.006808,
        0.006945,
        0.006762,
        0.006791,
        0.003750,
        0.003398,
        0.002337,
        0.003162,
        0.006751,
        0.006674,
        0.006587,
        0.006656,
        0.006628,
        0.006676,
        0.006648,
        0.004951,
        0.006102,
        0.006169,
        0.005273,
        0.005616,
        0.006114,
        0.005643,
        0.005138,
        0.005631,
      };
    }

    internal static List<double> YInMillimeter() {
      return new List<double>() {
        0.02066,
        0.02075,
        0.02087,
        0.02080,
        0.02077,
        0.01290,
        0.01149,
        0.01000,
        0.01146,
        0.02096,
        0.02098,
        0.02090,
        0.02098,
        0.02095,
        0.02094,
        0.02096,
        0.01900,
        0.01913,
        0.01996,
        0.01992,
        0.01906,
        0.01956,
        0.01997,
        0.01952,
        0.01955,
      };
    }

    internal static List<double> ZInMillimeter() {
      return new List<double>() {
        -0.9462,
        -0.7019,
        -1.130,
        -1.429,
        -1.052,
        -0.4841,
        -0.4597,
        -0.3314,
        -0.4251,
        -3.927,
        -3.991,
        -3.805,
        -3.967,
        -3.902,
        -3.866,
        -3.913,
        -0.8958,
        -1.266,
        -1.242,
        -0.9045,
        -1.082,
        -1.255,
        -1.074,
        -0.9020,
        -1.080,
      };
    }

    internal static List<double> XyzInMillimeter() {
      return new List<double>() {
        0.9464,
        0.7023,
        1.130,
        1.430,
        1.052,
        0.4843,
        0.4599,
        0.3316,
        0.4252,
        3.927,
        3.991,
        3.805,
        3.967,
        3.902,
        3.866,
        3.913,
        0.8960,
        1.266,
        1.242,
        0.9047,
        1.082,
        1.256,
        1.074,
        0.9022,
        1.080,
      };
    }
  }
}
