using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GsaApi;
using GsaGH.Parameters;
using GsaGH.Parameters.Results;
using GsaGHTests._1_BaseParameters._5_Results.Collections.RegressionValues;
using GsaGHTests.Helper;
using GsaGHTests.Helpers;
using GsaGHTests.Parameters.Results;
using OasysUnits;
using OasysUnits.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using OasysGH.Parameters;
using Xunit;
using SteelUtilisations = GsaGH.Components.SteelUtilisations;

namespace GsaGHTests.Components.Results {
  [Collection("GrasshopperFixture collection")]
  public class SteelUtilisationsTests {
    private static string _memberList = "250 to 260";

    [Fact]
    public void InvalidInputErrorTests() {
      var comp = new SteelUtilisations();
      ComponentTestHelper.SetInput(comp, "not a result");
      comp.Params.Output[0].CollectData();
      Assert.True((int)comp.RuntimeMessageLevel >= 10);
    }

    [Fact]
    public void SteelUtilisationsIdsFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelFootfall, 1);

      // Act
      var comp = new SteelUtilisations();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));

      for (int i = 0; i < comp.Params.Output.Count; i++) { // loop through each output
        IList<GH_Path> paths = ComponentTestHelper.GetPathOutput(comp, i);
        Assert.Equal(43, paths.Count);

        var cases = paths.Select(x => x.Indices[0]).ToList();
        foreach (int caseid in cases) {
          Assert.Equal(1, caseid);
        }

        var permutations = paths.Select(x => x.Indices[1]).ToList();
        foreach (int permutation in permutations) {
          Assert.Equal(0, permutation);
        }
      }
    }

    [Fact]
    public void SteelUtilisationsOutputAnalysisCaseTest() {
        // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelFootfall, 1);

      var comp = new SteelUtilisations();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, _memberList, 1);

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
  }
}
