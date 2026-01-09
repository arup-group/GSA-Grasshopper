using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Components.Properties;
using GsaGHTests.Helpers;

using OasysGH.Components;

using Xunit;

namespace GsaGHTests.Components.Analysis {
  [Collection("GrasshopperFixture collection")]
  public class CreateSteelSectionPoolTests {

    public static GH_OasysComponent ComponentMother() {
      var comp = new CreateSteelSectionPool();
      comp.CreateAttributes();

      var section = (GsaSectionGoo)ComponentTestHelper.GetOutput(
        CreateSectionTests.ComponentMother("STD CH(mm) 40 30 2 1"));

      ComponentTestHelper.SetInput(comp, 7, 0);
      ComponentTestHelper.SetInput(comp, section, 1);

      return comp;
    }

    [Fact]
    public void CreateComponent() {
      GH_OasysComponent comp = ComponentMother();

      var output = (GsaSectionGoo)ComponentTestHelper.GetOutput(ComponentMother());
      Assert.Equal(7, output.Value.ApiSection.Pool);
    }

    [Fact]
    public void TestDuplicate() {
      var comp = new CreateSteelSectionPool();
      comp.CreateAttributes();

      var section = (GsaSectionGoo)ComponentTestHelper.GetOutput(
        CreateSectionTests.ComponentMother("STD CH(mm) 40 30 2 1"));
      Assert.Equal(0, section.Value.ApiSection.Pool);

      ComponentTestHelper.SetInput(comp, 1, 0);
      ComponentTestHelper.SetInput(comp, section, 1);

      var output = (GsaSectionGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(0, section.Value.ApiSection.Pool);
      Assert.Equal(1, output.Value.ApiSection.Pool);
    }
  }
}
