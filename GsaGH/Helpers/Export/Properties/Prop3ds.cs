using System.Collections.Generic;
using System.Linq;
using GsaAPI;
using GsaGH.Parameters;

namespace GsaGH.Helpers.Export {
  internal class Prop3ds {

    internal static int AddProp3d(GsaProp3d prop, ref GsaGuidDictionary<Prop3D> apiProp3d, ref GsaGuidDictionary<AnalysisMaterial> apiMaterials) {
      Materials.AddMaterial(ref prop, ref apiMaterials);

      if (prop.Id <= 0) {
        return apiProp3d.AddValue(prop.Guid, prop.ApiProp3d);
      }

      apiProp3d.SetValue(prop.Id, prop.Guid, prop.ApiProp3d);
      return prop.Id;
    }

    internal static int ConvertProp3d(GsaProp3d prop3d, ref GsaGuidDictionary<Prop3D> apiProp3ds, ref GsaGuidDictionary<AnalysisMaterial> apiMaterials) {
      if (prop3d == null) { 
        return 0; 
      }
      if (prop3d.IsReferencedById || prop3d.ApiProp3d == null) { 
        return prop3d.Id; 
      }
      return AddProp3d(prop3d, ref apiProp3ds, ref apiMaterials);
    }

    internal static void ConvertProp3d(List<GsaProp3d> prop3Ds, ref GsaGuidDictionary<Prop3D> apiProp3ds, ref GsaGuidDictionary<AnalysisMaterial> apiMaterials) {
      if (prop3Ds == null) {
        return;
      }

      prop3Ds = prop3Ds.OrderByDescending(p => p.Id).ToList();
      foreach (GsaProp3d prop3D in prop3Ds.Where(prop3D => prop3D != null)) {
        ConvertProp3d(prop3D, ref apiProp3ds, ref apiMaterials);
      }
    }
  }
}
