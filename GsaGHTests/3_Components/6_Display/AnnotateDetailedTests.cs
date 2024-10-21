using System.Collections.Generic;

using GsaGH.Components;

using OasysGH.UI;

using Xunit;

namespace GsaGHTests.Components.Display {
  [Collection("GrasshopperFixture collection")]
  public class AnnotateDetailedTests {
    [Fact]
    public void DefaultDropSelectionsTest() {
      var comp = new AnnotateDetailed();
      Assert.Equal("Regular Dot", comp.SelectedItems[0]);

      comp.SetSelected(0, 1);
      Assert.Equal("TextTag 3D", comp.SelectedItems[0]);
    }

    [Fact]
    public void SetCheckBoxesTest() {
      var comp = new AnnotateDetailed();
      comp.SetCheckBoxes(new List<bool> {
        false, false, false, false }
      );
      Assert.NotNull((DropDownCheckBoxesComponentAttributes)comp.Attributes);
      comp.SetCheckBoxes(new List<bool> {
        true, true, true, true }
      );
      Assert.NotNull((DropDownCheckBoxesComponentAttributes)comp.Attributes);
    }
  }
}
