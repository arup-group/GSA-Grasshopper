﻿using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Components.Properties;
using GsaGHTests.Helpers;
using OasysGH.Components;
using Rhino.Geometry;
using Xunit;
using static GsaGH.Parameters.GsaMaterial;

namespace GsaGHTests.Components.Geometry {
  [Collection("GrasshopperFixture collection")]
  public class CreateMember3dTests {

    public static GH_OasysComponent ComponentMother() {
      var comp = new CreateMember3d();
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
    public void CreateComponentTest() {
      GH_OasysComponent comp = ComponentMother();

      var output = (GsaMember3dGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(MatType.Concrete, output.Value.Prop3d.Material.MaterialType);
      Assert.Equal(0.5, output.Value.MeshSize);
    }
  }
}
