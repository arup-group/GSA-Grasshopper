using System;
using System.IO;
using System.Reflection;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Xunit;

namespace IntegrationTests.Parameters
{
  [Collection("GrasshopperFixture collection")]
  public class CreateBool6Test
  {
    public static GH_Document Document()
    {
      string fileName = MethodBase.GetCurrentMethod().DeclaringType + ".gh";
      fileName = fileName.Replace("IntegrationTests.Parameters.", string.Empty);

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent.FullName;
      string path = Path.Combine(new string[] { solutiondir, "ExampleFiles", "Parameters", "1_Properties" });
      GH_DocumentIO io = new GH_DocumentIO();
      Assert.True(File.Exists(Path.Combine(path, fileName)));
      Assert.True(io.Open(Path.Combine(path, fileName)));
      io.Document.NewSolution(true);
      return io.Document;
    }

    [Fact]
    //[InlineData("X", true)]
    //[InlineData("Y", false)]
    //[InlineData("Z", true)]
    //[InlineData("XX", false)]
    //[InlineData("YY", true)]
    //[InlineData("ZZ", false)]
    //public void OutputTest(string groupIdentifier, bool expected)
    public void OutputTest()
    {
      GH_Document doc = Document();

      foreach (var obj in (doc.Objects))
        if (obj is Grasshopper.Kernel.IGH_Component comp)
        {
          Console.WriteLine("Component " + comp.NickName);
          foreach (string message in comp.RuntimeMessages(GH_RuntimeMessageLevel.Error))
            Console.WriteLine("Error: " + message);
          foreach (string message in comp.RuntimeMessages(GH_RuntimeMessageLevel.Warning))
            Console.WriteLine("Warning: " + message);
          foreach (string message in comp.RuntimeMessages(GH_RuntimeMessageLevel.Remark))
            Console.WriteLine("Remark: " + message);
        }
        else if (obj is Grasshopper.Kernel.IGH_Param p)
        {
          Console.WriteLine("Parameter " + p.NickName);
          foreach (string message in p.RuntimeMessages(GH_RuntimeMessageLevel.Error))
            Console.WriteLine("Error: " + message);
          foreach (string message in p.RuntimeMessages(GH_RuntimeMessageLevel.Warning))
            Console.WriteLine("Warning: " + message);
          foreach (string message in p.RuntimeMessages(GH_RuntimeMessageLevel.Remark))
            Console.WriteLine("Remark: " + message);
        }


      GH_Param<GH_Boolean> param = Helper.FindComponentInDocumentByGroup<GH_Boolean>(doc, "X");
      param.CollectData();
      param.ComputeData();

      Assert.Equal(1, param.VolatileData.DataCount);
      var data = param.VolatileData.AllData(true).GetEnumerator();
      data.Reset();
      data.MoveNext();
      GH_Boolean b = (GH_Boolean)data.Current;


      Assert.True(b.Value);
    }

    //[Fact]
    //public void NoRuntimeErrorTest()
    //{
    //  Helper.TestNoRuntimeMessagesInDocument(Document(), GH_RuntimeMessageLevel.Error);
    //}
  }
}
