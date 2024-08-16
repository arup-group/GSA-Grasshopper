using Grasshopper.Kernel.Types;

using GsaGH.Components;

using GsaGHTests.Helpers;

using Xunit;

namespace GsaGHTests.Components.Geometry {
  [Collection("GrasshopperFixture collection")]
  public class CreateMemberEndRestraintTests {

    [Theory]
    [InlineData(0, "Pinned")]
    [InlineData(1, "Fixed")]
    [InlineData(2, "Free")]
    [InlineData(3, "FullRotational")]
    [InlineData(4, "PartialRotational")]
    [InlineData(5, "TopFlangeLateral")]
    public void ChangeShortcutDropdownTest(int index, string expected) {
      var comp = new CreateMemberEndRestraint();
      comp.CreateAttributes();

      comp.SetSelected(0, index);
      var output = (GH_String)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(expected, output.Value);
    }

    [Fact]
    public void InputsNoneFromTextTest() {
      var comp = new CreateMemberEndRestraint();
      comp.CreateAttributes();
      ComponentTestHelper.SetInput(comp, "None", 0);
      ComponentTestHelper.SetInput(comp, "None", 1);
      ComponentTestHelper.SetInput(comp, "None", 2);
      ComponentTestHelper.SetInput(comp, "None", 3);
      ComponentTestHelper.SetInput(comp, "None", 4);
      ComponentTestHelper.SetInput(comp, "None", 5);
      ComponentTestHelper.SetInput(comp, "None", 6);
      ComponentTestHelper.SetInput(comp, "None", 7);
      ComponentTestHelper.SetInput(comp, "None", 8);

      var output = (GH_String)ComponentTestHelper.GetOutput(comp);
      Assert.Equal("Free", output.Value);
    }

    [Fact]
    public void InputsFullFromTextTest() {
      var comp = new CreateMemberEndRestraint();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(comp, "Full", 0);
      ComponentTestHelper.SetInput(comp, "Full", 1);
      ComponentTestHelper.SetInput(comp, "Full", 2);
      ComponentTestHelper.SetInput(comp, "Full", 3);
      ComponentTestHelper.SetInput(comp, "Full", 4);
      ComponentTestHelper.SetInput(comp, "Full", 5);
      ComponentTestHelper.SetInput(comp, "Full", 6);
      ComponentTestHelper.SetInput(comp, "Full", 7);
      ComponentTestHelper.SetInput(comp, "Full", 8);

      var output = (GH_String)ComponentTestHelper.GetOutput(comp);
      Assert.Equal("Fixed", output.Value);
    }

    [Fact]
    public void InputsPartialFromTextTest() {
      var comp = new CreateMemberEndRestraint();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(comp, "Partial", 0);
      ComponentTestHelper.SetInput(comp, "Partial", 1);
      ComponentTestHelper.SetInput(comp, "Partial", 2);
      ComponentTestHelper.SetInput(comp, "Partial", 3);
      ComponentTestHelper.SetInput(comp, "Partial", 4);

      var output = (GH_String)ComponentTestHelper.GetOutput(comp);
      Assert.Equal("F1P F2P TP MAJP MINP", output.Value);
    }

    [Fact]
    public void InputFrictionFromTextTest() {
      var comp = new CreateMemberEndRestraint();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(comp, "Frictional", 2);

      var output = (GH_String)ComponentTestHelper.GetOutput(comp);
      Assert.Equal("TF", output.Value);
    }
  }
}
