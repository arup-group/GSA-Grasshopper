using System.IO;
using System.Reflection;

using Grasshopper.Kernel;

using GsaGH.Parameters;

using OasysUnits;
using OasysUnits.Units;

using Xunit;

namespace IntegrationTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class CreateOffsetTest {
    public static GH_Document Document => document ?? (document = OpenDocument());
    private static GH_Document document = null;

    [Fact]
    public void NoRuntimeErrorTest() {
      Helper.TestNoRuntimeMessagesInDocument(Document, GH_RuntimeMessageLevel.Error);
    }

    [Theory]
    [InlineData("Of", 1, 2, 3, 4, LengthUnit.Meter)]
    public void OutputTest(
      string groupIdentifier, double expectedX1, double expectedX2, double expectedY,
      double expectedZ, LengthUnit expectedUnit) {
      GH_Document doc = Document;
      IGH_Param param = Helper.FindParameter(doc, groupIdentifier);
      var output = (GsaOffsetGoo)param.VolatileData.get_Branch(0)[0];
      GsaOffset offset = output.Value;
      Assert.Equal(new Length(expectedX1, expectedUnit), offset.X1);
      Assert.Equal(new Length(expectedX2, expectedUnit), offset.X2);
      Assert.Equal(new Length(expectedY, expectedUnit), offset.Y);
      Assert.Equal(new Length(expectedZ, expectedUnit), offset.Z);
    }

    private static GH_Document OpenDocument() {
      string fileName = MethodBase.GetCurrentMethod().DeclaringType + ".gh";
      fileName = fileName.Replace("IntegrationTests.Parameters.", string.Empty);

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent
       .Parent.FullName;
      string path = Path.Combine(new string[] {
        solutiondir,
        "ExampleFiles",
        "Parameters",
        "1_Properties",
      });

      return Helper.CreateDocument(Path.Combine(path, fileName));
    }
  }
}
