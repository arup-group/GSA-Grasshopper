using System;
using System.Linq;
using System.Windows.Forms;

using GsaGH.Parameters.Results;

using OasysGH.Units.Helpers;

namespace GsaGH.Helpers.GH {
  internal static class GenerateToolStripMenuItem {
    public static ToolStripMenuItem GetSubMenuItem(
      string name, EngineeringUnits units, string unitString, Action<string> action) {
      if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(unitString)) {
        return null;
      }

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

    public static ToolStripMenuItem GetEnvelopeSubMenuItem(
      EnvelopeMethod current, Action<string> action) {
      var menu = new ToolStripMenuItem("Envelope Method") {
        Enabled = true,
      };
      foreach (ToolStripMenuItem toolStripMenuItem in Enum.GetNames(typeof(EnvelopeMethod))
       .Select(type => new ToolStripMenuItem(type, null, (s, e) => action(type)) {
         Checked = type == current.ToString(),
         Enabled = true,
       })) {
        menu.DropDownItems.Add(toolStripMenuItem);
      }

      return menu;
    }
  }
}
