using System;
using System.IO;
using System.Reflection;

using Grasshopper.Kernel;

using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace IntegrationTests.Components {
  [Collection("GrasshopperFixture collection")]
  public class LoadPanelTests {
    private static GH_Document Document
      => document ?? (document = OpenDocument());
    private static GH_Document document;


    [Fact]
    public void TestNoWarningOrErrors() {
      Assert.NotNull(Document);
      Helper.TestNoRuntimeMessagesInDocument(Document, GH_RuntimeMessageLevel.Error);
      Helper.TestNoRuntimeMessagesInDocument(Document, GH_RuntimeMessageLevel.Warning);
    }

    public static GH_Document OpenDocument() {
      string[] paths = new[] {
        "ExampleFiles",
        "Components",
      };
      Type thisClass = MethodBase.GetCurrentMethod().DeclaringType;
      string fileName = $"{thisClass.Name}.gh";
      fileName = fileName.Replace(thisClass.Namespace, string.Empty).Replace("Tests", string.Empty);

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent
       .Parent.FullName;
      var pathsWithSolution = new string[paths.Length + 1];
      pathsWithSolution[0] = solutiondir;
      Array.Copy(paths, 0, pathsWithSolution, 1, paths.Length);
      string path = Path.Combine(pathsWithSolution);

      return Helper.CreateDocument(Path.Combine(path, fileName));
    }

  }
}
