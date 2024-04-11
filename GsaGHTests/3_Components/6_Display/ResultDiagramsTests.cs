﻿using System.Collections.Generic;
using Grasshopper.Kernel;
using GsaGH.Components;
using GsaGH.Parameters;
using GsaGH.Parameters.Results;
using GsaGHTests.Helper;
using GsaGHTests.Helpers;
using GsaGHTests.Parameters;
using Xunit;

namespace GsaGHTests.Components.Display {
  [Collection("GrasshopperFixture collection")]
  public class ResultDiagramsTests {
  [Fact]
    public void CombinationCaseWithMultiplePermutationsMessageTests() {
      var caseResult = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 2, new List<int>() { 1, 2, 3, });

      var comp = new ResultDiagrams();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(caseResult));
      comp.Params.Output[0].CollectData();
      IList<string> messages = comp.RuntimeMessages(GH_RuntimeMessageLevel.Remark);

      Assert.Single(messages);
      Assert.Equal("Combination Case 2 contains 3 permutations and diagrams will show on top of eachother for each permutaion.\r\nTo select a single permutation use the 'Select Results' component.", messages[0]);
    }

    [Fact]
    public void UpdateForceTest() {
      var comp = new ResultDiagrams();
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      comp.SetSelected(0, 1); // force
      comp.SetSelected(1, 0); // Axial force
      comp.UpdateForce("MN");
      comp.Params.Output[0].CollectData();
      Assert.Equal("MN", comp.Message);
    }

    [Fact]
    public void UpdateStressTest() {
      var comp = new ResultDiagrams();
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      comp.SetSelected(0, 2); // stress
      comp.UpdateStress("kPa");
      comp.Params.Output[0].CollectData();
      Assert.Equal("kPa", comp.Message);
    }

    [Fact]
    public void UpdateMomentTest() {
      var comp = new ResultDiagrams();
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      comp.SetSelected(0, 1); // force
      comp.UpdateMoment("MN·m");
      comp.Params.Output[0].CollectData();
      Assert.Equal("MN·m", comp.Message);
    }
  }
}
