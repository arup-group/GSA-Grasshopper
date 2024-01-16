using System.IO;
using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Xunit;

namespace GsaGHTests.Model {
  [Collection("GrasshopperFixture collection")]
  public class SaveGsaModelTests {
    [Theory]
    [InlineData("GSA-Grasshopper1.gwa")]
    [InlineData("GSA-Grasshopper2.gwb")]
    [InlineData("GSA-Grasshopper3.gwc")]
    public void SaveGsaModelTest(string filename) {
      var comp = new SaveGsaModel();
      comp.CreateAttributes();
      ComponentTestHelper.SetInput(comp, ModelTests.GsaModelGooMother);
      ComponentTestHelper.SetInput(comp, true, 1);
      string path = Path.GetTempPath() + filename;
      ComponentTestHelper.SetInput(comp, path, 2);
      var output = (GsaModelGoo)ComponentTestHelper.GetOutput(comp);
      Assert.NotNull(output);
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Warning));
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error));
      string expectedPath = Path.GetTempPath() + (path.EndsWith("gwc") ? "GSA-Grasshopper3.gwa" : filename);

      Assert.True(File.Exists(expectedPath));
    }
  }
}
