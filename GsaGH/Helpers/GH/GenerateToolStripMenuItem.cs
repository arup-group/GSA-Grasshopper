using System;
using System.Linq;
using System.Windows.Forms;
using OasysGH.Units.Helpers;

namespace GsaGH.Helpers.GH {
  internal static class GenerateToolStripMenuItem {
    public static ToolStripMenuItem GetSubMenuItem(
      string name, EngineeringUnits units, string unitString, Action<string> action) {
      var menu = new ToolStripMenuItem(name) {
        Enabled = true,
      };
      foreach (ToolStripMenuItem toolStripMenuItem in UnitsHelper.GetFilteredAbbreviations(units)
       .Select(unit => new ToolStripMenuItem(unit, null, (s, e) => action(unit)) {
          Checked = unit == unitString,
          Enabled = true,
        })) {
        menu.DropDownItems.Add(toolStripMenuItem);
      }

      return menu;
    }
  }
}
