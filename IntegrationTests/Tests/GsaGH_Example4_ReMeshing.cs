using System;
using System.IO;
using System.Reflection;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Xunit;

namespace IntegrationTests.ExampleFiles
{
  [Collection("GrasshopperFixture collection")]
  public class Example4_ReMeshing_Test
  {
    public static GH_Document Document()
    {
      Type thisClass = MethodBase.GetCurrentMethod().DeclaringType;
      string fileName = "GsaGH_" + thisClass.Name + ".gh";
      fileName = fileName.Replace(thisClass.Namespace, string.Empty).Replace("_Test", string.Empty);

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent.FullName;
      string path = Path.Combine(solutiondir, "ExampleFiles");

      return Helper.CreateDocument(Path.Combine(path, fileName));
    }

    [Fact]
    public void CountElementsTest()
    {
      IGH_Param param = Helper.FindParameter(Document(), "Elements");
      Assert.NotNull(param);
      param.CollectData();
      GH_Integer output = (GH_Integer)param.VolatileData.get_Branch(0)[0];
      Assert.Equal(48, output.Value);
    }

    [Fact]
    public void CountMeshTest()
    {
      IGH_Param param = Helper.FindParameter(Document(), "Vertices");
      Assert.NotNull(param);
      param.CollectData();
      GH_Integer output = (GH_Integer)param.VolatileData.get_Branch(0)[0];
      Assert.Equal(561, output.Value);

      IGH_Param param2 = Helper.FindParameter(Document(), "Faces");
      Assert.NotNull(param2);
      param2.CollectData();
      GH_Integer output2 = (GH_Integer)param2.VolatileData.get_Branch(0)[0];
      Assert.Equal(150, output2.Value);
    }

    [Fact]
    public void NoRuntimeErrorsTest()
    {
      Helper.TestNoRuntimeMessagesInDocument(Document(), GH_RuntimeMessageLevel.Error);
    }
  }
}
