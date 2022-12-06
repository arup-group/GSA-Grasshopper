using System.IO;
using System.Reflection;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Xunit;

namespace IntegrationTests.Parameters
{
  [Collection("GrasshopperFixture collection")]
  public class CreateEditBucklingFactorsTest
  {
    public static GH_Document Document()
    {
      string fileName = MethodBase.GetCurrentMethod().DeclaringType + ".gh";
      fileName = fileName.Replace("IntegrationTests.Parameters.", string.Empty);
      fileName = fileName.Replace("Test", string.Empty);

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent.FullName;
      string path = Path.Combine(new string[] { solutiondir, "ExampleFiles", "Parameters", "1_Properties" });
      
      return Helper.CreateDocument(Path.Combine(path, fileName));
    }

    [Fact]
    public void OutputTest()
    {
      GH_Document doc = CreateEditBucklingFactorsTest.Document();

      IGH_Param y = Helper.FindParameter(doc, "TestLsy");
      GH_Number outputY = (GH_Number)y.VolatileData.get_Branch(0)[0];
      Assert.Equal(10, outputY.Value);

      IGH_Param z = Helper.FindParameter(doc, "TestLsz");
      GH_Number outputZ = (GH_Number)z.VolatileData.get_Branch(0)[0];
      Assert.Equal(15, outputZ.Value);

      IGH_Param lt = Helper.FindParameter(doc, "TestLtb");
      GH_Number outputLTB = (GH_Number)lt.VolatileData.get_Branch(0)[0];
      Assert.Equal(20, outputLTB.Value);
    }

    [Fact]
    public void NoRuntimeErrorTest()
    {
      Helper.TestNoRuntimeMessagesInDocument(CreateEditBucklingFactorsTest.Document(), GH_RuntimeMessageLevel.Error);
    }
  }
}
