using System;
using GsaAPI;
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
      // Arrange
      var original = new GsaSection();
      original.Name = "Name";

      // Act
      GsaSection duplicate = original.Duplicate();

      // Assert
      Duplicates.AreEqual(original, duplicate);
    }

    [Fact]
    public void TestCreateSection() {
      // string defining the profile
      string profile = "STD CHS 200 10";
      double myarea = Math.PI / 4 * Math.Pow(200, 2)
          - Math.PI / 4 * Math.Pow(200 - 2 * 10, 2);
      var area_expected = new Area(myarea, AreaUnit.SquareMillimeter);

      // create new section
      var sect = new GsaSection(profile);

      Assert.Equal(area_expected.Value, sect.Area.SquareMillimeters, 10);

      // set other properties in section

      sect.Material.GradeProperty = 2;
      sect.Material.MaterialType = GsaMaterial.MatType.Concrete;
      sect.Name = "mariam";
      sect.Pool = 4;

      Assert.Equal(0, sect.Material.AnalysisProperty);
      Assert.Equal(2, sect.Material.GradeProperty);
      Assert.Equal(MaterialType.CONCRETE.ToString(),
          sect.Material.MaterialType.ToString());
      Assert.Equal("mariam", sect.Name);
      Assert.Equal(4, sect.Pool);
    }

    [Fact]
    public void TestCreateSectionProfile() {
      // string defining the profile
      string profile = "STD R 15 20";
      double myarea = 15 * 20;
      var area_expected = new Area(myarea, AreaUnit.SquareMillimeter);

      // create new section with profile and ID
      var sect = new GsaSection(profile, 15);

      Assert.Equal(area_expected, sect.Area);
      Assert.Equal(15, sect.Id);
    }

    [Fact]
    public void TestCreateGsaSectionCat() {
      string profile = "CAT HE HE200.B";
      var section = new GsaSection(profile);

      var area_expected = new Area(7808.121, AreaUnit.SquareMillimeter);
      Assert.Equal(area_expected.Value, section.Area.SquareMillimeters, 10);
    }

    [Fact]
    public void TestDuplicateSection() {
      string profile = "CAT HE HE200.B";
      double myarea1 = 7808.121;
      var orig = new GsaSection(profile);

      // set other properties in section
      orig.MaterialID = 1;
      orig.Material.GradeProperty = 2;
      orig.Material.MaterialType = GsaMaterial.MatType.Steel;
      orig.Name = "mariam";
      orig.Pool = 12;

      // duplicate original
      GsaSection dup = orig.Duplicate();

      // make some changes to original
      string profile2 = "STD%R%15%20";
      double myarea2 = 15 * 20;
      var area_expected = new Area(myarea2, AreaUnit.SquareMillimeter);
      orig.Profile = profile2;
      orig.Material.AnalysisProperty = 4;
      orig.Material.MaterialType = GsaMaterial.MatType.Timber;
      orig.Name = "kris";
      orig.Pool = 99;

      Assert.Equal("STD R 15 20", orig.Profile);
      Assert.Equal(area_expected.SquareMillimeters, orig.Area.SquareMillimeters);

      Assert.Equal(profile, dup.Profile);
      Assert.Equal(myarea1, dup.Area.SquareMillimeters, 5);

      Assert.Equal(0, dup.Material.AnalysisProperty);
      Assert.Equal(2, dup.Material.GradeProperty);
      Assert.Equal(MaterialType.STEEL.ToString(),
          dup.Material.MaterialType.ToString());
      Assert.Equal("mariam", dup.Name);
      Assert.Equal(12, dup.Pool);

      Assert.Equal(4, orig.Material.AnalysisProperty);
      Assert.Equal(0, orig.Material.GradeProperty);
      Assert.Equal(MaterialType.TIMBER.ToString(),
          orig.Material.MaterialType.ToString());
      Assert.Equal("kris", orig.Name);
      Assert.Equal(99, orig.Pool);
    }

    [Fact]
    public void TestDuplicateEmptySection() {
      var section = new GsaSection();

      GsaSection dup = section.Duplicate();
      Assert.NotNull(dup);
    }
  }
}
