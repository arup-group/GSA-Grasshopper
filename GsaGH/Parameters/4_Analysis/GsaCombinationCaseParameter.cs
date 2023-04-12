using System;
using System.Drawing;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using GsaGH.Properties;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaCombinationCaseGoo" /> type.
  /// </summary>
  public class GsaCombinationCaseParameter : GH_OasysPersistentParam<GsaCombinationCaseGoo> {
    public GsaCombinationCaseParameter() : base(new GH_InstanceDescription(
      GsaCombinationCaseGoo.Name,
      GsaCombinationCaseGoo.NickName,
      GsaCombinationCaseGoo.Description + " parameter",
      CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    public override string InstanceDescription
      => m_data.DataCount == 0
        ? "Empty " + GsaCombinationCaseGoo.Name + " parameter"
        : base.InstanceDescription;

    public override string TypeName
      => SourceCount == 0
        ? GsaCombinationCaseGoo.Name
        : base.TypeName;

    public override Guid ComponentGuid => new Guid("f8d0651f-b235-473f-ba71-30c97b9497cd");

    public override GH_Exposure Exposure => GH_Exposure.quarternary | GH_Exposure.obscure;

    protected override Bitmap Icon => Resources.CombinationCaseParam;

    protected override GsaCombinationCaseGoo PreferredCast(object data) => data.GetType() == typeof(GsaCombinationCase)
      ? new GsaCombinationCaseGoo((GsaCombinationCase)data)
      : base.PreferredCast(data);
  }
}
