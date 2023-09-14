using GsaAPI;
using GsaGH.Parameters;
using System;
using System.Collections.Generic;

namespace GsaGH.Helpers.Export {
  internal class Properties {
    internal GsaGuidDictionary<Section> Sections;
    internal GsaIntDictionary<SectionModifier> SecionModifiers;
    internal GsaGuidDictionary<Prop2D> Prop2ds;
    internal GsaGuidDictionary<Prop3D> Prop3ds;
    internal Materials Materials;
    internal int Count => Sections.Count + Prop2ds.Count + Prop3ds.Count;
    internal Properties(GsaModel model) { 
      Materials = new Materials(model);
      (Sections, SecionModifiers) = GetSectionDictionary(model);
      Prop2ds = GetProp2dDictionary(model);
      Prop3ds = GetProp3dDictionary(model);
    }
    internal void Assemble(ref Model apiModel) {
      apiModel.SetSections(Sections.ReadOnlyDictionary);
      apiModel.SetSectionModifiers(SecionModifiers.ReadOnlyDictionary);
      apiModel.SetProp2Ds(Prop2ds.ReadOnlyDictionary);
      apiModel.SetProp3Ds(Prop3ds.ReadOnlyDictionary);
      Materials.Assemble(ref apiModel);
    }

    internal string GetReferenceDefinition(Guid guid) {
      if (Sections.GuidDictionary.TryGetValue(guid, out int steelId)) {
        return "PB" + steelId;
      }

      if (Prop2ds.GuidDictionary.TryGetValue(guid, out int concreteId)) {
        return "PA" + concreteId;
      }

      if (Prop3ds.GuidDictionary.TryGetValue(guid, out int frpId)) {
        return "PV" + frpId;
      }

      return Materials.GetReferenceDefinition(guid);
    }

    internal static (GsaGuidDictionary<Section>, GsaIntDictionary<SectionModifier>)
      GetSectionDictionary(GsaModel model) {
      var sections = new GsaGuidDictionary<Section>(new Dictionary<int, Section>());
      var modifiers = new GsaIntDictionary<SectionModifier>(new Dictionary<int, SectionModifier>());
      foreach (KeyValuePair<int, GsaSectionGoo> section in model.Properties.Sections) {
        sections.SetValue(section.Key, section.Value.Value.Guid, section.Value.Value.ApiSection);
        if (section.Value.Value.Modifier != null && section.Value.Value.Modifier.IsModified) {
          modifiers.SetValue(section.Key, section.Value.Value.Modifier.ApiSectionModifier);
        }
      }

      return (sections, modifiers);
    }

    internal static GsaGuidDictionary<Prop2D> GetProp2dDictionary(GsaModel model) {
      var properties = new GsaGuidDictionary<Prop2D>(new Dictionary<int, Prop2D>());
      foreach (KeyValuePair<int, GsaProperty2dGoo> prop in model.Properties.Prop2ds) {
        properties.SetValue(prop.Key, prop.Value.Value.Guid, prop.Value.Value.ApiProp2d);
      }
      return properties;
    }

    internal static GsaGuidDictionary<Prop3D> GetProp3dDictionary(GsaModel model) {
      var properties = new GsaGuidDictionary<Prop3D>(new Dictionary<int, Prop3D>());
      foreach (KeyValuePair<int, GsaProperty3dGoo> prop in model.Properties.Prop3ds) {
        properties.SetValue(prop.Key, prop.Value.Value.Guid, prop.Value.Value.ApiProp3d);
      }
      return properties;
    }
  }
}
