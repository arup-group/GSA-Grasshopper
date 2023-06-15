using System;
using GsaAPI;
using GsaGH.Helpers.GH;
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
      double myarea = (Math.PI / 4 * Math.Pow(200, 2))
        - (Math.PI / 4 * Math.Pow(200 - (2 * 10), 2));
      var areaExpected = new Area(myarea, AreaUnit.SquareMillimeter);

      var sect = new GsaSection(profile);

      Assert.Equal(areaExpected.Value, sect.Area.SquareMillimeters, 10);

      sect.Material.Id = 2;
      //sect.Material.MaterialType = GsaMaterial.MatType.Concrete;
      sect.Name = "mariam";
      sect.Pool = 4;
      sect.BasicOffset = BasicOffset.TopRight;
      sect.AdditionalOffsetY = new Length(1, LengthUnit.Centimeter);
      sect.AdditionalOffsetZ = new Length(2, LengthUnit.Centimeter);

      Assert.Equal(0, sect.Material.Id);
      Assert.Equal(2, sect.Material.Id);
      Assert.Equal(MaterialType.CONCRETE.ToString().ToPascalCase(),
        sect.Material.MaterialType.ToString());
      Assert.Equal("mariam", sect.Name);
      Assert.Equal(4, sect.Pool);
      Assert.Equal(BasicOffset.TopRight, sect.BasicOffset);
      Assert.Equal(new Length(1, LengthUnit.Centimeter), sect.AdditionalOffsetY);
      Assert.Equal(new Length(2, LengthUnit.Centimeter), sect.AdditionalOffsetZ);
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
        //Material = {
        //  Id = 2,
        //  MaterialType = GsaMaterial.MatType.Steel,
        //},
        Name = "mariam",
        Pool = 12,
        BasicOffset = BasicOffset.BottomLeft,
        AdditionalOffsetY = new Length(-1, LengthUnit.Foot),
        AdditionalOffsetZ = new Length(-2, LengthUnit.Foot)
      };

      GsaSection dup = orig.Duplicate();

      string profile2 = "STD%R%15%20";
      double myarea2 = 15 * 20;
      var areaExpected = new Area(myarea2, AreaUnit.SquareMillimeter);
      orig.Profile = profile2;
      orig.Material.Id = 4;
      //orig.Material.MaterialType = GsaMaterial.MatType.Timber;
      orig.Name = "kris";
      orig.Pool = 99;
      orig.BasicOffset = BasicOffset.TopLeft;
      orig.AdditionalOffsetY = new Length(1, LengthUnit.Centimeter);
      orig.AdditionalOffsetZ = new Length(2, LengthUnit.Centimeter);

      Assert.Equal("STD R 15 20", orig.Profile);
      Assert.Equal(areaExpected.SquareMillimeters, orig.Area.SquareMillimeters);

      Assert.Equal(profile, dup.Profile.Substring(0, profile.Length));
      Assert.Equal(myarea1, dup.Area.SquareMillimeters, 5);

      Assert.Equal(0, dup.Material.Id);
      Assert.Equal(2, dup.Material.Id);
      Assert.Equal(MaterialType.STEEL.ToString().ToPascalCase(),
        dup.Material.MaterialType.ToString());
      Assert.Equal("mariam", dup.Name);
      Assert.Equal(12, dup.Pool);
      Assert.Equal(BasicOffset.BottomLeft, dup.BasicOffset);
      Assert.Equal(new Length(-1, LengthUnit.Foot), dup.AdditionalOffsetY);
      Assert.Equal(new Length(-2, LengthUnit.Foot), dup.AdditionalOffsetZ);

      Assert.Equal(4, orig.Material.Id);
      Assert.Equal(0, orig.Material.Id);
      Assert.Equal(MaterialType.TIMBER.ToString().ToPascalCase(),
        orig.Material.MaterialType.ToString());
      Assert.Equal("kris", orig.Name);
      Assert.Equal(99, orig.Pool);
      Assert.Equal(BasicOffset.TopLeft, orig.BasicOffset);
      Assert.Equal(new Length(1, LengthUnit.Centimeter), orig.AdditionalOffsetY);
      Assert.Equal(new Length(2, LengthUnit.Centimeter), orig.AdditionalOffsetZ);
    }
  }
}
