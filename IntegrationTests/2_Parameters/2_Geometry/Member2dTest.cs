using System.IO;
using System.Reflection;

using Grasshopper.Kernel;

using GsaGH.Parameters;

using GsaGHTests.Helpers;

using GsaGH.Helpers;
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
      GsaMember2d mem = goo.Value;
      Assert.Equal(2, mem.Brep.Loops.Count);
      Assert.Equal(2, mem.InclusionPoints.Count);
      Assert.Equal(2, mem.InclusionLines.Count);
      Assert.Equal(-1, mem.InclusionPoints[0].X, DoubleComparer.Default);
      Assert.Equal(2, mem.InclusionPoints[0].Y, DoubleComparer.Default);
      Assert.Equal(0, mem.InclusionPoints[0].Z, DoubleComparer.Default);
      Assert.Equal(-1, mem.InclusionPoints[1].X, DoubleComparer.Default);
      Assert.Equal(4, mem.InclusionPoints[1].Y, DoubleComparer.Default);
      Assert.Equal(0, mem.InclusionPoints[1].Z, DoubleComparer.Default);
      Assert.Equal("A", mem.InclusionLinesTopologyType[0][1]);
      Assert.Equal(3, mem.InclusionLines[1].PointAtStart.X, DoubleComparer.Default);
      Assert.Equal(3, mem.InclusionLines[1].PointAtStart.Y, DoubleComparer.Default);
      Assert.Equal(0, mem.InclusionLines[1].PointAtStart.Z, DoubleComparer.Default);
      Assert.Equal(3, mem.InclusionLines[1].PointAtEnd.X, DoubleComparer.Default);
      Assert.Equal(1, mem.InclusionLines[1].PointAtEnd.Y, DoubleComparer.Default);
      Assert.Equal(0, mem.InclusionLines[1].PointAtEnd.Z, DoubleComparer.Default);
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
