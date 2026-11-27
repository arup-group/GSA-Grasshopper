using System;
using System.IO;
using System.Reflection;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaGH.Helpers;

using Xunit;

namespace IntegrationTests.Components {
  public sealed class NodeReactionForceResultDocumentFixture : IDisposable {
    public GH_Document Document { get; }

    public NodeReactionForceResultDocumentFixture(GrasshopperFixture grasshopper) {
      Document = NodeReactionForceResultTests.Document();
    }

    public void Dispose() {
      Document.Dispose();
    }
  }

  [Collection("GrasshopperFixture collection")]
  public class NodeReactionForceResultTests : IClassFixture<NodeReactionForceResultDocumentFixture> {
    private readonly GH_Document _document;

    public NodeReactionForceResultTests(NodeReactionForceResultDocumentFixture fixture) {
      _document = fixture.Document;
    }

    public static GH_Document Document() {
      Type thisClass = MethodBase.GetCurrentMethod().DeclaringType;
      string fileName = thisClass.Name + ".gh";
      fileName = fileName.Replace(thisClass.Namespace, string.Empty).Replace("Tests", string.Empty);

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent
       .Parent.FullName;
      string path = Path.Combine(new string[] {
        solutiondir,
        "ExampleFiles",
        "Components",
      });

      return Helper.CreateDocument(Path.Combine(path, fileName));
    }

    [Fact]
    public void FAssert() {
      IGH_Param param = Helper.FindParameter(_document, "F");
      var output1 = (GH_Number)param.VolatileData.get_Branch(0)[0];
      var output2 = (GH_Number)param.VolatileData.get_Branch(0)[1];
      var output3 = (GH_Number)param.VolatileData.get_Branch(0)[2];
      var output4 = (GH_Number)param.VolatileData.get_Branch(0)[3];
      Assert.Equal(209.900, output1.Value, DoubleComparer.Default);
      Assert.Equal(205.700, output2.Value, DoubleComparer.Default);
      Assert.Equal(209.900, output3.Value, DoubleComparer.Default);
      Assert.Equal(205.700, output4.Value, DoubleComparer.Default);
    }

    [Fact]
    public void FxAssert() {
      IGH_Param param = Helper.FindParameter(_document, "Fx");
      var output1 = (GH_Number)param.VolatileData.get_Branch(0)[0];
      var output2 = (GH_Number)param.VolatileData.get_Branch(0)[1];
      var output3 = (GH_Number)param.VolatileData.get_Branch(0)[2];
      var output4 = (GH_Number)param.VolatileData.get_Branch(0)[3];
      Assert.Equal(-0.000785, output1.Value, 7);
      Assert.Equal(0.1297, output2.Value, 4);
      Assert.Equal(0.000785, output3.Value, 7);
      Assert.Equal(-0.1297, output4.Value, 4);
    }

    [Fact]
    public void FyAssert() {
      IGH_Param param = Helper.FindParameter(_document, "Fy");
      var output1 = (GH_Number)param.VolatileData.get_Branch(0)[0];
      var output2 = (GH_Number)param.VolatileData.get_Branch(0)[1];
      var output3 = (GH_Number)param.VolatileData.get_Branch(0)[2];
      var output4 = (GH_Number)param.VolatileData.get_Branch(0)[3];
      Assert.Equal(-205.200, output1.Value, DoubleComparer.Default);
      Assert.Equal(205.200, output2.Value, DoubleComparer.Default);
      Assert.Equal(-205.200, output3.Value, DoubleComparer.Default);
      Assert.Equal(205.200, output4.Value, DoubleComparer.Default);
    }

    [Fact]
    public void FzAssert() {
      IGH_Param param = Helper.FindParameter(_document, "Fz");
      var output1 = (GH_Number)param.VolatileData.get_Branch(0)[0];
      var output2 = (GH_Number)param.VolatileData.get_Branch(0)[1];
      var output3 = (GH_Number)param.VolatileData.get_Branch(0)[2];
      var output4 = (GH_Number)param.VolatileData.get_Branch(0)[3];
      Assert.Equal(44.160, output1.Value, DoubleComparer.Default);
      Assert.Equal(13.700, output2.Value, DoubleComparer.Default);
      Assert.Equal(44.160, output3.Value, DoubleComparer.Default);
      Assert.Equal(13.700, output4.Value, DoubleComparer.Default);
    }

    [Fact]
    public void MAssert() {
      IGH_Param param = Helper.FindParameter(_document, "M");
      var output1 = (GH_Number)param.VolatileData.get_Branch(0)[0];
      var output2 = (GH_Number)param.VolatileData.get_Branch(0)[1];
      var output3 = (GH_Number)param.VolatileData.get_Branch(0)[2];
      var output4 = (GH_Number)param.VolatileData.get_Branch(0)[3];
      Assert.Equal(51.740, output1.Value, DoubleComparer.Default);
      Assert.Equal(42.250, output2.Value, DoubleComparer.Default);
      Assert.Equal(51.740, output3.Value, DoubleComparer.Default);
      Assert.Equal(42.250, output4.Value, DoubleComparer.Default);
    }

    [Fact]
    public void MxxAssert() {
      IGH_Param param = Helper.FindParameter(_document, "Mxx");
      var output1 = (GH_Number)param.VolatileData.get_Branch(0)[0];
      var output2 = (GH_Number)param.VolatileData.get_Branch(0)[1];
      var output3 = (GH_Number)param.VolatileData.get_Branch(0)[2];
      var output4 = (GH_Number)param.VolatileData.get_Branch(0)[3];
      Assert.Equal(51.740, output1.Value, DoubleComparer.Default);
      Assert.Equal(42.250, output2.Value, DoubleComparer.Default);
      Assert.Equal(51.740, output3.Value, DoubleComparer.Default);
      Assert.Equal(42.250, output4.Value, DoubleComparer.Default);
    }

    [Fact]
    public void MyyAssert() {
      IGH_Param param = Helper.FindParameter(_document, "Myy");
      var output1 = (GH_Number)param.VolatileData.get_Branch(0)[0];
      var output2 = (GH_Number)param.VolatileData.get_Branch(0)[1];
      var output3 = (GH_Number)param.VolatileData.get_Branch(0)[2];
      var output4 = (GH_Number)param.VolatileData.get_Branch(0)[3];
      Assert.Equal(-0.1376, output1.Value, 4);
      Assert.Equal(-0.000013, output2.Value, 8);
      Assert.Equal(0.1376, output3.Value, 4);
      Assert.Equal(0.000013, output4.Value, 8);
    }

    [Fact]
    public void MzzAssert() {
      IGH_Param param = Helper.FindParameter(_document, "Mzz");
      var output1 = (GH_Number)param.VolatileData.get_Branch(0)[0];
      var output2 = (GH_Number)param.VolatileData.get_Branch(0)[1];
      var output3 = (GH_Number)param.VolatileData.get_Branch(0)[2];
      var output4 = (GH_Number)param.VolatileData.get_Branch(0)[3];
      Assert.Equal(0.00008, output1.Value, 8);
      Assert.Equal(-0.4818, output2.Value, 4);
      Assert.Equal(-0.00008, output3.Value, 8);
      Assert.Equal(0.4818, output4.Value, 4);
    }

    [Fact]
    public void NoRuntimeErrorsTest() {
      Helper.TestNoRuntimeMessagesInDocument(_document, GH_RuntimeMessageLevel.Error);
    }
  }
}
