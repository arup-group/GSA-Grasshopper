using System;
using System.Drawing;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using GsaGH.Properties;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaEffectiveLengthGoo" /> type.
  /// </summary>
  public class
    GsaEffectiveLengthParameter : GH_OasysPersistentParam<GsaEffectiveLengthGoo> {
    public override Guid ComponentGuid => new Guid("e2349b4f-1ebb-4661-99d9-07c6a3ef22b9");
    public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.obscure;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaEffectiveLengthGoo.Name + " parameter" :
        base.InstanceDescription;
    public override string TypeName
      => SourceCount == 0 ? GsaEffectiveLengthGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.EffectiveLengthParam;

    public GsaEffectiveLengthParameter() : base(new GH_InstanceDescription(
      GsaEffectiveLengthGoo.Name, GsaEffectiveLengthGoo.NickName,
      GsaEffectiveLengthGoo.Description + " parameter", CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    protected override GsaEffectiveLengthGoo PreferredCast(object data) {
      switch (data) {
        case GsaMember1dGoo mem1d:
          var eff = new GsaEffectiveLength(mem1d.Value);
          return new GsaEffectiveLengthGoo(eff);
      }

      this.AddRuntimeError($"Data conversion failed from {data.GetTypeName()} to Effective Length");
      return new GsaEffectiveLengthGoo(null);
    }
  }
}
