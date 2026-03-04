using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using DocsGeneration.Data;

using Helpers;

namespace DocsGeneration.MarkDowns.Helpers {
  public class FileHelper {
    public static string iconPath = "./images/";
    public static List<string> iconNames = new List<string>();

    public static string CreateMarkDownFileName(Parameter parameter, Configuration config) {
      string fileLink = CreateFileName(parameter.Name, "parameter", config.ProjectName);
      return $@"{config.OutputPath}\{fileLink}.md";
    }

    public static string CreateMarkDownFileName(Component component, Configuration config) {
      string fileLink = CreateFileName(component.Name, "component", config.ProjectName);
      return $@"{config.OutputPath}\{fileLink}.md";
    }

    public static string CreateIconLink(Component component) {
      string name = component.Name.Replace("3D", "3d").Replace("2D", "2d").Replace("1D", "1d");
      name = name.ToPascalCase();
      iconNames.Add(name);
      return $"![{component.Name}]({iconPath}{name}.png)";
    }

    public static string CreateIconLink(Parameter parameter) {
      if (parameter.ParameterType.StartsWith("UnitNumber")) {
        iconNames.Add("UnitParam");
        return $"![UnitNumber]({iconPath}UnitParam.png)";
      }

      string name = parameter.ParameterType.Replace(" (List)", string.Empty);
      name = name.Replace(" (Tree)", string.Empty);
      name = name.Replace(" ", string.Empty);
      if (name.Contains("LoadValue") && name.Contains("kN,kN/m,kN/m²")) {
        name = "Load";
      }

      name = $"{name}Param";
      iconNames.Add(name);
      // ![Material](./images/gsagh/MaterialParam.png)
      return $"![{name}]({iconPath}{name}.png)";
    }

    public static void WriteIconNames(string outputPath) {
      string text = string.Join("\r\n", iconNames.Distinct().OrderBy(x => x));
      Writer.Write($@"{outputPath}\Helper\iconNames.txt", text);
    }

    public static string CreateParameterLink(Parameter parameter, List<string> parameterNames, Configuration config) {
      string parameterName = parameter.ParameterType;
      parameterName = parameterName.Replace(" (List)", string.Empty);
      parameterName = parameterName.Replace(" (Tree)", string.Empty);
      parameterName = SplitCamelCase(parameterName, " ");
      string list = parameter.ParameterType.Contains(" (List)") ? " _List_" : string.Empty;
      string tree = parameter.ParameterType.Contains(" (Tree)") ? " _Tree_" : string.Empty;

      if (parameterNames.Contains(parameterName.ToUpper())) {
        string fileName = CreateFileName(parameterName, "parameter", config.ProjectName);

        parameterName = parameterName.Replace(" 3d", " 3D").Replace(" 2d", " 2D").Replace(" 1d", " 1D");

        return $"[{parameterName}]({fileName}.md)" + list + tree;
      }

      if (parameterName.Contains("Unit Number")) {
        string link = $"[Unit Number](gsagh-unitnumber-parameter.md)";
        return parameterName.Replace(" `", "`").Replace("` ", "`").Replace("Unit Number", link) + list + tree;
      }

      parameterName = parameterName.Replace("I Geometric", "Geometry");
      if (!parameterName.StartsWith("[")) {
        parameterName = $"`{parameterName}`";
      }

      return parameterName + list + tree;
    }

    public static string CreateSideBarFileName(string fileLink) {
      fileLink = fileLink.Replace(@"\", "/");
      return fileLink.TrimStart('/');
    }

    public static string CreatePageLink(Parameter parameter, string projectName) {
      string fileName = CreateFileName(parameter, projectName);
      return $"[{parameter.Name}]({fileName}.md)";
    }

    public static string CreatePageLink(Component component, Configuration config) {
      string fileName = CreateFileName(component, config.ProjectName);
      return $"[{component.Name}]({fileName}.md)";
    }

    public static string CreateFileName(Component component, string projectName) {
      return CreateFileName(component.Name, "component", projectName);
    }

    public static string CreateFileName(Parameter parameter, string projectName) {
      return CreateFileName(parameter.Name, "parameter", projectName);
    }

    internal static string CreateFileName(string name, string postfix, string prefix) {
      string spacer = "-";
      return $"{prefix.ToLower()}{spacer}" + $"{name.Replace(" ", "-").ToLower()}{spacer}" + $"{postfix.ToLower()}";
    }

    internal static string SplitCamelCase(string s, string spacer) {
      // `CreateModel` => `Create Model`
      var r = new Regex(@"
        (?<=[A-Z])(?=[A-Z][a-z]) |
        (?<=[^A-Z])(?=[A-Z]) |
        (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);
      return r.Replace(s, spacer).Replace("Bool 6", "Bool6");
    }
  }
}
