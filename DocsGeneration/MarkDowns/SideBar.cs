using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using DocsGeneration.Data;
using DocsGeneration.MarkDowns.Helpers;

namespace DocsGeneration.MarkDowns {
  public class SideBar {
    public static void CreateSideBar(
      Dictionary<string, List<Component>> components, Dictionary<string, List<Parameter>> parameters,
      Configuration config) {
      Console.WriteLine($"Writing sidebar");
      // intro
      var sb = new StringBuilder();
      sb.Append("/*\n --- Start of auto-generated text --- \n"
        + "This part of the sidebar file has been auto-generated, do not change it manually! Edit"
        + " the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration\n*/\n");
      int ind = 2;
      sb.Append("{\r\n");
      sb.Append(AddLine(ind, "type: 'category',"));
      sb.Append("\tlabel: 'GSA Grasshopper plugin',\r\n");
      sb.Append(AddLine(ind, "items: ["));
      ind += 2;

      string projectNameLower = config.ProjectName.ToLower();
      string page = $"references/{projectNameLower}/";
      // Parameter sidebar
      sb.Append(AddLine(ind, "{"));
      ind += 2;
      sb.Append(AddLine(ind, "type: 'category',"));
      sb.Append(AddLine(ind, "label: 'Parameters',"));
      sb.Append(AddLine(ind, $"link: {{type: 'doc', id: '{page}{projectNameLower}-parameters'}},"));
      sb.Append(AddLine(ind, "items: ["));
      ind += 2;
      foreach (string key in parameters.Keys) {
        foreach (Parameter parameter in parameters[key]) {
          string file = FileHelper.CreateSideBarFileName(FileHelper.CreateFileName(parameter, config.ProjectName));
          sb.Append(AddLine(ind, $"'{page}{file}',"));
        }
      }

      sb.Append(AddLine(ind, $"'{page}{projectNameLower}-unitnumber-parameter'"));
      ind -= 2;
      sb.Append(AddLine(ind, "]"));
      ind -= 2;
      sb.Append(AddLine(ind, "},"));

      // Components sidebar
      sb.Append(AddLine(ind, "{"));
      ind += 2;
      sb.Append(AddLine(ind, "type: 'category',"));
      sb.Append(AddLine(ind, "label: 'Components',"));
      sb.Append(AddLine(ind, $"link: {{type: 'doc', id: '{page}{projectNameLower}-components'}},"));
      sb.Append(AddLine(ind, "items: ["));

      // Per category sidebar
      ind += 2;
      foreach (string key in components.Keys) {
        sb.Append(AddLine(ind, "{"));
        ind += 2;
        sb.Append(AddLine(ind, "type: 'category',"));
        sb.Append(AddLine(ind, $"label: '{key}',"));
        sb.Append(AddLine(ind,
          $"link: {{type: 'doc', id: '{page}{projectNameLower}-{key.ToLower()}-components-overview'}},"));
        sb.Append(AddLine(ind, "items: ["));
        ind += 2;

        for (int i = 0; i < 7; i++) {
          foreach (Component component in components[key]) {
            if (component.SubCategory - 1 != i) {
              continue;
            }

            string file = FileHelper.CreateSideBarFileName(FileHelper.CreateFileName(component, config.ProjectName));
            sb.Append(AddLine(ind, $"'{page}{file}',"));
          }
        }

        TrimEndFor(sb, ',');
        ind -= 2;
        sb.Append(AddLine(ind, "]"));
        ind -= 2;
        sb.Append(AddLine(ind, "},"));
      }

      TrimEndFor(sb, ',');
      ind -= 2;
      sb.Append(AddLine(ind, "]"));
      ind -= 2;
      sb.Append(AddLine(ind, "}"));
      ind -= 2;
      sb.Append(AddLine(ind, "]"));
      sb.Append(AddLine(0, "}"));

      sb.Append("/*\n--- End of auto-generated text ---\n*/\n");

      string filePath = $@"{config.OutputPath}\Helper\sidebar-{projectNameLower}.js";
      string directory = Path.GetDirectoryName(filePath);
      if (!Directory.Exists(directory)) {
        Directory.CreateDirectory(directory);
      }

      using (var js = new StreamWriter(filePath)) {
        js.Write(sb);
      }
    }

    private static void TrimEndFor(StringBuilder sb, char c) {
      if (sb.Length > 0 && sb[sb.Length - 1] == c) {
        sb.Length--;
      }
    }

    private static string AddLine(int indentation, string text) {
      return new string(' ', indentation) + text + "\r\n";
    }
  }
}
