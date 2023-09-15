using System.Collections.Generic;
using System.Linq;
using GsaAPI;
using GsaGH.Parameters;

namespace GsaGH.Helpers.Export {
  internal class Prop3ds {
    internal static void ConvertProp3ds(
      List<GsaProperty3d> prop3Ds, ref Properties existingProperties) {
      if (prop3Ds == null) {
        return;
      }

      prop3Ds = prop3Ds.OrderByDescending(p => p.Id).ToList();
      foreach (GsaProperty3d prop3D in prop3Ds.Where(prop3D => prop3D != null)) {
        ConvertProp3d(prop3D, ref existingProperties);
      }
    }

    internal static int ConvertProp3d(GsaProperty3d prop3d, ref Properties existingProperties) {
      if (prop3d == null) {
        return 0;
      }

      if (prop3d.IsReferencedById || prop3d.ApiProp3d == null) {
        return prop3d.Id;
      }

      return AddProp3d(prop3d, ref existingProperties);
    }

    internal static int AddProp3d(GsaProperty3d prop, ref Properties existingProperties) {
      existingProperties.Materials.AddMaterial(ref prop);

      if (prop.Id <= 0) {
        return existingProperties.Prop3ds.AddValue(prop.Guid, prop.ApiProp3d);
      }

      existingProperties.Prop3ds.SetValue(prop.Id, prop.Guid, prop.ApiProp3d);
      return prop.Id;
    }
  }
}