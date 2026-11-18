using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaGH.Helpers;

using Xunit;

namespace IntegrationTests.ExampleFiles {
  public sealed class TrussDocumentFixture : IDisposable {
    public GH_Document Document { get; }

    public TrussDocumentFixture(GrasshopperFixture grasshopper) {
      Document = Example3_Truss_Test.Document();
    }

    public void Dispose() {
      Document.Dispose();
    }
  }

  [Collection("GrasshopperFixture collection")]
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  public class Example3_Truss_Test : IClassFixture<TrussDocumentFixture> {
    private readonly GH_Document _document;

    public Example3_Truss_Test(TrussDocumentFixture fixture) {
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
    public void MaxDeformationAssert() {
      IGH_Param param = Helper.FindParameter(_document, "MaxU");
      var output = (GH_Number)param.VolatileData.get_Branch(0)[0];
      Assert.Equal(6.914599, output.Value, DoubleComparer.Default);
    }

    [Fact]
    public void NoRuntimeErrorsTest() {
      Helper.TestNoRuntimeMessagesInDocument(_document, GH_RuntimeMessageLevel.Error);
    }
  }
}
