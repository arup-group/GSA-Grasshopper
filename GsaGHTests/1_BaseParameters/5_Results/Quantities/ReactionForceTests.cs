﻿using System;

using GsaAPI;

using GsaGH.Parameters.Results;

using OasysUnits.Units;

using Xunit;

using ForceUnit = OasysUnits.Units.ForceUnit;

namespace GsaGHTests.Parameters.Results {
  [Collection("GrasshopperFixture collection")]
  public partial class ReactionForceTests {
    [Theory]
    [InlineData(1, 2, 3, 4, 5, 6)]
    [InlineData(double.NaN, double.NaN, double.NaN, 4, 5, 6)]
    [InlineData(4, 5, 6, double.NaN, double.NaN, double.NaN)]
    public void ReactionForceConstructorTest(double x, double y, double z, double xx, double yy, double zz) {
      var apiResult = new Double6(x, y, z, xx, yy, zz);
      var reaction = new ReactionForce(apiResult);
      ForceUnit fUnit = ForceUnit.Newton;
      MomentUnit mUnit = MomentUnit.NewtonMeter;
      if (double.IsNaN(x)) {
        Assert.Null(reaction.XAs(fUnit));
      } else {
        Assert.Equal(x, reaction.XAs(fUnit));
      }

      if (double.IsNaN(y)) {
        Assert.Null(reaction.YAs(fUnit));
      } else {
        Assert.Equal(y, reaction.YAs(fUnit));
      }

      if (double.IsNaN(z)) {
        Assert.Null(reaction.ZAs(fUnit));
      } else {
        Assert.Equal(z, reaction.ZAs(fUnit));
      }

      if (double.IsNaN(x)) {
        Assert.Null(reaction.XyzAs(fUnit));
      } else {
        double p = Math.Sqrt((x * x) + (y * y) + (z * z));
        Assert.Equal(p, reaction.XyzAs(fUnit));
      }

      if (double.IsNaN(xx)) {
        Assert.Null(reaction.XxAs(mUnit));
      } else {
        Assert.Equal(xx, reaction.XxAs(mUnit));
      }

      if (double.IsNaN(yy)) {
        Assert.Null(reaction.YyAs(mUnit));
      } else {
        Assert.Equal(yy, reaction.YyAs(mUnit));
      }

      if (double.IsNaN(zz)) {
        Assert.Null(reaction.ZzAs(mUnit));
      } else {
        Assert.Equal(zz, reaction.ZzAs(mUnit));
      }

      if (double.IsNaN(xx)) {
        Assert.Null(reaction.XxyyzzAs(mUnit));
      } else {
        double p = Math.Sqrt((xx * xx) + (yy * yy) + (zz * zz));
        Assert.Equal(p, reaction.XxyyzzAs(mUnit));
      }
    }
  }
}
