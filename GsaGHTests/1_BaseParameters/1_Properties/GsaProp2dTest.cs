﻿using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Xunit;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaProp2dTests {

    [Fact]
    public void DuplicateTest() {
      var original = new GsaProp2d {
        Name = "Name",
        Thickness = new Length(200, LengthUnit.Millimeter),
        AdditionalOffsetZ = new Length(1, LengthUnit.Centimeter),
        ReferenceSurface = ReferenceSurface.Top
      };

      GsaProp2d duplicate = original.Duplicate();

      Duplicates.AreEqual(original, duplicate);
    }

    [Fact]
    public void TestCreateProp2d() {
      int axisProperty = 1;
      int materialGradeProperty = 4;
      MaterialType materialType = MaterialType.GENERIC;
      string name = "mariam";
      string description = "awesome property";
      Property2D_Type type = Property2D_Type.LOAD;

      var prop = new GsaProp2d {
        AxisProperty = axisProperty,
        Name = name,
        Description = description,
        Type = type,
        SupportType = SupportType.ThreeEdges,
        ReferenceEdge = 2,
      };
      var mat = new GsaMaterial((int)materialType) {
        Id = materialGradeProperty,
      };
      prop.Material = mat;

      Assert.Equal(1, prop.AxisProperty);
      Assert.Equal(4, prop.Material.Id);
      Assert.Equal(0, prop.Material.Id);
      Assert.Equal(MaterialType.GENERIC.ToString().ToPascalCase(),
        prop.Material.MaterialType.ToString());
      Assert.Equal("mariam", prop.Name);
      Assert.Equal("awesome property", prop.Description);
      Assert.Equal(Property2D_Type.LOAD.ToString().ToPascalCase(), 
        prop.Type.ToString().ToPascalCase());
      Assert.Equal(SupportType.ThreeEdges, prop.SupportType);
      Assert.Equal(2, prop.ReferenceEdge);
      Assert.Equal(0, prop.Id);
    }

    [Fact]
    public void TestDuplicateProp2d() {
      int axisProperty = 0;
      int materialAnalysisProperty = 13;
      MaterialType materialType = MaterialType.UNDEF;
      string name = "mariam";
      string description = "awesome property";
      Property2D_Type type = Property2D_Type.SHELL;
      ReferenceSurface referenceSurface = ReferenceSurface.Bottom;
      var offset = new Length(-100.0, LengthUnit.Millimeter);

      var orig = new GsaProp2d(14) {
        AxisProperty = axisProperty,
        Name = name,
        Description = description,
        Type = type,
        ReferenceSurface = referenceSurface,
        AdditionalOffsetZ = offset
      };
      var mat = new GsaMaterial((int)materialType) {
        Id = materialAnalysisProperty,
      };
      orig.Material = mat;

      GsaProp2d dup = orig.Duplicate();

      orig.Id = 4;
      orig.AxisProperty = 1;
      orig.Material.Id = 4;
      orig.Material.Id = 42;
      orig.Material.MaterialType = GsaMaterial.MatType.Fabric;
      orig.Name = "kris";
      orig.Description = "less cool property";
      orig.Type = Property2D_Type.LOAD;
      orig.SupportType = SupportType.AllEdges;
      orig.ReferenceEdge = 4;
      orig.ReferenceSurface = ReferenceSurface.Top;
      orig.AdditionalOffsetZ = new Length(50.0, LengthUnit.Millimeter);

      Assert.Equal(0, dup.AxisProperty);
      Assert.Equal(0, dup.Material.Id);
      Assert.Equal(13, dup.Material.Id);
      Assert.Equal(MaterialType.UNDEF.ToString().ToPascalCase(),
        dup.Material.MaterialType.ToString());
      Assert.Equal("mariam", dup.Name);
      Assert.Equal("awesome property", dup.Description);
      Assert.Equal(Property2D_Type.SHELL.ToString(), dup.Type.ToString());
      Assert.Equal(14, dup.Id);
      Assert.Equal(ReferenceSurface.Bottom, dup.ReferenceSurface);
      Assert.Equal(-100, dup.AdditionalOffsetZ.As(LengthUnit.Millimeter));

      Assert.Equal(1, orig.AxisProperty);
      Assert.Equal(0, orig.Material.Id);
      Assert.Equal(42, orig.Material.Id);
      Assert.Equal(MaterialType.FABRIC.ToString().ToPascalCase(),
        orig.Material.MaterialType.ToString());
      Assert.Equal("kris", orig.Name);
      Assert.Equal("less cool property", orig.Description);
      Assert.Equal(Property2D_Type.LOAD.ToString(), orig.Type.ToString());
      Assert.Equal(SupportType.AllEdges, orig.SupportType);
      Assert.Equal(4, orig.ReferenceEdge);
      Assert.Equal(4, orig.Id);
      Assert.Equal(ReferenceSurface.Top, orig.ReferenceSurface);
      Assert.Equal(50, orig.AdditionalOffsetZ.As(LengthUnit.Millimeter));
    }
  }
}
