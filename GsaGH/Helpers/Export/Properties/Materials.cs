using GsaAPI;
using GsaGH.Parameters;

namespace GsaGH.Helpers.Export {
  internal class Materials {

    internal static int AddMaterial(GsaMaterial material, ref GsaGuidDictionary<AnalysisMaterial> apiMaterials) {
      if (material.AnalysisProperty <= 0 || material.AnalysisMaterial == null) {
        return apiMaterials.AddValue(material.Guid, material.AnalysisMaterial);
      }
      apiMaterials.SetValue(material.AnalysisProperty, material.Guid, material.AnalysisMaterial);
      return material.AnalysisProperty;
    }

    internal static void AddMaterial(ref GsaSection section, ref GsaGuidDictionary<AnalysisMaterial> apiMaterials) {
      if (section.ApiSection.MaterialAnalysisProperty != 0 && section.Material != null && section.Material.AnalysisMaterial != null) {
        section.ApiSection.MaterialAnalysisProperty = ConvertCustomMaterial(section.Material, ref apiMaterials);
      }
    }

    internal static void AddMaterial(ref GsaProp2d prop2d, ref GsaGuidDictionary<AnalysisMaterial> apiMaterials) {
      if (prop2d.ApiProp2d.MaterialAnalysisProperty != 0 && prop2d.Material != null && prop2d.Material.AnalysisMaterial != null) {
        prop2d.ApiProp2d.MaterialAnalysisProperty = ConvertCustomMaterial(prop2d.Material, ref apiMaterials);
      }
    }

    internal static void AddMaterial(ref GsaProp3d prop3d, ref GsaGuidDictionary<AnalysisMaterial> apiMaterials) {
      if (prop3d.ApiProp3d.MaterialAnalysisProperty != 0 && prop3d.Material != null && prop3d.Material.AnalysisMaterial != null) {
        prop3d.ApiProp3d.MaterialAnalysisProperty = ConvertCustomMaterial(prop3d.Material, ref apiMaterials);
      }
    }

    internal static int ConvertCustomMaterial(GsaMaterial material, ref GsaGuidDictionary<AnalysisMaterial> apiMaterials) {
      return material == null
        ? 0
        : AddMaterial(material, ref apiMaterials);
    }

    internal static MaterialType ConvertType(GsaMaterial material) {
      MaterialType matType = MaterialType.NONE;

      if (material != null) {
        matType = (MaterialType)(int)material.MaterialType;
      }

      return matType;
    }
  }
}
