using System;

using GsaAPI;

using GsaGH.Parameters.Results;

using OasysUnits;

using Xunit;

namespace GsaGHTests.Parameters.Results {
  [Collection("GrasshopperFixture collection")]
  public partial class DisplacementQuantityTests {
    [Fact]
    public void DisplacementQuantityConstructorTest() {
      var apiResult = new Double6(1.1, 2.2, 3.3, 4.4, 5.5, 6.6);
      var displacementQuantity = new Displacement(apiResult);
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
    public void DisplacementQuantityAngleNanTest() {
      var apiResult = new Double6(1.1, 2.2, 3.3, double.NaN, double.NaN, double.NaN);
      var displacementQuantity = new Displacement(apiResult);

      Assert.Equal(Angle.Zero, displacementQuantity.Xx);
      Assert.Equal(Angle.Zero, displacementQuantity.Yy);
      Assert.Equal(Angle.Zero, displacementQuantity.Zz);
      Assert.Equal(Angle.Zero, displacementQuantity.Xxyyzz);
    }

    [Fact]
    public void DisplacementQuantityAngleInfinityTest() {
      var apiResult = new Double6(1.1, 2.2, 3.3, double.PositiveInfinity, double.NegativeInfinity, double.NaN);
      var displacementQuantity = new Displacement(apiResult);

      Assert.Equal(360, displacementQuantity.Xx.Degrees);
      Assert.Equal(-360, displacementQuantity.Yy.Degrees);
      Assert.Equal(Angle.Zero, displacementQuantity.Zz);
    }
  }
}
