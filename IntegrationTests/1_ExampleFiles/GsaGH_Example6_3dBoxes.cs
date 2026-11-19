using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaGH.Helpers;

using Xunit;

namespace IntegrationTests.ExampleFiles {

  public sealed class BoxesDocumentFixture : IDisposable {
    public GH_Document Document { get; }

    public BoxesDocumentFixture(GrasshopperFixture grasshopper) {
      // The existing collection fixture is initialized first.
      Document = Example6_3dBoxes_Test.Document();
    }

    public void Dispose() {
      Document.Dispose();
    }
  }

  [Collection("GrasshopperFixture collection")]
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  public class Example6_3dBoxes_Test: IClassFixture<BoxesDocumentFixture>{

    private readonly GH_Document _document;

    public Example6_3dBoxes_Test(BoxesDocumentFixture fixture) {
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
    public void MaxDeflectionTest() {
      IGH_Param param1 = Helper.FindParameter(_document, "TestMaxValue");
      var output1 = (GH_Number)param1.VolatileData.get_Branch(0)[0];

      IGH_Param param2 = Helper.FindParameter(_document, "TestDeflectedShape");
      var output2 = (GH_Number)param2.VolatileData.get_Branch(0)[0];

      Assert.Equal(output1.Value, output2.Value, DoubleComparer.Default);
      Assert.NotEqual(0.0, output1.Value, DoubleComparer.Default);
    }

    [Fact]
    public void Stress3dTest() {
      IGH_Param param = Helper.FindParameter(_document, "3dStress");
      var output1 = (GH_Number)param.VolatileData.get_Branch(0)[0];
      var output2 = (GH_Number)param.VolatileData.get_Branch(0)[1];
      var output3 = (GH_Number)param.VolatileData.get_Branch(0)[2];
      var output4 = (GH_Number)param.VolatileData.get_Branch(0)[3];
      var output5 = (GH_Number)param.VolatileData.get_Branch(0)[4];
      var output6 = (GH_Number)param.VolatileData.get_Branch(0)[5];
      Assert.Equal(3.03, output1.Value, DoubleComparer.Default);
      Assert.Equal(3.03, output2.Value, DoubleComparer.Default);
      Assert.Equal(11.46, output3.Value, DoubleComparer.Default);
      Assert.Equal(0, output4.Value, DoubleComparer.Default);
      Assert.Equal(1.29, output5.Value, DoubleComparer.Default);
      Assert.Equal(4.01, output6.Value, DoubleComparer.Default);
    }

    [Fact]
    public void CombinationStress3dTest() {
      IGH_Param param = Helper.FindParameter(_document, "Comb3dStress");
      var output1 = (GH_Number)param.VolatileData.get_Branch(0)[0];
      var output2 = (GH_Number)param.VolatileData.get_Branch(0)[1];
      var output3 = (GH_Number)param.VolatileData.get_Branch(0)[2];
      var output4 = (GH_Number)param.VolatileData.get_Branch(0)[3];
      var output5 = (GH_Number)param.VolatileData.get_Branch(0)[4];
      var output6 = (GH_Number)param.VolatileData.get_Branch(0)[5];
      Assert.Equal(6.069, output1.Value, DoubleComparer.Default);
      Assert.Equal(6.069, output2.Value, DoubleComparer.Default);
      Assert.Equal(22.93, output3.Value, DoubleComparer.Default);
      Assert.Equal(0, output4.Value, DoubleComparer.Default);
      Assert.Equal(2.580, output5.Value, DoubleComparer.Default);
      Assert.Equal(8.023, output6.Value, DoubleComparer.Default);
    }

    [Fact]
    public void CombinationDisplament3dTest() {
      IGH_Param param = Helper.FindParameter(_document, "Comb3dDisplacement");
      var output1 = (GH_Number)param.VolatileData.get_Branch(0)[0];
      var output2 = (GH_Number)param.VolatileData.get_Branch(0)[1];
      var output3 = (GH_Number)param.VolatileData.get_Branch(0)[2];
      Assert.Equal(0.5203, output1.Value, DoubleComparer.Default);
      Assert.Equal(0.3206, output2.Value, DoubleComparer.Default);
      Assert.Equal(0.5856, output3.Value, DoubleComparer.Default);
    }

    [Fact]
    public void NoRuntimeErrorsTest() {
      Helper.TestNoRuntimeMessagesInDocument(_document, GH_RuntimeMessageLevel.Error);
    }
  }
}
