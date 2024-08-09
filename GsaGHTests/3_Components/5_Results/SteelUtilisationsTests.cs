using System;
using System.Collections.Generic;
using System.Linq;

using Grasshopper.Kernel.Data;

using GsaGH.Helpers.GsaApi;
using GsaGH.Parameters;
using GsaGH.Parameters.Results;

using GsaGHTests.Helper;
using GsaGHTests.Helpers;
using GsaGHTests.Parameters;
using GsaGHTests.Parameters.Results;

using OasysGH.Parameters;

using OasysUnits;

using Xunit;

using GsaResultTests = GsaGHTests.Parameters.Results.GsaResultTests;
using SteelUtilisations = GsaGH.Components.SteelUtilisations;

namespace GsaGHTests.Components.Results {
  [Collection("GrasshopperFixture collection")]
  public class SteelUtilisationsTests {
    private static string _memberList = "all";

    [Fact]
    public void InvalidInputErrorTests() {
      var comp = new SteelUtilisations();
      ComponentTestHelper.SetInput(comp, "not a result");
      comp.Params.Output[0].CollectData();
      Assert.True((int)comp.RuntimeMessageLevel >= 10);
    }

    [Theory]
    [InlineData("A", 1)]
    [InlineData("A", 2)]
    [InlineData("C", 1)]
    public void SteelUtilisationsIdsFromCaseTest(string caseName, int index) {
      // Assemble
      GsaResult result = caseName.Equals("A")
        ? (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignSimple, index)
        : (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignSimple, index, new List<int>() { 1 });

      Assert.NotNull(result);
      // Act
      var comp = new SteelUtilisations();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));

      for (int i = 0; i < comp.Params.Output.Count; i++) { // loop through each output
        IList<GH_Path> paths = ComponentTestHelper.GetPathOutput(comp, i);
        Assert.Single(paths);

        var cases = paths.Select(x => x.Indices[0]).ToList();
        foreach (int caseid in cases) {
          Assert.Equal(index, caseid);
        }

        var permutations = paths.Select(x => x.Indices[1]).ToList();
        foreach (int permutation in permutations) {
          Assert.Equal(caseName.Equals("A") ? 0 : 1, permutation);
        }
      }
    }

    [Theory]
    [InlineData("A", 1)]
    [InlineData("A", 2)]
    [InlineData("C", 1)]
    public void SteelUtilisationsOutputCaseTest(string caseName, int index) {
      // Assemble
      GsaResult result = caseName.Equals("A")
        ? (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignSimple, index)
        : (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignSimple, index, new List<int>() { 1 });

      var comp = new SteelUtilisations();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, _memberList, 1);

      List<List<double?>> expectedOutput = GetExpectedOutput(caseName, index);

      for (int i = 0; i < expectedOutput.Count; i++) {
        var outputResult = ComponentTestHelper.GetResultOutputAllData(comp, i).Select(x => ((GH_UnitNumber)x).Value?.Value).ToList();

        Assert.Equal(expectedOutput[i].Count, outputResult.Count);

        for (int j = 0; j < expectedOutput[i].Count; j++) {
          Assert.Equal(expectedOutput[i][j],
            outputResult[j] == null ? outputResult[j] :
              ResultHelper.RoundToSignificantDigits((double)outputResult[j], 4));
        }
      }
    }

    [Theory]
    [InlineData("A", 1)]
    [InlineData("A", 2)]
    [InlineData("C", 1)]
    public void SteelUtilisationsMinFromCaseTest(string caseName, int index) {
      // Assemble
      GsaResult result = caseName.Equals("A")
        ? (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignSimple, index)
        : (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignSimple, index, new List<int>() { 1 });

      List<List<double?>> expectedOutput = GetExpectedOutput(caseName, index);

      for (int i = 0; i < expectedOutput.Count; i++) {
        var values = new List<double?>();
        values.AddRange(expectedOutput[i]);
        double? expected = MaxMinHelper.Min(values);

        // Act
        var comp = new SteelUtilisations();
        comp.SetSelected(0, 15 + i);
        ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
        ComponentTestHelper.SetInput(comp, _memberList, 1);

        List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, i);

        // Assert Min in set
        if (output.Contains(null)) {
          foreach (IQuantity quantity in output) {
            Assert.Null(quantity);
          }
        } else {
          double min = output.Min().As(Unit());
          Assert.Equal(expected, ResultHelper.RoundToSignificantDigits(min, 4));
        }
      }
    }

    [Theory]
    [InlineData("A", 1)]
    [InlineData("A", 2)]
    [InlineData("C", 1)]
    public void SteelUtilisationsMaxFromCaseTest(string caseName, int index) {
      // Assemble
      GsaResult result = caseName.Equals("A")
        ? (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignSimple, index)
        : (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignSimple, index, new List<int>() { 1 });

      List<List<double?>> expectedOutput = GetExpectedOutput(caseName, index);

      for (int i = 0; i < expectedOutput.Count; i++) {
        var values = new List<double?>();
        values.AddRange(expectedOutput[i]);
        double? expected = MaxMinHelper.Max(values);

        // Act
        var comp = new SteelUtilisations();
        comp.SetSelected(0, i);
        ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
        ComponentTestHelper.SetInput(comp, _memberList, 1);

        List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, i);

        // Assert Max in set
        if (output.Contains(null)) {
          foreach (IQuantity quantity in output) {
            Assert.Null(quantity);
          }
        } else {
          double max = output.Max().As(Unit());
          Assert.Equal(expected, ResultHelper.RoundToSignificantDigits(max, 4));
        }
      }
    }

    private Enum Unit() {
      Enum unit = Ratio.BaseUnit;

      return unit;
    }

    private static List<List<double?>> GetExpectedOutput(string caseName, int index) {
      var expectedOutput = new List<List<double?>>();
      switch (caseName) {
        case "A" when index == 1:
          expectedOutput = ExpectedA1Output();
          break;
        case "A" when index == 2:
          expectedOutput = ExpectedA2Output();
          break;
        case "C" when index == 1:
          expectedOutput = ExpectedC1Output();
          break;
        default: break;
      }

      return expectedOutput;
    }

    private static List<List<double?>> ExpectedA1Output() {
      var expectedOutput = new List<List<double?>>() {
        SteelUtilisationsA1.Overall,
        SteelUtilisationsA1.LocalCombined,
        SteelUtilisationsA1.BucklingCombined,
        SteelUtilisationsA1.LocalAxis,
        SteelUtilisationsA1.LocalSu,
        SteelUtilisationsA1.LocalSv,
        SteelUtilisationsA1.LocalTorsion,
        SteelUtilisationsA1.LocalMuu,
        SteelUtilisationsA1.LocalMvv,
        SteelUtilisationsA1.BucklingUu,
        SteelUtilisationsA1.BucklingVv,
        SteelUtilisationsA1.BucklingLt,
        SteelUtilisationsA1.BucklingTor,
        SteelUtilisationsA1.BucklingFt,
      };
      return expectedOutput;
    }
    private static List<List<double?>> ExpectedA2Output() {
      var expectedOutput = new List<List<double?>>() {
        SteelUtilisationsA2.Overall,
        SteelUtilisationsA2.LocalCombined,
        SteelUtilisationsA2.BucklingCombined,
        SteelUtilisationsA2.LocalAxis,
        SteelUtilisationsA2.LocalSu,
        SteelUtilisationsA2.LocalSv,
        SteelUtilisationsA2.LocalTorsion,
        SteelUtilisationsA2.LocalMuu,
        SteelUtilisationsA2.LocalMvv,
        SteelUtilisationsA2.BucklingUu,
        SteelUtilisationsA2.BucklingVv,
        SteelUtilisationsA2.BucklingLt,
        SteelUtilisationsA2.BucklingTor,
        SteelUtilisationsA2.BucklingFt,
      };
      return expectedOutput;
    }
    private static List<List<double?>> ExpectedC1Output() {
      var expectedOutput = new List<List<double?>>() {
        SteelUtilisationsC1.Overall,
        SteelUtilisationsC1.LocalCombined,
        SteelUtilisationsC1.BucklingCombined,
        SteelUtilisationsC1.LocalAxis,
        SteelUtilisationsC1.LocalSu,
        SteelUtilisationsC1.LocalSv,
        SteelUtilisationsC1.LocalTorsion,
        SteelUtilisationsC1.LocalMuu,
        SteelUtilisationsC1.LocalMvv,
        SteelUtilisationsC1.BucklingUu,
        SteelUtilisationsC1.BucklingVv,
        SteelUtilisationsC1.BucklingLt,
        SteelUtilisationsC1.BucklingTor,
        SteelUtilisationsC1.BucklingFt,
      };
      return expectedOutput;
    }

  }
}
