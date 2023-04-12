using Grasshopper.Kernel;

namespace GsaGH.Helpers.GH {
  public static class IghActiveObjectExtension {

    public static void AddRuntimeError(this IGH_ActiveObject activeObject, string text) {
      activeObject.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, text);
    }

    public static void AddRuntimeRemark(this IGH_ActiveObject activeObject, string text) {
      activeObject.AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, text);
    }

    public static void AddRuntimeWarning(this IGH_ActiveObject activeObject, string text) {
      activeObject.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, text);
    }
  }
}
