using System;
using System.Collections.Generic;

using GsaAPI;

using GsaGH.Helpers;
using GsaGH.Parameters;

using GsaGHTests.Helpers;

using OasysUnits;
using OasysUnits.Units;

using Xunit;

using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaSectionTest {
    [Fact]
    public void DuplicateTest() {
      var original = new GsaSection {
        ApiSection = new Section {
          Name = "Name",
        }
      };

      var duplicate = new GsaSection(original);
      Duplicates.AreEqual(original, duplicate, new List<string>() { "Guid" });
    }

    [Fact]
    public void DuplicateReferenceTest() {
      var original = new GsaSection(4);
      var duplicate = new GsaSection(original);
      Assert.Equal(4, duplicate.Id);
      Assert.True(duplicate.IsReferencedById);
    }

    [Fact]
    public void DuplicateReferenceTest2() {
      var original = new GsaSection(4);
      var duplicate = new GsaSection(original);
      Duplicates.AreEqual(original, duplicate, new List<string>() { "Guid" });
    }

    [Fact]
    public void TestCreateGsaSectionCat() {
      string profile = "CAT HE HE200.B";
      var section = new GsaSection(profile);

      var areaExpected = new Area(7808.121, AreaUnit.SquareMillimeter);
      Assert.Equal(areaExpected.Value, section.SectionProperties.Area.SquareMillimeters, DoubleComparer.Default);
    }

    [Fact]
    public void TestCreateSection() {
      string profile = "STD CHS 200 10";
      double myarea = (Math.PI / 4 * Math.Pow(200, 2))
        - (Math.PI / 4 * Math.Pow(200 - (2 * 10), 2));
      var areaExpected = new Area(myarea, AreaUnit.SquareMillimeter);

      var sect = new GsaSection(profile);
      var material = new GsaCustomMaterial(GsaMaterialTest.TestAnalysisMaterial(), 42);
      sect.Material = material;

      Assert.Equal(areaExpected.Value, sect.SectionProperties.Area.SquareMillimeters, DoubleComparer.Default);

      sect.Material.Id = 2;
      sect.ApiSection.Name = "mariam";
      sect.ApiSection.Pool = 4;
      sect.ApiSection.BasicOffset = BasicOffset.TopRight;
      sect.AdditionalOffsetY = new Length(1, LengthUnit.Centimeter);
      sect.AdditionalOffsetZ = new Length(2, LengthUnit.Centimeter);

      Assert.Equal(2, sect.Material.Id);
      Assert.Equal("Custom", sect.Material.MaterialType.ToString());
      Assert.Equal("mariam", sect.ApiSection.Name);
      Assert.Equal(4, sect.ApiSection.Pool);
      Assert.Equal(BasicOffset.TopRight, sect.ApiSection.BasicOffset);
      var tolerance = Length.FromMeters(1e-6);
      Assert.True(new Length(1, LengthUnit.Centimeter).Equals(sect.AdditionalOffsetY, tolerance));
      Assert.True(new Length(2, LengthUnit.Centimeter).Equals(sect.AdditionalOffsetZ, tolerance));

    }

    [Fact]
    public void TestCreateSectionProfile() {
      string profile = "STD R 15 20";
      double myarea = 15 * 20;
      var areaExpected = new Area(myarea, AreaUnit.SquareMillimeter);

      var sect = new GsaSection(profile) {
        Id = 15
      };

      var tolerance = Area.FromSquareMeters(1e-6);
      Assert.True(areaExpected.Equals(sect.SectionProperties.Area, tolerance));
      Assert.Equal(15, sect.Id);
    }

    [Fact]
    public void TestDuplicateEmptySection() {
      var section = new GsaSection();

      var dup = new GsaSection(section);
      Assert.NotNull(dup);
    }

    [Fact]
    public void TestDuplicateSection() {
      string profile = "CAT HE HE200.B";
      double myarea1 = 7808.121;
      var orig = new GsaSection(profile);
      orig.ApiSection.Name = "mariam";
      orig.ApiSection.Pool = 12;
      orig.ApiSection.BasicOffset = BasicOffset.BottomLeft;
      orig.AdditionalOffsetY = new Length(-1, LengthUnit.Foot);
      orig.AdditionalOffsetZ = new Length(-2, LengthUnit.Foot);

      orig.Material = new GsaCustomMaterial(GsaMaterialTest.TestAnalysisMaterial(), 42);

      var dup = new GsaSection(orig);

      string profile2 = "STD R 15 20";
      double myarea2 = 15 * 20;
      var areaExpected = new Area(myarea2, AreaUnit.SquareMillimeter);
      orig.ApiSection.Profile = profile2;
      orig.Material.Id = 4;
      orig.ApiSection.Name = "kris";
      orig.ApiSection.Pool = 99;
      orig.ApiSection.BasicOffset = BasicOffset.TopLeft;
      orig.AdditionalOffsetY = new Length(1, LengthUnit.Centimeter);
      orig.AdditionalOffsetZ = new Length(2, LengthUnit.Centimeter);
      var tolerance = Length.FromMeters(1e-6);

      Assert.Equal("STD R 15 20", orig.ApiSection.Profile);
      Assert.Equal(areaExpected.SquareMillimeters, orig.SectionProperties.Area.SquareMillimeters, DoubleComparer.Default);

      Assert.Equal(profile, dup.ApiSection.Profile.Substring(0, profile.Length));
      Assert.Equal(myarea1, dup.SectionProperties.Area.SquareMillimeters, DoubleComparer.Default);

      Assert.Equal(4, dup.Material.Id);
      Assert.Equal("Custom", dup.Material.MaterialType.ToString());
      Assert.Equal("mariam", dup.ApiSection.Name);
      Assert.Equal(12, dup.ApiSection.Pool);
      Assert.Equal(BasicOffset.BottomLeft, dup.ApiSection.BasicOffset);
      Assert.True(new Length(-1, LengthUnit.Foot).Equals(dup.AdditionalOffsetY, tolerance));
      Assert.True(new Length(-2, LengthUnit.Foot).Equals(dup.AdditionalOffsetZ, tolerance));

      Assert.Equal(4, orig.Material.Id);
      Assert.Equal("Custom", orig.Material.MaterialType.ToString());
      Assert.Equal("kris", orig.ApiSection.Name);
      Assert.Equal(99, orig.ApiSection.Pool);
      Assert.Equal(BasicOffset.TopLeft, orig.ApiSection.BasicOffset);
      Assert.True(new Length(1, LengthUnit.Centimeter).Equals(orig.AdditionalOffsetY, tolerance));
      Assert.True(new Length(2, LengthUnit.Centimeter).Equals(orig.AdditionalOffsetZ, tolerance));
    }
  }
}
