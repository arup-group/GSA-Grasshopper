﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Xunit;

namespace IntegrationTests.Components {
  [Collection("GrasshopperFixture collection")]
  public class Elem1dResultsTests {
    [Fact]
    public void AverageStrainEnergyDensityTests() {
      GH_Document doc = Document;
      IGH_Param param = Helper.FindParameter(doc, "AvgStrainEnergyDensity");
      var output = (List<GH_Number>)param.VolatileData.get_Branch(0);

      var expectedVals = new List<double>() {
        48.27,
        39.61,
        51.98,
        85.38,
        211.9,
        200.3,
        213.6,
        251.9,
        46.92,
        39.32,
        51.07,
        82.18,
        157.2,
        146.6,
        159.0,
        194.3,
        933.1,
        925.6,
        918.2,
        910.9,
        155.8,
        146.2,
        158.4,
        192.4,
        132.5,
        122.6,
        134.1,
        166.8,
        927.2,
        919.3,
        912.4,
        906.5,
        175.5,
        164.7,
        178.9,
        218.0,
        132.2,
        121.6,
        134.7,
        171.5,
        634.3,
        626.9,
        624.3,
        626.4,
        195.7,
        187.3,
        195.6,
        220.8,
        47.59,
        14.61,
        2.537,
        11.36,
        49.80,
        18.45,
        4.399,
        7.649,
        47.31,
        14.47,
        2.498,
        11.38,
        62.15,
        19.23,
        3.674,
        15.48,
        5.140,
        3.848,
        2.984,
        2.549,
        63.72,
        19.81,
        3.762,
        15.56,
        57.53,
        18.04,
        4.010,
        15.45,
        4.883,
        4.222,
        3.655,
        3.183,
        67.41,
        21.54,
        4.301,
        15.69,
        65.06,
        19.43,
        3.674,
        17.81,
        24.57,
        8.498,
        3.246,
        8.817,
        49.77,
        14.79,
        3.359,
        15.46,
        7.228,
        21.80,
        33.18,
        23.89,
        6.676,
        60.26,
        61.27,
        7.946,
        23.44,
        33.34,
        22.31,
        6.356,
        5.105,
        28.98,
        36.84,
        26.37,
        3.443,
        69.58,
        70.58,
        2.875,
        26.77,
        41.81,
        30.53,
        4.820,
        3.356,
        30.23,
        41.35,
        21.78,
        3.013,
        68.11,
        63.66,
        3.300,
        25.42,
        40.34,
        38.22,
        7.355,
        4.970,
        35.56,
        39.13,
        28.09,
        3.820,
        66.95,
        62.11,
        0.5303,
        15.47,
        30.43,
        33.30,
        6.996,
        5.621,
        15.84,
        20.51,
        8.512,
        22.98,
        14.51,
        3.139,
        5.865,
        2.896,
        14.07,
        13.06,
        2.700,
        6.810,
        9.745,
        3.703,
        12.84,
        8.367,
        33.87,
        28.19,
        12.80,
        40.52,
        36.90,
        7.370,
        12.14,
        5.020,
        43.19,
        50.76,
        8.054,
        24.33,
        25.03,
        11.40,
        28.19,
        4.174,
        13.29,
        15.50,
        7.369,
        18.82,
        13.89,
        3.694,
        5.560,
        2.614,
        21.39,
        33.56,
        10.45,
        22.50,
        19.80,
        6.038,
        46.38,
        3.638,
        0.7010,
        2.762,
        2.770,
        0.7074,
        3.592,
        3.503,
        0.7129,
        2.751,
        2.699,
        0.6706,
        3.816,
        6.515,
        0.5432,
        2.353,
        3.154,
        1.189,
        1.735,
        1.706,
        1.194,
        3.143,
        2.323,
        0.5414,
        6.637,
        6.590,
        0.5515,
        2.416,
        3.282,
        1.286,
        1.595,
        1.811,
        1.112,
        3.013,
        2.235,
        0.5312,
        6.688,
        6.029,
        0.5040,
        2.119,
        2.629,
        0.8192,
        2.513,
        2.378,
        0.9991,
        3.123,
        2.673,
        0.6045,
        4.909,
        1.724,
        0.4974,
        1.205,
        0.4645,
        1.926,
        0.6977,
        0.7999,
        1.144,
        0.3271,
        3.983,
        3.860,
        0.7269,
        2.952,
        3.132,
        0.8952,
        2.906,
        5.125,
        0.3169,
        0.8286,
        0.6247,
        0.7042,
        0.5535,
        1.157,
        1.569,
        0.4297,
        3.411,
        2.835,
        0.7635,
        2.632,
        2.264,
        0.5184,
        5.287,
        1.469,
        0.5544,
        1.220,
        0.4337,
        2.195,
        0.8191,
        0.7601,
        1.195,
        0.3438,
        3.461,
        4.168,
        0.6875,
        2.901,
        3.186,
        0.9549,
        2.654,
        92.00,
        78.58,
        99.85,
        155.8,
        420.3,
        414.3,
        410.9,
        410.0,
        91.21,
        77.99,
        98.92,
        154.0,
        665.2,
        658.8,
        652.5,
        646.3,
        104.4,
        89.86,
        113.0,
        173.8,
        81.97,
        70.69,
        88.80,
        136.3,
        779.0,
        771.9,
        765.3,
        759.2,
        29.82,
        19.38,
        39.92,
        91.44,
        104.4,
        90.62,
        113.2,
        172.1,
        136.0,
        121.8,
        147.9,
        214.2,
        29.30,
        19.36,
        39.70,
        90.33,
        8.008,
        1.648,
        2.925,
        11.84,
        3.587,
        2.751,
        4.757,
        9.603,
        8.147,
        1.676,
        2.860,
        11.70,
        8.293,
        2.126,
        4.257,
        14.69,
        5.153,
        3.321,
        2.409,
        2.418,
        8.432,
        2.181,
        4.329,
        14.88,
        9.463,
        2.494,
        4.497,
        15.47,
        3.507,
        3.108,
        2.747,
        2.424,
        8.418,
        2.295,
        4.512,
        15.07,
        13.43,
        2.886,
        4.087,
        17.03,
        7.752,
        3.007,
        4.175,
        11.26,
        16.07,
        3.349,
        3.754,
        17.29,
        1.979,
        1.882,
        3.835,
        1.890,
        1.960,
        2.067,
        0.9252,
        1.679,
        0.5536,
        5.637,
        7.716,
        1.233,
        5.066,
        5.825,
        1.980,
        3.860,
        3.704,
        1.677,
        4.831,
        3.766,
        0.9408,
        1.363,
        1.440,
        2.168,
        0.6672,
        5.921,
        5.002,
        1.815,
        6.123,
        6.103,
        1.793,
        5.090,
        1.904,
        1.852,
        3.683,
        1.728,
        2.181,
        2.505,
        0.7959,
        1.607,
        0.5499,
        5.097,
        7.338,
        1.292,
        5.209,
        5.884,
        1.970,
        3.980,
        1.168,
        4.695,
        7.822,
        5.879,
        1.590,
        5.070,
        5.043,
        1.597,
        5.894,
        7.831,
        4.695,
        1.168,
        1.378,
        5.228,
        10.11,
        9.420,
        3.953,
        1.892,
        1.880,
        3.964,
        9.427,
        10.10,
        5.212,
        1.377,
        1.380,
        5.178,
        10.06,
        9.399,
        3.954,
        1.881,
        1.864,
        4.023,
        9.526,
        10.20,
        5.283,
        1.380,
        1.262,
        5.090,
        9.033,
        7.556,
        2.516,
        3.165,
        3.317,
        2.382,
        7.305,
        8.791,
        4.941,
        1.245,
      };
      for (int i = 0; i < expectedVals.Count; i++)
        Assert.Equal(expectedVals[i],
          output[i]
            .Value,
          0.05);
    }

    [Theory]
    [InlineData("Ux",
      new double[] {
        0.0,
        923.8E-6,
        0.002982,
        0.005859,
        0.009242,
      },
      6)]
    [InlineData("Uy",
      new double[] {
        0.0,
        -6.773E-6,
        -24.27E-6,
        -50.69E-6,
        -84.24E-6,
      },
      8)]
    [InlineData("Uz",
      new double[] {
        0.0,
        -0.02056,
        -0.04112,
        -0.06169,
        -0.08225,
      },
      5)]
    [InlineData("U",
      new double[] {
        0.0,
        0.02058,
        0.04123,
        0.06196,
        0.08277,
      },
      5)]
    [InlineData("Rxx",
      new double[] {
        3.711E-9,
        56.84E-9,
        101.8E-9,
        138.4E-9,
        166.9E-9,
      },
      10)]
    [InlineData("Ryy",
      new double[] {
        1.153E-6,
        7.054E-6,
        11.52E-6,
        14.55E-6,
        16.14E-6,
      },
      8)]
    [InlineData("Rzz",
      new double[] {
        0.0,
        -12.26E-9,
        -24.51E-9,
        -36.77E-9,
        -49.03E-9,
      },
      11)]
    [InlineData("R",
      new double[] {
        1.153E-6,
        7.055E-6,
        11.52E-6,
        14.55E-6,
        16.14E-6,
      },
      6)]
    public void BeamDisplacementTests(string name, double[] expectedVals, int precision = 6) {
      GH_Document doc = Document;
      IGH_Param param = Helper.FindParameter(doc, name);
      var output = (List<GH_Number>)param.VolatileData.get_Branch(0);
      for (int i = 0; i < expectedVals.Length; i++)
        Assert.Equal(expectedVals[i],
          output[i]
            .Value,
          precision);
    }

    [Theory]
    [InlineData("Fx",
      new double[] {
        -110.8,
        -110.8,
        -110.8,
        -110.8,
        -110.8,
        -110.8,
        -110.8,
        -110.8,
        -110.8,
      },
      1)]
    [InlineData("Fy",
      new double[] {
        6.332,
        6.332,
        6.332,
        6.332,
        6.332,
        6.332,
        6.332,
        6.332,
        6.332,
      },
      3)]
    [InlineData("Fz",
      new double[] {
        6.790,
        6.790,
        6.790,
        6.790,
        6.790,
        6.790,
        6.790,
        6.790,
        6.790,
      },
      3)]
    [InlineData("Fyz",
      new double[] {
        9.284,
        9.284,
        9.284,
        9.284,
        9.284,
        9.284,
        9.284,
        9.284,
        9.284,
      },
      3)]
    [InlineData("Mxx",
      new double[] {
        -0.003526,
        -0.003526,
        -0.003526,
        -0.003526,
        -0.003526,
        -0.003526,
        -0.003526,
        -0.003526,
        -0.003526,
      },
      6)]
    [InlineData("Myy",
      new double[] {
        -7.964,
        -7.221,
        -6.479,
        -5.736,
        -4.994,
        -4.251,
        -3.508,
        -2.766,
        -2.023,
      },
      3)]
    [InlineData("Mzz",
      new double[] {
        7.180,
        6.487,
        5.794,
        5.102,
        4.409,
        3.717,
        3.024,
        2.331,
        1.639,
      },
      3)]
    [InlineData("Myz",
      new double[] {
        10.72,
        9.707,
        8.692,
        7.677,
        6.662,
        5.646,
        4.632,
        3.617,
        2.603,
      },
      2)]
    public void BeamForcesTests(string name, double[] expectedVals, int precision = 6) {
      GH_Document doc = Document;
      IGH_Param param = Helper.FindParameter(doc, name);
      var output = (List<GH_Number>)param.VolatileData.get_Branch(0);
      for (int i = 0; i < expectedVals.Length; i++)
        Assert.Equal(expectedVals[i],
          output[i]
            .Value,
          precision);
    }

    [Theory]
    [InlineData("FxContour",
      new double[] {
        -110.8,
        -110.8,
        -110.8,
        -110.8,
        -110.8,
        -110.8,
        -110.8,
        -110.8,
        -110.8,
      },
      1)]
    [InlineData("MyyContour",
      new double[] {
        -7.964,
        -7.221,
        -6.479,
        -5.736,
        -4.994,
        -4.251,
        -3.508,
        -2.766,
        -2.023,
      },
      3)]
    public void Elem1dContourForcesTests(string name, double[] expectedVals, int precision = 6) {
      GH_Document doc = Document;
      IGH_Param param = Helper.FindParameter(doc, name);
      var output = (List<GH_Number>)param.VolatileData.get_Branch(0);
      for (int i = 0; i < expectedVals.Length; i++)
        Assert.Equal(expectedVals[i],
          output[i]
            .Value,
          precision);
    }

    [Fact]
    public void Elem1dContourScaledDeformationTests() {
      GH_Document doc = Document;
      IGH_Param param1 = Helper.FindParameter(doc, "ScaledResults");
      var output1 = (List<GH_Number>)param1.VolatileData.get_Branch(0);
      IGH_Param param2 = Helper.FindParameter(doc, "ScaledContours");
      var output2 = (List<GH_Number>)param2.VolatileData.get_Branch(0);
      for (int i = 0; i < output1.Count; i++)
        Assert.Equal(output2[i]
            .Value,
          output1[i]
            .Value,
          6);
    }

    [Fact]
    public void Elem1dContourStrainEnergyDensityTests() {
      GH_Document doc = Document;
      IGH_Param param = Helper.FindParameter(doc, "StrainEnergyDensityContour");
      var output = (List<GH_Number>)param.VolatileData.get_Branch(0);

      var expectedVals = new List<double>() {
        459.7E-9,
        1.029E-6,
        1.828E-6,
        2.652E-6,
        3.342E-6,
      };
      for (int i = 0; i < expectedVals.Count; i++)
        Assert.Equal(expectedVals[i],
          output[i]
            .Value,
          0.01);
    }

    [Fact]
    public void StrainEnergyDensityTests() {
      GH_Document doc = Document;
      IGH_Param param = Helper.FindParameter(doc, "StrainEnergyDensity");
      var output = (List<GH_Number>)param.VolatileData.get_Branch(0);

      var expectedVals = new List<double>() {
        459.7E-9,
        1.029E-6,
        1.828E-6,
        2.652E-6,
        3.342E-6,
        26.01E-9,
        37.16E-9,
        50.99E-9,
        67.52E-9,
        86.73E-9,
        11.17E-6,
        9.274E-6,
        7.563E-6,
        6.032E-6,
        4.683E-6,
        0.003189,
        0.005969,
        0.009000,
        0.01168,
        0.01356,
        686.5E-6,
        525.1E-6,
        388.0E-6,
        275.4E-6,
        187.2E-6,
        0.002943,
        0.005677,
        0.008734,
        0.01149,
        0.01345,
        0.005873,
        0.009208,
        0.01237,
        0.01482,
        0.01618,
        710.2E-6,
        565.0E-6,
        435.2E-6,
        322.4E-6,
        228.4E-6,
        0.3759,
        0.3179,
        0.2651,
        0.2174,
        0.1748,
        0.3318,
        0.2872,
        0.2462,
        0.2086,
        0.1744,
        0.001563,
        0.001179,
        867.1E-6,
        628.6E-6,
        462.9E-6,
        66.19E-6,
        57.07E-6,
        49.03E-6,
        42.05E-6,
        36.14E-6,
        0.001022,
        988.5E-6,
        959.3E-6,
        933.8E-6,
        912.0E-6,
        138.4E-6,
        134.2E-6,
        131.2E-6,
        129.4E-6,
        128.8E-6,
        181.9E-6,
        225.0E-6,
        594.1E-6,
        0.001289,
        0.002311,
        0.02112,
        0.01737,
        0.01426,
        0.01180,
        0.009984,
        0.01798,
        0.01268,
        0.008428,
        0.005223,
        0.003065,
        0.009267,
        0.002071,
        0.002782,
        0.01140,
        0.02792,
        0.2594,
        0.2779,
        0.3075,
        0.3483,
        0.4003,
        0.0,
        0.0,
        0.0,
        0.0,
        0.0,
        0.0,
        0.0,
        0.0,
        0.0,
        0.0,
        0.0,
        0.0,
        0.0,
        0.0,
        0.0,
      };
      for (int i = 0; i < expectedVals.Count; i++)
        Assert.Equal(expectedVals[i],
          output[i]
            .Value,
          0.01);
    }

    private static GH_Document Document => s_document ?? (s_document = OpenDocument());
    private static GH_Document s_document = null;
    private static GH_Document OpenDocument() {
      Type thisClass = MethodBase.GetCurrentMethod()
        .DeclaringType;
      string fileName = thisClass.Name + ".gh";
      fileName = fileName.Replace(thisClass.Namespace, string.Empty)
        .Replace("Tests", string.Empty);

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory())
        .Parent.Parent.Parent.Parent.FullName;
      string path = Path.Combine(new string[] {
        solutiondir,
        "ExampleFiles",
        "Components",
      });

      return Helper.CreateDocument(Path.Combine(path, fileName));
    }
  }
}
