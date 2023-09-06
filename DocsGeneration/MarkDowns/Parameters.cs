﻿using System.Collections.Generic;
using System;
using DocsGeneration.Data;
using DocsGeneration.MarkDowns.Helpers;

namespace DocsGeneration.MarkDowns {
  public class Parameters {
    public static readonly string ParametersOverview =
      "# Parameters\n" +
      "\n" +
      "::: warning" +
      "\nGSA-Grasshopper plugin is pre-release and under active development, including further testing to be undertaken. It is provided \\\"as-is\\\" and you bear the risk of using it. Future versions may contain breaking changes. Any files, results, or other types of output information created using the plugin should not be relied upon without thorough and independent checking.\n:::\n" +
      "\n" +
      "The GSA plugin introduces a new set of custom Grasshopper parameters. Parameters are what is passed from one component's output to another component's input.\n" +
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
        "Name",
        "Description"
      };

      var widths = new List<int>() {
        Table.IconWidth,
        Table.NameWidth,
        Table.DescriptionWidth
      };

      foreach (string header in parameters.Keys) {
        var table = new Table(header, 2, tableHeaders, widths);
        foreach (Parameter parameter in parameters[header]) {
          table.AddRow(new List<string>(){
            FileHelper.CreateIconLink(parameter),
            FileHelper.CreatePageLink(parameter),
            parameter.Description.Replace("GSA ", string.Empty)
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
      string filePath = FileHelper.CreateMarkDownFileName(parameter);
      Console.WriteLine($"Writing {filePath}");
      
      string text = $"# {parameter.Name}\n\n";
      text += StringHelper.AddBetaWarning();

      var iconHeaders = new List<string>() {
        "Icon"
      };
      var iconTable = new Table(string.Empty, 2, iconHeaders, new List<int>() { 150 });
      iconTable.AddRow(new List<string>() {
        FileHelper.CreateIconLink(parameter),
      });

      text += iconTable.Finalise();

      if (parameter.Name == "Bool6") {
        text += StringHelper.Tip("Did you know?",
          "The `Bool6` icon takes inspiration from the central pin/hinge/charnier connection " +
          "[Ove Arup's Kingsgate footbridge](https://www.arup.com/projects/kingsgate-footbridge" +
          ").\r\n![Kingsgate Footbridge Durham](./images/Kingsgate-Footbridge-Durham.jpg)\r\n" +
          "*(c) Giles Rocholl / Arup*");
      }

      text += StringHelper.SummaryDescription(parameter.Summary);

      if (parameter.Properties != null && parameter.Properties.Count != 0) {
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

        var table = new Table("Properties", 2, headers, widths);
        foreach (Parameter property in parameter.Properties) {
          table.AddRow(new List<string>() {
            FileHelper.CreateIconLink(property),
            FileHelper.CreateParameterLink(property, parmeterNames),
            StringHelper.MakeBold(property.Name),
            property.Description,
         });
        }

        text += table.Finalise();

        if (parameter.PropertiesComponent != null) {
          string link = FileHelper.CreatePageLink(parameter.PropertiesComponent);
          string note = $"Note: the above properties can be retrieved using the {link} component";
          text += StringHelper.MakeItalic(note);
        }
      }

      Writer.Write(filePath, text);
    }
  }
}
