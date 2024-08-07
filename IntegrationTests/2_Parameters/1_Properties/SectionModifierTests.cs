using System.IO;
using System.Reflection;

using Grasshopper.Kernel;

using Xunit;

namespace IntegrationTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class SectionModifierTests {
    public static GH_Document Document => document ?? (document = OpenDocument());
    private static GH_Document document = null;

    [Fact]
    public void NoRuntimeErrorTest() {
      Helper.TestNoRuntimeMessagesInDocument(Document, GH_RuntimeMessageLevel.Error);
      Helper.TestNoRuntimeMessagesInDocument(Document, GH_RuntimeMessageLevel.Warning);
    }

    [Theory]
    [InlineData("TOvalues", new double[] {
      3600.0,
      1.08e+6,
      1.08e+6,
      1825199.9568,
      0.833333,
      0.833333,
      0.36,
      500,
    })]
    [InlineData("TObooleans", new bool[] {
      false,
      false,
    })]
    [InlineData("TOstressOption", 1)]
    [InlineData("BYvalues", new double[] {
      50,
      105,
      70,
      30,
      35,
      45,
      90,
      1250,
    })]
    [InlineData("BYbooleans", new bool[] {
      true,
      true,
    })]
    [InlineData("BYstressOption", 2)]
    public void Test(string groupIdentifier, object expected) {
      IGH_Param param = Helper.FindParameter(Document, groupIdentifier);
      Helper.TestGhPrimitives(param, expected);
    }

    private static GH_Document OpenDocument() {
      string fileName = MethodBase.GetCurrentMethod().DeclaringType + ".gh";
      fileName = fileName.Replace("IntegrationTests.Parameters.", string.Empty);

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent
       .Parent.FullName;
      string path = Path.Combine(new string[] {
        solutiondir,
        "ExampleFiles",
        "Parameters",
        "1_Properties",
      });

      return Helper.CreateDocument(Path.Combine(path, fileName));
    }
  }
}
