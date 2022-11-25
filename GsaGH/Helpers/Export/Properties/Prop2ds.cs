using GsaAPI;
using System;
using System.Collections.Generic;
using GsaGH.Parameters;
using System.Linq;
using static System.Collections.Specialized.BitVector32;

namespace GsaGH.Helpers.Export
{
  internal class Prop2ds
  {
    internal static int AddProp2d(GsaProp2d prop, ref GsaGuidDictionary<Prop2D> apiProp2ds, ref GsaGuidDictionary<AnalysisMaterial> apiMaterials)
    {
      Materials.AddMaterial(ref prop, ref apiMaterials);

      if (prop.Id > 0)
      {
        apiProp2ds.SetValue(prop.Id, prop.Guid, prop.API_Prop2d);
        return prop.Id;
      }
      else
        return apiProp2ds.AddValue(prop.Guid, prop.API_Prop2d);
    }

    internal static int ConvertProp2d(GsaProp2d prop2d, ref GsaGuidDictionary<Prop2D> apiProp2ds, ref GsaGuidDictionary<AnalysisMaterial> apiMaterials)
    {
      if (prop2d == null) { return 0; }
      if (prop2d.API_Prop2d == null) { return prop2d.Id; }
      return AddProp2d(prop2d, ref apiProp2ds, ref apiMaterials);
    }

    internal static void ConvertProp2d(List<GsaProp2d> prop2Ds, ref GsaGuidDictionary<Prop2D> apiProp2ds, ref GsaGuidDictionary<AnalysisMaterial> apiMaterials)
    {
      if (prop2Ds != null)
        for (int i = 0; i < prop2Ds.Count; i++)
          if (prop2Ds[i] != null)
            ConvertProp2d(prop2Ds[i], ref apiProp2ds, ref apiMaterials);
    }
  }
}
