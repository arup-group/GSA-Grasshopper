using GsaGH.Graphics;

using Xunit;

namespace GsaGHTests.UI {
  [Collection("GrasshopperFixture collection")]
  public class AboutBoxTests {
    [Fact]
    public void AboutBoxTest() {
      var box = new AboutBox();
      Assert.Equal($"About GSA Grasshopper plugin", box.Text);
      Assert.Equal("Copyright © Oasys 1985 - 2025", box.AssemblyCompany);
      Assert.NotNull(box.AssemblyDescription);
      Assert.NotNull(box.AssemblyTitle);
    }
  }
}
