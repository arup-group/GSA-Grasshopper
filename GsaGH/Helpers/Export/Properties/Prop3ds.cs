using System.Collections.Generic;
using System.Linq;
using GsaGH.Parameters;

namespace GsaGH.Helpers.Export {
  internal partial class ModelAssembly {
    private int AddProp3d(GsaProperty3d prop) {
      AddMaterial(ref prop);

      if (prop.Id <= 0) {
        return _prop3ds.AddValue(prop.Guid, prop.ApiProp3d);
      }

      _prop3ds.SetValue(prop.Id, prop.Guid, prop.ApiProp3d);
      return prop.Id;
    }

    private void ConvertProp3ds(List<GsaProperty3d> prop3Ds) {
      if (prop3Ds == null) {
        return;
      }

      prop3Ds = prop3Ds.OrderByDescending(p => p.Id).ToList();
      foreach (GsaProperty3d prop3D in prop3Ds.Where(prop3D => prop3D != null)) {
        ConvertProp3d(prop3D);
      }
    }

    private int ConvertProp3d(GsaProperty3d prop3d) {
      if (prop3d == null) {
        return 0;
      }

      if (prop3d.IsReferencedById || prop3d.ApiProp3d == null) {
        return prop3d.Id;
      }

      return AddProp3d(prop3d);
    }
  }
}
