using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using GsaGH.Helpers;
using GsaGH.Helpers.GsaApi;
using GsaGH.Parameters.Results;

using GsaGHTests.Helper;


using OasysUnits;
using OasysUnits.Units;

using Xunit;

namespace GsaGHTests.Parameters.Results {
  [Collection("GrasshopperFixture collection")]
  public class Entity2dForceEnvelopeTests {
    [Theory]
    [ClassData(typeof(TestDataGenerator))]
    public void Element2dForceEnvelopeTest(
      ResultTensor2InAxis component, EnvelopeMethod envelope) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.Element2dSimple, 2);
      List<double> expectedP1 = ExpectedForcesC2p1Values(component);
      List<double> expectedP2 = ExpectedForcesC2p2Values(component);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds("420 430 440 445", 2);
      IMeshResultSubset<IMeshQuantity<IForce2d>, IForce2d, ResultTensor2InAxis<Entity2dExtremaKey>> resultSet
        = result.Element2dForces.ResultSubset(elementIds);

      ConcurrentDictionary<int, IList<IQuantity>> values = ResultsUtility.GetResultComponent(
        resultSet.Subset, Selector(component), result.SelectedPermutationIds, envelope);

      int i = 0;
      foreach (int elementId in elementIds) {
        for (int vertex = 0; vertex < values[elementId].Count; vertex++) {
          double expected = TestsResultHelper.Envelope(expectedP1[i], expectedP2[i++], envelope);
          double actual = values[elementId][vertex].Value;
          Assert.Equal(expected, actual, DoubleComparer.Default);
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
        Array components = Enum.GetValues(typeof(ResultTensor2InAxis));
        Array envelopes = Enum.GetValues(typeof(EnvelopeMethod));
        foreach (ResultVector6 component in components) {
          foreach (EnvelopeMethod envelope in envelopes) {
            data.Add(new object[] {
              component, envelope
            });
          }
        }

        return data;
      }
    }

    private Func<IForce2d, IQuantity> Selector(ResultTensor2InAxis component) {
      switch (component) {
        case ResultTensor2InAxis.Nx:
          return (r) => r.Nx.ToUnit(ForcePerLengthUnit.KilonewtonPerMeter);
        case ResultTensor2InAxis.Ny:
          return (r) => r.Ny.ToUnit(ForcePerLengthUnit.KilonewtonPerMeter);
        case ResultTensor2InAxis.Nxy:
          return (r) => r.Nxy.ToUnit(ForcePerLengthUnit.KilonewtonPerMeter);
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedForcesC2p1Values(ResultTensor2InAxis component) {
      switch (component) {
        case ResultTensor2InAxis.Nx: return Element2dForcesC2p1.NxInKiloNewtonPerMeter();
        case ResultTensor2InAxis.Ny: return Element2dForcesC2p1.NyInKiloNewtonPerMeter();
        case ResultTensor2InAxis.Nxy: return Element2dForcesC2p1.NxyInKiloNewtonPerMeter();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedForcesC2p2Values(ResultTensor2InAxis component) {
      switch (component) {
        case ResultTensor2InAxis.Nx: return Element2dForcesC2p2.NxInKiloNewtonPerMeter();
        case ResultTensor2InAxis.Ny: return Element2dForcesC2p2.NyInKiloNewtonPerMeter();
        case ResultTensor2InAxis.Nxy: return Element2dForcesC2p2.NxyInKiloNewtonPerMeter();
      }

      throw new NotImplementedException();
    }
  }
}
