using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class Element2dShearForcesC2p1 {
    // these are regression tests. the values are taken directly from GSA results
    // "Element2d_Simple.gwb" for elements 420 430 440 445 for combination case C2p1
    internal static List<double> QxInKiloNewtonPerMeter() {
      return new List<double>() {
        -23.66,
        -22.02,
        -28.50,
        -30.66,
        -26.21,
        8.012,
        8.012,
        8.012,
        8.012,
        44.55,
        -48.17,
        -66.25,
        -1.806,
        -57.21,
        -10.85,
        -23.29,
        59.58,
        -54.86,
        -61.51,
        42.95,
        2.357,
        -58.19,
        -9.280,
        51.27,
        -3.461,
      };
    }

    internal static List<double> QyInKiloNewtonPerMeter(){
      return new List<double>(){
        -64.79,
        -64.80,
        -43.20,
        -44.30,
        -54.27,
        6.272,
        6.272,
        6.272,
        6.272,
        -5.016,
        -66.40,
        19.90,
        -35.71,
        -23.25,
        7.444,
        -17.17,
        -48.58,
        -73.08,
        80.64,
        41.04,
        -60.83,
        3.783,
        60.84,
        -3.769,
        0.006942,
      };
    }
  }
}
