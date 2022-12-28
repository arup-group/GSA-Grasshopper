using System;
using System.IO;
using System.Reflection;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Xunit;

namespace IntegrationTests.Components
{
  [Collection("GrasshopperFixture collection")]
  public class GlobalPerformance_ModalTests
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
    public void EffectiveMassesZTest()
    {
      GH_Document doc = Document();
      IGH_Param param = Helper.FindParameter(doc, "EffectiveMassesZ");
      GH_Number output1 = (GH_Number)param.VolatileData.get_Branch(0)[0];
      Assert.Equal(77.91, output1.Value, 2);
    }

    [Fact]
    public void ModesTest()
    {
      GH_Document doc = Document();
      IGH_Param param = Helper.FindParameter(doc, "Modes");
      GH_Integer output1 = (GH_Integer)param.VolatileData.get_Branch(0)[0];
      GH_Integer output2 = (GH_Integer)param.VolatileData.get_Branch(0)[1];
      GH_Integer output3 = (GH_Integer)param.VolatileData.get_Branch(0)[2];
      GH_Integer output4 = (GH_Integer)param.VolatileData.get_Branch(0)[3];
      Assert.Equal(1, output1.Value);
      Assert.Equal(2, output2.Value);
      Assert.Equal(4, output3.Value);
      Assert.Equal(5, output4.Value);
    }

    [Fact]
    public void ModalMassesTest()
    {
      GH_Document doc = Document();
      IGH_Param param = Helper.FindParameter(doc, "ModalMasses");
      GH_Number output1 = (GH_Number)param.VolatileData.get_Branch(0)[0];
      GH_Number output2 = (GH_Number)param.VolatileData.get_Branch(0)[1];
      GH_Number output3 = (GH_Number)param.VolatileData.get_Branch(0)[2];
      GH_Number output4 = (GH_Number)param.VolatileData.get_Branch(0)[3];
      Assert.Equal(45.62, output1.Value, 2);
      Assert.Equal(19.75, output2.Value, 2);
      Assert.Equal(16.14, output3.Value, 2);
      Assert.Equal(34.85, output4.Value, 2);
    }

    [Fact]
    public void ModalStiffnessesTest()
    {
      GH_Document doc = Document();
      IGH_Param param = Helper.FindParameter(doc, "ModalStiffnesses");
      GH_Number output1 = (GH_Number)param.VolatileData.get_Branch(0)[0];
      GH_Number output2 = (GH_Number)param.VolatileData.get_Branch(0)[1];
      GH_Number output3 = (GH_Number)param.VolatileData.get_Branch(0)[2];
      GH_Number output4 = (GH_Number)param.VolatileData.get_Branch(0)[3];
      Assert.Equal(27740 / 10, output1.Value / 10, 0);
      Assert.Equal(61630 / 10, output2.Value / 10, 0);
      Assert.Equal(83060 / 10, output3.Value / 10, 0);
      Assert.Equal(621900 / 100, output4.Value / 100, 0);
    }

    [Fact]
    public void FrequenciesTest()
    {
      GH_Document doc = Document();
      IGH_Param param = Helper.FindParameter(doc, "Frequencies");
      GH_Number output1 = (GH_Number)param.VolatileData.get_Branch(0)[0];
      GH_Number output2 = (GH_Number)param.VolatileData.get_Branch(0)[1];
      GH_Number output3 = (GH_Number)param.VolatileData.get_Branch(0)[2];
      GH_Number output4 = (GH_Number)param.VolatileData.get_Branch(0)[3];
      Assert.Equal(3.925, output1.Value, 3);
      Assert.Equal(8.891, output2.Value, 3);
      Assert.Equal(11.42, output3.Value, 2);
      Assert.Equal(21.26, output4.Value, 2);
    }
  }
}
