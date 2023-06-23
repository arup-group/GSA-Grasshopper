using Grasshopper.Kernel;
using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysGH.Components;
using Xunit;

namespace GsaGHTests.Model {
  [Collection("GrasshopperFixture collection")]
  public class CreateListTest {

    public static GH_OasysDropDownComponent ComponentMother() {
      var comp = new CreateList();
      comp.CreateAttributes();
      return comp;
    }

    [Theory]
    [InlineData(0)] // node
    [InlineData(1)] // element
    [InlineData(2)] // member
    [InlineData(3)] // case
    public void TestErrorMessages(int i) {
      GH_OasysDropDownComponent comp = ComponentMother();

      // Act
      comp.SetSelected(0, i); // change dropdown
      // set (invalid) definition input
      ComponentTestHelper.SetInput(comp, "-5", 2);
      var output = (GsaListGoo)ComponentTestHelper.GetOutput(comp);

      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Blank));
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Remark));
      Assert.Single(comp.RuntimeMessages(GH_RuntimeMessageLevel.Warning));
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }
  }
}
