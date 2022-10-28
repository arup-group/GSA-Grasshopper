using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;

namespace GsaGH.UI.Menu
{
  public class MenuLoad
  {
    private static ToolStripMenuItem oasysMenu;
    internal static void OnStartup(GH_Canvas canvas)
    {
      oasysMenu = new ToolStripMenuItem("Oasys");
      oasysMenu.Name = "Oasys";

      PopulateSub(oasysMenu);

      GH_DocumentEditor editor = null;

      while (editor == null)
      {
        editor = Grasshopper.Instances.DocumentEditor;
        Thread.Sleep(321);
      }

      if (!editor.MainMenuStrip.Items.ContainsKey("Oasys"))
        editor.MainMenuStrip.Items.Add(oasysMenu);
      else
      {
        oasysMenu = (ToolStripMenuItem)editor.MainMenuStrip.Items["Oasys"];
        lock (oasysMenu)
        {
          oasysMenu.DropDown.Items.Add(new ToolStripSeparator());
          PopulateSub(oasysMenu);
        }
      }

      Grasshopper.Instances.CanvasCreated -= OnStartup;
    }

    private static void PopulateSub(ToolStripMenuItem menuItem)
    {
      // add documentation
      menuItem.DropDown.Items.Add("GSA Documentation", Properties.Resources.Documentation, (s, a) =>
      {
        Process.Start(new ProcessStartInfo
        {
          FileName = "https://docs.oasys-software.com/structural/gsa/explanations/gsagh-introduction.html",
          UseShellExecute = true
        });
      });
      // add example files
      menuItem.DropDown.Items.Add("GsaGH Example files", Properties.Resources.ExampleFiles, (s, a) =>
      {
        Process.Start(new ProcessStartInfo
        {
          FileName = "https://github.com/arup-group/GSA-Grasshopper/tree/main/ExampleFiles",
          UseShellExecute = true
        });
      });
      // add info
      menuItem.DropDown.Items.Add("GSA Info", Properties.Resources.GSAInfo, (s, a) =>
      {
        AboutBox aboutBox = new AboutBox();
        aboutBox.ShowDialog();
      });
    }
  }
}
