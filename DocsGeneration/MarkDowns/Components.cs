using System;
using System.Collections.Generic;
using System.Linq;

using DocsGeneration.Data;
using DocsGeneration.MarkDowns.Helpers;

using Helpers;

namespace DocsGeneration.MarkDowns {
  public class Components {
    private static readonly List<string> _iconTableHeader = new List<string>() {
      "Icon",
    };
    private static readonly List<string> _defaultTableHeaders = new List<string>() {
      "Icon",
      "Type",
      "Name",
      "Description",
    };
    private static readonly List<int> _imageWidths = new List<int>() {
      Table.IconWidth,
      Table.NameWidth,
      Table.NameWidth,
      Table.DescriptionWidth,
    };

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

      string text = $"# {component.Name}\n\n";
      text += config.IsBeta ? StringHelper.AddBetaWarning() : string.Empty;

      int linkImageWidth = 150;
      var tempIconTable = new List<List<string>> {
        _iconTableHeader,
        new List<string>() {
          FileHelper.CreateIconLink(component),
        },
      };

      var iconTable = new Table(string.Empty, 2, GetColumnsWidth(tempIconTable));
      iconTable.AddTableHeader(_iconTableHeader, new List<int>() {
        linkImageWidth,
      });
      iconTable.AddRow(tempIconTable[1]);
      text += iconTable.Finalise();

      text += $"## Description\n\n{component.Description}\n\n";

      switch (component.ComponentType) {
        case "DropDownComponent":
          text += StringHelper.MakeItalic("Note: This is a dropdown component and input/output "
            + "may vary depending on the selected dropdown") + "\n\n";
          break;

        case "Section3dPreviewComponent":
          text += StringHelper.MakeItalic("Note: This component can preview 3D Sections, right-click the middle of the "
            + "component to toggle the section preview.") + "\n\n";
          break;

        case "Section3dPreviewDropDownComponent":
          text += StringHelper.MakeItalic("Note: This is a dropdown component and input/output "
            + "may vary depending on the selected dropdown") + "\n" + StringHelper.MakeItalic(
            "This component can preview 3D Sections, right-click the middle of the "
            + "component to toggle the section preview.") + "\n\n";
          break;
      }

      if (component.Inputs != null && component.Inputs.Count != 0) {
        var tempInputTable = new List<List<string>> {
          _defaultTableHeaders,
        };
        foreach (Parameter property in component.Inputs) {
          var cells = new List<string>() {
            FileHelper.CreateIconLink(property),
            FileHelper.CreateParameterLink(property, parmeterNames, config),
            StringHelper.MakeBold(property.Name),
            StringHelper.Replace(property.Description.Replace(StringHelper.PrefixBetweenTypes, string.Empty)),
          };
          tempInputTable.Add(cells);
        }

        var table = new Table("Input parameters", 3, GetColumnsWidth(tempInputTable));
        table.AddTableHeader(_defaultTableHeaders, _imageWidths);
        tempInputTable.Skip(1).ToList().ForEach(row => table.AddRow(row));
        text += table.Finalise();
      }

      if (component.Outputs != null && component.Outputs.Count != 0) {
        var tempOutputTable = new List<List<string>> {
          _defaultTableHeaders,
        };
        string note = string.Empty;
        foreach (Parameter property in component.Outputs) {
          string description = property.Description;
          note = CheckForResultNote(ref description, config.ResultNotes);
          var cells = new List<string>() {
            FileHelper.CreateIconLink(property),
            FileHelper.CreateParameterLink(property, parmeterNames, config),
            StringHelper.MakeBold(property.Name),
            StringHelper.Replace(description),
          };
          tempOutputTable.Add(cells);
        }

        var table = new Table("Output parameters", 3, GetColumnsWidth(tempOutputTable));
        table.AddTableHeader(_defaultTableHeaders, _imageWidths);
        tempOutputTable.Skip(1).ToList().ForEach(row => table.AddRow(row));
        text += table.Finalise();
        if (!string.IsNullOrEmpty(note)) {
          note = note.Replace(Environment.NewLine, " ").TrimSpaces();
          text += "\n\n" + StringHelper.Replace(StringHelper.MakeItalic("* " + note) + "\n\n");
        }
      }

      Writer.Write(filePath, text);
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

      string text = "# Components\n\n";
      if (config.IsBeta) {
        text += StringHelper.AddBetaWarning();
        text += "\n";
      }

      text += $" ![{config.ProjectName}-Ribbon](./images/RibbonLayout.gif)\n\n";

      foreach (string category in categories) {
        text += $"[{category} components]({lowerCaseName}-{category.ToLower()}-components-overview.md)\n\n";
      }

      Writer.Write(filePath, text);
    }

    private static void CreateComponentOverview(
      string category, List<Component> components, List<string> parameterNames, Configuration config) {
      string filePath
        = $@"{config.OutputPath}\{config.ProjectName.ToLower()}-{category.ToLower()}-components-overview.md";
      Console.WriteLine($"Writing {filePath}");

      string text = $"# {category} components \n\n";

      var tableHeaders = new List<string>() {
        " ", // icon
        "Name",
        "Description",
      };

      var imageWidths = new List<int>() {
        Table.IconWidth,
        Table.NameWidth,
        Table.DescriptionWidth,
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

      //for (int i = 0; i < subCategories.Count; i++) {

      //  var table = new Table(subCategories[i], 4);
      //  foreach (Component component in components) {
      //    if (component.SubCategory - 1 != i) {
      //      continue;
      //    }

      //    var rows = new List<string>() {
      //      FileHelper.CreateIconLink(component),
      //      FileHelper.CreatePageLink(component, config),
      //      StringHelper.Replace(StringHelper.ComponentDescription(component.Description, parameterNames)),
      //    };
      //    for (int j = 0; j < textMaxWidths.Count; j++) {
      //      textMaxWidths[j] = Math.Max(textMaxWidths[j], rows[j].Length);
      //    }
      //  }

      //  table.AddTableHeader(tableHeaders, imageWidths, textMaxWidths);
      //  text += table.Finalise();
      //}

      Writer.Write(filePath, text);
    }

    private static List<int> GetColumnsWidth(List<List<string>> table) {
      const int _columnMinWidth = 30;
      return Enumerable.Range(0, table[0].Count).Select(i
        => Math.Max(_columnMinWidth, table.Where(row => i < row.Count).Max(row => row[i].Length))).ToList();
    }

  }
}
