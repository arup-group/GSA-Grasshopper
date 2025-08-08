using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using GsaGH.Components;
using GsaGH.Helpers.GsaApi;
using GsaGH.Parameters;
using GsaGH.Parameters.Results;

using GsaGHTests.Helper;
using GsaGHTests.Helpers;
using GsaGHTests.Parameters.Results;
using GsaGHTests.TestHelpers;

using OasysUnits;
using OasysUnits.Units;

using Xunit;

namespace GsaGHTests.Components.Results {
  [Collection("GrasshopperFixture collection")]
  public class FootfallResultsTests {
    private static readonly string NodeList = "200 to 206";

    [Fact]
    public void InvalidInputErrorTests() {
      var comp = new FootfallResults();
      ComponentTestHelper.SetInput(comp, "not a result");
      comp.Params.Output[0].CollectData();
      Assert.True((int)comp.RuntimeMessageLevel >= 10);

      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelFootfall, 1);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      comp.Params.Output[0].CollectData();
      Assert.True((int)comp.RuntimeMessageLevel >= 10);
    }

    [Fact]
    public void NodeFootfallNodeIdsFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelFootfall, 16);

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      var comp = new FootfallResults();
      comp.SetSelected(0, 1);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, NodeList, 1);

      for (int i = 0; i < comp.Params.Output.Count; i++) { // loop through each output
        IList<GH_Path> paths = ComponentTestHelper.GetPathOutput(comp, i);
        Assert.Single(paths);

        var cases = paths.Select(x => x.Indices[0]).ToList();
        foreach (int caseid in cases) {
          Assert.Equal(16, caseid);
        }

        var permutations = paths.Select(x => x.Indices[1]).ToList();
        foreach (int permutation in permutations) {
          Assert.Equal(0, permutation);
        }
      }

      var ids = (IList<GH_Integer>)ComponentTestHelper.GetListOutput(comp, 7);
      for (int j = 0; j < ids.Count; j++) {
        // Assert element IDs
        var expectedIds = result.Model.ApiModel.Nodes(NodeList).Keys.OrderBy(x => x).ToList();
        Assert.Equal(expectedIds[j], ids[j].Value);
      }
    }

    [Fact]
    public void NodeFootfallResonantResponseFactorTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelFootfall, 16);
      List<double> expected = NodeFootfallResonantA16.MaximumResponseFactor();

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      var comp = new FootfallResults();
      comp.SetSelected(0, 0);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, NodeList, 1);

      // Assert values
      var resultSet = (IList<GH_Number>)ComponentTestHelper.GetListOutput(comp, 0);
      for (int i = 0; i < resultSet.Count; i++) {
        double value = ResultHelper.RoundToSignificantDigits(resultSet[i].Value, 4);
        DoubleAssertHelper.Equals(expected[i], value);
      }

      // Assert Max in set
      comp.SetSelected(1, 1);
      resultSet = (IList<GH_Number>)ComponentTestHelper.GetListOutput(comp, 0);
      Assert.Single(resultSet);
      DoubleAssertHelper.Equals(expected.Max(), ResultHelper.RoundToSignificantDigits(resultSet[0].Value, 4));

      // Assert Min in set
      comp.SetSelected(1, 7);
      resultSet = (IList<GH_Number>)ComponentTestHelper.GetListOutput(comp, 0);
      Assert.Single(resultSet);
      DoubleAssertHelper.Equals(expected.Min(), ResultHelper.RoundToSignificantDigits(resultSet[0].Value, 4));
    }

    [Fact]
    public void NodeFootfallTransientResponseFactorTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelFootfall, 17);
      List<double> expected = NodeFootfallTransientA17.MaximumResponseFactor();

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      var comp = new FootfallResults();
      comp.SetSelected(0, 1);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, NodeList, 1);

      // Assert values
      var resultSet = (IList<GH_Number>)ComponentTestHelper.GetListOutput(comp, 0);
      for (int i = 0; i < resultSet.Count; i++) {
        double value = ResultHelper.RoundToSignificantDigits(resultSet[i].Value, 4);
        DoubleAssertHelper.Equals(expected[i], value);
      }

      // Assert Max in set
      comp.SetSelected(1, 1);
      resultSet = (IList<GH_Number>)ComponentTestHelper.GetListOutput(comp, 0);
      Assert.Single(resultSet);
      DoubleAssertHelper.Equals(expected.Max(), ResultHelper.RoundToSignificantDigits(resultSet[0].Value, 4));

      // Assert Min in set
      comp.SetSelected(1, 7);
      resultSet = (IList<GH_Number>)ComponentTestHelper.GetListOutput(comp, 0);
      Assert.Single(resultSet);
      DoubleAssertHelper.Equals(expected.Min(), ResultHelper.RoundToSignificantDigits(resultSet[0].Value, 4));
    }

    [Fact]
    public void NodeFootfallResonantVelocitiesTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelFootfall, 16);
      List<double> expectedPeak = NodeFootfallResonantA16.PeakVelocity();
      List<double> expectedRMS = NodeFootfallResonantA16.RMSVelocity();

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      var comp = new FootfallResults();
      comp.SetSelected(0, 0);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, NodeList, 1);

      // Assert values
      List<IQuantity> resultSetPeak = ComponentTestHelper.GetResultOutput(comp, 1);
      List<IQuantity> resultSetRMS = ComponentTestHelper.GetResultOutput(comp, 2);
      Assert.Equal(resultSetPeak.Count, resultSetRMS.Count);
      for (int i = 0; i < resultSetPeak.Count; i++) {
        double peak = ResultHelper.RoundToSignificantDigits(
          resultSetPeak[i].As(SpeedUnit.MeterPerSecond), 4);
        DoubleAssertHelper.Equals(expectedPeak[i], peak);

        double rms = ResultHelper.RoundToSignificantDigits(
          resultSetRMS[i].As(SpeedUnit.MeterPerSecond), 4);
        DoubleAssertHelper.Equals(expectedRMS[i], rms);
      }

      // Assert Max Peak in set
      comp.SetSelected(1, 2);
      resultSetPeak = ComponentTestHelper.GetResultOutput(comp, 1);
      Assert.Single(resultSetPeak);
      DoubleAssertHelper.Equals(expectedPeak.Max(),
        ResultHelper.RoundToSignificantDigits(resultSetPeak[0].As(SpeedUnit.MeterPerSecond), 4));

      // Assert Max RMS in set
      comp.SetSelected(1, 3);
      resultSetRMS = ComponentTestHelper.GetResultOutput(comp, 2);
      Assert.Single(resultSetRMS);
      DoubleAssertHelper.Equals(expectedRMS.Max(),
        ResultHelper.RoundToSignificantDigits(resultSetRMS[0].As(SpeedUnit.MeterPerSecond), 4));

      // Assert Min Peak in set
      comp.SetSelected(1, 8);
      resultSetPeak = ComponentTestHelper.GetResultOutput(comp, 1);
      Assert.Single(resultSetPeak);
      DoubleAssertHelper.Equals(expectedPeak.Min(),
        ResultHelper.RoundToSignificantDigits(resultSetPeak[0].As(SpeedUnit.MeterPerSecond), 4));

      // Assert Max RMS in set
      comp.SetSelected(1, 9);
      resultSetRMS = ComponentTestHelper.GetResultOutput(comp, 2);
      Assert.Single(resultSetRMS);
      DoubleAssertHelper.Equals(expectedRMS.Min(),
        ResultHelper.RoundToSignificantDigits(resultSetRMS[0].As(SpeedUnit.MeterPerSecond), 4));
    }

    [Fact]
    public void NodeFootfallTransientVelocitiesTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelFootfall, 17);
      List<double> expectedPeak = NodeFootfallTransientA17.PeakVelocity();
      List<double> expectedRMS = NodeFootfallTransientA17.RMSVelocity();

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      var comp = new FootfallResults();
      comp.SetSelected(0, 1);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, NodeList, 1);

      // Assert values
      List<IQuantity> resultSetPeak = ComponentTestHelper.GetResultOutput(comp, 1);
      List<IQuantity> resultSetRMS = ComponentTestHelper.GetResultOutput(comp, 2);
      Assert.Equal(resultSetPeak.Count, resultSetRMS.Count);
      for (int i = 0; i < resultSetPeak.Count; i++) {
        double peak = ResultHelper.RoundToSignificantDigits(
          resultSetPeak[i].As(SpeedUnit.MeterPerSecond), 4);
        DoubleAssertHelper.Equals(expectedPeak[i], peak);

        double rms = ResultHelper.RoundToSignificantDigits(
          resultSetRMS[i].As(SpeedUnit.MeterPerSecond), 4);
        DoubleAssertHelper.Equals(expectedRMS[i], rms);
      }

      // Assert Max Peak in set
      comp.SetSelected(1, 2);
      resultSetPeak = ComponentTestHelper.GetResultOutput(comp, 1);
      Assert.Single(resultSetPeak);
      DoubleAssertHelper.Equals(expectedPeak.Max(),
        ResultHelper.RoundToSignificantDigits(resultSetPeak[0].As(SpeedUnit.MeterPerSecond), 4));

      // Assert Max RMS in set
      comp.SetSelected(1, 3);
      resultSetRMS = ComponentTestHelper.GetResultOutput(comp, 2);
      Assert.Single(resultSetRMS);
      DoubleAssertHelper.Equals(expectedRMS.Max(),
        ResultHelper.RoundToSignificantDigits(resultSetRMS[0].As(SpeedUnit.MeterPerSecond), 4));

      // Assert Min Peak in set
      comp.SetSelected(1, 8);
      resultSetPeak = ComponentTestHelper.GetResultOutput(comp, 1);
      Assert.Single(resultSetPeak);
      DoubleAssertHelper.Equals(expectedPeak.Min(),
        ResultHelper.RoundToSignificantDigits(resultSetPeak[0].As(SpeedUnit.MeterPerSecond), 4));

      // Assert Max RMS in set
      comp.SetSelected(1, 9);
      resultSetRMS = ComponentTestHelper.GetResultOutput(comp, 2);
      Assert.Single(resultSetRMS);
      DoubleAssertHelper.Equals(expectedRMS.Min(),
        ResultHelper.RoundToSignificantDigits(resultSetRMS[0].As(SpeedUnit.MeterPerSecond), 4));
    }

    [Fact]
    public void NodeFootfallResonantAccelerationsTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelFootfall, 16);
      List<double> expectedPeak = NodeFootfallResonantA16.PeakAcceleration();
      List<double> expectedRMS = NodeFootfallResonantA16.RMSAcceleration();

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      var comp = new FootfallResults();
      comp.SetSelected(0, 0);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, NodeList, 1);

      // Assert values
      List<IQuantity> resultSetPeak = ComponentTestHelper.GetResultOutput(comp, 3);
      List<IQuantity> resultSetRMS = ComponentTestHelper.GetResultOutput(comp, 4);
      DoubleAssertHelper.Equals(resultSetPeak.Count, resultSetRMS.Count);
      for (int i = 0; i < resultSetPeak.Count; i++) {
        double peak = ResultHelper.RoundToSignificantDigits(
          resultSetPeak[i].As(AccelerationUnit.MeterPerSecondSquared), 4);
        DoubleAssertHelper.Equals(expectedPeak[i], peak);
        double rms = ResultHelper.RoundToSignificantDigits(
          resultSetRMS[i].As(AccelerationUnit.MeterPerSecondSquared), 4);
        DoubleAssertHelper.Equals(expectedRMS[i], rms);
      }

      // Assert Max Peak in set
      comp.SetSelected(1, 4);
      resultSetPeak = ComponentTestHelper.GetResultOutput(comp, 3);
      Assert.Single(resultSetPeak);
      DoubleAssertHelper.Equals(expectedPeak.Max(),
        ResultHelper.RoundToSignificantDigits(resultSetPeak[0].As(AccelerationUnit.MeterPerSecondSquared), 4));

      // Assert Max RMS in set
      comp.SetSelected(1, 5);
      resultSetRMS = ComponentTestHelper.GetResultOutput(comp, 4);
      Assert.Single(resultSetRMS);
      DoubleAssertHelper.Equals(expectedRMS.Max(),
        ResultHelper.RoundToSignificantDigits(resultSetRMS[0].As(AccelerationUnit.MeterPerSecondSquared), 4));

      // Assert Min Peak in set
      comp.SetSelected(1, 10);
      resultSetPeak = ComponentTestHelper.GetResultOutput(comp, 3);
      Assert.Single(resultSetPeak);
      DoubleAssertHelper.Equals(expectedPeak.Min(),
        ResultHelper.RoundToSignificantDigits(resultSetPeak[0].As(AccelerationUnit.MeterPerSecondSquared), 4));

      // Assert Min RMS in set
      comp.SetSelected(1, 11);
      resultSetRMS = ComponentTestHelper.GetResultOutput(comp, 4);
      Assert.Single(resultSetRMS);
      DoubleAssertHelper.Equals(expectedRMS.Min(),
        ResultHelper.RoundToSignificantDigits(resultSetRMS[0].As(AccelerationUnit.MeterPerSecondSquared), 4));
    }

    [Fact]
    public void NodeFootfallTransientAccelerationsTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelFootfall, 17);
      List<double> expectedPeak = NodeFootfallTransientA17.PeakAcceleration();
      List<double> expectedRMS = NodeFootfallTransientA17.RMSAcceleration();

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      var comp = new FootfallResults();
      comp.SetSelected(0, 1);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, NodeList, 1);

      // Assert values
      List<IQuantity> resultSetPeak = ComponentTestHelper.GetResultOutput(comp, 3);
      List<IQuantity> resultSetRMS = ComponentTestHelper.GetResultOutput(comp, 4);
      DoubleAssertHelper.Equals(resultSetPeak.Count, resultSetRMS.Count);
      for (int i = 0; i < resultSetPeak.Count; i++) {
        double peak = ResultHelper.RoundToSignificantDigits(
          resultSetPeak[i].As(AccelerationUnit.MeterPerSecondSquared), 4);
        DoubleAssertHelper.Equals(expectedPeak[i], peak);
        double rms = ResultHelper.RoundToSignificantDigits(
          resultSetRMS[i].As(AccelerationUnit.MeterPerSecondSquared), 4);
        DoubleAssertHelper.Equals(expectedRMS[i], rms);
      }

      // Assert Max Peak in set
      comp.SetSelected(1, 4);
      resultSetPeak = ComponentTestHelper.GetResultOutput(comp, 3);
      Assert.Single(resultSetPeak);
      DoubleAssertHelper.Equals(expectedPeak.Max(),
        ResultHelper.RoundToSignificantDigits(resultSetPeak[0].As(AccelerationUnit.MeterPerSecondSquared), 4));

      // Assert Max RMS in set
      comp.SetSelected(1, 5);
      resultSetRMS = ComponentTestHelper.GetResultOutput(comp, 4);
      Assert.Single(resultSetRMS);
      DoubleAssertHelper.Equals(expectedRMS.Max(),
        ResultHelper.RoundToSignificantDigits(resultSetRMS[0].As(AccelerationUnit.MeterPerSecondSquared), 4));

      // Assert Min Peak in set
      comp.SetSelected(1, 10);
      resultSetPeak = ComponentTestHelper.GetResultOutput(comp, 3);
      Assert.Single(resultSetPeak);
      DoubleAssertHelper.Equals(expectedPeak.Min(),
        ResultHelper.RoundToSignificantDigits(resultSetPeak[0].As(AccelerationUnit.MeterPerSecondSquared), 4));

      // Assert Min RMS in set
      comp.SetSelected(1, 11);
      resultSetRMS = ComponentTestHelper.GetResultOutput(comp, 4);
      Assert.Single(resultSetRMS);
      DoubleAssertHelper.Equals(expectedRMS.Min(),
        ResultHelper.RoundToSignificantDigits(resultSetRMS[0].As(AccelerationUnit.MeterPerSecondSquared), 4));
    }

    [Fact]
    public void NodeFootfallResonantFrequencyTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelFootfall, 16);
      List<double> expected = NodeFootfallResonantA16.CriticalFrequency();

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      var comp = new FootfallResults();
      comp.SetSelected(0, 0);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, NodeList, 1);

      // Assert values
      List<IQuantity> resultSet = ComponentTestHelper.GetResultOutput(comp, 6);
      for (int i = 0; i < resultSet.Count; i++) {
        double value = ResultHelper.RoundToSignificantDigits(resultSet[i].Value, 4);
        DoubleAssertHelper.Equals(expected[i], value);
      }

      // Assert Max in set
      comp.SetSelected(1, 6);
      resultSet = ComponentTestHelper.GetResultOutput(comp, 6);
      Assert.Single(resultSet);
      DoubleAssertHelper.Equals(expected.Max(), ResultHelper.RoundToSignificantDigits(resultSet[0].Value, 4));

      // Assert Min in set
      comp.SetSelected(1, 12);
      resultSet = ComponentTestHelper.GetResultOutput(comp, 6);
      Assert.Single(resultSet);
      DoubleAssertHelper.Equals(expected.Min(), ResultHelper.RoundToSignificantDigits(resultSet[0].Value, 4));
    }

    [Fact]
    public void NodeFootfallTransientFrequencyTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelFootfall, 17);
      List<double> expected = NodeFootfallTransientA17.CriticalFrequency();

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      var comp = new FootfallResults();
      comp.SetSelected(0, 1);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, NodeList, 1);

      // Assert values
      List<IQuantity> resultSet = ComponentTestHelper.GetResultOutput(comp, 6);
      for (int i = 0; i < resultSet.Count; i++) {
        double value = ResultHelper.RoundToSignificantDigits(resultSet[i].Value, 4);
        DoubleAssertHelper.Equals(expected[i], value);
      }

      // Assert Max in set
      comp.SetSelected(1, 6);
      resultSet = ComponentTestHelper.GetResultOutput(comp, 6);
      Assert.Single(resultSet);
      DoubleAssertHelper.Equals(expected.Max(), ResultHelper.RoundToSignificantDigits(resultSet[0].Value, 4));

      // Assert Min in set
      comp.SetSelected(1, 12);
      resultSet = ComponentTestHelper.GetResultOutput(comp, 6);
      Assert.Single(resultSet);
      DoubleAssertHelper.Equals(expected.Min(), ResultHelper.RoundToSignificantDigits(resultSet[0].Value, 4));
    }
  }
}
