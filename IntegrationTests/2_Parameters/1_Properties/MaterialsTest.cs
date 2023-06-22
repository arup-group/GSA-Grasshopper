using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types.Transforms;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysUnits;
using Rhino.Geometry;
using Xunit;
using static System.Net.WebRequestMethods;

namespace IntegrationTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class MaterialsTest {
    public static GH_Document Document => document ?? (document = OpenDocument());
    private static GH_Document document = null;

    [Fact]
    public void NoRuntimeErrorTest() {
      Helper.TestNoRuntimeMessagesInDocument(Document, GH_RuntimeMessageLevel.Error);
    }

    [Theory]
    [InlineData("ID", new int[] {
      4, 5, 6, 7, 1, 1, 1, 1
    })]
    [InlineData("Name", new string[] {
      "S235",
      "C30/37",
      "Sika CarboDur S",
      "1050A",
      "EN 338 C14",
      "Soda-lime",
      "Taconic Solus 1120",
      "my Custom",
    })]
    [InlineData("Type", new string[] {
      "Steel",
      "Concrete",
      "Frp",
      "Aluminium",
      "Timber",
      "Glass",
      "Fabric",
      "Generic",
    })]
    [InlineData("E-modulus", new double[] {
      210000,
      32836.568031,
      165000,
      69000,
      7000,
      70000,
      123456,
    })]
    public void Test(string groupIdentifier, object expected) {
      IGH_Param param = Helper.FindParameter(Document, groupIdentifier);
      Helper.TestGhPrimitives(param, expected);
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
        "1_Properties",
      });

      return Helper.CreateDocument(Path.Combine(path, fileName));
    }
  }
}
