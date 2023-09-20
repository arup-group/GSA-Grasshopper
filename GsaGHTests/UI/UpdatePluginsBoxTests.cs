using GsaGH.Graphics;
using GsaGH.Properties;
using System.Drawing;
using System.Reflection;
using System.Resources;
using Xunit;

namespace GsaGHTests.UI {
  [Collection("GrasshopperFixture collection")]
  public class UpdatePluginsBoxTests {
    [Fact]
    public void UpdatePluginsBoxTest() {
      string process = @"rhino://package/search?name=oasys";
      string text = "Updates are avaiable for AdSecGH and ComposGH";
      string header = "Update Oasys Plugins";
      Bitmap expectedIcon = Resources.OasysGHUpdate;

      var box = new UpdatePluginsBox(header, text, process, expectedIcon);
      Assert.Equal(header, box.Text);
      Assert.Equal(text, box.textBox.Text);

      // Test component icon is equal to class name
      ResourceManager rm = Resources.ResourceManager;
      // Find icon with expected name in resources
      string className = "OasysGHUpdate";
      var iconExpected = (Bitmap)rm.GetObject(className);
      Assert.True(iconExpected != null, $"{className} not found in resources");
      var icon = (Bitmap)box.pictureBox.Image;
      Assert.Equal(iconExpected.RawFormat.Guid, icon.RawFormat.Guid);
    }
  }
}
