using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaGH.Helpers.GsaApi;
using GsaGH.Parameters.Results;
using GsaGHTests.Helper;
using OasysUnits;
using OasysUnits.Units;
using Xunit;

namespace GsaGHTests.Parameters.Results {
  [Collection("GrasshopperFixture collection")]
  public class NodeEnvelopeTests {
    [Theory]
    [ClassData(typeof(TestDataGenerator))]
    public void NodeDisplacementsEnvelopeTest(
      ResultVector6HelperEnum component, EnvelopeMethod envelope) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      List<double> expectedP1 = ExpectedDisplacementC4p1Values(component);
      List<double> expectedP2 = ExpectedDisplacementC4p2Values(component);

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds("442 to 468");
      INodeResultSubset<IDisplacement, ResultVector6<NodeExtremaKey>> resultSet
        = result.NodeDisplacements.ResultSubset(nodeIds);

      ConcurrentDictionary<int, IQuantity> values = ResultsUtility.GetResultComponent(resultSet.Subset, DisplacementSelector(component), envelope);

      int i = 0;
      foreach (int nodeId in nodeIds) {
        double expected = TestsResultHelper.Envelope(expectedP1[i], expectedP2[i++], envelope);
        double actual = ResultHelper.RoundToSignificantDigits(values[nodeId].Value, 4);
        Assert.Equal(expected, actual, 4);
      }
    }

    [Theory]
    [ClassData(typeof(TestDataGenerator))]
    public void NodeReactionForceEnvelopeTest(
      ResultVector6HelperEnum component, EnvelopeMethod envelope) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      List<double?> expectedP1 = ExpectedReactionForceC4p1Values(component);
      List<double?> expectedP2 = ExpectedReactionForceC4p2Values(component);
      bool isInvalid = expectedP1.Any(x => x == null);

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds("1324 to 1327");
      INodeResultSubset<IReactionForce, ResultVector6<NodeExtremaKey>> resultSet
        = result.NodeReactionForces.ResultSubset(nodeIds);

      if (isInvalid) {
        Assert.Throws<AggregateException>(() =>
        ResultsUtility.GetResultComponent(resultSet.Subset, ReactionForceSelector(component), envelope));
        return;
      }

      ConcurrentDictionary<int, IQuantity> values = ResultsUtility.GetResultComponent(resultSet.Subset, ReactionForceSelector(component), envelope);

      int i = 0;
      foreach (int nodeId in nodeIds) {
        double expected = TestsResultHelper.Envelope(expectedP1[i], expectedP2[i++], envelope);
        double actual = ResultHelper.RoundToSignificantDigits(values[nodeId].Value, 4);
        Assert.Equal(expected, actual, 4);
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

    private Func<IReactionForce, IQuantity> ReactionForceSelector(ResultVector6HelperEnum component) {
      switch (component) {
        case ResultVector6HelperEnum.X: return (r) => r.XToUnit(ForceUnit.Kilonewton);
        case ResultVector6HelperEnum.Y: return (r) => r.YToUnit(ForceUnit.Kilonewton);
        case ResultVector6HelperEnum.Z: return (r) => r.ZToUnit(ForceUnit.Kilonewton);
        case ResultVector6HelperEnum.Xyz: return (r) => r.XyzToUnit(ForceUnit.Kilonewton);
        case ResultVector6HelperEnum.Xx: return (r) => r.XxToUnit(MomentUnit.KilonewtonMeter);
        case ResultVector6HelperEnum.Yy: return (r) => r.YyToUnit(MomentUnit.KilonewtonMeter);
        case ResultVector6HelperEnum.Zz: return (r) => r.ZzToUnit(MomentUnit.KilonewtonMeter); 
        case ResultVector6HelperEnum.Xxyyzz: return (r) => r.XxyyzzToUnit(MomentUnit.KilonewtonMeter);
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedDisplacementC4p1Values(ResultVector6HelperEnum component) {
      switch (component) {
        case ResultVector6HelperEnum.X: return NodeDisplacementsC4p1.XInMillimeter();
        case ResultVector6HelperEnum.Y: return NodeDisplacementsC4p1.YInMillimeter();
        case ResultVector6HelperEnum.Z: return NodeDisplacementsC4p1.ZInMillimeter();
        case ResultVector6HelperEnum.Xyz: return NodeDisplacementsC4p1.XyzInMillimeter();
        case ResultVector6HelperEnum.Xx: return NodeDisplacementsC4p1.XxInRadian();
        case ResultVector6HelperEnum.Yy: return NodeDisplacementsC4p1.YyInRadian();
        case ResultVector6HelperEnum.Zz: return NodeDisplacementsC4p1.ZzInRadian();
        case ResultVector6HelperEnum.Xxyyzz: return NodeDisplacementsC4p1.XxyyzzInRadian();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedDisplacementC4p2Values(ResultVector6HelperEnum component) {
      switch (component) {
        case ResultVector6HelperEnum.X: return NodeDisplacementsC4p2.XInMillimeter();
        case ResultVector6HelperEnum.Y: return NodeDisplacementsC4p2.YInMillimeter();
        case ResultVector6HelperEnum.Z: return NodeDisplacementsC4p2.ZInMillimeter();
        case ResultVector6HelperEnum.Xyz: return NodeDisplacementsC4p2.XyzInMillimeter();
        case ResultVector6HelperEnum.Xx: return NodeDisplacementsC4p2.XxInRadian();
        case ResultVector6HelperEnum.Yy: return NodeDisplacementsC4p2.YyInRadian();
        case ResultVector6HelperEnum.Zz: return NodeDisplacementsC4p2.ZzInRadian();
        case ResultVector6HelperEnum.Xxyyzz: return NodeDisplacementsC4p2.XxyyzzInRadian();
      }

      throw new NotImplementedException();
    }

    private List<double?> ExpectedReactionForceC4p1Values(ResultVector6HelperEnum component) {
      switch (component) {
        case ResultVector6HelperEnum.X: return NodeReactionForcesC4p1.XInKiloNewtons();
        case ResultVector6HelperEnum.Y: return NodeReactionForcesC4p1.YInKiloNewtons();
        case ResultVector6HelperEnum.Z: return NodeReactionForcesC4p1.ZInKiloNewtons();
        case ResultVector6HelperEnum.Xyz: return NodeReactionForcesC4p1.XyzInKiloNewtons();
        case ResultVector6HelperEnum.Xx: return NodeReactionForcesC4p1.XxInKiloNewtonsPerMeter();
        case ResultVector6HelperEnum.Yy: return NodeReactionForcesC4p1.YyInKiloNewtonsPerMeter();
        case ResultVector6HelperEnum.Zz: return NodeReactionForcesC4p1.ZzInKiloNewtonsPerMeter();
        case ResultVector6HelperEnum.Xxyyzz:
          return NodeReactionForcesC4p1.XxyyzzInKiloNewtonsPerMeter();
      }

      throw new NotImplementedException();
    }

    private List<double?> ExpectedReactionForceC4p2Values(ResultVector6HelperEnum component) {
      switch (component) {
        case ResultVector6HelperEnum.X: return NodeReactionForcesC4p2.XInKiloNewtons();
        case ResultVector6HelperEnum.Y: return NodeReactionForcesC4p2.YInKiloNewtons();
        case ResultVector6HelperEnum.Z: return NodeReactionForcesC4p2.ZInKiloNewtons();
        case ResultVector6HelperEnum.Xyz: return NodeReactionForcesC4p2.XyzInKiloNewtons();
        case ResultVector6HelperEnum.Xx: return NodeReactionForcesC4p2.XxInKiloNewtonsPerMeter();
        case ResultVector6HelperEnum.Yy: return NodeReactionForcesC4p2.YyInKiloNewtonsPerMeter();
        case ResultVector6HelperEnum.Zz: return NodeReactionForcesC4p2.ZzInKiloNewtonsPerMeter();
        case ResultVector6HelperEnum.Xxyyzz:
          return NodeReactionForcesC4p2.XxyyzzInKiloNewtonsPerMeter();
      }

      throw new NotImplementedException();
    }
  }
}
