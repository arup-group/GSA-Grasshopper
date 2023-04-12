using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Xunit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaProp2dTests {
    [Fact]
    public void DuplicateTest() {
      var original = new GsaProp2d {
        Name = "Name",
        Thickness = new Length(200, LengthUnit.Millimeter),
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
        GradeProperty = materialGradeProperty,
      };
      prop.Material = mat;

      Assert.Equal(1, prop.AxisProperty);
      Assert.Equal(4, prop.Material.GradeProperty);
      Assert.Equal(0, prop.Material.AnalysisProperty);
      Assert.Equal(MaterialType.GENERIC.ToString().ToPascalCase(), prop.Material.MaterialType.ToString());
      Assert.Equal("mariam", prop.Name);
      Assert.Equal("awesome property", prop.Description);
      Assert.Equal(Property2D_Type.LOAD.ToString(), prop.Type.ToString());
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

      var orig = new GsaProp2d(14) {
        AxisProperty = axisProperty,
        Name = name,
        Description = description,
        Type = type,
      };
      var mat = new GsaMaterial((int)materialType) {
        AnalysisProperty = materialAnalysisProperty,
      };
      orig.Material = mat;

      GsaProp2d dup = orig.Duplicate();

      orig.Id = 4;
      orig.AxisProperty = 1;
      orig.Material.GradeProperty = 4;
      orig.Material.AnalysisProperty = 42;
      orig.Material.MaterialType = GsaMaterial.MatType.Fabric;
      orig.Name = "kris";
      orig.Description = "less cool property";
      orig.Type = Property2D_Type.LOAD;
      orig.SupportType = SupportType.AllEdges;
      orig.ReferenceEdge = 4;

      Assert.Equal(0, dup.AxisProperty);
      Assert.Equal(0, dup.Material.GradeProperty);
      Assert.Equal(13, dup.Material.AnalysisProperty);
      Assert.Equal(MaterialType.UNDEF.ToString().ToPascalCase(), dup.Material.MaterialType.ToString());
      Assert.Equal("mariam", dup.Name);
      Assert.Equal("awesome property", dup.Description);
      Assert.Equal(Property2D_Type.SHELL.ToString(), dup.Type.ToString());
      Assert.Equal(14, dup.Id);

      Assert.Equal(1, orig.AxisProperty);
      Assert.Equal(0, orig.Material.GradeProperty);
      Assert.Equal(42, orig.Material.AnalysisProperty);
      Assert.Equal(MaterialType.FABRIC.ToString().ToPascalCase(), orig.Material.MaterialType.ToString());
      Assert.Equal("kris", orig.Name);
      Assert.Equal("less cool property", orig.Description);
      Assert.Equal(Property2D_Type.LOAD.ToString(), orig.Type.ToString());
      Assert.Equal(SupportType.AllEdges, orig.SupportType);
      Assert.Equal(4, orig.ReferenceEdge);
      Assert.Equal(4, orig.Id);
    }
  }
}
