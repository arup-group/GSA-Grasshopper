using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using OasysGH.Parameters;
using OasysUnits;
using Rhino.Geometry;

namespace GsaGHTests.Helpers {
  public class ComponentTestHelper {

    public static object GetOutput(
      GH_Component component, int index = 0, int branch = 0, int item = 0,
      bool forceUpdate = false) {
      if (forceUpdate || component.Params.Output[index].VolatileDataCount == 0) {
        component.ExpireSolution(true);
        component.Params.Output[index].ExpireSolution(true);
        component.Params.Output[index].CollectData();
      }

      return component.Params.Output[index].VolatileData.get_Branch(branch)[item];
    }

    public static object GetOutput(
      GH_Component component, int index, GH_Path path, int item = 0, bool forceUpdate = false) {
      if (forceUpdate || component.Params.Output[index].VolatileDataCount == 0) {
        component.ExpireSolution(true);
        component.Params.Output[index].ExpireSolution(true);
        component.Params.Output[index].CollectData();
      }

      return component.Params.Output[index].VolatileData.get_Branch(path)[item];
    }

    public static object GetOutput(IGH_Param param, int branch = 0, int item = 0) {
      return param.VolatileData.get_Branch(branch)[item];
    }

    public static void SetInput(IGH_Param param, object data, int index = 0) {
      param.CreateAttributes();
      var path = new GH_Path(0);
      param.AddVolatileData(path, index, data);
    }

    public static void SetInput(GH_Component component, string str, int index = 0) {
      var input = new Param_String();
      input.CreateAttributes();
      input.PersistentData.Append(new GH_String(str));
      component.Params.Input[index].AddSource(input);
    }

    public static void SetInput(GH_Component component, bool @bool, int index = 0) {
      var input = new Param_Boolean();
      input.CreateAttributes();
      input.PersistentData.Append(new GH_Boolean(@bool));
      component.Params.Input[index].AddSource(input);
    }

    public static void SetInput(GH_Component component, double number, int index = 0) {
      var input = new Param_Number();
      input.CreateAttributes();
      input.PersistentData.Append(new GH_Number(number));
      component.Params.Input[index].AddSource(input);
    }

    public static void SetInput(GH_Component component, int id, int index = 0) {
      var input = new Param_Integer();
      input.CreateAttributes();
      input.PersistentData.Append(new GH_Integer(id));
      component.Params.Input[index].AddSource(input);
    }

    public static void SetInput(GH_Component component, object obj, int index = 0) {
      var input = new Param_GenericObject();
      input.CreateAttributes();
      if (typeof(IQuantity).IsAssignableFrom(obj.GetType())) {
        input.PersistentData.Append(new GH_UnitNumber((IQuantity)obj));
      } else {
        input.PersistentData.Append(new GH_ObjectWrapper(obj));
      }

      component.Params.Input[index].AddSource(input);
    }

    public static void SetInput(GH_Component component, List<object> objs, int index = 0) {
      var input = new Param_GenericObject();
      input.CreateAttributes();
      input.Access = GH_ParamAccess.list;
      foreach (object obj in objs) {
        input.PersistentData.Append(new GH_ObjectWrapper(obj));
      }

      component.Params.Input[index].AddSource(input);
    }

    public static void SetInput(GH_Component component, Point3d pt, int index = 0) {
      var input = new Param_Point();
      input.CreateAttributes();
      input.PersistentData.Append(new GH_Point(pt));
      component.Params.Input[index].AddSource(input);
    }

    public static void SetInput(GH_Component component, Curve curve, int index = 0) {
      var input = new Param_Curve();
      input.CreateAttributes();
      input.PersistentData.Append(new GH_Curve(curve));
      component.Params.Input[index].AddSource(input);
    }

    public static void SetInput(GH_Component component, Brep brep, int index = 0) {
      var input = new Param_Brep();
      input.CreateAttributes();
      input.PersistentData.Append(new GH_Brep(brep));
      component.Params.Input[index].AddSource(input);
    }

    public static void SetInput(GH_Component component, Mesh mesh, int index = 0) {
      var input = new Param_Mesh();
      input.CreateAttributes();
      input.PersistentData.Append(new GH_Mesh(mesh));
      component.Params.Input[index].AddSource(input);
    }
  }
}
