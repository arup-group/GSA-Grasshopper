using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DocsGeneration.Data;
using DocsGeneration.MarkDowns.Helpers;

namespace DocsGeneration.MarkDowns {
  public class Parameters {
    public static void CreateOverview(Dictionary<string, List<Parameter>> parameters, Configuration config) {
      string filePath = $@"{config.OutputPath}\{config.ProjectName.ToLower()}-parameters.md";
      Console.WriteLine($"Writing {filePath}");

      var stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("# Parameters\n");
      if (config.IsBeta) {
        stringBuilder.Append(StringHelper.AddBetaWarning());
      }

      stringBuilder.AppendLine(
        "The GSA plugin introduces a new set of custom Grasshopper parameters. Parameters are what is passed from one component's output to another component's input.\n"
        + "![Parameters](https://developer.rhino3d.com/api/grasshopper/media/ParameterKinds.png)\n" + "\n"
        + "## Custom GSA Parameters" + "\n");

      foreach (string header in parameters.Keys) {
        List<List<string>> propertieTable = GetParametersTempTable(parameters, config, header);
        stringBuilder.Append(TableBuilder.CreateTableString(header, TableBuilder.TableType.Properties, propertieTable));
      }

      Writer.Write(filePath, stringBuilder.ToString());
    }

    private static List<List<string>> GetParametersTempTable(
      Dictionary<string, List<Parameter>> parameters, Configuration config, string header) {
      var parametersTable = new List<List<string>>();
      parametersTable.AddRange(parameters[header].Select(parameter => new List<string>() {
        FileHelper.CreateIconLink(parameter),
        FileHelper.CreatePageLink(parameter, config.ProjectName),
        parameter.Description.Replace(StringHelper.PrefixBetweenTypes, string.Empty),
      }));
      return parametersTable;
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

      var stringBuilder = new StringBuilder();
      stringBuilder.Append($"# {parameter.Name}\n\n");
      stringBuilder.Append(config.IsBeta ? StringHelper.AddBetaWarning() : string.Empty);

      var tempIconTable = new List<List<string>> {
        new List<string>() {
          FileHelper.CreateIconLink(parameter),
        },
      };
      stringBuilder.Append(TableBuilder.CreateTableString(string.Empty, TableBuilder.TableType.IconOnly,
        tempIconTable));

      if (parameter.Name == "Bool6") {
        stringBuilder.Append(StringHelper.Admonition("Did you know?", AdmonitionType.Info,
          "The `Bool6` icon takes inspiration from the central pin/hinge/charnier connection "
          + "[Ove Arup's Kingsgate footbridge](https://www.arup.com/projects/kingsgate-footbridge"
          + ").\r\n![Kingsgate Footbridge Durham](./images/Kingsgate-Footbridge-Durham.jpg)\r\n"
          + "*(c) Giles Rocholl / Arup*"));
      }

      stringBuilder.Append(StringHelper.SummaryDescription(parameter.Summary, config));

      if (parameter.Properties != null && parameter.Properties.Count != 0) {
        List<List<string>> propertieTable = GetPropertyTempTable(parameter, parameterNames, config);

        stringBuilder.Append(TableBuilder.CreateTableString("Properties", TableBuilder.TableType.InputOutput,
          propertieTable));
        if (parameter.PropertiesComponent != null) {
          string link = FileHelper.CreatePageLink(parameter.PropertiesComponent, config);
          string note = $"Note: the above properties can be retrieved using the {link} component";
          stringBuilder.Append(StringHelper.MakeItalic(note));
        }
      }

      Writer.Write(filePath, stringBuilder.ToString());
    }

    private static List<List<string>> GetPropertyTempTable(
      Parameter parameter, List<string> parameterNames, Configuration config) {
      var propertieTable = new List<List<string>> { };
      propertieTable.AddRange(parameter.Properties.Select(property => new List<string>() {
        FileHelper.CreateIconLink(property),
        FileHelper.CreateParameterLink(property, parameterNames, config),
        StringHelper.MakeBold(property.Name),
        property.Description,
      }));
      return propertieTable;
    }
  }
}
