using System.Drawing;
using System.Windows.Forms;

using Grasshopper.Kernel;

using GsaGH.Components;

using OasysUnits.Units;

using Xunit;

namespace GsaGHTests.Model {
  [Collection("GrasshopperFixture collection")]
  public class ToleranceContextMenuTests {
    [Fact]
    public void ChangeToleranceTest() {
      var comp = new CreateModel();
      comp.CreateAttributes();
      ToleranceContextMenu toleranceMenu = comp.ToleranceMenu;
      var form = new ContextMenuStrip();
      toleranceMenu.AppendAdditionalMenuItems(comp, form, LengthUnit.Meter);
      var setTol = (ToolStripMenuItem)form.Items[0];
      setTol.Text = "1 horse";
      toleranceMenu.MaintainText(setTol);
      Assert.Equal(Color.FromArgb(255, 255, 100, 100), setTol.BackColor);
      setTol.Text = "1 cm";
      toleranceMenu.MaintainText(setTol);
      Assert.Equal(Color.FromArgb(255, 180, 255, 150), setTol.BackColor);
      toleranceMenu.UpdateMessage(comp, LengthUnit.Meter);
      Assert.Equal("Tol: 0.01m", comp.Message);

      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Remark));
      setTol.Text = "0.5mm";
      toleranceMenu.MaintainText(setTol);
      toleranceMenu.UpdateMessage(comp, LengthUnit.Meter);
      Assert.Single(comp.RuntimeMessages(GH_RuntimeMessageLevel.Remark));
      setTol.Text = "1 cm";
      toleranceMenu.MaintainText(setTol);
      toleranceMenu.UpdateMessage(comp, LengthUnit.Meter);
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Remark));
      setTol.Text = "0.5m";
      toleranceMenu.MaintainText(setTol);
      toleranceMenu.UpdateMessage(comp, LengthUnit.Meter);
      Assert.Single(comp.RuntimeMessages(GH_RuntimeMessageLevel.Remark));
    }
  }
}
