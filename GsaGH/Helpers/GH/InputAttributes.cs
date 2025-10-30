using System.Linq;

using Grasshopper.Kernel;

namespace GsaGH.Helpers.GH {
  public struct InputAttributes {
    public string Name { get; set; }
    public string NickName { get; set; }
    public string Description { get; set; }
    public IGH_Param ParamType { get; set; }
    public GH_ParamAccess Access { get; set; }
    public bool Optional { get; set; }
  }

  public static class CreateParameter {
    public static void Create(
      GH_ComponentParamServer parameters, int index, InputAttributes inputAttribute) {
      parameters.RegisterInputParam(inputAttribute.ParamType, index);
      parameters.Input[index].Name = inputAttribute.Name;
      parameters.Input[index].NickName = inputAttribute.NickName;
      parameters.Input[index].Description = inputAttribute.Description;
      parameters.Input[index].Access = inputAttribute.Access;
      parameters.Input[index].Optional = inputAttribute.Optional;
    }
  }
}
