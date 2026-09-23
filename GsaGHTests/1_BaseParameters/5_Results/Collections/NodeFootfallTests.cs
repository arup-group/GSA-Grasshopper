using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using GsaGH.Helpers;
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
        double value = resultSet.Subset[nodeIds[i]][0].MaximumResponseFactor;
        Assert.Equal(expected[i], value, DoubleComparer.Default);
      }

      // Assert Max in set
      Entity0dExtremaKey key = resultSet.Max.MaximumResponseFactor;
      IFootfall extrema = resultSet.GetExtrema(key);
      double max = extrema.MaximumResponseFactor;
      Assert.Equal(expected.Max(), max, DoubleComparer.Default);
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
        double value = resultSet.Subset[nodeIds[i]][0].MaximumResponseFactor;
        Assert.Equal(expected[i], value, DoubleComparer.Default);
      }

      // Assert Max in set
      Entity0dExtremaKey key = resultSet.Max.MaximumResponseFactor;
      IFootfall extrema = resultSet.GetExtrema(key);
      double max = extrema.MaximumResponseFactor;
      Assert.Equal(expected.Max(), max, DoubleComparer.Default);
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
        double peak = resultSet.Subset[nodeIds[i]][0].PeakVelocity.Value;
        Assert.Equal(expectedPeak[i], peak, DoubleComparer.Default);
        double rms = resultSet.Subset[nodeIds[i]][0].RmsVelocity.Value;
        Assert.Equal(expectedRMS[i], rms, DoubleComparer.Default);
      }

      // Assert Max in set
      Entity0dExtremaKey key = resultSet.Max.PeakVelocity;
      IFootfall extrema = resultSet.GetExtrema(key);
      double max = extrema.PeakVelocity.Value;
      Assert.Equal(expectedPeak.Max(), max, DoubleComparer.Default);

      key = resultSet.Max.RmsVelocity;
      extrema = resultSet.GetExtrema(key);
      max = extrema.RmsVelocity.Value;
      Assert.Equal(expectedRMS.Max(), max, DoubleComparer.Default);
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
        double peak = resultSet.Subset[nodeIds[i]][0].PeakVelocity.Value;
        Assert.Equal(expectedPeak[i], peak, DoubleComparer.Default);
        double rms = resultSet.Subset[nodeIds[i]][0].RmsVelocity.Value;
        Assert.Equal(expectedRMS[i], rms, DoubleComparer.Default);
      }

      // Assert Max in set
      Entity0dExtremaKey key = resultSet.Max.PeakVelocity;
      IFootfall extrema = resultSet.GetExtrema(key);
      double max = extrema.PeakVelocity.Value;
      Assert.Equal(expectedPeak.Max(), max, DoubleComparer.Default);

      key = resultSet.Max.RmsVelocity;
      extrema = resultSet.GetExtrema(key);
      max = extrema.RmsVelocity.Value;
      Assert.Equal(expectedRMS.Max(), max, DoubleComparer.Default);
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
        double peak = resultSet.Subset[nodeIds[i]][0].PeakAcceleration.Value;
        Assert.Equal(expectedPeak[i], peak, DoubleComparer.Default);
        double rms = resultSet.Subset[nodeIds[i]][0].RmsAcceleration.Value;
        Assert.Equal(expectedRMS[i], rms, DoubleComparer.Default);
      }

      // Assert Max in set
      Entity0dExtremaKey key = resultSet.Max.PeakVelocity;
      IFootfall extrema = resultSet.GetExtrema(key);
      double max = extrema.PeakAcceleration.Value;
      Assert.Equal(expectedPeak.Max(), max, DoubleComparer.Default);

      key = resultSet.Max.RmsVelocity;
      extrema = resultSet.GetExtrema(key);
      max = extrema.RmsAcceleration.Value;
      Assert.Equal(expectedRMS.Max(), max, DoubleComparer.Default);
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
        double peak = resultSet.Subset[nodeIds[i]][0].PeakAcceleration.Value;
        Assert.Equal(expectedPeak[i], peak, DoubleComparer.Default);
        double rms = resultSet.Subset[nodeIds[i]][0].RmsAcceleration.Value;
        Assert.Equal(expectedRMS[i], rms, DoubleComparer.Default);
      }

      // Assert Max in set
      Entity0dExtremaKey key = resultSet.Max.PeakAcceleration;
      IFootfall extrema = resultSet.GetExtrema(key);
      double max = extrema.PeakAcceleration.Value;
      Assert.Equal(expectedPeak.Max(), max, DoubleComparer.Default);

      key = resultSet.Max.RmsAcceleration;
      extrema = resultSet.GetExtrema(key);
      max = extrema.RmsAcceleration.Value;
      Assert.Equal(expectedRMS.Max(), max, DoubleComparer.Default);
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
        double frequency = resultSet.Subset[nodeIds[i]][0].CriticalFrequency.Value;
        Assert.Equal(expectedFrequency[i], frequency, DoubleComparer.Default);
        int node = resultSet.Subset[nodeIds[i]][0].CriticalNode;
        Assert.Equal(expectedNode[i], node);
      }

      // Assert Max in set
      Entity0dExtremaKey key = resultSet.Max.CriticalFrequency;
      IFootfall extrema = resultSet.GetExtrema(key);
      double max = extrema.CriticalFrequency.Value;
      Assert.Equal(expectedFrequency.Max(), max, DoubleComparer.Default);
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
        double frequency = resultSet.Subset[nodeIds[i]][0].CriticalFrequency.Value;
        Assert.Equal(expectedFrequency[i], frequency, DoubleComparer.Default);
        int node = resultSet.Subset[nodeIds[i]][0].CriticalNode;
        Assert.Equal(expectedNode[i], node);
      }

      // Assert Max in set
      Entity0dExtremaKey key = resultSet.Max.CriticalFrequency;
      IFootfall extrema = resultSet.GetExtrema(key);
      double max = extrema.CriticalFrequency.Value;
      Assert.Equal(expectedFrequency.Max(), max, DoubleComparer.Default);
    }
  }
}
