using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using Xunit;

namespace IntegrationTests.Components {
  [Collection("GrasshopperFixture collection")]
  public class Elem2dResultsTests {
    private static GH_Document Document => document ?? (document = OpenDocument());
    private static GH_Document document = null;

    [Theory]
    [InlineData("MyContours", new double[] {
      2.814,
      10.57,
      20.10,
      27.78,
      31.92,
      2.420,
      2.066,
      3.138,
      4.540,
      5.461,
      3.408,
      -1.911,
      -5.824,
      -8.287,
      -9.454,
      4.627,
      -3.393,
      -10.09,
      -14.77,
      -17.16,
      5.366,
      -3.804,
      -11.74,
      -17.42,
      -20.36,
    }, 2)]
    [InlineData("MyContoursCentrePointValues", new double[] {
      2.814,
      10.57,
      20.10,
      27.78,
      31.92,
      2.420,
      2.066,
      3.138,
      4.540,
      5.461,
      3.408,
      -1.911,
      -5.824,
      -8.287,
      -9.454,
      4.627,
      -3.393,
      -10.09,
      -14.77,
      -17.16,
      5.366,
      -3.804,
      -11.74,
      -17.42,
      -20.36,
    }, 2)]
    public void Elem2dContourForcesTests(string name, double[] expectedVals, int precision = 6) {
      GH_Document doc = Document;
      IGH_Param param = Helper.FindParameter(doc, name);
      var output = (List<GH_Number>)param.VolatileData.get_Branch(0);
      for (int i = 0; i < expectedVals.Length; i++) {
        Assert.Equal(expectedVals[i], output[i].Value, precision);
      }
    }

    [Fact]
    public void Elem2dContourScaledDeformationTests() {
      GH_Document doc = Document;
      IGH_Param param1 = Helper.FindParameter(doc, "DeformedValue");
      var output1 = (GH_Number)param1.VolatileData.get_Branch(0)[0];
      IGH_Param param2 = Helper.FindParameter(doc, "ContourValueScaled");
      var output2 = (GH_Number)param2.VolatileData.get_Branch(0)[0];
      IGH_Param param3 = Helper.FindParameter(doc, "ResultScaled");
      var output3 = (GH_Number)param3.VolatileData.get_Branch(0)[0];

      Assert.Equal(output1.Value, output3.Value, 4);
      Assert.Equal(output2.Value, output3.Value, 4);
    }

    [Fact]
    public void Elem2dContourStressTests() {
      GH_Document doc = Document;
      IGH_Param param = Helper.FindParameter(doc, "yzTopContour");
      var output = (List<GH_Number>)param.VolatileData.get_Branch(0);

      var expectedVals = new List<double>() {
        -83.78,
        -87.78,
        -59.51,
        -55.46,
        -85.78,
        -73.65,
        -57.49,
        -69.62,
        -71.63,
      };
      for (int i = 0; i < expectedVals.Count; i++) {
        Assert.Equal(expectedVals[i], output[i].Value, 2);
      }
    }

    [Theory]
    [InlineData("Ux", new double[] {
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
    }, 1)]
    [InlineData("Uy", new double[] {
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
    }, 1)]
    [InlineData("Uz", new double[] {
      -0.7564,
      -0.8108,
      -0.8694,
      -0.8108,
      -0.7972,
      -0.8548,
      -0.8548,
      -0.7972,
      -0.8401,
    }, 4)]
    [InlineData("U", new double[] {
      0.7564,
      0.8108,
      0.8694,
      0.8108,
      0.7972,
      0.8548,
      0.8548,
      0.7972,
      0.8401,
    }, 4)]
    public void Elem2dDisplacementTests(string name, double[] expectedVals, int precision = 6) {
      GH_Document doc = Document;
      IGH_Param param = Helper.FindParameter(doc, name);
      var output = (List<GH_Number>)param.VolatileData.get_Branch(0);
      for (int i = 0; i < expectedVals.Length; i++) {
        Assert.Equal(expectedVals[i], output[i].Value, precision);
      }
    }

    [Theory]
    [InlineData("NxComb", new double[] {
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
    }, 1)]
    [InlineData("NyComb", new double[] {
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
    }, 1)]
    [InlineData("NxyComb", new double[] {
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
    }, 1)]
    [InlineData("QxComb", new double[] {
      11.005,
      17.088,
      -4.239,
      -3.623,
      14.047,
      6.425,
      -3.931,
      3.691,
      5.058,
    }, 3)]
    [InlineData("QyComb", new double[] {
      11.005,
      -3.623,
      -4.239,
      17.088,
      3.691,
      -3.931,
      6.425,
      14.047,
      5.058,
    }, 3)]
    [InlineData("MxComb", new double[] {
      1.634,
      3.916,
      7.018,
      14.30,
      2.476,
      3.974,
      10.361,
      6.474,
      4.9254,
    }, 3)]
    [InlineData("MyComb", new double[] {
      1.634,
      14.30,
      7.018,
      3.916,
      6.474,
      10.361,
      3.974,
      2.476,
      4.9254,
    }, 3)]
    [InlineData("MxyComb", new double[] {
      -0.4632,
      1.336,
      12.692,
      1.336,
      0.8351,
      7.413,
      7.413,
      0.8351,
      4.522,
    }, 3)]
    [InlineData("M*xComb", new double[] {
      2.097,
      5.253,
      19.71,
      15.637,
      3.311,
      11.387,
      17.773,
      7.309,
      9.448,
    }, 3)]
    [InlineData("M*yComb", new double[] {
      2.097,
      15.637,
      19.71,
      5.253,
      7.309,
      17.773,
      11.387,
      3.311,
      9.448,
    }, 3)]
    public void Elem2dForceCombinationTests(string name, double[] expectedVals, int precision = 6) {
      GH_Document doc = Document;
      IGH_Param param = Helper.FindParameter(doc, name);
      var output = (List<GH_Number>)param.VolatileData.get_Branch(0);
      for (int i = 0; i < expectedVals.Length; i++) {
        Assert.Equal(expectedVals[i], output[i].Value, precision);
      }
    }

    [Theory]
    [InlineData("Nx", new double[] {
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
    }, 1)]
    [InlineData("Ny", new double[] {
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
    }, 1)]
    [InlineData("Nxy", new double[] {
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
    }, 1)]
    [InlineData("Qx", new double[] {
      3.393,
      2.756,
      0.1455,
      1.992,
      3.075,
      1.451,
      1.069,
      2.692,
      2.072,
    }, 3)]
    [InlineData("Qy", new double[] {
      -41.89,
      -43.89,
      -29.75,
      -27.73,
      -42.89,
      -36.82,
      -28.74,
      -34.81,
      -35.82,
    }, 2)]
    [InlineData("Mx", new double[] {
      9.117,
      9.704,
      0.4307,
      0.2494,
      9.494,
      5.474,
      0.4240,
      5.090,
      5.366,
    }, 3)]
    [InlineData("My", new double[] {
      46.94,
      49.77,
      15.16,
      13.79,
      48.77,
      32.55,
      14.89,
      30.45,
      31.92,
    }, 2)]
    [InlineData("Mxy", new double[] {
      0.3305,
      -0.1698,
      0.1910,
      3.3354,
      0.08636,
      0.08220,
      1.7692,
      1.9045,
      0.9994,
    }, 4)]
    public void Elem2dForcesTests(string name, double[] expectedVals, int precision = 6) {
      GH_Document doc = Document;
      IGH_Param param = Helper.FindParameter(doc, name);
      var output = (List<GH_Number>)param.VolatileData.get_Branch(0);
      for (int i = 0; i < expectedVals.Length; i++) {
        Assert.Equal(expectedVals[i], output[i].Value, precision);
      }
    }

    [Theory]
    [InlineData("xx", new double[] {
      -218.8,
      -232.9,
      -10.34,
      -5.987,
      -227.9,
      -131.4,
      -10.17,
      -122.2,
      -128.8,
    }, 1)]
    [InlineData("yy", new double[] {
      -1127,
      -1194,
      -363.8,
      -330.9,
      -1171,
      -781.1,
      -357.4,
      -730.7,
      -766.0,
    }, 0)]
    [InlineData("zz", new double[] {
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
    }, 1)]
    [InlineData("xy", new double[] {
      -7.932,
      4.074,
      -4.584,
      -80.05,
      -2.073,
      -1.973,
      -42.46,
      -45.71,
      -23.98,
    }, 2)]
    [InlineData("yz", new double[] {
      -83.78,
      -87.78,
      -59.51,
      -55.46,
      -85.78,
      -73.65,
      -57.49,
      -69.62,
      -71.63,
    }, 2)]
    [InlineData("zx", new double[] {
      6.785,
      5.513,
      0.2910,
      3.983,
      6.149,
      2.902,
      2.137,
      5.384,
      4.143,
    }, 3)]
    [InlineData("Cxx", new double[] {
      -0.3829,
-0.4076,
-0.01809,
-0.01048,
-0.3988,
-0.2299,
-0.01781,
-0.2138,
-0.2254
    }, 3)]
    [InlineData("Cyy", new double[] {
      -1.972,
-2.090,
-0.6367,
-0.5791,
-2.049,
-1.367,
-0.6255,
-1.279,
-1.340,
    }, 3)]
    [InlineData("Czz", new double[] {
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
      0.0,
    }, 1)]
    [InlineData("Cxy", new double[] {
      -0.01388,
0.007131,
-0.008022,
-0.1401,
-0.003627,
-0.003452,
-0.07431,
-0.07999,
-0.04197,
    }, 4)]
    [InlineData("Cyz", new double[] {
      -0.1466,
-0.1536,
-0.1041,
-0.09706,
-0.1501,
-0.1289,
-0.1006,
-0.1218,
-0.1254
    }, 4)]
    [InlineData("Czx", new double[] {
      0.01187,
0.009647,
509.3E-6,
0.006971,
0.01076,
0.005078,
0.003740,
0.009423,
0.007250,
    }, 5)]
    public void Elem2dStressTests(string name, double[] expectedVals, int precision = 6) {
      GH_Document doc = Document;
      IGH_Param param = Helper.FindParameter(doc, name);
      var output = (List<GH_Number>)param.VolatileData.get_Branch(0);
      for (int i = 0; i < expectedVals.Length; i++) {
        Assert.Equal(expectedVals[i], output[i].Value, precision);
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
