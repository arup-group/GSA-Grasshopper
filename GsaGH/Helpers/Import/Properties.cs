using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaAPI;
using GsaGH.Parameters;
using Rhino.Geometry;

namespace GsaGH.Helpers.Import {
  /// <summary>
  /// Class containing functions to import various object types from GSA
  /// </summary>
  internal class Properties {
    /// <summary>
    /// Method to import Sections from a GSA model.
    /// Will output a list of GsaSectionsGoo.
    /// </summary>
    /// <param name="sDict">Dictionary of pre-filtered sections to import</param>
    /// <param name="analysisMaterials"></param>
    /// <param name="modDict"></param>
    /// <returns></returns>
    internal static List<GsaSectionGoo> GetSections(IReadOnlyDictionary<int, Section> sDict, ReadOnlyDictionary<int, AnalysisMaterial> analysisMaterials, IReadOnlyDictionary<int, SectionModifier> modDict) {
      var sections = new List<GsaSectionGoo>();

      foreach (int key in sDict.Keys) {
        if (!sDict.TryGetValue(key, out Section apisection)) {
          continue;
        }

        var sect = new GsaSection(key) { ApiSection = apisection };
        if (sect.ApiSection.MaterialAnalysisProperty != 0) {
          if (analysisMaterials.ContainsKey(sect.ApiSection.MaterialAnalysisProperty))
            sect.Material.AnalysisMaterial = analysisMaterials[apisection.MaterialAnalysisProperty];
        }
        if (modDict.Keys.Contains(key))
          sect.Modifier = new GsaSectionModifier(modDict[key]);
        sections.Add(new GsaSectionGoo(sect));
      }
      return sections;
    }

    /// <summary>
    /// Method to import Prop2ds from a GSA model.
    /// Will output a list of GsaProp2dGoo.
    /// </summary>
    /// <param name="pDict">Dictionary of pre-filtered 2D Properties to import</param>
    /// <param name="analysisMaterials"></param>
    /// <param name="axDict"></param>
    /// <returns></returns>
    internal static List<GsaProp2dGoo> GetProp2ds(IReadOnlyDictionary<int, Prop2D> pDict, ReadOnlyDictionary<int, AnalysisMaterial> analysisMaterials, ReadOnlyDictionary<int, Axis> axDict = null) {
      var prop2ds = new List<GsaProp2dGoo>();

      foreach (int key in pDict.Keys) {
        if (!pDict.TryGetValue(key, out Prop2D apisection)) {
          continue;
        }

        var prop = new GsaProp2d(key) { ApiProp2d = apisection };
        if (prop.ApiProp2d.MaterialAnalysisProperty != 0) {
          if (analysisMaterials.ContainsKey(prop.ApiProp2d.MaterialAnalysisProperty))
            prop.Material.AnalysisMaterial = analysisMaterials[apisection.MaterialAnalysisProperty];
        }

        // Axis property 0 = Global, -1 = Topological
        if (prop.ApiProp2d.AxisProperty > 0) {
          if (axDict != null && axDict.ContainsKey(prop.ApiProp2d.AxisProperty)) {
            Axis ax = axDict[prop.ApiProp2d.AxisProperty];
            prop.LocalAxis = new Plane(new Point3d(ax.Origin.X, ax.Origin.Y, ax.Origin.Z),
              new Vector3d(ax.XVector.X, ax.XVector.Y, ax.XVector.Z),
              new Vector3d(ax.XYPlane.X, ax.XYPlane.Y, ax.XYPlane.Z)
            );
          }
        }

        prop2ds.Add(new GsaProp2dGoo(prop));
      }
      return prop2ds;
    }
    internal static List<GsaProp3dGoo> GetProp3ds(IReadOnlyDictionary<int, Prop3D> pDict, ReadOnlyDictionary<int, AnalysisMaterial> analysisMaterials) {
      var prop2ds = new List<GsaProp3dGoo>();

      foreach (int key in pDict.Keys) {
        if (!pDict.TryGetValue(key, out Prop3D apisection)) {
          continue;
        }

        var prop = new GsaProp3d(key);
        prop.ApiProp3d = apisection;
        if (prop.ApiProp3d.MaterialAnalysisProperty != 0) {
          if (analysisMaterials.ContainsKey(prop.ApiProp3d.MaterialAnalysisProperty))
            prop.Material.AnalysisMaterial = analysisMaterials[apisection.MaterialAnalysisProperty];
        }
        prop2ds.Add(new GsaProp3dGoo(prop));
      }
      return prop2ds;
    }
  }
}
