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
    public override Guid ComponentGuid => new Guid("f8d0651f-b235-473f-ba71-30c97b9497cd");
    public override GH_Exposure Exposure => GH_Exposure.quinary | GH_Exposure.obscure;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaCombinationCaseGoo.Name + " parameter" :
        base.InstanceDescription;
    public override string TypeName
      => SourceCount == 0 ? GsaCombinationCaseGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.CombinationCaseParam;

    public GsaCombinationCaseParameter() : base(new GH_InstanceDescription(
      GsaCombinationCaseGoo.Name, GsaCombinationCaseGoo.NickName,
      GsaCombinationCaseGoo.Description + " parameter", CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    protected override GsaCombinationCaseGoo PreferredCast(object data) {
      if (GH_Convert.ToInt32(data, out int id, GH_Conversion.Both)) {
        return new GsaCombinationCaseGoo(
          new GsaCombinationCase(id, "Combination Case " + id, string.Empty));
      }

      if (GH_Convert.ToString(data, out string name, GH_Conversion.Both)) {
        return new GsaCombinationCaseGoo(new GsaCombinationCase(0, "Combination Case", name));
      }

      this.AddRuntimeError($"Data conversion failed from {data.GetTypeName()} to CombinationCase");
      return new GsaCombinationCaseGoo(null);
    }
  }
}
