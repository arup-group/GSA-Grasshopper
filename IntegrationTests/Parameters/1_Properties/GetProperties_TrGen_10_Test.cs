using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Grasshopper.Kernel;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Xunit;

namespace IntegrationTests.Parameters
{
  [Collection("GrasshopperFixture collection")]
  public class GetProperties_TrGen_10_Test
  {
    public static GH_Document Document()
    {
      string fileName = MethodBase.GetCurrentMethod().DeclaringType + ".gh";
      fileName = fileName.Replace("IntegrationTests.Parameters.", string.Empty);
      fileName = fileName.Replace("_Test", string.Empty);

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent.FullName;
      string path = Path.Combine(new string[] { solutiondir, "ExampleFiles", "Parameters", "1_Properties" });

      return Helper.CreateDocument(Path.Combine(path, fileName));
    }

    [Theory]
    [InlineData("PbIds", new int[] { 1, 2, 3, 11, 12, 13 })]
    [InlineData("Profiles", new string[] { "STD R(m) 0.4 0.3", "STD R(m) 0.45 0.25", "STD R(m) 0.45 0.25", "CAT UC UC152x152x30 - S/S", "CAT UB UB203x133x25 - S/S", "CAT UB UB254x146x37 - S/S" })]
    [InlineData("PbE", new double[] { 14000, 205000 })]
    [InlineData("PbPoisson", new double[] { 0.2, 0.3 })]
    [InlineData("PbDensity", new double[] { 2400, 7850 })]
    [InlineData("PbThermal", new double[] { 0.00001, 0.000012 })]
    [InlineData("MatType", new string[] { "Concrete", "Steel" })]
    [InlineData("Pool", 1)]
    [InlineData("PbName", new string[] { "Column", "Beam - x", "Beam - y", "Column - top floor", "Beam - x - top floor", "Beam - y - top floor" })]
    [InlineData("PaIds", new int[] { 1, 2, 3 })]
    [InlineData("PaE", new double[] { 14000 })]
    [InlineData("PaPoisson", new double[] { 0.2 })]
    [InlineData("PaDensity", new double[] { 2400 })]
    [InlineData("PaThermal", new double[] { 0.00001 })]
    [InlineData("Thickness", new double[] { 30.000001, 30.000001, 30.000001 })]
    [InlineData("PaAxis", new int[] { 0, 2, 1 })]
    [InlineData("PaName", new string[] { "2D property 1", "2D property 2", "2D property 3" })]
    [InlineData("PaType", new string[] { "Shell", "Shell", "Shell" })]
    public void Test(string groupIdentifier, object expected)
    {
      IGH_Param param = Helper.FindParameter(Document(), groupIdentifier);
      Helper.TestGHPrimitives(param, expected);
    }

    [Fact]
    public void TestPBsAreEqual() 
    {
      IGH_Param sectionFromGetPropertyParam = Helper.FindParameter(Document(), "PBs");
      List<GsaSection> sectionsFromGetProperty = new List<GsaSection>();
      for(int i = 0; i < sectionFromGetPropertyParam.VolatileDataCount; i++)
        sectionsFromGetProperty.Add(((GsaSectionGoo)sectionFromGetPropertyParam.VolatileData.get_Branch(0)[i]).Value);

      IGH_Param sectionFromGetGeometryElemParam = Helper.FindParameter(Document(), "PBsFromElem");
      List<GsaSection> sectionFromGetGeometryElem = new List<GsaSection>();
      for (int i = 0; i < sectionFromGetGeometryElemParam.VolatileDataCount; i++)
        sectionFromGetGeometryElem.Add(((GsaSectionGoo)sectionFromGetGeometryElemParam.VolatileData.get_Branch(0)[i]).Value);

      Assert.True(Duplicates.AreEqual(sectionsFromGetProperty, sectionFromGetGeometryElem));

      IGH_Param sectionFromGetGeometryMemParam = Helper.FindParameter(Document(), "PBsFromMem");
      List<GsaSection> sectionFromGetGeometryMem = new List<GsaSection>();
      for (int i = 0; i < sectionFromGetGeometryMemParam.VolatileDataCount; i++)
        sectionFromGetGeometryMem.Add(((GsaSectionGoo)sectionFromGetGeometryMemParam.VolatileData.get_Branch(0)[i]).Value);

      Assert.True(Duplicates.AreEqual(sectionsFromGetProperty, sectionFromGetGeometryMem));
    }

    [Fact]
    public void TestPAsAreEqual()
    {
      IGH_Param sectionFromGetPropertyParam = Helper.FindParameter(Document(), "PAs");
      List<GsaProp2d> sectionsFromGetProperty = new List<GsaProp2d>();
      for (int i = 0; i < sectionFromGetPropertyParam.VolatileDataCount; i++)
        sectionsFromGetProperty.Add(((GsaProp2dGoo)sectionFromGetPropertyParam.VolatileData.get_Branch(0)[i]).Value);

      IGH_Param sectionFromGetGeometryElemParam = Helper.FindParameter(Document(), "PAsFromElem");
      List<GsaProp2d> sectionFromGetGeometryElem = new List<GsaProp2d>();
      for (int i = 0; i < sectionFromGetGeometryElemParam.VolatileDataCount; i++)
        sectionFromGetGeometryElem.Add(((GsaProp2dGoo)sectionFromGetGeometryElemParam.VolatileData.get_Branch(0)[i]).Value);

      Assert.True(Duplicates.AreEqual(sectionsFromGetProperty, sectionFromGetGeometryElem));

      IGH_Param sectionFromGetGeometryMemParam = Helper.FindParameter(Document(), "PAsFromMem");
      List<GsaProp2d> sectionFromGetGeometryMem = new List<GsaProp2d>();
      for (int i = 0; i < sectionFromGetGeometryMemParam.VolatileDataCount; i++)
        sectionFromGetGeometryMem.Add(((GsaProp2dGoo)sectionFromGetGeometryMemParam.VolatileData.get_Branch(0)[i]).Value);

      Assert.True(Duplicates.AreEqual(sectionsFromGetProperty, sectionFromGetGeometryMem));
    }

    [Fact]
    public void NoRuntimeErrorTest()
    {
      Helper.TestNoRuntimeMessagesInDocument(Document(), GH_RuntimeMessageLevel.Error);
    }
  }
}
