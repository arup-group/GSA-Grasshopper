using GsaGhDocs.Helpers;
using GsaGhDocs.Parameters;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;

namespace GsaGhDocs.Components {
  public class Component {
    public string Name { get; set; }
    public string NickName { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public int SubCategory { get; set; }
    public string ComponentType { get; set; }
    public List<Parameter> Inputs { get; set; } = new List<Parameter>();
    public List<Parameter> Outputs { get; set; } = new List<Parameter>();
    public Component(Type type) {
      var componentObject = (GH_Component)Activator.CreateInstance(type, null);
      Name = componentObject.Name;
      NickName = componentObject.NickName;
      Description = componentObject.Description;
      Category = componentObject.SubCategory.Trim();
      SubCategory = Exposure.GetExposure(componentObject.Exposure);
      ComponentType = type.BaseType.Name
        .Replace("GH_", string.Empty).Replace("Oasys", string.Empty);
      Inputs = GetParameters(componentObject.Params.Input);
      Outputs = GetParameters(componentObject.Params.Output);
    }

    public override string ToString() {
      return Name;
    }

    public static List<Component> GetComponents(Type[] typelist) {
      Console.WriteLine($"Finding components...");
      var components = new List<Component>();
      foreach (Type type in typelist) {
        if (type.Namespace == null) {
          continue;
        }

        if (type.Namespace.StartsWith("GsaGH.Components")) {
          if (type.Name.Contains("OBSOLETE")) {
            continue;
          }

          try {
            var comp = new Component(type);
            components.Add(comp);
            Console.WriteLine($"Added {comp.Name} component");
          } catch (Exception) {
            continue;
          }
        }
      }
      Console.WriteLine($"Completed finding components - found {components.Count}");
      return components;
    }

    public static Dictionary<string, List<Component>> SortComponents(List<Component> components) {
      Console.WriteLine($"Sorting components by ribbon category");
      var dict = new Dictionary<string, List<Component>>() {
        {
          "Model", new List<Component>()
        }, {
          "Properties", new List<Component>()
        }, {
          "Geometry", new List<Component>()
        }, {
          "Loads", new List<Component>()
        }, {
          "Analysis", new List<Component>()
        }, {
          "Results", new List<Component>()
        }, {
          "Display", new List<Component>()
        },
      };

      foreach (Component component in components) {
        dict[component.Category].Add(component);
      }

      return dict;
    }

    private List<Parameter> GetParameters(List<IGH_Param> parameters) {
      var outparams = new List<Parameter>();
      foreach (IGH_Param param in parameters) {
        var parameter = new Parameter(param.GetType()) {
          Name = param.Name,
          NickName = param.NickName,
          Description = param.Description
        };
        if (parameter.ParameterType == "Generic" &&
          parameter.Name.Contains("[")) {
          parameter.ParameterType = Parameter.CheckIfUnitNumber(parameter.Name);
          parameter.Name = parameter.Name.Replace("[m]",string.Empty);
          parameter.Name = parameter.Name.Replace("[cm]", string.Empty);
          parameter.Name = parameter.Name.Replace("[mm]", string.Empty);
          parameter.Name = parameter.Name.Trim();
        }
        outparams.Add(parameter);
      }
      return outparams;
    }
  }
}
