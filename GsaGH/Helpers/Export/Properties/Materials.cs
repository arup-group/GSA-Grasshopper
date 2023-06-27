using GsaAPI;
using GsaAPI.Materials;
using GsaGH.Parameters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    internal string ConcreteDesignCode = string.Empty;
    internal string SteelDesignCode = string.Empty;
    internal int Count => SteelMaterials.Count + ConcreteMaterials.Count + FrpMaterials.Count
      + AluminiumMaterials.Count + TimberMaterials.Count + GlassMaterials.Count
      + FabricMaterials.Count + AnalysisMaterials.Count;
    private Dictionary<Guid, GsaMaterial> _materials;

    internal Materials(GsaModel model) {
      SteelMaterials = GetStandardMaterialDictionary<SteelMaterial>(model.Materials.SteelMaterials);
      ConcreteMaterials = GetStandardMaterialDictionary<ConcreteMaterial>(model.Materials.ConcreteMaterials);
      FrpMaterials = GetStandardMaterialDictionary<FrpMaterial>(model.Materials.FrpMaterials);
      AluminiumMaterials = GetStandardMaterialDictionary<AluminiumMaterial>(model.Materials.AluminiumMaterials);
      TimberMaterials = GetStandardMaterialDictionary<TimberMaterial>(model.Materials.TimberMaterials);
      GlassMaterials = GetStandardMaterialDictionary<GlassMaterial>(model.Materials.GlassMaterials);
      FabricMaterials = GetStandardMaterialDictionary<FabricMaterial>(model.Materials.FabricMaterials);
      AnalysisMaterials = GetCustomMaterialDictionary(model.Materials.AnalysisMaterials);
      ConcreteDesignCode = model.Model.ConcreteDesignCode();
      SteelDesignCode = model.Model.SteelDesignCode();
      GetGsaGhMaterialsDictionary(model.Materials);
    }

    internal void Assemble(ref Model apiModel) {
      RebuildMaterialsToDesignCodes(apiModel);

      if (AnalysisMaterials.Count > 0) {
        foreach (KeyValuePair<int, AnalysisMaterial> mat in AnalysisMaterials.ReadOnlyDictionary) {
          apiModel.SetAnalysisMaterial(mat.Key, mat.Value);
        }
      }

      if (AluminiumMaterials.Count > 0) {
        foreach (KeyValuePair<int, AluminiumMaterial> mat in AluminiumMaterials.ReadOnlyDictionary) {
          apiModel.SetAluminiumMaterial(mat.Key, mat.Value);
        }
      }

      if (ConcreteMaterials.Count > 0) {
        foreach (KeyValuePair<int, ConcreteMaterial> mat in ConcreteMaterials.ReadOnlyDictionary) {
          apiModel.SetConcreteMaterial(mat.Key, mat.Value);
        }
      }

      if (FabricMaterials.Count > 0) {
        foreach (KeyValuePair<int, FabricMaterial> mat in FabricMaterials.ReadOnlyDictionary) {
          apiModel.SetFabricMaterial(mat.Key, mat.Value);
        }
      }

      if (FrpMaterials.Count > 0) {
        foreach (KeyValuePair<int, FrpMaterial> mat in FrpMaterials.ReadOnlyDictionary) {
          apiModel.SetFrpMaterial(mat.Key, mat.Value);
        }
      }

      if (GlassMaterials.Count > 0) {
        foreach (KeyValuePair<int, GlassMaterial> mat in GlassMaterials.ReadOnlyDictionary) {
          apiModel.SetGlassMaterial(mat.Key, mat.Value);
        }
      }

      if (SteelMaterials.Count > 0) {
        foreach (KeyValuePair<int, SteelMaterial> mat in SteelMaterials.ReadOnlyDictionary) {
          apiModel.SetSteelMaterial(mat.Key, mat.Value);
        }
      }

      if (TimberMaterials.Count > 0) {
        foreach (KeyValuePair<int, TimberMaterial> mat in TimberMaterials.ReadOnlyDictionary) {
          apiModel.SetTimberMaterial(mat.Key, mat.Value);
        }
      }
    }

    internal string GetReferenceDefinition(Guid guid) {
      if (SteelMaterials.GuidDictionary.TryGetValue(guid, out int steelId)) {
        return "MS" + steelId;
      }

      if (ConcreteMaterials.GuidDictionary.TryGetValue(guid, out int concreteId)) {
        return "MC" + concreteId;
      }

      if (FrpMaterials.GuidDictionary.TryGetValue(guid, out int frpId)) {
        return "MP" + frpId;
      }

      if (AnalysisMaterials.GuidDictionary.TryGetValue(guid, out int customId)) {
        return "M" + customId;
      }

      return string.Empty;
    }

    internal static void AddMaterial(
      ref GsaSection section, ref Materials apiMaterials) {
      if (section.Material != null) {
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
      if (prop2d.Material != null) {
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
      if (prop3d.Material != null) {
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
      if (!apiMaterials._materials.ContainsKey(material.Guid)) {
        apiMaterials._materials.Add(material.Guid, material);
      }

      if (material.IsCustom) {
        return AddOrSetCustomMaterial(material, ref apiMaterials.AnalysisMaterials);
      }

      switch (material.MaterialType) {
        case MatType.Aluminium:
          return AddOrSetStandardMaterial(material, ref apiMaterials.AluminiumMaterials);

        case MatType.Concrete:
          UpdateCode(material, ref apiMaterials);
          return AddOrSetStandardMaterial(material, ref apiMaterials.ConcreteMaterials);

        case MatType.Fabric:
          return AddOrSetStandardMaterial(material, ref apiMaterials.FabricMaterials);

        case MatType.Frp:
          return AddOrSetStandardMaterial(material, ref apiMaterials.FrpMaterials);

        case MatType.Glass:
          return AddOrSetStandardMaterial(material, ref apiMaterials.GlassMaterials);

        case MatType.Steel:
          UpdateCode(material, ref apiMaterials);
          return AddOrSetStandardMaterial(material, ref apiMaterials.SteelMaterials);

        case MatType.Timber:
          return AddOrSetStandardMaterial(material, ref apiMaterials.TimberMaterials);

        default:
          return AddOrSetCustomMaterial(material, ref apiMaterials.AnalysisMaterials);
      }
    }

    private static int AddOrSetStandardMaterial<T>(
      GsaMaterial material, ref GsaGuidDictionary<T> matDict) {
      var standardMaterial = (T)material.StandardMaterial;

      if (standardMaterial == null) {
        return 0;
      }

      return AddOrSetMaterial(material.Id, material.Guid, standardMaterial, ref matDict);
    }

    private static int AddOrSetCustomMaterial(GsaMaterial material,
      ref GsaGuidDictionary<AnalysisMaterial> matDict) {

      AnalysisMaterial analysisMaterial = material.AnalysisMaterial;
      if (analysisMaterial == null) {
        return 0;
      }

      return AddOrSetMaterial(material.Id, material.Guid, analysisMaterial, ref matDict);
    }

    private static int AddOrSetMaterial<T>(
      int id, Guid guid, T material, ref GsaGuidDictionary<T> matDict) {
      if (id <= 0) {
        return matDict.AddValue(guid, material);
      }

      matDict.SetValue(id, guid, material);
      return id;
    }

    private static GsaGuidDictionary<T> GetStandardMaterialDictionary<T>(
      ReadOnlyDictionary<int, GsaMaterial> existingStandardMaterials) {
      var materialsDictionary = new GsaGuidDictionary<T>(new Dictionary<int, T>());
      foreach (KeyValuePair<int, GsaMaterial> mat in existingStandardMaterials) {
        materialsDictionary.SetValue(mat.Key, mat.Value.Guid, (T)mat.Value.StandardMaterial);
      }
      return materialsDictionary;
    }

    private static GsaGuidDictionary<AnalysisMaterial> GetCustomMaterialDictionary(
      ReadOnlyDictionary<int, GsaMaterial> existingAnalysisMaterials) {
      var materialsDictionary = new GsaGuidDictionary<AnalysisMaterial>(
        new Dictionary<int, AnalysisMaterial>());
      foreach (KeyValuePair<int, GsaMaterial> mat in existingAnalysisMaterials) {
        materialsDictionary.SetValue(mat.Key, mat.Value.Guid, mat.Value.AnalysisMaterial);
      }
      return materialsDictionary;
    }

    private void GetGsaGhMaterialsDictionary(Import.Materials materials) {
      _materials = new Dictionary<Guid, GsaMaterial>();
      foreach (KeyValuePair<int, GsaMaterial> mat in materials.SteelMaterials) {
        _materials.Add(mat.Value.Guid, mat.Value);
      }
      foreach (KeyValuePair<int, GsaMaterial> mat in materials.ConcreteMaterials) {
        _materials.Add(mat.Value.Guid, mat.Value);
      }
      foreach (KeyValuePair<int, GsaMaterial> mat in materials.FrpMaterials) {
        _materials.Add(mat.Value.Guid, mat.Value);
      }
      foreach (KeyValuePair<int, GsaMaterial> mat in materials.AluminiumMaterials) {
        _materials.Add(mat.Value.Guid, mat.Value);
      }
      foreach (KeyValuePair<int, GsaMaterial> mat in materials.TimberMaterials) {
        _materials.Add(mat.Value.Guid, mat.Value);
      }
      foreach (KeyValuePair<int, GsaMaterial> mat in materials.GlassMaterials) {
        _materials.Add(mat.Value.Guid, mat.Value);
      }
      foreach (KeyValuePair<int, GsaMaterial> mat in materials.FabricMaterials) {
        _materials.Add(mat.Value.Guid, mat.Value);
      }
      foreach (KeyValuePair<int, GsaMaterial> mat in materials.AnalysisMaterials) {
        _materials.Add(mat.Value.Guid, mat.Value);
      }
    }

    private void RebuildMaterialsToDesignCodes(Model apiModel) {
      SteelMaterials = RebuildApiMaterials<SteelMaterial>(
        apiModel, SteelMaterials.GuidDictionary);
      
      ConcreteMaterials = RebuildApiMaterials<ConcreteMaterial>(
        apiModel, ConcreteMaterials.GuidDictionary);
      
      FrpMaterials = RebuildApiMaterials<FrpMaterial>(
        apiModel, FrpMaterials.GuidDictionary);
      
      AluminiumMaterials = RebuildApiMaterials<AluminiumMaterial>(
        apiModel, AluminiumMaterials.GuidDictionary); ;
      
      TimberMaterials = RebuildApiMaterials<TimberMaterial>(
        apiModel, TimberMaterials.GuidDictionary);
      
      GlassMaterials = RebuildApiMaterials<GlassMaterial>(
        apiModel, GlassMaterials.GuidDictionary);
      
      FabricMaterials = RebuildApiMaterials<FabricMaterial>(
        apiModel, FabricMaterials.GuidDictionary);
    }

    private GsaGuidDictionary<T> RebuildApiMaterials<T>(
      Model apiModel, ReadOnlyDictionary<Guid, int> guidDictionary) {
      var newMaterials = new GsaGuidDictionary<T>(new Dictionary<int, T>());
      foreach (KeyValuePair<Guid, int> item in guidDictionary) {
        if (_materials.TryGetValue(item.Key, out GsaMaterial material)) {
          material.RecreateForDesignCode(apiModel);
          newMaterials.SetValue(item.Value, item.Key, (T)material.StandardMaterial);
        }
      }
      return newMaterials;
    }

    private static void UpdateCode(GsaMaterial material, ref Materials apiMaterials) {
      if (apiMaterials.ConcreteDesignCode == string.Empty 
        && apiMaterials.SteelDesignCode == string.Empty) {
        apiMaterials.ConcreteDesignCode = material.ConcreteDesignCodeName;
        apiMaterials.SteelDesignCode = material.SteelDesignCodeName;
      }
      
      if (apiMaterials.ConcreteDesignCode != material.ConcreteDesignCodeName) {
        throw new Exception($"Material with {material.ConcreteDesignCodeName} Design Code" +
          $" cannot be added to a model with {apiMaterials.ConcreteDesignCode} Design Code.");
      }

      if (apiMaterials.SteelDesignCode != material.SteelDesignCodeName) {
        throw new Exception($"Material with {material.SteelDesignCodeName} Design Code" +
          $" cannot be added to a model with {apiMaterials.SteelDesignCode} Design Code.");
      }
    }
  }
}