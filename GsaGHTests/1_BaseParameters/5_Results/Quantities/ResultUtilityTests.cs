using System;
using System.Collections.Generic;

using GsaGH.Parameters.Results;

using OasysUnits;
using OasysUnits.Units;

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
      IQuantity quantity = QuantityUtility.PythagoreanQuadruple(x, y, z);

      double pyth = Math.Sqrt((a * a) + (b * b) + (c * c));
      IQuantity expected = Quantity.From(pyth, unit);

      Assert.Equal(expected.Value, quantity.Value);
      Assert.Equal(expected.Unit, quantity.Unit);
      Assert.Equal(expected, quantity);
    }

    [Theory]
    [InlineData(0, 1, -2, EnvelopeMethod.Absolute)]
    [InlineData(0, -7, 2, EnvelopeMethod.Maximum)]
    [InlineData(0, 1, -2, EnvelopeMethod.Minimum)]
    [InlineData(0, 2, -7, EnvelopeMethod.SignedAbsolute)]
    public void RatioEnvelopeTest(double value1, double value2, double value3, EnvelopeMethod method) {
      // Assemble
      IList<Ratio?> subset = new List<Ratio?>() {
        null,
        new Ratio(value1, RatioUnit.DecimalFraction),
        new Ratio(value2, RatioUnit.DecimalFraction),
        new Ratio(value3, RatioUnit.DecimalFraction),
        null
      };

      // Act
      Ratio envelope = ResultsUtility.Envelope(subset, method);

      // Assert
      double expected = 0;
      if (method == EnvelopeMethod.Absolute || method == EnvelopeMethod.Maximum) {
        expected = Math.Abs(value3);
      } else {
        expected = value3;
      }
      Assert.Equal(expected, envelope.Value);
    }
  }
}
