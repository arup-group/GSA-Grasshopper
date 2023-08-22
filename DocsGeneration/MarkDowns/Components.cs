using GsaGhDocs.Parameters;
using GsaGhDocs.Helpers;
using System.Collections.Generic;
using System.IO;
using System;
using GsaGhDocs.Components;
using System.Xml.Linq;
using System.Linq;
using System.Security.Cryptography;

namespace GsaGhDocs.MarkDowns {
  public class Components {
    public static readonly string ComponentsOverview =
      "# Components\n" +
      "\n" +
      "::: warning" +
      "\nGSA-Grasshopper plugin [GsaGH] is pre-release and under active development, including further testing to be undertaken. It is provided \\\"as-is\\\" and you bear the risk of using it. Future versions may contain breaking changes. Any files, results, or other types of output information created using GsaGH should not be relied upon without thorough and independent checking.\n:::\n" +
      "\n" +
      "![GsaGH-Ribbon](./images/gsagh/RibbonScreenshot.png)\n" +
      "\n"; 

    public static void CreateOverview(Dictionary<string, List<Component>> components) {
      string componentsOverview = StringHelper.FileName("components", string.Empty);
      CreateComponentOverview(components.Keys.ToList());

      foreach (KeyValuePair<string, List<Component>> kvp in components) {
        CreateComponentOverview(kvp.Key, kvp.Value);
      }
    }

    public static void CreateComponents(
      List<Component> components, List<Parameter> parameters) {
      var parameterNames = new List<string>();
      foreach (Parameter parameter in parameters) {
        parameterNames.Add(parameter.Name);
      }

      foreach (Component parameter in components) {
        CreateComponent(parameter, parameterNames);
      }
    }

    private static void CreateComponent(Component component, List<string> parmeterNames) {
      string filePath = StringHelper.FileName(component.Name, "Component");
      Console.WriteLine($"Writing {filePath}");

      string text = $"# {component.Name}\n\n";
      text += StringHelper.BetaWarning();

      var iconHeaders = new List<string>() {
        "Icon"
      };
      var iconTable = new Table(string.Empty, iconHeaders);
      iconTable.AddRow(new List<string>() {
        StringHelper.Icon(component.Name),
      });

      text += iconTable.Finalise();

      text += $"## Description\n\n{component.Description}\n\n";

      if (component.Inputs != null && component.Inputs.Count != 0) {
        var headers = new List<string>() {
          "Icon",
          "Type",
          "Name",
          "Description"
        };
        var table = new Table("Inputs", headers);
        foreach (Parameter property in component.Inputs) {
          table.AddRow(new List<string>() {
            StringHelper.Icon(property.ParameterType, "Param"),
            StringHelper.ParameterLink(property.ParameterType, parmeterNames),
            StringHelper.Bold(property.Name),
            property.Description,
         });
        }

        text += table.Finalise();
      }

      if (component.Outputs != null && component.Outputs.Count != 0) {
        var headers = new List<string>() {
          "Icon",
          "Type",
          "Name",
          "Description"
        };
        var table = new Table("Outputs", headers);
        foreach (Parameter property in component.Outputs) {
          table.AddRow(new List<string>() {
            StringHelper.Icon(property.ParameterType, "Param"),
            StringHelper.ParameterLink(property.ParameterType, parmeterNames),
            StringHelper.Bold(property.Name),
            property.Description,
         });
        }

        text += table.Finalise();
      }

      Writer.Write(filePath, text);
    }

    private static void CreateComponentOverview(List<string> categories) {
      string filePath = $@"Output\gsagh-components.md";
      Console.WriteLine($"Writing {filePath}");

      string text = ComponentsOverview;

      foreach (string category in categories) {
        text += $"[{category} components](gsagh-{category.ToLower()}-components-overview.html)\n\n";
      }

      Writer.Write(filePath, text);
    }

    private static void CreateComponentOverview(
      string category, List<Component> components) {
      string filePath = $@"Output\gsagh-{category.ToLower()}-components-overview.md";
      Console.WriteLine($"Writing {filePath}");

      string text = $"# {category} components \n\n";

      var tableHeaders = new List<string>() {
        " ", // icon
        "Name",
        "Description"
      };

      var subCategories = new List<string>() {
        "Primary",
        "Secondary",
        "Tertiary",
        "Quarternary",
        "Quinary",
        "Senary",
        "Septenary",
      };

      for (int i = 0; i < subCategories.Count; i++) {
        var table = new Table(subCategories[i], tableHeaders);
        foreach (Component component in components) {
          if (component.SubCategory - 1 != i) {
            continue;
          }

          table.AddRow(new List<string>(){
            StringHelper.Icon(component.Name),
            StringHelper.Link(component.Name, "Component"),
            component.Description
          });
        }
        text += table.Finalise();
      }

      Writer.Write(filePath, text);
    }
  }
}
