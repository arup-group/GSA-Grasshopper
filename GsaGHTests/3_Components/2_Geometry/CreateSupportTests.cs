using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Components.Properties;
using GsaGHTests.Helpers;
using OasysGH.Components;
using Rhino.Geometry;
using Xunit;

namespace GsaGHTests.Components.Geometry {
  [Collection("GrasshopperFixture collection")]
  public class CreateSupportTests {
    public static GH_OasysComponent ComponentMother() {
      var comp = new CreateSupport();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(comp, new Point3d(0, -1, 0), 0);

      return comp;
    }

    [Fact]
    public void CreateComponentFromPtPlaneAndBool6Test() {
      GH_OasysComponent comp = ComponentMother();
      ComponentTestHelper.SetInput(comp, Plane.WorldYZ, 1);
      ComponentTestHelper.SetInput(comp,
        ComponentTestHelper.GetOutput(CreateBool6Tests.ComponentMother()),
        2);

      var output = (GsaNodeGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(0, output.Value.ApiNode.Position.X);
      Assert.Equal(-1, output.Value.ApiNode.Position.Y);
      Assert.Equal(0, output.Value.ApiNode.Position.Z);
      Assert.Equal(0, output.Value.Point.X);
      Assert.Equal(-1, output.Value.Point.Y);
      Assert.Equal(0, output.Value.Point.Z);
      Assert.True(output.Value.IsSupport);
      Duplicates.AreEqual(new GsaBool6(true,
          true,
          true,
          true,
          true,
          true),
        output.Value.Restraint);
      Assert.Equal(0, output.Value.Id);
      Duplicates.AreEqual(Plane.WorldYZ, output.Value.LocalAxis);
      Assert.Equal(0, output.Value.DamperProperty);
      Assert.Equal(0, output.Value.MassProperty);
      Assert.Equal(0, output.Value.SpringProperty);
    }

    [Fact]
    public void CreateComponentFromPtTest() {
      GH_OasysComponent comp = ComponentMother();

      var output = (GsaNodeGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(0, output.Value.ApiNode.Position.X);
      Assert.Equal(-1, output.Value.ApiNode.Position.Y);
      Assert.Equal(0, output.Value.ApiNode.Position.Z);
      Assert.Equal(0, output.Value.Point.X);
      Assert.Equal(-1, output.Value.Point.Y);
      Assert.Equal(0, output.Value.Point.Z);
      Assert.False(output.Value.IsSupport);
      Duplicates.AreEqual(new GsaBool6(), output.Value.Restraint);
      Assert.Equal(0, output.Value.Id);
      Duplicates.AreEqual(Plane.WorldXY, output.Value.LocalAxis);
      Assert.Equal(0, output.Value.DamperProperty);
      Assert.Equal(0, output.Value.MassProperty);
      Assert.Equal(0, output.Value.SpringProperty);
    }
  }
}
