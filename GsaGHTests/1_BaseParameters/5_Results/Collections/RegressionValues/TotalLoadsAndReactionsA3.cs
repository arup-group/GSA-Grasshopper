using System.Collections.Generic;

namespace GsaGHTests._1_BaseParameters._5_Results.Collections.RegressionValues {
  public class TotalLoadsAndReactionsA3 {
    // these are regression tests, the values are taken directly from GSA results
    // "Steel_Design_Complex.gwb" for all nodes for analysis case A3

    internal static List<double> TotalLoad = new List<double>() { //in kN
      80160,
      -80160,
      -59.45E-12,
      113400,
      450100,
      450100,
      -3.206E+6,
      3.269E+6,
    };

    internal static List<double> TotalReaction = new List<double>() { // in kN
      -80160,
      80160,
      0.001875,
      113400,
      -450100,
      -450100,
      3.206E+6,
      3.269E+6,
    };

    internal static int Mode = 1;

    internal static double ModalStiffness = 105900; // KilonewtonPerMeter
    internal static double ModalMass = 4264; // Tonne
    internal static double? ModalGeometricStiffness = 0;
    internal static double? LoadFactor = 0;
    internal static double Frequency = 0.7930; // Hertz
    internal static double? EigenValue = null;
    internal static List<double> EffectiveInertia = null;
    internal static List<double> EffectiveMass = new List<double>() { // [t|tm2]
      2445,
      2445,
      0,
    };
  }
}
