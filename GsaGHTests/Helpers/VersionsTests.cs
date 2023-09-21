using System;
using System.Drawing;
using GsaGH.Graphics;
using GsaGH.Helpers;
using GsaGH.Properties;
using Xunit;

namespace GsaGHTests.Helpers {
  [Collection("GrasshopperFixture collection")]
  public class VersionsTests {

    [Fact]
    public void CheckTest() {
      Versions.CheckAndShowDialogue();
      Assert.True(true);
    }

    [Fact]
    public void IsPluginOutdatedTest() {
      Assert.False(Versions.IsPluginOutdated(GsaGH.GsaGhInfo.guid));
    }

    [Theory]
    [InlineData("0.0.1", true)]
    [InlineData("2.0", false)]
    public void IsVersionOutdatedTest(string version, bool expected) {
      var v = new Version(version);
      Assert.Equal(expected, Versions.IsVersionOutdated(v));
    }

    [Fact]
    public void CheckAdSecIsOutdatedTest() {
      UpdatePluginsBox box = Versions.CreatePluginUpdateDialogue(true, false);

      string text = "An update is available for AdSecGH Plugin.\n\nClick OK to update now.";
      string header = "Update AdSec";
      Bitmap expectedIcon = Resources.AdSecGHUpdate;

      TestUpdatePluginsBox(box, header, text, expectedIcon);
    }

    [Fact]
    public void CheckComposIsOutdatedTest() {
      UpdatePluginsBox box = Versions.CreatePluginUpdateDialogue(false, true);

      string text = "An update is available for ComposGH Plugin.\n\nClick OK to update now.";
      string header = "Update Compos";
      Bitmap expectedIcon = Resources.ComposGHUpdate;

      TestUpdatePluginsBox(box, header, text, expectedIcon);
    }

    [Fact]
    public void CheckBothAreOutdatedTest() {
      UpdatePluginsBox box = Versions.CreatePluginUpdateDialogue(true, true);

      string text = "Updates are avaiable for AdSecGH and ComposGH.\n\nClick OK to update now.";
      string header = "Update Oasys Plugins";
      Bitmap expectedIcon = Resources.OasysGHUpdate;

      TestUpdatePluginsBox(box, header, text, expectedIcon);
    }

    private static void TestUpdatePluginsBox(
      UpdatePluginsBox box, string header, string text, Bitmap expectedIcon) {
      Assert.Equal(header, box.Text);
      Assert.Equal(text, box.textBox.Text);
      var icon = (Bitmap)box.pictureBox.Image;
      Assert.Equal(expectedIcon.RawFormat.Guid, icon.RawFormat.Guid);
    }
  }
}
