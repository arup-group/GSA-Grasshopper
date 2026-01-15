using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DocsGeneration.Data;
using DocsGeneration.MarkDowns.Helpers;

using Helpers;

namespace DocsGeneration.MarkDowns {
  public class Components {
    public static void CreateOverview(
      Dictionary<string, List<Component>> components, List<Parameter> parameters, Configuration config) {
      CreateComponentOverview(components.Keys.ToList(), config);

      var parameterNames = new List<string>();
      foreach (Parameter parameter in parameters) {
        parameterNames.Add(parameter.Name.ToUpper());
      }

      foreach (KeyValuePair<string, List<Component>> kvp in components) {
        CreateComponentOverview(kvp.Key, kvp.Value, parameterNames, config);
      }
    }

    public static void CreateComponents(List<Component> components, List<Parameter> parameters, Configuration config) {
      var parameterNames = new List<string>();
      foreach (Parameter parameter in parameters) {
        parameterNames.Add(parameter.Name.ToUpper());
      }

      foreach (Component parameter in components) {
        CreateComponent(parameter, parameterNames, config);
      }
    }

    private static void CreateComponent(Component component, List<string> parmeterNames, Configuration config) {
      string filePath = FileHelper.CreateMarkDownFileName(component, config);
      Console.WriteLine($"Writing {filePath}");

      var stringBuilder = new StringBuilder();
      stringBuilder.Append($"# {component.Name}\n\n");
      stringBuilder.Append(config.IsBeta ? StringHelper.AddBetaWarning() : string.Empty);

      var tempIconTable = new List<List<string>> {
        new List<string>() {
          FileHelper.CreateIconLink(component),
        },
      };
      stringBuilder.Append(TableBuilder.CreateTableString(string.Empty, TableBuilder.TableType.IconOnly,
        tempIconTable));
      stringBuilder.Append($"## Description\n\n{component.Description}\n\n");

      switch (component.ComponentType) {
        case "DropDownComponent":
          stringBuilder.Append(StringHelper.MakeItalic("Note: This is a dropdown component and input/output "
            + "may vary depending on the selected dropdown") + "\n\n");
          break;

        case "Section3dPreviewComponent":
          stringBuilder.Append(StringHelper.MakeItalic(
            "Note: This component can preview 3D Sections, right-click the middle of the "
            + "component to toggle the section preview.") + "\n\n");
          break;

        case "Section3dPreviewDropDownComponent":
          stringBuilder.Append(
            StringHelper.MakeItalic("Note: This is a dropdown component and input/output "
              + "may vary depending on the selected dropdown") + "\n" + StringHelper.MakeItalic(
              "This component can preview 3D Sections, right-click the middle of the "
              + "component to toggle the section preview.") + "\n\n");
          break;
      }

      if (component.Inputs != null && component.Inputs.Count != 0) {
        var tempInputTable = new List<List<string>> { };
        foreach (Parameter property in component.Inputs) {
          string desc = property.Description.Replace(StringHelper.PrefixBetweenTypes, string.Empty);
          List<string> cells = GetPropertiesCells(parmeterNames, config, property, desc);
          tempInputTable.Add(cells);
        }

        stringBuilder.Append(TableBuilder.CreateTableString("Input parameters", TableBuilder.TableType.InputOutput,
          tempInputTable, 3));
      }

      if (component.Outputs != null && component.Outputs.Count != 0) {
        var tempOutputTable = new List<List<string>> { };
        string note = string.Empty;
        foreach (Parameter property in component.Outputs) {
          string description = property.Description;
          note = CheckForResultNote(ref description, config.ResultNotes);
          List<string> cells = GetPropertiesCells(parmeterNames, config, property, description);
          tempOutputTable.Add(cells);
        }

        stringBuilder.Append(TableBuilder.CreateTableString("Output parameters", TableBuilder.TableType.InputOutput,
          tempOutputTable, 3));

        if (!string.IsNullOrEmpty(note)) {
          note = note.Replace(Environment.NewLine, " ").TrimSpaces();
          stringBuilder.Append(StringHelper.Replace(StringHelper.MakeItalic("* " + note)));
        }
      }

      Writer.Write(filePath, stringBuilder.ToString().TrimEnd());
    }

    private static List<string> GetPropertiesCells(
      List<string> parmeterNames, Configuration config, Parameter property, string description) {
      var cells = new List<string>() {
        FileHelper.CreateIconLink(property),
        FileHelper.CreateParameterLink(property, parmeterNames, config),
        StringHelper.Replace(StringHelper.MakeBold(property.Name)),
        StringHelper.Replace(description),
      };
      return cells;
    }

    private static string CheckForResultNote(ref string description, List<string> notesToCheckFor) {
      string noteOut = string.Empty;

      foreach (string note in notesToCheckFor) {
        if (!description.Contains(note)) {
          continue;
        }

        description = @"\* " + description.Replace(note, string.Empty);
        noteOut = note;
      }

      return noteOut;
    }

    private static void CreateComponentOverview(List<string> categories, Configuration config) {
      string lowerCaseName = config.ProjectName.ToLower();
      string filePath = $@"{config.OutputPath}\{lowerCaseName}-components.md";
      Console.WriteLine($"Writing {filePath}");

      var stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("# Components\n");
      if (config.IsBeta) {
        stringBuilder.Append(StringHelper.AddBetaWarning());
      }

      stringBuilder.AppendLine($" ![{config.ProjectName}-Ribbon](./images/RibbonLayout.gif)\n");

      foreach (string category in categories) {
        stringBuilder.Append(
          $"[{category} components]({lowerCaseName}-{category.ToLower()}-components-overview.md)\n\n");
      }

      Writer.Write(filePath, stringBuilder.ToString());
    }

    private static void CreateComponentOverview(
      string category, List<Component> components, List<string> parameterNames, Configuration config) {
      string filePath
        = $@"{config.OutputPath}\{config.ProjectName.ToLower()}-{category.ToLower()}-components-overview.md";
      Console.WriteLine($"Writing {filePath}");

      var stringBuilder = new StringBuilder();
      stringBuilder.AppendLine($"# {category} components \n");

      var tempTable = new List<List<string>> { };

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
        foreach (Component component in components) {
          if (component.SubCategory - 1 != i) {
            continue;
          }

          List<string> cells = GetSubCategoryCells(parameterNames, config, component);
          tempTable.Add(cells);
        }

        stringBuilder.Append(TableBuilder.CreateTableString(subCategories[i], TableBuilder.TableType.Properties,
          tempTable, 4));
      }

      Writer.Write(filePath, stringBuilder.ToString());
    }

    private static List<string> GetSubCategoryCells(
      List<string> parameterNames, Configuration config, Component component) {
      var cells = new List<string>() {
        FileHelper.CreateIconLink(component),
        FileHelper.CreatePageLink(component, config),
        StringHelper.Replace(StringHelper.ComponentDescription(component.Description, parameterNames)),
      };
      return cells;
    }
  }
}
