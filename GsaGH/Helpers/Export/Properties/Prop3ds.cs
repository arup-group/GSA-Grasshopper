using GsaAPI;
using System;
using System.Collections.Generic;
using GsaGH.Parameters;
using System.Linq;
using static System.Collections.Specialized.BitVector32;

namespace GsaGH.Helpers.Export
{
  internal class Prop3ds
  {
    internal static int AddProp3d(GsaProp3d prop, ref GsaGuidDictionary<Prop3D> apiProp3d, ref GsaGuidDictionary<AnalysisMaterial> apiMaterials)
    {
      Materials.AddMaterial(ref prop, ref apiMaterials);

      if (prop.Id > 0)
      {
        apiProp3d.SetValue(prop.Id, prop.Guid, prop.API_Prop3d);
        return prop.Id;
      }
      else
        return apiProp3d.AddValue(prop.Guid, prop.API_Prop3d);
    }

    internal static int ConvertProp3d(GsaProp3d prop3d, ref GsaGuidDictionary<Prop3D> apiProp3ds, ref GsaGuidDictionary<AnalysisMaterial> apiMaterials)
    {
      if (prop3d == null) { return 0; }
      if (prop3d.IsReferencedByID || prop3d.API_Prop3d == null) { return prop3d.Id; }
      return AddProp3d(prop3d, ref apiProp3ds, ref apiMaterials);
    }

    internal static void ConvertProp3d(List<GsaProp3d> prop3Ds, ref GsaGuidDictionary<Prop3D> apiProp3ds, ref GsaGuidDictionary<AnalysisMaterial> apiMaterials)
    {
      if (prop3Ds != null)
        for (int i = 0; i < prop3Ds.Count; i++)
          if (prop3Ds[i] != null)
            ConvertProp3d(prop3Ds[i], ref apiProp3ds, ref apiMaterials);
    }
  }
}
