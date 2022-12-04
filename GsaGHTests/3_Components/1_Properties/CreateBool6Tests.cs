using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysGH.Components;
using Xunit;

namespace GsaGHTests.Components.Properties
{
  [Collection("GrasshopperFixture collection")]
  public class CreateBool6Tests
  {
    public static GH_OasysDropDownComponent ComponentMother()
    {
      var comp = new CreateBool6();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(comp, true, 0);
      ComponentTestHelper.SetInput(comp, true, 1);
      ComponentTestHelper.SetInput(comp, true, 2);
      ComponentTestHelper.SetInput(comp, true, 3);
      ComponentTestHelper.SetInput(comp, true, 4);
      ComponentTestHelper.SetInput(comp, true, 5);

      return comp;
    }

    [Fact]
    public void CreateComponent()
    {
      // Arrange & Act
      var comp = ComponentMother();

      // Assert
      GsaBool6Goo output = (GsaBool6Goo)ComponentTestHelper.GetOutput(comp);
      Assert.True(output.Value.X);
      Assert.True(output.Value.Y);
      Assert.True(output.Value.Z);
      Assert.True(output.Value.XX);
      Assert.True(output.Value.YY);
      Assert.True(output.Value.ZZ);
    }
  }
}
