using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using GsaGH.Helpers.GsaApi;
using GsaGH.Parameters.Results;

using GsaGHTests.Helper;

using Xunit;

namespace GsaGHTests.Parameters.Results {
  [Collection("GrasshopperFixture collection")]
  public class NodeFootfallTests {

    private static readonly string NodeList = "200 to 206";

    [Fact]
    public void NodeFootfallNodeIdsFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelFootfall, 16);

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      IEntity0dResultSubset<IFootfall, ResultFootfall<Entity0dExtremaKey>> resultSet
        = result.NodeResonantFootfalls.ResultSubset(nodeIds);

      // Assert node IDs
      var expectedIds = result.Model.ApiModel.Nodes(NodeList).Keys.OrderBy(x => x).ToList();
      Assert.Equal(expectedIds, resultSet.Ids);

      result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelFootfall, 17);
      resultSet = result.NodeTransientFootfalls.ResultSubset(nodeIds);
      Assert.Equal(expectedIds, resultSet.Ids);
    }

    [Fact]
    public void NodeFootfallResonantResponseFactorTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelFootfall, 16);
      List<double> expected = NodeFootfallResonantA16.MaximumResponseFactor();

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      IEntity0dResultSubset<IFootfall, ResultFootfall<Entity0dExtremaKey>> resultSet
        = result.NodeResonantFootfalls.ResultSubset(nodeIds);

      // Assert values
      for (int i = 0; i < nodeIds.Count; i++) {
        double value = ResultHelper.RoundToSignificantDigits(
          resultSet.Subset[nodeIds[i]][0].MaximumResponseFactor, 4);
        Assert.Equal(expected[i], value);
      }

      // Assert Max in set
      Entity0dExtremaKey key = resultSet.Max.MaximumResponseFactor;
      IFootfall extrema = resultSet.GetExtrema(key);
      double max = ResultHelper.RoundToSignificantDigits(extrema.MaximumResponseFactor, 4);
      Assert.Equal(expected.Max(), max);
    }

    [Fact]
    public void NodeFootfallTransientResponseFactorTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelFootfall, 17);
      List<double> expected = NodeFootfallTransientA17.MaximumResponseFactor();

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      IEntity0dResultSubset<IFootfall, ResultFootfall<Entity0dExtremaKey>> resultSet
        = result.NodeTransientFootfalls.ResultSubset(nodeIds);

      // Assert values
      for (int i = 0; i < nodeIds.Count; i++) {
        double value = ResultHelper.RoundToSignificantDigits(
          resultSet.Subset[nodeIds[i]][0].MaximumResponseFactor, 4);
        Assert.Equal(expected[i], value);
      }

      // Assert Max in set
      Entity0dExtremaKey key = resultSet.Max.MaximumResponseFactor;
      IFootfall extrema = resultSet.GetExtrema(key);
      double max = ResultHelper.RoundToSignificantDigits(extrema.MaximumResponseFactor, 4);
      Assert.Equal(expected.Max(), max);
    }

    [Fact]
    public void NodeFootfallResonantVelocitiesTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelFootfall, 16);

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      IEntity0dResultSubset<IFootfall, ResultFootfall<Entity0dExtremaKey>> resultSet
        = result.NodeResonantFootfalls.ResultSubset(nodeIds);

      List<double> expectedPeak = NodeFootfallResonantA16.PeakVelocity();
      List<double> expectedRMS = NodeFootfallResonantA16.RMSVelocity();
      for (int i = 0; i < nodeIds.Count; i++) {
        double peak = ResultHelper.RoundToSignificantDigits(
          resultSet.Subset[nodeIds[i]][0].PeakVelocity.Value, 4);
        Assert.Equal(expectedPeak[i], peak);
        double rms = ResultHelper.RoundToSignificantDigits(
          resultSet.Subset[nodeIds[i]][0].RmsVelocity.Value, 4);
        Assert.Equal(expectedRMS[i], rms);
      }

      // Assert Max in set
      Entity0dExtremaKey key = resultSet.Max.PeakVelocity;
      IFootfall extrema = resultSet.GetExtrema(key);
      double max = ResultHelper.RoundToSignificantDigits(extrema.PeakVelocity.Value, 4);
      Assert.Equal(expectedPeak.Max(), max);

      key = resultSet.Max.RmsVelocity;
      extrema = resultSet.GetExtrema(key);
      max = ResultHelper.RoundToSignificantDigits(extrema.RmsVelocity.Value, 4);
      Assert.Equal(expectedRMS.Max(), max);
    }

    [Fact]
    public void NodeFootfallTransientVelocitiesTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelFootfall, 17);

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      IEntity0dResultSubset<IFootfall, ResultFootfall<Entity0dExtremaKey>> resultSet
        = result.NodeTransientFootfalls.ResultSubset(nodeIds);

      List<double> expectedPeak = NodeFootfallTransientA17.PeakVelocity();
      List<double> expectedRMS = NodeFootfallTransientA17.RMSVelocity();
      for (int i = 0; i < nodeIds.Count; i++) {
        double peak = ResultHelper.RoundToSignificantDigits(
          resultSet.Subset[nodeIds[i]][0].PeakVelocity.Value, 4);
        Assert.Equal(expectedPeak[i], peak);
        double rms = ResultHelper.RoundToSignificantDigits(
          resultSet.Subset[nodeIds[i]][0].RmsVelocity.Value, 4);
        Assert.Equal(expectedRMS[i], rms);
      }

      // Assert Max in set
      Entity0dExtremaKey key = resultSet.Max.PeakVelocity;
      IFootfall extrema = resultSet.GetExtrema(key);
      double max = ResultHelper.RoundToSignificantDigits(extrema.PeakVelocity.Value, 4);
      Assert.Equal(expectedPeak.Max(), max);

      key = resultSet.Max.RmsVelocity;
      extrema = resultSet.GetExtrema(key);
      max = ResultHelper.RoundToSignificantDigits(extrema.RmsVelocity.Value, 4);
      Assert.Equal(expectedRMS.Max(), max);
    }

    [Fact]
    public void NodeFootfallResonantAccelerationsTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelFootfall, 16);

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      IEntity0dResultSubset<IFootfall, ResultFootfall<Entity0dExtremaKey>> resultSet
        = result.NodeResonantFootfalls.ResultSubset(nodeIds);

      List<double> expectedPeak = NodeFootfallResonantA16.PeakAcceleration();
      List<double> expectedRMS = NodeFootfallResonantA16.RMSAcceleration();
      for (int i = 0; i < nodeIds.Count; i++) {
        double peak = ResultHelper.RoundToSignificantDigits(
          resultSet.Subset[nodeIds[i]][0].PeakAcceleration.Value, 4);
        Assert.Equal(expectedPeak[i], peak);
        double rms = ResultHelper.RoundToSignificantDigits(
          resultSet.Subset[nodeIds[i]][0].RmsAcceleration.Value, 4);
        Assert.Equal(expectedRMS[i], rms);
      }

      // Assert Max in set
      Entity0dExtremaKey key = resultSet.Max.PeakVelocity;
      IFootfall extrema = resultSet.GetExtrema(key);
      double max = ResultHelper.RoundToSignificantDigits(extrema.PeakAcceleration.Value, 4);
      Assert.Equal(expectedPeak.Max(), max);

      key = resultSet.Max.RmsVelocity;
      extrema = resultSet.GetExtrema(key);
      max = ResultHelper.RoundToSignificantDigits(extrema.RmsAcceleration.Value, 4);
      Assert.Equal(expectedRMS.Max(), max);
    }

    [Fact]
    public void NodeFootfallTransientAccelerationsTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelFootfall, 17);

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      IEntity0dResultSubset<IFootfall, ResultFootfall<Entity0dExtremaKey>> resultSet
        = result.NodeTransientFootfalls.ResultSubset(nodeIds);

      List<double> expectedPeak = NodeFootfallTransientA17.PeakAcceleration();
      List<double> expectedRMS = NodeFootfallTransientA17.RMSAcceleration();
      for (int i = 0; i < nodeIds.Count; i++) {
        double peak = ResultHelper.RoundToSignificantDigits(
          resultSet.Subset[nodeIds[i]][0].PeakAcceleration.Value, 4);
        Assert.Equal(expectedPeak[i], peak);
        double rms = ResultHelper.RoundToSignificantDigits(
          resultSet.Subset[nodeIds[i]][0].RmsAcceleration.Value, 4);
        Assert.Equal(expectedRMS[i], rms);
      }

      // Assert Max in set
      Entity0dExtremaKey key = resultSet.Max.PeakAcceleration;
      IFootfall extrema = resultSet.GetExtrema(key);
      double max = ResultHelper.RoundToSignificantDigits(extrema.PeakAcceleration.Value, 4);
      Assert.Equal(expectedPeak.Max(), max);

      key = resultSet.Max.RmsAcceleration;
      extrema = resultSet.GetExtrema(key);
      max = ResultHelper.RoundToSignificantDigits(extrema.RmsAcceleration.Value, 4);
      Assert.Equal(expectedRMS.Max(), max);
    }

    [Fact]
    public void NodeFootfallResonantFrequencyTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelFootfall, 16);

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      IEntity0dResultSubset<IFootfall, ResultFootfall<Entity0dExtremaKey>> resultSet
        = result.NodeResonantFootfalls.ResultSubset(nodeIds);

      List<double> expectedFrequency = NodeFootfallResonantA16.CriticalFrequency();
      List<int> expectedNode = NodeFootfallResonantA16.CriticalNode();
      for (int i = 0; i < nodeIds.Count; i++) {
        double frequency = ResultHelper.RoundToSignificantDigits(
          resultSet.Subset[nodeIds[i]][0].CriticalFrequency.Value, 4);
        Assert.Equal(expectedFrequency[i], frequency);
        int node = resultSet.Subset[nodeIds[i]][0].CriticalNode;
        Assert.Equal(expectedNode[i], node);
      }

      // Assert Max in set
      Entity0dExtremaKey key = resultSet.Max.CriticalFrequency;
      IFootfall extrema = resultSet.GetExtrema(key);
      double max = ResultHelper.RoundToSignificantDigits(extrema.CriticalFrequency.Value, 4);
      Assert.Equal(expectedFrequency.Max(), max);
    }

    [Fact]
    public void NodeFootfallTransientFrequencyTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelFootfall, 17);

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      IEntity0dResultSubset<IFootfall, ResultFootfall<Entity0dExtremaKey>> resultSet
        = result.NodeTransientFootfalls.ResultSubset(nodeIds);

      List<double> expectedFrequency = NodeFootfallTransientA17.CriticalFrequency();
      List<int> expectedNode = NodeFootfallTransientA17.CriticalNode();
      for (int i = 0; i < nodeIds.Count; i++) {
        double frequency = ResultHelper.RoundToSignificantDigits(
          resultSet.Subset[nodeIds[i]][0].CriticalFrequency.Value, 4);
        Assert.Equal(expectedFrequency[i], frequency);
        int node = resultSet.Subset[nodeIds[i]][0].CriticalNode;
        Assert.Equal(expectedNode[i], node);
      }

      // Assert Max in set
      Entity0dExtremaKey key = resultSet.Max.CriticalFrequency;
      IFootfall extrema = resultSet.GetExtrema(key);
      double max = ResultHelper.RoundToSignificantDigits(extrema.CriticalFrequency.Value, 4);
      Assert.Equal(expectedFrequency.Max(), max);
    }
  }
}
