using System;
using System.Drawing;

using Grasshopper.Kernel;

using GsaGH.Helpers.GH;
using GsaGH.Properties;

using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaEffectiveLengthOptionsGoo" /> type.
  /// </summary>
  public class
    GsaEffectiveLengthOptionsParameter : GH_OasysPersistentParam<GsaEffectiveLengthOptionsGoo> {
    public override Guid ComponentGuid => new Guid("e2349b4f-1ebb-4661-99d9-07c6a3ef22b9");
    public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.obscure;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaEffectiveLengthOptionsGoo.Name + " parameter" :
        base.InstanceDescription;
    public override string TypeName
      => SourceCount == 0 ? GsaEffectiveLengthOptionsGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.EffectiveLengthOptionsParam;

    public GsaEffectiveLengthOptionsParameter() : base(new GH_InstanceDescription(
      GsaEffectiveLengthOptionsGoo.Name, GsaEffectiveLengthOptionsGoo.NickName,
      GsaEffectiveLengthOptionsGoo.Description + " parameter", CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    protected override GsaEffectiveLengthOptionsGoo PreferredCast(object data) {
      switch (data) {
        case GsaMember1dGoo mem1d:
          var eff = new GsaEffectiveLengthOptions(mem1d.Value);
          return new GsaEffectiveLengthOptionsGoo(eff);
      }

      this.AddRuntimeError($"Data conversion failed from {data.GetTypeName()} to Effective Length Options");
      return new GsaEffectiveLengthOptionsGoo(null);
    }
  }
}
