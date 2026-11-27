using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaGH.Helpers;

using Xunit;

namespace IntegrationTests.ExampleFiles {
  public sealed class GerberMemberDocumentFixture : IDisposable {
    public GH_Document Document { get; }

    public GerberMemberDocumentFixture(GrasshopperFixture grasshopper) {
      Document = Example1_GerberMember_Test.Document();
    }

    public void Dispose() {
      Document.Dispose();
    }
  }

  [Collection("GrasshopperFixture collection")]
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  public class Example1_GerberMember_Test : IClassFixture<GerberMemberDocumentFixture> {
    private readonly GH_Document _document;

    public Example1_GerberMember_Test(GerberMemberDocumentFixture fixture) {
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
    public void MaxMyyAssert() {
      IGH_Param param = Helper.FindParameter(_document, "MaxMyy");
      var output = (GH_Number)param.VolatileData.get_Branch(0)[0];
      Assert.Equal(60.028894, output.Value, DoubleComparer.Default);
    }

    [Fact]
    public void NoRuntimeErrorsTest() {
      Helper.TestNoRuntimeMessagesInDocument(_document, GH_RuntimeMessageLevel.Error);
    }
  }
}
