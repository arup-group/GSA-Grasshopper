using System.IO;
using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Xunit;

namespace GsaGHTests.Model {
  [Collection("GrasshopperFixture collection")]
  public class SaveGsaModelTests {
    [Theory]
    [InlineData("GSA-Grasshopper_temp2.gwa")]
    [InlineData("GSA-Grasshopper_temp2.gwb")]
    public void SaveGsaModelTest(string path) {
      var comp = new SaveGsaModel();
      comp.CreateAttributes();
      ComponentTestHelper.SetInput(comp, ModelTests.GsaModelGooMother);
      ComponentTestHelper.SetInput(comp, true, 1);
      string tempfilename = Path.GetTempPath() + path;
      ComponentTestHelper.SetInput(comp, tempfilename, 2);
      var output = (GsaModelGoo)ComponentTestHelper.GetOutput(comp);
      Assert.NotNull(output);
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Warning));
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error));
      string expectedPath = Path.GetTempPath() + "GSA-Grasshopper_temp2.gwa";

      Assert.True(File.Exists(expectedPath));
    }
  }
}
