﻿using System;
using System.IO;
using System.Reflection;
using Grasshopper.Kernel;
using Xunit;

namespace IntegrationTests.Components {
  [Collection("GrasshopperFixture collection")]
  public class LoadsFromReferenceObjectsTests {
    private static GH_Document Document => s_document ?? (s_document = OpenDocument());
    private static GH_Document s_document = null;

    [Fact]
    public void NoRuntimeErrorTest() {
      Helper.TestNoRuntimeMessagesInDocument(Document, GH_RuntimeMessageLevel.Error);
      Helper.TestNoRuntimeMessagesInDocument(Document, GH_RuntimeMessageLevel.Warning);
    }

    [Theory]
    [InlineData("BeamLoadFromElementTest", true)]
    [InlineData("BeamLoadFromElementOldTest", true)]
    [InlineData("FaceLoadFromElementTest", 0)]
    [InlineData("FaceLoadsFromElementOldApplied",
      "1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28 29 30 31 32 33 34 35 36 37 38 39 40 41 42 43 44 45 46 47 48 49 50 51 52 53 54 55 56 57 58 59 60 61 62 63 64 65 66 67 68 69 70 71 72 73 74 75 76 77 78 79 80 81 82 83 84 85 86 87 88 89 90 91 92 93 94 95 96 97 98 99 100")]
    [InlineData("BeamLoadFromMemberTest", 0)]
    [InlineData("BeamLoadFromSectionTest", "PB7")]
    [InlineData("FaceLoadFromMemberTest", 0)]
    [InlineData("FaceLoadFromProp2dTest", "PA19")]
    [InlineData("GravityLoadFromSectionTest", "PB7")]
    [InlineData("GravityLoadFromProp2dTest", "PA19")]
    [InlineData("GravityLoadFromElem1dTest", 42)]
    [InlineData("GravityLoadFromElem2dTest", 0)]
    [InlineData("GravityLoadFromMember1dTest", 0)]
    [InlineData("GravityLoadFromMember2dTest", 0)]
    [InlineData("GravityOldTest", "all not (PB4)")]
    [InlineData("GridPlnSrfFromSectionTest", "PB7")]
    [InlineData("GridPlnSrfFromProp2dTest", "PA19")]
    [InlineData("GridPlnSrfFromElem2dTest", 0)]
    [InlineData("GridPlnSrfFromMem1dTest", 0)]
    [InlineData("GridPlnSrfFromMem2dTest", 0)]
    [InlineData("NodeLoadFromPtTest", 0)]
    [InlineData("NodeLoadOldID", new int[] {
      4,
      4,
      4,
      4,
    })]
    [InlineData("NodeLoadOldName", new string[] {
      "Wind X",
      "Wind X",
      "Wind X",
      "Wind X",
    })]
    [InlineData("NodeLoadOldDefinition", new string[] {
      "1",
      "1",
      "15 to 31 not 27",
      "15 to 31 not 27",
    })]
    [InlineData("LoadNodeOldDirection", new string[] {
      "X",
      "Y",
      "X",
      "Y",
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
