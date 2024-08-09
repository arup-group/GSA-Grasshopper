using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using GsaAPI;

using GsaGH.Parameters;

namespace GsaGH.Helpers.Import {
  internal class Elements {
    internal ConcurrentBag<GsaAssemblyGoo> Assemblies { get; private set; }
    internal ConcurrentBag<GsaElement1dGoo> Element1ds { get; private set; }
    internal ConcurrentBag<GsaElement2dGoo> Element2ds { get; private set; }
    internal ConcurrentBag<GsaElement3dGoo> Element3ds { get; private set; }

    internal Elements(GsaModel model, string elementList = "All") {
      var elem1dDict = new ConcurrentDictionary<int, GSAElement>();
      var elem2dDict = new ConcurrentDictionary<int, GSAElement>();
      var elem3dDict = new ConcurrentDictionary<int, GSAElement>();
      ReadOnlyDictionary<int, GsaAPI.Assembly> aDict = model.ApiModel.Assemblies();
      ReadOnlyDictionary<int, Element> eDict = model.ApiModel.Elements(elementList);
      ReadOnlyDictionary<int, LoadPanelElement> loadPanels = model.ApiModel.LoadPanelElements(elementList);
      Parallel.ForEach(loadPanels, item => elem2dDict.TryAdd(item.Key, new GSAElement(item.Value)));
      Parallel.ForEach(eDict, item => {
        int elemDimension = 1; // default assume 1D element
        ElementType type = item.Value.Type;
        switch (type) {
          case ElementType.TRI3:
          case ElementType.TRI6:
          case ElementType.QUAD4:
          case ElementType.QUAD8:
            elemDimension = 2;
            break;

          case ElementType.BRICK8:
          case ElementType.WEDGE6:
          case ElementType.PYRAMID5:
          case ElementType.TETRA4:
            elemDimension = 3;
            break;
        }

        switch (elemDimension) {
          case 1:
            elem1dDict.TryAdd(item.Key, new GSAElement(item.Value));

            break;

          case 2:
            elem2dDict.TryAdd(item.Key, new GSAElement(item.Value));
            break;

          case 3:
            elem3dDict.TryAdd(item.Key, new GSAElement(item.Value));
            break;
        }
      });

      var steps = new List<int> {
        0, 1, 2, 3
      };
      Parallel.ForEach(steps, i => {
        switch (i) {
          case 0:
            Element1ds = new ConcurrentBag<GsaElement1dGoo>();
            if (elem1dDict.Count > 0) {
              Element1ds = GsaElementFactory.CreateElement1dFromApi(elem1dDict, model);
            }
            break;

          case 1:
            Element2ds = new ConcurrentBag<GsaElement2dGoo>();
            if (elem2dDict.Count > 0) {
              Element2ds = GsaElementFactory.CreateElement2dFromApi(elem2dDict, model);
            }
            break;

          case 2:
            Element3ds = new ConcurrentBag<GsaElement3dGoo>();
            if (elem3dDict.Count > 0) {
              Element3ds = GsaElementFactory.CreateElement3dFromApi(elem3dDict, model);
            }
            break;

          case 3:
            Assemblies = new ConcurrentBag<GsaAssemblyGoo>();
            Parallel.ForEach(aDict, item => {
              var assembly = new GsaAssembly(item);
              Assemblies.Add(new GsaAssemblyGoo(assembly));
            });
            break;
        }
      });
    }
  }
}
