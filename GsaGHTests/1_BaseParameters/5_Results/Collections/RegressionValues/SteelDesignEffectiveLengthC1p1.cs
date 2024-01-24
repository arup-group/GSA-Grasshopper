using Grasshopper.Kernel.Types;
using Grasshopper;
using OasysGH.Parameters;
using System.Collections.Generic;

namespace GsaGHTests._1_BaseParameters._5_Results.Collections.RegressionValues {
  public class SteelDesignEffectiveLengthC1P1 {
    //results are taken directly from GSA results
    // "Steel_Design_Complex.gwb" for member id=46,47,48 for analysis case C1P1
    //Length and Positions are in meters

    internal static List<double> MemberLength= new List<double>() {
      7.071,
      7.071,
      7.071,
      8.000,
      8.000,
      8.000,
      8.000,
      8.000,
      8.000,
      8.000,
      8.000,
      8.000,
      8.000,
      8.000,
      8.000,
    };

    internal static List<double> Span = new List<double>() {
      1,
      1,
      1,
      1,
      2,
      1,
      2,
      1,
      2,
      1,
      2,
      1,
      2,
      1,
      2,
    };

    internal static List<double> SpanElements = new List<double>() {
      28,
      28,
      28,
      29,
      30,
      29,
      30,
      29,
      30,
      31,
      32,
      31,
      32,
      31,
      32,
    };
    internal static List<double> StartPosition = new List<double>() {
      0.0,
      0.0,
      0.0,
      0.0,
      5.0,
      0.0,
      5.0,
      0.0,
      5.0,
      0.0,
      5.0,
      0.0,
      5.0,
      0.0,
      5.0,
    };
    internal static List<double> EndPosition = new List<double>() {
      7.071,
      7.071,
      7.071,
      5.000,
      8.000,
      5.000,
      8.000,
      5.000,
      8.000,
      5.000,
      8.000,
      5.000,
      8.000,
      5.000,
      8.000,
    };
    internal static List<double> SpanLength= new List<double>() {
      7.071,
      7.071,
      7.071,
      5.000,
      3.000,
      5.000,
      3.000,
      5.000,
      3.000,
      5.000,
      3.000,
      5.000,
      3.000,
      5.000,
      3.000,
    };
    internal static List<double> EffectiveLength= new List<double>() {
      7.071,
      7.071,
      7.071,
      3.500,
      2.100,
      4.250,
      3.000,
      5.000,
      3.000,
      3.500,
      2.100,
      4.250,
      3.000,
      5.000,
      3.000,
    };
    internal static List<double> EffectiveLengthEffectiveSpanRatio= new List<double>() {};
    internal static List<double> EffectiveLengthEffectiveSpanRatio2= new List<double>() {};
    internal static List<double> SlendernessRatio = new List<double>() {
      217.1,
      217.1,
      217.1,
      18.97,
      11.38,
      38.56,
      27.22,
      27.1,
      16.26,
      18.97,
      11.38,
      38.56,
      27.22,
      27.1,
      16.26,
    };
  }
}
