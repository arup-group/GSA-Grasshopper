using System.IO;
using System.Reflection;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Xunit;

namespace IntegrationTests
{
  [Collection("GrasshopperFixture collection")]
  public class Example1_GerberMember_Test
  {
    public static GH_Document Document()
    {
      string fileName = "GsaGH_" + MethodBase.GetCurrentMethod().DeclaringType + ".gh";
      fileName = fileName.Replace("IntegrationTests.", string.Empty).Replace("_Test", string.Empty);

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent.FullName;
      string path = Path.Combine(solutiondir, "ExampleFiles");
      GH_DocumentIO io = new GH_DocumentIO();
      Assert.True(File.Exists(Path.Combine(path, fileName)));
      Assert.True(io.Open(Path.Combine(path, fileName)));
      io.Document.NewSolution(true);
      return io.Document;
    }

    [Fact]
    public void MaxMyyAssert()
    {
      GH_Document doc = Document();
      GH_Param<GH_Number> param = Helper.FindComponentInDocumentByGroup<GH_Number>(doc, "MaxMyy");
      Assert.NotNull(param);
      param.CollectData();
      GH_Number output = (GH_Number)param.VolatileData.get_Branch(0)[0];
      Assert.Equal(60.028894, output.Value, 6);
    }

    [Fact]
    public void NoRuntimeErrorsTest()
    {
      Helper.TestNoRuntimeMessagesInDocument(Document(), GH_RuntimeMessageLevel.Error);
    }
  }
}
