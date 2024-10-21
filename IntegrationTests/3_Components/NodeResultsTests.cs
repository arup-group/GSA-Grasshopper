using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using Xunit;

namespace IntegrationTests.Components {
  [Collection("GrasshopperFixture collection")]
  public class NodeResultsTests {
    private static GH_Document Document => document ?? (document = OpenDocument());
    private static GH_Document document = null;

    [Fact]
    public void NodeContourScaledDeformationTests() {
      GH_Document doc = Document;
      IGH_Param param1 = Helper.FindParameter(doc, "ScaledResult");
      var output1 = (GH_Number)param1.VolatileData.get_Branch(0)[0];
      IGH_Param param2 = Helper.FindParameter(doc, "ScaledContour");
      var output2 = (GH_Number)param2.VolatileData.get_Branch(0)[0];
      Assert.Equal(output1.Value, output2.Value, 6);
    }

    [Fact]
    public void NodeContourSupportPtsCountTests() {
      GH_Document doc = Document;
      IGH_Param param = Helper.FindParameter(doc, "SupportPtsCount");
      var output1 = (GH_Integer)param.VolatileData.get_Branch(0)[0];
      Assert.Equal(23, output1.Value);
    }

    [Theory]
    [InlineData("FContour", 580.7, 66.14, 1)]
    public void NodeContourTests(
      string name, double expected1, double expected2, int precision = 6) {
      GH_Document doc = Document;
      IGH_Param param = Helper.FindParameter(doc, name);
      var output1 = (GH_Number)param.VolatileData.get_Branch(0)[0];
      var output2 = (GH_Number)param.VolatileData.get_Branch(0)[1];
      Assert.Equal(expected1, output1.Value, precision);
      Assert.Equal(expected2, output2.Value, precision);
    }

    [Theory]
    [InlineData("Ux", 0.5310, 0.7965, 4)]
    [InlineData("Uy", -0.2841, -0.4261, 4)]
    [InlineData("Uz", -16.31, -24.47, 2)]
    [InlineData("U", 16.32, 24.49, 2)]
    [InlineData("Rxx", -42.87E-6, -64.31E-6, 8)]
    [InlineData("Ryy", 34.83E-6, 52.25E-6, 8)]
    [InlineData("Rzz", -13.97E-6, -20.96E-6, 8)]
    [InlineData("R", 56.98E-6, 85.47E-6, 8)]
    public void NodeDisplacementTests(
      string name, double expected1, double expected2, int precision = 6) {
      GH_Document doc = Document;
      IGH_Param param = Helper.FindParameter(doc, name);
      var output1 = (GH_Number)param.VolatileData.get_Branch(new GH_Path(3, 1))[0];
      var output2 = (GH_Number)param.VolatileData.get_Branch(new GH_Path(3, 2))[0];
      Assert.Equal(expected1, output1.Value, precision);
      Assert.Equal(expected2, output2.Value, precision);
    }

    [Fact]
    public void PermutationIDsTests() {
      GH_Document doc = Document;
      IGH_Param param = Helper.FindParameter(doc, "Permutations");
      var output1 = (GH_Integer)param.VolatileData.get_Branch(new GH_Path(1))[0];
      Assert.Equal(1, output1.Value);

      Assert.Null(param.VolatileData.get_Branch(new GH_Path(2))[0]);

      var output31 = (GH_Integer)param.VolatileData.get_Branch(new GH_Path(3))[0];
      var output32 = (GH_Integer)param.VolatileData.get_Branch(new GH_Path(3))[1];
      Assert.Equal(1, output31.Value);
      Assert.Equal(2, output32.Value);
    }

    [Fact]
    public void ReactionForcesIDsTests() {
      GH_Document doc = Document;
      IGH_Param param = Helper.FindParameter(doc, "Rids");
      var output1 = (List<GH_Integer>)param.VolatileData.get_Branch(0);
      var expected = new List<int>() {
        1,
        2,
        3,
        4,
        5,
        6,
        7,
        8,
        9,
        10,
        11,
        12,
        3017,
        3018,
        3019,
        3020,
        3021,
        3022,
        3023,
        3024,
        3025,
        3026,
        3027,
      };

      for (int i = 0; i < expected.Count; i++) {
        Assert.Equal(expected[i], output1[i].Value);
      }
    }

    [Theory]
    [InlineData("Fx", 10.87, -12.72, 2)]
    [InlineData("Fy", 9.892, -10.43, 2)]
    [InlineData("Fz", 871.0, 65.74, 1)]
    [InlineData("F", 871.0, 66.14, 1)]
    [InlineData("Mxx", 38.73, -38.05, 2)]
    [InlineData("Myy", 41.55, -46.94, 2)]
    [InlineData("Mzz", 0.01092, -0.005168, 5)]
    [InlineData("M", 53.98, 0.5144, 2)]
    public void ReactionForcesTests(
      string name, double expected1, double expected2, int precision = 6) {
      GH_Document doc = Document;
      IGH_Param param = Helper.FindParameter(doc, name);
      var output1 = (GH_Number)param.VolatileData.get_Branch(0)[0];
      var output2 = (GH_Number)param.VolatileData.get_Branch(0)[1];
      Assert.Equal(expected1, output1.Value, precision);
      Assert.Equal(expected2, output2.Value, precision);
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
