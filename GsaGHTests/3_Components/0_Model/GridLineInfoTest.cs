﻿using System;
using System.Net;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Components.Properties;
using GsaGHTests.Helpers;
using OasysGH.Components;
using OasysUnits;
using Rhino.Geometry;
using Xunit;

namespace GsaGHTests.Model {
  [Collection("GrasshopperFixture collection")]
  public class GridLineInfoTest {

    public static GH_OasysComponent GridLineComponentMother() {
      var comp = new GridLineInfo();
      comp.CreateAttributes();

      var gridLineGoo = (GsaGridLineGoo)ComponentTestHelper.GetOutput(CreateGridLineTest.GridLineComponentMother(), 0);
      ComponentTestHelper.SetInput(comp, gridLineGoo, 0);

      return comp;
    }

    public static GH_OasysComponent GridArcComponentMother() {
      var comp = new GridLineInfo();
      comp.CreateAttributes();

      var gridLineGoo = (GsaGridLineGoo)ComponentTestHelper.GetOutput(CreateGridLineTest.GridArcComponentMother(), 0);
      ComponentTestHelper.SetInput(comp, gridLineGoo, 0);

      return comp;
    }

    [Fact]
    public void GridLineInfoComponent() {
      GH_OasysComponent comp = GridLineComponentMother();

      var label = (GH_String)ComponentTestHelper.GetOutput(comp, 0);
      var point = (GH_Point)ComponentTestHelper.GetOutput(comp, 1);
      var length = (GH_Number)ComponentTestHelper.GetOutput(comp, 2);
      var shape = (GH_String)ComponentTestHelper.GetOutput(comp, 3);
      var theta1 = (GH_Number)ComponentTestHelper.GetOutput(comp, 4);
      var theta2 = (GH_Number)ComponentTestHelper.GetOutput(comp, 5);

      Assert.Equal("Line", label.Value);
      Assert.Equal(0, point.Value.X);
      Assert.Equal(0, point.Value.Y);
      Assert.Equal(0, point.Value.Z);
      Assert.Equal(Math.Sqrt(2), length.Value);
      Assert.Equal("Line", shape.Value);
      Assert.Equal(45, theta1.Value, 0.0000001);
      Assert.Equal(0, theta2.Value);
    }

    [Fact]
    public void GridArcInfoComponent() {
      GH_OasysComponent comp = GridArcComponentMother();

      var label = (GH_String)ComponentTestHelper.GetOutput(comp, 0);
      var point = (GH_Point)ComponentTestHelper.GetOutput(comp, 1);
      var length = (GH_Number)ComponentTestHelper.GetOutput(comp, 2);
      var shape = (GH_String)ComponentTestHelper.GetOutput(comp, 3);
      var theta1 = (GH_Number)ComponentTestHelper.GetOutput(comp, 4);
      var theta2 = (GH_Number)ComponentTestHelper.GetOutput(comp, 5);

      Assert.Equal("Arc", label.Value);
      Assert.Equal(1, point.Value.X);
      Assert.Equal(0, point.Value.Y);
      Assert.Equal(0, point.Value.Z);
      Assert.Equal(Math.PI, length.Value);
      Assert.Equal("Arc", shape.Value);
      Assert.Equal(-180, theta1.Value, 0.0000001);
      Assert.Equal(0, theta2.Value, 0.0000001);
    }
  }
}
