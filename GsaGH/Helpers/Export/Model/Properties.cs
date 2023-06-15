using GsaAPI;
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
  }
}
