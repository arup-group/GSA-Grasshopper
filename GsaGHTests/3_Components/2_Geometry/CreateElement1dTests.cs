﻿using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysGH.Components;
using Rhino.Geometry;
using Xunit;

namespace GsaGHTests.Components.Geometry {
  [Collection("GrasshopperFixture collection")]
  public class CreateElement1dTests {

    public static GH_OasysComponent ComponentMother() {
      var comp = new CreateElement1d();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(
        comp, new LineCurve(new Point3d(0, -1, 0), new Point3d(7, 3, 1)), 0);
      ComponentTestHelper.SetInput(comp, "STD CH(ft) 1 2 3 4", 1);

      return comp;
    }

    [Fact]
    public void CreateComponentTest() {
      GH_OasysComponent comp = ComponentMother();

      var output = (GsaElement1dGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(0, output.Value.Line.PointAtStart.X);
      Assert.Equal(-1, output.Value.Line.PointAtStart.Y);
      Assert.Equal(0, output.Value.Line.PointAtStart.Z);
      Assert.Equal(7, output.Value.Line.PointAtEnd.X);
      Assert.Equal(3, output.Value.Line.PointAtEnd.Y);
      Assert.Equal(1, output.Value.Line.PointAtEnd.Z);
      Assert.Equal("STD CH(ft) 1 2 3 4", output.Value.Section.Profile);
    }
  }
}