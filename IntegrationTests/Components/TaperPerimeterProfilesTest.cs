using System;
using System.IO;
using System.Reflection;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Xunit;

namespace IntegrationTests.Components
{
  [Collection("GrasshopperFixture collection")]
  public class TaperPerimeterProfilesTest
  {
    public static GH_Document Document()
    {
      Type thisClass = MethodBase.GetCurrentMethod().DeclaringType;
      string fileName = thisClass.Name + ".gh";
      fileName = fileName.Replace(thisClass.Namespace, string.Empty).Replace("Test", string.Empty);

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent.FullName;
      string path = Path.Combine(new string[] { solutiondir, "ExampleFiles", "Components"});
      GH_DocumentIO io = new GH_DocumentIO();
      Assert.True(File.Exists(Path.Combine(path, fileName)));
      Assert.True(io.Open(Path.Combine(path, fileName)));
      io.Document.NewSolution(true);
      return io.Document;
    }

    [Fact]
    public void PerimeterProfilesTaperTest()
    {
      GH_Document doc = Document();
      GH_Param<GH_String> param = Helper.FindComponentInDocumentByGroup<GH_String>(doc, "Test1");
      Assert.NotNull(param);
      param.CollectData();
      GH_String output = (GH_String)param.VolatileData.get_Branch(0)[0];

      string expected = "GEO P(cm) M(7.3470091169301|8.77260630300588) L(10.3722481650778|-6.13139150210088) L(-6.2665665997345|-13.0174158044603) L(-12.3170446960299|-6.03706240206856) L(-6.48265510317362|7.64065710261802) L(7.3470091169301|8.77260630300588) : M(4.70387778827842|6.43324462220431) L(6.64076864227541|-4.49635376820731) L(-4.01213105470806|-9.54610492327091) L(-7.88591276270205|-4.42717909485028) L(-4.15048040142213|5.60314854191988) L(4.70387778827842|6.43324462220431)";
      Assert.Equal(expected, output.Value);
    }

    [Fact]
    public void IncorrectProfilesTest()
    {
      GH_Document doc = Document();
      GH_Component comp = Helper.FindComponentInDocumentByGroup(doc, "Test2");
      Assert.NotNull(comp);
      comp.Params.Output[0].CollectData();
      var output = comp.Params.Output[0].VolatileData.get_Branch(0)[0];
      Assert.Null(output);

      Assert.Equal(GH_RuntimeMessageLevel.Error, comp.RuntimeMessageLevel);

      string expected = "Start and End Profile must contain similar number of points";
      Assert.Equal(expected, comp.RuntimeMessages(GH_RuntimeMessageLevel.Error)[0]);
    }
  }
}
