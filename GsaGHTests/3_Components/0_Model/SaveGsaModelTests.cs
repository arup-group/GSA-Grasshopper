using System.IO;
using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Xunit;

namespace GsaGHTests.Model {
  [Collection("GrasshopperFixture collection")]
  public class SaveGsaModelTests {
    [Fact]
    public void SaveGsaModelTest() {
      var comp = new SaveGsaModel();
      comp.CreateAttributes();
      ComponentTestHelper.SetInput(comp, ModelTests.GsaModelGooMother);
      ComponentTestHelper.SetInput(comp, true, 1);
      string tempfilename = Path.GetTempPath() + "GSA-Grasshopper_temp2.gwb";
      ComponentTestHelper.SetInput(comp, tempfilename, 2);
      var output = (GsaModelGoo)ComponentTestHelper.GetOutput(comp);
      Assert.NotNull(output);
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Warning));
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error));
      Assert.True(File.Exists(tempfilename));
    }
  }
}
