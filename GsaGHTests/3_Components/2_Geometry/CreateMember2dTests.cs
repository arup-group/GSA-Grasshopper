using GsaAPI;
using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Components.Properties;
using GsaGHTests.Helpers;
using OasysGH.Components;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;
using Xunit;

namespace GsaGHTests.Components.Geometry {
  [Collection("GrasshopperFixture collection")]
  public class CreateMember2dTests {

    public static GH_OasysComponent ComponentMother() {
      var comp = new CreateMember2d();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(comp,
        Brep.CreateFromCornerPoints(new Point3d(0, 0, 0),
          new Point3d(10, 0, 0),
          new Point3d(10, 10, 0),
          new Point3d(0, 10, 0),
          1),
        0);
      ComponentTestHelper.SetInput(comp,
        ComponentTestHelper.GetOutput(CreateProp2dTests.ComponentMother(false)),
        3);
      ComponentTestHelper.SetInput(comp, 0.5, 4);

      return comp;
    }

    [Fact]
    public void CreateComponentTest() {
      GH_OasysComponent comp = ComponentMother();

      var output = (GsaMember2dGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(100, output.Value.Brep.GetArea());
      Assert.Equal(Property2D_Type.PLATE, output.Value.Property.Type);
      Assert.Equal(new Length(14, LengthUnit.Inch), output.Value.Property.Thickness);
      Assert.Equal(0.5, output.Value.MeshSize);
    }
  }
}
