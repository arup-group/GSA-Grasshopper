using System;
using System.Drawing;

using Grasshopper.Kernel;

using GsaGH.Components;
using GsaGH.Helpers.GH;
using GsaGH.Properties;

using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaResultGoo" /> type.
  /// </summary>
  public class GsaResultParameter : GH_OasysPersistentParam<GsaResultGoo> {
    public override Guid ComponentGuid => new Guid("81f6f103-cb53-414c-908b-6adf46c3260d");
    public override GH_Exposure Exposure => GH_Exposure.senary | GH_Exposure.obscure;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaResultGoo.Name + " parameter" :
        base.InstanceDescription;
    public override string TypeName => SourceCount == 0 ? GsaResultGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.ResultParam;

    public GsaResultParameter() : base(new GH_InstanceDescription(GsaResultGoo.Name,
      GsaResultGoo.NickName, GsaResultGoo.Description + " parameter", CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    protected override GsaResultGoo PreferredCast(object data) {
      if (data.GetType() == typeof(GsaModelGoo) &&
        Attributes.Parent != null) { // if persistent parameter (this class) parent is null
        IGH_Param modelParam = Sources[0];
        var comp = new SelectResult();
        comp.Attributes.Pivot = new PointF(
          (modelParam.Attributes.Pivot.X + Attributes.Pivot.X) / 2,
          (modelParam.Attributes.Pivot.Y + Attributes.Pivot.Y) / 2);
        OnPingDocument().AddObject(comp, false);
        comp.Params.Input[0].AddSource(modelParam);
        comp.Params.Output[0].CollectData();
        this.AddRuntimeError($"Use 'SelectResults' component to pick results case." +
          $"{Environment.NewLine}Data conversion failed from {data.GetTypeName()} to Result" +
          $"Connect{comp.Params.Output[0].InstanceGuid}");
      } else {
        this.AddRuntimeError($"Data conversion failed from {data.GetTypeName()} to Result");
      }
      return new GsaResultGoo(null);
    }
  }
}
