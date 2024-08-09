using System;
using System.Drawing;

using Grasshopper.Kernel;

using GsaGH.Helpers.GH;
using GsaGH.Properties;

using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaSectionGoo" /> type.
  /// </summary>
  public class GsaSectionParameter : GH_OasysPersistentParam<GsaSectionGoo> {
    public override Guid ComponentGuid => new Guid("8500f335-fad7-46a0-b1be-bdad22ab1474");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaSectionGoo.Name + " parameter" :
        base.InstanceDescription;
    public override string TypeName => SourceCount == 0 ? GsaSectionGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.SectionParam;

    public GsaSectionParameter() : base(new GH_InstanceDescription(GsaSectionGoo.Name,
      GsaSectionGoo.NickName, GsaSectionGoo.Description + " parameter", CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    protected override GsaSectionGoo PreferredCast(object data) {
      string mes = string.Empty;
      string defaultText = $"{data.GetTypeName()} does not contain a Section";
      switch (data) {
        case GsaElement1dGoo elem1d:
          if (elem1d.Value.Section == null) {
            mes = defaultText;
            break;
          }

          return new GsaSectionGoo(elem1d.Value.Section);

        case GsaMember1dGoo mem1d:
          if (mem1d.Value.Section == null) {
            mes = defaultText;
            break;
          }

          return new GsaSectionGoo(mem1d.Value.Section);

        case GsaPropertyGoo prop:
          if (prop.Value is GsaSection section) {
            return new GsaSectionGoo(section);
          } else {
            this.AddRuntimeError($"Data conversion failed from Spring to Section" + mes);
            return new GsaSectionGoo(null);
          }
      }

      if (!string.IsNullOrEmpty(mes)) {
        mes = "." + Environment.NewLine + mes;
        this.AddRuntimeError($"Data conversion failed from {data.GetTypeName()} to Section" + mes);
        return new GsaSectionGoo(null);
      }

      if (GH_Convert.ToInt32(data, out int id, GH_Conversion.Both)) {
        var section = new GsaSection(id);
        return new GsaSectionGoo(section);
      }

      GH_Convert.ToString(data, out string profile, GH_Conversion.Both);

      if (!GsaSection.IsValidProfile(profile)) {
        this.AddRuntimeError($"Data conversion failed from {data.GetTypeName()} to Section." +
          $"{Environment.NewLine}Invalid profile syntax: {profile}");
        return new GsaSectionGoo(null);
      } else {
        var section = new GsaSection(profile);
        return new GsaSectionGoo(section);
      }
    }
  }
}
