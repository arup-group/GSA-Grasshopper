using System.IO;
using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helper;
using GsaGHTests.Helpers;
using OasysGH.Components;
using Xunit;

namespace GsaGHTests.Model {
  [Collection("GrasshopperFixture collection")]
  public class SaveGsaModelTests {
    public static GsaModelGoo GsaModelGooMother => (GsaModelGoo)ComponentTestHelper.GetOutput(SaveGsaModelMother());

    public static GH_OasysComponent SaveGsaModelMother() {
      var comp = new OpenModel();
      comp.CreateAttributes();

      string file = GsaFile.Element2dMultiPropsParentMember;
      ComponentTestHelper.SetInput(comp, file);

      return comp;
    }

    [Theory]
    [InlineData("GSA-Grasshopper_temp2.gwa")]
    [InlineData("GSA-Grasshopper_temp2.gwb")]
    public void SaveGsaModelTest(string path) {
      var comp = new SaveGsaModel();
      comp.CreateAttributes();
      ComponentTestHelper.SetInput(comp, GsaModelGooMother);
      ComponentTestHelper.SetInput(comp, true, 1);
      string tempfilename = Path.GetTempPath() + path;
      ComponentTestHelper.SetInput(comp, tempfilename, 2);
      var output = (GsaModelGoo)ComponentTestHelper.GetOutput(comp);
      Assert.NotNull(output);
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Warning));
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error));
      string expectedPath = Path.GetTempPath() + path;

      Assert.True(File.Exists(expectedPath));
    }
  }
}
