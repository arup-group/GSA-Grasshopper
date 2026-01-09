using GsaGH.Components;
using GsaGH.Parameters;
using GsaGH.Parameters.Enums;

using GsaGHTests.Components.Geometry;
using GsaGHTests.Helpers;

using OasysGH.Components;

using Xunit;

namespace GsaGHTests.Components.Loads {
  [Collection("GrasshopperFixture collection")]
  public class CreateBeamLoadTests {
    [Fact]
    public void CreateUniformTest() {
      var comp = new CreateBeamLoad();
      ComponentTestHelper.SetInput(comp, "All", 1);
      ComponentTestHelper.SetInput(comp, -5, 6);

      var output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);
      var load = (GsaBeamLoad)output.Value;
      Assert.Equal(-5000, load.ApiLoad.Value(0));
      Assert.Equal(GsaAPI.BeamLoadType.UNIFORM, load.ApiLoad.Type);
      Assert.Equal(ReferenceType.None, load.ReferenceType);
    }

    [Fact]
    public void CreatePointTest() {
      var comp = new CreateBeamLoad();
      comp.CreateAttributes();
      comp.SetSelected(0, 0); // Point
      ComponentTestHelper.SetInput(comp, "All", 1);
      ComponentTestHelper.SetInput(comp, -5, 6);
      ComponentTestHelper.SetInput(comp, 0.6, 7);
      var output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);

      var load = (GsaBeamLoad)output.Value;
      Assert.Equal(-5000, load.ApiLoad.Value(0));
      Assert.Equal(GsaAPI.BeamLoadType.POINT, load.ApiLoad.Type);
      Assert.Equal(-0.6, load.ApiLoad.Position(0), 12);
    }

    [Fact]
    public void CreateLinearTest() {
      var comp = new CreateBeamLoad();
      comp.CreateAttributes();
      comp.SetSelected(0, 2); // Linear
      ComponentTestHelper.SetInput(comp, "All", 1);
      ComponentTestHelper.SetInput(comp, -5, 6);
      ComponentTestHelper.SetInput(comp, -6, 7);
      var output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);

      var load = (GsaBeamLoad)output.Value;
      Assert.Equal(-5000, load.ApiLoad.Value(0));
      Assert.Equal(-6000, load.ApiLoad.Value(1));
      Assert.Equal(GsaAPI.BeamLoadType.LINEAR, load.ApiLoad.Type);
    }

    [Fact]
    public void CreatePatchTest() {
      var comp = new CreateBeamLoad();
      comp.CreateAttributes();
      comp.SetSelected(0, 3); // Patch
      ComponentTestHelper.SetInput(comp, "All", 1);
      ComponentTestHelper.SetInput(comp, -5, 6);
      ComponentTestHelper.SetInput(comp, 0.2, 7);
      ComponentTestHelper.SetInput(comp, -6, 8);
      ComponentTestHelper.SetInput(comp, 0.8, 9);
      var output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);

      var load = (GsaBeamLoad)output.Value;
      Assert.Equal(-5000, load.ApiLoad.Value(0));
      Assert.Equal(-0.2, load.ApiLoad.Position(0), 12);
      Assert.Equal(-6000, load.ApiLoad.Value(1));
      Assert.Equal(-0.8, load.ApiLoad.Position(1), 12);
      Assert.Equal(GsaAPI.BeamLoadType.PATCH, load.ApiLoad.Type);
    }

    [Fact]
    public void CreateTriLinearTest() {
      var comp = new CreateBeamLoad();
      comp.CreateAttributes();
      comp.SetSelected(0, 4); // Trilinear
      ComponentTestHelper.SetInput(comp, "All", 1);
      ComponentTestHelper.SetInput(comp, -5, 6);
      ComponentTestHelper.SetInput(comp, 0.2, 7);
      ComponentTestHelper.SetInput(comp, -6, 8);
      ComponentTestHelper.SetInput(comp, 0.8, 9);
      var output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);

      var load = (GsaBeamLoad)output.Value;
      Assert.Equal(-5000, load.ApiLoad.Value(0));
      Assert.Equal(-0.2, load.ApiLoad.Position(0), 12);
      Assert.Equal(-6000, load.ApiLoad.Value(1));
      Assert.Equal(-0.8, load.ApiLoad.Position(1), 12);
      Assert.Equal(GsaAPI.BeamLoadType.TRILINEAR, load.ApiLoad.Type);
    }

    [Fact]
    public void NameTest() {
      var comp = new CreateBeamLoad();
      ComponentTestHelper.SetInput(comp, "All", 1);
      ComponentTestHelper.SetInput(comp, "myBeamLoad", 2);
      ComponentTestHelper.SetInput(comp, -5, 6);

      var output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);
      var load = (GsaBeamLoad)output.Value;
      Assert.Equal(-5000, load.ApiLoad.Value(0));
      Assert.Equal("myBeamLoad", load.ApiLoad.Name);
    }

    [Fact]
    public void AxisTest() {
      var comp = new CreateBeamLoad();
      ComponentTestHelper.SetInput(comp, "All", 1);
      ComponentTestHelper.SetInput(comp, -1, 3);
      ComponentTestHelper.SetInput(comp, -5, 6);

      var output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);
      var load = (GsaBeamLoad)output.Value;
      Assert.Equal(-5000, load.ApiLoad.Value(0));
      Assert.Equal(-1, load.ApiLoad.AxisProperty);
    }

    [Theory]
    [InlineData("X")]
    [InlineData("Y")]
    [InlineData("XX")]
    [InlineData("YY")]
    [InlineData("ZZ")]
    public void DirectionTest(string direction) {
      var comp = new CreateBeamLoad();
      ComponentTestHelper.SetInput(comp, "All", 1);
      ComponentTestHelper.SetInput(comp, direction, 4);
      ComponentTestHelper.SetInput(comp, -5, 6);

      var output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);
      var load = (GsaBeamLoad)output.Value;
      Assert.Equal(-5000, load.ApiLoad.Value(0));
      Assert.Equal(direction, load.ApiLoad.Direction.ToString());
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ProjectedTest(bool project) {
      var comp = new CreateBeamLoad();
      ComponentTestHelper.SetInput(comp, "All", 1);
      ComponentTestHelper.SetInput(comp, project, 5);
      ComponentTestHelper.SetInput(comp, -5, 6);

      var output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);
      var load = (GsaBeamLoad)output.Value;
      Assert.Equal(-5000, load.ApiLoad.Value(0));
      Assert.Equal(project, load.ApiLoad.IsProjected);
    }

    [Fact]
    public void EntityListTypeErrorTest() {
      var comp = new CreateBeamLoad();
      var list = new GsaList("test", "1 2 3", GsaAPI.EntityType.Node);
      ComponentTestHelper.SetInput(comp, new GsaListGoo(list), 1);
      ComponentTestHelper.SetInput(comp, -5, 6);

      var output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);
      comp.Params.Output[0].ExpireSolution(true);
      comp.Params.Output[0].CollectData();
      Assert.Single(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void CreateElement1dLoadTest() {
      var comp = new CreateBeamLoad();
      GH_OasysComponent element1dComp = CreateElement1dTests.ComponentMother();
      var element1dGoo = (GsaElement1dGoo)ComponentTestHelper.GetOutput(element1dComp);

      ComponentTestHelper.SetInput(comp, element1dGoo, 1);
      ComponentTestHelper.SetInput(comp, -5, 6);

      var output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);
      var load = (GsaBeamLoad)output.Value;
      Assert.Equal(-5000, load.ApiLoad.Value(0));
      Assert.Equal(ReferenceType.Element, load.ReferenceType);
      Assert.Equal(GsaAPI.EntityType.Element, load.ApiLoad.EntityType);
    }

    [Fact]
    public void CreateMember1dLoadTest() {
      var comp = new CreateBeamLoad();
      GH_OasysComponent member1dComp = CreateMember1dTests.ComponentMother();
      var member1dGoo = (GsaMember1dGoo)ComponentTestHelper.GetOutput(member1dComp);

      ComponentTestHelper.SetInput(comp, member1dGoo, 1);
      ComponentTestHelper.SetInput(comp, -5, 6);

      var output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);
      var load = (GsaBeamLoad)output.Value;
      Assert.Equal(-5000, load.ApiLoad.Value(0));
      Assert.Equal(ReferenceType.Member, load.ReferenceType);
      Assert.Equal(GsaAPI.EntityType.Member, load.ApiLoad.EntityType);
    }
  }
}
