using GsaAPI;

using GsaGH.Parameters;

using GsaGHTests.Helpers;

using Xunit;

using LoadCaseType = GsaGH.Parameters.LoadCaseType;
using GsaGH.Helpers;
namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaFaceLoadTest {
    [Fact]
    public void ConstructorTest() {
      var load = new GsaFaceLoad();

      Assert.Equal(FaceLoadType.CONSTANT, load.ApiLoad.Type);
    }

    [Fact]
    public void LoadCaseTest() {
      var load = new GsaFaceLoad();
      Assert.Null(load.LoadCase);
      load.LoadCase = new GsaLoadCase(99);
      Assert.Equal(99, load.LoadCase.Id);
    }

    [Fact]
    public void DuplicateConstantTest() {
      var original = new GsaFaceLoad {
        ApiLoad = {
          AxisProperty = 5,
          Case = 6,
          Direction = Direction.Z,
          EntityList = "all",
          EntityType = GsaAPI.EntityType.Element,
          Name = "name",
          Type = FaceLoadType.CONSTANT,
        },
      };
      var duplicate = (GsaFaceLoad)original.Duplicate();

      Duplicates.AreEqual(original, duplicate);

      duplicate.ApiLoad.Type = FaceLoadType.POINT;
      duplicate.ApiLoad.AxisProperty = 1;
      duplicate.ApiLoad.Case = 1;
      duplicate.ApiLoad.Direction = Direction.X;
      duplicate.ApiLoad.EntityList = "";
      duplicate.ApiLoad.Name = "";
      duplicate.ApiLoad.IsProjected = true;
      duplicate.ApiLoad.SetValue(0, 99);

      Assert.Equal(FaceLoadType.CONSTANT, original.ApiLoad.Type);
      Assert.Equal(5, original.ApiLoad.AxisProperty);
      Assert.Equal(6, original.ApiLoad.Case);
      Assert.Equal(Direction.Z, original.ApiLoad.Direction);
      Assert.Equal("all", original.ApiLoad.EntityList);
      Assert.Equal(GsaAPI.EntityType.Element, original.ApiLoad.EntityType);
      Assert.Equal("name", original.ApiLoad.Name);
      Assert.False(original.ApiLoad.IsProjected);
      Assert.Equal(0, original.ApiLoad.Value(0));
    }

    [Fact]
    public void DuplicateGeneralTest() {
      var original = new GsaFaceLoad {
        ApiLoad = {
          AxisProperty = 5,
          Case = 6,
          Direction = Direction.Z,
          EntityList = "all",
          EntityType = GsaAPI.EntityType.Element,
          Name = "name",
          Type = FaceLoadType.GENERAL,
        },
      };
      var duplicate = (GsaFaceLoad)original.Duplicate();

      Duplicates.AreEqual(original, duplicate);

      duplicate.ApiLoad.Type = FaceLoadType.POINT;
      duplicate.ApiLoad.AxisProperty = 1;
      duplicate.ApiLoad.Case = 1;
      duplicate.ApiLoad.Direction = Direction.X;
      duplicate.ApiLoad.EntityList = "";
      duplicate.ApiLoad.Name = "";
      duplicate.ApiLoad.IsProjected = true;
      duplicate.ApiLoad.SetValue(0, 99);

      Assert.Equal(FaceLoadType.GENERAL, original.ApiLoad.Type);
      Assert.Equal(5, original.ApiLoad.AxisProperty);
      Assert.Equal(6, original.ApiLoad.Case);
      Assert.Equal(Direction.Z, original.ApiLoad.Direction);
      Assert.Equal("all", original.ApiLoad.EntityList);
      Assert.Equal(GsaAPI.EntityType.Element, original.ApiLoad.EntityType);
      Assert.Equal("name", original.ApiLoad.Name);
      Assert.False(original.ApiLoad.IsProjected);
      Assert.Equal(0, original.ApiLoad.Value(0));
      Assert.Equal(0, original.ApiLoad.Value(1));
      Assert.Equal(0, original.ApiLoad.Value(2));
      Assert.Equal(0, original.ApiLoad.Value(3));
    }

    [Fact]
    public void DuplicatePointTest() {
      var original = new GsaFaceLoad {
        ApiLoad = {
          AxisProperty = 5,
          Case = 6,
          Direction = Direction.Z,
          EntityList = "all",
          EntityType = GsaAPI.EntityType.Element,
          Name = "name",
          Type = FaceLoadType.POINT,
          Position = new Vector2(0.5, 0.5)
        },
      };
      var duplicate = (GsaFaceLoad)original.Duplicate();

      Duplicates.AreEqual(original, duplicate);

      duplicate.ApiLoad.Type = FaceLoadType.GENERAL;
      duplicate.ApiLoad.AxisProperty = 1;
      duplicate.ApiLoad.Case = 1;
      duplicate.ApiLoad.Direction = Direction.X;
      duplicate.ApiLoad.EntityList = "";
      duplicate.ApiLoad.Name = "";
      duplicate.ApiLoad.IsProjected = true;
      duplicate.ApiLoad.SetValue(0, 99);

      Assert.Equal(FaceLoadType.POINT, original.ApiLoad.Type);
      Assert.Equal(5, original.ApiLoad.AxisProperty);
      Assert.Equal(6, original.ApiLoad.Case);
      Assert.Equal(Direction.Z, original.ApiLoad.Direction);
      Assert.Equal("all", original.ApiLoad.EntityList);
      Assert.Equal(GsaAPI.EntityType.Element, original.ApiLoad.EntityType);
      Assert.Equal("name", original.ApiLoad.Name);
      Assert.False(original.ApiLoad.IsProjected);
      Assert.Equal(0, original.ApiLoad.Value(0));
      Assert.Equal(0.5, original.ApiLoad.Position.X, DoubleComparer.Default);
      Assert.Equal(0.5, original.ApiLoad.Position.Y, DoubleComparer.Default);
    }

    [Fact]
    public void DuplicateEquationTest() {
      var original = new GsaFaceLoad {
        ApiLoad = {
          AxisProperty = 5,
          Case = 6,
          Direction = Direction.Z,
          EntityList = "all",
          EntityType = GsaAPI.EntityType.Element,
          Name = "name",
          Type = FaceLoadType.EQUATION,
        },
      };
      var equation = new PressureEquation() {
        Axis = 1,
        Expression = "2*x+4*y+z",
        LengthUnits = LengthUnit.Meter,
        IsUniform = true,
      };
      original.ApiLoad.SetEquation(equation);
      var duplicate = (GsaFaceLoad)original.Duplicate();

      Duplicates.AreEqual(original, duplicate);

      duplicate.ApiLoad.AxisProperty = 1;
      duplicate.ApiLoad.Case = 1;
      duplicate.ApiLoad.Direction = Direction.X;
      duplicate.ApiLoad.EntityList = "";
      duplicate.ApiLoad.Name = "";
      duplicate.ApiLoad.IsProjected = true;
      var newEquation = new PressureEquation() {
        Axis = 2,
        Expression = "3*x+5*y+7*z",
        LengthUnits = LengthUnit.Millimeter,
        IsUniform = false,
      };
      duplicate.ApiLoad.SetEquation(newEquation);

      Assert.Equal(FaceLoadType.EQUATION, original.ApiLoad.Type);
      Assert.Equal(5, original.ApiLoad.AxisProperty);
      Assert.Equal(6, original.ApiLoad.Case);
      Assert.Equal(Direction.Z, original.ApiLoad.Direction);
      Assert.Equal("all", original.ApiLoad.EntityList);
      Assert.Equal(GsaAPI.EntityType.Element, original.ApiLoad.EntityType);
      Assert.Equal("name", original.ApiLoad.Name);
      Assert.False(original.ApiLoad.IsProjected);

      Assert.Equal(equation.Axis, original.ApiLoad.Equation().Axis);
      Assert.Equal(equation.Expression, original.ApiLoad.Equation().Expression);
      Assert.Equal(equation.LengthUnits, original.ApiLoad.Equation().LengthUnits);
      Assert.Equal(equation.IsUniform, original.ApiLoad.Equation().IsUniform);
    }


    [Fact]
    public void DuplicateLoadCaseTest() {
      var load = new GsaFaceLoad();
      Assert.Null(load.LoadCase);
      var duplicate = (GsaFaceLoad)load.Duplicate();
      Assert.Null(duplicate.LoadCase);

      load.LoadCase = new GsaLoadCase(99);

      duplicate = (GsaFaceLoad)load.Duplicate();
      Assert.Equal(99, duplicate.LoadCase.Id);

      duplicate.LoadCase = new GsaLoadCase(1, LoadCaseType.Dead, "DeadLoad");

      Assert.Equal(99, load.LoadCase.Id);
      Assert.Equal(1, duplicate.LoadCase.Id);
      Assert.Equal("Dead", duplicate.LoadCase.LoadCase.CaseType.ToString());
      Assert.Equal("DeadLoad", duplicate.LoadCase.LoadCase.Name);
    }
  }
}
