using System;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Xunit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaSectionTest {
    [Fact]
    public void DuplicateTest() {
      var original = new GsaSection {
        Name = "Name",
      };

      GsaSection duplicate = original.Duplicate();

      Duplicates.AreEqual(original, duplicate);
    }

    [Fact]
    public void TestCreateGsaSectionCat() {
      string profile = "CAT HE HE200.B";
      var section = new GsaSection(profile);

      var areaExpected = new Area(7808.121, AreaUnit.SquareMillimeter);
      Assert.Equal(areaExpected.Value, section.Area.SquareMillimeters, 10);
    }

    [Fact]
    public void TestCreateSection() {
      string profile = "STD CHS 200 10";
      double myarea = Math.PI / 4 * Math.Pow(200, 2) - Math.PI / 4 * Math.Pow(200 - 2 * 10, 2);
      var areaExpected = new Area(myarea, AreaUnit.SquareMillimeter);

      var sect = new GsaSection(profile);

      Assert.Equal(areaExpected.Value, sect.Area.SquareMillimeters, 10);

      sect.Material.GradeProperty = 2;
      sect.Material.MaterialType = GsaMaterial.MatType.Concrete;
      sect.Name = "mariam";
      sect.Pool = 4;

      Assert.Equal(0, sect.Material.AnalysisProperty);
      Assert.Equal(2, sect.Material.GradeProperty);
      Assert.Equal(MaterialType.CONCRETE.ToString().ToPascalCase(), sect.Material.MaterialType.ToString());
      Assert.Equal("mariam", sect.Name);
      Assert.Equal(4, sect.Pool);
    }

    [Fact]
    public void TestCreateSectionProfile() {
      string profile = "STD R 15 20";
      double myarea = 15 * 20;
      var areaExpected = new Area(myarea, AreaUnit.SquareMillimeter);

      var sect = new GsaSection(profile, 15);

      Assert.Equal(areaExpected, sect.Area);
      Assert.Equal(15, sect.Id);
    }

    [Fact]
    public void TestDuplicateEmptySection() {
      var section = new GsaSection();

      GsaSection dup = section.Duplicate();
      Assert.NotNull(dup);
    }

    [Fact]
    public void TestDuplicateSection() {
      string profile = "CAT HE HE200.B";
      double myarea1 = 7808.121;
      var orig = new GsaSection(profile) {
        MaterialId = 1,
        Material = {
          GradeProperty = 2,
          MaterialType = GsaMaterial.MatType.Steel,
        },
        Name = "mariam",
        Pool = 12,
      };

      GsaSection dup = orig.Duplicate();

      string profile2 = "STD%R%15%20";
      double myarea2 = 15 * 20;
      var areaExpected = new Area(myarea2, AreaUnit.SquareMillimeter);
      orig.Profile = profile2;
      orig.Material.AnalysisProperty = 4;
      orig.Material.MaterialType = GsaMaterial.MatType.Timber;
      orig.Name = "kris";
      orig.Pool = 99;

      Assert.Equal("STD R 15 20", orig.Profile);
      Assert.Equal(areaExpected.SquareMillimeters, orig.Area.SquareMillimeters);

      Assert.Equal(profile, dup.Profile.Substring(0, profile.Length));
      Assert.Equal(myarea1, dup.Area.SquareMillimeters, 5);

      Assert.Equal(0, dup.Material.AnalysisProperty);
      Assert.Equal(2, dup.Material.GradeProperty);
      Assert.Equal(MaterialType.STEEL.ToString().ToPascalCase(), dup.Material.MaterialType.ToString());
      Assert.Equal("mariam", dup.Name);
      Assert.Equal(12, dup.Pool);

      Assert.Equal(4, orig.Material.AnalysisProperty);
      Assert.Equal(0, orig.Material.GradeProperty);
      Assert.Equal(MaterialType.TIMBER.ToString().ToPascalCase(), orig.Material.MaterialType.ToString());
      Assert.Equal("kris", orig.Name);
      Assert.Equal(99, orig.Pool);
    }
  }
}
