using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Components.Properties;
using GsaGHTests.Helpers;

using OasysGH.Components;

using Rhino.Geometry;

using Xunit;

namespace GsaGHTests.Components.Geometry {
  [Collection("GrasshopperFixture collection")]
  public class CreateMember3dTests {
    private readonly GsaMember3dGoo _gsaMember3dGoo;

    public CreateMember3dTests() {
      GH_OasysComponent comp = ComponentMother();

      _gsaMember3dGoo = (GsaMember3dGoo)ComponentTestHelper.GetOutput(comp);
    }

    public static GH_OasysComponent ComponentMother() {
      var comp = new Create3dMember();
      comp.CreateAttributes();

      Box box = Box.Empty;
      box.X = new Interval(0, 10);
      box.Y = new Interval(0, 10);
      box.Z = new Interval(0, 10);
      ComponentTestHelper.SetInput(comp, box, 0);
      ComponentTestHelper.SetInput(comp,
        ComponentTestHelper.GetOutput(CreateProp3dTests.ComponentMother()), 1);
      ComponentTestHelper.SetInput(comp, 0.5, 2);

      return comp;
    }

    [Fact]
    public void ComponentShouldReturnConcreteMaterialTypeAsDefault() {
      Assert.Equal(MatType.Concrete, _gsaMember3dGoo.Value.Prop3d.Material.MaterialType);
    }

    [Fact]
    public void ComponentShouldReturn05MeshSizeValue() {
      Assert.Equal(0.5, _gsaMember3dGoo.Value.ApiMember.MeshSize);
    }

    [Fact]
    public void ComponentShouldReturnDefaultGroupValue() {
      Assert.Equal(1, _gsaMember3dGoo.Value.ApiMember.Group);
    }
  }
}
