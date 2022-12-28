using System;
using System.IO;
using System.Reflection;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Xunit;

namespace IntegrationTests.Components
{
  [Collection("GrasshopperFixture collection")]
  public class SectionAlignmentTest
  {
    public static GH_Document Document
    {
      get
      {
        if (_document == null)
          _document = OpenDocument();
        return _document;
      }
    }
    private static GH_Document _document = null;
    private static GH_Document OpenDocument()
    {
      Type thisClass = MethodBase.GetCurrentMethod().DeclaringType;
      string fileName = thisClass.Name + ".gh";
      fileName = fileName.Replace(thisClass.Namespace, string.Empty).Replace("Test", string.Empty);

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent.FullName;
      string path = Path.Combine(new string[] { solutiondir, "ExampleFiles", "Components"});
      
      return Helper.CreateDocument(Path.Combine(path, fileName));
    }

    [Fact]
    public void CalculatedOffsetsTest()
    {
      GH_Document doc = Document;
      IGH_Param param1 = Helper.FindParameter(doc, "Y");
      GH_Number output1 = (GH_Number)param1.VolatileData.get_Branch(0)[0];
      GH_Number output2 = (GH_Number)param1.VolatileData.get_Branch(0)[1];
      Assert.Equal(0, output1.Value, 6);
      Assert.Equal(-150, output2.Value, 6);
      IGH_Param param2 = Helper.FindParameter(doc, "Z");
      GH_Number output3 = (GH_Number)param2.VolatileData.get_Branch(0)[0];
      GH_Number output4 = (GH_Number)param2.VolatileData.get_Branch(0)[1];
      Assert.Equal(750, output3.Value, 6);
      Assert.Equal(350, output4.Value, 6);
    }
  }
}
