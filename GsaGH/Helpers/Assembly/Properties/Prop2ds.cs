using System.Collections.Generic;
using System.Linq;

using GsaAPI;

using GsaGH.Parameters;

using Rhino.Geometry;

namespace GsaGH.Helpers.Assembly {
  internal partial class ModelAssembly {
    private int AddProp2d(GsaProperty2d prop) {
      Prop2D api = prop.DuplicateApiObject();
      AddMaterial(prop.Material, ref api);

      if (prop.LocalAxis != null && prop.LocalAxis.IsValid) {
        if (prop.LocalAxis != Plane.WorldXY && prop.LocalAxis != Plane.Unset) {
          Axis ax = prop.GetAxisFromPlane(_unit);
          api.AxisProperty = _axes.AddValue(ax);
        } else {
          api.AxisProperty = 0;
        }
      }

      if (prop.Id <= 0) {
        return _prop2ds.AddValue(prop.Guid, api);
      }

      _prop2ds.SetValue(prop.Id, prop.Guid, api);
      return prop.Id;
    }

    private int ConvertProp2d(GsaProperty2d prop2d) {
      if (prop2d == null) {
        return 0;
      }

      if (prop2d.IsReferencedById || prop2d.ApiProp2d == null) {
        return prop2d.Id;
      }

      return AddProp2d(prop2d);
    }

    private void ConvertProp2ds(List<GsaProperty2d> prop2Ds) {
      if (prop2Ds == null) {
        return;
      }

      prop2Ds = prop2Ds.OrderByDescending(p => p.Id).ToList();
      foreach (GsaProperty2d prop2d in prop2Ds.Where(prop2d => prop2d != null)) {
        ConvertProp2d(prop2d);
      }
    }
  }
}
