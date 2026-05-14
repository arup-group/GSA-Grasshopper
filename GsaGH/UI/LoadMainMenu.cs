using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Grasshopper;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;

using GsaGH.Properties;

using static GsaGH.UI.MessageDialogBox;

namespace GsaGH.Graphics.Menu {
  public class MenuLoad {
    private static ToolStripMenuItem oasysMenu;
    public const string Name = "Oasys";
    private static readonly object menuLock = new object();

    internal static void OnStartup(GH_Canvas canvas) {
      oasysMenu = new ToolStripMenuItem(Name) {
        Name = Name,
      };
      AddToRibbon();
    }

    private static void AddToRibbon() {
      GH_DocumentEditor editor = WaitForDocumentEditor();
      if (editor == null) {
        Instances.CanvasCreated -= OnStartup;
        ShowMessage(MenuState.FailedToInitialize);
        return;
      }

      AddOrUpdateOasysMenu(editor);
      Instances.CanvasCreated -= OnStartup;
    }

    private static GH_DocumentEditor WaitForDocumentEditor() {
      DateTime start = DateTime.UtcNow;
      var timeout = TimeSpan.FromSeconds(60);
      while (DateTime.UtcNow - start < timeout) {
        GH_DocumentEditor editor = Instances.DocumentEditor;
        if (editor != null) {
          return editor;
        }

        Thread.Sleep(321);
      }

      return null;
    }

    private static void AddOrUpdateOasysMenu(GH_DocumentEditor editor) {
      if (!editor.MainMenuStrip.Items.ContainsKey(Name)) {
        editor.MainMenuStrip.Items.Add(oasysMenu);
      } else {
        oasysMenu = (ToolStripMenuItem)editor.MainMenuStrip.Items[Name];
      }

      lock (menuLock) {
        _ = PopulateOasysMenu(oasysMenu);
      }
    }

    internal static async Task PopulateOasysMenu(ToolStripMenuItem menuItem) {
      try {
        ToolStripMenuItem documentation = CreateDocumentationMenuItem();
        ToolStripMenuItem examples = await ExamplesMenu.CreateExamplesMenuItemAsync();
        ToolStripMenuItem info = CreateInfoMenuItem();
        menuItem.DropDown.Items.Insert(0, info);
        menuItem.DropDown.Items.Insert(0, examples);
        menuItem.DropDown.Items.Insert(0, documentation);
      } catch {
        ToolStrip parent = menuItem.GetCurrentParent();
        if (parent != null) {
          parent.BeginInvoke((Action)(() => ShowMessage(FileState.NoFilesFound, string.Empty)));
        } else {
          ShowMessage(FileState.NoFilesFound, string.Empty);
        }
      }
    }

    internal static ToolStripMenuItem CreateDocumentationMenuItem() {
      return new ToolStripMenuItem("GSA Documentation", Resources.Documentation,
        (s, a) => Process.Start(new ProcessStartInfo {
          FileName = Resources.DocumentationUrl,
          UseShellExecute = true,
        }));
    }

    internal static ToolStripMenuItem CreateInfoMenuItem() {
      return new ToolStripMenuItem("GSA Info", Resources.GSAInfo, (s, a) => {
        var aboutBox = new AboutBox();
        aboutBox.ShowDialog();
      });
    }
  }
}
