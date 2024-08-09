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
  public class GsaElementMemberListParameter : GH_OasysPersistentParam<GsaListGoo> {
    public override Guid ComponentGuid => new Guid("54dce20d-a42c-4651-9075-57158ee4e556");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaListGoo.Name + " parameter" :
        base.InstanceDescription;
    public override string TypeName => SourceCount == 0 ? GsaListGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.ListParam;
    public GsaElementMemberListParameter() : base(new GH_InstanceDescription(
      "Element/Member filter List",
      "El",
      $"Filter the Elements or Members by list. (by default 'all'){Environment.NewLine}" +
      $"Element/Member list should take the form:{Environment.NewLine}" +
      $" 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)" +
      $"{Environment.NewLine}Refer to GSA help file for definition of lists and full vocabulary.",
      CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    protected override GsaListGoo PreferredCast(object data) {
      if (data is GsaElement1dGoo element1d) {
        var list = new GsaList() {
          EntityType = EntityType.Element,
          Definition = element1d.Value.Id.ToString(),
        };

        return new GsaListGoo(list);
      }

      if (data is GsaMember1dGoo member1d) {
        var list = new GsaList() {
          EntityType = EntityType.Member,
          Definition = member1d.Value.Id.ToString(),
        };

        return new GsaListGoo(list);
      }

      if (GH_Convert.ToString(data, out string text, GH_Conversion.Both)) {
        var list = new GsaList() {
          EntityType = EntityType.Element,
          Definition = text
        };

        return new GsaListGoo(list);
      }

      this.AddRuntimeError($"Data conversion failed from {data.GetTypeName()} to Element or Member List");
      return new GsaListGoo(null);
    }
  }
}
