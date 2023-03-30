using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Xunit;

namespace IntegrationTests.ExampleFiles {
  [Collection("GrasshopperFixture collection")]
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  public class Example6_3dBoxes_Test {
    public static GH_Document Document() {
      Type thisClass = MethodBase.GetCurrentMethod()
        .DeclaringType;
      string fileName = "GsaGH_" + thisClass.Name + ".gh";
      fileName = fileName.Replace(thisClass.Namespace, string.Empty)
        .Replace("_Test", string.Empty);

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory())
        .Parent.Parent.Parent.Parent.FullName;
      string path = Path.Combine(solutiondir, "ExampleFiles");

      return Helper.CreateDocument(Path.Combine(path, fileName));
    }

    [Fact]
    public void MaxDeflectionTest() {
      IGH_Param param1 = Helper.FindParameter(Document(), "TestMaxValue");
      var output1 = (GH_Number)param1.VolatileData.get_Branch(0)[0];

      IGH_Param param2 = Helper.FindParameter(Document(), "TestDeflectedShape");
      var output2 = (GH_Number)param2.VolatileData.get_Branch(0)[0];

      Assert.Equal(output1.Value, output2.Value, 2);
      Assert.NotEqual(0.0, output1.Value);
    }

    [Fact]
    public void NoRuntimeErrorsTest()
      => Helper.TestNoRuntimeMessagesInDocument(Document(), GH_RuntimeMessageLevel.Error);
  }
}
