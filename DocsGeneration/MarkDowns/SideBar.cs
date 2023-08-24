﻿using System;
using System.Collections.Generic;
using System.IO;
using DocsGeneration.Data;
using DocsGeneration.MarkDowns.Helpers;

namespace DocsGeneration.MarkDowns {
  public class SideBar {
    public static string page = "/references/";

    public static void CreateSideBar(
      Dictionary<string, List<Component>> components,
      Dictionary<string, List<Parameter>> parameters) {
      Console.WriteLine($"Writing sidebar");
      // intro
      string sb = "/*\n --- Start of auto-generated text --- \n" +
        "This part of the sidebar file has been auto-generated, do not change it manually! Edit" +
        " the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration\n*/\n";
      sb += "{\r\n";
      sb += "\ttitle: 'GSA Grasshopper plugin',\r\n";
      int ind = 4;
      sb += AddLine(ind, "collapsable: false,");
      sb += AddLine(ind, "children: [");
      ind += 2;
      sb += AddLine(ind, $"'{page}gsagh-introduction',");

      // Parameter sidebar
      sb += AddLine(ind, "{");
      ind += 2;
      sb += AddLine(ind, "title: 'Parameters',");
      sb += AddLine(ind, $"path: '{page}gsagh-parameters',");
      sb += AddLine(ind, "children: [");
      ind += 2;
      foreach(string key in parameters.Keys) {
        foreach (Parameter parameter in parameters[key]) {
          string file = FileHelper.CreateSideBarFileName(parameter);
          sb += AddLine(ind, $"'{page}{file}',");
        }
      }
      string folder = "gsagh/parameters/";
      sb += AddLine(ind, $"'{page}{folder}gsagh-unitnumber-parameter'");
      ind -= 2;
      sb += AddLine(ind, "]");
      ind -= 2;
      sb += AddLine(ind, "},");
      
      // Components sidebar
      sb += AddLine(ind, "{");
      ind += 2;
      sb += AddLine(ind, "title: 'Components',");
      sb += AddLine(ind, $"path: '{page}gsagh-components',");
      sb += AddLine(ind, "children: [");
      
      // Per category sidebar
      ind += 2;
      foreach (string key in components.Keys) {
        sb += AddLine(ind, "{");
        ind += 2;
        sb += AddLine(ind, $"title: '{key}',");
        folder = "gsagh/components/";
        sb += AddLine(ind, $"path: '{page}{folder}gsagh-{key.ToLower()}-components-overview',");
        sb += AddLine(ind, "children: [");
        ind += 2;

        for (int i = 0; i < 7; i++) {
          foreach (Component component in components[key]) {
            if (component.SubCategory - 1 != i) {
              continue;
            }

            string file = FileHelper.CreateSideBarFileName(component);
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
      sb += AddLine(0, "},");

      sb += "/*\n--- End of auto-generated text ---\n*/\n";

      var js = new StreamWriter($@"Output\sidebar-gsagh.js");
      js.Write(sb);
      js.Close();
    }

    private static string AddLine(int indentation, string text) {
      return new string(' ', indentation) + text + "\r\n";
    }
  }
}
