using Grasshopper.Kernel;

namespace GsaGH
{
  public static class IGH_ActiveObjectExtension
  {
    public static void AddRuntimeError(this IGH_ActiveObject activeObject, string text)
    {
      activeObject.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, text);  
    }

    public static void AddRuntimeWarning(this IGH_ActiveObject activeObject, string text)
    {
      activeObject.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, text);
    }

    public static void AddRuntimeRemark(this IGH_ActiveObject activeObject, string text)
    {
      activeObject.AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, text);
    }
  }
}
