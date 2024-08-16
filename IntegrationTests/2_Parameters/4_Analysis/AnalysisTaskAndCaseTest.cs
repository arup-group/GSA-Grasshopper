using System.IO;
using System.Reflection;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using Xunit;

namespace IntegrationTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class AnalysisTaskAndCaseTest {
    public static GH_Document Document => document ?? (document = OpenDocument());
    private static GH_Document document = null;

    [Fact]
    public void AnalysisTaskNameTest() {
      IGH_Param param = TestHelper(MethodBase.GetCurrentMethod()
       .Name.Replace("Test", string.Empty));
      var output0 = (GH_String)param.VolatileData.get_Branch(0)[0];

      Assert.Equal("Task 1", output0.Value);
    }

    [Fact]
    public void AnalysisTaskDescriptionsTest() {
      IGH_Param param = TestHelper(MethodBase.GetCurrentMethod()
       .Name.Replace("Test", string.Empty));
      var output0 = (GH_String)param.VolatileData.get_Branch(0)[0];
      var output1 = (GH_String)param.VolatileData.get_Branch(0)[1];

      Assert.Equal("GSA AnalysisCase (ID:1 'DL' L1)", output0.Value);
      Assert.Equal("GSA AnalysisCase (ID:2 'LL' L2)", output1.Value);
    }

    [Fact]
    public void AnalysisTaskTypeTest() {
      IGH_Param param = TestHelper(MethodBase.GetCurrentMethod()
       .Name.Replace("Test", string.Empty));
      var output0 = (GH_String)param.VolatileData.get_Branch(0)[0];

      Assert.Equal("Static", output0.Value);
    }

    [Fact]
    public void AnalysisTaskIdTest() {
      IGH_Param param = TestHelper(MethodBase.GetCurrentMethod()
       .Name.Replace("Test", string.Empty));
      var output0 = (GH_Integer)param.VolatileData.get_Branch(0)[0];

      Assert.Equal(1, output0.Value);
    }

    [Fact]
    public void NoRuntimeErrorTest() {
      Helper.TestNoRuntimeMessagesInDocument(Document, GH_RuntimeMessageLevel.Error);
    }

    [Fact]
    public void AnalysisCaseNameTest() {
      IGH_Param param = TestHelper(MethodBase.GetCurrentMethod()
       .Name.Replace("Test", string.Empty));
      var output0 = (GH_String)param.VolatileData.get_Branch(0)[0];
      var output1 = (GH_String)param.VolatileData.get_Branch(0)[1];
      var output2 = (GH_String)param.VolatileData.get_Branch(0)[2];
      var output3 = (GH_String)param.VolatileData.get_Branch(0)[3];

      Assert.Equal("DL", output0.Value);
      Assert.Equal("LL", output1.Value);
      Assert.Equal("DL", output2.Value);
      Assert.Equal("LL", output3.Value);
    }

    [Fact]
    public void AnalysisCaseDescriptionTest() {
      IGH_Param param = TestHelper(MethodBase.GetCurrentMethod()
       .Name.Replace("Test", string.Empty));
      var output0 = (GH_String)param.VolatileData.get_Branch(0)[0];
      var output1 = (GH_String)param.VolatileData.get_Branch(0)[1];
      var output2 = (GH_String)param.VolatileData.get_Branch(0)[2];
      var output3 = (GH_String)param.VolatileData.get_Branch(0)[3];

      Assert.Equal("L1", output0.Value);
      Assert.Equal("L2", output1.Value);
      Assert.Equal("L1", output2.Value);
      Assert.Equal("L2", output3.Value);
    }

    [Fact]
    public void AnalysisCaseIdTest() {
      IGH_Param param = TestHelper(MethodBase.GetCurrentMethod()
       .Name.Replace("Test", string.Empty));
      var output0 = (GH_Integer)param.VolatileData.get_Branch(0)[0];
      var output1 = (GH_Integer)param.VolatileData.get_Branch(0)[1];
      var output2 = (GH_Integer)param.VolatileData.get_Branch(0)[2];
      var output3 = (GH_Integer)param.VolatileData.get_Branch(0)[3];

      Assert.Equal(1, output0.Value);
      Assert.Equal(2, output1.Value);
      Assert.Equal(1, output2.Value);
      Assert.Equal(2, output3.Value);
    }

    [Fact]
    public void CombinationCaseIdTest() {
      IGH_Param param = TestHelper(MethodBase.GetCurrentMethod()
       .Name.Replace("Test", string.Empty));
      var output0 = (GH_Integer)param.VolatileData.get_Branch(0)[0];

      Assert.Equal(1, output0.Value);
    }

    [Fact]
    public void CombinationCaseNameTest() {
      IGH_Param param = TestHelper(MethodBase.GetCurrentMethod()
       .Name.Replace("Test", string.Empty));
      var output0 = (GH_String)param.VolatileData.get_Branch(0)[0];

      Assert.Equal("ULS", output0.Value);
    }

    [Fact]
    public void CombinationCaseDescriptionTest() {
      IGH_Param param = TestHelper(MethodBase.GetCurrentMethod()
       .Name.Replace("Test", string.Empty));
      var output0 = (GH_String)param.VolatileData.get_Branch(0)[0];

      Assert.Equal("1.4A1 + 1.6A2", output0.Value);
    }

    private static GH_Document OpenDocument() {
      string fileName = MethodBase.GetCurrentMethod().DeclaringType + ".gh";
      fileName = fileName.Replace("IntegrationTests.Parameters.", string.Empty);
      fileName = fileName.Replace("Test", string.Empty);

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent
       .Parent.FullName;
      string path = Path.Combine(new string[] {
        solutiondir,
        "ExampleFiles",
        "Parameters",
        "4_Analysis",
      });

      return Helper.CreateDocument(Path.Combine(path, fileName));
    }

    private IGH_Param TestHelper(string groupIdentifier) {
      IGH_Param param = Helper.FindParameter(Document, groupIdentifier);
      return param;
    }
  }
}
