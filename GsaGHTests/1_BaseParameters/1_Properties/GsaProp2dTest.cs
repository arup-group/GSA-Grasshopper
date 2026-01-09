using System;
using System.Collections.Generic;

using GsaAPI;

using GsaGH.Helpers;
using GsaGH.Parameters;

using GsaGHTests.Helpers;

using OasysUnits;

using Xunit;

using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaProp2dTests {

    [Fact]
    public void DuplicateTest() {
      var original = new GsaProperty2d {
        ApiProp2d = new Prop2D() {
          Name = "Name",
          ReferenceSurface = ReferenceSurface.Top
        },
        Thickness = new Length(200, LengthUnit.Millimeter),
        AdditionalOffsetZ = new Length(1, LengthUnit.Centimeter),
      };

      var duplicate = new GsaProperty2d(original);

      Duplicates.AreEqual(original, duplicate, new List<string>() { "Guid" });
    }

    [Fact]
    public void DuplicateReferenceTest() {
      var original = new GsaProperty2d(4);
      var duplicate = new GsaProperty2d(original);
      Assert.Equal(4, duplicate.Id);
      Assert.True(duplicate.IsReferencedById);
    }

    [Fact]
    public void DuplicateReferenceTest2() {
      var original = new GsaProperty2d(4);
      var duplicate = new GsaProperty2d(original);
      Duplicates.AreEqual(original, duplicate, new List<string>() { "Guid" });
    }

    [Fact]
    public void TestCreateProp2d() {
      int axisProperty = 1;
      string name = "mariam";
      string description = "awesome property";
      Property2D_Type type = Property2D_Type.LOAD;
      //load panel must be in global axis
      Assert.Throws<ArgumentException>(() => new GsaProperty2d {
        ApiProp2d = new Prop2D() {
          Type = type,
          AxisProperty = axisProperty,
          Name = name,
          Description = description,
          SupportType = SupportType.ThreeEdges,
          ReferenceEdge = 2,
        },
      });

      axisProperty = 0;
      var prop = new GsaProperty2d {
        ApiProp2d = new Prop2D() {
          Type = type,
          AxisProperty = axisProperty,
          Name = name,
          Description = description,
          SupportType = SupportType.ThreeEdges,
          ReferenceEdge = 2,
        },
      };
      var material = new GsaCustomMaterial(GsaMaterialTest.TestAnalysisMaterial(), 99);
      prop.Material = material;

      Assert.Equal(0, prop.ApiProp2d.AxisProperty);
      Assert.Equal(99, prop.Material.Id);
      Assert.Equal("Custom", prop.Material.MaterialType.ToString());
      Assert.Equal("mariam", prop.ApiProp2d.Name);
      Assert.Equal("awesome property", prop.ApiProp2d.Description);
      Assert.Equal(Property2D_Type.LOAD.ToString().ToPascalCase(),
        prop.ApiProp2d.Type.ToString().ToPascalCase());
      Assert.Equal(SupportType.ThreeEdges, prop.ApiProp2d.SupportType);
      Assert.Equal(2, prop.ApiProp2d.ReferenceEdge);
      Assert.Equal(0, prop.Id);
    }

    [Fact]
    public void TestDuplicateProp2d() {
      int axisProperty = 0;
      string name = "mariam";
      string description = "awesome property";
      Property2D_Type type = Property2D_Type.SHELL;
      ReferenceSurface referenceSurface = ReferenceSurface.Bottom;
      var offset = new Length(-100.0, LengthUnit.Millimeter);

      var orig = new GsaProperty2d() {
        Id = 14,
        ApiProp2d = new Prop2D() {
          AxisProperty = axisProperty,
          Name = name,
          Description = description,
          Type = type,
          ReferenceSurface = referenceSurface,
        },
        AdditionalOffsetZ = offset
      };
      var material = new GsaCustomMaterial(GsaMaterialTest.TestAnalysisMaterial(), 42);
      orig.Material = material;

      var dup = new GsaProperty2d(orig);

      orig.Id = 4;
      orig.ApiProp2d.AxisProperty = 0;
      orig.Material.Id = 99;
      orig.ApiProp2d.Name = "kris";
      orig.ApiProp2d.Description = "less cool property";
      orig.ApiProp2d.Type = Property2D_Type.LOAD;
      orig.ApiProp2d.SupportType = SupportType.AllEdges;
      orig.ApiProp2d.ReferenceSurface = ReferenceSurface.Top;
      orig.AdditionalOffsetZ = new Length(50.0, LengthUnit.Millimeter);

      Assert.Equal(0, dup.ApiProp2d.AxisProperty);
      Assert.Equal(99, dup.Material.Id);
      Assert.Equal("Custom", dup.Material.MaterialType.ToString());
      Assert.Equal("mariam", dup.ApiProp2d.Name);
      Assert.Equal("awesome property", dup.ApiProp2d.Description);
      Assert.Equal(Property2D_Type.SHELL.ToString(), dup.ApiProp2d.Type.ToString());
      Assert.Equal(14, dup.Id);
      Assert.Equal(ReferenceSurface.Bottom, dup.ApiProp2d.ReferenceSurface);
      Assert.Equal(-100, dup.AdditionalOffsetZ.As(LengthUnit.Millimeter), DoubleComparer.Default);

      Assert.Equal(0, orig.ApiProp2d.AxisProperty);
      Assert.Equal(99, orig.Material.Id);
      Assert.Equal("Custom", orig.Material.MaterialType.ToString());
      Assert.Equal("kris", orig.ApiProp2d.Name);
      Assert.Equal("less cool property", orig.ApiProp2d.Description);
      Assert.Equal(Property2D_Type.LOAD.ToString(), orig.ApiProp2d.Type.ToString());
      Assert.Equal(SupportType.AllEdges, orig.ApiProp2d.SupportType);
      Assert.Equal(4, orig.Id);
      Assert.Equal(ReferenceSurface.Top, orig.ApiProp2d.ReferenceSurface);
      Assert.Equal(50, orig.AdditionalOffsetZ.As(LengthUnit.Millimeter), DoubleComparer.Default);
    }
  }
}
