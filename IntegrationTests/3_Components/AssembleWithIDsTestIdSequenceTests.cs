using System;
using System.IO;
using System.Reflection;

using Grasshopper.Kernel;

using Xunit;

namespace IntegrationTests.Components {
  [Collection("GrasshopperFixture collection")]
  public class AssembleWithIDsTestIdSequenceTests {
    public static GH_Document Document {
      get {
        if (document == null) {
          document = OpenDocument();
        }

        return document;
      }
    }
    private static GH_Document document = null;

    [Fact]
    public void NoRuntimeErrorTest() {
      Helper.TestNoRuntimeMessagesInDocument(Document, GH_RuntimeMessageLevel.Error);
      Helper.TestNoRuntimeMessagesInDocument(Document, GH_RuntimeMessageLevel.Warning);
    }

    [Theory]
    [InlineData("NPoints", 401)]
    [InlineData("FixedId", 5)]
    [InlineData("NodeSame", true)]
    [InlineData("Elem1dIds", new int[] {
      1,
      2,
      3,
      14,
    })]
    [InlineData("Elem1dSame", true)]
    [InlineData("Mem1dIds", new int[] {
      1,
      2,
      3,
      14,
    })]
    [InlineData("Mem1dSame", true)]
    [InlineData("Mem2dIds", new int[] {
      1,
      2,
      3,
    })]
    [InlineData("Mem2dSame", true)]
    public void Test(string groupIdentifier, object expected) {
      IGH_Param param = Helper.FindParameter(Document, groupIdentifier);
      Helper.TestGhPrimitives(param, expected);
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
