﻿using GsaAPI;
using GsaGH.Parameters;
using System;

namespace GsaGH.Helpers.Export {
  internal class Materials {
    internal GsaGuidDictionary<SteelMaterial> SteelMaterials;
    internal GsaGuidDictionary<ConcreteMaterial> ConcreteMaterials;
    internal GsaGuidDictionary<FrpMaterial> FrpMaterials;
    internal GsaGuidDictionary<AluminiumMaterial> AluminiumMaterials;
    internal GsaGuidDictionary<TimberMaterial> TimberMaterials;
    internal GsaGuidDictionary<GlassMaterial> GlassMaterials;
    internal GsaGuidDictionary<FabricMaterial> FabricMaterials;
    internal GsaGuidDictionary<ReinforcementMaterial> ReinforcementMaterials;
    internal GsaGuidDictionary<AnalysisMaterial> AnalysisMaterials;

    internal Materials(Model model) {
      string concreteCodeName = model.ConcreteDesignCode();
      string steelCodeName = model.SteelDesignCode();

      SteelMaterials = new GsaGuidDictionary<SteelMaterial>(model.SteelMaterials());
      ConcreteMaterials = new GsaGuidDictionary<ConcreteMaterial>(model.ConcreteMaterials());
      FrpMaterials = new GsaGuidDictionary<FrpMaterial>(model.FrpMaterials());
      AluminiumMaterials = new GsaGuidDictionary<AluminiumMaterial>(model.AluminiumMaterials());
      TimberMaterials = new GsaGuidDictionary<TimberMaterial>(model.TimberMaterials());
      GlassMaterials = new GsaGuidDictionary<GlassMaterial>(model.GlassMaterials());
      FabricMaterials = new GsaGuidDictionary<FabricMaterial>(model.FabricMaterials());
      ReinforcementMaterials = 
        new GsaGuidDictionary<ReinforcementMaterial>(model.ReinforcementMaterials());
      AnalysisMaterials = new GsaGuidDictionary<AnalysisMaterial>(model.AnalysisMaterials());
    }

    internal static void AddMaterial(
      ref GsaSection section, ref Materials apiMaterials) {
      if (section.ApiSection.MaterialAnalysisProperty != 0 && section.Material != null
        && ((IGsaApiMaterial)section.Material).AnalysisMaterial != null) {
        // set material type in API prop
        section.ApiSection.MaterialType = 
          (MaterialType)Enum.Parse(typeof(MaterialType), section.Material.Type.ToString(), true);

        // convert material and set it in dictionary
        int id = ConvertMaterial(section.Material, ref apiMaterials);

        // update API prop depending on std material type
        if (IsStandard(section.Material)) {
          section.ApiSection.MaterialGradeProperty = id;
          section.ApiSection.MaterialAnalysisProperty = 0;
        } else {
          section.ApiSection.MaterialGradeProperty = 0;
          section.ApiSection.MaterialAnalysisProperty = id;
        }
      }
    }

    internal static void AddMaterial(
      ref GsaProp2d prop2d, ref Materials apiMaterials) {
      if (prop2d.ApiProp2d.MaterialAnalysisProperty != 0 && prop2d.Material != null
        && ((IGsaApiMaterial)prop2d.Material).AnalysisMaterial != null) {
        // set material type in API prop
        prop2d.ApiProp2d.MaterialType =
          (MaterialType)Enum.Parse(typeof(MaterialType), 
          prop2d.Material.Type.ToString(), true);

        // convert material and set it in dictionary
        int id = ConvertMaterial(prop2d.Material, ref apiMaterials);

        // update API prop depending on std material type
        if (IsStandard(prop2d.Material)) {
          prop2d.ApiProp2d.MaterialGradeProperty = id;
          prop2d.ApiProp2d.MaterialAnalysisProperty = 0;
        } else {
          prop2d.ApiProp2d.MaterialGradeProperty = 0;
          prop2d.ApiProp2d.MaterialAnalysisProperty = id;
        }
      }
    }

    internal static void AddMaterial(
      ref GsaProp3d prop3d, ref Materials apiMaterials) {
      if (prop3d.ApiProp3d.MaterialAnalysisProperty != 0 && prop3d.Material != null
        && ((IGsaApiMaterial)prop3d.Material).AnalysisMaterial != null) {
        // set material type in API prop
        prop3d.ApiProp3d.MaterialType =
          (MaterialType)Enum.Parse(typeof(MaterialType), 
          prop3d.Material.Type.ToString(), true);

        // convert material and set it in dictionary
        int id = ConvertMaterial(prop3d.Material, ref apiMaterials);

        // update API prop depending on std material type
        if (IsStandard(prop3d.Material)) {
          prop3d.ApiProp3d.MaterialGradeProperty = id;
          prop3d.ApiProp3d.MaterialAnalysisProperty = 0;
        } else {
          prop3d.ApiProp3d.MaterialGradeProperty = 0;
          prop3d.ApiProp3d.MaterialAnalysisProperty = id;
        }
      }
    }

    private static int ConvertMaterial(
      IGsaMaterial material, ref Materials apiMaterials) {
      return material == null ? 0 : AddMaterial(material, ref apiMaterials);
    }

    private static int AddMaterial(
      IGsaMaterial material, ref Materials apiMaterials) {
      switch (material.Type) {
        case MatType.Aluminium:
          return AddOrSetStandardMaterial(
            (GsaAluminiumMaterial)material, ref apiMaterials.AluminiumMaterials);

        case MatType.Concrete:
          return AddOrSetStandardMaterial(
            (GsaConcreteMaterial)material, ref apiMaterials.ConcreteMaterials);

        case MatType.Fabric:
          return AddOrSetStandardMaterial(
            (GsaFabricMaterial)material, ref apiMaterials.FabricMaterials);

        case MatType.Frp:
          return AddOrSetStandardMaterial(
            (GsaFrpMaterial)material, ref apiMaterials.FrpMaterials);

        case MatType.Glass:
          return AddOrSetStandardMaterial(
            (GsaGlassMaterial)material, ref apiMaterials.GlassMaterials);

        case MatType.Rebar:
          return AddOrSetStandardMaterial(
            (GsaReinforcementMaterial)material, 
            ref apiMaterials.ReinforcementMaterials);

        case MatType.Steel:
          return AddOrSetStandardMaterial(
            (GsaSteelMaterial)material, ref apiMaterials.SteelMaterials);

        case MatType.Timber:
          return AddOrSetStandardMaterial(
            (GsaTimberMaterial)material, ref apiMaterials.TimberMaterials);

        default:
          return AddOrSetCustomMaterial(
            (GsaCustomMaterial)material, ref apiMaterials.AnalysisMaterials);
      }
    }

    private static int AddOrSetStandardMaterial<T>(
      IGsaStandardMaterial<T> material, ref GsaGuidDictionary<T> matDict) {
      if (material.Id <= 0) {
        return matDict.AddValue(material.Guid, material.StandardMaterial);
      }

      matDict.SetValue(material.Id, material.Guid, material.StandardMaterial);
      return material.Id;
    }
    private static int AddOrSetCustomMaterial(IGsaApiMaterial material, 
      ref GsaGuidDictionary<AnalysisMaterial> matDict) {
      if (material.Id <= 0) {
        return matDict.AddValue(material.Guid, material.AnalysisMaterial);
      }

      matDict.SetValue(material.Id, material.Guid, material.AnalysisMaterial);
      return material.Id;
    }

    private static bool IsStandard(IGsaMaterial material) {
      return material.GetType() != typeof(GsaCustomMaterial);
    }
  }
}
