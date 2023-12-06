using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class Element2dDisplacementsC2p1 {
    // these are regression tests, the values are taken directly from GSA results
    // "Element2d_Simple.gwb" for elements 420 430 440 445 for combination case C2p1
    internal static List<double> XInMillimeter() {
      return new List<double>() {
        0.006623,
0.006781,
0.006918,
0.006734,
0.006764,
0.003736,
0.003384,
0.002325,
0.003148,
0.006718,
0.006641,
0.006554,
0.006623,
0.006595,
0.006643,
0.006615,
0.004928,
0.006080,
0.006148,
0.005251,
0.005594,
0.006092,
0.005620,
0.005115,
0.005609
      };
    }

    internal static List<double> YInMillimeter() {
      return new List<double>() {
        0.02067,
0.02076,
0.02088,
0.02081,
0.02078,
0.01290,
0.01149,
0.01001,
0.01147,
0.02098,
0.02100,
0.02092,
0.02100,
0.02097,
0.02096,
0.02098,
0.01902,
0.01914,
0.01997,
0.01993,
0.01908,
0.01958,
0.01998,
0.01954,
0.01957
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
-1.080
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
1.080
      };
    }
  }
}
