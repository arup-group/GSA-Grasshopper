using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using GsaAPI;
using GsaAPI.Materials;

using GsaGH.Helpers.GsaApi;

namespace GsaGH.Parameters {
  internal static class GsaMaterialFactory {
    internal static GsaMaterial CreateMaterial(GsaMaterial other) {
      GsaMaterial material = null;
      switch (other) {
        case GsaAluminiumMaterial aluminiumMaterial:
          material = new GsaAluminiumMaterial(aluminiumMaterial);
          break;
        case GsaConcreteMaterial concreteMaterial:
          material = new GsaConcreteMaterial(concreteMaterial);
          break;
        case GsaFabricMaterial fabricMaterial:
          material = new GsaFabricMaterial(fabricMaterial);
          break;
        case GsaFrpMaterial frpMaterial:
          material = new GsaFrpMaterial(frpMaterial);
          break;
        case GsaGlassMaterial glassMaterial:
          material = new GsaGlassMaterial(glassMaterial);
          break;
        case GsaSteelMaterial steelMaterial:
          material = new GsaSteelMaterial(steelMaterial);
          break;
        case GsaTimberMaterial timberMaterial:
          material = new GsaTimberMaterial(timberMaterial);
          break;
        case GsaCustomMaterial customMaterial:
          material = new GsaCustomMaterial(customMaterial);
          break;
      }

      return material;
    }

    internal static GsaMaterial CreateMaterialFromApi(object standardMaterial, int id, Model model) {
      GsaMaterial material = standardMaterial switch {
        AluminiumMaterial aluminiumMaterial => new GsaAluminiumMaterial(aluminiumMaterial, true, model.ConcreteDesignCode(), model.SteelDesignCode()) {
          Id = id,
        },
        ConcreteMaterial concreteMaterial => new GsaConcreteMaterial(concreteMaterial, true, model.ConcreteDesignCode(), model.SteelDesignCode()) {
          Id = id,
        },
        FabricMaterial fabricMaterial => new GsaFabricMaterial(fabricMaterial, true, model.ConcreteDesignCode(), model.SteelDesignCode()) {
          Id = id,
        },
        FrpMaterial frpMaterial => new GsaFrpMaterial(frpMaterial, true, model.ConcreteDesignCode(), model.SteelDesignCode()) {
          Id = id,
        },
        GlassMaterial glassMaterial => new GsaGlassMaterial(glassMaterial, true, model.ConcreteDesignCode(), model.SteelDesignCode()) {
          Id = id,
        },
        SteelMaterial steelMaterial => new GsaSteelMaterial(steelMaterial, true, model.ConcreteDesignCode(), model.SteelDesignCode()) {
          Id = id,
        },
        TimberMaterial timberMaterial => new GsaTimberMaterial(timberMaterial, true, model.ConcreteDesignCode(), model.SteelDesignCode()) {
          Id = id,
        },
        _ => throw new Exception($"{standardMaterial} is not a standard material"),
      };
      List<string> gradeNames = GetGradeNames(material.MaterialType, model);
      if (!gradeNames.Contains(material.Name)) {
        material.IsUserDefined = true;
        return material;
      }

      // fabric material has no analysis material that could be modified
      if (material.MaterialType == MatType.Fabric) {
        return material;
      }

      // check if analysis material properties are according to code
      try {
        if (!AreAnalysisMaterialsEqual(CreateCodeAnalysisMaterial(material), material.AnalysisMaterial)) {
          // analysis material properties are not according to code/have been modified
          material.IsUserDefined = true;
        }
      } catch { }

      return material;
    }

    internal static ReadOnlyDictionary<int, GsaMaterial> CreateMaterialsFromApi(
  ReadOnlyDictionary<int, AluminiumMaterial> materials, Model model) {
      var dict = new Dictionary<int, GsaMaterial>();
      foreach (KeyValuePair<int, AluminiumMaterial> mat in materials) {
        GsaMaterial gsaMaterial = CreateMaterialFromApi(mat.Value, mat.Key, model);
        dict.Add(gsaMaterial.Id, gsaMaterial);
      }
      return new ReadOnlyDictionary<int, GsaMaterial>(dict);
    }

    internal static ReadOnlyDictionary<int, GsaMaterial> CreateMaterialsFromApi(
      ReadOnlyDictionary<int, ConcreteMaterial> materials, Model model) {
      var dict = new Dictionary<int, GsaMaterial>();
      foreach (KeyValuePair<int, ConcreteMaterial> mat in materials) {
        GsaMaterial gsaMaterial = CreateMaterialFromApi(mat.Value, mat.Key, model);
        dict.Add(gsaMaterial.Id, gsaMaterial);
      }
      return new ReadOnlyDictionary<int, GsaMaterial>(dict);
    }

    internal static ReadOnlyDictionary<int, GsaMaterial> CreateMaterialsFromApi(
      ReadOnlyDictionary<int, FabricMaterial> materials, Model model) {
      var dict = new Dictionary<int, GsaMaterial>();
      foreach (KeyValuePair<int, FabricMaterial> mat in materials) {
        GsaMaterial gsaMaterial = CreateMaterialFromApi(mat.Value, mat.Key, model);
        dict.Add(gsaMaterial.Id, gsaMaterial);
      }
      return new ReadOnlyDictionary<int, GsaMaterial>(dict);
    }

    internal static ReadOnlyDictionary<int, GsaMaterial> CreateMaterialsFromApi(
      ReadOnlyDictionary<int, FrpMaterial> materials, Model model) {
      var dict = new Dictionary<int, GsaMaterial>();
      foreach (KeyValuePair<int, FrpMaterial> mat in materials) {
        GsaMaterial gsaMaterial = CreateMaterialFromApi(mat.Value, mat.Key, model);
        dict.Add(gsaMaterial.Id, gsaMaterial);
      }
      return new ReadOnlyDictionary<int, GsaMaterial>(dict);
    }

    internal static ReadOnlyDictionary<int, GsaMaterial> CreateMaterialsFromApi(
      ReadOnlyDictionary<int, GlassMaterial> materials, Model model) {
      var dict = new Dictionary<int, GsaMaterial>();
      foreach (KeyValuePair<int, GlassMaterial> mat in materials) {
        GsaMaterial gsaMaterial = CreateMaterialFromApi(mat.Value, mat.Key, model);
        dict.Add(gsaMaterial.Id, gsaMaterial);
      }
      return new ReadOnlyDictionary<int, GsaMaterial>(dict);
    }

    internal static ReadOnlyDictionary<int, GsaMaterial> CreateMaterialsFromApi(
      ReadOnlyDictionary<int, SteelMaterial> materials, Model model) {
      var dict = new Dictionary<int, GsaMaterial>();
      foreach (KeyValuePair<int, SteelMaterial> mat in materials) {
        GsaMaterial gsaMaterial = CreateMaterialFromApi(mat.Value, mat.Key, model);
        dict.Add(gsaMaterial.Id, gsaMaterial);
      }
      return new ReadOnlyDictionary<int, GsaMaterial>(dict);
    }

    internal static ReadOnlyDictionary<int, GsaMaterial> CreateMaterialsFromApi(
      ReadOnlyDictionary<int, TimberMaterial> materials, Model model) {
      var dict = new Dictionary<int, GsaMaterial>();
      foreach (KeyValuePair<int, TimberMaterial> mat in materials) {
        GsaMaterial gsaMaterial = GsaMaterialFactory.CreateMaterialFromApi(mat.Value, mat.Key, model);
        dict.Add(gsaMaterial.Id, gsaMaterial);
      }
      return new ReadOnlyDictionary<int, GsaMaterial>(dict);
    }

    internal static ReadOnlyDictionary<int, GsaMaterial> CreateMaterialsFromApi(
      ReadOnlyDictionary<int, AnalysisMaterial> materials) {
      var dict = new Dictionary<int, GsaMaterial>();
      foreach (KeyValuePair<int, AnalysisMaterial> mat in materials) {
        GsaMaterial gsaMaterial = new GsaCustomMaterial(mat.Value, mat.Key);
        dict.Add(gsaMaterial.Id, gsaMaterial);
      }
      return new ReadOnlyDictionary<int, GsaMaterial>(dict);
    }

    internal static IGsaStandardMaterial CreateStandardMaterial(MatType type, string materialName, string codeName = "") {
      string concreteDesignCode = type == MatType.Concrete ? codeName : string.Empty;
      string steelDesignCode = type == MatType.Steel ? codeName : string.Empty;
      Model model = ModelFactory.CreateModelFromCodes(concreteDesignCode, steelDesignCode);

      IGsaStandardMaterial material;
      switch (type) {
        case MatType.Aluminium:
          material = new GsaAluminiumMaterial(model.CreateAluminiumMaterial(materialName));
          return material;

        case MatType.Concrete:
          material = new GsaConcreteMaterial(model.CreateConcreteMaterial(materialName), codeName);
          return material;

        case MatType.Fabric:
          material = new GsaFabricMaterial(model.CreateFabricMaterial(materialName));
          return material;

        case MatType.Frp:
          material = new GsaFrpMaterial(model.CreateFrpMaterial(materialName));
          return material;

        case MatType.Glass:
          material = new GsaGlassMaterial(model.CreateGlassMaterial(materialName));
          return material;

        case MatType.Steel:
          material = new GsaSteelMaterial(model.CreateSteelMaterial(materialName), codeName);
          return material;

        case MatType.Timber:
          material = new GsaTimberMaterial(model.CreateTimberMaterial(materialName));
          return material;

        default:
          throw new Exception($"Material type {type} does not have standard materials");
      }
    }

    internal static List<string> GetGradeNames(MatType type, string concreteDesignCode = "", string steelDesignCode = "") {
      Model m = ModelFactory.CreateModelFromCodes(concreteDesignCode, steelDesignCode);
      return GetGradeNames(type, m);
    }

    internal static MatType GetMatType(MaterialType type) {
      string value = type.ToString();
      if (Enum.TryParse(value, true, out MatType matType)) {
        return matType;
      }

      return MatType.Custom;
    }

    internal static GsaMaterial RecreateForDesignCode(GsaMaterial material, Model model) {
      GsaMaterial recreation = null;

      List<string> gradeNames = GetGradeNames(material.MaterialType, model);
      if (!gradeNames.Contains(material.Name)) {
        // we can´t use the name to recreate the material
        return material;
      }

      switch (material) {
        case GsaAluminiumMaterial aluminiumMaterial:
          recreation = new GsaAluminiumMaterial(model.CreateAluminiumMaterial(material.Name));
          break;

        case GsaConcreteMaterial concreteMaterial:
          recreation = new GsaConcreteMaterial(model.CreateConcreteMaterial(material.Name), model.ConcreteDesignCode());
          break;

        case GsaFabricMaterial fabricMaterial:
          recreation = new GsaFabricMaterial(model.CreateFabricMaterial(material.Name));
          return recreation;

        case GsaFrpMaterial frpMaterial:
          recreation = new GsaFrpMaterial(model.CreateFrpMaterial(material.Name));
          break;

        case GsaGlassMaterial glassMaterial:
          recreation = new GsaGlassMaterial(model.CreateGlassMaterial(material.Name));
          break;

        case GsaSteelMaterial steelMaterial:
          recreation = new GsaSteelMaterial(model.CreateSteelMaterial(material.Name), model.SteelDesignCode());
          break;

        case GsaTimberMaterial timberMaterial:
          recreation = new GsaTimberMaterial(model.CreateTimberMaterial(material.Name));
          break;
      }

      if (!AreAnalysisMaterialsEqual(CreateCodeAnalysisMaterial(material), material.AnalysisMaterial)) {
        // analysis material properties are not according to code/have been modified
        recreation.DuplicateAnalysisMaterial(material);
      }

      return recreation;
    }

    private static bool AreAnalysisMaterialsEqual(AnalysisMaterial a, AnalysisMaterial b) {
      if (a == null || b == null) {
        return false;
      }

      return Math.Round(a.CoefficientOfThermalExpansion, 11) ==
        Math.Round(b.CoefficientOfThermalExpansion, 11)
        && Math.Round(a.Density, 11) == Math.Round(b.Density, 11)
        && Math.Round(a.ElasticModulus, 11) == Math.Round(b.ElasticModulus, 11)
        && Math.Round(a.PoissonsRatio, 11) == Math.Round(b.PoissonsRatio, 11);
    }

    private static AnalysisMaterial CreateCodeAnalysisMaterial(GsaMaterial material) {
      if (material.MaterialType == MatType.Fabric) {
        throw new ArgumentException("Can not create analysis material for fabric material");
      }

      Model m = ModelFactory.CreateModelFromCodes(material.ConcreteDesignCodeName, material.SteelDesignCodeName);
      return material.MaterialType switch {
        MatType.Aluminium => m.CreateAluminiumMaterial(material.Name).AnalysisMaterial,
        MatType.Concrete => m.CreateConcreteMaterial(material.Name).AnalysisMaterial,
        MatType.Frp => m.CreateFrpMaterial(material.Name).AnalysisMaterial,
        MatType.Glass => m.CreateGlassMaterial(material.Name).AnalysisMaterial,
        MatType.Steel => m.CreateSteelMaterial(material.Name).AnalysisMaterial,
        MatType.Timber => m.CreateTimberMaterial(material.Name).AnalysisMaterial,
        _ => null,
      };
    }

    private static List<string> GetGradeNames(MatType type, Model model) {
      return type switch {
        MatType.Aluminium => new List<string>(model.GetStandardAluminumMaterialNames()),
        MatType.Concrete => new List<string>(model.GetStandardConcreteMaterialNames()),
        MatType.Fabric => new List<string>(model.GetStandardFabricMaterialNames()),
        MatType.Frp => new List<string>(model.GetStandardFrpMaterialNames()),
        MatType.Glass => new List<string>(model.GetStandardGlassMaterialNames()),
        MatType.Steel => new List<string>(model.GetStandardSteelMaterialNames()),
        MatType.Timber => new List<string>(model.GetStandardTimberMaterialNames()),
        _ => throw new Exception($"Material type {type} does not have standard materials"),
      };
    }
  }
}
