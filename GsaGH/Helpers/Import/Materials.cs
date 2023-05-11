using System.Collections.Generic;
using System.Collections.ObjectModel;
using GsaAPI;
using GsaGH.Parameters;

namespace GsaGH.Helpers.Import {
  internal class Materials {
    internal ReadOnlyDictionary<int, IGsaMaterial> SteelMaterials { get; private set; }
    internal ReadOnlyDictionary<int, IGsaMaterial> ConcreteMaterials { get; private set; }
    internal ReadOnlyDictionary<int, IGsaMaterial> FrpMaterials { get; private set; }
    internal ReadOnlyDictionary<int, IGsaMaterial> AluminiumMaterials { get; private set; }
    internal ReadOnlyDictionary<int, IGsaMaterial> TimberMaterials { get; private set; }
    internal ReadOnlyDictionary<int, IGsaMaterial> GlassMaterials { get; private set; }
    internal ReadOnlyDictionary<int, IGsaMaterial> FabricMaterials { get; private set; }
    internal ReadOnlyDictionary<int, IGsaMaterial> ReinforcementMaterials { get; private set; }
    internal ReadOnlyDictionary<int, IGsaMaterial> AnalysisMaterials { get; private set; }

    internal Materials(Model model) {
      string concreteCodeName = model.ConcreteDesignCode();
      string steelCodeName = model.SteelDesignCode();

      SteelMaterials = 
        CreateMaterialsFromAPI(model.SteelMaterials(), concreteCodeName, steelCodeName);
      ConcreteMaterials = 
        CreateMaterialsFromAPI(model.ConcreteMaterials(), concreteCodeName, steelCodeName);
      FrpMaterials = 
        CreateMaterialsFromAPI(model.FrpMaterials(), concreteCodeName, steelCodeName);
      AluminiumMaterials = 
        CreateMaterialsFromAPI(model.AluminiumMaterials(), concreteCodeName, steelCodeName);
      TimberMaterials = 
        CreateMaterialsFromAPI(model.TimberMaterials(), concreteCodeName, steelCodeName);
      GlassMaterials = 
        CreateMaterialsFromAPI(model.GlassMaterials(), concreteCodeName, steelCodeName);
      FabricMaterials = 
        CreateMaterialsFromAPI(model.FabricMaterials(), concreteCodeName, steelCodeName);
      ReinforcementMaterials = 
        CreateMaterialsFromAPI(model.ReinforcementMaterials(), concreteCodeName, steelCodeName);
      AnalysisMaterials = CreateMaterialsFromAPI(model.AnalysisMaterials());
    }

    internal IGsaMaterial GetMaterial(GsaSection section) {
      Section s = section.ApiSection;
      return GetMaterial(s.MaterialType, s.MaterialAnalysisProperty, s.MaterialGradeProperty);
    }

    internal IGsaMaterial GetMaterial(GsaProp2d prop2d) {
      Prop2D s = prop2d.ApiProp2d;
      return GetMaterial(s.MaterialType, s.MaterialAnalysisProperty, s.MaterialGradeProperty);
    }

    internal IGsaMaterial GetMaterial(GsaProp3d prop3d) {
      Prop3D s = prop3d.ApiProp3d;
      return GetMaterial(s.MaterialType, s.MaterialAnalysisProperty, s.MaterialGradeProperty);
    }

    private IGsaMaterial GetMaterial(MaterialType type, int analysisProp, int gradeProp) {
      int id = analysisProp;
      if (id != 0) { // it is a custom material
        return AnalysisMaterials[id];
      }

      id = gradeProp;
      switch (type) {
        case MaterialType.ALUMINIUM:
          return AluminiumMaterials[id];

        case MaterialType.CONCRETE:
          return ConcreteMaterials[id];

        case MaterialType.FABRIC:
          return FabricMaterials[id];

        case MaterialType.FRP:
          return FrpMaterials[id];

        case MaterialType.GLASS:
          return GlassMaterials[id];

        case MaterialType.REBAR:
          return ReinforcementMaterials[id];

        case MaterialType.STEEL:
          return SteelMaterials[id];

        case MaterialType.TIMBER:
          return TimberMaterials[id];

        default:
          return null;
      }
    }

    internal static ReadOnlyDictionary<int, IGsaMaterial> CreateMaterialsFromAPI(
      ReadOnlyDictionary<int, AluminiumMaterial> materials, string concreteCodeName,
      string steelCodeName) {
      var dict = new Dictionary<int, IGsaMaterial>();
      foreach (KeyValuePair<int, AluminiumMaterial> mat in materials) {
        var gsaMaterial = new GsaAluminiumMaterial() {
          StandardMaterial = mat.Value,
          Id = mat.Key,
          ConcreteDesignCodeName = concreteCodeName,
          SteelDesignCodeName = steelCodeName,
        };
        dict.Add(gsaMaterial.Id, gsaMaterial);
      }
      return new ReadOnlyDictionary<int, IGsaMaterial>(dict);
    }

    internal static ReadOnlyDictionary<int, IGsaMaterial> CreateMaterialsFromAPI(
      ReadOnlyDictionary<int, ConcreteMaterial> materials, string concreteCodeName,
      string steelCodeName) {
      var dict = new Dictionary<int, IGsaMaterial>();
      foreach (KeyValuePair<int, ConcreteMaterial> mat in materials) {
        var gsaMaterial = new GsaConcreteMaterial() {
          StandardMaterial = mat.Value,
          Id = mat.Key,
          ConcreteDesignCodeName = concreteCodeName,
          SteelDesignCodeName = steelCodeName,
        };
        dict.Add(gsaMaterial.Id, gsaMaterial);
      }
      return new ReadOnlyDictionary<int, IGsaMaterial>(dict);
    }

    internal static ReadOnlyDictionary<int, IGsaMaterial> CreateMaterialsFromAPI(
      ReadOnlyDictionary<int, FabricMaterial> materials, string concreteCodeName,
      string steelCodeName) {
      var dict = new Dictionary<int, IGsaMaterial>();
      foreach (KeyValuePair<int, FabricMaterial> mat in materials) {
        var gsaMaterial = new GsaFabricMaterial() {
          StandardMaterial = mat.Value,
          Id = mat.Key,
          ConcreteDesignCodeName = concreteCodeName,
          SteelDesignCodeName = steelCodeName,
        };
        dict.Add(gsaMaterial.Id, gsaMaterial);
      }
      return new ReadOnlyDictionary<int, IGsaMaterial>(dict);
    }

    internal static ReadOnlyDictionary<int, IGsaMaterial> CreateMaterialsFromAPI(
      ReadOnlyDictionary<int, FrpMaterial> materials, string concreteCodeName,
      string steelCodeName) {
      var dict = new Dictionary<int, IGsaMaterial>();
      foreach (KeyValuePair<int, FrpMaterial> mat in materials) {
        var gsaMaterial = new GsaFrpMaterial() {
          StandardMaterial = mat.Value,
          Id = mat.Key,
          ConcreteDesignCodeName = concreteCodeName,
          SteelDesignCodeName = steelCodeName,
        };
        dict.Add(gsaMaterial.Id, gsaMaterial);
      }
      return new ReadOnlyDictionary<int, IGsaMaterial>(dict);
    }

    internal static ReadOnlyDictionary<int, IGsaMaterial> CreateMaterialsFromAPI(
      ReadOnlyDictionary<int, GlassMaterial> materials, string concreteCodeName,
      string steelCodeName) {
      var dict = new Dictionary<int, IGsaMaterial>();
      foreach (KeyValuePair<int, GlassMaterial> mat in materials) {
        var gsaMaterial = new GsaGlassMaterial() {
          StandardMaterial = mat.Value,
          Id = mat.Key,
          ConcreteDesignCodeName = concreteCodeName,
          SteelDesignCodeName = steelCodeName,
        };
        dict.Add(gsaMaterial.Id, gsaMaterial);
      }
      return new ReadOnlyDictionary<int, IGsaMaterial>(dict);
    }

    internal static ReadOnlyDictionary<int, IGsaMaterial> CreateMaterialsFromAPI(
      ReadOnlyDictionary<int, ReinforcementMaterial> materials, string concreteCodeName,
      string steelCodeName) {
      var dict = new Dictionary<int, IGsaMaterial>();
      foreach (KeyValuePair<int, ReinforcementMaterial> mat in materials) {
        var gsaMaterial = new GsaReinforcementMaterial() {
          StandardMaterial = mat.Value,
          Id = mat.Key,
          ConcreteDesignCodeName = concreteCodeName,
          SteelDesignCodeName = steelCodeName,
        };
        dict.Add(gsaMaterial.Id, gsaMaterial);
      }
      return new ReadOnlyDictionary<int, IGsaMaterial>(dict);
    }

    internal static ReadOnlyDictionary<int, IGsaMaterial> CreateMaterialsFromAPI(
      ReadOnlyDictionary<int, SteelMaterial> materials, string concreteCodeName,
      string steelCodeName) {
      var dict = new Dictionary<int, IGsaMaterial>();
      foreach (KeyValuePair<int, SteelMaterial> mat in materials) {
        var gsaMaterial = new GsaSteelMaterial() {
          StandardMaterial = mat.Value,
          Id = mat.Key,
          ConcreteDesignCodeName = concreteCodeName,
          SteelDesignCodeName = steelCodeName,
        };
        dict.Add(gsaMaterial.Id, gsaMaterial);
      }
      return new ReadOnlyDictionary<int, IGsaMaterial>(dict);
    }

    internal static ReadOnlyDictionary<int, IGsaMaterial> CreateMaterialsFromAPI(
      ReadOnlyDictionary<int, TimberMaterial> materials, string concreteCodeName,
      string steelCodeName) {
      var dict = new Dictionary<int, IGsaMaterial>();
      foreach (KeyValuePair<int, TimberMaterial> mat in materials) {
        var gsaMaterial = new GsaTimberMaterial() {
          StandardMaterial = mat.Value,
          Id = mat.Key,
          ConcreteDesignCodeName = concreteCodeName,
          SteelDesignCodeName = steelCodeName,
        };
        dict.Add(gsaMaterial.Id, gsaMaterial);
      }
      return new ReadOnlyDictionary<int, IGsaMaterial>(dict);
    }

    internal static ReadOnlyDictionary<int, IGsaMaterial> CreateMaterialsFromAPI(
      ReadOnlyDictionary<int, AnalysisMaterial> materials) {
      var dict = new Dictionary<int, IGsaMaterial>();
      foreach (KeyValuePair<int, AnalysisMaterial> mat in materials) {
        var gsaMaterial = new GsaCustomMaterial() {
          AnalysisMaterial = mat.Value,
          Id = mat.Key,
        };
        dict.Add(gsaMaterial.Id, gsaMaterial);
      }
      return new ReadOnlyDictionary<int, IGsaMaterial>(dict);
    }
  }
}
