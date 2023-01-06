using System;
using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helper;
using GsaGHTests.Helpers;
using OasysGH.Components;
using Xunit;

namespace GsaGHTests.Model
{
  [Collection("GrasshopperFixture collection")]
  public class ModelTests
  {
    public static GsaModelGoo GsaModelGooMother
    {
      get
      {
        return (GsaModelGoo)ComponentTestHelper.GetOutput(OpenModelComponentMother());
      }
    }
    
    public static GH_OasysComponent OpenModelComponentMother()
    {
      // create the component
      var comp = new OpenModel();
      comp.CreateAttributes();

      // input parameter
      string file = GsaFile.Steel_Design_Simple;
      ComponentTestHelper.SetInput(comp, file);

      return comp;
    }

    [Fact]
    public void OpenComponentTest()
    {
      var comp = OpenModelComponentMother();

      // Get output from component
      GsaModelGoo output = (GsaModelGoo)ComponentTestHelper.GetOutput(comp);

      // cast from -goo to Gsa-GH data type
      GsaModel model = new GsaModel();
      output.CastTo(ref model);

      Assert.Equal(GsaFile.Steel_Design_Simple, model.FileNameAndPath);
      Assert.NotEqual(new Guid(), model.Guid);
    }
  }
}
