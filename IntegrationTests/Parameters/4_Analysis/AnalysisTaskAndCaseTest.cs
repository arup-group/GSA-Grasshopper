using System;
using System.IO;
using System.Reflection;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Parameters;
using Rhino.UI;
using Xunit;

namespace IntegrationTests.Parameters
{
  [Collection("GrasshopperFixture collection")]
  public class GetCreateAnalysisTaskAndCaseTest
  {
    public static GH_Document Document()
    {
      string fileName = MethodBase.GetCurrentMethod().DeclaringType + ".gh";
      fileName = fileName.Replace("IntegrationTests.Parameters.", string.Empty);
      fileName = fileName.Replace("Test", string.Empty);

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent.FullName;
      string path = Path.Combine(new string[] { solutiondir, "ExampleFiles", "Parameters", "4_Analysis" });
      
      return Helper.CreateDocument(Path.Combine(path, fileName));
    }

    [Fact]
    public void OriginalTaskTest()
    {
      IGH_Param param = TestHelper(MethodBase.GetCurrentMethod().Name.Replace("Test", string.Empty));
      GsaAnalysisTaskGoo output = (GsaAnalysisTaskGoo)param.VolatileData.get_Branch(0)[0];
      GsaAnalysisTask gsaghobject = output.Value;
      
      Assert.Equal("Task 1", gsaghobject.Name);
      Assert.Equal(1, gsaghobject.ID);
      Assert.Equal(2, gsaghobject.Cases.Count);
      Assert.Equal("DL", gsaghobject.Cases[0].Name);
      Assert.Equal("LL", gsaghobject.Cases[1].Name);
      Assert.Equal("L1", gsaghobject.Cases[0].Description);
      Assert.Equal("L2", gsaghobject.Cases[1].Description);
      Assert.Equal(GsaAnalysisTask.AnalysisType.Static, gsaghobject.Type);
    }

    [Fact]
    public void OriginalTaskNameTest()
    {
      IGH_Param param = TestHelper(MethodBase.GetCurrentMethod().Name.Replace("Test", string.Empty));
      GH_String output = (GH_String)param.VolatileData.get_Branch(0)[0];
      string gsaghobject = output.Value;

      Assert.Equal("Task 1", gsaghobject);
    }

    [Fact]
    public void OriginalCaseNamesTest()
    {
      IGH_Param param = TestHelper(MethodBase.GetCurrentMethod().Name.Replace("Test", string.Empty));
      GH_String output1 = (GH_String)param.VolatileData.get_Branch(0)[0];
      GH_String output2 = (GH_String)param.VolatileData.get_Branch(0)[1];

      Assert.Equal("DL", output1.Value);
      Assert.Equal("LL", output2.Value);
    }

    [Fact]
    public void OriginalCaseDescriptionsTest()
    {
      IGH_Param param = TestHelper(MethodBase.GetCurrentMethod().Name.Replace("Test", string.Empty));
      GH_String output1 = (GH_String)param.VolatileData.get_Branch(0)[0];
      GH_String output2 = (GH_String)param.VolatileData.get_Branch(0)[1];

      Assert.Equal("L1", output1.Value);
      Assert.Equal("L2", output2.Value);
    }

    [Fact]
    public void OriginalCaseIDsTest()
    {
      IGH_Param param = TestHelper(MethodBase.GetCurrentMethod().Name.Replace("Test", string.Empty));
      GH_Integer output1 = (GH_Integer)param.VolatileData.get_Branch(0)[0];
      GH_Integer output2 = (GH_Integer)param.VolatileData.get_Branch(0)[1];

      Assert.Equal(1, output1.Value);
      Assert.Equal(2, output2.Value);
    }

    [Fact]
    public void OriginalTaskTypeTest()
    {
      IGH_Param param = TestHelper(MethodBase.GetCurrentMethod().Name.Replace("Test", string.Empty));
      GH_String output = (GH_String)param.VolatileData.get_Branch(0)[0];
      string gsaghobject = output.Value;

      Assert.Equal("Static", gsaghobject);
    }

    [Fact]
    public void OriginalTaskIDTest()
    {
      IGH_Param param = TestHelper(MethodBase.GetCurrentMethod().Name.Replace("Test", string.Empty));
      GH_Integer output = (GH_Integer)param.VolatileData.get_Branch(0)[0];
      int gsaghobject = output.Value;

      Assert.Equal(1, gsaghobject);
    }

    [Fact]
    public void NewCaseNamesTest()
    {
      IGH_Param param = TestHelper(MethodBase.GetCurrentMethod().Name.Replace("Test", string.Empty));
      GH_String output1 = (GH_String)param.VolatileData.get_Branch(0)[0];
      GH_String output2 = (GH_String)param.VolatileData.get_Branch(0)[1];

      Assert.Equal("acase1", output1.Value);
      Assert.Equal("acase2", output2.Value);
    }

    [Fact]
    public void NewCaseDescriptionsTest()
    {
      IGH_Param param = TestHelper(MethodBase.GetCurrentMethod().Name.Replace("Test", string.Empty));
      GH_String output1 = (GH_String)param.VolatileData.get_Branch(0)[0];
      GH_String output2 = (GH_String)param.VolatileData.get_Branch(0)[1];

      Assert.Equal("1.5L2", output1.Value);
      Assert.Equal("0.9L1", output2.Value);
    }

    [Fact]
    public void NewCaseIDsTest()
    {
      IGH_Param param = TestHelper(MethodBase.GetCurrentMethod().Name.Replace("Test", string.Empty));
      GH_Integer output1 = (GH_Integer)param.VolatileData.get_Branch(0)[0];
      GH_Integer output2 = (GH_Integer)param.VolatileData.get_Branch(0)[1];

      Assert.Equal(0, output1.Value);
      Assert.Equal(0, output2.Value);
    }


    [Fact]
    public void NoRuntimeErrorTest()
    {
      Helper.TestNoRuntimeMessagesInDocument(Document(), GH_RuntimeMessageLevel.Error);
    }

    IGH_Param TestHelper(string groupIdentifier)
    {
      IGH_Param param = Helper.FindParameter(Document(), groupIdentifier);
      return param;
    }
  }
}
