using GsaGH.Components;

using GsaGH.Parameters;

using GsaGH.Parameters.Enums;


using GsaGHTests.Components.Geometry;

using GsaGHTests.Helpers;


using OasysGH.Components;


using Rhino.Geometry;


using GsaGH.Helpers;

using Xunit;
namespace GsaGHTests.Components.Loads {
  [Collection("GrasshopperFixture collection")]
  public class CreateGravityLoadTests {
    [Fact]
    public void CreateLoadTest() {
      var comp = new CreateGravityLoad();
      ComponentTestHelper.SetInput(comp, 7, 0);
      ComponentTestHelper.SetInput(comp, "All", 1);
      ComponentTestHelper.SetInput(comp, "myGravityLoad", 2);
      ComponentTestHelper.SetInput(comp, new Vector3d(0, 0, -2), 3);

      var output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);
      var load = (GsaGravityLoad)output.Value;
      Assert.Equal(7, load.LoadCase.Id);
      Assert.Equal("myGravityLoad", load.ApiLoad.Name);
      Assert.Equal(0, load.ApiLoad.Factor.X, DoubleComparer.Default);
      Assert.Equal(0, load.ApiLoad.Factor.Y, DoubleComparer.Default);
      Assert.Equal(-2, load.ApiLoad.Factor.Z, DoubleComparer.Default);
      Assert.Equal(ReferenceType.None, load.ReferenceType);
    }

    [Fact]
    public void CreateMember1dLoadTest() {
      var comp = new CreateGravityLoad();
      GH_OasysComponent member1dComp = CreateMember1dTests.ComponentMother();
      var member1dGoo = (GsaMember1dGoo)ComponentTestHelper.GetOutput(member1dComp);

      ComponentTestHelper.SetInput(comp, 7, 0);
      ComponentTestHelper.SetInput(comp, member1dGoo, 1);

      var output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);
      var load = (GsaGravityLoad)output.Value;
      Assert.Equal(ReferenceType.Member, load.ReferenceType);
      Assert.Equal(GsaAPI.EntityType.Member, load.ApiLoad.EntityType);
    }

    [Fact]
    public void CreateMember2dLoadTest() {
      var comp = new CreateGravityLoad();
      GH_OasysComponent member2dComp = CreateMember2dTests.ComponentMother();
      var member2dGoo = (GsaMember2dGoo)ComponentTestHelper.GetOutput(member2dComp);

      ComponentTestHelper.SetInput(comp, 7, 0);
      ComponentTestHelper.SetInput(comp, member2dGoo, 1);

      var output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);
      var load = (GsaGravityLoad)output.Value;
      Assert.Equal(7, load.LoadCase.Id);
      Assert.Equal(ReferenceType.Member, load.ReferenceType);
      Assert.Equal(GsaAPI.EntityType.Member, load.ApiLoad.EntityType);
    }

    [Fact]
    public void EntityListTypeErrorTest() {
      var comp = new CreateGravityLoad();
      var list = new GsaList("test", "1 2 3", GsaAPI.EntityType.Node);
      ComponentTestHelper.SetInput(comp, new GsaListGoo(list), 1);
      ComponentTestHelper.SetInput(comp, -5, 3);

      var output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);
      comp.Params.Output[0].ExpireSolution(true);
      comp.Params.Output[0].CollectData();
      Assert.Single(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error));
    }
  }
}
