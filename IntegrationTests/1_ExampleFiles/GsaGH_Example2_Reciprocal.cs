using System;
using System.IO;
using System.Reflection;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Xunit;

namespace IntegrationTests.ExampleFiles
{
  [Collection("GrasshopperFixture collection")]
  public class Example2_Reciprocal_Test
  {
    public static GH_Document Document()
    {
      Type thisClass = MethodBase.GetCurrentMethod().DeclaringType;
      string fileName = "GsaGH_" + thisClass.Name + ".gh";
      fileName = fileName.Replace(thisClass.Namespace, string.Empty).Replace("_Test", string.Empty);

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent.FullName;
      string path = Path.Combine(solutiondir, "ExampleFiles");

      return Helper.CreateDocument(Path.Combine(path, fileName));
    }

    [Fact]
    public void MaxMyyAssert()
    {
      IGH_Param param = Helper.FindParameter(Document(), "MaxMyy");
      GH_Number output = (GH_Number)param.VolatileData.get_Branch(0)[0];
      Assert.Equal(-157.193672, output.Value, 6);
    }

    [Fact]
    public void SumLoadForceAssert()
    {
      IGH_Param param = Helper.FindParameter(Document(), "SumLoadForce");
      GH_Boolean output = (GH_Boolean)param.VolatileData.get_Branch(0)[0];
      Assert.True(output.Value);
    }

    [Fact]
    public void NoRuntimeErrorsTest()
    {
      Helper.TestNoRuntimeMessagesInDocument(Document(), GH_RuntimeMessageLevel.Error);
    }
  }
}
