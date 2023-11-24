﻿using System.Collections.Generic;
using GsaGH.Helpers.GsaApi;
using GsaGH.Parameters.Results;
using GsaGHTests._1_BaseParameters._5_Results.Collections.RegressionValues;
using GsaGHTests.Helper;
using OasysUnits;
using OasysUnits.Units;
using Xunit;

namespace GsaGHTests.Parameters.Results {
  [Collection("GrasshopperFixture collection")]
  public class TotalLoadsAndReactionsTests {

    [Fact]
    public void NullEffectiveInnertiaTest() {
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 3);
      List<double> expected = TotalLoadsAndReactionsA3.EffectiveInertia; //null here!

      IEffectiveInertia actual = result.GlobalResults.EffectiveInertia;

      Assert.Null(expected);
      Assert.Null(actual);
    }

    [Fact]
    public void EffectiveMassTest() {
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 3);
      List<double> expected = TotalLoadsAndReactionsA3.EffectiveMass;

      IEffectiveMass actual = result.GlobalResults.EffectiveMass;

      Assert.Equal(expected[0], RoundTo4SigDigits(actual.X.ToUnit(MassUnit.Tonne)));
      Assert.Equal(expected[1], RoundTo4SigDigits(actual.Y.ToUnit(MassUnit.Tonne)));
      Assert.Equal(expected[2], RoundTo4SigDigits(actual.Z.ToUnit(MassUnit.Tonne)));
    }

    [Fact]
    public void EigenValueTest() {
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 3);
      double? expected = TotalLoadsAndReactionsA3.EigenValue;

      double? actual = result.GlobalResults.Eigenvalue;

      Assert.Equal(expected, actual);
    }

    [Fact]
    public void FrequencyValueTest() {
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 3);
      double expected = TotalLoadsAndReactionsA3.Frequency;

      Frequency actual = result.GlobalResults.Frequency;

      Assert.Equal(expected, RoundTo4SigDigits(actual));
    }

    [Fact]
    public void LoadFactorValueTest() {
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 3);
      double? expected = TotalLoadsAndReactionsA3.LoadFactor;

      Ratio actual = result.GlobalResults.LoadFactor;

      Assert.Equal(expected, RoundTo4SigDigits(actual));
    }

    [Fact]
    public void ModalGeometricStiffnessValueTest() {
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 3);
      double? expected = TotalLoadsAndReactionsA3.ModalGeometricStiffness;

      ForcePerLength actual = result.GlobalResults.ModalGeometricStiffness;

      Assert.Equal(expected, RoundTo4SigDigits(actual));
    }

    [Fact]
    public void ModalStiffnessValueTest() {
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 3);
      double expected = TotalLoadsAndReactionsA3.ModalStiffness;

      ForcePerLength actual
        = result.GlobalResults.ModalStiffness.ToUnit(ForcePerLengthUnit.KilonewtonPerMeter);

      Assert.Equal(expected, RoundTo4SigDigits(actual));
    }

    [Fact]
    public void ModalMassValueTest() {
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 3);
      double expected = TotalLoadsAndReactionsA3.ModalMass;

      Mass actual = result.GlobalResults.ModalMass;

      Assert.Equal(expected, RoundTo4SigDigits(actual.ToUnit(MassUnit.Tonne)));
    }

    [Fact]
    public void ModeValueTest() {
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 3);
      int expected = TotalLoadsAndReactionsA3.Mode;

      int? actual = result.GlobalResults.Mode;

      Assert.Equal(expected, actual);
    }

    [Fact]
    public void TotalLoadTest() {
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 3);
      List<double> expected = TotalLoadsAndReactionsA3.TotalLoad;

      IInternalForce actual = result.GlobalResults.TotalLoad;

      Assert.Equal(expected[0], RoundTo4SigDigits(actual.X.ToUnit(ForceUnit.Kilonewton)));
      Assert.Equal(expected[1], RoundTo4SigDigits(actual.Y.ToUnit(ForceUnit.Kilonewton)));
      Assert.Equal(expected[2], RoundTo4SigDigits(actual.Z.ToUnit(ForceUnit.Kilonewton)));
      Assert.Equal(expected[3], RoundTo4SigDigits(actual.Xyz.ToUnit(ForceUnit.Kilonewton)));
      Assert.Equal(expected[4], RoundTo4SigDigits(actual.Xx.ToUnit(MomentUnit.KilonewtonMeter)));
      Assert.Equal(expected[5], RoundTo4SigDigits(actual.Yy.ToUnit(MomentUnit.KilonewtonMeter)));
      Assert.Equal(expected[6], RoundTo4SigDigits(actual.Zz.ToUnit(MomentUnit.KilonewtonMeter)));
      Assert.Equal(expected[7], RoundTo4SigDigits(actual.Xxyyzz.ToUnit(MomentUnit.KilonewtonMeter)));
    }

    [Fact]
    public void TotalReactionTest() {
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 3);
      List<double> expected = TotalLoadsAndReactionsA3.TotalReaction;

      IInternalForce actual = result.GlobalResults.TotalReaction;

      Assert.Equal(expected[0], RoundTo4SigDigits(actual.X.ToUnit(ForceUnit.Kilonewton)));
      Assert.Equal(expected[1], RoundTo4SigDigits(actual.Y.ToUnit(ForceUnit.Kilonewton)));
      Assert.Equal(expected[2], RoundTo4SigDigits(actual.Z.ToUnit(ForceUnit.Kilonewton)));
      Assert.Equal(expected[3], RoundTo4SigDigits(actual.Xyz.ToUnit(ForceUnit.Kilonewton)));
      Assert.Equal(expected[4], RoundTo4SigDigits(actual.Xx.ToUnit(MomentUnit.KilonewtonMeter)));
      Assert.Equal(expected[5], RoundTo4SigDigits(actual.Yy.ToUnit(MomentUnit.KilonewtonMeter)));
      Assert.Equal(expected[6], RoundTo4SigDigits(actual.Zz.ToUnit(MomentUnit.KilonewtonMeter)));
      Assert.Equal(expected[7], RoundTo4SigDigits(actual.Xxyyzz.ToUnit(MomentUnit.KilonewtonMeter)));
    }

    private double RoundTo4SigDigits(IQuantity quantity) {
      return ResultHelper.RoundToSignificantDigits(quantity.Value, 4);
    }
  }
}