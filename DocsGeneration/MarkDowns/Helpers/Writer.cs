using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocsGeneration.MarkDowns.Helpers {
  public class Writer {
    public static void Write(string filePath, string text) {
      string directory = Path.GetDirectoryName(filePath);
      if (!Directory.Exists(directory)) {
        Directory.CreateDirectory(directory);
      }
      var file = new StreamWriter(filePath);
      file.WriteLine("<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->");
      file.Write(text);
      file.Close();
    }
  }
}
