using System;
using System.IO;
using System.Reflection;

using Grasshopper.Kernel;

using Xunit;

namespace IntegrationTests.Components {
  [Collection("GrasshopperFixture collection")]
  public class TrGen_10DisplayTests {
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
    }

    [Fact(Skip = "GSA team should make this file error and warning free https://samples.oasys-software.com/gsa/10.2/Training_General/TrGen_10.gwb")]
    public void NoRuntimeWarningTest() {
      Helper.TestNoRuntimeMessagesInDocument(Document, GH_RuntimeMessageLevel.Warning);
    }

    [Theory]
    [InlineData("Annotate", true)]
    [InlineData("Preview", true)]
    [InlineData("Diagrams", true)]
    [InlineData("Results", true)]
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
