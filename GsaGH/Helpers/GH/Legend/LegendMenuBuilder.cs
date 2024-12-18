using System;
using System.Windows.Forms;

using Grasshopper.GUI;
using Grasshopper.Kernel;

using GsaGH.Helpers.GH;

using OasysGH.Components;

namespace GsaGH.Helpers {
  public class LegendMenuBuilder {
    private string _scaleLegendTxt;
    public delegate void SetLegendScaleDelegate(double scale);

    public LegendMenuBuilder() { }

    public ToolStripMenuItem CreateLegendToolStripMenuItem(
      GH_OasysDropDownComponent component, Action updateUI, SetLegendScaleDelegate setLegendScaleDelegate,
      double currentScale) {
      var legendScaleTextBox = new ToolStripTextBox {
        Text = currentScale.ToString(),
      };

      legendScaleTextBox.TextChanged += (s, e) => MaintainScaleLegendText(legendScaleTextBox);

      var legendScaleMenuItem = new ToolStripMenuItem("Scale Legend") {
        Enabled = true,
        ImageScaling = ToolStripItemImageScaling.SizeToFit,
      };

      var menuControl
        = new GH_MenuCustomControl(legendScaleMenuItem.DropDown, legendScaleTextBox.Control, true,
          200); // needed! don't remove

      legendScaleMenuItem.DropDownItems[1].MouseUp += (s, e) => {
        UpdateLegendScale(component, updateUI, setLegendScaleDelegate);
        (component as IGH_VariableParameterComponent).VariableParameterMaintenance();
        component.ExpireSolution(true);
      };

      return legendScaleMenuItem;
    }

    private void UpdateLegendScale(
      GH_OasysDropDownComponent component, Action updateUI, SetLegendScaleDelegate setLegendScaleDelegate) {
      try {
        double newScale = double.Parse(_scaleLegendTxt);
        setLegendScaleDelegate(newScale);
      } catch (Exception) {
        component.AddRuntimeWarning("Invalid scale value. Please enter a valid number.");
      }

      component.ExpirePreview(true);
      updateUI();
    }

    private void MaintainScaleLegendText(ToolStripItem menuitem) {
      _scaleLegendTxt = menuitem.Text;
    }
  }
}
