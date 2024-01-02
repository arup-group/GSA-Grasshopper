using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaGH.Helpers.GsaApi;
using GsaGH.Helpers.Import;
using GsaGH.Parameters.Results;
using GsaGHTests.Helper;
using OasysUnits;
using OasysUnits.Units;
using Xunit;

namespace GsaGHTests.Parameters.Results {
  [Collection("GrasshopperFixture collection")]
  public class Entity1dEnvelopeTests {
    [Theory]
    [ClassData(typeof(TestDataGenerator))]
    public void Element1dDisplacementsEnvelopeTest(
      ResultVector6HelperEnum component, EnvelopeMethod envelope) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      List<double> expectedP1 = ExpectedDisplacementC4p1Values(component);
      List<double> expectedP2 = ExpectedDisplacementC4p2Values(component);
      int positionsCount = 4;

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds("24 to 30", 1);
      IEntity1dResultSubset<IDisplacement, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Element1dDisplacements.ResultSubset(elementIds, positionsCount);

      ConcurrentDictionary<int, IList<IQuantity>> values = ResultsUtility.GetResultComponent(resultSet.Subset, DisplacementSelector(component), result.SelectedPermutationIds, envelope);

      int i = 0;
      foreach (int elementId in elementIds) {
        IEnumerable<int> positions = Enumerable.Range(0, positionsCount);
        foreach (int position in positions) {
          double expected = TestsResultHelper.Envelope(expectedP1[i], expectedP2[i++], envelope);
          double actual = ResultHelper.RoundToSignificantDigits(values[elementId][position].Value, 4);
          Assert.Equal(expected, actual, 4);
        }
      }
    }

    [Theory]
    [ClassData(typeof(TestDataGenerator))]
    public void Element1dForcesAndMomentsEnvelopeTest(
      ResultVector6HelperEnum component, EnvelopeMethod envelope) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      List<double> expectedP1 = ExpectedForcesAndMomentsC4p1Values(component);
      List<double> expectedP2 = ExpectedForcesAndMomentsC4p2Values(component);
      int positionsCount = 5;

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds("2 to 6", 1);
      IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Element1dInternalForces.ResultSubset(elementIds, positionsCount);

      ConcurrentDictionary<int, IList<IQuantity>> values = ResultsUtility.GetResultComponent(resultSet.Subset, ForceSelector(component), result.SelectedPermutationIds, envelope);

      int i = 0;
      foreach (int elementId in elementIds) {
        IEnumerable<int> positions = Enumerable.Range(0, positionsCount);
        foreach (int position in positions) {
          double expected = TestsResultHelper.Envelope(expectedP1[i], expectedP2[i++], envelope);
          double actual = ResultHelper.RoundToSignificantDigits(values[elementId][position].Value, 4);
          Assert.Equal(expected, actual, 4);
        }
      }
    }

    private class TestDataGenerator : IEnumerable<object[]> {
      private readonly List<object[]> _data = GetAllComponents();

      public IEnumerator<object[]> GetEnumerator() {
        return _data.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
      }

      private static List<object[]> GetAllComponents() {
        var data = new List<object[]>();
        Array components = Enum.GetValues(typeof(ResultVector6HelperEnum));
        Array envelopes = Enum.GetValues(typeof(EnvelopeMethod));
        foreach (ResultVector6HelperEnum component in components) {
          foreach (EnvelopeMethod envelope in envelopes) {
            data.Add(new object[] {
              component, envelope
            });
          }
        }

        return data;
      }
    }

    private Func<IDisplacement, IQuantity> DisplacementSelector(ResultVector6HelperEnum component) {
      switch (component) {
        case ResultVector6HelperEnum.X: return (r) => r.X.ToUnit(LengthUnit.Millimeter);
        case ResultVector6HelperEnum.Y: return (r) => r.Y.ToUnit(LengthUnit.Millimeter);
        case ResultVector6HelperEnum.Z: return (r) => r.Z.ToUnit(LengthUnit.Millimeter);
        case ResultVector6HelperEnum.Xyz: return (r) => r.Xyz.ToUnit(LengthUnit.Millimeter);
        case ResultVector6HelperEnum.Xx: return (r) => r.Xx;
        case ResultVector6HelperEnum.Yy: return (r) => r.Yy;
        case ResultVector6HelperEnum.Zz: return (r) => r.Zz;
        case ResultVector6HelperEnum.Xxyyzz: return (r) => r.Xxyyzz;
      }

      throw new NotImplementedException();
    }

    private Func<IInternalForce, IQuantity> ForceSelector(ResultVector6HelperEnum component) {
      switch (component) {
        case ResultVector6HelperEnum.X: return (r) => r.X.ToUnit(ForceUnit.Kilonewton);
        case ResultVector6HelperEnum.Y: return (r) => r.Y.ToUnit(ForceUnit.Kilonewton);
        case ResultVector6HelperEnum.Z: return (r) => r.Z.ToUnit(ForceUnit.Kilonewton);
        case ResultVector6HelperEnum.Xyz: return (r) => r.Xyz.ToUnit(ForceUnit.Kilonewton);
        case ResultVector6HelperEnum.Xx: return (r) => r.Xx.ToUnit(MomentUnit.KilonewtonMeter);
        case ResultVector6HelperEnum.Yy: return (r) => r.Yy.ToUnit(MomentUnit.KilonewtonMeter);
        case ResultVector6HelperEnum.Zz: return (r) => r.Zz.ToUnit(MomentUnit.KilonewtonMeter); 
        case ResultVector6HelperEnum.Xxyyzz: return (r) => r.Xxyyzz.ToUnit(MomentUnit.KilonewtonMeter);
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedDisplacementC4p1Values(ResultVector6HelperEnum component) {
      switch (component) {
        case ResultVector6HelperEnum.X: return Element1dDisplacementsC4p1.XInMillimeter();
        case ResultVector6HelperEnum.Y: return Element1dDisplacementsC4p1.YInMillimeter();
        case ResultVector6HelperEnum.Z: return Element1dDisplacementsC4p1.ZInMillimeter();
        case ResultVector6HelperEnum.Xyz: return Element1dDisplacementsC4p1.XyzInMillimeter();
        case ResultVector6HelperEnum.Xx: return Element1dDisplacementsC4p1.XxInRadian();
        case ResultVector6HelperEnum.Yy: return Element1dDisplacementsC4p1.YyInRadian();
        case ResultVector6HelperEnum.Zz: return Element1dDisplacementsC4p1.ZzInRadian();
        case ResultVector6HelperEnum.Xxyyzz: return Element1dDisplacementsC4p1.XxyyzzInRadian();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedDisplacementC4p2Values(ResultVector6HelperEnum component) {
      switch (component) {
        case ResultVector6HelperEnum.X: return Element1dDisplacementsC4p2.XInMillimeter();
        case ResultVector6HelperEnum.Y: return Element1dDisplacementsC4p2.YInMillimeter();
        case ResultVector6HelperEnum.Z: return Element1dDisplacementsC4p2.ZInMillimeter();
        case ResultVector6HelperEnum.Xyz: return Element1dDisplacementsC4p2.XyzInMillimeter();
        case ResultVector6HelperEnum.Xx: return Element1dDisplacementsC4p2.XxInRadian();
        case ResultVector6HelperEnum.Yy: return Element1dDisplacementsC4p2.YyInRadian();
        case ResultVector6HelperEnum.Zz: return Element1dDisplacementsC4p2.ZzInRadian();
        case ResultVector6HelperEnum.Xxyyzz: return Element1dDisplacementsC4p2.XxyyzzInRadian();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedForcesAndMomentsC4p1Values(ResultVector6HelperEnum component) {
      switch (component) {
        case ResultVector6HelperEnum.X: return Element1dForcesAndMomentsC4p1.XInKiloNewton();
        case ResultVector6HelperEnum.Y: return Element1dForcesAndMomentsC4p1.YInKiloNewton();
        case ResultVector6HelperEnum.Z: return Element1dForcesAndMomentsC4p1.ZInKiloNewton();
        case ResultVector6HelperEnum.Xyz: return Element1dForcesAndMomentsC4p1.XyzInKiloNewton();
        case ResultVector6HelperEnum.Xx: return Element1dForcesAndMomentsC4p1.XxInKiloNewtonMeter();
        case ResultVector6HelperEnum.Yy: return Element1dForcesAndMomentsC4p1.YyInKiloNewtonMeter();
        case ResultVector6HelperEnum.Zz: return Element1dForcesAndMomentsC4p1.ZzInKiloNewtonMeter();
        case ResultVector6HelperEnum.Xxyyzz: return Element1dForcesAndMomentsC4p1.XxyyzzInKiloNewtonMeter();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedForcesAndMomentsC4p2Values(ResultVector6HelperEnum component) {
      switch (component) {
        case ResultVector6HelperEnum.X: return Element1dForcesAndMomentsC4p2.XInKiloNewton();
        case ResultVector6HelperEnum.Y: return Element1dForcesAndMomentsC4p2.YInKiloNewton();
        case ResultVector6HelperEnum.Z: return Element1dForcesAndMomentsC4p2.ZInKiloNewton();
        case ResultVector6HelperEnum.Xyz: return Element1dForcesAndMomentsC4p2.XyzInKiloNewton();
        case ResultVector6HelperEnum.Xx: return Element1dForcesAndMomentsC4p2.XxInKiloNewtonMeter();
        case ResultVector6HelperEnum.Yy: return Element1dForcesAndMomentsC4p2.YyInKiloNewtonMeter();
        case ResultVector6HelperEnum.Zz: return Element1dForcesAndMomentsC4p2.ZzInKiloNewtonMeter();
        case ResultVector6HelperEnum.Xxyyzz: return Element1dForcesAndMomentsC4p2.XxyyzzInKiloNewtonMeter();
      }

      throw new NotImplementedException();
    }
  }
}
