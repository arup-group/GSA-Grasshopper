﻿using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Components.Properties;
using GsaGHTests.Helpers;
using OasysGH.Components;
using Rhino.Geometry;
using System.Collections.Generic;
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
        ComponentTestHelper.GetOutput(CreateBool6Tests.ComponentMother()), 2);

      var output = (GsaNodeGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(0, output.Value.ApiNode.Position.X);
      Assert.Equal(-1, output.Value.ApiNode.Position.Y);
      Assert.Equal(0, output.Value.ApiNode.Position.Z);
      Assert.Equal(0, output.Value.Point.X);
      Assert.Equal(-1, output.Value.Point.Y);
      Assert.Equal(0, output.Value.Point.Z);
      Assert.True(output.Value.IsSupport);
      Duplicates.AreEqual(new GsaBool6(true, true, true, true, true, true), output.Value.Restraint);
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

    [Theory]
    [InlineData(new bool[] { true, true, true, false, false, false })]
    [InlineData(new bool[] { false, false, false, true, true, true })]
    [InlineData(new bool[] { true, false, true, false, true, false })]
    [InlineData(new bool[] { false, true, false, true, false, true })]
    [InlineData(new bool[] { false, false, false, false, false, false })]
    [InlineData(new bool[] { true, true, true, true, true, true })]
    public void CanToggleRestraints(bool[] releases) {
      var comp = (CreateSupport)ComponentMother();
      int i = 0;
      comp.SetRestraints(releases[i++],
          releases[i++],
          releases[i++],
          releases[i++],
          releases[i++],
          releases[i++]);

      var output = (GsaNodeGoo)ComponentTestHelper.GetOutput(comp);
      i = 0;
      Assert.Equal(releases[i++], output.Value.Restraint.X);
      Assert.Equal(releases[i++], output.Value.Restraint.Y);
      Assert.Equal(releases[i++], output.Value.Restraint.Z);
      Assert.Equal(releases[i++], output.Value.Restraint.Xx);
      Assert.Equal(releases[i++], output.Value.Restraint.Yy);
      Assert.Equal(releases[i++], output.Value.Restraint.Zz);
    }
  }
}
