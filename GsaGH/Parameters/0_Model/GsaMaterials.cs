using System.Collections.ObjectModel;
using GsaAPI;

namespace GsaGH.Parameters {
  internal class GsaMaterials {
    internal ReadOnlyDictionary<int, GsaMaterial> SteelMaterials { get; private set; }
    internal ReadOnlyDictionary<int, GsaMaterial> ConcreteMaterials { get; private set; }
    internal ReadOnlyDictionary<int, GsaMaterial> FrpMaterials { get; private set; }
    internal ReadOnlyDictionary<int, GsaMaterial> AluminiumMaterials { get; private set; }
    internal ReadOnlyDictionary<int, GsaMaterial> TimberMaterials { get; private set; }
    internal ReadOnlyDictionary<int, GsaMaterial> GlassMaterials { get; private set; }
    internal ReadOnlyDictionary<int, GsaMaterial> FabricMaterials { get; private set; }
    internal ReadOnlyDictionary<int, GsaMaterial> AnalysisMaterials { get; private set; }

    internal GsaMaterials(Model model) {
      SteelMaterials = GsaMaterialFactory.CreateMaterialsFromApi(model.SteelMaterials(), model);
      ConcreteMaterials = GsaMaterialFactory.CreateMaterialsFromApi(model.ConcreteMaterials(), model);
      FrpMaterials = GsaMaterialFactory.CreateMaterialsFromApi(model.FrpMaterials(), model);
      AluminiumMaterials = GsaMaterialFactory.CreateMaterialsFromApi(model.AluminiumMaterials(), model);
      TimberMaterials = GsaMaterialFactory.CreateMaterialsFromApi(model.TimberMaterials(), model);
      GlassMaterials = GsaMaterialFactory.CreateMaterialsFromApi(model.GlassMaterials(), model);
      FabricMaterials = GsaMaterialFactory.CreateMaterialsFromApi(model.FabricMaterials(), model);
      AnalysisMaterials = GsaMaterialFactory.CreateMaterialsFromApi(model.AnalysisMaterials());
    }

    internal GsaMaterial GetMaterial(Section section) {
      return GetMaterial(section.MaterialType, section.MaterialAnalysisProperty, section.MaterialGradeProperty);
    }

    internal GsaMaterial GetMaterial(Prop2D property) {
      return GetMaterial(property.MaterialType, property.MaterialAnalysisProperty, property.MaterialGradeProperty);
    }

    internal GsaMaterial GetMaterial(Prop3D property) {
      return GetMaterial(property.MaterialType, property.MaterialAnalysisProperty, property.MaterialGradeProperty);
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
        if (material.ConcreteDesignCodeName == GsaModel.GenericConcreteCodeName) {
          return true;
        }

        if (material.SteelDesignCodeName == GsaModel.GenericSteelCodeName) {
          return true;
        }
      }

      return false;
    }
  }
}
