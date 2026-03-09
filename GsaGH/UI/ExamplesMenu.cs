using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Grasshopper;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;

using GsaGH.Properties;

using HtmlAgilityPack;

using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace GsaGH.Graphics.Menu {
  public class ExamplesMenu {
    private static ToolStripMenuItem examplesMenu;
    private const string name = "GSAGH Examples";

    private static string DownloadsPath
      => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

    internal static void OnStartup(GH_Canvas canvas) {
      examplesMenu = new ToolStripMenuItem(name) {
        Name = name,
      };

      PopulateSub(examplesMenu);

      GH_DocumentEditor editor = null;

      while (editor == null) {
        editor = Instances.DocumentEditor;
        Thread.Sleep(321);
      }

      if (!editor.MainMenuStrip.Items.ContainsKey(name)) {
        editor.MainMenuStrip.Items.Add(examplesMenu);
      } else {
        examplesMenu = (ToolStripMenuItem)editor.MainMenuStrip.Items[name];
        lock (examplesMenu) {
          examplesMenu.DropDown.Items.Add(new ToolStripSeparator());
          PopulateSub(examplesMenu);
        }
      }

      Instances.CanvasCreated -= OnStartup;
    }

    internal static void PopulateSub(ToolStripMenuItem menuItem) {
      AddExampleFilesAsync(menuItem);
    }

    private static void AddExampleFilesAsync(ToolStripMenuItem menuItem) {
      Task.Run(async () => {
        try {
          const string url = "https://samples.oasys-software.com/gsa/10.2/Gsa_GH/";
          List<FileEntry> files = await GetFilesFromWebPageAsync(url);

          if (files != null && files.Count > 0) {
            menuItem.GetCurrentParent().BeginInvoke((Action)(() => {
              foreach (FileEntry file in files) {
                AddFileMenuItem(menuItem, file);
              }
            }));
          }
        } catch (Exception ex) {
          // Log error or show message if needed
        }
      });
    }

    private static async Task DownloadFileAsync(string fileUrl, string savePath) {
      using var client = new HttpClient();
      using HttpResponseMessage response = await client.GetAsync(fileUrl);
      response.EnsureSuccessStatusCode();
      using var fs = new FileStream(savePath, FileMode.Create, FileAccess.Write);
      await response.Content.CopyToAsync(fs);
    }

    private static void AddFileMenuItem(ToolStripMenuItem menuItem, FileEntry file) {
      menuItem.DropDown.Items.Add(file.Name, Resources.Documentation, async (s, a) => {
        string savePath = Path.Combine(DownloadsPath, file.Name);
        try {
          await DownloadFileAsync(file.Url, savePath);

          if (Path.GetExtension(savePath).Equals(".gh", StringComparison.OrdinalIgnoreCase)) {
            var io = new GH_DocumentIO();
            if (io.Open(savePath)) {
              Instances.DocumentEditor.Invoke((Action)(() => Instances.ActiveCanvas.Document = io.Document));
            } else {
              MessageBox.Show($"Failed to open the Grasshopper file: {file.Name}", "Error");
            }
          } else {
            MessageBox.Show($"File downloaded to: {savePath}", "Download Complete");
          }
        } catch (Exception ex) {
          MessageBox.Show($"Download of the file: {file.Name}, failed: {ex.Message}", "Error");
        }
      });
    }

    public static async Task<List<FileEntry>> GetFilesFromWebPageAsync(string url) {
      var fileList = new List<FileEntry>();
      using var client = new HttpClient();
      string html = await client.GetStringAsync(url);
      var doc = new HtmlDocument();
      doc.LoadHtml(html);

      HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//a[@href]");
      if (nodes == null) {
        return fileList;
      }

      foreach (HtmlNode link in nodes) {
        string href = link.GetAttributeValue("href", "");
        if (href.EndsWith(".gh") || href.EndsWith(".gwa") || href.EndsWith(".gwb") || href.EndsWith(".3dm")) {
          fileList.Add(new FileEntry {
            Name = link.InnerText.Trim(),
            Url = href.StartsWith("http") ? href : new Uri(new Uri(url), href).ToString(),
          });
        }
      }

      return fileList;
    }
  }

  public class FileEntry {
    public string Name { get; set; }
    public string Url { get; set; }
  }

}
