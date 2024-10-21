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
  public class AnnotateTests {
    private static GH_Document Document => document ?? (document = OpenDocument());
    private static GH_Document document = null;

    [Fact]
    public void TestGraftedShowIds() {
      GH_Document doc = Document;
      IGH_Param param = Helper.FindParameter(doc, "Res1dIds");
      for (int branch = 1; branch <= 18; branch++) {
        int[] expectedVals = new int[] {
          branch,
          branch,
          branch,
          branch,
          branch,
        };
        var output = (List<GH_Integer>)param.VolatileData.get_Branch(new GH_Path(branch));
        for (int i = 0; i < expectedVals.Length; i++) {
          Assert.Equal(expectedVals[i], output[i].Value);
        }
      }
    }

    [Fact]
    public void TestNoWarningOrErrors() {
      Helper.TestNoRuntimeMessagesInDocument(Document, GH_RuntimeMessageLevel.Error);
      Helper.TestNoRuntimeMessagesInDocument(Document, GH_RuntimeMessageLevel.Warning, "throwWarning");

      GH_Document doc = Document;
      IGH_Param param = Helper.FindParameter(doc, "invalidTest");
      var output = (GH_Boolean)param.VolatileData.get_Branch(0)[0];
      Assert.True(output.Value);
    }

    [Theory]
    [InlineData("NodeIds", new int[] {
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
      13,
      14,
      15,
      16,
      17,
      22,
      23,
      24,
      31,
      32,
      35,
      36,
      37,
      38,
      41,
      42,
      43,
      44,
      47,
      49,
      54,
      55,
      56,
      57,
      58,
      61,
      62,
      63,
      64,
      66,
      67,
      68,
      69,
      70,
    })]
    [InlineData("Elem3dIds", new int[] {
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
      13,
      14,
      15,
    })]
    [InlineData("Mem3dId", new int[] {
      33,
    })]
    [InlineData("Elem2dIds", new int[] {
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
      13,
      14,
      15,
      16,
    })]
    [InlineData("Mem2dId", new int[] {
      22,
    })]
    [InlineData("Elem1dIds", new int[] {
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
      13,
      14,
      15,
      16,
      17,
      18,
    })]
    [InlineData("Mem1dId", new int[] {
      11,
    })]
    [InlineData("NodeRes3dIds", new int[] {
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
      13,
      14,
      15,
      16,
      17,
      22,
      23,
      24,
      31,
      32,
      35,
      36,
      37,
      38,
      41,
      42,
      43,
      44,
      47,
      49,
      54,
      55,
      56,
      57,
      58,
      61,
      62,
      63,
      64,
      66,
      67,
      68,
      69,
      70,
    })]
    [InlineData("Res3dIds", new int[] {
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
      13,
      14,
      15,
    })]
    [InlineData("NodeRes2dIds", new int[] {
      1,
      2,
      3,
      4,
      9,
      10,
      11,
      12,
      13,
      14,
      15,
      16,
      17,
      18,
      19,
      20,
      21,
      22,
      23,
      24,
      25,
      26,
      27,
      28,
      29,
      30,
      31,
      32,
      33,
    })]
    [InlineData("Res2dIds", new int[] {
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
      13,
      14,
      15,
      16,
    })]
    [InlineData("NodeRes1dIds", new int[] {
      1,
      2,
      4,
      5,
      7,
      8,
      9,
      10,
      11,
      13,
      14,
      15,
      16,
      17,
      19,
      20,
      21,
      22,
      23,
    })]
    public void TestShowIds(string name, int[] expectedVals) {
      GH_Document doc = Document;
      IGH_Param param = Helper.FindParameter(doc, name);
      var output = (List<GH_Integer>)param.VolatileData.get_Branch(0);
      for (int i = 0; i < expectedVals.Length; i++) {
        Assert.Equal(expectedVals[i], output[i].Value);
      }
    }

    [Theory]
    [InlineData("DiagramValues", new string[] {
      "-1.667 kN·m",
      "6.178 kN·m",
      "-6.374 kN·m",
      "-1.667 kN·m",
      "-7.943 kN·m",
      "-6.374 kN·m",
      "-6.374 kN·m",
      "-7.943 kN·m",
      "-1.667 kN·m",
      "-6.374 kN·m",
      "6.178 kN·m",
      "-1.667 kN·m",
    })]
    [InlineData("DiagramLineNull", new string[] {
      "False",
      "False"
    })]
    public void TestAnnotationText(string name, string[] expectedVals) {
      GH_Document doc = Document;
      IGH_Param param = Helper.FindParameter(doc, name);
      var output = (List<GH_String>)param.VolatileData.get_Branch(0);
      for (int i = 0; i < expectedVals.Length; i++) {
        Assert.Equal(expectedVals[i], output[i].Value);
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
