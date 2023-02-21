using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using GsaAPI;
using GsaGH.Parameters;
using Rhino.Geometry;

namespace GsaGH.Helpers.Import
{
  /// <summary>
  /// Class containing functions to import various object types from GSA
  /// </summary>
  internal class Properties
  {
    /// <summary>
    /// Method to import Sections from a GSA model.
    /// Will output a list of GsaSectionsGoo.
    /// </summary>
    /// <param name="sDict">Dictionary of pre-filtered sections to import</param>
    /// <returns></returns>
    internal static List<GsaSectionGoo> GetSections(IReadOnlyDictionary<int, Section> sDict, ReadOnlyDictionary<int, AnalysisMaterial> analysisMaterials, IReadOnlyDictionary<int, SectionModifier> modDict)
    {
      List<GsaSectionGoo> sections = new List<GsaSectionGoo>();

      // Loop through all sections in Section dictionary and create new GsaSections
      foreach (int key in sDict.Keys)
      {
        if (sDict.TryGetValue(key, out Section apisection)) //1-base numbering
        {
          GsaSection sect = new GsaSection(key);
          sect.API_Section = apisection;
          if (sect.API_Section.MaterialAnalysisProperty != 0)
          {
            if (analysisMaterials.ContainsKey(sect.API_Section.MaterialAnalysisProperty))
              sect.Material.AnalysisMaterial = analysisMaterials[apisection.MaterialAnalysisProperty];
          }
          if (modDict.Keys.Contains(key))
            sect.Modifier = new GsaSectionModifier(modDict[key]);
          sections.Add(new GsaSectionGoo(sect));
        }
      }
      return sections;
    }
    /// <summary>
    /// Method to import Prop2ds from a GSA model.
    /// Will output a list of GsaProp2dGoo.
    /// </summary>
    /// <param name="pDict">Dictionary of pre-filtered 2D Properties to import</param>
    /// <returns></returns>
    internal static List<GsaProp2dGoo> GetProp2ds(IReadOnlyDictionary<int, Prop2D> pDict, ReadOnlyDictionary<int, AnalysisMaterial> analysisMaterials, ReadOnlyDictionary<int, Axis> axDict = null)
    {
      List<GsaProp2dGoo> prop2ds = new List<GsaProp2dGoo>();

      // Loop through all sections in Properties dictionary and create new GsaProp2d
      foreach (int key in pDict.Keys)
      {
        if (pDict.TryGetValue(key, out Prop2D apisection)) //1-base numbering
        {
          GsaProp2d prop = new GsaProp2d(key);
          prop.API_Prop2d = apisection;
          if (prop.API_Prop2d.MaterialAnalysisProperty != 0)
          {
            if (analysisMaterials.ContainsKey(prop.API_Prop2d.MaterialAnalysisProperty))
              prop.Material.AnalysisMaterial = analysisMaterials[apisection.MaterialAnalysisProperty];
          }

          // Axis property 0 = Global, -1 = Topological
          if(prop.API_Prop2d.AxisProperty > 0)
          {
            if (axDict != null && axDict.ContainsKey(prop.API_Prop2d.AxisProperty))
            {
              Axis ax = axDict[prop.API_Prop2d.AxisProperty];
              prop.LocalAxis = new Plane(new Point3d(ax.Origin.X, ax.Origin.Y, ax.Origin.Z),
                new Vector3d(ax.XVector.X, ax.XVector.Y, ax.XVector.Z),
                new Vector3d(ax.XYPlane.X, ax.XYPlane.Y, ax.XYPlane.Z)
                );
            }
          }

          prop2ds.Add(new GsaProp2dGoo(prop));
        }
      }
      return prop2ds;
    }
    internal static List<GsaProp3dGoo> GetProp3ds(IReadOnlyDictionary<int, Prop3D> pDict, ReadOnlyDictionary<int, AnalysisMaterial> analysisMaterials)
    {
      List<GsaProp3dGoo> prop2ds = new List<GsaProp3dGoo>();

      // Loop through all sections in Properties dictionary and create new GsaProp2d
      foreach (int key in pDict.Keys)
      {
        if (pDict.TryGetValue(key, out Prop3D apisection)) //1-base numbering
        {
          GsaProp3d prop = new GsaProp3d(key);
          prop.API_Prop3d = apisection;
          if (prop.API_Prop3d.MaterialAnalysisProperty != 0)
          {
            if (analysisMaterials.ContainsKey(prop.API_Prop3d.MaterialAnalysisProperty))
              prop.Material.AnalysisMaterial = analysisMaterials[apisection.MaterialAnalysisProperty];
          }
          prop2ds.Add(new GsaProp3dGoo(prop));
        }
      }
      return prop2ds;
    }
  }
}
