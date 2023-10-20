using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
  public partial class ResultUtilityTests {
    [Theory]
    [InlineData(2.0, 3.0, 4.0, LengthUnit.Meter)]
    public void PythagoreanQuadrupleTest(double a, double b, double c, Enum unit) {
      IQuantity x = Quantity.From(a, unit);
      IQuantity y = Quantity.From(b, unit);
      IQuantity z = Quantity.From(c, unit);
      IQuantity quantity = ResultUtility.PythagoreanQuadruple(x, y, z);

      double pyth = Math.Sqrt((a * a) + (b * b) + (c * c));
      IQuantity expected = Quantity.From(pyth, unit);

      Assert.Equal(expected.Value, quantity.Value);
      Assert.Equal(expected.Unit, quantity.Unit);
      Assert.Equal(expected, quantity);
    }
  }
}
