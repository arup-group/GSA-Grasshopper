using GsaAPI;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysUnits.Units;
using Xunit;

namespace GsaGHTests.Parameters
{
  [Collection("GrasshopperFixture collection")]
  public class GsaProp2dTests
  {
    [Fact]
    public void DuplicateTest()
    {
      // Arrange
      GsaProp2d original = new GsaProp2d();
      original.Name = "Name";
      original.Thickness = new OasysUnits.Length(200, LengthUnit.Millimeter);

      // Act
      GsaProp2d duplicate = original.Duplicate();

      // Assert
      Duplicates.AreEqual(original, duplicate);
    }

    [Fact]
    public void TestCreateProp2d()
    {
      int axisProperty = 1;
      int materialGradeProperty = 4;
      int materialAnalysisProperty = 42;
      MaterialType materialType = MaterialType.GENERIC;
      string name = "mariam";
      string description = "awesome property";
      Property2D_Type type = Property2D_Type.LOAD;

      // create new 2D property
      GsaProp2d prop = new GsaProp2d
      {
        AxisProperty = axisProperty,
        Name = name,
        Description = description,
        Type = type
      };
      GsaMaterial mat = new GsaMaterial((int)materialType)
      {
        AnalysisProperty = materialAnalysisProperty,
        GradeProperty = materialGradeProperty,
      };
      prop.Material = mat;

      Assert.Equal(-1, prop.AxisProperty);
      Assert.Equal(4, prop.Material.GradeProperty);
      Assert.Equal(42, prop.Material.AnalysisProperty);
      Assert.Equal(MaterialType.GENERIC.ToString(),
          prop.Material.MaterialType.ToString());
      Assert.Equal("mariam", prop.Name);
      Assert.Equal("awesome property", prop.Description);
      Assert.Equal(Property2D_Type.LOAD.ToString(),
          prop.Type.ToString());
      Assert.Equal(0, prop.ID);
    }

    [Fact]
    public void TestDuplicateProp2d()
    {
      int axisProperty = 0;
      int materialGradeProperty = 2;
      int materialAnalysisProperty = 13;
      MaterialType materialType = MaterialType.UNDEF;
      string name = "mariam";
      string description = "awesome property";
      Property2D_Type type = Property2D_Type.SHELL;

      // create new 2D property
      GsaProp2d orig = new GsaProp2d(14)
      {
        AxisProperty = axisProperty,
        Name = name,
        Description = description,
        Type = type
      };
      GsaMaterial mat = new GsaMaterial((int)materialType)
      {
        AnalysisProperty = materialAnalysisProperty,
        GradeProperty = materialGradeProperty,
      };
      orig.Material = mat;

      // duplicate prop
      GsaProp2d dup = orig.Duplicate();

      // make some changes to original
      orig.ID = 4;

      orig.AxisProperty = 1;
      orig.Material.GradeProperty = 4;
      orig.Material.AnalysisProperty = 42;
      orig.Material.MaterialType = GsaMaterial.MatType.FABRIC;
      orig.Name = "kris";
      orig.Description = "less cool property";
      orig.Type = Property2D_Type.CURVED_SHELL;

      Assert.Equal(0, dup.AxisProperty);
      Assert.Equal(2, dup.Material.GradeProperty);
      Assert.Equal(13, dup.Material.AnalysisProperty);
      Assert.Equal(MaterialType.UNDEF.ToString(),
          dup.Material.MaterialType.ToString());
      Assert.Equal("mariam", dup.Name);
      Assert.Equal("awesome property", dup.Description);
      Assert.Equal(Property2D_Type.SHELL.ToString(),
          dup.Type.ToString());
      Assert.Equal(14, dup.ID);

      Assert.Equal(-1, orig.AxisProperty);
      Assert.Equal(4, orig.Material.GradeProperty);
      Assert.Equal(42, orig.Material.AnalysisProperty);
      Assert.Equal(MaterialType.FABRIC.ToString(),
          orig.Material.MaterialType.ToString());
      Assert.Equal("kris", orig.Name);
      Assert.Equal("less cool property", orig.Description);
      Assert.Equal(Property2D_Type.CURVED_SHELL.ToString(),
          orig.Type.ToString());
      Assert.Equal(4, orig.ID);
    }
  }
}
