using System.IO;
using System.Reflection;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Xunit;

namespace IntegrationTests.Parameters
{
  [Collection("GrasshopperFixture collection")]
  public class GsaBool6Test
  {
    public static GH_Document Document()
    {
      string fileName = MethodBase.GetCurrentMethod().DeclaringType + ".gh";
      fileName = fileName.Replace("IntegrationTests.Parameters.", string.Empty);

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent.FullName;
      string path = Path.Combine(solutiondir, "ExampleFiles");
      GH_DocumentIO io = new GH_DocumentIO();
      Assert.True(File.Exists(Path.Combine(path, fileName)));
      Assert.True(io.Open(Path.Combine(path, fileName)));
      io.Document.NewSolution(true);
      return io.Document;
    }

    [Theory]
    [InlineData("X", true)]
    [InlineData("Y", false)]
    [InlineData("Z", true)]
    [InlineData("XX", false)]
    [InlineData("YY", true)]
    [InlineData("ZZ", false)]
    public void OutputTest(string groupIdentifier, bool expected)
    {
      GH_Document doc = GsaBool6Test.Document();
      GH_Param<GH_Boolean> param = Helper.FindComponentInDocumentByGroup<GH_Boolean>(doc, groupIdentifier);
      Assert.NotNull(param);
      param.CollectData();
      GH_Boolean output = (GH_Boolean)param.VolatileData.get_Branch(0)[0];
      Assert.Equal(expected, output.Value);
    }

    [Fact]
    public void NoRuntimeErrorsTest()
    {
      Helper.TestNoRuntimeMessagesInDocument(GsaBool6Test.Document(), GH_RuntimeMessageLevel.Error);
    }
  }
}
