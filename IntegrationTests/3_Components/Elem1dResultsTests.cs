using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaGH.Helpers;

using Xunit;

namespace IntegrationTests.Components {
  [Collection("GrasshopperFixture collection")]
  public class Elem1dResultsTests {
    private static GH_Document Document => document ?? (document = OpenDocument());
    private static GH_Document document = null;

    [Fact]
    public void AverageStrainEnergyDensityTests() {
      GH_Document doc = Document;
      IGH_Param param = Helper.FindParameter(doc, "AvgStrainEnergyDensity");
      var output = (List<GH_Number>)param.VolatileData.get_Branch(0);

      var expectedVals = new List<double>() {
        24.90,
83.84,
24.63,
65.18,
301.1,
64.47,
58.53,
299.9,
73.17,
54.21,
203.5,
71.82,
9.996,
10.66,
10.03,
14.16,
3.912,
14.34,
14.31,
4.268,
15.54,
14.38,
7.610,
13.46,
7.363,
4.687,
7.301,
6.320,
3.062,
6.353,
7.133,
3.278,
6.856,
10.87,
7.431,
11.18,
6.041,
6.070,
8.323,
8.432,
8.365,
9.111,
7.867,
9.447,
2.682,
1.835,
3.536,
4.628,
2.003,
3.915,
2.730,
1.936,
3.582,
3.635,
3.626,
3.536,
3.534,
3.538,
3.532,
3.540,
3.550,
1.763,
1.905,
3.614,
1.811,
1.875,
3.532,
1.781,
1.880,
3.615,
5.245,
5.244,
5.302,
5.302,
5.295,
5.326,
5.284,
5.264,
3.063,
3.140,
6.184,
3.191,
3.163,
6.138,
3.066,
3.127,
6.171,
43.73,
105.6,
39.98,
166.6,
45.27,
36.32,
197.3,
19.92,
45.25,
51.31,
19.79
      };
      for (int i = 0; i < expectedVals.Count; i++) {
        Assert.Equal(expectedVals[i], output[i].Value, DoubleComparer.Default);
      }
    }

    [Theory]
    [InlineData("Ux", new double[] {
      0.0,
-0.1616,
-0.3233,
-0.4849,
-0.6465,
    })]
    [InlineData("Uy", new double[] {
      0.0,
-0.00087348,
-0.002832,
-0.004959,
-0.006340,
    })]
    [InlineData("Uz", new double[] {
      0.0,
0.01502,
0.04055,
0.05115,
0.02138,
    })]
    [InlineData("U", new double[] {
     0.0,
0.1623,
0.3258,
0.4876,
0.6469
    })]
    [InlineData("Rxx", new double[] {
      0.0,
-16.95E-9,
-33.89E-9,
-50.84E-9,
-67.78E-9,
    })]
    [InlineData("Ryy", new double[] {
      -1.458E-6,
-28.02E-6,
-25.50E-6,
6.104E-6,
66.78E-6
    })]
    [InlineData("Rzz", new double[] {
     -29.53E-9,
-1.793E-6,
-2.509E-6,
-2.179E-6,
-802.5E-9,
    })]
    [InlineData("R", new double[] {
      1.459E-6,
28.07E-6,
25.62E-6,
6.481E-6,
66.79E-6,
    })]
    public void BeamDisplacementTests(string name, double[] expectedVals) {
      GH_Document doc = Document;
      IGH_Param param = Helper.FindParameter(doc, name);
      var output = (List<GH_Number>)param.VolatileData.get_Branch(0);
      for (int i = 0; i < expectedVals.Length; i++) {
        Assert.Equal(expectedVals[i], output[i].Value, DoubleComparer.Default);
      }
    }

    [Theory]
    [InlineData("Fx", new double[] {
      -135.5,
-135.5,
-135.5,
-135.5,
-135.5,
-135.5,
-135.5,
-135.5,
-135.5,
    }, 1)]
    [InlineData("Fy", new double[] {
     5.654,
5.654,
5.654,
5.654,
5.654,
5.654,
5.654,
5.654,
5.654,
    }, 3)]
    [InlineData("Fz", new double[] {
     1.331,
1.331,
1.331,
1.331,
1.331,
1.331,
1.331,
1.331,
1.331,
    }, 3)]
    [InlineData("Fyz", new double[] {
     5.808,
5.808,
5.808,
5.808,
5.808,
5.808,
5.808,
5.808,
5.808
    }, 3)]
    [InlineData("Mxx", new double[] {
      -0.003950,
-0.003950,
-0.003950,
-0.003950,
-0.003950,
-0.003950,
-0.003950,
-0.003950,
-0.003950,
    }, 6)]
    [InlineData("Myy", new double[] {
      -1.758,
-1.176,
-0.5937,
-0.01130,
0.5711,
1.153,
1.736,
2.318,
2.901
    }, 3)]
    [InlineData("Mzz", new double[] {
      6.442,
3.968,
1.495,
-0.9789,
-3.452,
-5.926,
-8.400,
-10.873,
-13.347
    }, 3)]
    [InlineData("Myz", new double[] {
      6.677,
4.139,
1.608,
0.9790,
3.499,
6.037,
8.577,
11.12,
13.66,
    }, 2)]
    public void BeamForcesTests(string name, double[] expectedVals, int precision = 6) {
      GH_Document doc = Document;
      IGH_Param param = Helper.FindParameter(doc, name);
      var output = (List<GH_Number>)param.VolatileData.get_Branch(0);
      for (int i = 0; i < expectedVals.Length; i++) {
        Assert.Equal(expectedVals[i], output[i].Value, precision);
      }
    }

    [Theory]
    [InlineData("FxContour", new double[] {
      -135.5,
-135.5,
-135.5,
-135.5,
-135.5,
-135.5,
-135.5,
-135.5,
-135.5,
    }, 1)]
    [InlineData("MyyContour", new double[] {
      -1.758,
-1.176,
-0.5937,
-0.01130,
0.5711,
1.153,
1.736,
2.318,
2.901
    }, 3)]
    public void Elem1dContourForcesTests(string name, double[] expectedVals, int precision = 6) {
      GH_Document doc = Document;
      IGH_Param param = Helper.FindParameter(doc, name);
      var output = (List<GH_Number>)param.VolatileData.get_Branch(0);
      for (int i = 0; i < expectedVals.Length; i++) {
        Assert.Equal(expectedVals[i], output[i].Value, precision);
      }
    }

    [Fact]
    public void Elem1dContourScaledDeformationTests() {
      GH_Document doc = Document;
      IGH_Param param1 = Helper.FindParameter(doc, "ScaledResults");
      var output1 = (List<GH_Number>)param1.VolatileData.get_Branch(0);
      IGH_Param param2 = Helper.FindParameter(doc, "ScaledContours");
      var output2 = (List<GH_Number>)param2.VolatileData.get_Branch(0);
      for (int i = 0; i < output1.Count; i++) {
        Assert.Equal(output2[i].Value, output1[i].Value, DoubleComparer.Default);
      }
    }

    [Fact]
    public void Elem1dContourStrainEnergyDensityTests() {
      GH_Document doc = Document;
      IGH_Param param = Helper.FindParameter(doc, "StrainEnergyDensityContour");
      var output = (List<GH_Number>)param.VolatileData.get_Branch(0);

      var expectedVals = new List<double>() {
        26.641624,
18.934925,
19.042608,
26.964672,
42.701118,
      };
      for (int i = 0; i < expectedVals.Count; i++) {
        Assert.Equal(expectedVals[i], output[i].Value, DoubleComparer.Default);
      }
    }

    [Fact]
    public void StrainEnergyDensityTests() {
      GH_Document doc = Document;
      IGH_Param param = Helper.FindParameter(doc, "StrainEnergyDensity");
      var output = (List<GH_Number>)param.VolatileData.get_Branch(0);

      var expectedVals = new List<double>() {
        26.641624,
18.934925,
19.042608,
26.964672,
42.701118,
      };
      for (int i = 0; i < expectedVals.Count; i++) {
        Assert.Equal(expectedVals[i], output[i].Value, DoubleComparer.Default);
      }
    }

    [Fact]
    public void CombinationStrainEnergyDensityTests() {
      GH_Document doc = Document;
      IGH_Param param = Helper.FindParameter(doc, "CombinationStrainEnergyDensity");
      var output = (List<GH_Number>)param.VolatileData.get_Branch(0);

      var expectedVals = new List<double>() {
        319.789713,
261.51206,
263.542504,
325.881045,
448.527682,
      };
      for (int i = 0; i < expectedVals.Count; i++) {
        Assert.Equal(expectedVals[i], output[i].Value, DoubleComparer.Default);
      }
    }

    [Fact]
    public void CombinationAverageStrainEnergyDensityTests() {
      GH_Document doc = Document;
      IGH_Param param = Helper.FindParameter(doc, "CombinationAverageStrainEnergyDensity");
      var output = (List<GH_Number>)param.VolatileData.get_Branch(0);

      var expectedVals = new List<double>() {
        308.773577,
1427.428253,
305.021729,
1119.967272,
6258.026143,
1109.127358,
1072.311979,
6622.261561,
1302.790141,
764.514954,
      };
      for (int i = 0; i < expectedVals.Count; i++) {
        Assert.Equal(expectedVals[i], output[i].Value, DoubleComparer.Default);
      }
    }

    private static GH_Document OpenDocument() {
      Type thisClass = MethodBase.GetCurrentMethod().DeclaringType;
      string fileName = thisClass.Name + ".gh";
      fileName = fileName.Replace(thisClass.Namespace, string.Empty).Replace("Tests", string.Empty);

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent
       .Parent.FullName;
      string path = Path.Combine(new string[] {
        solutiondir,
        "ExampleFiles",
        "Components",
      });

      return Helper.CreateDocument(Path.Combine(path, fileName));
    }
  }
}
