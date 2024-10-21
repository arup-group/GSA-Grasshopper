using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

using Grasshopper;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;

using GsaGH.Properties;

namespace GsaGH.Graphics.Menu {
  public class MenuLoad {
    private static ToolStripMenuItem oasysMenu;

    internal static void OnStartup(GH_Canvas canvas) {
      oasysMenu = new ToolStripMenuItem("Oasys") {
        Name = "Oasys",
      };

      PopulateSub(oasysMenu);

      GH_DocumentEditor editor = null;

      while (editor == null) {
        editor = Instances.DocumentEditor;
        Thread.Sleep(321);
      }

      if (!editor.MainMenuStrip.Items.ContainsKey("Oasys")) {
        editor.MainMenuStrip.Items.Add(oasysMenu);
      } else {
        oasysMenu = (ToolStripMenuItem)editor.MainMenuStrip.Items["Oasys"];
        lock (oasysMenu) {
          oasysMenu.DropDown.Items.Add(new ToolStripSeparator());
          PopulateSub(oasysMenu);
        }
      }

      Instances.CanvasCreated -= OnStartup;
    }

    internal static void PopulateSub(ToolStripMenuItem menuItem) {
      menuItem.DropDown.Items.Add("GSA Documentation", Resources.Documentation, (s, a)
        => Process.Start(new ProcessStartInfo {
          FileName
            = "https://docs.oasys-software.com/structural/gsa/explanations/gsagh-introduction.html?source=grasshopper",
          UseShellExecute = true,
        }));
      menuItem.DropDown.Items.Add("GSA Example files", Resources.ExampleFiles, (s, a)
        => Process.Start(new ProcessStartInfo {
          FileName = "https://github.com/arup-group/GSA-Grasshopper/tree/main/ExampleFiles",
          UseShellExecute = true,
        }));
      menuItem.DropDown.Items.Add("GSA Info", Resources.GSAInfo, (s, a) => {
        var aboutBox = new AboutBox();
        aboutBox.ShowDialog();
      });
    }
  }
}
