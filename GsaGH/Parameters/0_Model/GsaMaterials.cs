using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
    internal List<string> UnsupportedMaterials { get; private set; } = new List<string>();

    internal GsaMaterials(Model model) {
      SteelMaterials = GsaMaterialFactory.CreateMaterialsFromApi(model.SteelMaterials(), model);
      ConcreteMaterials = GsaMaterialFactory.CreateMaterialsFromApi(model.ConcreteMaterials(), model);
      FrpMaterials = GsaMaterialFactory.CreateMaterialsFromApi(model.FrpMaterials(), model);
      AluminiumMaterials = GsaMaterialFactory.CreateMaterialsFromApi(model.AluminiumMaterials(), model);
      TimberMaterials = GsaMaterialFactory.CreateMaterialsFromApi(model.TimberMaterials(), model);
      GlassMaterials = GsaMaterialFactory.CreateMaterialsFromApi(model.GlassMaterials(), model);
      FabricMaterials = GsaMaterialFactory.CreateMaterialsFromApi(model.FabricMaterials(), model);
      AnalysisMaterials = GsaMaterialFactory.CreateMaterialsFromApi(model.AnalysisMaterials());
      DetectUnsupportedAnalysisMaterials(model);
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
      if (analysisProp != 0) { // it is a custom material
        if (AnalysisMaterials.ContainsKey(analysisProp)) {
          return AnalysisMaterials[analysisProp];
        } else {
          return new GsaReferencedMaterial(analysisProp, MatType.Custom);
        }
      }

      return type switch {
        MaterialType.ALUMINIUM => AluminiumMaterials.TryGetValue(gradeProp, out GsaMaterial aluminium) ? aluminium
                    : new GsaReferencedMaterial(gradeProp, MatType.Aluminium),
        MaterialType.CONCRETE => ConcreteMaterials.TryGetValue(gradeProp, out GsaMaterial concrete) ? concrete
                    : new GsaReferencedMaterial(gradeProp, MatType.Concrete),
        MaterialType.FABRIC => FabricMaterials.TryGetValue(gradeProp, out GsaMaterial fabric) ? fabric
                    : new GsaReferencedMaterial(gradeProp, MatType.Fabric),
        MaterialType.FRP => FrpMaterials.TryGetValue(gradeProp, out GsaMaterial frp) ? frp
                    : new GsaReferencedMaterial(gradeProp, MatType.Frp),
        MaterialType.GLASS => GlassMaterials.TryGetValue(gradeProp, out GsaMaterial glass) ? glass
                    : new GsaReferencedMaterial(gradeProp, MatType.Glass),
        MaterialType.FIRST => SteelMaterials.TryGetValue(gradeProp, out GsaMaterial steel) ? steel
                    : new GsaReferencedMaterial(gradeProp, MatType.Steel),
        MaterialType.TIMBER => TimberMaterials.TryGetValue(gradeProp, out GsaMaterial timber) ? timber
                    : new GsaReferencedMaterial(gradeProp, MatType.Timber),
        _ => new GsaReferencedMaterial(gradeProp, GsaMaterialFactory.GetMatType(type)),
      };
    }

    private void DetectUnsupportedAnalysisMaterials(Model model) {
      var unsupportedIds = new HashSet<int>();

      foreach (KeyValuePair<int, Section> kvp in model.Sections()) {
        int analysisProp = kvp.Value.MaterialAnalysisProperty;
        if (analysisProp != 0 && !AnalysisMaterials.ContainsKey(analysisProp)) {
          unsupportedIds.Add(analysisProp);
        }
      }

      foreach (KeyValuePair<int, Prop2D> kvp in model.Prop2Ds()) {
        int analysisProp = kvp.Value.MaterialAnalysisProperty;
        if (analysisProp != 0 && !AnalysisMaterials.ContainsKey(analysisProp)) {
          unsupportedIds.Add(analysisProp);
        }
      }

      foreach (KeyValuePair<int, Prop3D> kvp in model.Prop3Ds()) {
        int analysisProp = kvp.Value.MaterialAnalysisProperty;
        if (analysisProp != 0 && !AnalysisMaterials.ContainsKey(analysisProp)) {
          unsupportedIds.Add(analysisProp);
        }
      }

      foreach (int id in unsupportedIds.OrderBy(x => x)) {
        UnsupportedMaterials.Add(
          $"Analysis Material (ID: {id}) was not imported. " +
          "Only elastic isotropic analysis materials are supported; " +
          "fabric and orthotropic materials cannot be imported.");
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
