using System.IO;
using System.Reflection;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Xunit;

namespace IntegrationTests.Parameters
{
  [Collection("GrasshopperFixture collection")]
  public class EditBool6Test
  {
    public static GH_Document Document()
    {
      string fileName = MethodBase.GetCurrentMethod().DeclaringType + ".gh";
      fileName = fileName.Replace("IntegrationTests.Parameters.", string.Empty);

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent.FullName;
      string path = Path.Combine(new string[] { solutiondir, "ExampleFiles", "Parameters", "1_Properties" });

      return Helper.CreateDocument(Path.Combine(path, fileName));
    }

    [Theory]
    [InlineData("X", false)]
    [InlineData("Y", true)]
    [InlineData("Z", false)]
    [InlineData("XX", true)]
    [InlineData("YY", false)]
    [InlineData("ZZ", true)]
    public void OutputTest(string groupIdentifier, bool expected)
    {
      GH_Document doc = Document();
      GH_Param<GH_Boolean> param = Helper.FindComponentInDocumentByGroup<GH_Boolean>(doc, groupIdentifier);
      Assert.NotNull(param);
      param.CollectData();
      GH_Boolean output = (GH_Boolean)param.VolatileData.get_Branch(0)[0];
      Assert.Equal(expected, output.Value);
    }

    [Fact]
    public void NoRuntimeErrorTest()
    {
      Helper.TestNoRuntimeMessagesInDocument(Document(), GH_RuntimeMessageLevel.Error);
    }
  }
}
