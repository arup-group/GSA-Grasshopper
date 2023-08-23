using Grasshopper.Kernel;
using DocsGeneration.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using GsaGhDocs.Data.Helpers;

namespace GsaGhDocs.Data {
  public class Parameter {
    public string Name { get; set; }
    public string NickName { get; set; }
    public string Description { get; set; }
    public string Summary { get; set; }
    public int SubCategory { get; set; }
    public string ParameterType { get; set; }
    public List<Parameter> Properties { get; set; }
    public Parameter(Type type, bool summary = false) {
      var persistentParam = (IGH_Param)Activator.CreateInstance(type, null);
      Name = persistentParam.Name.Replace("parameter", string.Empty).Trim();
      if (Name.Contains('[')) {
        Name = Name.Split('[')[0];
      }

      NickName = persistentParam.NickName;
      Description = persistentParam.Description;
      SubCategory = Exposure.GetExposure(persistentParam.Exposure);
      ParameterType = GetParameterType(type);

      if (summary && SubCategory > 0) {
        Summary = GetClassSummary(persistentParam.GetType());
      }
    }

    private string GetParameterType(Type type) {
      if (type.BaseType.GenericTypeArguments[0].Name == "IGH_Goo") {
        return "Generic";
      }

      string s = type.BaseType.GenericTypeArguments[0].Name
        .Replace("Goo", string.Empty).Replace("Gsa", string.Empty)
        .Replace("GH_", string.Empty).Replace("String", "Text"); ;
      s = CheckIfUnitNumber(s);

      return s;
    }

    internal static string CheckIfUnitNumber(string s) {
      if (s.Contains(" in [")) {
        return "Generic";
      }

      if (s.Contains("[m]") || s.Contains("[cm]") || s.Contains("[mm]")) {
        return "UnitNumber `Length`";
      }

      if (s.Contains("[kN]")) {
        return "UnitNumber `Force`";
      }

      if (s.Contains("[kN/m]")) {
        return "UnitNumber `ForcePerLength`";
      }

      if (s.Contains("[kN/m²]") || s.Contains("[MPa]")) {
        return "UnitNumber `Pressure`";
      }

      if (s.Contains("[kN·m]")) {
        return "UnitNumber `Moment`";
      }

      if (s.Contains("[MJ/m³]")) {
        return "UnitNumber `EnergyDensity`";
      }

      return s;
    }

    private string GetClassSummary(Type paramType) {
      PropertyInfo[] paramPropertyInfo = paramType.GetProperties(
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
      foreach (PropertyInfo paramProperty in paramPropertyInfo) {
        if (paramProperty.Name == "PersistentData") {
          Type gooType = paramProperty.DeclaringType.GenericTypeArguments[0];
          PropertyInfo[] gooPropertyInfo = gooType.GetProperties(
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
          foreach (PropertyInfo gooProperty in gooPropertyInfo) {
            if (gooProperty.Name == "Value") {
              //object gooValue = gooProperty.GetValue(gooType, null);
              Type valueType = gooProperty.PropertyType;
              string path = "T:" + valueType.FullName;
              XmlNode xmlDocuOfMethod = GsaGhDll.GsaGhXml.SelectSingleNode(
                  "//member[starts-with(@name, '" + path + "')]");
              string text = xmlDocuOfMethod.InnerXml
                .Replace("<summary>", string.Empty).Replace("</summary>", string.Empty);
              string cleanStr = Regex.Replace(text, @"\s+", " ");
              return cleanStr.Trim();
            }
          }
        }
      }
      return string.Empty;
    }

    public override string ToString() {
      return Name;
    }

    public static List<Parameter> GetParameters(Type[] typelist, List<Component> components) {
      Console.WriteLine($"Finding parameters...");
      var parameters = new List<Parameter>();
      foreach (Type type in typelist) {
        if (type.BaseType == null || !type.BaseType.Name.StartsWith("GH_OasysPersistent")) {
          continue;
        }

        try {
          var param = new Parameter(type, true);
          if (param.SubCategory > 0) {
            param.UpdateParameters(components);
            parameters.Add(param);
            Console.WriteLine($"Added {param.Name} parameter");
          }
        }
        catch (Exception) {
          continue;
        }
      }

      Console.WriteLine($"Completed finding parameters - found {parameters.Count}");
      return parameters;
    }

    public static Dictionary<string, List<Parameter>> SortParameters(List<Parameter> parameters) {
      Console.WriteLine($"Sorting parameters by sub-category");
      var dict = new Dictionary<string, List<Parameter>>() {
        {
          "Model", new List<Parameter>()
        }, {
          "Properties", new List<Parameter>()
        }, {
          "Geometry", new List<Parameter>()
        }, {
          "Loads", new List<Parameter>()
        }, {
          "Analysis", new List<Parameter>()
        }, {
          "Results", new List<Parameter>()
        }, {
          "Display", new List<Parameter>()
        },
      };

      foreach (Parameter parameter in parameters) {
        switch (parameter.SubCategory) {
          case 1:
            dict["Model"].Add(parameter);
            break;
          case 2:
            dict["Properties"].Add(parameter);
            break;
          case 3:
            dict["Geometry"].Add(parameter);
            break;
          case 4:
            dict["Loads"].Add(parameter);
            break;
          case 5:
            dict["Analysis"].Add(parameter);
            break;
          case 6:
            dict["Results"].Add(parameter);
            break;
          case 7:
            dict["Display"].Add(parameter);
            break;
        }
      }

      return dict;
    }

    private void UpdateParameters(List<Component> components) {
      if (Name == "Model") {
        return;
      }

      string parameterName = Name.ToUpper().Replace(" ", string.Empty);
      if (parameterName.ToUpper().Contains("1D")) {
        // fx Element1D => 1D Element
        parameterName = "1D" + parameterName.ToUpper().Replace("1D", string.Empty);
      }

      if (parameterName.ToUpper().Contains("2D")) {
        parameterName = "2D" + parameterName.ToUpper().Replace("2D", string.Empty);
      }

      if (parameterName.ToUpper().Contains("3D")) {
        parameterName = "3D" + parameterName.ToUpper().Replace("3D", string.Empty);
      }

      foreach (Component component in components) {
        string componentName = component.Name.ToUpper().Replace(" ", string.Empty);
        if (componentName.Contains("EDIT") && componentName.Contains(parameterName)) {
          Properties = CleanOutputParams(
            component.Outputs.GetRange(1, component.Outputs.Count - 1));
          return;
        }
      }

      foreach (Component component in components) {
        string componentName = component.Name.ToUpper().Replace(" ", string.Empty);
        if (componentName.Contains("PROPERTIES") && componentName.Contains(parameterName)) {
          Properties = CleanOutputParams(component.Outputs.ToList());
          return;
        }
      }

      foreach (Component component in components) {
        string componentName = component.Name.ToUpper().Replace(" ", string.Empty);
        if (componentName.Contains("INFO") && componentName.Contains(parameterName)) {
          Properties = CleanOutputParams(component.Outputs.ToList());
          return;
        }
      }

      foreach (Component component in components) {
        string componentName = component.Name.ToUpper().Replace(" ", string.Empty);
        if (componentName.Contains("GET") && componentName.Contains(parameterName)) {
          Properties = CleanOutputParams(component.Outputs.ToList());
          return;
        }
      }
    }

    private List<Parameter> CleanOutputParams(List<Parameter> outputParams) {
      var cleaned = new List<Parameter>();
      foreach (Parameter parameter in outputParams) {
        parameter.Description = parameter.Description.Replace("Get", string.Empty).Trim();
        if (parameter.Name.Contains("[")) {
          parameter.Name = parameter.Name.Split('[')[0].Trim();
        }

        parameter.Name = parameter.Name.Replace("parameter", string.Empty).Trim();

        cleaned.Add(parameter);
      }

      return cleaned;
    }
  }
}
