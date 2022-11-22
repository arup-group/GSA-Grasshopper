using Grasshopper.Kernel.Types;
using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysGH.Components;
using Xunit;

namespace GsaGHTests.Components.Geometry
{
  [Collection("GrasshopperFixture collection")]
  public class EditMember3dTests
  {
    public static GH_OasysComponent ComponentMother()
    {
      var comp = new EditMember3d();
      comp.CreateAttributes();

      //ComponentTestHelper.SetInput(comp, ComponentTestHelper.GetOutput(CreateMember1dTests.ComponentMother()), 0);

      return comp;
    }

    [Fact]
    public void CreateComponentTest1()
    {
      // Arrange & Act
      var comp = ComponentMother();

      // Assert
     
    }
  }
}
