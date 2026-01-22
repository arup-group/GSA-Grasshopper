using Grasshopper.Kernel;

namespace GsaGH.Helpers.GH {
  public struct InputAttributes {
    public IGH_Param ParamType { get; set; }
    public string NickName { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public GH_ParamAccess Access { get; set; }
    public bool Optional { get; set; }
  }
}
