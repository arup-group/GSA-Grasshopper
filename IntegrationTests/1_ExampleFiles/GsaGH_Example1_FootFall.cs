using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using Xunit;

namespace IntegrationTests.ExampleFiles {
  [Collection("GrasshopperFixture collection")]
  public class Example1_FootFall_Test {

    public static GH_Document Document() {
      Type thisClass = MethodBase.GetCurrentMethod().DeclaringType;
      string fileName = "GsaGH_" + thisClass.Name + ".gh";
      fileName = fileName.Replace(thisClass.Namespace, string.Empty).Replace("_Test", string.Empty);

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent
       .Parent.FullName;
      string path = Path.Combine(solutiondir, "ExampleFiles");

      return Helper.CreateDocument(Path.Combine(path, fileName));
    }


    [Fact]
    public void NoRuntimeErrorsTest() {
      Helper.TestNoRuntimeMessagesInDocument(Document(), GH_RuntimeMessageLevel.Error);
    }
  }
}
