using System;
using System.Collections.Generic;

using GsaAPI;

using GsaGH.Parameters;

namespace GsaGH.Helpers.Assembly {
  internal partial class ModelAssembly {
    private GsaGuidDictionary<Section> _sections;
    private GsaIntDictionary<SectionModifier> _secionModifiers;
    private GsaGuidDictionary<Prop2D> _prop2ds;
    private GsaGuidDictionary<Prop3D> _prop3ds;
    private int PropertiesCount => _sections.Count + _prop2ds.Count + _prop3ds.Count;

    private static (GsaGuidDictionary<Section>, GsaIntDictionary<SectionModifier>)
      GetSectionDictionary(GsaModel model) {
      var sections = new GsaGuidDictionary<Section>(new Dictionary<int, Section>());
      var modifiers = new GsaIntDictionary<SectionModifier>(new Dictionary<int, SectionModifier>());
      foreach (KeyValuePair<int, GsaSectionGoo> section in model.Sections) {
        sections.SetValue(section.Key, section.Value.Value.Guid, section.Value.Value.ApiSection);
        if (section.Value.Value.Modifier != null && section.Value.Value.Modifier.IsModified) {
          modifiers.SetValue(section.Key, section.Value.Value.Modifier.ApiSectionModifier);
        }
      }

      return (sections, modifiers);
    }

    private static GsaGuidDictionary<Prop2D> GetProp2dDictionary(GsaModel model) {
      var properties = new GsaGuidDictionary<Prop2D>(new Dictionary<int, Prop2D>());
      foreach (KeyValuePair<int, GsaProperty2dGoo> prop in model.Prop2ds) {
        properties.SetValue(prop.Key, prop.Value.Value.Guid, prop.Value.Value.ApiProp2d);
      }
      return properties;
    }

    private static GsaGuidDictionary<Prop3D> GetProp3dDictionary(GsaModel model) {
      var properties = new GsaGuidDictionary<Prop3D>(new Dictionary<int, Prop3D>());
      foreach (KeyValuePair<int, GsaProperty3dGoo> prop in model.Prop3ds) {
        properties.SetValue(prop.Key, prop.Value.Value.Guid, prop.Value.Value.ApiProp3d);
      }
      return properties;
    }

    private string GetPropertyReferenceDefinition(Guid guid) {
      if (_sections.GuidDictionary.TryGetValue(guid, out int sectionId)) {
        return "PB" + sectionId;
      }

      if (_prop2ds.GuidDictionary.TryGetValue(guid, out int pro2dId)) {
        return "PA" + pro2dId;
      }

      if (_prop3ds.GuidDictionary.TryGetValue(guid, out int prop3dId)) {
        return "PV" + prop3dId;
      }

      return GetMaterialReferenceDefinition(guid);
    }
  }
}
