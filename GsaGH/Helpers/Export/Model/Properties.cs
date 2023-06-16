using GsaAPI;
using GsaAPI.Materials;
using GsaGH.Parameters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GsaGH.Helpers.Export {
  internal class Properties {
    internal static GsaGuidDictionary<Section> GetSectionDictionary(GsaModel model) {
      var sectDict = new GsaGuidDictionary<Section>(new Dictionary<int, Section>());
      foreach (KeyValuePair<int, GsaSectionGoo> section in model.Properties.Sections) {
        sectDict.SetValue(section.Key, section.Value.Value.Guid, section.Value.Value.ApiSection);
      }
      return sectDict;
    }

    internal static GsaGuidDictionary<Prop2D> GetProp2dDictionary(GsaModel model) {
      var propDict = new GsaGuidDictionary<Prop2D>(new Dictionary<int, Prop2D>());
      foreach (KeyValuePair<int, GsaProp2dGoo> prop in model.Properties.Prop2ds) {
        propDict.SetValue(prop.Key, prop.Value.Value.Guid, prop.Value.Value.ApiProp2d);
      }
      return propDict;
    }

    internal static GsaGuidDictionary<Prop3D> GetProp3dDictionary(GsaModel model) {
      var propDict = new GsaGuidDictionary<Prop3D>(new Dictionary<int, Prop3D>());
      foreach (KeyValuePair<int, GsaProp3dGoo> prop in model.Properties.Prop3ds) {
        propDict.SetValue(prop.Key, prop.Value.Value.Guid, prop.Value.Value.ApiProp3d);
      }
      return propDict;
    }

    internal static GsaGuidDictionary<AluminiumMaterial> GetAluminiumMaterialDictionary(GsaModel model) {
      var propDict = new GsaGuidDictionary<AluminiumMaterial>(new Dictionary<int, AluminiumMaterial>());
      foreach (KeyValuePair<int, GsaMaterial> mat in model.Materials.AluminiumMaterials) {
        propDict.SetValue(mat.Key, mat.Value.Guid, (AluminiumMaterial)mat.Value.StandardMaterial);
      }
      return propDict;
    }
    internal static GsaGuidDictionary<ConcreteMaterial> GetConcreteMaterialDictionary(GsaModel model) {
      var propDict = new GsaGuidDictionary<ConcreteMaterial>(new Dictionary<int, ConcreteMaterial>());
      foreach (KeyValuePair<int, GsaMaterial> mat in model.Materials.ConcreteMaterials) {
        propDict.SetValue(mat.Key, mat.Value.Guid, (ConcreteMaterial)mat.Value.StandardMaterial);
      }
      return propDict;
    }
    internal static GsaGuidDictionary<FabricMaterial> GetFabricMaterialDictionary(GsaModel model) {
      var propDict = new GsaGuidDictionary<FabricMaterial>(new Dictionary<int, FabricMaterial>());
      foreach (KeyValuePair<int, GsaMaterial> mat in model.Materials.FabricMaterials) {
        propDict.SetValue(mat.Key, mat.Value.Guid, (FabricMaterial)mat.Value.StandardMaterial);
      }
      return propDict;
    }
    internal static GsaGuidDictionary<FrpMaterial> GetFrpMaterialDictionary(GsaModel model) {
      var propDict = new GsaGuidDictionary<FrpMaterial>(new Dictionary<int, FrpMaterial>());
      foreach (KeyValuePair<int, GsaMaterial> mat in model.Materials.FrpMaterials) {
        propDict.SetValue(mat.Key, mat.Value.Guid, (FrpMaterial)mat.Value.StandardMaterial);
      }
      return propDict;
    }
    internal static GsaGuidDictionary<GlassMaterial> GetGlassMaterialDictionary(GsaModel model) {
      var propDict = new GsaGuidDictionary<GlassMaterial>(new Dictionary<int, GlassMaterial>());
      foreach (KeyValuePair<int, GsaMaterial> mat in model.Materials.GlassMaterials) {
        propDict.SetValue(mat.Key, mat.Value.Guid, (GlassMaterial)mat.Value.StandardMaterial);
      }
      return propDict;
    }
    internal static GsaGuidDictionary<SteelMaterial> GetSteelMaterialDictionary(GsaModel model) {
      var propDict = new GsaGuidDictionary<SteelMaterial>(new Dictionary<int, SteelMaterial>());
      foreach (KeyValuePair<int, GsaMaterial> mat in model.Materials.SteelMaterials) {
        propDict.SetValue(mat.Key, mat.Value.Guid, (SteelMaterial)mat.Value.StandardMaterial);
      }
      return propDict;
    }
    internal static GsaGuidDictionary<TimberMaterial> GetTimberMaterialDictionary(GsaModel model) {
      var propDict = new GsaGuidDictionary<TimberMaterial>(new Dictionary<int, TimberMaterial>());
      foreach (KeyValuePair<int, GsaMaterial> mat in model.Materials.TimberMaterials) {
        propDict.SetValue(mat.Key, mat.Value.Guid, (TimberMaterial)mat.Value.StandardMaterial);
      }
      return propDict;
    }
    internal static GsaGuidDictionary<AnalysisMaterial> GetAnalysisMaterialDictionary(GsaModel model) {
      var propDict = new GsaGuidDictionary<AnalysisMaterial>(new Dictionary<int, AnalysisMaterial>());
      foreach (KeyValuePair<int, GsaMaterial> mat in model.Materials.AnalysisMaterials) {
        propDict.SetValue(mat.Key, mat.Value.Guid, mat.Value.AnalysisMaterial);
      }
      return propDict;
    }
  }
}
