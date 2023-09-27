using System.Collections.Generic;
using System.Collections.ObjectModel;
using GsaAPI;
using GsaAPI.Materials;
using GsaGH.Parameters;

namespace GsaGH.Helpers.Import {
  internal class Materials {
    public const string GenericConcreteCodeName = "generic conc.";
    public const string GenericSteelCodeName = "<steel generic>";

    internal ReadOnlyDictionary<int, GsaMaterial> SteelMaterials { get; private set; }
    internal ReadOnlyDictionary<int, GsaMaterial> ConcreteMaterials { get; private set; }
    internal ReadOnlyDictionary<int, GsaMaterial> FrpMaterials { get; private set; }
    internal ReadOnlyDictionary<int, GsaMaterial> AluminiumMaterials { get; private set; }
    internal ReadOnlyDictionary<int, GsaMaterial> TimberMaterials { get; private set; }
    internal ReadOnlyDictionary<int, GsaMaterial> GlassMaterials { get; private set; }
    internal ReadOnlyDictionary<int, GsaMaterial> FabricMaterials { get; private set; }
    internal ReadOnlyDictionary<int, GsaMaterial> AnalysisMaterials { get; private set; }

    internal Materials(Model model) {
      SteelMaterials = CreateMaterialsFromAPI(model.SteelMaterials(), model);
      ConcreteMaterials = CreateMaterialsFromAPI(model.ConcreteMaterials(), model);
      FrpMaterials = CreateMaterialsFromAPI(model.FrpMaterials(), model);
      AluminiumMaterials = CreateMaterialsFromAPI(model.AluminiumMaterials(), model);
      TimberMaterials = CreateMaterialsFromAPI(model.TimberMaterials(), model);
      GlassMaterials = CreateMaterialsFromAPI(model.GlassMaterials(), model);
      FabricMaterials = CreateMaterialsFromAPI(model.FabricMaterials(), model);
      AnalysisMaterials = CreateMaterialsFromAPI(model.AnalysisMaterials());
    }

    internal GsaMaterial GetMaterial(Section s) {
      return GetMaterial(s.MaterialType, s.MaterialAnalysisProperty, s.MaterialGradeProperty);
    }

    internal GsaMaterial GetMaterial(Prop2D p) {
      return GetMaterial(p.MaterialType, p.MaterialAnalysisProperty, p.MaterialGradeProperty);
    }

    internal GsaMaterial GetMaterial(Prop3D p) {
      return GetMaterial(p.MaterialType, p.MaterialAnalysisProperty, p.MaterialGradeProperty);
    }

    private GsaMaterial GetMaterial(MaterialType type, int analysisProp, int gradeProp) {
      int id = analysisProp;
      if (id != 0) { // it is a custom material
        if (AnalysisMaterials.ContainsKey(id)) {
          return AnalysisMaterials[id];
        } else {
          return new GsaReferencedMaterial(id);
        }
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

        case MaterialType.FIRST:
          return SteelMaterials.TryGetValue(id, out GsaMaterial steel) ? steel : null;

        case MaterialType.TIMBER:
          return TimberMaterials.TryGetValue(id, out GsaMaterial timber) ? timber : null;

        default:
          return null;
      }
    }

    internal static ReadOnlyDictionary<int, GsaMaterial> CreateMaterialsFromAPI(
      ReadOnlyDictionary<int, AluminiumMaterial> materials, Model model) {
      var dict = new Dictionary<int, GsaMaterial>();
      foreach (KeyValuePair<int, AluminiumMaterial> mat in materials) {
        GsaMaterial gsaMaterial = GsaMaterialFactory.CreateMaterialFromApi(mat.Value, mat.Key, model);
        dict.Add(gsaMaterial.Id, gsaMaterial);
      }
      return new ReadOnlyDictionary<int, GsaMaterial>(dict);
    }

    internal static ReadOnlyDictionary<int, GsaMaterial> CreateMaterialsFromAPI(
      ReadOnlyDictionary<int, ConcreteMaterial> materials, Model model) {
      var dict = new Dictionary<int, GsaMaterial>();
      foreach (KeyValuePair<int, ConcreteMaterial> mat in materials) {
        GsaMaterial gsaMaterial = GsaMaterialFactory.CreateMaterialFromApi(mat.Value, mat.Key, model);
        dict.Add(gsaMaterial.Id, gsaMaterial);
      }
      return new ReadOnlyDictionary<int, GsaMaterial>(dict);
    }

    internal static ReadOnlyDictionary<int, GsaMaterial> CreateMaterialsFromAPI(
      ReadOnlyDictionary<int, FabricMaterial> materials, Model model) {
      var dict = new Dictionary<int, GsaMaterial>();
      foreach (KeyValuePair<int, FabricMaterial> mat in materials) {
        GsaMaterial gsaMaterial = GsaMaterialFactory.CreateMaterialFromApi(mat.Value, mat.Key, model);
        dict.Add(gsaMaterial.Id, gsaMaterial);
      }
      return new ReadOnlyDictionary<int, GsaMaterial>(dict);
    }

    internal static ReadOnlyDictionary<int, GsaMaterial> CreateMaterialsFromAPI(
      ReadOnlyDictionary<int, FrpMaterial> materials, Model model) {
      var dict = new Dictionary<int, GsaMaterial>();
      foreach (KeyValuePair<int, FrpMaterial> mat in materials) {
        GsaMaterial gsaMaterial = GsaMaterialFactory.CreateMaterialFromApi(mat.Value, mat.Key, model);
        dict.Add(gsaMaterial.Id, gsaMaterial);
      }
      return new ReadOnlyDictionary<int, GsaMaterial>(dict);
    }

    internal static ReadOnlyDictionary<int, GsaMaterial> CreateMaterialsFromAPI(
      ReadOnlyDictionary<int, GlassMaterial> materials, Model model) {
      var dict = new Dictionary<int, GsaMaterial>();
      foreach (KeyValuePair<int, GlassMaterial> mat in materials) {
        GsaMaterial gsaMaterial = GsaMaterialFactory.CreateMaterialFromApi(mat.Value, mat.Key, model);
        dict.Add(gsaMaterial.Id, gsaMaterial);
      }
      return new ReadOnlyDictionary<int, GsaMaterial>(dict);
    }

    internal static ReadOnlyDictionary<int, GsaMaterial> CreateMaterialsFromAPI(
      ReadOnlyDictionary<int, SteelMaterial> materials, Model model) {
      var dict = new Dictionary<int, GsaMaterial>();
      foreach (KeyValuePair<int, SteelMaterial> mat in materials) {
        GsaMaterial gsaMaterial = GsaMaterialFactory.CreateMaterialFromApi(mat.Value, mat.Key, model);
        dict.Add(gsaMaterial.Id, gsaMaterial);
      }
      return new ReadOnlyDictionary<int, GsaMaterial>(dict);
    }

    internal static ReadOnlyDictionary<int, GsaMaterial> CreateMaterialsFromAPI(
      ReadOnlyDictionary<int, TimberMaterial> materials, Model model) {
      var dict = new Dictionary<int, GsaMaterial>();
      foreach (KeyValuePair<int, TimberMaterial> mat in materials) {
        GsaMaterial gsaMaterial = GsaMaterialFactory.CreateMaterialFromApi(mat.Value, mat.Key, model);
        dict.Add(gsaMaterial.Id, gsaMaterial);
      }
      return new ReadOnlyDictionary<int, GsaMaterial>(dict);
    }

    internal static ReadOnlyDictionary<int, GsaMaterial> CreateMaterialsFromAPI(
      ReadOnlyDictionary<int, AnalysisMaterial> materials) {
      var dict = new Dictionary<int, GsaMaterial>();
      foreach (KeyValuePair<int, AnalysisMaterial> mat in materials) {
        GsaMaterial gsaMaterial = new GsaCustomMaterial(mat.Value, mat.Key);
        dict.Add(gsaMaterial.Id, gsaMaterial);
      }
      return new ReadOnlyDictionary<int, GsaMaterial>(dict);
    }

    internal bool SanitizeGenericCodeNames() {
      if (SanitizeGenericCodeNames(AluminiumMaterials)) {
        return true;
      }
      if (SanitizeGenericCodeNames(ConcreteMaterials)) {
        return true;
      }
      if (SanitizeGenericCodeNames(FabricMaterials)) {
        return true;
      }
      if (SanitizeGenericCodeNames(FrpMaterials)) {
        return true;
      }
      if (SanitizeGenericCodeNames(GlassMaterials)) {
        return true;
      }
      if (SanitizeGenericCodeNames(TimberMaterials)) {
        return true;
      }
      if (SanitizeGenericCodeNames(SteelMaterials)) {
        return true;
      }

      return false;
    }

    private bool SanitizeGenericCodeNames(ReadOnlyDictionary<int, GsaMaterial> materials) {
      foreach (GsaMaterial material in materials.Values) {
        if (material.ConcreteDesignCodeName == GenericConcreteCodeName) {
          return true;
        }

        if (material.SteelDesignCodeName == GenericSteelCodeName) {
          return true;
        }
      }

      return false;
    }
  }
}
