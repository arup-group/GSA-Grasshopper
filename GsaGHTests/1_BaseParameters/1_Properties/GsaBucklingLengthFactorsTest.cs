using System.Collections.Generic;
using GsaAPI;
using GsaGH.Helpers.Export;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysUnits;
using Rhino.Geometry;
using Xunit;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaBucklingLengthFactorsTest {

    [Fact]
    public void AssembleWitMemberTest() {
      var m1d = new GsaMember1d(new LineCurve(new Point3d(0, 0, 0), new Point3d(10, 0, 0))) {
        ApiMember = {
          MomentAmplificationFactorStrongAxis = 1.5,
          MomentAmplificationFactorWeakAxis = 2.5,
          EquivalentUniformMomentFactor = 0.75,
        },
      };

      var assembled = new GsaModel {
        Model = AssembleModel.Assemble(null, null, null, null, null, null, 
        new List<GsaMember1d>() { m1d }, null, null, null, null, null, null, null, null, null, 
        null, LengthUnit.Meter, Length.Zero, false, null),
      };

      Member assembledMem1d = assembled.Model.Members()[1];

      Assert.Equal(1.5, assembledMem1d.MomentAmplificationFactorStrongAxis);
      Assert.Equal(2.5, assembledMem1d.MomentAmplificationFactorWeakAxis);
      Assert.Equal(0.75, assembledMem1d.EquivalentUniformMomentFactor);
    }

    [Theory]
    [InlineData(1, 2, 3)]
    public void ConstructorTest1(double factor1, double factor2, double factor3) {
      var factors = new GsaBucklingFactors(factor1, factor2, factor3);

      Assert.Equal(factor1, factors.MomentAmplificationFactorStrongAxis);
      Assert.Equal(factor2, factors.MomentAmplificationFactorWeakAxis);
      Assert.Equal(factor3, factors.EquivalentUniformMomentFactor);
    }

    [Theory]
    [InlineData(1, 2, 3)]
    public void ConstructorTest2(double factor1, double factor2, double factor3) {
      var member = new GsaMember1d {
        ApiMember = {
          MomentAmplificationFactorStrongAxis = factor1,
          MomentAmplificationFactorWeakAxis = factor2,
          EquivalentUniformMomentFactor = factor3,
        },
      };

      var factors = new GsaBucklingFactors(member);

      Assert.Equal(factor1, factors.MomentAmplificationFactorStrongAxis);
      Assert.Equal(factor2, factors.MomentAmplificationFactorWeakAxis);
      Assert.Equal(factor3, factors.EquivalentUniformMomentFactor);
    }

    [Theory]
    [InlineData(1, 2, 3)]
    public void ConstructorTest3(double factor1, double factor2, double factor3) {
      var member = new GsaMember1d {
        PolyCurve = new PolyCurve(),
        ApiMember = {
          MomentAmplificationFactorStrongAxis = factor1,
          MomentAmplificationFactorWeakAxis = factor2,
          EquivalentUniformMomentFactor = factor3,
        },
      };

      var factors = new GsaBucklingFactors(member);

      Assert.Equal(factor1, factors.MomentAmplificationFactorStrongAxis);
      Assert.Equal(factor2, factors.MomentAmplificationFactorWeakAxis);
      Assert.Equal(factor3, factors.EquivalentUniformMomentFactor);
    }

    [Theory]
    [InlineData(1, 2, 3)]
    public void DuplicateTest(double factor1, double factor2, double factor3) {
      var member = new GsaMember1d {
        ApiMember = {
          MomentAmplificationFactorStrongAxis = factor1,
          MomentAmplificationFactorWeakAxis = factor2,
          EquivalentUniformMomentFactor = factor3,
        },
      };

      var original = new GsaBucklingFactors(member);

      GsaBucklingFactors duplicate = original.Duplicate();

      Duplicates.AreEqual(original, duplicate);

      duplicate.MomentAmplificationFactorStrongAxis = 10;
      duplicate.MomentAmplificationFactorWeakAxis = 20;
      duplicate.EquivalentUniformMomentFactor = 30;

      Assert.Equal(factor1, original.MomentAmplificationFactorStrongAxis);
      Assert.Equal(factor2, original.MomentAmplificationFactorWeakAxis);
      Assert.Equal(factor3, original.EquivalentUniformMomentFactor);
    }

    [Fact]
    public void EmptyConstructorTest() {
      var factors = new GsaBucklingFactors();

      Assert.Null(factors.MomentAmplificationFactorStrongAxis);
      Assert.Null(factors.MomentAmplificationFactorWeakAxis);
      Assert.Null(factors.EquivalentUniformMomentFactor);
    }
  }
}
