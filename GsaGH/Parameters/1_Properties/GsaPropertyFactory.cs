using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using GsaAPI;

namespace GsaGH.Parameters {
  public static class GsaPropertyFactory {
    internal static ReadOnlyDictionary<int, GsaSectionGoo> CreateSectionsFromApi(
      IReadOnlyDictionary<int, Section> sections, GsaMaterials materials,
      IReadOnlyDictionary<int, SectionModifier> sectionModifiers) {
      var dict = new Dictionary<int, GsaSectionGoo>();
      foreach (KeyValuePair<int, Section> sec in sections) {
        dict.Add(sec.Key, CreateSectionFromApi(sec, materials, sectionModifiers));
      }
      return new ReadOnlyDictionary<int, GsaSectionGoo>(dict);
    }

    internal static ReadOnlyDictionary<int, GsaProperty2dGoo> CreateProp2dsFromApi(
      IReadOnlyDictionary<int, Prop2D> props, GsaMaterials materials,
      IReadOnlyDictionary<int, Axis> axes = null) {
      var dict = new Dictionary<int, GsaProperty2dGoo>();
      foreach (KeyValuePair<int, Prop2D> prop in props) {
        dict.Add(prop.Key, CreateProp2dFromApi(prop, materials, axes));
      }
      return new ReadOnlyDictionary<int, GsaProperty2dGoo>(dict);
    }

    internal static ReadOnlyDictionary<int, GsaProperty3dGoo> CreateProp3dsFromApi(
      IReadOnlyDictionary<int, Prop3D> props, GsaMaterials materials) {
      var dict = new Dictionary<int, GsaProperty3dGoo>();
      foreach (KeyValuePair<int, Prop3D> prop in props) {
        dict.Add(prop.Key, CreateProp3dFromApi(prop, materials));
      }
      return new ReadOnlyDictionary<int, GsaProperty3dGoo>(dict);
    }

    private static GsaProperty2dGoo CreateProp2dFromApi(KeyValuePair<int, Prop2D> prop2d,
      GsaMaterials materials, IReadOnlyDictionary<int, Axis> axes) {
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

    internal static GsaProperty3dGoo CreateProp3dFromApi(KeyValuePair<int, Prop3D> prop3d,
      GsaMaterials materials) {
      var prop = new GsaProperty3d(prop3d);

      GsaMaterial material = materials.GetMaterial(prop3d.Value);
      if (material != null) {
        prop.Material = material;
      }

      return new GsaProperty3dGoo(prop);
    }

    private static GsaSectionGoo CreateSectionFromApi(KeyValuePair<int, Section> section,
      GsaMaterials materials, IReadOnlyDictionary<int, SectionModifier> sectionModifiers) {
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

    internal static ReadOnlyDictionary<int, GsaSpringPropertyGoo> CreateSpringPropsFromApi(ReadOnlyDictionary<int, SpringProperty> springProps) {
      var dict = new Dictionary<int, GsaSpringPropertyGoo>();
      foreach (KeyValuePair<int, SpringProperty> prop in springProps) {
        dict.Add(prop.Key, CreateSpringPropFromApi(prop));
      }
      return new ReadOnlyDictionary<int, GsaSpringPropertyGoo>(dict);
    }

    private static GsaSpringPropertyGoo CreateSpringPropFromApi(KeyValuePair<int, SpringProperty> springProp) {
      var prop = new GsaSpringProperty(springProp);

      return new GsaSpringPropertyGoo(prop);
    }
  }
}
