using System;

using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Helper;
using GsaGHTests.Helpers;

using OasysGH.Components;

using Xunit;

namespace GsaGHTests.Model {
  [Collection("GrasshopperFixture collection")]
  public class ModelTests {
    public static GsaModelGoo GsaModelGooMother
      => (GsaModelGoo)ComponentTestHelper.GetOutput(OpenModelComponentMother());

    public static GH_OasysComponent OpenModelComponentMother() {
      var comp = new OpenModel();
      comp.CreateAttributes();

      string file = GsaFile.SteelDesignSimple;
      ComponentTestHelper.SetInput(comp, file);

      return comp;
    }

    [Fact]
    public void OpenComponentTest() {
      GH_OasysComponent comp = OpenModelComponentMother();
      var output = (GsaModelGoo)ComponentTestHelper.GetOutput(comp);

      GsaModel model = output.Value;

      Assert.Equal(GsaFile.SteelDesignSimple, model.FileNameAndPath);
      Assert.NotEqual(new Guid(), model.Guid);
    }
  }
}
