using System.IO;
using System.Reflection;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Xunit;

namespace IntegrationTests.Parameters {

  [Collection("GrasshopperFixture collection")]
  public class CreateEditBucklingFactorsTest {

    #region Properties + Fields
    private static GH_Document Document => s_document ?? (s_document = OpenDocument());
    private static GH_Document s_document = null;
    #endregion Properties + Fields

    #region Public Methods
    [Fact]
    public void NoRuntimeErrorTest()
      => Helper.TestNoRuntimeMessagesInDocument(Document, GH_RuntimeMessageLevel.Error);

    [Fact]
    public void OutputTest() {
      GH_Document doc = Document;

      IGH_Param y = Helper.FindParameter(doc, "TestLsy");
      var outputY = (GH_Number)y.VolatileData.get_Branch(0)[0];
      Assert.Equal(10, outputY.Value);

      IGH_Param z = Helper.FindParameter(doc, "TestLsz");
      var outputZ = (GH_Number)z.VolatileData.get_Branch(0)[0];
      Assert.Equal(15, outputZ.Value);

      IGH_Param lt = Helper.FindParameter(doc, "TestLtb");
      var outputLtb = (GH_Number)lt.VolatileData.get_Branch(0)[0];
      Assert.Equal(20, outputLtb.Value);
    }

    #endregion Public Methods

    #region Private Methods
    private static GH_Document OpenDocument() {
      string fileName = MethodBase.GetCurrentMethod()
          .DeclaringType
        + ".gh";
      fileName = fileName.Replace("IntegrationTests.Parameters.", string.Empty);
      fileName = fileName.Replace("Test", string.Empty);

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
