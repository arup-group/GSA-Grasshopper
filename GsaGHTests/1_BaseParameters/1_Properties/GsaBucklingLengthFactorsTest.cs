using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;
using System.Collections.Generic;
using System.Numerics;
using Xunit;
using static Rhino.UI.Controls.CollapsibleSectionImpl;

namespace GsaGHTests.Parameters
{
  [Collection("GrasshopperFixture collection")]
  public class GsaBucklingLengthFactorsTest
  {
    [Fact]
    public void EmptyConstructorTest()
    {
      // Act
      GsaBucklingLengthFactors factors = new GsaBucklingLengthFactors();

      // Assert
      Assert.Null(factors.MomentAmplificationFactorStrongAxis);
      Assert.Null(factors.MomentAmplificationFactorWeakAxis);
      Assert.Null(factors.LateralTorsionalBucklingFactor);
      Assert.False(factors.LengthIsSet);
    }

    [Theory]
    [InlineData(1, 2, 3)]
    public void ConstructorTest1(double factor1, double factor2, double factor3)
    {
      // Act
      GsaBucklingLengthFactors factors = new GsaBucklingLengthFactors(factor1, factor2, factor3);

      // Assert
      Assert.Equal(factor1, factors.MomentAmplificationFactorStrongAxis);
      Assert.Equal(factor2, factors.MomentAmplificationFactorWeakAxis);
      Assert.Equal(factor3, factors.LateralTorsionalBucklingFactor);
      Assert.False(factors.LengthIsSet);
    }

    [Theory]
    [InlineData(1, 2, 3)]
    public void ConstructorTest2(double factor1, double factor2, double factor3)
    {
      // Act
      GsaMember1d member = new GsaMember1d();
      member.ApiMember.MomentAmplificationFactorStrongAxis = factor1;
      member.ApiMember.MomentAmplificationFactorWeakAxis = factor2;
      member.ApiMember.LateralTorsionalBucklingFactor = factor3;

      GsaBucklingLengthFactors factors = new GsaBucklingLengthFactors(member);

      // Assert
      Assert.Equal(factor1, factors.MomentAmplificationFactorStrongAxis);
      Assert.Equal(factor2, factors.MomentAmplificationFactorWeakAxis);
      Assert.Equal(factor3, factors.LateralTorsionalBucklingFactor);
      Assert.False(factors.LengthIsSet);
    }

    [Theory]
    [InlineData(1, 2, 3, LengthUnit.Meter)]
    public void ConstructorTest3(double factor1, double factor2, double factor3, LengthUnit unit)
    {
      // Act
      GsaMember1d member = new GsaMember1d();
      member.PolyCurve = new PolyCurve();
      member.ApiMember.MomentAmplificationFactorStrongAxis = factor1;
      member.ApiMember.MomentAmplificationFactorWeakAxis = factor2;
      member.ApiMember.LateralTorsionalBucklingFactor = factor3;

      GsaBucklingLengthFactors factors = new GsaBucklingLengthFactors(member, unit);

      // Assert
      Assert.Equal(factor1, factors.MomentAmplificationFactorStrongAxis);
      Assert.Equal(factor2, factors.MomentAmplificationFactorWeakAxis);
      Assert.Equal(factor3, factors.LateralTorsionalBucklingFactor);
      Assert.True(factors.LengthIsSet);
      Assert.Equal(0, factors.Length.As(unit));
    }

    [Theory]
    [InlineData(1, 2, 3)]
    public void DuplicateTest(double factor1, double factor2, double factor3)
    {
      // Arrange
      GsaMember1d member = new GsaMember1d();
      member.ApiMember.MomentAmplificationFactorStrongAxis = factor1;
      member.ApiMember.MomentAmplificationFactorWeakAxis = factor2;
      member.ApiMember.LateralTorsionalBucklingFactor = factor3;

      GsaBucklingLengthFactors original = new GsaBucklingLengthFactors(member);

      // Act
      GsaBucklingLengthFactors duplicate = original.Duplicate();

      // Assert
      Duplicates.AreEqual(original, duplicate);

      // make some changes to duplicate
      duplicate.MomentAmplificationFactorStrongAxis = 10;
      duplicate.MomentAmplificationFactorWeakAxis = 20;
      duplicate.LateralTorsionalBucklingFactor = 30;

      Assert.Equal(factor1, original.MomentAmplificationFactorStrongAxis);
      Assert.Equal(factor2, original.MomentAmplificationFactorWeakAxis);
      Assert.Equal(factor3, original.LateralTorsionalBucklingFactor);
      Assert.False(original.LengthIsSet);
    }

    [Fact]
    public void AssembleWitMemberTest()
    {
      GsaMember1d m1d = new GsaMember1d(new LineCurve(new Point3d(0, 0, 0), new Point3d(10, 0, 0)));
      m1d.ApiMember.MomentAmplificationFactorStrongAxis = 1.5;
      m1d.ApiMember.MomentAmplificationFactorWeakAxis = 2.5;
      m1d.ApiMember.LateralTorsionalBucklingFactor = 0.75;

      GsaBucklingLengthFactors originalFactors = new GsaBucklingLengthFactors(m1d);

      GsaModel assembled = new GsaModel();
      assembled.Model = GsaGH.Helpers.Export.AssembleModel.Assemble(null, null, null, null, null, new List<GsaMember1d>() { m1d }, null, null, null, null, null, null, null, null, null, LengthUnit.Meter, Length.Zero, false, null);

      GsaAPI.Member assembledMem1d = assembled.Model.Members()[1];

      Assert.Equal(1.5, assembledMem1d.MomentAmplificationFactorStrongAxis);
      Assert.Equal(2.5, assembledMem1d.MomentAmplificationFactorWeakAxis);
      Assert.Equal(0.75, assembledMem1d.LateralTorsionalBucklingFactor);
    }
  }
}
