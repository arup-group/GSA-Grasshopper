using System;
using System.Reflection;
using GsaGH;
using GsaGH.Components;
using GsaGH.Graphics;
using OasysGH.Components;
using RhinoWindows.Forms;
using Xunit;

namespace GsaGHTests.UI {
  [Collection("GrasshopperFixture collection")]
  public class AboutBoxTests {
    [Fact]
    public void AboutBoxTest() {
      var box = new AboutBox();
      Assert.Equal($"About {"GsaGH"}", box.Text);
      Assert.Equal("Copyright © Oasys 1985 - 2023", box.AssemblyCompany);
      Assert.NotNull(box.AssemblyDescription);
      Assert.NotNull(box.AssemblyTitle);
    }
  }
}
