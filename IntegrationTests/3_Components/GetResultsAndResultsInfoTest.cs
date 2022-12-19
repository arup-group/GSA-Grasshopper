using System;
using System.IO;
using System.Reflection;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Xunit;

namespace IntegrationTests.Components
{
  [Collection("GrasshopperFixture collection")]
  public class GetResultsAndResultsInfoTest
  {
    public static GH_Document Document()
    {
      Type thisClass = MethodBase.GetCurrentMethod().DeclaringType;
      string fileName = thisClass.Name + ".gh";
      fileName = fileName.Replace(thisClass.Namespace, string.Empty).Replace("Test", string.Empty);

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent.FullName;
      string path = Path.Combine(new string[] { solutiondir, "ExampleFiles", "Components"});
      
      return Helper.CreateDocument(Path.Combine(path, fileName));
    }

    [Fact]
    public void Test()
    {
      GH_Document doc = Document();
      IGH_Param types = Helper.FindParameter(doc, "Types");
      GH_String types1 = (GH_String)types.VolatileData.get_Branch(0)[0];
      GH_String types2 = (GH_String)types.VolatileData.get_Branch(0)[1];
      GH_String types3 = (GH_String)types.VolatileData.get_Branch(0)[2];
      Assert.Equal("Analysis", types1.Value);
      Assert.Equal("Analysis", types2.Value);
      Assert.Equal("Combination", types3.Value);

      IGH_Param caseIDs = Helper.FindParameter(doc, "CaseIDs");
      GH_Integer case1 = (GH_Integer)caseIDs.VolatileData.get_Branch(0)[0];
      GH_Integer case2 = (GH_Integer)caseIDs.VolatileData.get_Branch(0)[1];
      GH_Integer case3 = (GH_Integer)caseIDs.VolatileData.get_Branch(0)[2];
      Assert.Equal(1, case1.Value);
      Assert.Equal(2, case2.Value);
      Assert.Equal(1, case3.Value);

      IGH_Param permutations = Helper.FindParameter(doc, "Permutations");
      GH_Integer perm3 = (GH_Integer)permutations.VolatileData.get_Branch(new GH_Path(1))[0];
      Assert.Equal(1, perm3.Value);

      IGH_Param myy = Helper.FindParameter(doc, "Myy");
      GH_Number myy1 = (GH_Number)myy.VolatileData.get_Branch(0)[0];
      GH_Number myy2 = (GH_Number)myy.VolatileData.get_Branch(0)[1];
      GH_Number myy3 = (GH_Number)myy.VolatileData.get_Branch(0)[2];
      GH_Number myy4 = (GH_Number)myy.VolatileData.get_Branch(0)[3];
      GH_Number myy5 = (GH_Number)myy.VolatileData.get_Branch(0)[4];
      Assert.Equal(-0.001112, myy1.Value, 6);
      Assert.Equal(-118828.126112, myy2.Value, 6);
      Assert.Equal(-188437.501112, myy3.Value, 6);
      Assert.Equal(-118828.126112, myy4.Value, 6);
      Assert.Equal(-0.001112, myy5.Value, 6);
    }
  }
}
