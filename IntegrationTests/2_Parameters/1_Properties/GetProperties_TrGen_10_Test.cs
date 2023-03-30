﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Grasshopper.Kernel;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Xunit;

namespace IntegrationTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  public class GetProperties_TrGen_10_Test {
    private static GH_Document s_document = null;

    public static GH_Document Document => s_document ?? (s_document = OpenDocument());

    private static GH_Document OpenDocument() {
      string fileName = MethodBase.GetCurrentMethod()
          .DeclaringType
        + ".gh";
      fileName = fileName.Replace("IntegrationTests.Parameters.", string.Empty);
      fileName = fileName.Replace("_Test", string.Empty);

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory())
        .Parent.Parent.Parent.Parent.FullName;
      string path = Path.Combine(new string[] {
        solutiondir,
        "ExampleFiles",
        "Parameters",
        "1_Properties",
      });

      return Helper.CreateDocument(Path.Combine(path, fileName));
    }

    [Theory]
    [InlineData("PbIds",
      new int[] {
        1,
        2,
        3,
        11,
        12,
        13,
      })]
    [InlineData("Profiles",
      new string[] {
        "STD R(m) 0.4 0.3",
        "STD R(m) 0.45 0.25",
        "STD R(m) 0.45 0.25",
        "CAT UC UC152x152x30 - S/S",
        "CAT UB UB203x133x25 - S/S",
        "CAT UB UB254x146x37 - S/S",
      })]
    [InlineData("PbE",
      new double[] {
        14000,
        205000,
      })]
    [InlineData("PbPoisson",
      new double[] {
        0.2,
        0.3,
      })]
    [InlineData("PbDensity",
      new double[] {
        2400,
        7850,
      })]
    [InlineData("PbThermal",
      new double[] {
        0.00001,
        0.000012,
      })]
    [InlineData("MatType",
      new string[] {
        "Concrete",
        "Steel",
      })]
    [InlineData("Pool", 1)]
    [InlineData("PbName",
      new string[] {
        "Column",
        "Beam - x",
        "Beam - y",
        "Column - top floor",
        "Beam - x - top floor",
        "Beam - y - top floor",
      })]
    [InlineData("PaIds",
      new int[] {
        1,
        2,
        3,
      })]
    [InlineData("PaE",
      new double[] {
        14000,
      })]
    [InlineData("PaPoisson",
      new double[] {
        0.2,
      })]
    [InlineData("PaDensity",
      new double[] {
        2400,
      })]
    [InlineData("PaThermal",
      new double[] {
        0.00001,
      })]
    [InlineData("Thickness",
      new double[] {
        30.000001,
        30.000001,
        30.000001,
      })]
    [InlineData("PaAxis",
      new int[] {
        0,
      })]
    [InlineData("PaAxOX",
      new double[] {
        23.33013,
        16,
      })]
    [InlineData("PaAxOY",
      new double[] {
        9.303848,
        12,
      })]
    [InlineData("PaAxOZ",
      new double[] {
        3.5,
        0,
      })]
    [InlineData("PaAxZX",
      new double[] {
        0,
        0,
      })]
    [InlineData("PaAxZY",
      new double[] {
        0,
        0,
      })]
    [InlineData("PaAxZZ",
      new double[] {
        1,
        1,
      })]
    [InlineData("PaName",
      new string[] {
        "2D property 1",
        "2D property 2",
        "2D property 3",
      })]
    [InlineData("PaType",
      new string[] {
        "Shell",
        "Shell",
        "Shell",
      })]
    public void Test(string groupIdentifier, object expected) {
      IGH_Param param = Helper.FindParameter(Document, groupIdentifier);
      Helper.TestGhPrimitives(param, expected);
    }

    [Fact]
    public void TestPBsAreEqual() {
      IGH_Param sectionFromGetPropertyParam = Helper.FindParameter(Document, "PBs");
      var sectionsFromGetProperty = new List<GsaSection>();
      for (int i = 0; i < sectionFromGetPropertyParam.VolatileDataCount; i++)
        sectionsFromGetProperty.Add(
          ((GsaSectionGoo)sectionFromGetPropertyParam.VolatileData.get_Branch(0)[i]).Value);

      IGH_Param sectionFromGetGeometryElemParam = Helper.FindParameter(Document, "PBsFromElem");
      var sectionFromGetGeometryElem = new List<GsaSection>();
      for (int i = 0; i < sectionFromGetGeometryElemParam.VolatileDataCount; i++)
        sectionFromGetGeometryElem.Add(
          ((GsaSectionGoo)sectionFromGetGeometryElemParam.VolatileData.get_Branch(0)[i]).Value);

      Assert.True(Duplicates.AreEqual(sectionsFromGetProperty, sectionFromGetGeometryElem));

      IGH_Param sectionFromGetGeometryMemParam = Helper.FindParameter(Document, "PBsFromMem");
      var sectionFromGetGeometryMem = new List<GsaSection>();
      for (int i = 0; i < sectionFromGetGeometryMemParam.VolatileDataCount; i++)
        sectionFromGetGeometryMem.Add(
          ((GsaSectionGoo)sectionFromGetGeometryMemParam.VolatileData.get_Branch(0)[i]).Value);

      Assert.True(Duplicates.AreEqual(sectionsFromGetProperty, sectionFromGetGeometryMem));
    }

    [Fact]
    public void TestPAsAreEqual() {
      IGH_Param sectionFromGetPropertyParam = Helper.FindParameter(Document, "PAs");
      var sectionsFromGetProperty = new List<GsaProp2d>();
      for (int i = 0; i < sectionFromGetPropertyParam.VolatileDataCount; i++)
        sectionsFromGetProperty.Add(
          ((GsaProp2dGoo)sectionFromGetPropertyParam.VolatileData.get_Branch(0)[i]).Value);

      IGH_Param sectionFromGetGeometryElemParam = Helper.FindParameter(Document, "PAsFromElem");
      var sectionFromGetGeometryElem = new List<GsaProp2d>();
      for (int i = 0; i < sectionFromGetGeometryElemParam.VolatileDataCount; i++)
        sectionFromGetGeometryElem.Add(
          ((GsaProp2dGoo)sectionFromGetGeometryElemParam.VolatileData.get_Branch(0)[i]).Value);

      Assert.True(Duplicates.AreEqual(sectionsFromGetProperty, sectionFromGetGeometryElem));

      IGH_Param sectionFromGetGeometryMemParam = Helper.FindParameter(Document, "PAsFromMem");
      var sectionFromGetGeometryMem = new List<GsaProp2d>();
      for (int i = 0; i < sectionFromGetGeometryMemParam.VolatileDataCount; i++)
        sectionFromGetGeometryMem.Add(
          ((GsaProp2dGoo)sectionFromGetGeometryMemParam.VolatileData.get_Branch(0)[i]).Value);

      Assert.True(Duplicates.AreEqual(sectionsFromGetProperty, sectionFromGetGeometryMem));
    }

    [Fact]
    public void NoRuntimeErrorTest()
      => Helper.TestNoRuntimeMessagesInDocument(Document, GH_RuntimeMessageLevel.Error);
  }
}
