using System.IO;

namespace DocsGeneration.MarkDowns.Helpers {
  public class Writer {
    public static void Write(string filePath, string text) {
      string directory = Path.GetDirectoryName(filePath);
      if (!Directory.Exists(directory)) {
        Directory.CreateDirectory(directory);
      }
      
      string autogen = "<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->";
      string[] split = text.Split('\n');
      text = split[0] + '\n';
      text += autogen + '\n';
      for (int i = 1; i < split.Length; i++) {
        text += split[i] + '\n';
      }

      using (var file = new StreamWriter(filePath)) {
        file.Write(text);
      }
    }
  }
}
