using System;

using GsaAPI;

using GsaGH.Components;
using GsaGH.Parameters;
using GsaGH.Parameters.Enums;

using GsaGHTests.Components.Geometry;
using GsaGHTests.Helpers;

using OasysGH.Components;

using Xunit;

namespace GsaGHTests.Components.Loads {
  [Collection("GrasshopperFixture collection")]
  public class CreateFaceLoadTests {
    [Fact]
    public void CreateUniformTest() {
      var comp = new CreateFaceLoad();
      ComponentTestHelper.SetInput(comp, 7, 0);
      ComponentTestHelper.SetInput(comp, "All", 1);
      ComponentTestHelper.SetInput(comp, "myFaceLoad", 2);
      ComponentTestHelper.SetInput(comp, -5, 6);

      var output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);
      var load = (GsaFaceLoad)output.Value;
      Assert.Equal(7, load.LoadCase.Id);
      Assert.Equal("myFaceLoad", load.ApiLoad.Name);
      Assert.Equal(-5000, load.ApiLoad.Value(0));
      Assert.Equal(GsaAPI.FaceLoadType.CONSTANT, load.ApiLoad.Type);
      Assert.Equal(ReferenceType.None, load.ReferenceType);
    }

    [Fact]
    public void CreateVariableTest() {
      var comp = new CreateFaceLoad();
      comp.CreateAttributes();
      comp.SetSelected(0, 1); // Variable
      ComponentTestHelper.SetInput(comp, 7, 0);
      ComponentTestHelper.SetInput(comp, "All", 1);
      ComponentTestHelper.SetInput(comp, "myFaceLoad", 2);
      ComponentTestHelper.SetInput(comp, -1, 6);
      ComponentTestHelper.SetInput(comp, -2, 7);
      ComponentTestHelper.SetInput(comp, -3, 8);
      ComponentTestHelper.SetInput(comp, -4, 9);

      var output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);
      var load = (GsaFaceLoad)output.Value;
      Assert.Equal(7, load.LoadCase.Id);
      Assert.Equal("myFaceLoad", load.ApiLoad.Name);
      Assert.Equal(-1000, load.ApiLoad.Value(0));
      Assert.Equal(-2000, load.ApiLoad.Value(1));
      Assert.Equal(-3000, load.ApiLoad.Value(2));
      Assert.Equal(-4000, load.ApiLoad.Value(3));
      Assert.Equal(GsaAPI.FaceLoadType.GENERAL, load.ApiLoad.Type);
      Assert.Equal(ReferenceType.None, load.ReferenceType);
    }

    [Fact]
    public void CreatePointTest() {
      var comp = new CreateFaceLoad();
      comp.CreateAttributes();
      comp.SetSelected(0, 2); // Point
      ComponentTestHelper.SetInput(comp, 7, 0);
      ComponentTestHelper.SetInput(comp, "All", 1);
      ComponentTestHelper.SetInput(comp, "myFaceLoad", 2);
      ComponentTestHelper.SetInput(comp, -5, 6);
      ComponentTestHelper.SetInput(comp, 0.5, 7);
      ComponentTestHelper.SetInput(comp, 1.0, 8);

      var output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);
      var load = (GsaFaceLoad)output.Value;
      Assert.Equal(7, load.LoadCase.Id);
      Assert.Equal("myFaceLoad", load.ApiLoad.Name);
      Assert.Equal(-5000, load.ApiLoad.Value(0));
      Assert.Equal(0.5, load.ApiLoad.Position.X);
      Assert.Equal(1.0, load.ApiLoad.Position.Y);
      Assert.Equal(GsaAPI.FaceLoadType.POINT, load.ApiLoad.Type);
      Assert.Equal(ReferenceType.None, load.ReferenceType);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AxisTest(int axis) {
      var comp = new CreateFaceLoad();
      ComponentTestHelper.SetInput(comp, "All", 1);
      ComponentTestHelper.SetInput(comp, axis, 3);
      ComponentTestHelper.SetInput(comp, -5, 6);

      var output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);
      var load = (GsaFaceLoad)output.Value;
      Assert.Equal(-5000, load.ApiLoad.Value(0));
      Assert.Equal(axis, load.ApiLoad.AxisProperty);
    }

    [Theory]
    [InlineData("X")]
    [InlineData("Y")]
    [InlineData("Z")]
    public void DirectionTest(string direction) {
      var comp = new CreateFaceLoad();
      ComponentTestHelper.SetInput(comp, "All", 1);
      ComponentTestHelper.SetInput(comp, direction, 4);
      ComponentTestHelper.SetInput(comp, -5, 6);

      var output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);
      var load = (GsaFaceLoad)output.Value;
      Assert.Equal(-5000, load.ApiLoad.Value(0));
      Assert.Equal(direction, load.ApiLoad.Direction.ToString());
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ProjectedTest(bool project) {
      var comp = new CreateFaceLoad();
      ComponentTestHelper.SetInput(comp, "All", 1);
      ComponentTestHelper.SetInput(comp, project, 5);
      ComponentTestHelper.SetInput(comp, -5, 6);

      var output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);
      var load = (GsaFaceLoad)output.Value;
      Assert.Equal(-5000, load.ApiLoad.Value(0));
      Assert.Equal(project, load.ApiLoad.IsProjected);
    }

    [Fact]
    public void EntityListTypeErrorTest() {
      var comp = new CreateFaceLoad();
      var list = new GsaList("test", "1 2 3", GsaAPI.EntityType.Node);
      ComponentTestHelper.SetInput(comp, new GsaListGoo(list), 1);
      ComponentTestHelper.SetInput(comp, -5, 6);

      var output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);
      comp.Params.Output[0].ExpireSolution(true);
      comp.Params.Output[0].CollectData();
      Assert.Single(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void CreateElement2dLoadTest() {
      var comp = new CreateFaceLoad();
      GH_OasysComponent element2dComp = CreateElement2dTests.ComponentMother();
      var element2dGoo = (GsaElement2dGoo)ComponentTestHelper.GetOutput(element2dComp);

      ComponentTestHelper.SetInput(comp, element2dGoo, 1);
      ComponentTestHelper.SetInput(comp, -5, 6);

      var output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);
      var load = (GsaFaceLoad)output.Value;
      Assert.Equal(-5000, load.ApiLoad.Value(0));
      Assert.Equal(ReferenceType.Element, load.ReferenceType);
      Assert.Equal(GsaAPI.EntityType.Element, load.ApiLoad.EntityType);
    }

    [Fact]
    public void CreateMember2dLoadTest() {
      var comp = new CreateFaceLoad();
      GH_OasysComponent member2dComp = CreateMember2dTests.ComponentMother();
      var member2dGoo = (GsaMember2dGoo)ComponentTestHelper.GetOutput(member2dComp);

      ComponentTestHelper.SetInput(comp, member2dGoo, 1);
      ComponentTestHelper.SetInput(comp, -5, 6);

      var output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);
      var load = (GsaFaceLoad)output.Value;
      Assert.Equal(-5000, load.ApiLoad.Value(0));
      Assert.Equal(ReferenceType.Member, load.ReferenceType);
      Assert.Equal(GsaAPI.EntityType.Member, load.ApiLoad.EntityType);
    }

    [Fact]
    public void CreateDefaultEquationTest() {
      var comp = new CreateFaceLoad();
      comp.CreateAttributes();
      comp.SetSelected(0, 3); // Equation
      ComponentTestHelper.SetInput(comp, "All", 1);
      ComponentTestHelper.SetInput(comp, "myLoad", 2);
      ComponentTestHelper.SetInput(comp, "4*x+7*y-z", 7);

      var output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);
      var load = (GsaFaceLoad)output.Value;
      Assert.Equal("myLoad", load.ApiLoad.Name);
      Assert.Equal("4*x+7*y-z", load.ApiLoad.Equation().Expression);
      Assert.Equal(GsaAPI.FaceLoadType.EQUATION, load.ApiLoad.Type);
      Assert.Equal(0, load.ApiLoad.Equation().Axis);
      Assert.False(load.ApiLoad.Equation().IsUniform);
    }

    [Fact]
    public void CreateExtendedEquationTest() {
      var comp = new CreateFaceLoad();
      comp.CreateAttributes();
      comp.SetSelected(0, 3); // Equation
      ComponentTestHelper.SetInput(comp, "All", 1);
      ComponentTestHelper.SetInput(comp, "myLoad", 2);
      ComponentTestHelper.SetInput(comp, 1, 5); //axis
      ComponentTestHelper.SetInput(comp, true, 6); // constant
      ComponentTestHelper.SetInput(comp, "4*x+7*y-z", 7);

      var output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);
      var load = (GsaFaceLoad)output.Value;
      Assert.Equal("myLoad", load.ApiLoad.Name);
      Assert.Equal("4*x+7*y-z", load.ApiLoad.Equation().Expression);
      Assert.Equal(GsaAPI.FaceLoadType.EQUATION, load.ApiLoad.Type);
      Assert.Equal(1, load.ApiLoad.Equation().Axis);
      Assert.True(load.ApiLoad.Equation().IsUniform);
    }

    [Fact]
    public void ChangeUnitEquationTest() {
      var comp = new CreateFaceLoad();
      comp.CreateAttributes();
      comp.SetSelected(0, 3); // Equation
      ComponentTestHelper.SetInput(comp, "All", 1);
      ComponentTestHelper.SetInput(comp, "myLoad", 2);
      ComponentTestHelper.SetInput(comp, "4*x+7*y-z", 7);

      var output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);
      var load = (GsaFaceLoad)output.Value;
      Assert.Equal("myLoad", load.ApiLoad.Name);
      Assert.Equal("4*x+7*y-z", load.ApiLoad.Equation().Expression);
      Assert.Equal(GsaAPI.FaceLoadType.EQUATION, load.ApiLoad.Type);

      GsaAPI.StressUnit forceunit = StressUnit.Kilopascal;
      GsaAPI.LengthUnit lengthUnit = LengthUnit.Meter;
      Assert.Equal(lengthUnit, load.ApiLoad.Equation().LengthUnits);
      Assert.Equal(forceunit, load.ApiLoad.Equation().PressureUnits);

      comp.SetSelected(1, 0); // NewtonPerSquareMillimeter
      output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);
      load = (GsaFaceLoad)output.Value;
      forceunit = StressUnit.NewtonPerSquareMillimeter;
      lengthUnit = GsaAPI.LengthUnit.Millimeter;
      Assert.Equal(lengthUnit, load.ApiLoad.Equation().LengthUnits);
      Assert.Equal(forceunit, load.ApiLoad.Equation().PressureUnits);

      comp.SetSelected(1, 2); // NewtonPerSquareMeter
      output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);
      load = (GsaFaceLoad)output.Value;
      forceunit = StressUnit.NewtonPerSquareMeter;
      lengthUnit = GsaAPI.LengthUnit.Meter;
      Assert.Equal(lengthUnit, load.ApiLoad.Equation().LengthUnits);
      Assert.Equal(forceunit, load.ApiLoad.Equation().PressureUnits);

      comp.SetSelected(1, 4); // KilonewtonPerSquareMillimeter
      output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);
      load = (GsaFaceLoad)output.Value;
      forceunit = StressUnit.Gigapascal;
      lengthUnit = GsaAPI.LengthUnit.Millimeter;
      Assert.Equal(lengthUnit, load.ApiLoad.Equation().LengthUnits);
      Assert.Equal(forceunit, load.ApiLoad.Equation().PressureUnits);

      comp.SetSelected(1, 5); // KilonewtonPerSquareMeter
      output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);
      load = (GsaFaceLoad)output.Value;
      forceunit = StressUnit.Kilopascal;
      lengthUnit = GsaAPI.LengthUnit.Meter;
      Assert.Equal(lengthUnit, load.ApiLoad.Equation().LengthUnits);
      Assert.Equal(forceunit, load.ApiLoad.Equation().PressureUnits);

      comp.SetSelected(1, 6); // PoundForcePerSquareInch
      output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);
      load = (GsaFaceLoad)output.Value;
      forceunit = StressUnit.PoundForcePerSquareInch;
      lengthUnit = GsaAPI.LengthUnit.Inch;
      Assert.Equal(lengthUnit, load.ApiLoad.Equation().LengthUnits);
      Assert.Equal(forceunit, load.ApiLoad.Equation().PressureUnits);

      comp.SetSelected(1, 7); // PoundForcePerSquareFoot
      output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);
      load = (GsaFaceLoad)output.Value;
      forceunit = StressUnit.PoundForcePerSquareFoot;
      lengthUnit = GsaAPI.LengthUnit.Foot;
      Assert.Equal(lengthUnit, load.ApiLoad.Equation().LengthUnits);
      Assert.Equal(forceunit, load.ApiLoad.Equation().PressureUnits);

      comp.SetSelected(1, 8); // KilopoundForcePerSquareInch
      output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);
      load = (GsaFaceLoad)output.Value;
      forceunit = StressUnit.KilopoundForcePerSquareInch;
      lengthUnit = GsaAPI.LengthUnit.Inch;
      Assert.Equal(lengthUnit, load.ApiLoad.Equation().LengthUnits);
      Assert.Equal(forceunit, load.ApiLoad.Equation().PressureUnits);

      comp.SetSelected(1, 9); // KilopoundForcePerSquareFoot
      output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);
      load = (GsaFaceLoad)output.Value;
      forceunit = StressUnit.KilopoundForcePerSquareFoot;
      lengthUnit = GsaAPI.LengthUnit.Foot;
      Assert.Equal(lengthUnit, load.ApiLoad.Equation().LengthUnits);
      Assert.Equal(forceunit, load.ApiLoad.Equation().PressureUnits);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    public void ChangeUnitExceptionsEquationTest(int i) {
      var comp = new CreateFaceLoad();
      comp.CreateAttributes();
      comp.SetSelected(0, 3); // Equation
      ComponentTestHelper.SetInput(comp, "All", 1);
      ComponentTestHelper.SetInput(comp, "myLoad", 2);
      ComponentTestHelper.SetInput(comp, "4*x+7*y-z", 7);
      comp.SetSelected(1, i);
      Assert.Throws<ArgumentOutOfRangeException>(() => ComponentTestHelper.GetOutput(comp));
      Assert.Equal(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error, comp.RuntimeMessageLevel);
    }
  }
}
