using System;
using System.Collections.Generic;
using System.Linq;

using DocsGeneration.Data;
using DocsGeneration.MarkDowns.Helpers;

namespace DocsGeneration.MarkDowns {
  public class Parameters {
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

    public static void CreateOverview(Dictionary<string, List<Parameter>> parameters, Configuration config) {
      string filePath = $@"{config.OutputPath}\{config.ProjectName.ToLower()}-parameters.md";
      Console.WriteLine($"Writing {filePath}");

      string text = "# Parameters\n\n";
      if (config.IsBeta) {
        text += StringHelper.AddBetaWarning();
        text += "\n";
      }

      text
        += "The GSA plugin introduces a new set of custom Grasshopper parameters. Parameters are what is passed from one component's output to another component's input.\n"
        + "![Parameters](https://developer.rhino3d.com/api/grasshopper/media/ParameterKinds.png)\n" + "\n"
        + "## Custom GSA Parameters" + "\n\n";

      var tableHeaders = new List<string>() {
        " ", // icon
        "Name",
        "Description",
      };

      var widths = new List<int>() {
        Table.IconWidth,
        Table.NameWidth,
        Table.DescriptionWidth,
      };
      foreach (string header in parameters.Keys) {
        var propertieTable = new List<List<string>> {
          tableHeaders,
        };
        propertieTable.AddRange(parameters[header].Select(parameter => new List<string>() {
          FileHelper.CreateIconLink(parameter),
          FileHelper.CreatePageLink(parameter, config.ProjectName),
          parameter.Description.Replace(StringHelper.PrefixBetweenTypes, string.Empty),
        }));

        var table = new Table(header, 2, Table.GetColumnsWidth(propertieTable));
        table.AddTableHeader(tableHeaders, widths);
        propertieTable.Skip(1).ToList().ForEach(row => table.AddRow(row));
        text += table.Finalise();
      }

      Writer.Write(filePath, text);
    }

    public static void CreateParameters(List<Parameter> parameters, Configuration config) {
      var parameterNames = new List<string>();
      foreach (Parameter parameter in parameters) {
        parameterNames.Add(parameter.Name.ToUpper());
      }

      foreach (Parameter parameter in parameters) {
        CreateParameter(parameter, parameterNames, config);
      }
    }

    private static void CreateParameter(Parameter parameter, List<string> parameterNames, Configuration config) {
      string filePath = FileHelper.CreateMarkDownFileName(parameter, config);
      Console.WriteLine($"Writing {filePath}");

      string text = $"# {parameter.Name}\n\n";
      text += config.IsBeta ? StringHelper.AddBetaWarning() : string.Empty;

      int linkImageWidth = 150;
      var tempIconTable = new List<List<string>> {
        _iconTableHeader,
        new List<string>() {
          FileHelper.CreateIconLink(parameter),
        },
      };

      var iconTable = new Table(string.Empty, 2, Table.GetColumnsWidth(tempIconTable));
      iconTable.AddTableHeader(_iconTableHeader, new List<int>() {
        linkImageWidth,
      });
      iconTable.AddRow(tempIconTable[1]);
      text += iconTable.Finalise();

      if (parameter.Name == "Bool6") {
        text += StringHelper.Admonition("Did you know?", AdmonitionType.Info,
          "The `Bool6` icon takes inspiration from the central pin/hinge/charnier connection "
          + "[Ove Arup's Kingsgate footbridge](https://www.arup.com/projects/kingsgate-footbridge"
          + ").\r\n![Kingsgate Footbridge Durham](./images/Kingsgate-Footbridge-Durham.jpg)\r\n"
          + "*(c) Giles Rocholl / Arup*");
      }

      text += StringHelper.SummaryDescription(parameter.Summary, config);

      if (parameter.Properties != null && parameter.Properties.Count != 0) {
        var propertieTable = new List<List<string>> {
          _defaultTableHeaders,
        };
        propertieTable.AddRange(parameter.Properties.Select(property => new List<string>() {
          FileHelper.CreateIconLink(property),
          FileHelper.CreateParameterLink(property, parameterNames, config),
          StringHelper.MakeBold(property.Name),
          property.Description,
        }));
        var table = new Table("Properties", 2, Table.GetColumnsWidth(propertieTable));

        table.AddTableHeader(_defaultTableHeaders, _imageWidths);
        propertieTable.Skip(1).ToList().ForEach(row => table.AddRow(row));
        text += table.Finalise();

        if (parameter.PropertiesComponent != null) {
          string link = FileHelper.CreatePageLink(parameter.PropertiesComponent, config);
          string note = $"Note: the above properties can be retrieved using the {link} component";
          text += StringHelper.MakeItalic(note);
        }
      }

      Writer.Write(filePath, text);
    }
  }
}
