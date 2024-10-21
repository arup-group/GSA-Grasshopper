using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using GsaAPI;
using GsaAPI.Materials;

using GsaGH.Parameters;

namespace GsaGH.Helpers.Assembly {
  internal partial class ModelAssembly {
    public const string GenericConcreteCodeName = "generic conc.";
    public const string GenericSteelCodeName = "<steel generic>";

    private GsaGuidDictionary<SteelMaterial> _steelMaterials;
    private GsaGuidDictionary<ConcreteMaterial> _concreteMaterials;
    private GsaGuidDictionary<FrpMaterial> _frpMaterials;
    private GsaGuidDictionary<AluminiumMaterial> _aluminiumMaterials;
    private GsaGuidDictionary<TimberMaterial> _timberMaterials;
    private GsaGuidDictionary<GlassMaterial> _glassMaterials;
    private GsaGuidDictionary<FabricMaterial> _fabricMaterials;
    private GsaGuidDictionary<AnalysisMaterial> _customMaterials;
    private string _concreteDesignCode = string.Empty;
    private string _steelDesignCode = string.Empty;
    private int MaterialCount => _steelMaterials.Count + _concreteMaterials.Count + _frpMaterials.Count
      + _aluminiumMaterials.Count + _timberMaterials.Count + _glassMaterials.Count
      + _fabricMaterials.Count + _customMaterials.Count;
    private Dictionary<Guid, GsaMaterial> _materials;

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

    private int AddMaterial(GsaMaterial material) {
      if (material is GsaReferencedMaterial refMat) {
        return refMat.Id;
      }

      if (!_materials.ContainsKey(material.Guid)) {
        _materials.Add(material.Guid, material);
      }

      if (material is GsaCustomMaterial) {
        return AddOrSetCustomMaterial(material, _customMaterials);
      }

      switch (material.MaterialType) {
        case MatType.Aluminium:
          return AddOrSetStandardMaterial(material, _aluminiumMaterials);

        case MatType.Concrete:
          UpdateDesignCode(material);
          return AddOrSetStandardMaterial(material, _concreteMaterials);

        case MatType.Fabric:
          return AddOrSetStandardMaterial(material, _fabricMaterials);

        case MatType.Frp:
          return AddOrSetStandardMaterial(material, _frpMaterials);

        case MatType.Glass:
          return AddOrSetStandardMaterial(material, _glassMaterials);

        case MatType.Steel:
          UpdateDesignCode(material);
          return AddOrSetStandardMaterial(material, _steelMaterials);

        case MatType.Timber:
          return AddOrSetStandardMaterial(material, _timberMaterials);

        case MatType.Custom:
        default:
          return AddOrSetCustomMaterial(material, _customMaterials);
      }
    }

    private void AddMaterial(GsaMaterial material, ref Prop2D prop2d) {
      if (material != null) {
        // set material type in API prop
        prop2d.MaterialType = GetMaterialType(material);

        // convert material and set it in dictionary
        int id = ConvertMaterial(material);

        // update API prop depending on std material type
        if (material is GsaCustomMaterial || material.MaterialType == MatType.Custom) {
          prop2d.MaterialGradeProperty = 0;
          prop2d.MaterialAnalysisProperty = id;
        } else {
          prop2d.MaterialGradeProperty = id;
          prop2d.MaterialAnalysisProperty = 0;
        }
      }
    }

    private void AddMaterial(GsaMaterial material, ref Prop3D prop3d) {
      if (material != null) {
        // set material type in API prop
        prop3d.MaterialType = GetMaterialType(material);

        // convert material and set it in dictionary
        int id = ConvertMaterial(material);

        // update API prop depending on std material type
        if (material is GsaCustomMaterial || material.MaterialType == MatType.Custom) {
          prop3d.MaterialGradeProperty = 0;
          prop3d.MaterialAnalysisProperty = id;
        } else {
          prop3d.MaterialGradeProperty = id;
          prop3d.MaterialAnalysisProperty = 0;
        }
      }
    }

    private void AddMaterial(GsaMaterial material, ref Section section) {
      if (material != null) {
        // set material type in API prop
        section.MaterialType = GetMaterialType(material);

        // convert material and set it in dictionary
        int id = ConvertMaterial(material);

        // update API prop depending on std material type
        if (material is GsaCustomMaterial || material.MaterialType == MatType.Custom) {
          section.MaterialGradeProperty = 0;
          section.MaterialAnalysisProperty = id;
        } else {
          section.MaterialGradeProperty = id;
          section.MaterialAnalysisProperty = 0;
        }
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

    private string GetConcreteDesignCode(Model model = null) {
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

    private void GetGsaGhMaterialsDictionary(GsaMaterials materials) {
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

    private string GetMaterialReferenceDefinition(Guid guid) {
      if (_steelMaterials.GuidDictionary.TryGetValue(guid, out int steelId)) {
        return "MS" + steelId;
      }

      if (_concreteMaterials.GuidDictionary.TryGetValue(guid, out int concreteId)) {
        return "MC" + concreteId;
      }

      if (_frpMaterials.GuidDictionary.TryGetValue(guid, out int frpId)) {
        return "MP" + frpId;
      }

      if (_customMaterials.GuidDictionary.TryGetValue(guid, out int customId)) {
        return "M" + customId;
      }

      return string.Empty;
    }

    private MaterialType GetMaterialType(GsaMaterial material) {
      string value = material.MaterialType.ToString();
      if (value.ToLower() == "custom") {
        value = "generic";
      }

      return (MaterialType)Enum.Parse(typeof(MaterialType), value, true);
    }

    private string GetSteelDesignCode(Model model = null) {
      if (_steelDesignCode == string.Empty) {
        // if there is no steel design code available
        // try looking for one in the materials created from API objects
        foreach (GsaMaterial material in _materials.Values) {
          if (!material.IsFromApi) {
            continue;
          }

          if (material.SteelDesignCodeName != string.Empty) {
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

    private void ValidateMaterialsToDesignCodes(Model apiModel) {
      _steelMaterials = ValidateOrRebuildApiMaterials<SteelMaterial>(
        apiModel, _steelMaterials.GuidDictionary);

      _concreteMaterials = ValidateOrRebuildApiMaterials<ConcreteMaterial>(
        apiModel, _concreteMaterials.GuidDictionary);

      _frpMaterials = ValidateOrRebuildApiMaterials<FrpMaterial>(
        apiModel, _frpMaterials.GuidDictionary);

      _aluminiumMaterials = ValidateOrRebuildApiMaterials<AluminiumMaterial>(
        apiModel, _aluminiumMaterials.GuidDictionary);

      _timberMaterials = ValidateOrRebuildApiMaterials<TimberMaterial>(
        apiModel, _timberMaterials.GuidDictionary);

      _glassMaterials = ValidateOrRebuildApiMaterials<GlassMaterial>(
        apiModel, _glassMaterials.GuidDictionary);

      _fabricMaterials = ValidateOrRebuildApiMaterials<FabricMaterial>(
        apiModel, _fabricMaterials.GuidDictionary);
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
