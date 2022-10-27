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
    public static GH_Document? Document;

    public static GH_Document GetDocument()
    {
      if (Document == null)
      {
        string fileName = MethodBase.GetCurrentMethod().DeclaringType + ".gh";
        fileName = fileName.Replace("IntegrationTests.Parameters.", string.Empty);

        string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent.FullName;
        string path = Path.Combine(new string[] { solutiondir, "ExampleFiles", "Parameters", "1_Properties" });

        Document = Helper.CreateDocument(Path.Combine(path, fileName));
      }
      return Document;
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
      GH_Document doc = GetDocument();
      IGH_Param param = Helper.FindParameter(doc, groupIdentifier);
      Assert.NotNull(param);
      param.CollectData();
      GH_Boolean output = (GH_Boolean)param.VolatileData.get_Branch(0)[0];
      Assert.Equal(expected, output.Value);
    }

    [Fact]
    public void NoRuntimeErrorTest()
    {
      Helper.TestNoRuntimeMessagesInDocument(GetDocument(), GH_RuntimeMessageLevel.Error);
    }
  }
}
