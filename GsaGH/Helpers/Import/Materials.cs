using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaAPI;
using GsaAPI.Materials;
using GsaGH.Parameters;

namespace GsaGH.Helpers.Import {
  internal class Materials {
    internal ReadOnlyDictionary<int, GsaMaterial> SteelMaterials { get; private set; }
    internal ReadOnlyDictionary<int, GsaMaterial> ConcreteMaterials { get; private set; }
    internal ReadOnlyDictionary<int, GsaMaterial> FrpMaterials { get; private set; }
    internal ReadOnlyDictionary<int, GsaMaterial> AluminiumMaterials { get; private set; }
    internal ReadOnlyDictionary<int, GsaMaterial> TimberMaterials { get; private set; }
    internal ReadOnlyDictionary<int, GsaMaterial> GlassMaterials { get; private set; }
    internal ReadOnlyDictionary<int, GsaMaterial> FabricMaterials { get; private set; }
    internal ReadOnlyDictionary<int, GsaMaterial> AnalysisMaterials { get; private set; }
    
    internal Materials(Model model) {
      string concreteCodeName = model.ConcreteDesignCode();
      string steelCodeName = model.SteelDesignCode();

      SteelMaterials =
        CreateMaterialsFromAPI(model.SteelMaterials(), steelCodeName);
      ConcreteMaterials =
        CreateMaterialsFromAPI(model.ConcreteMaterials(), concreteCodeName);
      FrpMaterials =
        CreateMaterialsFromAPI(model.FrpMaterials());
      AluminiumMaterials =
        CreateMaterialsFromAPI(model.AluminiumMaterials());
      TimberMaterials =
        CreateMaterialsFromAPI(model.TimberMaterials());
      GlassMaterials =
        CreateMaterialsFromAPI(model.GlassMaterials());
      FabricMaterials =
        CreateMaterialsFromAPI(model.FabricMaterials());
      
      AnalysisMaterials = CreateMaterialsFromAPI(model.AnalysisMaterials());
    }

    internal GsaMaterial GetMaterial(GsaSection section) {
      Section s = section.ApiSection;
      return GetMaterial(s.MaterialType, s.MaterialAnalysisProperty, s.MaterialGradeProperty);
    }

    internal GsaMaterial GetMaterial(GsaProp2d prop2d) {
      Prop2D s = prop2d.ApiProp2d;
      return GetMaterial(s.MaterialType, s.MaterialAnalysisProperty, s.MaterialGradeProperty);
    }

    internal GsaMaterial GetMaterial(GsaProp3d prop3d) {
      Prop3D s = prop3d.ApiProp3d;
      return GetMaterial(s.MaterialType, s.MaterialAnalysisProperty, s.MaterialGradeProperty);
    }

    private GsaMaterial GetMaterial(MaterialType type, int analysisProp, int gradeProp) {
      int id = analysisProp;
      if (id != 0) { // it is a custom material
        return AnalysisMaterials[id];
      }

      id = gradeProp;
      switch (type) {
        case MaterialType.ALUMINIUM:
          return AluminiumMaterials.TryGetValue(id, out GsaMaterial aluminium) ? aluminium : null;

        case MaterialType.CONCRETE:
          return ConcreteMaterials.TryGetValue(id, out GsaMaterial concrete) ? concrete : null;

        case MaterialType.FABRIC:
          return FabricMaterials.TryGetValue(id, out GsaMaterial fabric) ? fabric : null;

        case MaterialType.FRP:
          return FrpMaterials.TryGetValue(id, out GsaMaterial frp) ? frp : null;

        case MaterialType.GLASS:
          return GlassMaterials.TryGetValue(id, out GsaMaterial glass) ? glass : null;

        case MaterialType.STEEL:
          return SteelMaterials.TryGetValue(id, out GsaMaterial steel) ? steel : null;

        case MaterialType.TIMBER:
          return TimberMaterials.TryGetValue(id, out GsaMaterial timber) ? timber : null;

        default:
          return null;
      }
    }

    internal static ReadOnlyDictionary<int, GsaMaterial> CreateMaterialsFromAPI(
      ReadOnlyDictionary<int, AluminiumMaterial> materials) {
      var dict = new Dictionary<int, GsaMaterial>();
      foreach (KeyValuePair<int, AluminiumMaterial> mat in materials) {
        var gsaMaterial = new GsaMaterial(mat.Value, mat.Key) {
        };
        dict.Add(gsaMaterial.Id, gsaMaterial);
      }
      return new ReadOnlyDictionary<int, GsaMaterial>(dict);
    }

    internal static ReadOnlyDictionary<int, GsaMaterial> CreateMaterialsFromAPI(
      ReadOnlyDictionary<int, ConcreteMaterial> materials, string concreteCodeName) {
      var dict = new Dictionary<int, GsaMaterial>();
      foreach (KeyValuePair<int, ConcreteMaterial> mat in materials) {
        var gsaMaterial = new GsaMaterial(mat.Value, mat.Key, concreteCodeName) {
        };
        dict.Add(gsaMaterial.Id, gsaMaterial);
      }
      return new ReadOnlyDictionary<int, GsaMaterial>(dict);
    }

    internal static ReadOnlyDictionary<int, GsaMaterial> CreateMaterialsFromAPI(
      ReadOnlyDictionary<int, FabricMaterial> materials) {
      var dict = new Dictionary<int, GsaMaterial>();
      foreach (KeyValuePair<int, FabricMaterial> mat in materials) {
        var gsaMaterial = new GsaMaterial(mat.Value, mat.Key) {
        };
        dict.Add(gsaMaterial.Id, gsaMaterial);
      }
      return new ReadOnlyDictionary<int, GsaMaterial>(dict);
    }

    internal static ReadOnlyDictionary<int, GsaMaterial> CreateMaterialsFromAPI(
      ReadOnlyDictionary<int, FrpMaterial> materials) {
      var dict = new Dictionary<int, GsaMaterial>();
      foreach (KeyValuePair<int, FrpMaterial> mat in materials) {
        var gsaMaterial = new GsaMaterial(mat.Value, mat.Key) {
        };
        dict.Add(gsaMaterial.Id, gsaMaterial);
      }
      return new ReadOnlyDictionary<int, GsaMaterial>(dict);
    }

    internal static ReadOnlyDictionary<int, GsaMaterial> CreateMaterialsFromAPI(
      ReadOnlyDictionary<int, GlassMaterial> materials) {
      var dict = new Dictionary<int, GsaMaterial>();
      foreach (KeyValuePair<int, GlassMaterial> mat in materials) {
        var gsaMaterial = new GsaMaterial(mat.Value, mat.Key) {
        };
        dict.Add(gsaMaterial.Id, gsaMaterial);
      }
      return new ReadOnlyDictionary<int, GsaMaterial>(dict);
    }

    internal static ReadOnlyDictionary<int, GsaMaterial> CreateMaterialsFromAPI(
  ReadOnlyDictionary<int, SteelMaterial> materials, string steelCodeName) {
      var dict = new Dictionary<int, GsaMaterial>();
      foreach (KeyValuePair<int, SteelMaterial> mat in materials) {
        var gsaMaterial = new GsaMaterial(mat.Value, mat.Key, steelCodeName) {
        };
        dict.Add(gsaMaterial.Id, gsaMaterial);
      }
      return new ReadOnlyDictionary<int, GsaMaterial>(dict);
    }

    internal static ReadOnlyDictionary<int, GsaMaterial> CreateMaterialsFromAPI(
      ReadOnlyDictionary<int, TimberMaterial> materials) {
      var dict = new Dictionary<int, GsaMaterial>();
      foreach (KeyValuePair<int, TimberMaterial> mat in materials) {
        var gsaMaterial = new GsaMaterial(mat.Value, mat.Key) {
        };
        dict.Add(gsaMaterial.Id, gsaMaterial);
      }
      return new ReadOnlyDictionary<int, GsaMaterial>(dict);
    }

    internal static ReadOnlyDictionary<int, GsaMaterial> CreateMaterialsFromAPI(
      ReadOnlyDictionary<int, AnalysisMaterial> materials) {
      var dict = new Dictionary<int, GsaMaterial>();
      foreach (KeyValuePair<int, AnalysisMaterial> mat in materials) {
        var gsaMaterial = new GsaMaterial(mat.Value, mat.Key) {
        };
        dict.Add(gsaMaterial.Id, gsaMaterial);
      }
      return new ReadOnlyDictionary<int, GsaMaterial>(dict);
    }
  }
}