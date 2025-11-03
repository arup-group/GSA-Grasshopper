using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;

using DocsGeneration.Data.Helpers;

using Grasshopper.Kernel;

using OasysUnits;

namespace DocsGeneration.Data {
  public class Parameter {
    public string Name { get; set; }
    public string NickName { get; set; }
    public string Description { get; set; }
    public string Summary { get; set; }
    public int SubCategory { get; set; }
    public string ParameterType { get; set; }
    public List<Parameter> Properties { get; set; }
    public Component PropertiesComponent { get; set; }

    public Parameter(Type type, Configuration config, bool summary = false) {
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
        Summary = GetClassSummary(persistentParam.GetType(), config.XmlDocument);
      }
    }

    private string GetParameterType(Type type) {
      if (type.BaseType?.GenericTypeArguments == null || !type.BaseType.GenericTypeArguments.Any()) {
        return CleanUpName(type.BaseType.Name);
      }

      if (type.BaseType.GenericTypeArguments[0]?.Name == "IGH_Goo") {
        return "Generic";
      }

      string s = CleanUpName(type.BaseType.GenericTypeArguments[0].Name);
      s = CheckIfUnitNumber(s);

      return s;
    }

    private static string CleanUpName(string s) {
      s = s.Replace("Goo", string.Empty).Replace("Gsa", string.Empty).Replace("GH_", string.Empty)
       .Replace("String", "Text").Replace("Parameter", string.Empty);
      return s;
    }

    internal static string CheckIfUnitNumber(string s) {
      if (s.Contains(" in [")) {
        return "Generic";
      }

      string un = "UnitNumber";

      if (s.Contains("[")) {
        string measure = s.Split('[')[1];
        measure = "1 " + measure.Split(']')[0];

        var types = Quantity.Infos.Select(x => x.ValueType).ToList();

        Type axialStiffness = types.Where(t => t.Name == "AxialStiffness").ToList()[0];
        types.Remove(axialStiffness);

        Type radiation = types.Where(t => t.Name == "AbsorbedDoseOfIonizingRadiation").ToList()[0];
        types.Remove(radiation);
        Type bendingStiffness = types.Where(t => t.Name == "BendingStiffness").ToList()[0];
        types.Remove(bendingStiffness);
        Type duration = types.Where(t => t.Name == "Duration").ToList()[0];
        Type massFraction = types.Where(t => t.Name == "MassFraction").ToList()[0];
        types.Remove(duration);

        foreach (Type type in types) {
          if (Quantity.TryParse(type, measure, out IQuantity quantity)) {
            return $"{un} `{quantity.QuantityInfo.Name.Trim()}`";
          }
        }

        var alternativeTypes = new List<Type>();
        types.Add(axialStiffness);
        types.Add(bendingStiffness);
        types.Add(duration);
        types.Add(massFraction);
        foreach (Type type in alternativeTypes) {
          if (Quantity.TryParse(type, measure, out IQuantity quantity)) {
            return $"{un} `{quantity.QuantityInfo.Name}`";
          }
        }
      }

      if (s.Contains("[MJ/m³]")) {
        return un + " `EnergyDensity`";
      }

      if (s.Contains("[cm²/cm]")) {
        return un + " `SurfaceArea/UnitLength`";
      }

      if (s.Contains("[cm³/cm]")) {
        return un + " `Volume/UnitLength`";
      }

      if (s.Contains("[/°C]")) {
        return un + " `ThermalExpansion`";
      }

      if (s.Contains("[{ forceUnitAbbreviation = kN, forcePerLengthUnit = kN/m, forcePerAreaUnit = kN/m² }]")) {
        return un;
      }

      return s;
    }

    private string GetClassSummary(Type paramType, XmlDocument document) {
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
              XmlNode xmlDocuOfMethod = document.SelectSingleNode("//member[@name='" + path + "']");
              string text = xmlDocuOfMethod.InnerXml.Replace("<summary>", string.Empty)
               .Replace("</summary>", string.Empty);
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

    public static List<Parameter> GetParameters(Type[] typelist, List<Component> components, Configuration config) {
      Console.WriteLine($"Finding parameters...");
      var parameters = new List<Parameter>();
      foreach (Type type in typelist) {
        if (type.BaseType == null || !(type.Name.Contains("GsaRestraintParameter")
          || type.Name.Contains("GsaReleaseParameter") || type.BaseType.Name.StartsWith("GH_OasysPersistent"))) {
          continue;
        }
        // workaround for GsaRestraintParameter and GsaReleaseParameter that do not inherit from GH_OasysPersistentParam. Need to find a better solution.

        try {
          var param = new Parameter(type, config, true);
          if (param.SubCategory > 0) {
            param.UpdateParameters(components);
            parameters.Add(param);
            Console.WriteLine($"Added {param.Name} parameter");
          }
        } catch (Exception) {
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
      if (Name == "Model" || Name == "Result") {
        return;
      }

      string parameterName = Name.ToUpper().Replace(" ", string.Empty);

      if (parameterName.Contains("1D")) {
        parameterName = "1D" + parameterName.Replace("1D", string.Empty);
      }
      if (parameterName.Contains("2D")) {
        parameterName = "2D" + parameterName.Replace("2D", string.Empty);
      }
      if (parameterName.Contains("3D")) {
        parameterName = "3D" + parameterName.Replace("3D", string.Empty);
      }

      foreach (Component component in components) {
        string componentName = component.Name.ToUpper().Replace(" ", string.Empty);
        if (componentName.Contains("EDIT") && componentName.Contains(parameterName)) {
          Properties = CleanOutputParams(
            component.Outputs.GetRange(1, component.Outputs.Count - 1));
          PropertiesComponent = component;
          return;
        }
      }

      foreach (Component component in components) {
        string componentName = component.Name.ToUpper().Replace(" ", string.Empty);
        if (componentName.Contains("PROPERTIES") && componentName.Contains(parameterName)) {
          Properties = CleanOutputParams(component.Outputs.ToList());
          PropertiesComponent = component;
          return;
        }
      }

      foreach (Component component in components) {
        string componentName = component.Name.ToUpper().Replace(" ", string.Empty);
        if (componentName.Contains("INFO") && componentName.Contains(parameterName)) {
          Properties = CleanOutputParams(component.Outputs.ToList());
          PropertiesComponent = component;
          return;
        }
      }

      foreach (Component component in components) {
        string componentName = component.Name.ToUpper().Replace(" ", string.Empty);
        if (componentName.Contains("GET") && componentName.Contains(parameterName)) {
          Properties = CleanOutputParams(component.Outputs.ToList());
          PropertiesComponent = component;
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
