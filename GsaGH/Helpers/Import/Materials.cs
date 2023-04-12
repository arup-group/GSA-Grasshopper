using System.Collections.Generic;
using System.Collections.ObjectModel;
using GsaAPI;
using GsaGH.Parameters;

namespace GsaGH.Helpers.Import {
  internal class Materials {

    internal static List<GsaMaterialGoo> GetCustomMaterials(ReadOnlyDictionary<int, AnalysisMaterial> analysisMaterials) {
      var materials = new List<GsaMaterialGoo>();

      foreach (int key in analysisMaterials.Keys) {
        if (!analysisMaterials.TryGetValue(key, out AnalysisMaterial analysisMaterial)) {
          continue;
        }

        var material = new GsaMaterial {
          AnalysisProperty = key,
          AnalysisMaterial = analysisMaterial
        };

        materials.Add(new GsaMaterialGoo(material));
      }
      return materials;
    }
  }
}
