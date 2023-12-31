﻿using System;
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
  public class Entity2dDisplacementEnvelopeTests {
    [Theory]
    [ClassData(typeof(TestDataGenerator))]
    public void Element2dDisplacementsEnvelopeTest(
      ResultVector6HelperEnum component, EnvelopeMethod envelope) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.Element2dSimple, 2);
      List<double> expectedP1 = ExpectedDisplacementC2p1Values(component);
      List<double> expectedP2 = ExpectedDisplacementC2p2Values(component);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds("420 430 440 445", 2);
      IMeshResultSubset<IMeshQuantity<IDisplacement>, IDisplacement, ResultVector6<Entity2dExtremaKey>> resultSet
        = result.Element2dDisplacements.ResultSubset(elementIds);

      ConcurrentDictionary<int, IList<IQuantity>> values = ResultsUtility.GetResultComponent(resultSet.Subset, Selector(component), envelope);

      int i = 0;
      foreach (int elementId in elementIds) {
        for (int vertex = 0; vertex < values[elementId].Count; vertex++) {
          double expected = TestsResultHelper.Envelope(expectedP1[i], expectedP2[i++], envelope);
          double actual = ResultHelper.RoundToSignificantDigits(values[elementId][vertex].Value, 4);
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
        var components = new List<ResultVector6HelperEnum> {
          ResultVector6HelperEnum.X,
          ResultVector6HelperEnum.Y,
          ResultVector6HelperEnum.Z,
          ResultVector6HelperEnum.Xyz
        };
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

    private Func<IDisplacement, IQuantity> Selector(ResultVector6HelperEnum component) {
      switch (component) {
        case ResultVector6HelperEnum.X: return (r) => r.X.ToUnit(LengthUnit.Millimeter);
        case ResultVector6HelperEnum.Y: return (r) => r.Y.ToUnit(LengthUnit.Millimeter);
        case ResultVector6HelperEnum.Z: return (r) => r.Z.ToUnit(LengthUnit.Millimeter);
        case ResultVector6HelperEnum.Xyz: return (r) => r.Xyz.ToUnit(LengthUnit.Millimeter);
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedDisplacementC2p1Values(ResultVector6HelperEnum component) {
      switch (component) {
        case ResultVector6HelperEnum.X: return Element2dDisplacementsC2p1.XInMillimeter();
        case ResultVector6HelperEnum.Y: return Element2dDisplacementsC2p1.YInMillimeter();
        case ResultVector6HelperEnum.Z: return Element2dDisplacementsC2p1.ZInMillimeter();
        case ResultVector6HelperEnum.Xyz: return Element2dDisplacementsC2p1.XyzInMillimeter();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedDisplacementC2p2Values(ResultVector6HelperEnum component) {
      switch (component) {
        case ResultVector6HelperEnum.X: return Element2dDisplacementsC2p2.XInMillimeter();
        case ResultVector6HelperEnum.Y: return Element2dDisplacementsC2p2.YInMillimeter();
        case ResultVector6HelperEnum.Z: return Element2dDisplacementsC2p2.ZInMillimeter();
        case ResultVector6HelperEnum.Xyz: return Element2dDisplacementsC2p2.XyzInMillimeter();
      }

      throw new NotImplementedException();
    }
  }
}
