using GsaAPI;
using GsaAPI.Materials;
using GsaGH.Parameters;
using System;
using static GsaGH.Parameters.GsaMaterial;

namespace GsaGH.Helpers.Export {
  internal class Materials {
    internal GsaGuidDictionary<SteelMaterial> SteelMaterials;
    internal GsaGuidDictionary<ConcreteMaterial> ConcreteMaterials;
    internal GsaGuidDictionary<FrpMaterial> FrpMaterials;
    internal GsaGuidDictionary<AluminiumMaterial> AluminiumMaterials;
    internal GsaGuidDictionary<TimberMaterial> TimberMaterials;
    internal GsaGuidDictionary<GlassMaterial> GlassMaterials;
    internal GsaGuidDictionary<FabricMaterial> FabricMaterials;
    internal GsaGuidDictionary<AnalysisMaterial> AnalysisMaterials;

    internal Materials(GsaModel model) {
      SteelMaterials = Properties.GetSteelMaterialDictionary(model);
      ConcreteMaterials = Properties.GetConcreteMaterialDictionary(model);
      FrpMaterials = Properties.GetFrpMaterialDictionary(model);
      AluminiumMaterials = Properties.GetAluminiumMaterialDictionary(model);
      TimberMaterials = Properties.GetTimberMaterialDictionary(model);
      GlassMaterials = Properties.GetGlassMaterialDictionary(model);
      FabricMaterials = Properties.GetFabricMaterialDictionary(model);
      AnalysisMaterials = Properties.GetAnalysisMaterialDictionary(model);
    }

    internal static void AddMaterial(
      ref GsaSection section, ref Materials apiMaterials) {
      if (section.ApiSection.MaterialAnalysisProperty != 0
        && section.Material != null
        && section.Material.AnalysisMaterial != null) {
        // set material type in API prop
        section.ApiSection.MaterialType = GetMaterialType(section.Material);

        // convert material and set it in dictionary
        int id = ConvertMaterial(section.Material, ref apiMaterials);

        // update API prop depending on std material type
        if (section.Material.IsCustom) {
          section.ApiSection.MaterialGradeProperty = 0;
          section.ApiSection.MaterialAnalysisProperty = id;
        } else {
          section.ApiSection.MaterialGradeProperty = id;
          section.ApiSection.MaterialAnalysisProperty = 0;
        }
      }
    }

    internal static void AddMaterial(
      ref GsaProp2d prop2d, ref Materials apiMaterials) {
      if (prop2d.ApiProp2d.MaterialAnalysisProperty != 0
        && prop2d.Material != null
        && prop2d.Material.AnalysisMaterial != null) {
        // set material type in API prop
        prop2d.ApiProp2d.MaterialType = GetMaterialType(prop2d.Material);

        // convert material and set it in dictionary
        int id = ConvertMaterial(prop2d.Material, ref apiMaterials);

        // update API prop depending on std material type
        if (prop2d.Material.IsCustom) {
          prop2d.ApiProp2d.MaterialGradeProperty = 0;
          prop2d.ApiProp2d.MaterialAnalysisProperty = id;
        } else {
          prop2d.ApiProp2d.MaterialGradeProperty = id;
          prop2d.ApiProp2d.MaterialAnalysisProperty = 0;
        }
      }
    }

    internal static void AddMaterial(
      ref GsaProp3d prop3d, ref Materials apiMaterials) {
      if (prop3d.ApiProp3d.MaterialAnalysisProperty != 0
        && prop3d.Material != null
        && prop3d.Material.AnalysisMaterial != null) {
        // set material type in API prop
        prop3d.ApiProp3d.MaterialType = GetMaterialType(prop3d.Material);

        // convert material and set it in dictionary
        int id = ConvertMaterial(prop3d.Material, ref apiMaterials);

        // update API prop depending on std material type
        if (prop3d.Material.IsCustom) {
          prop3d.ApiProp3d.MaterialGradeProperty = 0;
          prop3d.ApiProp3d.MaterialAnalysisProperty = id;
        } else {
          prop3d.ApiProp3d.MaterialGradeProperty = id;
          prop3d.ApiProp3d.MaterialAnalysisProperty = 0;
        }
      }
    }

    internal static MaterialType GetMaterialType(GsaMaterial material) {
      return (MaterialType)Enum.Parse(typeof(MaterialType),
           material.MaterialType.ToString(), true);
    }

    private static int ConvertMaterial(
      GsaMaterial material, ref Materials apiMaterials) {
      return material == null ? 0 : AddMaterial(material, ref apiMaterials);
    }

    private static int AddMaterial(
      GsaMaterial material, ref Materials apiMaterials) {
      switch (material.MaterialType) {
        case MatType.Aluminium:
          return AddOrSetStandardMaterial(material, ref apiMaterials.AluminiumMaterials);

        case MatType.Concrete:
          return AddOrSetStandardMaterial(material, ref apiMaterials.ConcreteMaterials);

        case MatType.Fabric:
          return AddOrSetStandardMaterial(material, ref apiMaterials.FabricMaterials);

        case MatType.Frp:
          return AddOrSetStandardMaterial(material, ref apiMaterials.FrpMaterials);

        case MatType.Glass:
          return AddOrSetStandardMaterial(material, ref apiMaterials.GlassMaterials);

        case MatType.Steel:
          return AddOrSetStandardMaterial(material, ref apiMaterials.SteelMaterials);

        case MatType.Timber:
          return AddOrSetStandardMaterial(material, ref apiMaterials.TimberMaterials);

        default:
          return AddOrSetCustomMaterial(material, ref apiMaterials.AnalysisMaterials);
      }
    }

    private static int AddOrSetStandardMaterial<T>(
      GsaMaterial material, ref GsaGuidDictionary<T> matDict) {
      if (material.Id <= 0) {
        return matDict.AddValue(material.Guid, (T)material.StandardMaterial);
      }

      matDict.SetValue(material.Id, material.Guid, (T)material.StandardMaterial);
      return material.Id;
    }
    private static int AddOrSetCustomMaterial(GsaMaterial material,
      ref GsaGuidDictionary<AnalysisMaterial> matDict) {
      if (material.Id <= 0) {
        return matDict.AddValue(material.Guid, material.AnalysisMaterial);
      }

      matDict.SetValue(material.Id, material.Guid, material.AnalysisMaterial);
      return material.Id;
    }
  }
}