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

    internal static int AddMaterial(GsaMaterial material, ref GsaGuidDictionary<AnalysisMaterial> apiMaterials)
    {
      if (material.AnalysisProperty > 0 && material.AnalysisMaterial != null)
      {
        apiMaterials.SetValue(material.AnalysisProperty, material.Guid, material.AnalysisMaterial, true);
        return material.AnalysisProperty;
      }
      else
        return apiMaterials.AddValue(material.Guid, material.AnalysisMaterial);
    }

    internal static int ConvertCustomMaterial(GsaMaterial material, ref GsaGuidDictionary<AnalysisMaterial> apiMaterials)
    {
      if (material == null)
        return 0;
      return AddMaterial(material, ref apiMaterials);
    }

    internal static void AddMaterial(ref GsaSection section, ref GsaGuidDictionary<AnalysisMaterial> apiMaterials)
    {
      if (section.API_Section.MaterialAnalysisProperty != 0 && section.Material != null && section.Material.AnalysisMaterial != null)
        section.API_Section.MaterialAnalysisProperty = ConvertCustomMaterial(section.Material, ref apiMaterials);
    }

    internal static void AddMaterial(ref GsaProp2d prop2d, ref GsaGuidDictionary<AnalysisMaterial> apiMaterials)
    {
      if (prop2d.API_Prop2d.MaterialAnalysisProperty != 0 && prop2d.Material != null && prop2d.Material.AnalysisMaterial != null)
        prop2d.API_Prop2d.MaterialAnalysisProperty = ConvertCustomMaterial(prop2d.Material, ref apiMaterials);
    }

    internal static void AddMaterial(ref GsaProp3d prop3d, ref GsaGuidDictionary<AnalysisMaterial> apiMaterials)
    {
      if (prop3d.API_Prop3d.MaterialAnalysisProperty != 0 && prop3d.Material != null && prop3d.Material.AnalysisMaterial != null)
        prop3d.API_Prop3d.MaterialAnalysisProperty = ConvertCustomMaterial(prop3d.Material, ref apiMaterials);
    }
  }
}
