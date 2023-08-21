using Grasshopper.Kernel;
using GsaGhDocs.Helpers;
using System;
using System.Collections.Generic;

namespace GsaGhDocs.Parameters {
  public class Parameter {
    public string Name { get; set; }
    public string NickName { get; set; }
    public string Description { get; set; }
    public int SubCategory { get; set; }
    public Parameter(Type type) {
      var paramObject = (IGH_Param)Activator.CreateInstance(type, null);
      Name = paramObject.Name;
      NickName = paramObject.NickName;
      Description = paramObject.Description;
      SubCategory = Exposure.GetExposure(paramObject.Exposure);
    }

    public static List<Parameter> GetParameters(Type[] typelist) {
      var parameters = new List<Parameter>();

      foreach (Type type in typelist) {
        if (type.Namespace == null) {
          continue;
        }

        if (type.Namespace.StartsWith("GsaGH.Parameter")) {
          try {
            var param = new Parameter(type);
            if (param.SubCategory > 0) {
              parameters.Add(param);
            }
          } catch (Exception) {
            continue;
          }
        }
      }

      return parameters;
    }


  }
}
