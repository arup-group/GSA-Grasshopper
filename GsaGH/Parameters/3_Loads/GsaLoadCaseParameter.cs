using System;
using System.Drawing;

using Grasshopper.Kernel;

using GsaGH.Helpers.GH;
using GsaGH.Properties;

using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaLoadCaseGoo" /> type.
  /// </summary>
  public class GsaLoadCaseParameter : GH_OasysPersistentParam<GsaLoadCaseGoo> {
    public override Guid ComponentGuid => new Guid("5e21f03d-8851-495b-9837-122c9a3a0d67");
    public override GH_Exposure Exposure => GH_Exposure.quarternary | GH_Exposure.obscure;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaLoadCaseGoo.Name + " parameter" :
        base.InstanceDescription;
    public override string TypeName => SourceCount == 0 ? GsaLoadCaseGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.LoadCaseParam;

    public GsaLoadCaseParameter() : base(new GH_InstanceDescription(
      GsaLoadCaseGoo.Name, GsaLoadCaseGoo.NickName,
      GsaLoadCaseGoo.Description + " parameter", CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    protected override GsaLoadCaseGoo PreferredCast(object data) {
      if (GH_Convert.ToInt32(data, out int id, GH_Conversion.Both)) {
        return new GsaLoadCaseGoo(new GsaLoadCase(id));
      }

      this.AddRuntimeError($"Data conversion failed from {data.GetTypeName()} to LoadCase");
      return new GsaLoadCaseGoo(null);
    }
  }
}
