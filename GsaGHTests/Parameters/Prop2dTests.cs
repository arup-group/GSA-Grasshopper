using GsaAPI;
using GsaGH.Parameters;
using Xunit;

namespace ParamsIntegrationTests
{
  public class Prop2dTests
  {
    [Fact]
    public void TestCreateProp2d()
    {
      // create new api property
      Prop2D apiProp = new Prop2D
      {
        AxisProperty = 1,
        MaterialGradeProperty = 4,
        MaterialAnalysisProperty = 42,
        MaterialType = MaterialType.GENERIC,
        Name = "mariam",
        Description = "awesome property",
        Type = Property2D_Type.LOAD
      };

      // create new 2D property
      GsaProp2d prop = new GsaProp2d
      {
        AxisProperty = apiProp.AxisProperty,
        Name = apiProp.Name,
        Description = apiProp.Description,
        Type = apiProp.Type
      };
      GsaMaterial mat = new GsaMaterial((int)apiProp.MaterialType)
      {
        AnalysisProperty = apiProp.MaterialAnalysisProperty,
        GradeProperty = apiProp.MaterialGradeProperty,
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
      Prop2D apiPropOriginal = new Prop2D
      {
        AxisProperty = 0,
        MaterialGradeProperty = 2,
        MaterialAnalysisProperty = 13,
        MaterialType = MaterialType.UNDEF,
        Name = "mariam",
        Description = "awesome property",
        Type = Property2D_Type.SHELL
      };

      // create new 2D property
      GsaProp2d orig = new GsaProp2d(14)
      {
        AxisProperty = apiPropOriginal.AxisProperty,
        Name = apiPropOriginal.Name,
        Description = apiPropOriginal.Description,
        Type = apiPropOriginal.Type
      };
      GsaMaterial mat = new GsaMaterial((int)apiPropOriginal.MaterialType)
      {
        AnalysisProperty = apiPropOriginal.MaterialAnalysisProperty,
        GradeProperty = apiPropOriginal.MaterialGradeProperty,
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
