using GsaAPI;
using GsaAPI.Materials;
using GsaGH.Parameters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GsaGH.Helpers.Export {
  internal class Materials {
    public const string GenericConcreteCodeName = "generic conc.";
    public const string GenericSteelCodeName = "<steel generic>";

    internal GsaGuidDictionary<SteelMaterial> SteelMaterials;
    internal GsaGuidDictionary<ConcreteMaterial> ConcreteMaterials;
    internal GsaGuidDictionary<FrpMaterial> FrpMaterials;
    internal GsaGuidDictionary<AluminiumMaterial> AluminiumMaterials;
    internal GsaGuidDictionary<TimberMaterial> TimberMaterials;
    internal GsaGuidDictionary<GlassMaterial> GlassMaterials;
    internal GsaGuidDictionary<FabricMaterial> FabricMaterials;
    internal GsaGuidDictionary<AnalysisMaterial> CustomMaterials;
    private string _concreteDesignCode = string.Empty;
    private string _steelDesignCode = string.Empty;
    internal int Count => SteelMaterials.Count + ConcreteMaterials.Count + FrpMaterials.Count
      + AluminiumMaterials.Count + TimberMaterials.Count + GlassMaterials.Count
      + FabricMaterials.Count + CustomMaterials.Count;
    private Dictionary<Guid, GsaMaterial> _materials;

    internal Materials(GsaModel model) {
      SteelMaterials = GetStandardMaterialDictionary<SteelMaterial>(model.Materials.SteelMaterials);
      ConcreteMaterials = GetStandardMaterialDictionary<ConcreteMaterial>(model.Materials.ConcreteMaterials);
      FrpMaterials = GetStandardMaterialDictionary<FrpMaterial>(model.Materials.FrpMaterials);
      AluminiumMaterials = GetStandardMaterialDictionary<AluminiumMaterial>(model.Materials.AluminiumMaterials);
      TimberMaterials = GetStandardMaterialDictionary<TimberMaterial>(model.Materials.TimberMaterials);
      GlassMaterials = GetStandardMaterialDictionary<GlassMaterial>(model.Materials.GlassMaterials);
      FabricMaterials = GetStandardMaterialDictionary<FabricMaterial>(model.Materials.FabricMaterials);
      CustomMaterials = GetCustomMaterialDictionary(model.Materials.AnalysisMaterials);
      _concreteDesignCode = model.Model.ConcreteDesignCode();
      _steelDesignCode = model.Model.SteelDesignCode();
      GetGsaGhMaterialsDictionary(model.Materials);
    }

    private static int AddOrSetCustomMaterial(GsaMaterial material, GsaGuidDictionary<AnalysisMaterial> matDict) {
      AnalysisMaterial analysisMaterial = material.AnalysisMaterial;
      if (analysisMaterial == null) {
        return material.Id;
      }

      return AddOrSetMaterial(material.Id, material.Guid, analysisMaterial, matDict);
    }

    private static int AddOrSetMaterial<T>(int id, Guid guid, T material, GsaGuidDictionary<T> matDict) {
      if (id <= 0) {
        return matDict.AddValue(guid, material);
      }

      matDict.SetValue(id, guid, material);
      return id;
    }
    private static int AddOrSetStandardMaterial<T>(GsaMaterial material, GsaGuidDictionary<T> matDict) {
      var standardMaterial = (T)((IGsaStandardMaterial)material).StandardMaterial;

      if (standardMaterial == null) {
        return 0;
      }

      return AddOrSetMaterial(material.Id, material.Guid, standardMaterial, matDict);
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

    private static GsaGuidDictionary<T> GetStandardMaterialDictionary<T>(
      ReadOnlyDictionary<int, GsaMaterial> existingStandardMaterials) {
      var materialsDictionary = new GsaGuidDictionary<T>(new Dictionary<int, T>());
      foreach (KeyValuePair<int, GsaMaterial> mat in existingStandardMaterials) {
        materialsDictionary.SetValue(mat.Key, mat.Value.Guid, (T)((IGsaStandardMaterial)mat.Value).StandardMaterial);
      }
      return materialsDictionary;
    }

    internal void Assemble(ref Model apiModel) {
      ValidateMaterialsToDesignCodes(apiModel);

      foreach (KeyValuePair<int, AnalysisMaterial> mat in CustomMaterials.ReadOnlyDictionary) {
        apiModel.SetAnalysisMaterial(mat.Key, mat.Value);
      }

      foreach (KeyValuePair<int, AluminiumMaterial> mat in AluminiumMaterials.ReadOnlyDictionary) {
        apiModel.SetAluminiumMaterial(mat.Key, mat.Value);
      }

      foreach (KeyValuePair<int, ConcreteMaterial> mat in ConcreteMaterials.ReadOnlyDictionary) {
        apiModel.SetConcreteMaterial(mat.Key, mat.Value);
      }

      foreach (KeyValuePair<int, FabricMaterial> mat in FabricMaterials.ReadOnlyDictionary) {
        apiModel.SetFabricMaterial(mat.Key, mat.Value);
      }

      foreach (KeyValuePair<int, FrpMaterial> mat in FrpMaterials.ReadOnlyDictionary) {
        apiModel.SetFrpMaterial(mat.Key, mat.Value);
      }

      foreach (KeyValuePair<int, GlassMaterial> mat in GlassMaterials.ReadOnlyDictionary) {
        apiModel.SetGlassMaterial(mat.Key, mat.Value);
      }

      foreach (KeyValuePair<int, SteelMaterial> mat in SteelMaterials.ReadOnlyDictionary) {
        apiModel.SetSteelMaterial(mat.Key, mat.Value);
      }

      foreach (KeyValuePair<int, TimberMaterial> mat in TimberMaterials.ReadOnlyDictionary) {
        apiModel.SetTimberMaterial(mat.Key, mat.Value);
      }
    }

    internal void AddMaterial(ref GsaProperty2d prop2d) {
      if (prop2d.Material != null) {
        // set material type in API prop
        prop2d.ApiProp2d.MaterialType = GetMaterialType(prop2d.Material);

        // convert material and set it in dictionary
        int id = ConvertMaterial(prop2d.Material);

        // update API prop depending on std material type
        if (prop2d.Material is GsaCustomMaterial) {
          prop2d.ApiProp2d.MaterialGradeProperty = 0;
          prop2d.ApiProp2d.MaterialAnalysisProperty = id;
        } else {
          prop2d.ApiProp2d.MaterialGradeProperty = id;
          prop2d.ApiProp2d.MaterialAnalysisProperty = 0;
        }
      }
    }

    internal void AddMaterial(ref GsaProperty3d prop3d) {
      if (prop3d.Material != null) {
        // set material type in API prop
        prop3d.ApiProp3d.MaterialType = GetMaterialType(prop3d.Material);

        // convert material and set it in dictionary
        int id = ConvertMaterial(prop3d.Material);

        // update API prop depending on std material type
        if (prop3d.Material is GsaCustomMaterial) {
          prop3d.ApiProp3d.MaterialGradeProperty = 0;
          prop3d.ApiProp3d.MaterialAnalysisProperty = id;
        } else {
          prop3d.ApiProp3d.MaterialGradeProperty = id;
          prop3d.ApiProp3d.MaterialAnalysisProperty = 0;
        }
      }
    }

    internal void AddMaterial(ref GsaSection section) {
      if (section.Material != null) {
        // set material type in API prop
        section.ApiSection.MaterialType = GetMaterialType(section.Material);

        // convert material and set it in dictionary
        int id = ConvertMaterial(section.Material);

        // update API prop depending on std material type
        if (section.Material is GsaCustomMaterial) {
          section.ApiSection.MaterialGradeProperty = 0;
          section.ApiSection.MaterialAnalysisProperty = id;
        } else {
          section.ApiSection.MaterialGradeProperty = id;
          section.ApiSection.MaterialAnalysisProperty = 0;
        }
      }
    }

    internal string GetConcreteDesignCode(Model model = null) {
      if (_concreteDesignCode == string.Empty) {
        // if there is no concrete design code available
        // try looking for one in the materials created from API objects
        foreach (GsaMaterial material in _materials.Values) {
          if (!material.IsFromApi) {
            continue;
          }
          if (material.ConcreteDesignCodeName != string.Empty &&
            material.ConcreteDesignCodeName != GenericConcreteCodeName) {

            return material.ConcreteDesignCodeName;
          }
        }
        // then check the model
        if (model.ConcreteDesignCode() != string.Empty) {
          _concreteDesignCode = model.ConcreteDesignCode();
        }
        // or get a random (Eurocode) code
        else {
          _concreteDesignCode = DesignCode.GetConcreteDesignCodeNames()[8];
        }
      }

      return _concreteDesignCode;
    }

    internal MaterialType GetMaterialType(GsaMaterial material) {
      string value = material.MaterialType.ToString();
      if (value.ToLower() == "custom") {
        value = "generic";
      }
      return (MaterialType)Enum.Parse(typeof(MaterialType), value, true);
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

      if (CustomMaterials.GuidDictionary.TryGetValue(guid, out int customId)) {
        return "M" + customId;
      }

      return string.Empty;
    }

    internal string GetSteelDesignCode(Model model = null) {
      if (_steelDesignCode == string.Empty) {
        // if there is no concrete design code available
        // try looking for one in the materials created from API objects
        foreach (GsaMaterial material in _materials.Values) {
          if (!material.IsFromApi) {
            continue;
          }
          if (material.SteelDesignCodeName != string.Empty &&
            material.SteelDesignCodeName != GenericSteelCodeName) {

            return material.SteelDesignCodeName;
          }
        }
        // then check the model
        if (model.ConcreteDesignCode() != string.Empty) {
          _steelDesignCode = model.ConcreteDesignCode();
        }
        // or get a random (Eurocode) code
        else {
          _steelDesignCode = DesignCode.GetSteelDesignCodeNames()[8];
        }
      }

      return _steelDesignCode;
    }

    private int AddMaterial(GsaMaterial material) {
      if (!_materials.ContainsKey(material.Guid)) {
        _materials.Add(material.Guid, material);
      }

      if (material is GsaCustomMaterial) {
        return AddOrSetCustomMaterial(material, CustomMaterials);
      }

      switch (material.MaterialType) {
        case MatType.Aluminium:
          return AddOrSetStandardMaterial(material, AluminiumMaterials);

        case MatType.Concrete:
          UpdateDesignCode(material);
          return AddOrSetStandardMaterial(material, ConcreteMaterials);

        case MatType.Fabric:
          return AddOrSetStandardMaterial(material, FabricMaterials);

        case MatType.Frp:
          return AddOrSetStandardMaterial(material, FrpMaterials);

        case MatType.Glass:
          return AddOrSetStandardMaterial(material, GlassMaterials);

        case MatType.Steel:
          UpdateDesignCode(material);
          return AddOrSetStandardMaterial(material, SteelMaterials);

        case MatType.Timber:
          return AddOrSetStandardMaterial(material, TimberMaterials);

        case MatType.Custom:
        default:
          return AddOrSetCustomMaterial(material, CustomMaterials);
      }
    }

    private void CheckConcreteDesignCode(GsaMaterial material) {
      if (_concreteDesignCode == string.Empty || material.ConcreteDesignCodeName == string.Empty ||
        material.ConcreteDesignCodeName == GenericConcreteCodeName) {
        return;
      }

      if (material.ConcreteDesignCodeName != _concreteDesignCode) {
        throw new Exception($"Material with {material.ConcreteDesignCodeName} Design Code" +
          $" cannot be added to a model with {_concreteDesignCode} Design Code.");
      }
    }

    private void CheckSteelDesignCode(GsaMaterial material) {
      if (_steelDesignCode == string.Empty || material.SteelDesignCodeName == string.Empty ||
        material.SteelDesignCodeName == GenericSteelCodeName) {
        return;
      }

      if (_steelDesignCode != material.SteelDesignCodeName) {
        throw new Exception($"Material with {material.SteelDesignCodeName} Design Code" +
          $" cannot be added to a model with {_steelDesignCode} Design Code.");
      }
    }

    private int ConvertMaterial(GsaMaterial material) {
      return material == null ? 0 : AddMaterial(material);
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

    private void ValidateMaterialsToDesignCodes(Model apiModel) {
      SteelMaterials = ValidateOrRebuildApiMaterials<SteelMaterial>(
        apiModel, SteelMaterials.GuidDictionary);

      ConcreteMaterials = ValidateOrRebuildApiMaterials<ConcreteMaterial>(
        apiModel, ConcreteMaterials.GuidDictionary);

      FrpMaterials = ValidateOrRebuildApiMaterials<FrpMaterial>(
        apiModel, FrpMaterials.GuidDictionary);

      AluminiumMaterials = ValidateOrRebuildApiMaterials<AluminiumMaterial>(
        apiModel, AluminiumMaterials.GuidDictionary);

      TimberMaterials = ValidateOrRebuildApiMaterials<TimberMaterial>(
        apiModel, TimberMaterials.GuidDictionary);

      GlassMaterials = ValidateOrRebuildApiMaterials<GlassMaterial>(
        apiModel, GlassMaterials.GuidDictionary);

      FabricMaterials = ValidateOrRebuildApiMaterials<FabricMaterial>(
        apiModel, FabricMaterials.GuidDictionary);
    }

    private GsaGuidDictionary<T> ValidateOrRebuildApiMaterials<T>(Model apiModel, ReadOnlyDictionary<Guid, int> guidDictionary) {
      var newMaterials = new GsaGuidDictionary<T>(new Dictionary<int, T>());
      foreach (KeyValuePair<Guid, int> item in guidDictionary) {

        if (_materials.TryGetValue(item.Key, out GsaMaterial material)) {

          if (material.IsFromApi) {
            CheckConcreteDesignCode(material);
            CheckSteelDesignCode(material);

          } else {
            // material was created in Grasshopper
            if (material.MaterialType == MatType.Concrete) {
              CheckConcreteDesignCode(material);
            } 

            if (material.MaterialType == MatType.Steel) {
              CheckSteelDesignCode(material);
            }

            material = GsaMaterialFactory.RecreateForDesignCode(material, apiModel);
          }
          newMaterials.SetValue(item.Value, item.Key, (T)((IGsaStandardMaterial)material).StandardMaterial);
        }
      }
      return newMaterials;
    }

    private void UpdateDesignCode(GsaMaterial material) {
      if (material.MaterialType != MatType.Steel && material.MaterialType != MatType.Concrete) {
        return;
      }

      if (material.MaterialType == MatType.Concrete && _concreteDesignCode == string.Empty && material.ConcreteDesignCodeName != GenericConcreteCodeName) {
        _concreteDesignCode = material.ConcreteDesignCodeName;
        return;
      }

      if (material.MaterialType == MatType.Steel && _steelDesignCode == string.Empty &&
        material.SteelDesignCodeName != GenericSteelCodeName) {
        _steelDesignCode = material.SteelDesignCodeName;
        return;
      }
    }
  }
}
