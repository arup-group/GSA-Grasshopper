using System;
using System.IO;
using System.Reflection;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Xunit;

namespace IntegrationTests.Components
{
  [Collection("GrasshopperFixture collection")]
  public class GlobalPerformance_BucklingTests
  {
    public static GH_Document Document()
    {
      Type thisClass = MethodBase.GetCurrentMethod().DeclaringType;
      string fileName = thisClass.Name + ".gh";
      fileName = fileName.Replace(thisClass.Namespace, string.Empty).Replace("Tests", string.Empty);

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent.FullName;
      string path = Path.Combine(new string[] { solutiondir, "ExampleFiles", "Components"});
      
      return Helper.CreateDocument(Path.Combine(path, fileName));
    }

    [Fact]
    public void ModesTest()
    {
      GH_Document doc = Document();
      IGH_Param param = Helper.FindParameter(doc, "Modes");
      GH_Integer output1 = (GH_Integer)param.VolatileData.get_Branch(0)[0];
      GH_Integer output2 = (GH_Integer)param.VolatileData.get_Branch(0)[1];
      GH_Integer output3 = (GH_Integer)param.VolatileData.get_Branch(0)[2];
      Assert.Equal(1, output1.Value);
      Assert.Equal(2, output2.Value);
      Assert.Equal(3, output3.Value);
    }

    [Fact]
    public void LoadFactorTest()
    {
      GH_Document doc = Document();
      IGH_Param param = Helper.FindParameter(doc, "LoadFactors");
      GH_Number output1 = (GH_Number)param.VolatileData.get_Branch(0)[0];
      GH_Number output2 = (GH_Number)param.VolatileData.get_Branch(0)[1];
      GH_Number output3 = (GH_Number)param.VolatileData.get_Branch(0)[2];
      Assert.Equal(-0.4183, output1.Value, 4);
      Assert.Equal(-0.5784, output2.Value, 4);
      Assert.Equal(-0.8993, output3.Value, 4);
    }
  }
}
