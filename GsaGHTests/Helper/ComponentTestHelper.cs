using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace ComposGHTests.Helpers
{
  public class ComponentTestHelper
  {
    public static void SetInput(GH_Component component, string text_input, int index = 0)
    {
      var input = new Param_String();
      input.CreateAttributes();
      input.PersistentData.Append(new GH_String(text_input));
      component.Params.Input[index].AddSource(input);
    }

    public static void SetInput(GH_Component component, bool bool_input, int index = 0)
    {
      var input = new Param_Boolean();
      input.CreateAttributes();
      input.PersistentData.Append(new GH_Boolean(bool_input));
      component.Params.Input[index].AddSource(input);
    }

    public static void SetInput(GH_Component component, double number_input, int index = 0)
    {
      var input = new Param_Number();
      input.CreateAttributes();
      input.PersistentData.Append(new GH_Number(number_input));
      component.Params.Input[index].AddSource(input);
    }

    public static void SetInput(GH_Component component, object generic_input, int index = 0)
    {
      var input = new Param_GenericObject();
      input.CreateAttributes();
      input.PersistentData.Append(new GH_ObjectWrapper(generic_input));
      component.Params.Input[index].AddSource(input);
    }

    public static void SetInput(GH_Component component, List<object> generic_input, int index = 0)
    {
      var input = new Param_GenericObject();
      input.CreateAttributes();
      input.Access = GH_ParamAccess.list;
      foreach (object obj in generic_input)
        input.PersistentData.Append(new GH_ObjectWrapper(obj));
      component.Params.Input[index].AddSource(input);
    }

    public static void SetInput(GH_Component component, Point3d point_input, int index = 0)
    {
      var input = new Param_Point();
      input.CreateAttributes();
      input.PersistentData.Append(new GH_Point(point_input));
      component.Params.Input[index].AddSource(input);
    }

    public static void SetInput(GH_Component component, Curve curve_input, int index = 0)
    {
      var input = new Param_Curve();
      input.CreateAttributes();
      input.PersistentData.Append(new GH_Curve(curve_input));
      component.Params.Input[index].AddSource(input);
    }

    public static void SetInput(GH_Component component, Brep brep_input, int index = 0)
    {
      var input = new Param_Brep();
      input.CreateAttributes();
      input.PersistentData.Append(new GH_Brep(brep_input));
      component.Params.Input[index].AddSource(input);
    }

    public static void SetInput(GH_Component component, Mesh mesh_input, int index = 0)
    {
      var input = new Param_Mesh();
      input.CreateAttributes();
      input.PersistentData.Append(new GH_Mesh(mesh_input));
      component.Params.Input[index].AddSource(input);
    }

    public static object GetOutput(GH_Component component, int index = 0, int branch = 0, int item = 0, bool forceUpdate = false)
    {
      if (forceUpdate || component.Params.Output[index].VolatileDataCount == 0)
      {
        component.ExpireSolution(true);
        component.Params.Output[index].ExpireSolution(true);
        component.Params.Output[index].CollectData();
      }
      return component.Params.Output[index].VolatileData.get_Branch(branch)[item];
    }
  }
}
