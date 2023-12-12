using Grasshopper.Kernel;
using GsaGH.Components;
using GsaGH.Parameters.Results;
using GsaGH.Parameters;
using GsaGHTests.Helper;
using GsaGHTests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GsaGHTests.Parameters;
using Xunit;

namespace GsaGHTests.Components.Display {
  [Collection("GrasshopperFixture collection")]
  public class ResultDiagramTests {
    [Fact]
    public void CombinationCaseWithMultiplePermutationsMessageTests() {
      var caseResult = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 2, new List<int>() { 1, 2, 3, });

      var comp = new ResultDiagrams();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(caseResult));
      comp.Params.Output[0].CollectData();
      IList<string> messages = comp.RuntimeMessages(GH_RuntimeMessageLevel.Warning);

      Assert.Single(messages);
      Assert.Equal("Combination Case 2 contains 3 permutations - only one permutation can be displayed at a time.\r\nDisplaying first permutation; please use the 'Select Results' to select other single permutations", messages[0]);
    }

  }
}
