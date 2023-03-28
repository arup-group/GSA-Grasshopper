﻿using System;
using System.IO;
using System.Reflection;
using Grasshopper.Kernel;
using Xunit;

namespace IntegrationTests.Components {
  [Collection("GrasshopperFixture collection")]
  public class CreateModelWithReferenceIDsTests {
    private static GH_Document s_document = null;
    private static GH_Document Document => s_document ?? (s_document = OpenDocument());

    private static GH_Document OpenDocument() {
      Type thisClass = MethodBase.GetCurrentMethod()
        .DeclaringType;
      string fileName = thisClass.Name + ".gh";
      fileName = fileName.Replace(thisClass.Namespace, string.Empty)
        .Replace("Tests", string.Empty);

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory())
        .Parent.Parent.Parent.Parent.FullName;
      string path = Path.Combine(new string[] {
        solutiondir,
        "ExampleFiles",
        "Components",
      });

      return Helper.CreateDocument(Path.Combine(path, fileName));
    }

    [Theory]
    [InlineData("Profiles",
      new string[] {
        "CAT HE HE240.A",
        "CAT HE HE260.B",
      })]
    [InlineData("SteelE",
      new double[] {
        200000,
        200000,
      })]
    [InlineData("Thickness", (double)25)]
    [InlineData("ConcreteE", (double)18000)]
    public void Test(string groupIdentifier, object expected) {
      IGH_Param param = Helper.FindParameter(Document, groupIdentifier);
      Helper.TestGhPrimitives(param, expected);
    }

    [Fact]
    public void NoRuntimeErrorTest() {
      Helper.TestNoRuntimeMessagesInDocument(Document, GH_RuntimeMessageLevel.Error);
      Helper.TestNoRuntimeMessagesInDocument(Document, GH_RuntimeMessageLevel.Warning);
    }
  }
}
