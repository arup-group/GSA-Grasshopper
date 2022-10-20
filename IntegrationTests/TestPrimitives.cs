using System;
using System.IO;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Xunit;

namespace IntegrationTests
{
  [Collection("GrasshopperFixture collection")]
  public class TestPrimitives
  {
    public static GH_Document Document()
    {
      string fileName = "TestPrimitives.gh";

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent.FullName;
      string path = Path.Combine(new string[] { solutiondir, "ExampleFiles" });
      GH_DocumentIO io = new GH_DocumentIO();

      Assert.True(File.Exists(Path.Combine(path, fileName)));
      Assert.True(io.Open(Path.Combine(path, fileName)));
      io.Document.NewSolution(true);
      return io.Document;
    }

    [Fact]
    public void TestCircle()
    {
      GH_Document doc = Document();
      foreach (var obj in (doc.Objects))
        if (obj is Grasshopper.Kernel.IGH_Param param)
          if (param.NickName == "TestCircleOutput")
          {
            param.CollectData();
            param.ComputeData();

            Assert.Equal(1, param.VolatileData.DataCount);
            var data = param.VolatileData.AllData(true).GetEnumerator();
            data.Reset();
            data.MoveNext();
            var theCircle = data.Current;
            Assert.True(theCircle.CastTo(out Circle circle));
            Assert.Equal(1.0, circle.Radius);
            Assert.Equal(Math.PI * 2.0, circle.Circumference);
            return;
          }
      Assert.True(false, "Did not find oputput");
    }

    [Fact]
    public void TestLine()
    {
      GH_Document doc = Document();
      foreach (var obj in (doc.Objects))
        if (obj is Grasshopper.Kernel.IGH_Param param)
          if (param.NickName == "TestLineOutput")
          {
            param.CollectData();
            param.ComputeData();

            Assert.Equal(1, param.VolatileData.DataCount);
            var data = param.VolatileData.AllData(true).GetEnumerator();
            data.Reset();
            data.MoveNext();
            var theLine = data.Current;
            Assert.True(theLine.CastTo(out Line line));
            Assert.Equal(Math.Sqrt(1.0 * 1.0 + -5.0 * -5.0 + 3.0 * 3.0), line.Length);
            return;
          }
      Assert.True(false, "Did not find oputput");
    }
  }
}
