using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class Element2dShearForcesA1 {
    // these are regression tests. the values are taken directly from GSA results
    // "Element2d_Simple.gwb" for elements 420 430 440 445 for analysis case A1
    internal static List<double> QxInKiloNewtonPerMeter() {
      return new List<double>() {
        -10.64,
        -9.901,
        -12.81,
        -13.79,
        -11.79,
        3.602,
        3.602,
        3.602,
        3.602,
        20.03,
        -21.66,
        -29.79,
        -0.8129,
        -25.72,
        -4.878,
        -10.47,
        26.79,
        -24.67,
        -27.66,
        19.31,
        1.060,
        -26.16,
        -4.173,
        23.05,
        -1.556,
      };
    }

    internal static List<double> QyInKiloNewtonPerMeter(){
      return new List<double>(){
        -29.13,
        -29.14,
        -19.42,
        -19.92,
        -24.40,
        2.820,
        2.820,
        2.820,
        2.820,
        -2.257,
        -29.86,
        8.949,
        -16.06,
        -10.45,
        3.346,
        -7.721,
        -21.84,
        -32.86,
        36.26,
        18.45,
        -27.35,
        1.701,
        27.36,
        -1.695,
        0.003124,
      };
    }
  }
}
