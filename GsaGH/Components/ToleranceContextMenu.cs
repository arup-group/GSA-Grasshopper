using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using Grasshopper.GUI;
using Grasshopper.Kernel;

using GsaGH.Helpers.GH;
using GsaGH.Properties;

using OasysGH.Units;

using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components {
  internal class ToleranceContextMenu {
    internal Length Tolerance = DefaultUnits.Tolerance;
    internal string Text = string.Empty;

    internal ToleranceContextMenu() { }

    internal void AppendAdditionalMenuItems(GH_Component owner, ToolStripDropDown menu, LengthUnit unit) {
      if (!(menu is ContextMenuStrip)) {
        return; // this method is also called when clicking EWR balloon
      }

      GH_DocumentObject.Menu_AppendSeparator(menu);

      var tolerance = new ToolStripTextBox();
      Text = Tolerance.ToUnit(unit).ToString().Replace(" ", string.Empty);
      tolerance.Text = Text;
      tolerance.BackColor = Color.FromArgb(255, 180, 255, 150);
      tolerance.TextChanged += (s, e) => MaintainText(tolerance);

      var toleranceMenu = new ToolStripMenuItem("Calculation Tolerance", Resources.ModelUnits) {
        Enabled = true,
        ImageScaling = ToolStripItemImageScaling.SizeToFit,
        ToolTipText = "Set tolerance values for various operations, including:"
          + "\n- Collapsing coincident nodes for components like AnalyseModel, CreateModel, CreateElementsFromMembers, and Create2dElementsFromBrep."
          + "\n- Calculating positions for EllipseHollowProfile and CircleHollowProfile in ExpandBeamToShell."
          + "\n- Converting non-planar quadrilaterals to triangles in Create2dElementsFromBrep."
          + "\n\nNote: These tolerances do not affect the preference tolerance in GSA.",
        //ToolTipText = "Set the tolerance for operations, including:"
        //  + "\nCalculating coincident node collapses for components such as:"
        //  + "\n\tAnalyseModel, CreateModel, CreateElementsFromMembers, and Create2dElementsFromBrep."
        //  + "\nAdditionally, calculate the positions of EllipseHollowProfile and CircleHollowProfile for the ExpandBeamToShell component,"
        //  + "\nor convert non-planar quadrilaterals to triangles, and calculate positions for the Create2dElementsFromBrep component. "
        //  + "\n\nNote that none of these tolerances are used to set preference tolerance in GSA.",
      };

      //only for init submenu
      var useless = new GH_MenuCustomControl(toleranceMenu.DropDown, tolerance.Control, true, 200);
      toleranceMenu.DropDownItems[1].MouseUp += (s, e) => {
        UpdateMessage(owner, unit);
        (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
        owner.ExpireSolution(true);
      };
      menu.Items.Add(toleranceMenu);

      GH_DocumentObject.Menu_AppendSeparator(menu);

      (owner as IGH_VariableParameterComponent).VariableParameterMaintenance();
    }

    internal void MaintainText(ToolStripItem tolerance) {
      Text = tolerance.Text;
      tolerance.BackColor = Length.TryParse(Text, out Length _) ?
        Color.FromArgb(255, 180, 255, 150) : Color.FromArgb(255, 255, 100, 100);
    }

    internal void UpdateMessage(GH_Component owner, LengthUnit unit) {
      if (Text != string.Empty) {
        try {
          Tolerance = Length.Parse(Text);
        } catch (Exception e) {
          MessageBox.Show(e.Message);
          return;
        }
      }

      Tolerance = Tolerance.ToUnit(unit);
      owner.Message = "Tol: " + Tolerance.ToString().Replace(" ", string.Empty);
      if (Tolerance.Meters < 0.001) {
        owner.AddRuntimeRemark(
          "Set tolerance is quite small, you can change this by right-clicking the component.");
      } else if (Tolerance.Meters > 0.25) {
        owner.AddRuntimeRemark(
          "Set tolerance is quite large, you can change this by right-clicking the component.");
      } else {
        ClearToleranceRuntimeRemarkMessages(owner);
      }
    }

    private void ClearToleranceRuntimeRemarkMessages(GH_Component owner) {
      var remarks = owner.RuntimeMessages(GH_RuntimeMessageLevel.Remark).ToList();
      var warnings = owner.RuntimeMessages(GH_RuntimeMessageLevel.Warning).ToList();
      var errors = owner.RuntimeMessages(GH_RuntimeMessageLevel.Error).ToList();
      owner.ClearRuntimeMessages();
      foreach (string remark in remarks) {
        if (!remark.StartsWith("Set tolerance is quite ")) {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, remark);
        }
      }
      foreach (string warning in warnings) {
        owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, warning);
      }
      foreach (string error in errors) {
        owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, error);
      }
    }
  }
}
