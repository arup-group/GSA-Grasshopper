using Grasshopper.Kernel;
using GsaGhDocs.Components;
using GsaGhDocs.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GsaGhDocs.Parameters {
  public class Parameter {
    public string Name { get; set; }
    public string NickName { get; set; }
    public string Description { get; set; }
    public int SubCategory { get; set; }
    public List<Parameter> Parameters { get; set; }
    public Parameter(Type type) {
      var paramObject = (IGH_Param)Activator.CreateInstance(type, null);
      Name = paramObject.Name;
      NickName = paramObject.NickName;
      Description = paramObject.Description;
      SubCategory = Exposure.GetExposure(paramObject.Exposure);
    }

    public static List<Parameter> GetParameters(Type[] typelist, List<Component> components) {
      var parameters = new List<Parameter>();

      foreach (Type type in typelist) {
        if (type.Namespace == null) {
          continue;
        }

        if (type.Namespace.StartsWith("GsaGH.Parameter")) {
          try {
            var param = new Parameter(type);
            if (param.SubCategory > 0) {
              param.UpdateParameters(components);
              parameters.Add(param);
            }
          } catch (Exception) {
            continue;
          }
        }
      }

      return parameters;
    }

    private void UpdateParameters(List<Component> components) {
      if (Name == "Model") {
        return;
      }
      
      foreach (Component component in components) {
        if (component.Name.Contains("Edit") && component.Name.Contains(Name)) {
          Parameters = component.Outputs.GetRange(1, component.Outputs.Count - 1);
          return;
        }
      }

      foreach (Component component in components) {
        if (component.Name.Contains("Properties") && component.Name.Contains(Name)) {
          Parameters = component.Outputs.ToList();
          return;
        }
      }

      foreach (Component component in components) {
        if (component.Name.Contains("Info") && component.Name.Contains(Name)) {
          Parameters = component.Outputs.ToList();
          return;
        }
      }

      foreach (Component component in components) {
        if (component.Name.Contains("Get") && component.Name.Contains(Name)) {
          Parameters = component.Outputs.ToList();
          return;
        }
      }
    }
  }
}
