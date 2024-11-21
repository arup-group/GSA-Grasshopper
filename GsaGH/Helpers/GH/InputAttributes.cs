using System.Linq;

using Grasshopper.Kernel;

namespace GsaGH.Helpers.GH {
  public struct InputAttributes {
    public IGH_Param ParamType;
    public string NickName;
    public string Name;
    public string Description;
    public GH_ParamAccess Access;
    public bool Optional;
  }

  public static class CreateParameter {
    public static void Create(
      GH_ComponentParamServer parameters, IGH_Param parameter, int index, string name, string nickname, string description, GH_ParamAccess access, bool optional = true) {
      parameters.RegisterInputParam(parameter, index);
      parameters.Input[index].Name = name;
      parameters.Input[index].NickName = nickname;
      parameters.Input[index].Description = description;
      parameters.Input[index].Access = access;
      parameters.Input[index].Optional = optional;
    }
  }
}
