using System;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  /// This class provides a parameter interface for the <see cref="GsaBucklingLengthFactorsGoo"/> type.
  /// </summary>
  public class GsaBucklingLengthFactorsParameter : GH_OasysPersistentParam<GsaBucklingLengthFactorsGoo> {
    public override string InstanceDescription => m_data.DataCount == 0
      ? "Empty " + GsaBucklingLengthFactorsGoo.Name + " parameter"
      : base.InstanceDescription;
    public override string TypeName => SourceCount == 0
      ? GsaBucklingLengthFactorsGoo.Name
      : base.TypeName;
    public override Guid ComponentGuid => new Guid("e2349b4f-1ebb-4661-99d9-07c6a3ef22b9");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.BucklingFactorsParam;

    public GsaBucklingLengthFactorsParameter() : base(new GH_InstanceDescription(
      GsaBucklingLengthFactorsGoo.Name,
      GsaBucklingLengthFactorsGoo.NickName,
      GsaBucklingLengthFactorsGoo.Description + " parameter",
      CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    protected override GsaBucklingLengthFactorsGoo PreferredCast(object data) {
      return data.GetType() == typeof(GsaBucklingLengthFactors)
        ? new GsaBucklingLengthFactorsGoo((GsaBucklingLengthFactors)data)
        : base.PreferredCast(data);
    }
  }
}
