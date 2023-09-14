using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaAPI;
using GsaGH.Parameters;

namespace GsaGH.Helpers.Import {
  internal class Properties {
    internal ReadOnlyDictionary<int, GsaSectionGoo> Sections { get; private set; }
    internal ReadOnlyDictionary<int, GsaProperty2dGoo> Prop2ds { get; private set; }
    internal ReadOnlyDictionary<int, GsaProperty3dGoo> Prop3ds { get; private set; }

    internal Properties(Model model, Materials materials) {
      Sections = CreateSectionsFromAPI(model.Sections(), materials, model.SectionModifiers());
      Prop2ds = CreateProp2dsFromAPI(model.Prop2Ds(), materials, model.Axes());
      Prop3ds = CreateProp3dsFromAPI(model.Prop3Ds(), materials);
    }

    internal GsaSection GetSection(Element e) {
      return Sections.TryGetValue(e.Property, out GsaSectionGoo section) 
        ? section.Value
        : e.Property > 0 ? new GsaSection(e.Property) : null;
    }

    internal GsaSection GetSection(Member m) {
      return Sections.TryGetValue(m.Property, out GsaSectionGoo section) 
        ? section.Value 
        : m.Property > 0 ? new GsaSection(m.Property) : null;
    }

    internal GsaProperty2d GetProp2d(Element e) {
      return Prop2ds.TryGetValue(e.Property, out GsaProperty2dGoo prop) 
        ? prop.Value
        : e.Property > 0 ? new GsaProperty2d(e.Property) : null;
    }

    internal GsaProperty2d GetProp2d(Member m) {
      return Prop2ds.TryGetValue(m.Property, out GsaProperty2dGoo prop) 
        ? prop.Value
        : m.Property > 0 ? new GsaProperty2d(m.Property) : null;
    }

    internal GsaProperty3d GetProp3d(Element e) {
      return Prop3ds.TryGetValue(e.Property, out GsaProperty3dGoo prop) 
        ? prop.Value
        : e.Property > 0 ? new GsaProperty3d(e.Property) : null;
    }

    internal GsaProperty3d GetProp3d(Member m) {
      return Prop3ds.TryGetValue(m.Property, out GsaProperty3dGoo prop) 
        ? prop.Value
        : m.Property > 0 ? new GsaProperty3d(m.Property) : null;
    }

    private static ReadOnlyDictionary<int, GsaSectionGoo> CreateSectionsFromAPI(
      IReadOnlyDictionary<int, Section> sections,
      Materials materials,
      IReadOnlyDictionary<int, SectionModifier> sectionModifiers) {
      var dict = new Dictionary<int, GsaSectionGoo>();
      foreach (KeyValuePair<int, Section> sec in sections) {
        dict.Add(sec.Key, CreateSectionFromApi(sec, materials, sectionModifiers));
      }
      return new ReadOnlyDictionary<int, GsaSectionGoo>(dict);
    }

    private static ReadOnlyDictionary<int, GsaProperty2dGoo> CreateProp2dsFromAPI(
      IReadOnlyDictionary<int, Prop2D> props,
      Materials materials,
      IReadOnlyDictionary<int, Axis> axes = null) {
      var dict = new Dictionary<int, GsaProperty2dGoo>();
      foreach (KeyValuePair<int, Prop2D> prop in props) {
        dict.Add(prop.Key, CreateProp2dFromApi(prop, materials, axes));
      }
      return new ReadOnlyDictionary<int, GsaProperty2dGoo>(dict);
    }

    private static ReadOnlyDictionary<int, GsaProperty3dGoo> CreateProp3dsFromAPI(
      IReadOnlyDictionary<int, Prop3D> props,
      Materials materials) {
      var dict = new Dictionary<int, GsaProperty3dGoo>();
      foreach (KeyValuePair<int, Prop3D> prop in props) {
        dict.Add(prop.Key, CreateProp3dFromApi(prop, materials));
      }
      return new ReadOnlyDictionary<int, GsaProperty3dGoo>(dict);
    }

    private static GsaProperty2dGoo CreateProp2dFromApi(
        KeyValuePair<int, Prop2D> prop2d,
        Materials materials,
        IReadOnlyDictionary<int, Axis> axes) {
      var prop = new GsaProperty2d(prop2d);
      
      GsaMaterial material = materials.GetMaterial(prop2d.Value);
      if (material != null) {
        prop.Material = material;
      }

      // Axis property 0 = Global, -1 = Topological
      if (prop.ApiProp2d.AxisProperty > 0) {
        if (axes != null && axes.ContainsKey(prop.ApiProp2d.AxisProperty)) {
          Axis ax = axes[prop.ApiProp2d.AxisProperty];
          prop.SetPlaneFromAxis(ax);
        }
      }

      return new GsaProperty2dGoo(prop);
    }

    internal static GsaProperty3dGoo CreateProp3dFromApi(
      KeyValuePair<int, Prop3D> prop3d,
      Materials materials) {
      var prop = new GsaProperty3d(prop3d);

      GsaMaterial material = materials.GetMaterial(prop3d.Value);
      if (material!= null) {
        prop.Material = material;
      }

      return new GsaProperty3dGoo(prop);
    }

    private static GsaSectionGoo CreateSectionFromApi(
      KeyValuePair<int, Section> section,
      Materials materials,
      IReadOnlyDictionary<int, SectionModifier> sectionModifiers) {
      var sect = new GsaSection(section);

      GsaMaterial material = materials.GetMaterial(section.Value);
      if (material != null) {
        sect.Material = material;
      }

      if (sectionModifiers.Keys.Contains(section.Key)) {
        sect.Modifier = new GsaSectionModifier(sectionModifiers[section.Key]);
      }

      return new GsaSectionGoo(sect);
    }
  }
}
