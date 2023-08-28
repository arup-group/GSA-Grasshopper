using System;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
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
      switch (data) {
        case GsaSectionGoo section: 
            return new GsaMaterialGoo(section.Value.Material);

        case GsaProperty2dGoo prop2d:
          return new GsaMaterialGoo(prop2d.Value.Material);

        case GsaProperty3dGoo prop3d:
          return new GsaMaterialGoo(prop3d.Value.Material);

        case GsaElement1dGoo elem1d:
          return new GsaMaterialGoo(elem1d.Value.Section.Material);

        case GsaElement2dGoo elem2d:
          return new GsaMaterialGoo(elem2d.Value.Prop2ds[0].Material);

        case GsaElement3dGoo elem3d:
          return new GsaMaterialGoo(elem3d.Value.Prop3ds[0].Material);

        case GsaMember1dGoo mem1d:
          return new GsaMaterialGoo(mem1d.Value.Section.Material);

        case GsaMember2dGoo mem2d:
          return new GsaMaterialGoo(mem2d.Value.Prop2d.Material);

        case GsaMember3dGoo mem3d:
          return new GsaMaterialGoo(mem3d.Value.Prop3d.Material);
      }

      if (GH_Convert.ToInt32(data, out int id, GH_Conversion.Both)) {
        var customMaterial = new GsaMaterial(id);
        return new GsaMaterialGoo(customMaterial);
      }

      this.AddRuntimeError($"Data conversion failed from {data.GetTypeName()} to Material");
      return new GsaMaterialGoo(null);
    }
  }
}
