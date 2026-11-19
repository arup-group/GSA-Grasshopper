using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using Xunit;

namespace IntegrationTests.ExampleFiles {
  public sealed class ReMeshingDocumentFixture : IDisposable {
    public GH_Document Document { get; }

    public ReMeshingDocumentFixture(GrasshopperFixture grasshopper) {
      Document = Example4_ReMeshing_Test.Document();
    }

    public void Dispose() {
      Document.Dispose();
    }
  }

  [Collection("GrasshopperFixture collection")]
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  public class Example4_ReMeshing_Test : IClassFixture<ReMeshingDocumentFixture> {
    private readonly GH_Document _document;

    public Example4_ReMeshing_Test(ReMeshingDocumentFixture fixture) {
      _document = fixture.Document;
    }

    public static GH_Document Document() {
      Type thisClass = MethodBase.GetCurrentMethod().DeclaringType;
      string fileName = "GsaGH_" + thisClass.Name + ".gh";
      fileName = fileName.Replace(thisClass.Namespace, string.Empty).Replace("_Test", string.Empty);

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent
       .Parent.FullName;
      string path = Path.Combine(solutiondir, "ExampleFiles");

      return Helper.CreateDocument(Path.Combine(path, fileName));
    }

    [Fact]
    public void CountElementsTest() {
      IGH_Param param = Helper.FindParameter(_document, "Elements");
      var output = (GH_Integer)param.VolatileData.get_Branch(0)[0];
      Assert.Equal(48, output.Value);
    }

    [Fact]
    public void CountMeshTest() {
      IGH_Param param = Helper.FindParameter(_document, "Vertices");
      var output = (GH_Integer)param.VolatileData.get_Branch(0)[0];
      Assert.Equal(561, output.Value);

      IGH_Param param2 = Helper.FindParameter(_document, "Faces");
      var output2 = (GH_Integer)param2.VolatileData.get_Branch(0)[0];
      Assert.Equal(150, output2.Value);
    }

    [Fact]
    public void NoRuntimeErrorsTest() {
      Helper.TestNoRuntimeMessagesInDocument(_document, GH_RuntimeMessageLevel.Error);
    }
  }
}
