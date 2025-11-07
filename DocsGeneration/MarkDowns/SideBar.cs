using System;
using System.Collections.Generic;
using System.IO;

using DocsGeneration.Data;
using DocsGeneration.MarkDowns.Helpers;

namespace DocsGeneration.MarkDowns {
  public class SideBar {
    public static void CreateSideBar(
      Dictionary<string, List<Component>> components, Dictionary<string, List<Parameter>> parameters,
      Configuration config) {
      Console.WriteLine($"Writing sidebar");
      // intro
      string sb = "/*\n --- Start of auto-generated text --- \n"
        + "This part of the sidebar file has been auto-generated, do not change it manually! Edit"
        + " the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration\n*/\n";
      int ind = 2;
      sb += "{\r\n";
      sb += AddLine(ind, "type: 'category',");
      sb += "\tlabel: 'GSA Grasshopper plugin',\r\n";
      sb += AddLine(ind, "items: [");
      ind += 2;

      string projectNameLower = config.ProjectName.ToLower();
      var page = $"references/{projectNameLower}/";
      // Parameter sidebar
      sb += AddLine(ind, "{");
      ind += 2;
      sb += AddLine(ind, "type: 'category',");
      sb += AddLine(ind, "label: 'Parameters',");
      sb += AddLine(ind, $"link: {{type: 'doc', id: '{page}{projectNameLower}-parameters'}},");
      sb += AddLine(ind, "items: [");
      ind += 2;
      foreach (string key in parameters.Keys) {
        foreach (Parameter parameter in parameters[key]) {
          string file = FileHelper.CreateSideBarFileName(FileHelper.CreateFileName(parameter, config.ProjectName));
          sb += AddLine(ind, $"'{page}{file}',");
        }
      }

      sb += AddLine(ind, $"'{page}{projectNameLower}-unitnumber-parameter'");
      ind -= 2;
      sb += AddLine(ind, "]");
      ind -= 2;
      sb += AddLine(ind, "},");

      // Components sidebar
      sb += AddLine(ind, "{");
      ind += 2;
      sb += AddLine(ind, "type: 'category',");
      sb += AddLine(ind, "label: 'Components',");
      sb += AddLine(ind, $"link: {{type: 'doc', id: '{page}{projectNameLower}-components'}},");
      sb += AddLine(ind, "items: [");

      // Per category sidebar
      ind += 2;
      foreach (string key in components.Keys) {
        sb += AddLine(ind, "{");
        ind += 2;
        sb += AddLine(ind, "type: 'category',");
        sb += AddLine(ind, $"label: '{key}',");
        sb += AddLine(ind,
          $"link: {{type: 'doc', id: '{page}{projectNameLower}-{key.ToLower()}-components-overview'}},");
        sb += AddLine(ind, "items: [");
        ind += 2;

        for (int i = 0; i < 7; i++) {
          foreach (Component component in components[key]) {
            if (component.SubCategory - 1 != i) {
              continue;
            }

            string file = FileHelper.CreateSideBarFileName(FileHelper.CreateFileName(component, config.ProjectName));
            sb += AddLine(ind, $"'{page}{file}',");
          }
        }

        sb = sb.TrimEnd(',');
        ind -= 2;
        sb += AddLine(ind, "]");
        ind -= 2;
        sb += AddLine(ind, "},");
      }

      sb = sb.TrimEnd(',');
      ind -= 2;
      sb += AddLine(ind, "]");
      ind -= 2;
      sb += AddLine(ind, "}");
      ind -= 2;
      sb += AddLine(ind, "]");
      ind -= 4;
      sb += AddLine(0, "}");

      sb += "/*\n--- End of auto-generated text ---\n*/\n";

      string filePath = $@"{config.OutputPath}\Helper\sidebar-{projectNameLower}.js";
      string directory = Path.GetDirectoryName(filePath);
      if (!Directory.Exists(directory)) {
        Directory.CreateDirectory(directory);
      }

      using (var js = new StreamWriter(filePath)) {
        js.Write(sb);
      }
    }

    private static string AddLine(int indentation, string text) {
      return new string(' ', indentation) + text + "\r\n";
    }
  }
}
