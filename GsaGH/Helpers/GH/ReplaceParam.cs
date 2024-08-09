using System;
using System.Linq;

using Grasshopper.Kernel;

namespace GsaGH.Helpers.GH {
  public static class ReplaceParam {
    public static void ReplaceInputParameter(
      this GH_ComponentParamServer parameters, IGH_Param newParam, int id, bool optional = false) {
      var sources = parameters.Input[id].Sources.ToList();
      parameters.UnregisterInputParameter(parameters.Input[id], false);
      parameters.RegisterInputParam(newParam, id);
      parameters.Input[id].Optional = optional;
      foreach (IGH_Param source in sources) {
        parameters.Input[id].AddSource(source);
      }
    }

    public static void ReplaceOutputParameter(
      this GH_ComponentParamServer parameters, IGH_Param newParam, int id) {
      Guid guid = parameters.Output[id].InstanceGuid;
      parameters.UnregisterOutputParameter(parameters.Output[id], false);
      parameters.RegisterOutputParam(newParam, id);
      parameters.Output[id].NewInstanceGuid(guid);
    }
  }
}
