using System;
using System.Drawing;

using Grasshopper.Kernel;

using GsaGH.Helpers.GH;
using GsaGH.Properties;

using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaPropertyGoo" /> type.
  /// </summary>
  public class GsaPropertyParameter : GH_OasysPersistentParam<GsaPropertyGoo> {
    public override Guid ComponentGuid => new Guid("e59023e2-c8a4-4341-b2b4-66b675bfc5e5");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaPropertyGoo.Name + " parameter" :
        base.InstanceDescription;
    public override string TypeName => SourceCount == 0 ? GsaPropertyGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.SectionParam;

    public GsaPropertyParameter() : base(new GH_InstanceDescription(GsaPropertyGoo.Name,
      GsaPropertyGoo.NickName, GsaPropertyGoo.Description + " parameter", CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    protected override GsaPropertyGoo PreferredCast(object data) {
      string mes = string.Empty;
      string defaultText = $"{data.GetTypeName()} does not contain a Section";
      switch (data) {
        case GsaSectionGoo section:
          return new GsaPropertyGoo(section.Value);

        case GsaSpringPropertyGoo springProperty:
          return new GsaPropertyGoo(springProperty.Value);
      }

      if (!string.IsNullOrEmpty(mes)) {
        mes = "." + Environment.NewLine + mes;
        this.AddRuntimeError($"Data conversion failed from {data.GetTypeName()} to Section" + mes);
        return new GsaPropertyGoo(null);
      }

      if (GH_Convert.ToInt32(data, out int id, GH_Conversion.Both)) {
        var section = new GsaSection(id);
        return new GsaPropertyGoo(section);
      }

      GH_Convert.ToString(data, out string profile, GH_Conversion.Both);

      if (!GsaSection.IsValidProfile(profile)) {
        this.AddRuntimeError($"Data conversion failed from {data.GetTypeName()} to Section." +
          $"{Environment.NewLine}Invalid profile syntax: {profile}");
        return new GsaPropertyGoo(null);
      } else {
        var section = new GsaSection(profile);
        return new GsaPropertyGoo(section);
      }
    }
  }
}
