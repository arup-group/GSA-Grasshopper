using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Helper;
using GsaGHTests.Helpers;

using Xunit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public partial class GsaResultTests {
    public static GsaResultGoo NodeAndElement1dAnalysisCaseResultsMother() {
      var open = new OpenModel();
      open.CreateAttributes();
      string file = GsaFile.SteelDesignComplex;
      ComponentTestHelper.SetInput(open, file);
      var model = (GsaModelGoo)ComponentTestHelper.GetOutput(open);
      var getResults = new GetResult();
      getResults.CreateAttributes();

      ComponentTestHelper.SetInput(getResults, model);

      return (GsaResultGoo)ComponentTestHelper.GetOutput(getResults);
    }

    public static GsaResultGoo NodeAndElement2dAnalysisCaseResultsMother() {
      var open = new OpenModel();
      open.CreateAttributes();
      string file = GsaFile.Element2dSimple;
      ComponentTestHelper.SetInput(open, file);
      var model = (GsaModelGoo)ComponentTestHelper.GetOutput(open);
      var getResults = new GetResult();
      getResults.CreateAttributes();

      ComponentTestHelper.SetInput(getResults, model);

      return (GsaResultGoo)ComponentTestHelper.GetOutput(getResults);
    }

    public static GsaResultGoo NodeAndElement3dAnalysisCaseResultsMother() {
      var open = new OpenModel();
      open.CreateAttributes();
      string file = GsaFile.Element3dSimple;
      ComponentTestHelper.SetInput(open, file);
      var model = (GsaModelGoo)ComponentTestHelper.GetOutput(open);
      var getResults = new GetResult();
      getResults.CreateAttributes();

      ComponentTestHelper.SetInput(getResults, model);

      return (GsaResultGoo)ComponentTestHelper.GetOutput(getResults);
    }

    public static GsaResultGoo NodeAndElement1dCombinationResultsMother() {
      var open = new OpenModel();
      open.CreateAttributes();
      string file = GsaFile.SteelDesignComplex;
      ComponentTestHelper.SetInput(open, file);
      var model = (GsaModelGoo)ComponentTestHelper.GetOutput(open);
      var getResults = new GetResult();
      getResults.CreateAttributes();

      ComponentTestHelper.SetInput(getResults, model);
      ComponentTestHelper.SetInput(getResults, "C", 1);

      return (GsaResultGoo)ComponentTestHelper.GetOutput(getResults);
    }

    public static GsaResultGoo NodeAndElement2dCombinationResultsMother() {
      var open = new OpenModel();
      open.CreateAttributes();
      string file = GsaFile.Element2dSimple;
      ComponentTestHelper.SetInput(open, file);
      var model = (GsaModelGoo)ComponentTestHelper.GetOutput(open);
      var getResults = new GetResult();
      getResults.CreateAttributes();

      ComponentTestHelper.SetInput(getResults, model);
      ComponentTestHelper.SetInput(getResults, "C", 1);

      return (GsaResultGoo)ComponentTestHelper.GetOutput(getResults);
    }

    public static GsaResultGoo FabricModelResultsMother() {
      var open = new OpenModel();
      open.CreateAttributes();
      string file = GsaFile.FabricMaterialModel;
      ComponentTestHelper.SetInput(open, file);
      var model = (GsaModelGoo)ComponentTestHelper.GetOutput(open);
      var getResults = new GetResult();
      getResults.CreateAttributes();

      ComponentTestHelper.SetInput(getResults, model);
      ComponentTestHelper.SetInput(getResults, "A1", 1);

      return (GsaResultGoo)ComponentTestHelper.GetOutput(getResults);
    }

    public static GsaResultGoo NodeAndElement3dCombinationResultsMother() {
      var open = new OpenModel();
      open.CreateAttributes();
      string file = GsaFile.Element3dSimple;
      ComponentTestHelper.SetInput(open, file);
      var model = (GsaModelGoo)ComponentTestHelper.GetOutput(open);
      var getResults = new GetResult();
      getResults.CreateAttributes();

      ComponentTestHelper.SetInput(getResults, model);
      ComponentTestHelper.SetInput(getResults, "C", 1);

      return (GsaResultGoo)ComponentTestHelper.GetOutput(getResults);
    }

    public static GsaResultGoo NodeAndElement1dFootfallResultsMother() {
      var open = new OpenModel();
      open.CreateAttributes();
      string file = GsaFile.SteelDesignComplex;
      ComponentTestHelper.SetInput(open, file);
      var model = (GsaModelGoo)ComponentTestHelper.GetOutput(open);
      var getResults = new GetResult();
      getResults.CreateAttributes();

      ComponentTestHelper.SetInput(getResults, model);
      ComponentTestHelper.SetInput(getResults, "A", 1);
      ComponentTestHelper.SetInput(getResults, 13, 2);

      return (GsaResultGoo)ComponentTestHelper.GetOutput(getResults);
    }

    public static GsaResultGoo NodeAndElement2dFootfallResultsMother() {
      var open = new OpenModel();
      open.CreateAttributes();
      string file = GsaFile.Element2dSimple;
      ComponentTestHelper.SetInput(open, file);
      var model = (GsaModelGoo)ComponentTestHelper.GetOutput(open);
      var getResults = new GetResult();
      getResults.CreateAttributes();

      ComponentTestHelper.SetInput(getResults, model);
      ComponentTestHelper.SetInput(getResults, "A", 1);
      ComponentTestHelper.SetInput(getResults, 13, 2);

      return (GsaResultGoo)ComponentTestHelper.GetOutput(getResults);
    }
  }
}
