using System.IO;
using System.Reflection;

using Grasshopper.Kernel;

using GsaGH.Parameters;

using GsaGHTests.Helpers;

using Xunit;

namespace IntegrationTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class Member2dTest {
    public static GH_Document Document => document ?? (document = OpenDocument());
    private static GH_Document document = null;

    [Fact]
    public void NoRuntimeErrorTest() {
      Helper.TestNoRuntimeMessagesInDocument(Document, GH_RuntimeMessageLevel.Error);
    }

    [Fact]
    public void Member2dDuplicateTest() {
      IGH_Param param = Helper.FindParameter(Document, "Member2d");
      var goo = (GsaMember2dGoo)ComponentTestHelper.GetOutput(param);
      GsaMember2D mem = goo.Value;
      Assert.Equal(2, mem.Brep.Loops.Count);
      Assert.Equal(2, mem.InclusionPoints.Count);
      Assert.Equal(2, mem.InclusionLines.Count);
      Assert.Equal(-1, mem.InclusionPoints[0].X);
      Assert.Equal(2, mem.InclusionPoints[0].Y);
      Assert.Equal(0, mem.InclusionPoints[0].Z);
      Assert.Equal(-1, mem.InclusionPoints[1].X);
      Assert.Equal(4, mem.InclusionPoints[1].Y);
      Assert.Equal(0, mem.InclusionPoints[1].Z);
      Assert.Equal("A", mem.InclusionLinesTopologyType[0][1]);
      Assert.Equal(3, mem.InclusionLines[1].PointAtStart.X);
      Assert.Equal(3, mem.InclusionLines[1].PointAtStart.Y);
      Assert.Equal(0, mem.InclusionLines[1].PointAtStart.Z);
      Assert.Equal(3, mem.InclusionLines[1].PointAtEnd.X);
      Assert.Equal(1, mem.InclusionLines[1].PointAtEnd.Y);
      Assert.Equal(0, mem.InclusionLines[1].PointAtEnd.Z);
    }

    private static GH_Document OpenDocument() {
      string fileName = MethodBase.GetCurrentMethod().DeclaringType + ".gh";
      fileName = fileName.Replace("IntegrationTests.Parameters.", string.Empty);
      fileName = fileName.Replace("Test", string.Empty);

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent
       .Parent.FullName;
      string path = Path.Combine(new string[] {
        solutiondir,
        "ExampleFiles",
        "Parameters",
        "2_Geometry",
      });

      return Helper.CreateDocument(Path.Combine(path, fileName));
    }
  }
}
