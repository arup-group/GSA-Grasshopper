﻿using GsaGH.Components;
using GsaGH.Parameters;
using GsaGH.Parameters.Enums;
using GsaGHTests.Helpers;
using Rhino.Geometry;
using Xunit;
using static GsaAPI.GridLineLoad;

namespace GsaGHTests.Components.Loads {
  [Collection("GrasshopperFixture collection")]
  public class CreateGridLineLoadTests {
    [Fact]
    public void CreateGridLineLoadTest() {
      var comp = new CreateGridLineLoad();
      ComponentTestHelper.SetInput(comp, 7, 0);
      ComponentTestHelper.SetInput(comp, new LineCurve(new Point3d(10, 5, -1), new Point3d(20, 6, -1)), 1);
      ComponentTestHelper.SetInput(comp, "myGridLineLoad", 6);
      ComponentTestHelper.SetInput(comp, -5, 7);
      ComponentTestHelper.SetInput(comp, -3, 8);

      var output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);
      var load = (GsaGridLineLoad)output.Value;
      Assert.Equal(7, load.LoadCase.Id);
      Assert.Equal("(10,5) (20,6)(m)", load.ApiLoad.PolyLineDefinition);
      Assert.Equal(0, load.ApiLoad.PolyLineReference);
      Assert.Equal("myGridLineLoad", load.ApiLoad.Name);
      Assert.Equal(-5000, load.ApiLoad.ValueAtStart);
      Assert.Equal(-3000, load.ApiLoad.ValueAtEnd);
      Assert.Equal(ReferenceType.None, load.ReferenceType);
      Assert.Equal(PolyLineType.EXPLICIT_POLYLINE, load.ApiLoad.Type);
    }



  }
}
