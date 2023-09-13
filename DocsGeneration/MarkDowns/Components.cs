using System.Collections.Generic;
using System;
using System.Linq;
using DocsGeneration.Data;
using DocsGeneration.MarkDowns.Helpers;
using GsaGH.Components.Helpers;

namespace DocsGeneration.MarkDowns {
  public class Components {
    public static readonly string ComponentsOverview =
      "# Components\n" +
      "\n" +
      "::: warning" +
      "\nGSA-Grasshopper plugin is pre-release and under active development, including further testing to be undertaken. It is provided \\\"as-is\\\" and you bear the risk of using it. Future versions may contain breaking changes. Any files, results, or other types of output information created using the plugin should not be relied upon without thorough and independent checking.\n:::\n" +
      "\n" +
      "![GsaGH-Ribbon](./images/RibbonLayout.gif)\n" +
      "\n";

    public static void CreateOverview(
      Dictionary<string, List<Component>> components, List<Parameter> parameters) {
      CreateComponentOverview(components.Keys.ToList());

      var parameterNames = new List<string>();
      foreach (Parameter parameter in parameters) {
        parameterNames.Add(parameter.Name.ToUpper());
      }

      foreach (KeyValuePair<string, List<Component>> kvp in components) {
        CreateComponentOverview(kvp.Key, kvp.Value, parameterNames);
      }
    }

    public static void CreateComponents(
      List<Component> components, List<Parameter> parameters) {
      var parameterNames = new List<string>();
      foreach (Parameter parameter in parameters) {
        parameterNames.Add(parameter.Name.ToUpper());
      }

      foreach (Component parameter in components) {
        CreateComponent(parameter, parameterNames);
      }
    }

    private static void CreateComponent(Component component, List<string> parmeterNames) {
      string filePath = FileHelper.CreateMarkDownFileName(component);
      Console.WriteLine($"Writing {filePath}");

      string text = $"# {component.Name}\n\n";
      text += StringHelper.AddBetaWarning();

      var iconHeaders = new List<string>() {
        "Icon"
      };
      var iconTable = new Table(string.Empty, 2, iconHeaders, new List<int>() { 150 });
      iconTable.AddRow(new List<string>() {
        FileHelper.CreateIconLink(component),
      });

      text += iconTable.Finalise();

      text += $"## Description\n\n{component.Description}\n\n";

      switch (component.ComponentType) {
        case "DropDownComponent":
          text += StringHelper.MakeItalic("Note: This is a dropdown component and input/output " +
            "may vary depending on the selected dropdown") + "\n\n";
          break;

        case "Section3dPreviewComponent":
          text += StringHelper.MakeItalic(
            "Note: This component can preview 3D Sections, right-click the middle of the " +
            "component to toggle the section preview.") + "\n\n";
          break;

        case "Section3dPreviewDropDownComponent":
          text += StringHelper.MakeItalic("Note: This is a dropdown component and input/output " +
            "may vary depending on the selected dropdown") + "\n" +
            StringHelper.MakeItalic("This component can preview 3D Sections, right-click the middle of the " +
            "component to toggle the section preview.") + "\n\n";
          break;
      }

      var headers = new List<string>() {
          "Icon",
          "Type",
          "Name",
          "Description"
        };

      var widths = new List<int>() {
        Table.IconWidth,
        Table.NameWidth,
        Table.NameWidth,
        Table.DescriptionWidth
      };

      if (component.Inputs != null && component.Inputs.Count != 0) {

        var table = new Table("Input parameters", 3, headers, widths);
        foreach (Parameter property in component.Inputs) {
          table.AddRow(new List<string>() {
            FileHelper.CreateIconLink(property),
            FileHelper.CreateParameterLink(property, parmeterNames),
            StringHelper.MakeBold(property.Name),
            property.Description.Replace("GSA ", string.Empty),
         });
        }

        text += table.Finalise();
      }


      if (component.Outputs != null && component.Outputs.Count != 0) {
        var table = new Table("Output parameters", 3, headers, widths);

        string note = string.Empty;
        foreach (Parameter property in component.Outputs) {
          string description = property.Description;
          note = CheckForResultNote(ref description);
          
          table.AddRow(new List<string>() {
            FileHelper.CreateIconLink(property),
            FileHelper.CreateParameterLink(property, parmeterNames),
            StringHelper.MakeBold(property.Name),
            description,
          });
        }

        text += table.Finalise();
        if (!string.IsNullOrEmpty(note)) {
          note = note.Replace(Environment.NewLine, " ").Replace("  ", " ");
          text += "\n\n" + StringHelper.MakeItalic("* " + note) + "\n\n";
        }
      }

      Writer.Write(filePath, text);
    }

    private static string CheckForResultNote(ref string description) {
      string noteOut = string.Empty;

      var notesToCheckFor = new List<string>() {
        ResultNotes.NoteNodeResults,
        ResultNotes.Note1dResults,
        ResultNotes.Note2dForceResults,
        ResultNotes.Note2dStressResults,
        ResultNotes.Note3dStressResults,
        ResultNotes.Note2dResults,
      };

      foreach (string note in notesToCheckFor) {
        if (description.Contains(note)) {
          description = "* " + description.Replace(note, string.Empty);
          noteOut = note;
        }
      }

      return noteOut;
    }

    private static void CreateComponentOverview(List<string> categories) {
      string filePath = $@"Output\gsagh-components.md";
      Console.WriteLine($"Writing {filePath}");

      string text = ComponentsOverview;

      foreach (string category in categories) {
        text += $"[{category} components](gsagh-{category.ToLower()}-components-overview.md)\n\n";
      }

      Writer.Write(filePath, text);
    }

    private static void CreateComponentOverview(
      string category, List<Component> components, List<string> parameterNames) {
      string filePath = $@"Output\gsagh-{category.ToLower()}-components-overview.md";
      Console.WriteLine($"Writing {filePath}");

      string text = $"# {category} components \n\n";

      var tableHeaders = new List<string>() {
        " ", // icon
        "Name",
        "Description"
      };

      var widths = new List<int>() {
        Table.IconWidth,
        Table.NameWidth,
        Table.DescriptionWidth
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
        var table = new Table(subCategories[i], 4, tableHeaders, widths);
        foreach (Component component in components) {
          if (component.SubCategory - 1 != i) {
            continue;
          }

          table.AddRow(new List<string>(){
            FileHelper.CreateIconLink(component),
            FileHelper.CreatePageLink(component),
            StringHelper.ComponentDescription(component.Description, parameterNames)
          });
        }

        text += table.Finalise();
      }

      Writer.Write(filePath, text);
    }
  }
}
