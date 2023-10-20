﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaAPI;
using GsaGH.Components;
using GsaGH.Helpers.GsaApi;
using GsaGH.Parameters;
using GsaGH.Parameters.Results;
using GsaGHTests.Helper;
using OasysUnits;
using Xunit;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGHTests.Parameters.Results {
  [Collection("GrasshopperFixture collection")]
  public partial class GsaDisplacementQuantityTests {
    [Fact]
    public void GsaDisplacementQuantityConstructorTest() {
      var apiResult = new Double6(1.1, 2.2, 3.3, 4.4, 5.5, 6.6);
      var displacementQuantity = new GsaDisplacementQuantity(apiResult);
      Assert.Equal(apiResult.X, displacementQuantity.X.Meters);
      Assert.Equal(apiResult.Y, displacementQuantity.Y.Meters);
      Assert.Equal(apiResult.Z, displacementQuantity.Z.Meters);

      double lPyth = Math.Sqrt((1.1 * 1.1) + (2.2 * 2.2) + (3.3 * 3.3));
      Assert.Equal(lPyth, displacementQuantity.Xyz.Meters);

      Assert.Equal(apiResult.XX, displacementQuantity.Xx.Radians);
      Assert.Equal(apiResult.YY, displacementQuantity.Yy.Radians);
      Assert.Equal(apiResult.ZZ, displacementQuantity.Zz.Radians);

      double aPyth = Math.Sqrt((4.4 * 4.4) + (5.5 * 5.5) + (6.6 * 6.6));
      Assert.Equal(aPyth, displacementQuantity.Xxyyzz.Radians);
    }

    [Fact]
    public void GsaDisplacementQuantityAngleNanTest() {
      var apiResult = new Double6(1.1, 2.2, 3.3, double.NaN, double.NaN, double.NaN);
      var displacementQuantity = new GsaDisplacementQuantity(apiResult);
      
      Assert.Equal(Angle.Zero, displacementQuantity.Xx);
      Assert.Equal(Angle.Zero, displacementQuantity.Yy);
      Assert.Equal(Angle.Zero, displacementQuantity.Zz);
      Assert.Equal(Angle.Zero, displacementQuantity.Xxyyzz);
    }

    [Fact]
    public void GsaDisplacementQuantityAngleInfinityTest() {
      var apiResult = new Double6(1.1, 2.2, 3.3, double.PositiveInfinity, double.NegativeInfinity, double.NaN);
      var displacementQuantity = new GsaDisplacementQuantity(apiResult);

      Assert.Equal(360, displacementQuantity.Xx.Degrees);
      Assert.Equal(-360, displacementQuantity.Yy.Degrees);
      Assert.Equal(Angle.Zero, displacementQuantity.Zz);
    }
  }
}
