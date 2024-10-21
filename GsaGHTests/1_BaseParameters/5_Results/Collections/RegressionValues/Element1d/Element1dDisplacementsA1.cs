using System.Collections.Generic;

namespace GsaGHTests.Parameters.Results {
  public class Element1dDisplacementsA1 {
    // these are regression tests, the values are taken directly from GSA results
    // "Steel_Design_Complex.gwb" for elements 24 to 30 for analysis case A1
    internal static List<double> XInMillimeter() {
      return new List<double>() {
        1.923,
        2.210,
        2.498,
        2.786,
        9.883,
        9.890,
        9.897,
        9.904,
        2.786,
        3.073,
        3.359,
        3.646,
        9.990,
        9.996,
        10.00,
        10.01,
        3.646,
        3.907,
        4.168,
        4.429
      };
    }

    internal static List<double> YInMillimeter() {
      return new List<double>() {
        9.635,
        9.722,
        9.817,
        9.883,
        -2.786,
        -2.794,
        -2.826,
        -2.843,
        9.883,
        9.924,
        9.967,
        9.990,
        -3.646,
        -3.660,
        -3.682,
        -3.695,
        9.990,
        9.992,
        9.992,
        9.983
      };
    }

    internal static List<double> ZInMillimeter() {
      return new List<double>() {
        -74.35,
        -75.71,
        -76.72,
        -77.21,
        -77.21,
        -77.90,
        -78.69,
        -79.47,
        -77.21,
        -76.99,
        -76.29,
        -75.25,
        -75.25,
        -75.67,
        -76.17,
        -76.65,
        -75.25,
        -73.58,
        -71.36,
        -69.05
      };
    }

    internal static List<double> XyzInMillimeter() {
      return new List<double>() {
        74.99,
        76.36,
        77.39,
        77.89,
        77.89,
        78.58,
        79.36,
        80.13,
        77.89,
        77.69,
        77.01,
        76.00,
        76.00,
        76.42,
        76.91,
        77.38,
        76.00,
        74.36,
        72.17,
        69.90
      };
    }

    internal static List<double> XxInRadian() {
      return new List<double>() {
        -0.001404,
        -0.001227,
        -0.001050,
        -872.6E-6,
        169.9E-6,
        41.20E-6,
        -87.46E-6,
        -216.1E-6,
        -872.6E-6,
        -748.7E-6,
        -624.7E-6,
        -500.7E-6,
        -0.001669,
        -0.001760,
        -0.001852,
        -0.001943,
        -500.7E-6,
        -420.1E-6,
        -339.4E-6,
        -258.7E-6
      };
    }

    internal static List<double> YyInRadian() {
      return new List<double>() {
        0.002203,
        0.001829,
        0.001176,
        243.1E-6,
        915.4E-6,
        0.001139,
        0.001200,
        0.001098,
        109.8E-6,
        -731.8E-6,
        -0.001345,
        -0.001729,
        536.4E-6,
        712.2E-6,
        752.2E-6,
        656.4E-6,
        -0.001853,
        -0.003037,
        -0.003518,
        -0.003296
      };
    }

    internal static List<double> ZzInRadian() {
      return new List<double>() {
        102.4E-6,
        146.2E-6,
        131.0E-6,
        56.79E-6,
        25.64E-6,
        -40.36E-6,
        -46.60E-6,
        6.929E-6,
        49.65E-6,
        68.06E-6,
        54.74E-6,
        9.694E-6,
        -5.593E-6,
        -31.28E-6,
        -30.45E-6,
        -3.080E-6,
        3.376E-6,
        2.495E-6,
        -6.037E-6,
        -22.22E-6
      };
    }

    internal static List<double> XxyyzzInRadian() {
      return new List<double>() {
        0.002614,
        0.002207,
        0.001582,
        907.6E-6,
        931.4E-6,
        0.001141,
        0.001205,
        0.001120,
        880.9E-6,
        0.001049,
        0.001484,
        0.001800,
        0.001753,
        0.001899,
        0.001999,
        0.002051,
        0.001920,
        0.003066,
        0.003534,
        0.003306
      };
    }
  }
}
