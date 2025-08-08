using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Grasshopper.Kernel;

using Xunit;

namespace IntegrationTests.Components {
  [Collection("GrasshopperFixture collection")]
  public class Prop2dAxisTests {
    private static GH_Document Document => document ?? (document = OpenDocument());
    private static GH_Document document = null;

    [Theory]
    [InlineData("RotationAngleTest", 0.994838)]
    [InlineData("paOrigX", 3.5)]
    [InlineData("paOrigY", 2.5)]
    [InlineData("paOrigZ", 0.0)]
    [InlineData("paZX", 0.0)]
    [InlineData("paZY", 0.0)]
    [InlineData("paZZ", 1.0)]
    [InlineData("RotationAngleCheck", 0)]
    [InlineData("AxisRotationCheck", 0)]
    public void Test(string groupIdentifier, object expected) {
      IGH_Param param = Helper.FindParameter(Document, groupIdentifier);
      Helper.TestGhPrimitives(param, expected);
    }

    [Theory]
    [InlineData("elemOrigX", 3.5, 35)]
    [InlineData("elemOrigY", 2.5, 35)]
    [InlineData("elemOrigZ", 0.0, 35)]
    [InlineData("elemZX", 0.0, 35)]
    [InlineData("elemZY", 0.0, 35)]
    [InlineData("elemZZ", 1.0, 35)]
    public void TestList(
      string groupIdentifier, double expected, int listLength) {
      var expecteds = new List<double>();
      for (int i = 0; i < listLength; i++) {
        expecteds.Add(expected);
      }

      IGH_Param param = Helper.FindParameter(Document, groupIdentifier);
      Helper.TestGhPrimitives(param, expecteds.ToArray());
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
