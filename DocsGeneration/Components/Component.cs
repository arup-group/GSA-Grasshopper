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
    public List<Parameter> Inputs { get; set; } = new List<Parameter>();
    public List<Parameter> Outputs { get; set; } = new List<Parameter>();
    public Component(Type type) {
      var componentObject = (GH_Component)Activator.CreateInstance(type, null);
      Name = componentObject.Name;
      NickName = componentObject.NickName;
      Description = componentObject.Description;
      Category = componentObject.SubCategory.Trim();
      SubCategory = Exposure.GetExposure(componentObject.Exposure);
      Inputs = GetParameters(componentObject.Params.Input);
      Outputs = GetParameters(componentObject.Params.Output);
    }

    public static List<Component> GetComponents(Type[] typelist) {
      var components = new List<Component>();

      foreach (Type type in typelist) {
        if (type.Namespace == null) {
          continue;
        }

        if (type.Namespace.StartsWith("GsaGH.Components")) {
          var comp = new Component(type);
          if (!comp.Name.Contains("OBSOLETE")) {
            components.Add(comp);
          }
        }
      }

      return components;
    }

    private List<Parameter> GetParameters(List<IGH_Param> parameters) {
      var outparams = new List<Parameter>();
      foreach (IGH_Param param in parameters) {
        outparams.Add(new Parameter(param.GetType()));
      }
      return outparams;
    }
  }
}
