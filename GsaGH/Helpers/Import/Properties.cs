using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaAPI;
using GsaAPI.Materials;
using GsaGH.Helpers.Export;
using GsaGH.Parameters;
using Rhino.Geometry;

namespace GsaGH.Helpers.Import {
  internal class Properties {
    internal ReadOnlyDictionary<int, GsaSectionGoo> Sections { get; private set; }
    internal ReadOnlyDictionary<int, GsaProp2dGoo> Prop2ds { get; private set; }
    internal ReadOnlyDictionary<int, GsaProp3dGoo> Prop3ds { get; private set; }

    internal Properties(Model model, Materials materials) {
      Sections = CreateSectionsFromAPI(model.Sections(), materials, model.SectionModifiers());
      Prop2ds = CreateProp2dsFromAPI(model.Prop2Ds(), materials, model.Axes());
      Prop3ds = CreateProp3dsFromAPI(model.Prop3Ds(), materials);
    }

    internal static ReadOnlyDictionary<int, GsaSectionGoo> CreateSectionsFromAPI(
      IReadOnlyDictionary<int, Section> sections,
      Materials materials,
      IReadOnlyDictionary<int, SectionModifier> sectionModifiers) {
      var dict = new Dictionary<int, GsaSectionGoo>();
      foreach (KeyValuePair<int, Section> sec in sections) {
        dict.Add(sec.Key, CreateSectionFromApi(sec, materials, sectionModifiers));
      }
      return new ReadOnlyDictionary<int, GsaSectionGoo>(dict);
    }

    internal static ReadOnlyDictionary<int, GsaProp2dGoo> CreateProp2dsFromAPI(
      IReadOnlyDictionary<int, Prop2D> props,
      Materials materials,
      IReadOnlyDictionary<int, Axis> axes = null) {
      var dict = new Dictionary<int, GsaProp2dGoo>();
      foreach (KeyValuePair<int, Prop2D> prop in props) {
        dict.Add(prop.Key, CreateProp2dFromApi(prop, materials, axes));
      }
      return new ReadOnlyDictionary<int, GsaProp2dGoo>(dict);
    }

    internal static ReadOnlyDictionary<int, GsaProp3dGoo> CreateProp3dsFromAPI(
      IReadOnlyDictionary<int, Prop3D> props,
      Materials materials) {
      var dict = new Dictionary<int, GsaProp3dGoo>();
      foreach (KeyValuePair<int, Prop3D> prop in props) {
        dict.Add(prop.Key, CreateProp3dFromApi(prop, materials));
      }
      return new ReadOnlyDictionary<int, GsaProp3dGoo>(dict);
    }

    private static GsaProp2dGoo CreateProp2dFromApi(
        KeyValuePair<int, Prop2D> prop2d,
        Materials materials,
        IReadOnlyDictionary<int, Axis> axes) {
      var prop = new GsaProp2d(prop2d.Key) {
        ApiProp2d = prop2d.Value,
      };
      prop.Material = materials.GetMaterial(prop);

      // Axis property 0 = Global, -1 = Topological
      if (prop.ApiProp2d.AxisProperty > 0) {
        if (axes != null && axes.ContainsKey(prop.ApiProp2d.AxisProperty)) {
          Axis ax = axes[prop.ApiProp2d.AxisProperty];
          prop.LocalAxis = new Plane(new Point3d(ax.Origin.X, ax.Origin.Y, ax.Origin.Z),
            new Vector3d(ax.XVector.X, ax.XVector.Y, ax.XVector.Z),
            new Vector3d(ax.XYPlane.X, ax.XYPlane.Y, ax.XYPlane.Z));
        }
      }

      return new GsaProp2dGoo(prop);
    }

    internal static GsaProp3dGoo CreateProp3dFromApi(
      KeyValuePair<int, Prop3D> prop3d,
      Materials materials) {

      var prop = new GsaProp3d(prop3d.Key) {
        ApiProp3d = prop3d.Value,
      };
      prop.Material = materials.GetMaterial(prop);

      return new GsaProp3dGoo(prop);
    }

    private static GsaSectionGoo CreateSectionFromApi(
      KeyValuePair<int, Section> section,
      Materials materials,
      IReadOnlyDictionary<int, SectionModifier> sectionModifiers) {
      var sect = new GsaSection(section.Key) {
        ApiSection = section.Value,
      };
      sect.Material = materials.GetMaterial(sect);

      if (sectionModifiers.Keys.Contains(section.Key)) {
        sect.Modifier = new GsaSectionModifier(sectionModifiers[section.Key]);
      }

      return new GsaSectionGoo(sect);
    }

  }
}
