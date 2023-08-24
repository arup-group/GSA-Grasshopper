using System.Collections.Generic;
using System.IO;
using System.Linq;
using DocsGeneration.Data;

namespace DocsGeneration.MarkDowns.Helpers {
  public class FileHelper {
    public static List<string> iconNames = new List<string>();
    public static string CreateMarkDownFileName(Parameter parameter) {
      string fileLink = CreateFileName(parameter);
      return $@"Output\{fileLink}.md";
    }
    public static string CreateMarkDownFileName(Component component) {
      string fileLink = CreateFileName(component);
      return $@"Output\{fileLink}.md";
    }

    public static string CreateIconLink(Component component) {
      string name = component.Name.Replace(" ", string.Empty);
      iconNames.Add(name);
      return $"![{component.Name}](./images/gsagh/{name}.png)";
    }
    public static string CreateIconLink(Parameter parameter) {
      if (parameter.ParameterType.StartsWith("UnitNumber")) {
        iconNames.Add("UnitParam");
        return $"![UnitNumber](./images/gsagh/UnitParam.png)";
      }

      string name = parameter.ParameterType.Replace(" (List)", string.Empty);
      name = name.Replace(" (Tree)", string.Empty);
      name = name.Replace(" ", string.Empty);
      name = $"{name}Param";
      iconNames.Add(name);
      // ![Material](./images/gsagh/MaterialParam.png)
      return $"![{name}](./images/gsagh/{name}.png)";
    }

    public static void WriteIconNames() {
      string text = string.Join("\r\n", iconNames.Distinct().OrderBy(x => x));
      Writer.Write($@"Output\iconNames.txt", text);
    }

    public static string CreatePageLink(Parameter parameter) {
      // [Material](gsagh-material-parameter.html)
      string name = parameter.Name.Replace(" ", "-");
      return $"[{parameter.Name}](./gsagh/parameters/gsagh-{name.ToLower()}-parameter.html)";
    }
    public static string CreatePageLink(Component component) {
      string name = component.Name.Replace(" ", "-");
      return $"[{component.Name}]" +
        $"(./gsagh/components/{component.Category.ToLower()}/gsagh-{name.ToLower()}-parameter.html)";
    }

    public static string CreateParameterLink(Parameter parameter, List<string> parameterNames) {
      string parameterName = parameter.ParameterType;
      parameterName = parameterName.Replace(" (List)", string.Empty);
      parameterName = parameterName.Replace(" (Tree)", string.Empty);
      string list = parameter.ParameterType.Contains(" (List)") ? " (List)" : string.Empty;
      string tree = parameter.ParameterType.Contains(" (Tree)") ? " (Tree)" : string.Empty;

      if (parameterNames.Contains(parameterName.ToUpper())) {
        string name = parameterName.Replace(" ", "-");
        return
          $"[{parameterName}](./gsagh/parameters/gsagh-{name.ToLower()}-parameter.html)" + list + tree;
      }

      string link = $"[UnitNumber](gsagh-unitnumber-parameter.html)";
      string unitNumber = parameter.ParameterType.Replace("UnitNumber", link);

      return unitNumber;
    }

    public static string CreateSideBarFileName(Component component) {
      string fileLink = CreateFileName(component);
      fileLink = fileLink.Replace(@"\", "/");
      return fileLink.TrimStart('/');
    }
    public static string CreateSideBarFileName(Parameter parameter) {
      string fileLink = CreateFileName(parameter);
      fileLink = fileLink.Replace(@"\", "/");
      return fileLink.TrimStart('/');
    }

    private static string CreateFileName(Component component) {
      string path = $@"\gsagh\components\{component.Category.ToLower()}\";
      string name = component.Name.Replace(" ", string.Empty);
      return $"{path}gsagh-{name.ToLower()}-component";
    }
    private static string CreateFileName(Parameter parameter) {
      string path = $@"\gsagh\parameters\";
      string name = parameter.Name.Replace(" ", string.Empty);
      return $"{path}gsagh-{name.ToLower()}-parameter";
    }
  }
}
