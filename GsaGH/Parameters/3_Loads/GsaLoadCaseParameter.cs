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
    public override Guid ComponentGuid => new Guid("2833ef04-c595-4b05-8db3-622c75fa9a25");
    public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.obscure;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaLoadGoo.Name + " parameter" :
        base.InstanceDescription;
    public override string TypeName => SourceCount == 0 ? GsaLoadGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.LoadCaseParam;

    public GsaLoadCaseParameter() : base(new GH_InstanceDescription(GsaLoadCaseGoo.Name,
      GsaLoadCaseGoo.NickName, GsaLoadCaseGoo.Description + " parameter", CategoryName.Name(),
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
