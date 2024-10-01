using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel.Types;

using OasysGH.Parameters;

namespace GsaGHTests.Parameters.Results {
  public class SteelDesignEffectiveLengthC1P1 {
    //results are taken directly from GSA results
    // "BasicFrame.gwb" for member id=46,47,48 for analysis case C1P1
    //Length and Positions are in meters

    internal static List<double> MajorMemberLength = new List<double>() {
      7071,
      8000,
      8000,
    };

    internal static List<double> MajorSpan = new List<double>() {
      1,
      1,
      2,
      1,
      2,
    };

    internal static List<double> MajorSpanElements = new List<double>() {
      28,
      29,
      30,
      31,
      32,
    };
    internal static List<double> MajorStartPosition = new List<double>() {
      0,
      0,
      5000,
      0,
      5000,
    };
    internal static List<double> MajorEndPosition = new List<double>() {
      7071,
      5000,
      8000,
      5000,
      8000,
    };
    internal static List<double> MajorSpanLength = new List<double>() {
      7071,
      5000,
      3000,
      5000,
      3000,
    };
    internal static List<double> MajorEffectiveLength = new List<double>() {
      7071,
      3500,
      2100,
      3500,
      2100,
    };
    internal static List<double> MajorEffectiveLengthEffectiveSpanRatio = new List<double>() {
      1,
      0.4375,
      0.2625,
      0.4375,
      0.2625,
    };
    internal static List<double> MajorEffectiveLengthEffectiveSpanRatio2 = new List<double>() {
      1,
      0.7,
      0.7,
      0.7,
      0.7,
    };
    internal static List<double> MajorSlendernessRatio = new List<double>() {
      217.1,
      18.97,
      11.38,
      18.97,
      11.38,
    };

    internal static List<double> MinorMemberLength = new List<double>() {
      7071,
      8000,
      8000,
    };

    internal static List<double> MinorSpan = new List<double>() {
      1,
      1,
      2,
      1,
      2,
    };

    internal static List<double> MinorSpanElements = new List<double>() {
      28,
      29,
      30,
      31,
      32,
    };
    internal static List<double> MinorStartPosition = new List<double>() {
      0,
      0,
      5000,
      0,
      5000,
    };
    internal static List<double> MinorEndPosition = new List<double>() {
      7071,
      5000,
      8000,
      5000,
      8000,
    };
    internal static List<double> MinorSpanLength = new List<double>() {
      7071,
      5000,
      3000,
      5000,
      3000,
    };
    internal static List<double> MinorEffectiveLength = new List<double>() {
      7071,
      4250,
      3000,
      4250,
      3000,
    };
    internal static List<double> MinorEffectiveLengthEffectiveSpanRatio = new List<double>() {
      1.0,
      0.5312,
      0.375,
      0.5312,
      0.375,
    };
    internal static List<double> MinorEffectiveLengthEffectiveSpanRatio2 = new List<double>() {
      1,
      0.85,
      1,
      0.85,
      1,
    };
    internal static List<double> MinorSlendernessRatio = new List<double>() {
      217.1,
      38.56,
      27.22,
      38.56,
      27.22,
    };

    internal static List<double> LTMemberLength = new List<double>() {
      7071,
      8000,
      8000,
    };

    internal static List<double> LTSpan = new List<double>() {
      1,
      1,
      2,
      1,
      2,
    };

    internal static List<double> LTSpanElements = new List<double>() {
      28,
      29,
      30,
      31,
      32,
    };
    internal static List<double> LTStartPosition = new List<double>() {
      0,
      0,
      5000,
      0,
      5000,
    };
    internal static List<double> LTEndPosition = new List<double>() {
      7071,
      5000,
      8000,
      5000,
      8000,
    };
    internal static List<double> LTSpanLength = new List<double>() {
      7071,
      5000,
      3000,
      5000,
      3000,
    };
    internal static List<double> LTEffectiveLength = new List<double>() {
      7071,
      3625,
      3100,
      3500,
      2100,
    };
    internal static List<double> LTEffectiveLengthEffectiveSpanRatio = new List<double>() {
      1.0   ,
      0.4531,
      0.3875,
      0.4375,
      0.2625,
    };
    internal static List<double> LTEffectiveLengthEffectiveSpanRatio2 = new List<double>() {
      1.0,
      0.725,
      1.033,
      0.7,
      0.7,
    };
    internal static List<double> LTSlendernessRatio = new List<double>() {
      217.1,
      19.65,
      16.8 ,
      18.97,
      11.38,
    };
  }
}
