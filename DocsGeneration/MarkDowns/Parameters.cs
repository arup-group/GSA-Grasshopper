using GsaGhDocs.Parameters;
using GsaGhDocs.Helpers;
using System.Collections.Generic;
using System;

namespace GsaGhDocs.MarkDowns {
  public class Parameters {
    public static readonly string ParametersOverview =
      "# Parameters\n" +
      "\n" +
      "::: warning" +
      "\nGSA-Grasshopper plugin [GsaGH] is pre-release and under active development, including further testing to be undertaken. It is provided \\\"as-is\\\" and you bear the risk of using it. Future versions may contain breaking changes. Any files, results, or other types of output information created using GsaGH should not be relied upon without thorough and independent checking.\n:::\n" +
      "\n" +
      "The GSA Grasshopper plugin introduces a new set of custom Grasshopper parameters. Parameters are what is passed from one component's output to another component's input.\n" +
      "![Parameters](https://developer.rhino3d.com/api/grasshopper/media/ParameterKinds.png)\n" +
      "\n" +
      "## Custom GSA Parameters" +
      "\n\n";


    public static void CreateOverview(Dictionary<string, List<Parameter>> parameters) {
      string filePath = @"Output\gsagh-parameters.md";
      Console.WriteLine($"Writing {filePath}");

      string text = ParametersOverview;

      var tableHeaders = new List<string>() {
        " ", // icon
        "NickName",
        "Name",
        "Description"
      };

      foreach (string header in parameters.Keys) {
        var table = new Table(header, tableHeaders);
        foreach (Parameter parameter in parameters[header]) {
          table.AddRow(new List<string>(){
            StringHelper.Icon(parameter.Name, "Param"),
            parameter.NickName,
            StringHelper.Link(parameter.Name, "Param"),
            parameter.Description
          });
        }
        text += table.Finalise();
      }

      Writer.Write(filePath, text);
    }

    public static void CreateParameters(List<Parameter> parameters) {
      var parameterNames = new List<string>();
      foreach (Parameter parameter in parameters) {
        parameterNames.Add(parameter.Name.ToUpper());
      }

      foreach (Parameter parameter in parameters) {
        CreateParameter(parameter, parameterNames);
      }
    }

    private static void CreateParameter(Parameter parameter, List<string> parmeterNames) {
      string filePath = StringHelper.FileName(parameter.Name, "Parameter");
      Console.WriteLine($"Writing {filePath}");
      
      string text = $"# {parameter.Name}\n\n";
      text += StringHelper.BetaWarning();

      var iconHeaders = new List<string>() {
        "Icon"
      };
      var iconTable = new Table(string.Empty, iconHeaders);
      iconTable.AddRow(new List<string>() {
        StringHelper.Icon(parameter.Name, "Param"),
      });

      text += iconTable.Finalise();

      if (parameter.Name == "Bool6") {
        text += StringHelper.Tip("Did you know?",
          "The `Bool6` icon takes inspiration from the central pin/hinge/charnier connection [Ove Arup's Kingsgate footbridge](https://www.arup.com/projects/kingsgate-footbridge).\r\n![Kingsgate Footbridge Durham](./images/gsagh/Kingsgate-Footbridge-Durham.jpg)\r\n*(c) Giles Rocholl / Arup*");
      }

      text += StringHelper.SummaryDescription(parameter.Summary);

      if (parameter.Properties != null && parameter.Properties.Count != 0) {
        var headers = new List<string>() {
          "Icon",
          "Type",
          "Name",
          "Description"
        };
        var table = new Table("Properties", headers);
        foreach (Parameter property in parameter.Properties) {
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
  }
}
