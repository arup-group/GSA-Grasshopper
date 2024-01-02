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
6.834174,
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
      20.198621,
18.448831,
21.644601,
21.062508,
17.178511,
18.679186,
19.025189,
19.294091
      })]
    [InlineData("MaxElem1ds", new double[] {
      14.064917,
21.357738,
21.532153,
19.784934,
8.374727,
      })]
    [InlineData("MaxElem2ds", new double[] {
      21.062508,
21.644601,
19.294091,
19.025189,
20.256596,
19.025189,
19.294091,
16.983603,
16.801549,
18.026108,
      })]
    [InlineData("MaxElem3ds", new double[] {
      17.082706,
13.469645,
13.647967,
17.262514,
16.801549,
13.110067,
13.294991,
16.983603,
15.126534
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
