using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;

using GsaGH.Parameters;
using GsaGH.Parameters.Results;

using OasysGH.Components;
using OasysGH.Parameters;

using OasysUnits;

using Rhino.Geometry;

namespace GsaGHTests.Helpers {
  public partial class ComponentTestHelper {

    public static object GetOutput(
      GH_Component component, int index = 0, int branch = 0, int item = 0, bool forceUpdate = false) {
      if (forceUpdate || component.Params.Output[index].VolatileDataCount == 0) {
        ComputeOutput(component, index);
      }

      return component.Params.Output[index].VolatileData.get_Branch(branch)[item];
    }

    public static void ComputeOutput(GH_Component component, int index = 0) {
      component.ExpireSolution(true);
      component.Params.Output[index].ExpireSolution(true);
      component.Params.Output[index].CollectData();
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

    public static IList GetListOutput(GH_Component component, int index = 0, int branch = 0) {
      component.ExpireSolution(true);
      component.Params.Output[index].ExpireSolution(true);
      component.Params.Output[index].CollectData();
      return component.Params.Output[index].VolatileData.get_Branch(branch);
    }

    public static List<IQuantity> GetResultOutput(GH_Component component, int index, GH_Path path = null) {
      if (path == null) {
        component.Params.Output[index].DataMapping = GH_DataMapping.Flatten;
      }

      component.ExpireSolution(true);
      component.Params.Output[index].ExpireSolution(true);
      component.Params.Output[index].CollectData();

      IList<IGH_Goo> output = path == null ? component.Params.Output[index].VolatileData.AllData(false).ToList() :
        (IList<IGH_Goo>)component.Params.Output[index].VolatileData.get_Branch(path);
      return output.Select(x => ((GH_UnitNumber)x).Value).ToList();
    }

    public static List<IGH_Goo> GetResultOutputAllData(GH_Component component, int index, GH_Path path = null) {
      if (path == null) {
        component.Params.Output[index].DataMapping = GH_DataMapping.Flatten;
      }

      component.ExpireSolution(true);
      component.Params.Output[index].ExpireSolution(true);
      component.Params.Output[index].CollectData();

      return component.Params.Output[index].VolatileData.AllData(false).ToList();
    }

    public static IList<GH_Path> GetPathOutput(GH_Component component, int index) {
      component.ExpireSolution(true);
      component.Params.Output[index].ExpireSolution(true);
      component.Params.Output[index].CollectData();

      return component.Params.Output[index].VolatileData.Paths;
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
      Param_GenericObject input = GetGenericParameterFor(obj);
      component.Params.Input[index].AddSource(input);
    }

    public static void SetInput(GH_Component component, object obj, string name) {
      int index = component.Params.IndexOfInputParam(name);
      SetInput(component, obj, index);
    }

    private static Param_GenericObject GetGenericParameterFor(object obj) {
      var input = new Param_GenericObject();
      input.CreateAttributes();
      if (typeof(IQuantity).IsAssignableFrom(obj.GetType())) {
        input.PersistentData.Append(new GH_UnitNumber((IQuantity)obj));
      } else {
        input.PersistentData.Append(new GH_ObjectWrapper(obj));
      }

      return input;
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

    public static void SetListInput(GH_Component component, List<object> objs, int index = 0) {
      var input = new Param_GenericObject();
      input.CreateAttributes();
      input.Access = GH_ParamAccess.list;
      foreach (object obj in objs) {
        input.PersistentData.Append(new GH_ObjectWrapper(obj));
      }

      component.Params.Input[index].AddSource(input);
    }

    public static void SetResultInput(GH_Component component, GsaResult result) {
      var input = new GsaResultParameter();
      var resultGoo = new GsaResultGoo(result);
      input.PersistentData.Append(resultGoo);
      component.Params.Input[0].AddSource(input);
    }

    #region extra types output

    public static Color GetColorOutput(GH_OasysComponent component, int index) {
      var colour = (GH_Colour)GetOutput(component, index);
      return colour.Value;
    }

    public static Curve GetCurveOutput(GH_OasysComponent component, int index) {
      var curve = (GH_Curve)GetOutput(component, index);
      return curve.Value;
    }

    public Line GetLineOutput(GH_OasysComponent component, int index) {
      var line = (GH_Line)GetOutput(component, index);
      return line.Value;
    }

    public static Brep GetBrepOutput(GH_OasysComponent component, int index) {
      var output = (GH_Brep)GetOutput(component, index);
      return output.Value;
    }

    #endregion

    #region Goos output

    public static GsaElement1d GetElement1dOutput(GH_OasysComponent component, int index) {
      var element = (GsaElement1dGoo)GetOutput(component, index);
      return element.Value;
    }

    public static GsaMember1d GetMember1dOutput(GH_OasysComponent component, int index) {
      var member = (GsaMember1dGoo)GetOutput(component, index);
      return member.Value;
    }

    public static GsaMember2d GetMember2dOutput(GH_OasysComponent component, int index) {
      var member = (GsaMember2dGoo)GetOutput(component, index);
      return member.Value;
    }

    public IGsaProperty GetPropertyOutput(GH_OasysComponent component, int index) {
      var section = (GsaPropertyGoo)GetOutput(component, index);
      return section.Value;
    }

    public static GsaProperty2d Get2dPropertyOutput(GH_OasysComponent component, int index) {
      var output = (GsaProperty2dGoo)GetOutput(component, index);
      return output.Value;
    }

    public static GsaSpringProperty GetSpringPropertyOutput(GH_OasysComponent component, int index) {
      var property = (GsaSpringPropertyGoo)GetOutput(component, index);
      return property.Value;
    }

    public static GsaNode GetNodeOutput(GH_OasysComponent component, int index) {
      var orientation = (GsaNodeGoo)GetOutput(component, index);
      return orientation.Value;
    }

    public static GsaSection GetSectionOutput(GH_OasysComponent component, int index) {
      var section = (GsaSectionGoo)GetOutput(component, index);
      return section.Value;
    }

    public static GsaOffset GetOffsetOutput(GH_OasysComponent component, int index) {
      var output = (GsaOffsetGoo)GetOutput(component, index);
      return output.Value;
    }

    public static GsaBool6 GetBool6Output(GH_OasysComponent component, int index) {
      var startRelease = (GsaBool6Goo)GetOutput(component, index);
      return startRelease.Value;
    }

    public static GsaEffectiveLengthOptions GetEffectiveLengthOutput(GH_OasysComponent component, int index) {
      var effectiveLength = (GsaEffectiveLengthOptionsGoo)GetOutput(component, index);
      return effectiveLength.Value;
    }

    #endregion

    #region base Types output

    public static bool GetBoolOutput(GH_OasysComponent component, int index) {
      var output = (GH_Boolean)GetOutput(component, index);
      return output.Value;
    }

    public static double GetNumberOutput(GH_OasysComponent component, int index) {
      var output = (GH_Number)GetOutput(component, index);
      return output.Value;
    }

    public static int GetIntOutput(GH_OasysComponent component, int index) {
      var output = (GH_Integer)GetOutput(component, index);
      return output.Value;
    }

    public static string GetStringOutput(GH_OasysComponent component, int index) {
      var output = (GH_String)GetOutput(component, index);
      return output.Value;
    }

    #endregion

  }
}
