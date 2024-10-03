using System;

using Grasshopper.Kernel;

namespace GsaGH.Helpers.GH {
  public struct InputAttributes {
    public Type ParamType;
    public string NickName;
    public string Name;
    public string Description;
    public GH_ParamAccess Access;
    public bool Optional;
  }
}
