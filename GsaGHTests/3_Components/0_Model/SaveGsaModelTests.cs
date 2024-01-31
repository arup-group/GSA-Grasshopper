﻿using System.IO;
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
    [InlineData("GSA-Grasshopper1.gwa")]
    [InlineData("GSA-Grasshopper2.gwb")]
    public void SaveGsaModelTest(string filename) {
      string path = Path.GetTempPath() + filename;
      string expectedPath = Path.GetTempPath() + (path.EndsWith("gwc") ? "GSA-Grasshopper3.gwb" : filename);
      
      // clean up existing files
      if(File.Exists(expectedPath) ) {
        File.Delete(expectedPath);
      }

      var comp = new SaveGsaModel();
      comp.CreateAttributes();
      ComponentTestHelper.SetInput(comp, GsaModelGooMother);
      ComponentTestHelper.SetInput(comp, true, 1);
      ComponentTestHelper.SetInput(comp, path, 2);
      var output = (GsaModelGoo)ComponentTestHelper.GetOutput(comp);
      Assert.NotNull(output);
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Warning));
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error));

      Assert.True(File.Exists(expectedPath));
    }
  }
}
