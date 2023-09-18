using Grasshopper.GUI;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using GsaGH.Properties;
using OasysGH.Units;
using OasysUnits;
using OasysUnits.Units;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace GsaGH.Components {
  internal class ToleranceMenu {
    internal LengthUnit LengthUnit = DefaultUnits.LengthUnitGeometry;
    internal Length Tolerance = DefaultUnits.Tolerance;
    internal string Text = string.Empty;
    internal GH_Component Component;

    internal ToleranceMenu(GH_Component owner) {
      Component = owner;
    }

    internal void AppendAdditionalMenuItems(ToolStripDropDown menu) {
      if (!(menu is ContextMenuStrip)) {
        return; // this method is also called when clicking EWR balloon
      }

      GH_DocumentObject.Menu_AppendSeparator(menu);

      var tolerance = new ToolStripTextBox();
      Text = Tolerance.ToUnit(LengthUnit).ToString().Replace(" ", string.Empty);
      tolerance.Text = Text;
      tolerance.BackColor = Color.FromArgb(255, 180, 255, 150);
      tolerance.TextChanged += (s, e) => MaintainText(tolerance);

      var toleranceMenu = new ToolStripMenuItem("Set Tolerance", Resources.ModelUnits) {
        Enabled = true,
        ImageScaling = ToolStripItemImageScaling.SizeToFit,
      };

      //only for init submenu
      var useless = new GH_MenuCustomControl(toleranceMenu.DropDown, tolerance.Control, true, 200);
      toleranceMenu.DropDownItems[1].MouseUp += (s, e) => {
        UpdateMessage();
        (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
        Component.ExpireSolution(true);
      };
      menu.Items.Add(toleranceMenu);

      GH_DocumentObject.Menu_AppendSeparator(menu);

      (Component as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Component.ExpireSolution(true);
    }

    internal void MaintainText(ToolStripItem tolerance) {
      Text = tolerance.Text;
      tolerance.BackColor = Length.TryParse(Text, out Length _) ?
        Color.FromArgb(255, 180, 255, 150) : Color.FromArgb(255, 255, 100, 100);
    }

    internal void UpdateMessage() {
      if (Text != string.Empty) {
        try {
          Tolerance = Length.Parse(Text);
        }
        catch (Exception e) {
          MessageBox.Show(e.Message);
          return;
        }
      }

      Tolerance = Tolerance.ToUnit(LengthUnit);
      Component.Message = "Tol: " + Tolerance.ToString().Replace(" ", string.Empty);
      if (Tolerance.Meters < 0.001) {
        Component.AddRuntimeRemark(
          "Set tolerance is quite small, you can change this by right-clicking the component.");
      }
      else if (Tolerance.Meters > 0.25) {
        Component.AddRuntimeRemark(
          "Set tolerance is quite large, you can change this by right-clicking the component.");
      }
      else {
        ClearToleranceRuntimeRemarkMessages();
      }
    }

    private void ClearToleranceRuntimeRemarkMessages() {
      var remarks = Component.RuntimeMessages(GH_RuntimeMessageLevel.Remark).ToList();
      var warnings = Component.RuntimeMessages(GH_RuntimeMessageLevel.Warning).ToList();
      var errors = Component.RuntimeMessages(GH_RuntimeMessageLevel.Error).ToList();
      Component.ClearRuntimeMessages();
      foreach (string remark in remarks) {
        if (!remark.StartsWith("Set tolerance is quite ")) {
          Component.AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, remark);
        }
      }
      foreach (string warning in warnings) {
        Component.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, warning);
      }
      foreach (string error in errors) {
        Component.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, error);
      }
    }
  }
}
