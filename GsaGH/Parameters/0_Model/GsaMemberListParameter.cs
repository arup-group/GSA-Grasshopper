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
  public class GsaMemberListParameter : GH_OasysPersistentParam<GsaListGoo> {
    public override Guid ComponentGuid => new Guid("90ecaeae-c26e-4dfa-81f7-08ab28e36348");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaListGoo.Name + " parameter" :
        base.InstanceDescription;
    public override string TypeName => SourceCount == 0 ? GsaListGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.ListParam;
    public GsaMemberListParameter() : base(new GH_InstanceDescription(
      "Member filter list",
      "Me",
      $"Filter import by list. (by default 'all'){Environment.NewLine}" +
      $"Member list should take the form:{Environment.NewLine}" +
      $" 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (Z4 XY55)" +
      $"{Environment.NewLine}Refer to GSA help file for definition of lists and full vocabulary.",
      CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    protected override GsaListGoo PreferredCast(object data) {
      if (data is GsaMember1dGoo member1d) {
        var list = new GsaList() {
          EntityType = EntityType.Member,
          Definition = member1d.Value.Id.ToString(),
        };

        return new GsaListGoo(list);
      }

      if (GH_Convert.ToString(data, out string text, GH_Conversion.Both)) {
        var list = new GsaList() {
          EntityType = EntityType.Member,
          Definition = text
        };

        return new GsaListGoo(list);
      }

      this.AddRuntimeError($"Data conversion failed from {data.GetTypeName()} to Member List");
      return new GsaListGoo(null);
    }
  }
}
