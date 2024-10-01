using System;
using System.IO;
using System.Reflection;

using Grasshopper.Kernel;

using Xunit;

namespace IntegrationTests.Components {
  [Collection("GrasshopperFixture collection")]
  public class ContourEnvelopesTests {
    private static GH_Document Document => document ?? (document = OpenDocument());
    private static GH_Document document = null;

    [Fact]
    public void NoRuntimeErrorTest() {
      Helper.TestNoRuntimeMessagesInDocument(Document, GH_RuntimeMessageLevel.Error);
      Helper.TestNoRuntimeMessagesInDocument(Document, GH_RuntimeMessageLevel.Warning);
    }

    [Theory]
    [InlineData("NodeResults", 0)]
    [InlineData("1dResults", 0)]
    [InlineData("2dResults", 0)]
    [InlineData("3dResults", 0)]
    [InlineData("MinNodes", new double[] {
      6.54047,
      6.000259,
      7.053597,
      6.872579,
      6.222865,
      6.700295,
      6.749231,
      6.834174
    })]
    [InlineData("MinElem1ds", new double[] {
      4.453529,
      6.941855,
      7.020917,
      6.441967,
      2.670275
    })]
    [InlineData("MinElem2ds", new double[] {
      6.872579,
      7.053597,
      6.834174,
      6.749231,
      6.877394,
      6.749231,
      6.834174,
      6.517081,
      6.450017,
      6.637626
    })]
    [InlineData("MinElem3ds", new double[] {
      6.501138,
      5.538879,
      5.606204,
      6.568577,
      6.450017,
      5.477798,
      5.546827,
      6.517081,
      6.011854
    })]
    [InlineData("MaxNodes", new double[] {
      6.54047,
      6.000259,
      7.053597,
      6.872579,
      6.222865,
      6.700295,
      6.749231,
      6.834174
    })]
    [InlineData("MaxElem1ds", new double[] {
      4.453529,
      6.941855,
      7.020917,
      6.441967,
      2.670275
    })]
    [InlineData("MaxElem2ds", new double[] {
      6.872579,
      7.053597,
      6.834174,
      6.749231,
      6.877394,
      6.749231,
      6.834174,
      6.517081,
      6.450017,
      6.637626
    })]
    [InlineData("MaxElem3ds", new double[] {
      6.501138,
      5.538879,
      5.606204,
      6.568577,
      6.450017,
      5.477798,
      5.546827,
      6.517081,
      6.011854,
    })]
    [InlineData("Min2Nodes", new double[] {
      7.848564,
      7.20031,
      8.464316,
      8.247094,
      7.467438,
      8.040354,
      8.099077,
      8.201008
    })]
    [InlineData("Min2Elem1ds", new double[] {
      5.344235,
      8.330226,
      8.425101,
      7.730361,
      3.20433
    })]
    [InlineData("Min2Elem2ds", new double[] {
      8.247094,
      8.464316,
      8.201008,
      8.099077,
      8.252873,
      8.099077,
      8.201008,
      7.820497,
      7.74002,
      7.965151
    })]
    [InlineData("Min2Elem3ds", new double[] {
      7.801365,
      6.646655,
      6.727445,
      7.882292,
      7.74002,
      6.573358,
      6.656192,
      7.820497,
      7.214224
    })]
    public void Test(string groupIdentifier, object expected) {
      IGH_Param param = Helper.FindParameter(Document, groupIdentifier);
      Helper.TestGhPrimitives(param, expected);
    }

    private static GH_Document OpenDocument() {
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
  }
}
