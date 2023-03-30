using System.IO;
using System.Reflection;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Xunit;

namespace IntegrationTests.Parameters {

  [Collection("GrasshopperFixture collection")]
  public class EditBool6Test {

    #region Properties + Fields
    public static GH_Document Document => s_document ?? (s_document = OpenDocument());
    private static GH_Document s_document = null;
    #endregion Properties + Fields

    #region Public Methods
    [Fact]
    public void NoRuntimeErrorTest()
      => Helper.TestNoRuntimeMessagesInDocument(Document, GH_RuntimeMessageLevel.Error);

    [Theory]
    [InlineData("X", false)]
    [InlineData("Y", true)]
    [InlineData("Z", false)]
    [InlineData("XX", true)]
    [InlineData("YY", false)]
    [InlineData("ZZ", true)]
    public void OutputTest(string groupIdentifier, bool expected) {
      GH_Document doc = Document;
      IGH_Param param = Helper.FindParameter(doc, groupIdentifier);
      var output = (GH_Boolean)param.VolatileData.get_Branch(0)[0];
      Assert.Equal(expected, output.Value);
    }

    #endregion Public Methods

    #region Private Methods
    private static GH_Document OpenDocument() {
      string fileName = MethodBase.GetCurrentMethod()
          .DeclaringType
        + ".gh";
      fileName = fileName.Replace("IntegrationTests.Parameters.", string.Empty);

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory())
        .Parent.Parent.Parent.Parent.FullName;
      string path = Path.Combine(new string[] {
        solutiondir,
        "ExampleFiles",
        "Parameters",
        "1_Properties",
      });

      return Helper.CreateDocument(Path.Combine(path, fileName));
    }

    #endregion Private Methods
  }
}
