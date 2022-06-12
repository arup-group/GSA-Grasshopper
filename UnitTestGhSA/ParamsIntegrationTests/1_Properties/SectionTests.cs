using System;
using NUnit.Framework;
using GsaGH;
using GsaGH.Parameters;
using Rhino.Geometry;
using GsaAPI;

namespace ParamsIntegrationTests
{
  public class SectionTests
  {
    [TestCase]
    public void TestCreateSection()
    {
      // string defining the profile
      string profile = "STD CHS 200 10";
      double myarea = Math.Round(
          Math.PI / 4 * Math.Pow(200, 2)
          - Math.PI / 4 * Math.Pow(200 - 2 * 10, 2),
          10);

      // create new section
      GsaSection sect = new GsaSection(profile);

      double area = Math.Round(
          sect.Area * Math.Pow(10, 6), // unit conversion
          10);
      Assert.AreEqual(myarea, area);

      // set other properties in section

      sect.Material.AnalysisProperty = 1;
      sect.Material.GradeProperty = 2;
      sect.Material.MaterialType = GsaMaterial.MatType.CONCRETE;
      sect.Name = "mariam";
      sect.Pool = 4;

      Assert.AreEqual(1, sect.Material.AnalysisProperty);
      Assert.AreEqual(2, sect.Material.GradeProperty);
      Assert.AreEqual(MaterialType.CONCRETE.ToString(),
          sect.Material.MaterialType.ToString());
      Assert.AreEqual("mariam", sect.Name);
      Assert.AreEqual(4, sect.Pool);
    }

    [TestCase]
    public void TestCreateSectionProfile()
    {
      // string defining the profile
      string profile = "STD R 15 20";
      double myarea = 15 * 20;
      // create new section with profile and ID
      GsaSection sect = new GsaSection(profile, 15);

      double area = sect.Area * Math.Pow(10, 6); // unit conversion
      Assert.AreEqual(myarea, area);
      Assert.AreEqual(15, sect.ID);
    }

    [TestCase]
    public void TestCreateGsaSectionCat()
    {
      string profile = "CAT HE HE200.B";
      GsaSection section = new GsaSection(profile);

      double area = section.Area * Math.Pow(10, 6);
      Assert.AreEqual(7808.121, area);
    }

    [TestCase]
    public void TestDuplicateSection()
    {
      string profile = "CAT HE HE200.B";
      double myarea1 = 7808.121;
      GsaSection orig = new GsaSection(profile);

      // set other properties in section
      orig.MaterialID = 1;
      orig.Material.GradeProperty = 2;
      orig.Material.MaterialType = GsaMaterial.MatType.STEEL;
      orig.Name = "mariam";
      orig.Pool = 12;

      // duplicate original
      GsaSection dup = orig.Duplicate();

      // make some changes to original
      string profile2 = "STD%R%15%20";
      double myarea2 = 15 * 20;
      orig.Profile = profile2;
      orig.Material.AnalysisProperty = 4;
      orig.Material.GradeProperty = 6;
      orig.Material.MaterialType = GsaMaterial.MatType.TIMBER;
      orig.Name = "kris";
      orig.Pool = 99;

      double area2 = orig.Area * Math.Pow(10, 6);
      Assert.AreEqual("STD R 15 20", orig.Profile);
      Assert.AreEqual(myarea2, area2);

      double area1 = dup.Area * Math.Pow(10, 6);
      Assert.AreEqual(profile, dup.Profile);
      Assert.AreEqual(myarea1, area1);

      Assert.AreEqual(1, dup.Material.AnalysisProperty);
      Assert.AreEqual(2, dup.Material.GradeProperty);
      Assert.AreEqual(MaterialType.STEEL.ToString(),
          dup.Material.MaterialType.ToString());
      Assert.AreEqual("mariam", dup.Name);
      Assert.AreEqual(12, dup.Pool);

      Assert.AreEqual(4, orig.Material.AnalysisProperty);
      Assert.AreEqual(6, orig.Material.GradeProperty);
      Assert.AreEqual(MaterialType.TIMBER.ToString(),
          orig.Material.MaterialType.ToString());
      Assert.AreEqual("kris", orig.Name);
      Assert.AreEqual(99, orig.Pool);
    }

    [TestCase]
    public void TestDuplicateEmptySection()
    {
      GsaSection section = new GsaSection();

      GsaSection dup = section.Duplicate();
      Assert.IsNotNull(dup);
    }
  }
}