using System;
using System.Drawing;

using Grasshopper.Kernel;

using GsaGH.Helpers.GH;
using GsaGH.Properties;

using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaListGoo" /> type.
  /// </summary>
  public class GsaAssemblyListParameter : GH_OasysPersistentParam<GsaListGoo> {
    public override Guid ComponentGuid => new Guid("ddfaf67e-eab1-4bf8-8b57-b4957aedf8eb");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaListGoo.Name + " parameter" :
        base.InstanceDescription;
    public override string TypeName => SourceCount == 0 ? GsaListGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.ListParam;
    public GsaAssemblyListParameter() : base(new GH_InstanceDescription(
      "Assembly filter list",
      "Al",
      $"Filter the Assemblies by list. (by default 'all')",
      CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    protected override GsaListGoo PreferredCast(object data) {
      if (data is GsaAssemblyGoo assembly) {
        var list = new GsaList() {
          EntityType = EntityType.Assembly,
          Definition = assembly.Value.Id.ToString(),
        };

        return new GsaListGoo(list);
      }

      if (GH_Convert.ToString(data, out string text, GH_Conversion.Both)) {
        var list = new GsaList() {
          EntityType = EntityType.Assembly,
          Definition = text
        };

        return new GsaListGoo(list);
      }

      this.AddRuntimeError($"Data conversion failed from {data.GetTypeName()} to Assembly List");
      return new GsaListGoo(null);
    }
  }
}
