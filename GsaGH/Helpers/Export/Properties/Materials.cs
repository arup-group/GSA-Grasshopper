using GsaAPI;
using System;
using System.Collections.Generic;
using GsaGH.Parameters;
using System.Linq;

namespace GsaGH.Helpers.Export
{
  internal class Materials
  {
    internal static MaterialType ConvertType(GsaMaterial material)
    {
            MaterialType matType = global::GsaAPI.MaterialType.NONE;
      
      if (material != null)
        matType = (MaterialType)(int)material.MaterialType;

      return matType;
    }

    internal static int ConvertCustomMaterial(GsaMaterial material,
        ref Dictionary<int, AnalysisMaterial> existingMaterials,
        ref Dictionary<Guid, int> materials_guid)
    {
      if (material == null) { return 0; }

      int outID = material.AnalysisProperty;

      // test if material has already bee added to the dictionary
      if (materials_guid.ContainsKey(material.Guid))
      {
        materials_guid.TryGetValue(material.Guid, out int sID);
        // if guid exist in our dictionary it has been added to the model 
        return sID;
      }

      // material
      if (material.AnalysisProperty > 0)
      {
        if (material.AnalysisMaterial != null) // material can refer to an ID only, meaning that the material must already exist in the model. Else we set it in the model:
          existingMaterials[material.AnalysisProperty] = material.AnalysisMaterial;
        else
          return outID; // return without setting the GUID in the dictionary
      }
      else
      {
        if (material.AnalysisMaterial != null)
        {
          if (existingMaterials.Count > 0)
            outID = existingMaterials.Keys.Max() + 1;
          else
            outID = 1;

          existingMaterials.Add(outID, material.AnalysisMaterial);
        }
      }

      // set guid in dictionary
      materials_guid.Add(material.Guid, outID);

      return outID;
    }
  }
}
