using System;
using System.IO;
using System.Reflection;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using Xunit;

namespace IntegrationTests.Components {
  [Collection("GrasshopperFixture collection")]
  public class GetResultsAndResultsInfoTest {
    private static GH_Document Document => document ?? (document = OpenDocument());
    private static GH_Document document = null;

    [Fact]
    public void Test() {
      GH_Document doc = Document;
      IGH_Param types = Helper.FindParameter(doc, "Types");
      var types1 = (GH_String)types.VolatileData.get_Branch(0)[0];
      var types2 = (GH_String)types.VolatileData.get_Branch(0)[1];
      var types3 = (GH_String)types.VolatileData.get_Branch(0)[2];
      Assert.Equal("Analysis", types1.Value);
      Assert.Equal("Analysis", types2.Value);
      Assert.Equal("Combination", types3.Value);

      IGH_Param caseIDs = Helper.FindParameter(doc, "CaseIDs");
      var case1 = (GH_Integer)caseIDs.VolatileData.get_Branch(0)[0];
      var case2 = (GH_Integer)caseIDs.VolatileData.get_Branch(0)[1];
      var case3 = (GH_Integer)caseIDs.VolatileData.get_Branch(0)[2];
      Assert.Equal(1, case1.Value);
      Assert.Equal(2, case2.Value);
      Assert.Equal(1, case3.Value);

      IGH_Param permutations = Helper.FindParameter(doc, "Permutations");
      var perm3 = (GH_Integer)permutations.VolatileData.get_Branch(new GH_Path(1))[0];
      Assert.Equal(1, perm3.Value);

      IGH_Param myy1 = Helper.FindParameter(doc, "Myy1");
      var myy11 = (GH_Number)myy1.VolatileData.get_Branch(0)[0];
      var myy12 = (GH_Number)myy1.VolatileData.get_Branch(0)[1];
      var myy13 = (GH_Number)myy1.VolatileData.get_Branch(0)[2];
      var myy14 = (GH_Number)myy1.VolatileData.get_Branch(0)[3];
      var myy15 = (GH_Number)myy1.VolatileData.get_Branch(0)[4];
      Assert.Equal(-0.001112, myy11.Value, 6);
      Assert.Equal(-118828.126112, myy12.Value, 6);
      Assert.Equal(-188437.501112, myy13.Value, 6);
      Assert.Equal(-118828.126112, myy14.Value, 6);
      Assert.Equal(-0.001112, myy15.Value, 6);

      IGH_Param myy2 = Helper.FindParameter(doc, "Myy2");
      var myy21 = (GH_Number)myy1.VolatileData.get_Branch(0)[0];
      var myy22 = (GH_Number)myy1.VolatileData.get_Branch(0)[1];
      var myy23 = (GH_Number)myy1.VolatileData.get_Branch(0)[2];
      var myy24 = (GH_Number)myy1.VolatileData.get_Branch(0)[3];
      var myy25 = (GH_Number)myy1.VolatileData.get_Branch(0)[4];
      Assert.Equal(-0.001112, myy21.Value, 6);
      Assert.Equal(-118828.126112, myy22.Value, 6);
      Assert.Equal(-188437.501112, myy23.Value, 6);
      Assert.Equal(-118828.126112, myy24.Value, 6);
      Assert.Equal(-0.001112, myy25.Value, 6);
    }

    private static GH_Document OpenDocument() {
      Type thisClass = MethodBase.GetCurrentMethod().DeclaringType;
      string fileName = thisClass.Name + ".gh";
      fileName = fileName.Replace(thisClass.Namespace, string.Empty).Replace("Test", string.Empty);

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent
       .Parent.FullName;
      string path = Path.Combine(new string[] {
        solutiondir,
        "ExampleFiles",
        "Components",
      });

      return Helper.CreateDocument(Path.Combine(path, fileName));
    }
  }
}
