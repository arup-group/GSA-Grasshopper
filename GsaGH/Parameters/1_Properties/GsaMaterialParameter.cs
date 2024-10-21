using System;
using System.Drawing;

using Grasshopper.Kernel;

using GsaGH.Helpers;
using GsaGH.Helpers.GH;
using GsaGH.Properties;

using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaMaterialGoo" /> type.
  /// </summary>
  public class GsaMaterialParameter : GH_OasysPersistentParam<GsaMaterialGoo> {
    public override Guid ComponentGuid => new Guid("f13d079b-f7d1-4d8a-be7c-3b7e1e59c5ab");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaMaterialGoo.Name + " parameter" :
        base.InstanceDescription;
    public override string TypeName => SourceCount == 0 ? GsaMaterialGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.MaterialParam;

    public GsaMaterialParameter() : base(new GH_InstanceDescription(GsaMaterialGoo.Name,
      GsaMaterialGoo.NickName, GsaMaterialGoo.Description + " parameter", CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    protected override GsaMaterialGoo PreferredCast(object data) {
      string mes = string.Empty;
      string defaultText = $"{data.GetTypeName()} does not contain a Material";
      switch (data) {
        case GsaSectionGoo section:
          if (section.Value.Material == null) {
            mes = defaultText;
            break;
          }

          return new GsaMaterialGoo(section.Value.Material);

        case GsaProperty2dGoo prop2d:
          if (prop2d.Value.Material == null) {
            mes = defaultText;
            break;
          }

          return new GsaMaterialGoo(prop2d.Value.Material);

        case GsaProperty3dGoo prop3d:
          if (prop3d.Value.Material == null) {
            mes = defaultText;
            break;
          }

          return new GsaMaterialGoo(prop3d.Value.Material);

        case GsaElement1dGoo elem1d:
          if (elem1d.Value.Section == null || elem1d.Value.Section.Material == null) {
            mes = defaultText;
            break;
          }

          return new GsaMaterialGoo(elem1d.Value.Section.Material);

        case GsaElement2dGoo elem2d:
          if (elem2d.Value.Prop2ds.IsNullOrEmpty() || elem2d.Value.Prop2ds[0].Material == null) {
            mes = defaultText;
            break;
          }

          return new GsaMaterialGoo(elem2d.Value.Prop2ds[0].Material);

        case GsaElement3dGoo elem3d:
          if (elem3d.Value.Prop3ds.IsNullOrEmpty() || elem3d.Value.Prop3ds[0].Material == null) {
            mes = defaultText;
            break;
          }

          return new GsaMaterialGoo(elem3d.Value.Prop3ds[0].Material);

        case GsaMember1dGoo mem1d:
          if (mem1d.Value.Section == null || mem1d.Value.Section.Material == null) {
            mes = defaultText;
            break;
          }

          return new GsaMaterialGoo(mem1d.Value.Section.Material);

        case GsaMember2dGoo mem2d:
          if (mem2d.Value.Prop2d == null || mem2d.Value.Prop2d.Material == null) {
            mes = defaultText;
            break;
          }

          return new GsaMaterialGoo(mem2d.Value.Prop2d.Material);

        case GsaMember3dGoo mem3d:
          if (mem3d.Value.Prop3d == null || mem3d.Value.Prop3d.Material == null) {
            mes = defaultText;
            break;
          }

          return new GsaMaterialGoo(mem3d.Value.Prop3d.Material);
      }

      if (GH_Convert.ToInt32(data, out int id, GH_Conversion.Both)) {
        var customMaterial = new GsaReferencedMaterial(id, MatType.Custom);
        return new GsaMaterialGoo(customMaterial);
      }

      if (!string.IsNullOrEmpty(mes)) {
        mes = "." + Environment.NewLine + mes;
      }

      this.AddRuntimeError($"Data conversion failed from {data.GetTypeName()} to Material" + mes);
      return new GsaMaterialGoo(null);
    }
  }
}
