using GsaGH.Components;
using GsaGH.Parameters;
using GsaGH.Parameters.Enums;
using GsaGHTests.Helpers;
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
      // position is not yet working!
      ComponentTestHelper.SetInput(comp, 0.5, 7);
      ComponentTestHelper.SetInput(comp, 1.0, 8);

      var output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);
      var load = (GsaFaceLoad)output.Value;
      Assert.Equal(7, load.LoadCase.Id);
      Assert.Equal("myFaceLoad", load.ApiLoad.Name);
      Assert.Equal(-5000, load.ApiLoad.Value(0));
      //Assert.Equal(0.5, load.ApiLoad.Position.X); 
      //Assert.Equal(1.0, load.ApiLoad.Position.Y);
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
  }
}
