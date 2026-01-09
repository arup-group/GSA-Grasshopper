using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Helpers;

using OasysGH.Components;

using Xunit;

namespace GsaGHTests.Components.Analysis {
  [Collection("GrasshopperFixture collection")]
  public class CreateCombinationCaseTests {

    public static GH_OasysComponent ComponentMother() {
      var comp = new CreateCombinationCase();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(comp, 42, 0);
      ComponentTestHelper.SetInput(comp, "my Combination", 1);
      ComponentTestHelper.SetInput(comp, "1.35A1 + 1.5A2", 2);

      return comp;
    }

    [Fact]
    public void CreateComponentTest() {
      GH_OasysComponent comp = ComponentMother();

      var output = (GsaCombinationCaseGoo)ComponentTestHelper.GetOutput(comp);

      Assert.Equal(42, output.Value.Id);
      Assert.Equal("my Combination", output.Value.Name);
      Assert.Equal("1.35A1 + 1.5A2", output.Value.Definition);
    }
  }
}
